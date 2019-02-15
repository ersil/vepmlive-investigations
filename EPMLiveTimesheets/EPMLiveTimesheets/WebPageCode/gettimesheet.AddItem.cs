using System;
using System.Collections;
using System.Data;
using System.Linq;
using EPMLiveCore.Helpers;
using EPMLiveWebParts.WebPageCode;
using Microsoft.SharePoint;
using DiagTrace = System.Diagnostics.Trace;

namespace TimeSheets
{
    public partial class gettimesheet
    {
        private void addItem(DataRow dr, SortedList arrGTemp, SPWeb curWeb)
        {
            Guard.ArgumentIsNotNull(curWeb, nameof(curWeb));
            Guard.ArgumentIsNotNull(arrGTemp, nameof(arrGTemp));
            Guard.ArgumentIsNotNull(dr, nameof(dr));

            dtTemp = new DataTable();
            dtTemp.Columns.Add("WebId");
            dtTemp.Columns.Add("ListId");
            dtTemp.Columns.Add(IdText);
            dtTemp.Columns.Add(TitleText);
            dtTemp.Columns.Add("SiteUrl");
            dtTemp.Columns.Add("SiteId");
            dtTemp.Columns.Add(ProjectText);
            dtTemp.Columns.Add("TSDisableItem");
            dtTemp.Columns.Add(ListText);

            dtTemp.Rows.Add(
                dr[WebUId].ToString(),
                dr[ListUId].ToString(),
                dr[ItemId].ToString(),
                dr[TitleText].ToString(),
                curWeb.Site.ServerRelativeUrl,
                curWeb.Site.ID,
                dr[ProjectText].ToString(),
                1,
                dr[ListText].ToString());

            var counter = 9;

            foreach (var drCol in dsTSMeta.Tables[0]
               .Select($"list_uid=\'{dr[ListUId]}\' and item_id={dr[ItemId]}")
               .Where(drCol => !dtTemp.Columns.Contains(drCol[ColumnNameText].ToString())))
            {
                dtTemp.Columns.Add(drCol[ColumnNameText].ToString());
                dtTemp.Rows[0][counter] = drCol[ColumnValueText].ToString();
                counter++;
            }

            var tempGroup = new string[1] { null };

            if (arrGroupFields != null)
            {
                foreach (var groupBy in arrGroupFields)
                {
                    var field = list.Fields.GetFieldByInternalName(groupBy);
                    var newGroup = string.Empty;

                    try
                    {
                        newGroup = dr[groupBy].ToString();
                        newGroup = formatField(newGroup, groupBy, dr, true);
                    }
                    catch (Exception exception)
                    {
                        DiagTrace.WriteLine(exception);
                    }

                    if (string.IsNullOrWhiteSpace(newGroup))
                    {
                        newGroup = $" No {field.Title}";
                    }

                    GroupHelper.ProcessTempGroups(arrGTemp, field, newGroup, ref tempGroup);
                }
            }

            arrItems.Add($"{dr[WebUId]}.{dr[ListUId]}.{dr[ItemId]}", tempGroup);
            queueAllItems.Enqueue(dtTemp.Rows[0]);
        }

        private void addItem(SPListItem li, SortedList arrGTemp)
        {
            Guard.ArgumentIsNotNull(arrGTemp, nameof(arrGTemp));
            Guard.ArgumentIsNotNull(li, nameof(li));

            var tempGroup = new string[1] { null };

            if (arrGroupFields != null)
            {
                foreach (var groupBy in arrGroupFields)
                {
                    var field = list.Fields.GetFieldByInternalName(groupBy);
                    var newGroup = getField(li, groupBy, true);

                    try
                    {
                        newGroup = formatField(newGroup, groupBy, field.Type == SPFieldType.Calculated, true, li);
                    }
                    catch (Exception exception)
                    {
                        DiagTrace.WriteLine(exception);
                    }

                    if (string.IsNullOrWhiteSpace(newGroup))
                    {
                        newGroup = $" No {field.Title}";
                    }

                    GroupHelper.ProcessTempGroups(arrGTemp, field, newGroup, ref tempGroup);
                }
            }

            arrItems.Add($"{li.ParentList.ParentWeb.ID}.{li.ParentList.ID}.{li.ID}", tempGroup);
            queueAllItems.Enqueue(li);
        }
    }
}