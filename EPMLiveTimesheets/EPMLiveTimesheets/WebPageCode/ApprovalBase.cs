using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Xml;
using EPMLiveCore;
using EPMLiveWebParts;
using Microsoft.SharePoint;

namespace TimeSheets
{
    public abstract class ApprovalBase : getgriditems
    {
        private const string Dot = ".";
        private const int Position = 75;

        protected void AddCells(
            XmlNode xmlNode,
            SPSite site,
            XmlNode ndListId,
            XmlNode ndItemId,
            int period,
            DataSet dsTSHours,
            ArrayList arrayList,
            string bgcolor,
            bool usecurrent,
            string strColumns,
            DataSet dsTimesheetTasks,
            DataSet dsTimesheetMeta,
            bool timeeditor,
            string[] strworktypes,
            bool timenotes,
            SqlConnection connection)
        {
            var rowId = xmlNode.Attributes["id"].Value;
            var curUser = string.Empty;
            var firstDot = rowId.IndexOf(Dot, Position);
            curUser = rowId.Substring(firstDot + 1, rowId.LastIndexOf(Dot) - firstDot - 1);

            using (var command = new SqlCommand("spTSgetTSHours", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@username", curUser);
                command.Parameters.AddWithValue("@siteguid", site.ID);
                command.Parameters.AddWithValue("@period_id", period);
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dsTSHours);
                }
            }

            var dsTotalHours = new DataSet();

            using (var command = new SqlCommand("spTSGetTotalHoursForItem", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@listuid", ndListId.InnerText);
                command.Parameters.AddWithValue("@siteguid", site.ID);
                command.Parameters.AddWithValue("@itemid", ndItemId.InnerText);
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dsTotalHours);
                }
            }

            AddColumn1(xmlNode, arrayList);

            var tsItemUid = Guid.Empty.ToString();

