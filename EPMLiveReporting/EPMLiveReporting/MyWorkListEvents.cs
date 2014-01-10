﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.SharePoint;

namespace EPMLiveReportsAdmin
{
    public class MyWorkListEvents : SPItemEventReceiver
    {
        #region Fields (11)

        private const string TABLE_NAME = "LSTMyWork";
        private ArrayList _defaultColumns;
        private ArrayList _mandatoryHiddenFlds;
        private DataTable _listColumns;
        private Guid _listId;
        private SPListItem _listItem;
        private string _listName;
        private MyWorkReportData _myWorkReportData;
        private SPItemEventProperties _properties;
        private Guid _siteId;
        private string _siteName;
        private string _siteUrl;
        private Dictionary<string, object> currentValues;
        #endregion Fields

        #region Methods (9)

        // Public Methods (3) 

        /// <summary>
        /// Asynchronous After event that occurs after a new item has been added to its containing object.
        /// </summary>
        /// <param name="properties"></param>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                if (Initialize(true, properties))
                {
                    InsertItem();
                }

                _myWorkReportData.Dispose();
            }
            catch (Exception exception)
            {
                SPSecurity.RunWithElevatedPrivileges(
                    () => LogEvent(exception, 6001, "EPMLive My Work Reporting Item Added"));
            }
        }

        /// <summary>
        /// Synchronous Before event that occurs before an existing item is completely deleted.
        /// </summary>
        /// <param name="properties"></param>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            try
            {
                if (Initialize(true, properties))
                {
                    DeleteItem();

                    if (_myWorkReportData.ListReportsWork(TABLE_NAME))
                    {
                        _myWorkReportData.DeleteWork(_listId, _properties.ListItemId);
                    }
                }

                _myWorkReportData.Dispose();
            }
            catch (Exception exception)
            {
                SPSecurity.RunWithElevatedPrivileges(
                    () => LogEvent(exception, 6003, "EPMLive My Work Reporting Item Deleting"));
            }
        }

        /// <summary>
        /// Asynchronous After event that occurs after an existing item is changed, for example, when the user changes data in one or more fields.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties"/> object that represents properties of the event handler.</param>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                if (Initialize(true, properties))
                {
                    UpdateItem();
                }

                _myWorkReportData.Dispose();
            }
            catch (Exception exception)
            {
                SPSecurity.RunWithElevatedPrivileges(
                    () => LogEvent(exception, 6002, "EPMLive My Work Reporting Item Updated"));
            }
        }

        // Private Methods (6) 

        /// <summary>
        /// Deletes the item.
        /// </summary>
        private void DeleteItem()
        {
            _myWorkReportData.DeleteListItem(GetSql("DELETE"));
        }

        /// <summary>
        /// Gets the SQL.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <returns></returns>
        private string GetSql(string operation)
        {
            string sql = string.Empty;

            switch (operation)
            {
                case "INSERT":
                    sql = _myWorkReportData.InsertSQL(TABLE_NAME, _listName, _listColumns, _listItem, _defaultColumns, _mandatoryHiddenFlds);
                    break;
                case "DELETE":
                    sql = _myWorkReportData.DeleteSQL(TABLE_NAME, _listId, _listItem.ID);
                    break;
            }

            return sql;
        }

        /// <summary>
        /// Initializes the specified populate columns.
        /// </summary>
        /// <param name="populateColumns">if set to <c>true</c> [populate columns].</param>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        private bool Initialize(bool populateColumns, SPItemEventProperties properties)
        {
            try
            {
                _listName = properties.ListTitle;

                if (_listName.Equals("My Work")) return false;

                _siteId = properties.SiteId;
                _listId = properties.ListId;
                _listItem = properties.ListItem;
                _properties = properties;

                _myWorkReportData = new MyWorkReportData(_siteId);
                _siteName = _myWorkReportData.SiteName;
                _siteUrl = _myWorkReportData.SiteUrl;

                currentValues = GetItemFieldValueFromDB(properties.ListId.ToString(), properties.ListItemId.ToString());

                if (populateColumns)
                {
                    _defaultColumns = new ArrayList { "siteid", "webid", "listid", "itemid", "weburl" };
                    _mandatoryHiddenFlds = new ArrayList { "commenters", "commentersread", "commentcount", "workspaceurl" };
                    _listColumns = _myWorkReportData.GetListColumns("My Work");
                    _listColumns = _listColumns.DefaultView.ToTable(true,
                                                                    (from DataColumn dataColumn in _listColumns.Columns
                                                                     select dataColumn.ColumnName).ToArray());
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Inserts the item.
        /// </summary>
        private void InsertItem()
        {
            string sql = GetSql("INSERT");

            if (string.IsNullOrEmpty(sql)) return;

            foreach (var stmt in sql.Split(new[] { "!-x-x-x-x-x-!" }, StringSplitOptions.None).Where(stmt => !_myWorkReportData.InsertListItem(stmt)))
            {
                _myWorkReportData.LogStatus(_myWorkReportData.GetListId(_listName), _listName.Replace("'", string.Empty),
                                            "Url:" + _properties.RelativeWebUrl.Replace("'", string.Empty) +
                                            " Error: Add item was unsuccessful.", _myWorkReportData.GetError(), 2, 1);
            }

            if (_myWorkReportData.ListReportsWork(TABLE_NAME))
            {
                //Save list item "work" field values
                SaveWork();
            }
        }

        /// <summary>
        /// Logs the event.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="eventId">The event id.</param>
        /// <param name="eventSource">The event source.</param>
        private void LogEvent(Exception exception, int eventId, string eventSource)
        {
            if (!EventLog.SourceExists(eventSource)) EventLog.CreateEventSource(eventSource, "EPM Live");

            var eventLog = new EventLog("EPM Live", ".", eventSource) { MaximumKilobytes = 32768 };

            eventLog.WriteEntry(
                string.Format("Name: {0} Url: {1} ID: {2} : {3} {4}", _siteName, _siteUrl, _siteId, exception.Message,
                              exception.StackTrace), EventLogEntryType.Error, eventId);
        }

        /// <summary>
        /// Updates the item.
        /// </summary>
        private void UpdateItem()
        {
            DeleteItem();
            InsertItem();
        }

        private bool SaveWork()
        {
            bool blnWorkSaved = true;
            bool bHasChangedWork = false;
            bool bHasChangedAssignedTo = false;
            bool bHasChangedStartDate = false;
            bool bHasChangedDueDate = false;

            string sErrMsg = string.Empty;
            try
            {
                //List item "work" fields -- START 
                string sWork = string.Empty;
                string sAssignedTo = string.Empty;
                object StartDate = DBNull.Value;
                object DueDate = DBNull.Value;

                if (ItemHasValue(_listItem, "Work"))
                {
                    try
                    {
                        if (_properties.EventType == SPEventReceiverType.ItemAdded)
                        {
                            sWork = _listItem["Work"].ToString();
                            bHasChangedWork = true;
                        }
                        else if (_properties.EventType == SPEventReceiverType.ItemUpdated)
                        {
                            if (_properties.ListItem["Work"] != null &&
                                Math.Round(Convert.ToDouble(currentValues["Work"].ToString()), 2) != Math.Round(Convert.ToDouble(_properties.ListItem["Work"].ToString()), 2))
                            {
                                sWork = _listItem["Work"].ToString();
                                bHasChangedWork = true;
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    //blnWorkSaved = false;
                    sErrMsg = "Work";
                }

                if (ItemHasValue(_listItem, "AssignedTo"))
                {
                    try
                    {
                        if (_properties.EventType == SPEventReceiverType.ItemAdded)
                        {
                            sAssignedTo = ReportData.AddLookUpFieldValues(_listItem["AssignedTo"].ToString(), "id");
                            bHasChangedAssignedTo = true;
                        }
                        else if (_properties.EventType == SPEventReceiverType.ItemUpdated)
                        {
                            if (_properties.ListItem["AssignedTo"] != null)
                            {
                                List<int> lIdsBefore = new List<int>(currentValues["AssignedToIDs"].ToString().TrimEnd(',').Split(',').Select(int.Parse));
                                var lookupValAfter = new SPFieldLookupValueCollection(_properties.ListItem["AssignedTo"].ToString());
                                List<int> lIdsAfter = new List<int>();
                                foreach (SPFieldLookupValue v in lookupValAfter)
                                {
                                    lIdsAfter.Add(v.LookupId);
                                }

                                bool execute = false;
                                if (lIdsBefore.Count() != lIdsAfter.Count())
                                {
                                    execute = true;
                                }
                                else
                                {
                                    bool containsAll = true;

                                    foreach (var i in lIdsBefore)
                                    {
                                        if (!lIdsAfter.Contains(i))
                                        {
                                            containsAll = false;
                                            break;
                                        }
                                    }

                                    if (!containsAll)
                                    {
                                        execute = true;
                                    }
                                }

                                if (execute)
                                {
                                    sAssignedTo = ReportData.AddLookUpFieldValues(_listItem["AssignedTo"].ToString(), "id");
                                    bHasChangedAssignedTo = true;
                                }
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    //blnWorkSaved = false;
                    if (sErrMsg == string.Empty)
                    {
                        sErrMsg = "Assigned To";
                    }
                    else
                    {
                        sErrMsg = sErrMsg + "," + "Assigned To";
                    }
                }


                if (ItemHasValue(_listItem, "StartDate"))
                {
                    try
                    {
                        if (_properties.EventType == SPEventReceiverType.ItemAdded)
                        {
                            StartDate = _listItem["StartDate"];
                            bHasChangedStartDate = true;
                        }
                        else if (_properties.EventType == SPEventReceiverType.ItemUpdated)
                        {
                            if (_properties.ListItem["StartDate"] != null)
                            {
                                var dateBefore = Convert.ToDateTime(currentValues["StartDate"].ToString()).ToUniversalTime().Date;
                                var dateAfter = Convert.ToDateTime(_properties.ListItem["StartDate"].ToString()).ToUniversalTime().Date;

                                if (dateBefore != dateAfter)
                                {
                                    StartDate = _listItem["StartDate"];
                                    bHasChangedStartDate = true;
                                }
                            }
                        }
                    }
                    catch { }
                }
                else
                {
                    //blnWorkSaved = false;
                    if (sErrMsg == string.Empty)
                    {
                        sErrMsg = "Start Date";
                    }
                    else
                    {
                        sErrMsg = sErrMsg + "," + "Start Date (Start)";
                    }
                }


                if (ItemHasValue(_listItem, "DueDate"))
                {
                    try
                    {
                        if (_properties.EventType == SPEventReceiverType.ItemAdded)
                        {
                            DueDate = _listItem["DueDate"];
                            bHasChangedDueDate = true;
                        }
                        else if (_properties.EventType == SPEventReceiverType.ItemUpdated)
                        {
                            if (_properties.ListItem["DueDate"] != null)
                            {
                                var dueDateBefore = Convert.ToDateTime(currentValues["DueDate"].ToString()).ToUniversalTime().Date;
                                var dueDateAfter = Convert.ToDateTime(_properties.ListItem["DueDate"].ToString()).ToUniversalTime().Date;

                                if (dueDateBefore != dueDateAfter)
                                {
                                    DueDate = _listItem["DueDate"];
                                    bHasChangedDueDate = true;
                                }
                            }
                        }

                    }
                    catch { }
                }
                else
                {
                    //blnWorkSaved = false;
                    if (sErrMsg == string.Empty)
                    {
                        sErrMsg = "Due Date";
                    }
                    else
                    {
                        sErrMsg = sErrMsg + "," + "Due Date (Finish)";
                    }
                }

                Guid SiteID = _siteId;
                Guid ListID = _listItem.ParentList.ID;
                Guid ItemID = _listItem.UniqueId;
                // "work" fields -- END

                if (bHasChangedWork || bHasChangedAssignedTo || bHasChangedStartDate || bHasChangedDueDate)
                {
                    if (!_myWorkReportData.ProcessAssignments(sWork.Replace("'", ""), sAssignedTo, StartDate, DueDate, ListID, SiteID, _listItem.ID, _listItem.ParentList.Title)) // - CAT.NET false-positive: All single quotes are escaped/removed.
                    {
                        _myWorkReportData.LogStatus(string.Empty, string.Empty, "SaveWork() failed.", _myWorkReportData.GetError().Replace("'", ""), 2, 3, string.Empty); // - CAT.NET false-positive: All single quotes are escaped/removed.
                        blnWorkSaved = false;
                    }
                }
                // Missing required values
                // NOTE: we are assuming that values are missing because user did not want to submit work item
            }
            catch (Exception ex)
            {
                _myWorkReportData.LogStatus(string.Empty, string.Empty, "SaveWork() failed. Web: " + _listItem.ParentList.ParentWeb.Title + ". List: " + _listItem.ParentList.Title + ". Item: " + _listItem.Title + ".", ex.Message.Replace("'", ""), 2, 3, string.Empty); // - CAT.NET false-positive: All single quotes are escaped/removed.
                blnWorkSaved = false;
            }
            return blnWorkSaved;
        }

        private bool ItemHasValue(SPListItem i, string fldName)
        {
            string result = string.Empty;
            try
            {
                result = i[fldName].ToString(); 
            }
            catch { }
             
            return !string.IsNullOrEmpty(result);
        }

        private Dictionary<string, object> GetItemFieldValueFromDB(string listId, string itemId)
        {
            var res = new Dictionary<string, object>();
            var dt = _myWorkReportData.ExecuteSql("SELECT * FROM LSTMyWork WHERE [ListId] = '" + listId + "' AND [ItemId] = " + itemId + " AND [AssignedToID] != -99");
            
            var sAssignedToID = new object();
            var sWork = new object();
            var sStartDate = new object();
            var sDueDate = new object();

            try
            {
                res.Add("AssignedToIDs", dt.Rows[0]["AssignedToID"]);
                res.Add("Work", dt.Rows[0]["Work"]);
                res.Add("StartDate", dt.Rows[0]["StartDate"]);
                res.Add("DueDate", dt.Rows[0]["DueDate"]);
            }
            catch { }

            if (dt.Rows.Count > 1)
            {
                var sIds = "";
                foreach (DataRow r in dt.Rows)
                {
                    sIds += (r["AssignedToID"].ToString() + ",");
                }
                res["AssignedToIDs"] = sIds;

                double sum = 0;
                foreach (DataRow r in dt.Rows)
                {
                    double n = 0;
                    try
                    {
                        n = Convert.ToDouble(r["Work"].ToString());
                    }
                    catch { }
                    sum += n;
                }
                res["Work"] = sum.ToString();
            }

            return res;
        }

        #endregion Methods

    }
}