            using (var command =
                new SqlCommand(
                    "select ts_item_uid,submitted,approval_status from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id",
                    connection))
            {
                command.Parameters.AddWithValue("@listuid", ndListId.InnerText);
                command.Parameters.AddWithValue("@itemid", ndItemId.InnerText);
                command.Parameters.AddWithValue("@username", curUser);
                command.Parameters.AddWithValue("@period_id", period);

                var drItem = command.ExecuteReader();

                AddColumn2(xmlNode, arrayList);

                if (drItem.Read())
                {
                    tsItemUid = drItem.GetGuid(0).ToString();

                    AddCell1(drItem, xmlNode);
                }

                AddCell2(drItem, xmlNode);

                if (!usecurrent && drItem.GetBoolean(1))
                {
                    var arrCols = strColumns.Split(',');
                    var ndList = xmlNode.SelectNodes("cell");

                    for (var i = 0; i < ndList.Count; i++)
                    {
                        var cell = ndList[i].OuterXml;
                        var colid = arrCols[i].Replace("<![CDATA[", string.Empty).Replace("]]>", string.Empty);
                        if (colid == "Project")
                        {
                            var dataRows = dsTimesheetTasks.Tables[0].Select("ts_item_uid ='" + tsItemUid + "'");
                            if (dataRows.Length > 0)
                            {
                                ndList[i].InnerText = dataRows[0]["project"].ToString();
                            }
                        }
                        else if (colid != "Title" && colid != "List" && colid != "Site" && colid != string.Empty)
                        {
                            var drs = dsTimesheetMeta.Tables[0].Select("ts_item_uid='" + tsItemUid + "' and columnname='" + colid + "'");
                            var colval = string.Empty;
                            if (drs.Length > 0)
                            {
                                colval = drs[0]["columnvalue"].ToString();
                            }

                            var bIsIndicator = false;
                            try
                            {
                                var field = list.Fields.GetFieldByInternalName(colid);
                                if (field.Type == SPFieldType.Calculated && colval.ToLower().Contains(".gif"))
                                {
                                    bIsIndicator = true;
                                }
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine(ex);
                            }

                            if (bIsIndicator)
                            {
                                ndList[i].InnerText = "<img src=\"/_layouts/images/" + colval + "\">";
                            }
                            else
                            {
                                ndList[i].InnerText = colval;
                            }
                        }
                    }
                }
                drItem.Close();
            }
            double total = 0;
            XmlNode newCol;
            foreach (DateTime dt in arrayList)
            {
                if (timeeditor)
                {
                    newCol = CreateCell(dsTSHours, tsItemUid, strworktypes, dt, ref total);

                    if (timenotes)
                    {
                        var drs = dsTSHours.Tables[1].Select("ts_item_uid = '" + tsItemUid + "' and TS_ITEM_DATE=#" + dt.ToString("MM/dd/yyy") + "#");
                        if (drs.Length > 0)
                        {
                            newCol.InnerText += "|N|" + drs[0]["TS_ITEM_NOTES"];
                        }
                        else
                        {
                            newCol.InnerText += "|N|";
                        }
                    }

                    if (newCol.InnerText.Length > 1)
                    {
                        newCol.InnerText = newCol.InnerText.Substring(1);
                    }

                    xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);
                }
                else
                {
                    newCol = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
                    var attr = docXml.CreateAttribute("type");
                    attr.InnerText = "ro";
                    newCol.Attributes.Append(attr);
                    var drs = dsTSHours.Tables[0].Select("ts_item_uid = '" + tsItemUid + "' and TS_ITEM_DATE=#" + dt.ToString("MM/dd/yyyy") + "#");
                    if (drs.Length > 0)
                    {
                        newCol.InnerText = double.Parse(drs[0]["TS_ITEM_HOURS"].ToString()).ToString();
                        total += double.Parse(drs[0]["TS_ITEM_HOURS"].ToString());
                        xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);
                    }
                    else
                    {
                        newCol.InnerText = "0";
                        xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);
                    }
                }
            }

            newCol = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
            newCol.InnerText = total.ToString();
            var attrStyle1 = docXml.CreateAttribute("style");
            attrStyle1.Value = "background: #" + bgcolor + "; font-weight: bold";
            newCol.Attributes.Append(attrStyle1);
            xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);

            var ndWork = xmlNode.SelectSingleNode("userdata[@name='Work']");

            newCol = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
            if (dsTotalHours.Tables.Count > 0 && dsTotalHours.Tables[0].Rows.Count > 0)
            {
                newCol.InnerText = ndWork.InnerText + "|" + double.Parse(dsTotalHours.Tables[0].Rows[0][0].ToString());
            }
            else
            {
                newCol.InnerText = ndWork.InnerText + "|0";
            }

            attrStyle1 = docXml.CreateAttribute("style");
            attrStyle1.Value = "background: #" + bgcolor + "; font-weight: bold";
            newCol.Attributes.Append(attrStyle1);
            xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);
        }

        protected virtual void AddColumn1(XmlNode nd, ArrayList arr)
        {
            // empty default implementation
        }

        protected virtual void AddColumn2(XmlNode nd, ArrayList arr)
        {
            // empty default implementation
        }

        protected abstract void AddCell1(SqlDataReader drItem, XmlNode nd);

        protected virtual void AddCell2(SqlDataReader drItem, XmlNode nd)
        {
            // empty default implementation
        }

        protected abstract XmlNode CreateCell(DataSet dsTsHours, string tsItemUid, string[] strworktypes, DateTime dt, ref double total);

        protected void AddNodes(
            XmlNodeList ndCols, 
            SPSite site, 
            SqlConnection connection, 
            string innerXml, 
            string typeValue, 
            string widthValue, 
            int period,
            ArrayList arrayList, 
            ref string filterHead, 
            ref string worktypes, 
            ref bool timeeditor, 
            ref bool timenotes)
        {
            var newCol = docXml.CreateNode(XmlNodeType.Element, "column", docXml.NamespaceURI);
            newCol.InnerXml = "<![CDATA[#master_checkbox]]>";
            var attrType = docXml.CreateAttribute("type");
            attrType.Value = "ch";
            var attrWidth = docXml.CreateAttribute("width");
            attrWidth.Value = "25";
            var attrAlign = docXml.CreateAttribute("align");
            attrAlign.Value = "center";
            var attrColor = docXml.CreateAttribute("color");
            attrColor.Value = "#F0F0F0";

            newCol.Attributes.Append(attrType);
            newCol.Attributes.Append(attrWidth);
            newCol.Attributes.Append(attrAlign);
            newCol.Attributes.Append(attrColor);

            docXml.SelectSingleNode("//head").InsertBefore(newCol, ndCols[0]);

            newCol = docXml.CreateNode(XmlNodeType.Element, "column", docXml.NamespaceURI);
            newCol.InnerXml = innerXml;
            attrType = docXml.CreateAttribute("type");
            attrType.Value = typeValue;
            attrWidth = docXml.CreateAttribute("width");
            attrWidth.Value = widthValue;
            attrAlign = docXml.CreateAttribute("align");
            attrAlign.Value = "center";
            attrColor = docXml.CreateAttribute("color");
            attrColor.Value = "#F0F0F0";

            newCol.Attributes.Append(attrType);
            newCol.Attributes.Append(attrWidth);
            newCol.Attributes.Append(attrAlign);
            newCol.Attributes.Append(attrColor);

            docXml.SelectSingleNode("//head").InsertBefore(newCol, ndCols[0]);

            InsertNodes(ndCols);

            using (var command = new SqlCommand("select TSTYPE_ID from TSTYPE where site_uid=@siteid", connection))
            {
                command.Parameters.AddWithValue("@siteid", site.ID);
                var dataReader = command.ExecuteReader();
                var typesBuilder = new StringBuilder();
                typesBuilder.Append(worktypes);
                while (dataReader.Read())
                {
                    timeeditor = true;
                    typesBuilder.Append("|").Append(dataReader.GetInt32(0));
                }
                worktypes = typesBuilder.ToString();
                worktypes = worktypes != string.Empty ? worktypes.Substring(1) : "0";
                dataReader.Close();
            }

            var config = CoreFunctions.getConfigSetting(site.RootWeb, "EPMLiveTSAllowNotes");
            if (bool.TrueString.Equals(config, StringComparison.OrdinalIgnoreCase))
            {
                timenotes = true;
                timeeditor = true;
            }

            var dayDefs = CoreFunctions.getConfigSetting(site.RootWeb, "EPMLiveDaySettings").Split('|');

            using (
                var command = new SqlCommand("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid",
                    connection))
            {
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@period_id", period);
                command.Parameters.AddWithValue("@siteid", site.ID);
                var dataReader = command.ExecuteReader();

                if (dataReader.Read())
                {
                    var dtStart = dataReader.GetDateTime(0);
                    var dtEnd = dataReader.GetDateTime(1);
                    var ts = dtEnd - dtStart;
                    var colBase = docXml.SelectSingleNode("//head").SelectNodes("column").Count;
                    var colCount = 0;
                    for (var i = 0; i <= ts.Days; i++)
                    {
                        var showday = string.Empty;
                        try
                        {
                            showday = dayDefs[(int)dtStart.AddDays(i).DayOfWeek * 3];
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine(ex);
                        }

                        if (showday == "True")
                        {
                            filterHead += ",&nbsp;";
                            colCount++;
                            arrayList.Add(dtStart.AddDays(i));
                            newCol = docXml.CreateNode(XmlNodeType.Element, "column", docXml.NamespaceURI);
                            newCol.InnerXml = "<![CDATA[" + dtStart.AddDays(i).DayOfWeek.ToString().Substring(0, 3) + "<br>" + dtStart.AddDays(i).Day +
                                              "]]>";
                            attrType = docXml.CreateAttribute("type");
                            attrType.Value = "ro[=sum]";
                            attrWidth = docXml.CreateAttribute("width");
                            attrWidth.Value = "40";
                            attrAlign = docXml.CreateAttribute("align");
                            attrAlign.Value = "right";
                            var attrId1 = docXml.CreateAttribute("id");
                            attrId1.Value = "_TsDate_" + dtStart.AddDays(i).ToShortDateString().Replace("/", "_");

                            newCol.Attributes.Append(attrType);
                            newCol.Attributes.Append(attrWidth);
                            newCol.Attributes.Append(attrAlign);
                            newCol.Attributes.Append(attrId1);

                            docXml.SelectSingleNode("//head").AppendChild(newCol);
                        }
                    }
                    var cols = string.Empty;

                    var newCol1 = docXml.CreateNode(XmlNodeType.Element, "column", docXml.NamespaceURI);
                    newCol1.InnerText = "Total";
                    var attrType1 = docXml.CreateAttribute("type");
                    attrType1.Value = "ro[=sum]";
                    var attrWidth1 = docXml.CreateAttribute("width");
                    attrWidth1.Value = "50";
                    var attrAlign1 = docXml.CreateAttribute("align");
                    attrAlign1.Value = "right";

                    var attrId = docXml.CreateAttribute("id");
                    attrId.Value = "_TsTotal_";

                    newCol1.Attributes.Append(attrType1);
                    newCol1.Attributes.Append(attrWidth1);
                    newCol1.Attributes.Append(attrAlign1);
                    newCol1.Attributes.Append(attrId);

                    docXml.SelectSingleNode("//head").AppendChild(newCol1);
                }
                dataReader.Close();
            }
        }

        protected virtual void InsertNodes(XmlNodeList ndCols)
        {
            // empty default implementation
        }

        protected void AddCells(XmlNode xmlNode, string bgcolor, ArrayList arrayList)
        {
            var newCell = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
            newCell.InnerText = string.Empty;
            var attrType = docXml.CreateAttribute("type");
            attrType.Value = "ro";
            newCell.Attributes.Append(attrType);

            xmlNode.InsertBefore(newCell, xmlNode.SelectSingleNode("cell"));

            InsertCell1(xmlNode);

            newCell = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
            newCell.InnerText = string.Empty;
            attrType = docXml.CreateAttribute("type");
            attrType.Value = "ro";
            newCell.Attributes.Append(attrType);

            xmlNode.InsertBefore(newCell, xmlNode.SelectSingleNode("cell"));

            foreach (XmlNode ndCell in xmlNode.SelectNodes("cell"))
            {
                var attrStyle = docXml.CreateAttribute("style");
                attrStyle.Value = "background: #" + bgcolor + "; font-weight: bold;";
                ndCell.Attributes.Append(attrStyle);
            }

            foreach (DateTime dateTime in arrayList)
            {
                var newCol = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
                newCol.InnerText = string.Empty;
                var attrStyle = docXml.CreateAttribute("style");
                attrStyle.Value = "background: #" + bgcolor;
                newCol.Attributes.Append(attrStyle);

                xmlNode.InsertAfter(newCol, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);
            }

            var newCol2 = docXml.CreateNode(XmlNodeType.Element, "cell", docXml.NamespaceURI);
            newCol2.InnerText = string.Empty;
            var attrStyle2 = docXml.CreateAttribute("style");
            attrStyle2.Value = "background: #" + bgcolor + ";font-weight: bold;";
            newCol2.Attributes.Append(attrStyle2);

            xmlNode.InsertAfter(newCol2, xmlNode.SelectNodes("cell")[xmlNode.SelectNodes("cell").Count - 1]);

            InsertCell2(xmlNode);
        }

        protected virtual void InsertCell1(XmlNode nd)
        {
            // empty default implementation
        }

        protected virtual void InsertCell2(XmlNode nd)
        {
            // empty default implementation
        }
    }
}