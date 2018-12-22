﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.API;
using EPMLiveCore.ReportingProxy;
using EPMLiveWebParts;
using Microsoft.SharePoint;
using TimeSheets.Log;
using TimeSheets.Models;
using TimeSheets.Properties;
using CoreReportHelper = EPMLiveCore.ReportHelper;
using EpmCoreFunctions = EPMLiveCore.CoreFunctions;
using EpmWorkReportData = EPMLiveCore.ReportHelper.MyWorkReportData;

namespace TimeSheets
{
    public class TimesheetAPI
    {
        private static int myworktableid = 6;
        private const string UnSubmitTimesheetTemplate = "<UnSubmitTimesheet Status=\"{0}\">{1}</UnSubmitTimesheet>";
        private const string GridIoResultTemplate = "<Grid><IO Result=\"{0}\" Message=\"{1}\"/></Grid>";
        private const string StopWatchResultTemplate = "<StopWatch Status=\"{0}\">{1}</StopWatch>";
        private const string GetOtherHoursResultTemplate = "<GetOtherHours Status=\"{0}\">{1}</GetOtherHours>";

        static TimesheetAPI()
        {
            RoleChecker = new SPRoleChecker();
        }

        public static ISPRoleChecker RoleChecker { get; set; }

        public static string GetTimesheetUpdates(string data, SPWeb oWeb)
        {
            try
            {
                var docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                var id = docTimesheet.FirstChild.Attributes["ID"].Value;
                var rows = new ArrayList(docTimesheet.FirstChild.Attributes["Rows"].Value.Split(','));

                var docRet = new XmlDocument();
                docRet.LoadXml("<Grid><IO/><Changes/></Grid>");

                var nodeData = docRet.FirstChild.SelectSingleNode("//Changes");

                var settings = new TimesheetSettings(oWeb);

                var arrLookups = new ArrayList();

                var lstMyWork = oWeb.Site.RootWeb.Lists.TryGetList("My Work");

                if (lstMyWork != null)
                {
                    foreach (SPField field in lstMyWork.Fields)
                    {
                        if (field.Type == SPFieldType.Lookup)
                        {
                            arrLookups.Add(field.InternalName + "Text");
                        }
                    }
                }

                SqlConnection connection = null;
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        connection = new SqlConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                        connection.Open();
                    });

                    var userId = string.Empty;
                    var periodString = string.Empty;
                    using (var command = new SqlCommand(
                        @"SELECT USER_ID, PERIOD_ID 
                        FROM dbo.TSTIMESHEET 
                        INNER JOIN dbo.TSUSER ON dbo.TSTIMESHEET.TSUSER_UID = dbo.TSUSER.TSUSERUID 
                        where TS_UID=@uid",
                        connection))
                    {
                        command.Parameters.AddWithValue("@uid", id);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userId = reader.GetInt32(0).ToString();
                                periodString = reader.GetInt32(1).ToString();
                            }
                        }
                    }

                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        return string.Format(GridIoResultTemplate, -1, "Could not determine user");
                    }
                    else
                    {
                        var user = GetUser(oWeb, userId);
                        if (user.ID.ToString() == userId)
                        {
                            var dataSetTimestamp = GetTSDataSet(connection, oWeb, user, periodString);
                            var canEdit = true;

                            bool submitted;
                            bool.TryParse(dataSetTimestamp.Tables[1].Rows[0]["SUBMITTED"].ToString(), out submitted);
                            if (submitted)
                            {
                                canEdit = false;
                            }

                            var arrPeriods = GetPeriodDaysArray(connection, settings, oWeb, periodString);

                            foreach (DataRow row in dataSetTimestamp.Tables[2].Rows)
                            {
                                if (!rows.Contains(row["TS_ITEM_UID"].ToString()))
                                {
                                    var node = CreateTSRow(
                                        ref docRet,
                                        dataSetTimestamp,
                                        row,
                                        arrLookups,
                                        arrPeriods,
                                        settings,
                                        canEdit,
                                        oWeb);

                                    var attr = docRet.CreateAttribute("Added");
                                    attr.Value = "1";
                                    node.Attributes.Append(attr);

                                    nodeData.AppendChild(node);
                                }
                            }
                        }
                        else
                        {
                            return string.Format(GridIoResultTemplate, -1, "User mismatch or access denied");
                        }
                    }

                    return docRet.OuterXml;
                }
                finally
                {
                    connection?.Dispose();
                }
            }
            catch (Exception ex)
            {
                var message = string.Format(GridIoResultTemplate, -1, ex.Message);
                Logger.WriteLog(Logger.Category.Unexpected, message, ex.ToString());
                return message;
            }
        }

        public static string GetOtherHours(string data, SPWeb oWeb)
        {
            if (oWeb == null)
            {
                throw new ArgumentNullException(nameof(oWeb));
            }

            try
            {
                var docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                var hours = 0D;
                using (var connection = GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                {
                    using (var command = new SqlCommand(
                        @"SELECT SUM(dbo.TSITEMHOURS.TS_ITEM_HOURS) AS Hours 
                        FROM dbo.TSITEMHOURS 
                        INNER JOIN dbo.TSITEM ON dbo.TSITEMHOURS.TS_ITEM_UID = dbo.TSITEM.TS_ITEM_UID 
                        where LIST_UID=@listid and ITEM_ID=@itemid",
                        connection))
                    {
                        command.Parameters.AddWithValue("@listid", docTimesheet.FirstChild.Attributes["List"].Value);
                        command.Parameters.AddWithValue("@itemid", docTimesheet.FirstChild.Attributes["ID"].Value);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read() && !reader.IsDBNull(0))
                            {
                                hours = reader.GetDouble(0);
                            }
                        }
                    }
                }
                return string.Format(GetOtherHoursResultTemplate, 0, hours);
            }
            catch (Exception exception)
            {
                var message = string.Format(GetOtherHoursResultTemplate, 1, exception.Message);
                Logger.WriteLog(Logger.Category.Unexpected, message, exception.ToString());
                return message;
            }
        }

        public static string SaveView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                EPMLiveCore.API.ViewManager Views = GetViews(oWeb);

                Views.AddViewByXmlNode(docTimesheet.FirstChild);

                oWeb.AllowUnsafeUpdates = true;

                EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSViews", Views.ToString());

                return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }

        }

        public static string DeleteView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                EPMLiveCore.API.ViewManager Views = GetViews(oWeb);

                Views.DeleteView(docTimesheet.FirstChild.Attributes["Name"].Value);

                oWeb.AllowUnsafeUpdates = true;

                EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSViews", Views.ToString());

                return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }
            finally
            {
                oWeb.AllowUnsafeUpdates = false;
            }
        }

        public static string RenameView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                EPMLiveCore.API.ViewManager Views = GetViews(oWeb);

                Views.RenameView(docTimesheet.FirstChild.Attributes["Name"].Value, docTimesheet.FirstChild.Attributes["NewName"].Value, docTimesheet.FirstChild.Attributes["Default"].Value);

                oWeb.AllowUnsafeUpdates = true;

                EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSViews", Views.ToString());

                return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }
            finally
            {
                oWeb.AllowUnsafeUpdates = false;
            }

        }

        public static string SaveWorkView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                if (docTimesheet.FirstChild.Attributes["NonWork"].Value == "true")
                {
                    EPMLiveCore.API.ViewManager Views = GetNonWorkViews(oWeb);

                    Views.AddViewByXmlNode(docTimesheet.FirstChild);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSNonWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }
                else
                {

                    EPMLiveCore.API.ViewManager Views = GetWorkViews(oWeb);

                    Views.AddViewByXmlNode(docTimesheet.FirstChild);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }
            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }
            finally
            {
                oWeb.AllowUnsafeUpdates = false;
            }
        }

        public static string DeleteWorkView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                if (docTimesheet.FirstChild.Attributes["NonWork"].Value == "true")
                {
                    EPMLiveCore.API.ViewManager Views = GetNonWorkViews(oWeb);

                    Views.DeleteView(docTimesheet.FirstChild.Attributes["Name"].Value);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSNonWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }
                else
                {
                    EPMLiveCore.API.ViewManager Views = GetWorkViews(oWeb);

                    Views.DeleteView(docTimesheet.FirstChild.Attributes["Name"].Value);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }

            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }
            finally
            {
                oWeb.AllowUnsafeUpdates = false;
            }
        }

        public static string RenameWorkView(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                if (docTimesheet.FirstChild.Attributes["NonWork"].Value == "true")
                {
                    EPMLiveCore.API.ViewManager Views = GetNonWorkViews(oWeb);

                    Views.RenameView(docTimesheet.FirstChild.Attributes["Name"].Value, docTimesheet.FirstChild.Attributes["NewName"].Value, docTimesheet.FirstChild.Attributes["Default"].Value);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSNonWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }
                else
                {
                    EPMLiveCore.API.ViewManager Views = GetWorkViews(oWeb);

                    Views.RenameView(docTimesheet.FirstChild.Attributes["Name"].Value, docTimesheet.FirstChild.Attributes["NewName"].Value, docTimesheet.FirstChild.Attributes["Default"].Value);

                    oWeb.AllowUnsafeUpdates = true;

                    EPMLiveCore.CoreFunctions.setConfigSetting(oWeb, "EPMLiveTSWorkViews", Views.ToString());

                    return "<Views Status=\"0\">" + Views.ToJSON() + "</Views>";
                }
            }
            catch (Exception ex)
            {
                return "<Views Status=\"1\">" + ex.Message + "</Views>";
            }
            finally
            {
                oWeb.AllowUnsafeUpdates = false;
            }
        }

        internal static EPMLiveCore.API.ViewManager GetNonWorkViews(SPWeb oWeb)
        {
            EPMLiveCore.API.ViewManager vws = new EPMLiveCore.API.ViewManager(oWeb, "EPMLiveTSNonWorkViews");

            if (vws.Views.Count > 0)
            {

            }
            else
            {
                Dictionary<string, string> sVals = new Dictionary<string, string>();
                sVals.Add("Default", "True");
                sVals.Add("Filters", "");
                sVals.Add("Group", "");
                sVals.Add("Sort", "Title");
                sVals.Add("Cols", "Title");
                sVals.Add("Expanded", "True");

                vws.Views.Add("Default View", sVals);
            }

            return vws;
        }

        internal static EPMLiveCore.API.ViewManager GetWorkViews(SPWeb oWeb)
        {
            EPMLiveCore.API.ViewManager vws = new EPMLiveCore.API.ViewManager(oWeb, "EPMLiveTSWorkViews");

            if (vws.Views.Count > 0)
            {

            }
            else
            {
                Dictionary<string, string> sVals = new Dictionary<string, string>();
                sVals.Add("Default", "True");
                sVals.Add("Filters", "");
                sVals.Add("Group", "Project,WorkTypeField");
                sVals.Add("Sort", "Title");
                sVals.Add("Cols", "Title,Work|80,PercentComplete|100");
                sVals.Add("Expanded", "True");

                vws.Views.Add("Default View", sVals);
            }

            return vws;

        }

        internal static EPMLiveCore.API.ViewManager GetViews(SPWeb oWeb)
        {
            EPMLiveCore.API.ViewManager vws = new EPMLiveCore.API.ViewManager(oWeb, "EPMLiveTSViews");

            if (vws.Views.Count > 0)
            {

            }
            else
            {
                Dictionary<string, string> sVals = new Dictionary<string, string>();
                sVals.Add("Default", "True");
                sVals.Add("Filters", "");
                sVals.Add("Group", "Project");
                sVals.Add("Sort", "Title");
                sVals.Add("Cols", "Title,Work|80,PercentComplete|100");
                sVals.Add("Expanded", "True");

                vws.Views.Add("Default View", sVals);
            }

            return vws;

        }

        public static string UnSubmitTimesheet(string data, SPWeb oWeb)
        {
            try
            {
                var docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                var tsuid = docTimesheet.FirstChild.Attributes["ID"].Value;

                var message = string.Empty;
                SqlConnection connection = null;
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        connection = new SqlConnection(
                            EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                        connection.Open();
                    });

                    var userid = ReadSelectCommand(connection, tsuid);

                    if (userid != 0)
                    {
                        var user = GetUser(oWeb, userid.ToString());

                        if (user.ID != userid)
                        {
                            message = string.Format(UnSubmitTimesheetTemplate, 3, "You do not have access to edit that timesheet.");
                        }
                        else
                        {
                            var settings = new TimesheetSettings(oWeb);
                            var bLocked = false;
                            var approval = 0;
                            using (var command = new SqlCommand(
                                "SELECT LOCKED,APPROVAL_STATUS FROM TSTIMESHEET where TS_UID=@tsuid",
                                connection))
                            {
                                command.Parameters.AddWithValue("@tsuid", tsuid);
                                using (var reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        bLocked = reader.GetBoolean(0);
                                        approval = reader.GetInt32(1);
                                    }
                                }
                            }

                            if (bLocked)
                            {
                                message = string.Format(UnSubmitTimesheetTemplate, 4, "That timesheet is locked.");
                            }
                            else if (approval == 1 && !settings.DisableApprovals)
                            {
                                message = string.Format(UnSubmitTimesheetTemplate, 3, "That timesheet has already been approved.");
                            }
                            else
                            {
                                using (var command = new SqlCommand(
                                    @"Update TSTIMESHEET set 
                                    submitted=0,APPROVAL_STATUS=0,APPROVAL_DATE=NULL,LASTMODIFIEDBYU=@uname,LASTMODIFIEDBYN=@name 
                                    where TS_UID=@tsuid",
                                    connection))
                                {
                                    command.Parameters.AddWithValue("@uname", oWeb.CurrentUser.LoginName);
                                    command.Parameters.AddWithValue("@name", oWeb.CurrentUser.Name);
                                    command.Parameters.AddWithValue("@tsuid", tsuid);
                                    command.ExecuteNonQuery();
                                }

                                message = string.Format(UnSubmitTimesheetTemplate, 0, string.Empty);
                            }
                        }
                    }
                    else
                    {
                        message = string.Format(UnSubmitTimesheetTemplate, 2, "Invalid user found for timesheet.");
                    }
                }
                finally
                {
                    connection?.Dispose();
                }

                return message;
            }
            catch (Exception ex)
            {

                var errorMessage = $"<UnSubmitTimesheet Status=\"1\">Error: {ex.Message}</UnSubmitTimesheet>";
                Logger.WriteLog(Logger.Category.Unexpected, errorMessage, ex.ToString());
                return errorMessage;
            }
        }

        public static string SubmitTimesheet(string data, SPWeb sharepointWeb)
        {
            try
            {
                var docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);
                var timesheetGuid = docTimesheet.FirstChild.Attributes["ID"].Value;
                var message = string.Empty;

                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(sharepointWeb.Site.WebApplication.Id)))
                {
                    var userid = ReadSelectCommand(connection, timesheetGuid);

                    if (userid != 0)
                    {
                        var user = GetUser(sharepointWeb, userid.ToString());
                        if (user.ID != userid)
                        {
                            message =
                                "<SubmitTimesheet Status=\"3\">" +
                                "You do not have access to edit that timesheet.</SubmitTimesheet>";
                        }
                        else
                        {
                            var transaction = connection.BeginTransaction();
                            try
                            {
                                using (var command = new SqlCommand(
                                    "Update TSTIMESHEET set submitted=1,LASTMODIFIEDBYU=@uname,LASTMODIFIEDBYN=@name, " +
                                    "LastSubmittedByName=@lastSubmittedByName, LastSubmittedByUser=@lastSubmittedByUser " +
                                    "where TS_UID=@tsuid",
                                    connection,
                                    transaction))
                                {
                                    command.Parameters.AddWithValue("@uname", sharepointWeb.CurrentUser.LoginName);
                                    command.Parameters.AddWithValue("@name", sharepointWeb.CurrentUser.Name);
                                    command.Parameters.AddWithValue("@tsuid", timesheetGuid);
                                    command.Parameters.AddWithValue("@lastSubmittedByName", sharepointWeb.CurrentUser.Name);
                                    command.Parameters.AddWithValue(
                                        "@lastSubmittedByUser",
                                        sharepointWeb.CurrentUser.LoginName);
                                    command.ExecuteNonQuery();
                                }

                                var settings = new TimesheetSettings(sharepointWeb);
                                if (settings.DisableApprovals)
                                {
                                    var resultDocument = new XmlDocument();
                                    resultDocument.LoadXml(AutoApproveTimesheets(
                                        string.Format(
                                            "<Approve ApproveStatus=\"1\"><TS id=\"{0}\"></TS></Approve>",
                                            timesheetGuid),
                                        sharepointWeb,
                                        transaction));

                                    if (resultDocument.FirstChild.Attributes["Status"].Value == "1")
                                    {
                                        throw new InvalidDataException(
                                            HttpUtility.HtmlDecode(
                                                resultDocument.FirstChild.SelectSingleNode("//Error").InnerText));
                                    }
                                }
                                transaction.Commit();
                            }
                            catch (Exception exception)
                            {
                                if (transaction != null)
                                {
                                    transaction.Rollback();
                                }
                                Logger.WriteLog(
                                    Logger.Category.Unexpected,
                                    "TimesheepAPI SubmitTimesheet",
                                    exception.ToString());
                                throw;
                            }
                            message = "<SubmitTimesheet Status=\"0\"></SubmitTimesheet>";
                        }
                    }
                    else
                    {
                        message = "<SubmitTimesheet Status=\"2\">Invalid user found for timesheet.</SubmitTimesheet>";
                    }

                    ProcessFullMeta(sharepointWeb.Site, connection, timesheetGuid);
                }

                return message;
            }
            catch (Exception ex)
            {
                return "<SubmitTimesheet Status=\"1\">Error: " + ex.Message + "</SubmitTimesheet>";
            }
        }

        private static int ReadSelectCommand(SqlConnection connection, string timesheetGuid)
        {
            var userid = 0;
            using (var command = new SqlCommand(
                "SELECT dbo.TSUSER.USER_ID "
                + "FROM dbo.TSUSER INNER JOIN dbo.TSTIMESHEET "
                + "    ON dbo.TSUSER.TSUSERUID = dbo.TSTIMESHEET.TSUSER_UID "
                + "WHERE TS_UID=@tsuid",
                connection))
            {
                command.Parameters.AddWithValue("@tsuid", timesheetGuid);
                using (var reader = command.ExecuteReader())
                {
                    try
                    {
                        if (reader.Read())
                        {
                            userid = reader.GetInt32(0);
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.WriteLog(Logger.Category.Unexpected, "Timesheet SubmitTimesheet", exception.ToString());
                    }
                }
            }
            return userid;
        }

        public static void ProcessFullMeta(SPSite site, SqlConnection cn, string ts_uid)
        {
            var cmd = new SqlCommand("select ts_item_uid,web_uid,list_uid,item_id,project from TSITEM where TS_UID=@ts_uid", cn);
            cmd.Parameters.AddWithValue("@ts_uid", ts_uid);
            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            SPList pList = null;
            SPWeb iWeb = null;
            SPList iList = null;
            Guid webGuid = Guid.Empty;
            Guid listGuid = Guid.Empty;

            if (ds.Tables.Count > 0)
            {
                try
                {
                    foreach (DataRow dataRow in ds.Tables[0].Rows)
                    {

                        Guid wGuid = new Guid(dataRow["WEB_UID"].ToString());
                        Guid lGuid = new Guid(dataRow["LIST_UID"].ToString());

                        if (webGuid != wGuid)
                        {
                            if (iWeb != null)
                            {
                                iWeb.Close();
                                iWeb = site.OpenWeb(wGuid);
                            }
                            else
                                iWeb = site.OpenWeb(wGuid);
                            webGuid = iWeb.ID;
                        }
                        if (listGuid != lGuid)
                        {
                            iList = iWeb.Lists[lGuid];

                            pList = SharedFunctions.getProjectCenterList(iList);

                            listGuid = iList.ID;
                        }

                        try
                        {
                            // This will throw error if task is deleted
                            SPListItem li = iList.GetItemById(int.Parse(dataRow["ITEM_ID"].ToString()));
                            SharedFunctions.processMeta(iWeb, iList, li, new Guid(dataRow["ts_item_uid"].ToString()), dataRow["project"].ToString(), cn, pList);
                        }
                        catch (ArgumentException ex)
                        {
                            //The item is deleted and not found in SPList
                            Logger.WriteLog(Logger.Category.Unexpected, "Timesheet submission", ex.ToString());
                        }

                    }
                }
                finally
                {
                    if (iWeb != null)
                        iWeb.Close();
                }
            }
        }

        // [EPMLCID-9648] Begin: Checking if resource is allocating time to a project he/she is not member of.
        public static void CheckNonTeamMemberAllocation(SPWeb oWeb, string tsuid, string cn, string data)
        {
            List<TimeSheetItem> timeSheetItems;
            double qtdAllocatedHours = 0;

            timeSheetItems = GetTimeSheetItems(data, cn);
            qtdAllocatedHours = CalculateAllocatedHours(timeSheetItems, cn);

            if (qtdAllocatedHours > 0)
                RunPermissionsChecks(timeSheetItems, qtdAllocatedHours, oWeb);

        }

        private const string PROJECT_WORK_FIELD_NAME = "Project Center";
        private const string TASK_WORK_FIELD_NAME = "Task Center";
        private const string PORTFLOIO_WORK_FIELD_NAME = "Project Portfolios";
        private const string PROGRAM_WORK_FIELD_NAME = "Project Programs";
        private static void RunPermissionsChecks(List<TimeSheetItem> timeSheetItems, double allocatedHours, SPWeb oWeb)
        {
            timeSheetItems.ForEach(timeSheetItem =>
            {
                if (timeSheetItem.Changed)
                {
                    switch (timeSheetItem.WorkTypeField)
                    {
                        case TASK_WORK_FIELD_NAME:
                            CheckTaskTimeAllocation(oWeb, timeSheetItem, allocatedHours);
                            break;
                        case PROGRAM_WORK_FIELD_NAME:
                            CheckProgramTimeAllocation(oWeb, timeSheetItem, allocatedHours);
                            break;
                        case PROJECT_WORK_FIELD_NAME:
                            CheckProjectTimeAllocation(oWeb, timeSheetItem, allocatedHours);
                            break;
                        case PORTFLOIO_WORK_FIELD_NAME:
                            CheckPortfloioTimeAllocation(oWeb, timeSheetItem, allocatedHours);
                            break;
                    }
                }

            });
        }
        private static void CheckPortfloioTimeAllocation(SPWeb oWeb, TimeSheetItem timeSheetItem, double allocatedHours)
        {
            var user = IsResourceTeamMember(oWeb, timeSheetItem, PORTFLOIO_WORK_FIELD_NAME);
            var emailToList = new List<string>();
            var idToList = new List<int>();
            var userList = new List<SPUser>();

            if (!user.Item2) // Item2 = isMember
            {
                var resourcesToNotify = GetResourceToNotifyPortfolios(timeSheetItem.ItemID, oWeb);

                if (!string.IsNullOrWhiteSpace(resourcesToNotify))
                {
                    resourcesToNotify.Split(',').ToList().ForEach(userGroupID =>
                    {
                        userList = GetUserFromField(userGroupID, oWeb);
                        userList.ForEach(userItem =>
                        {
                            emailToList.Add(string.IsNullOrWhiteSpace(userItem.Email) ? GetEmailFromDB(userItem.ID, oWeb) : userItem.Email);
                            idToList.Add(userItem.ID);
                        });
                    });
                }

                SendNotifications(oWeb, emailToList, idToList, allocatedHours, $"{ timeSheetItem.ItemTitle } portfolio",
                    "an outside", "is not currently assigned to the Portfolio Team", PORTFLOIO_WORK_FIELD_NAME);
            }
        }

        private static string GetResourceToNotifyPortfolios(int portfolioID, SPWeb oWeb)
        {
            var rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(oWeb.Site.ID);
            var epmLPortManagerColumn = EPMLiveCore.CoreFunctions.getConfigSetting(oWeb, "EPMPortManagerColumn");
            var sql = string.Format(@"SELECT [{0}] FROM dbo.LSTProjectPortfolios WHERE [id] = {1}",
                                    string.IsNullOrWhiteSpace(epmLPortManagerColumn) ? "OwnerID" : epmLPortManagerColumn, portfolioID);
            var dataTable = rptData?.ExecuteSql(sql);

            if (dataTable?.Rows?.Count > 0)
                return dataTable.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        private static void CheckProjectTimeAllocation(SPWeb oWeb, TimeSheetItem timeSheetItem, double allocatedHours)
        {
            var user = IsResourceTeamMember(oWeb, timeSheetItem, PROJECT_WORK_FIELD_NAME);
            var emailToList = new List<string>();
            var userTypesTosend = new List<string>() { "Owner", "Planners", "ProjectManagers" };
            var idToList = new List<int>();

            if (!user.Item2) // Item2 = isMember
            {
                userTypesTosend.ForEach(userType =>
                {
                    var usersToSendNotification = GetUsersToSendNotification(user.Item1, userType, oWeb);
                    emailToList.AddRange(usersToSendNotification.Item1);
                    idToList.AddRange(usersToSendNotification.Item2);
                });

                SendNotifications(oWeb, emailToList, idToList, allocatedHours, $"{ timeSheetItem.ItemTitle } project",
                    "an outside", "is not currently assigned to the Project Team", PROJECT_WORK_FIELD_NAME);
            }
        }

        private static void CheckTaskTimeAllocation(SPWeb oWeb, TimeSheetItem timeSheetItem, double allocatedHours)
        {
            SPUserToken systoken = oWeb.Site.SystemAccount.UserToken;
            SPListItem projectItem = null;
            using (SPSite site = new SPSite(oWeb.Site.ID, systoken))
            {
                using (SPWeb webSysAdmin = site.OpenWeb())
                {
                    //var task = IsResourceTeamMember(oWeb, timeSheetItem, TASK_WORK_FIELD_NAME);
                    projectItem = webSysAdmin.Lists[PROJECT_WORK_FIELD_NAME].Items.OfType<SPListItem>()
                .Where(x => x.Name == timeSheetItem.ProjectName).FirstOrDefault();

                    var project = IsResourceTeamMember(oWeb, new TimeSheetItem() { ItemID = projectItem.ID }, PROJECT_WORK_FIELD_NAME);

                    var userTypesTosend = new List<string>() { "Owner", "Planners", "ProjectManagers" };
                    var emailToList = new List<string>();
                    var idToList = new List<int>();

                    if (!project.Item2) // Item2 = isMember
                    {
                        userTypesTosend.ForEach(userType =>
                        {
                            var usersToSendNotification = GetUsersToSendNotification(projectItem, userType, webSysAdmin);
                            emailToList.AddRange(usersToSendNotification.Item1);
                            idToList.AddRange(usersToSendNotification.Item2);
                        });

                        SendNotifications(oWeb, emailToList, idToList, allocatedHours, $"{timeSheetItem.ProjectName} - {timeSheetItem.ItemTitle}",
                            "an outside", "is not currently assigned to the Project Team", PROJECT_WORK_FIELD_NAME);
                    }
                    else if (timeSheetItem.AssignedToID == null || !timeSheetItem.AssignedToID.Split(',').ToList().Contains(oWeb.CurrentUser.ID.ToString()))
                    {
                        userTypesTosend.ForEach(userType =>
                        {
                            var usersToSendNotification = GetUsersToSendNotification(projectItem, userType, webSysAdmin);
                            emailToList.AddRange(usersToSendNotification.Item1);
                            idToList.AddRange(usersToSendNotification.Item2);
                        });

                        SendNotifications(oWeb, emailToList, idToList, allocatedHours, $"{timeSheetItem.ProjectName} - {timeSheetItem.ItemTitle}",
                            "unassigned", "is currently assigned to the project team but has not been assigned to the task where time has been allocated",
                            PROJECT_WORK_FIELD_NAME);
                    }
                }
            }
        }

        private static Tuple<List<string>, List<int>> GetUsersToSendNotification(SPListItem listItem, string fieldName, SPWeb oWeb)
        {
            var emailList = new List<string>();
            var idList = new List<int>();
            int userIdInt;
            SPUser userAux;

            if (listItem.Fields.OfType<SPField>().Where(x => x.Title == fieldName).Any())
            {
                listItem[fieldName]?.ToString().Replace("#", "").Split(';').ToList()
                .Where((item, index) => index % 2 == 0)?.ToList().ForEach(userID =>
                {
                    if (int.TryParse(userID, out userIdInt) && (userAux = oWeb.SiteUsers.GetByID(userIdInt)) != null)
                    {
                        emailList.Add(string.IsNullOrWhiteSpace(userAux.Email) ? GetEmailFromDB(userIdInt, oWeb) : userAux.Email);
                        idList.Add(userIdInt);
                    }
                });
            }

            return new Tuple<List<string>, List<int>>(emailList, idList);
        }

        private static void CheckProgramTimeAllocation(SPWeb oWeb, TimeSheetItem timeSheetItem, double allocatedHours)
        {
            var user = IsResourceTeamMember(oWeb, timeSheetItem, PROGRAM_WORK_FIELD_NAME);
            var emailToList = new List<string>();
            var userTypesTosend = new List<string>() { "Owner" };
            var idToList = new List<int>();

            if (!user.Item2) // Item2 = isMember
            {
                userTypesTosend.ForEach(userType =>
                {
                    var usersToSendNotification = GetUsersToSendNotification(user.Item1, userType, oWeb);
                    emailToList.AddRange(usersToSendNotification.Item1);
                    idToList.AddRange(usersToSendNotification.Item2);
                });

                SendNotifications(oWeb, emailToList, idToList, allocatedHours, $"{ timeSheetItem.ItemTitle } programme",
                    "an outside", "is not currently assigned to the Programme Team", PROGRAM_WORK_FIELD_NAME);
            }
        }

        private static Tuple<SPListItem, bool> IsResourceTeamMember(SPWeb oWeb, TimeSheetItem timeSheetItem, string itemTypeName)
        {
            bool isMember = false;
            var userList = new List<SPUser>();
            var listItem = oWeb.Lists[itemTypeName]?.Items?.OfType<SPListItem>()
                .Where(x => x.ID == timeSheetItem.ItemID).FirstOrDefault();

            if (listItem != null)
            {
                if (itemTypeName != PORTFLOIO_WORK_FIELD_NAME)
                    isMember = listItem.DoesUserHavePermissions(SPBasePermissions.ViewListItems);

                if (!isMember && listItem.Fields.OfType<SPField>().Where(x => x.InternalName == "AssignedTo").Any())
                {
                    var assignedToField = listItem["AssignedTo"]?.ToString().Replace("#", "").Split(';').ToList();
                    assignedToField?.Where((item, index) => index % 2 == 0)?.ToList().ForEach(userGroupID =>
                    {
                        userList = GetUserFromField(userGroupID, oWeb);
                        userList.ForEach(user => isMember = isMember || user.ID == oWeb.CurrentUser.ID);
                    });
                }

                if (!isMember && listItem.Fields.OfType<SPField>().Where(x => x.Title == "Owner").Any())
                {
                    var ownersField = listItem["Owner"]?.ToString().Replace("#", "").Split(';').ToList();
                    ownersField?.Where((item, index) => index % 2 == 0)?.ToList().ForEach(userGroupID =>
                    {
                        userList = GetUserFromField(userGroupID, oWeb);
                        userList.ForEach(user => isMember = isMember || user.ID == oWeb.CurrentUser.ID);
                    });
                }
            }

            return new Tuple<SPListItem, bool>(listItem, isMember);
        }

        private static List<SPUser> GetUserFromField(string value, SPWeb oWeb)
        {
            if (string.IsNullOrWhiteSpace(value))
                return new List<SPUser>();

            var splitedIDs = value.Split(',').ToList();
            var returnList = new List<SPUser>();
            int id = -1;

            splitedIDs.ForEach(i =>
            {
                if (!string.IsNullOrWhiteSpace(i) && int.TryParse(i, out id))
                {
                    if (oWeb.SiteUsers.OfType<SPUser>().Any(x => x.ID == id))
                        returnList.Add(oWeb.SiteUsers.GetByID(id));
                    else if (oWeb.SiteGroups.OfType<SPGroup>().Any(x => x.ID == id))
                        oWeb.SiteGroups.GetByID(id)?.Users?.OfType<SPUser>()?.ToList()
                            .Where(x => x.Name != "System Account").ToList()
                            .ForEach(x => returnList.Add(x));
                }
            });

            return returnList;
        }

        private static List<TimeSheetItem> GetTimeSheetItems(string data, string connectionString)
        {
            XDocument doc = XDocument.Parse(data);

            var items = doc.Element("Timesheet")?.Elements("Item").ToList();

            var timeSheetItems = (from item in items
                                  select new TimeSheetItem()
                                  {
                                      ItemID = int.Parse(item.Attribute("ItemID")?.Value),
                                      ProjectName = item.Attribute("Project")?.Value,
                                      Uid = Guid.Parse(item.Attribute("UID")?.Value ?? Guid.Empty.ToString()),
                                      Hours = (from hour in item.Element("Hours")?.Elements("Date")
                                               select new TimeSheetHourItem()
                                               {
                                                   Date = DateTime.Parse(hour.Attribute("Value")?.Value),
                                                   Hour = Convert.ToDouble(hour.Element("Time")?.Attribute("Hours")?.Value)
                                               }).ToList(),
                                      Changed = false,
                                      AssignedToID = item.Attribute("AssignedToID")?.Value.ToString(),
                                      ItemTitle = item.Attribute("Title")?.Value.ToString(),
                                      WorkTypeField = item.Attribute("WorkTypeField")?.Value.ToString()
                                  }).ToList();

            return timeSheetItems;
        }

        private static double CalculateAllocatedHours(List<TimeSheetItem> timeSheetItems, string connectionString)
        {
            double result = 0;

            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                using (SqlConnection cn = new SqlConnection(connectionString))
                {
                    cn.Open();

                    timeSheetItems.ForEach(timeSheetItem =>
                    {
                        List<TimeSheetHourItem> dbHourItems = new List<TimeSheetHourItem>();
                        SqlCommand cmd = new SqlCommand("SELECT TS_ITEM_DATE,TS_ITEM_HOURS  FROM TSITEMHOURS where TS_ITEM_UID=@tsitemuid", cn);
                        cmd.Parameters.AddWithValue("@tsitemuid", timeSheetItem.Uid);
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            while (dr.Read())
                            {
                                dbHourItems.Add(new TimeSheetHourItem()
                                {
                                    Date = dr.GetDateTime(0),
                                    Hour = dr.GetDouble(1)
                                });
                            }
                        }

                        timeSheetItem.Hours.ForEach(hourItem =>
                        {
                            var dbItem = dbHourItems.FirstOrDefault(x => x.Date == hourItem.Date);
                            if (dbItem == null || dbItem.Hour != hourItem.Hour)
                            {
                                result += hourItem.Hour;
                                timeSheetItem.Changed = true;
                            }
                        });
                    });
                }
            });

            return Math.Round(result * 4, 0) / 4;
        }

        private const int NON_TEAM_MEMBER_ALLOCATION_EMAIL = 16;
        private const int NON_TEAM_MEMBER_ALLOCATION_GENERAL_NOTIFICATION = 17;
        public static void SendNotifications(SPWeb oWeb, List<string> emailToList, List<int> idToList, double allocatedHours,
            string itemName, string outOrUnassigned, string reasonMessage, string urlCenter)
        {
            emailToList = emailToList.Distinct().Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
            idToList = idToList.Distinct().Where(x => x != 0).ToList();

            if (idToList.Count > 0 && allocatedHours > 0)
            {
                var projecturl = string.Format("{0}?ID={1}", oWeb.Lists[CultureInfo.CurrentCulture.TextInfo.ToTitleCase(urlCenter.ToLower())].DefaultDisplayFormUrl, 1); // TODO remove the 1
                Hashtable hshProps = new Hashtable();
                hshProps.Add("Item_Name", itemName);
                hshProps.Add("Hours", allocatedHours);
                hshProps.Add("Project_Url", projecturl);
                hshProps.Add("CurUser_Email", oWeb.CurrentUser.Email);
                hshProps.Add("Out_Unassigned", outOrUnassigned);
                hshProps.Add("Reason_Message", reasonMessage);

                APIEmail.QueueItemMessage(NON_TEAM_MEMBER_ALLOCATION_GENERAL_NOTIFICATION, false, hshProps,
                    idToList.Select(x => x.ToString()).ToArray(), null, true, true, oWeb, oWeb.CurrentUser, true);
            }

            if (emailToList.Count > 0)
                APIEmail.sendEmail(NON_TEAM_MEMBER_ALLOCATION_EMAIL,
                    new Hashtable() { { "Item_Name", itemName },
                                      { "Resource_Email", GetEmailFromDB(oWeb.CurrentUser.ID, oWeb) },
                                      { "Qty_Hours", allocatedHours },
                                      { "Out_Unassigned", outOrUnassigned },
                                      { "Reason_Message", reasonMessage } },
                    emailToList, string.Empty, oWeb, true);
        }

        private static string GetEmailFromDB(int iD, SPWeb oWeb)
        {
            var rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(oWeb.Site.ID);
            var sql = string.Format(@"Select [Email] from [LSTResourcepool] WHERE [SharePointAccountID] = {0}", iD);
            var tblEmail = rptData.ExecuteSql(sql);

            if (tblEmail?.Rows?.Count > 0 && tblEmail.Rows[0][0] != null)
                return tblEmail.Rows[0][0].ToString();
            else
                return string.Empty;
        }

        public static string SaveTimesheet(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                string tsuid = docTimesheet.FirstChild.Attributes["TSUID"].Value;

                SqlConnection connection = null;
                try
                {
                    string connectionString = null;

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        connectionString = EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id);
                        connection = new SqlConnection(connectionString);
                        connection.Open();
                    });

                    var submitted = false;
                    using (var command = new SqlCommand("SELECT submitted FROM TSTIMESHEET where TS_UID=@tsuid ", connection))
                    {
                        command.Parameters.AddWithValue("@tsuid", tsuid);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                submitted = reader.GetBoolean(0);
                            }
                        }
                    }

                    if (!submitted)
                    {
                        int status = 3;

                        using (var cmd = new SqlCommand("SELECT status,jobtype_id FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31", connection))
                        {
                            cmd.Parameters.AddWithValue("@tsuid", tsuid);
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    status = reader.GetInt32(0);
                                }
                            }
                        }

                        if (status == 3)
                        {
                            // [EPMLCID-9648] Begin: Checking if resource is allocating time to a project he/she is not member of
                            if (bool.Parse(EpmCoreFunctions.getConfigSetting(oWeb, "EPMLiveEnableNonTeamNotf")))
                            {
                                using (var command = new SqlCommand(
                                 "DELETE FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=33",
                                 connection))
                                {
                                    command.Parameters.AddWithValue("@tsuid", tsuid);
                                    command.ExecuteNonQuery();
                                }

                                using (var command = new SqlCommand(
                                   @"INSERT INTO TSQUEUE (TS_UID,STATUS,JOBTYPE_ID,USERID,JOBDATA) 
                              VALUES(@tsuid,0,33,@USERID,@JOBDATA)",
                                   connection))
                                {
                                    command.Parameters.AddWithValue("@tsuid", tsuid);
                                    command.Parameters.AddWithValue("@USERID", oWeb.CurrentUser.ID);
                                    command.Parameters.AddWithValue("@JOBDATA", data);
                                    command.ExecuteNonQuery();
                                }
                            }

                            using (var command = new SqlCommand(
                                "DELETE FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31",
                                connection))
                            {
                                command.Parameters.AddWithValue("@tsuid", tsuid);
                                command.ExecuteNonQuery();
                            }

                            using (var command = new SqlCommand(
                                @"INSERT INTO TSQUEUE (TS_UID,STATUS,JOBTYPE_ID,USERID,JOBDATA) 
                              VALUES(@tsuid,0,31,@USERID,@JOBDATA)",
                                connection))
                            {
                                command.Parameters.AddWithValue("@tsuid", tsuid);
                                command.Parameters.AddWithValue("@USERID", oWeb.CurrentUser.ID);
                                command.Parameters.AddWithValue("@JOBDATA", data);
                                command.ExecuteNonQuery();
                            }
                            return "<SaveTimesheet Status=\"0\">Save Queued</SaveTimesheet>";
                        }
                        else
                        {
                            return "<SaveTimesheet Status=\"2\">Timesheet is already being processed.</SaveTimesheet>";
                        }
                    }
                    else
                    {
                        return "<SaveTimesheet Status=\"3\">Timesheet is submitted and cannot save.</SaveTimesheet>";
                    }
                }
                finally
                {
                    connection?.Dispose();
                }
            }
            catch (Exception ex)
            {
                return "<SaveTimesheet Status=\"1\">Error: " + ex.Message + "</SaveTimesheet>";
            }
        }

        public static string StartStopWatch(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                var tsuid = doc.FirstChild.Attributes["ID"].Value;
                var userid = string.Empty;
                try
                {
                    userid = doc.FirstChild.Attributes["UserId"].Value;
                }
                catch
                {
                    Logger.WriteLog(
                        Logger.Category.Unexpected,
                        "TimeSheetAPI Approve TimeSheet",
                        $"No UserId in {nameof(data)} document.");
                }

                var isError = false;
                var message = string.Empty;
                SqlConnection connection = null;
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        connection = new SqlConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                        connection.Open();
                    });

                    var user = GetUser(oWeb, userid);

                    using (var command = new SqlCommand(
                        @"SELECT * FROM dbo.TSITEM 
                         INNER JOIN dbo.TSTIMESHEET ON dbo.TSITEM.TS_UID = dbo.TSTIMESHEET.TS_UID 
                         INNER JOIN dbo.TSSW ON dbo.TSITEM.TS_ITEM_UID = dbo.TSSW.TSITEMUID 
                         where USER_ID=@userid and site_uid=@siteid",
                        connection))
                    {
                        command.Parameters.AddWithValue("@userid", user.ID);
                        command.Parameters.AddWithValue("@siteid", oWeb.Site.ID);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                isError = true;
                                message = "Timer already started on another item.";
                            }
                        }
                    }

                    if (!isError)
                    {
                        var now = DateTime.Now;
                        using (var command = new SqlCommand(
                            @"INSERT INTO TSSW (TSITEMUID, STARTED, USER_ID) VALUES (@tsitemuid, @dt, @userid)",
                            connection))
                        {
                            command.Parameters.AddWithValue("@tsitemuid", tsuid);
                            command.Parameters.AddWithValue("@dt", now);
                            command.Parameters.AddWithValue("@userid", user.ID);
                            command.ExecuteNonQuery();
                        }
                        message = now.ToString("F");
                    }
                }
                finally
                {
                    connection?.Dispose();
                }

                if (isError)
                {
                    return string.Format(StopWatchResultTemplate, 1, $"Error: {message}");
                }
                else
                {
                    return string.Format(StopWatchResultTemplate, 0, message);
                }
            }
            catch (Exception exception)
            {
                var errorMessage = string.Format(StopWatchResultTemplate, 1, $"Error: {exception.Message}");
                Logger.WriteLog(
                        Logger.Category.Unexpected,
                        "TimeSheetAPI Approve TimeSheet",
                        errorMessage);
                return errorMessage;
            }
        }

        public static string CheckSaveStatus(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                string tsuid = doc.FirstChild.Attributes["ID"].Value;

                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                {
                    using (var command = new SqlCommand(
                        "SELECT STATUS,PERCENTCOMPLETE,RESULT,RESULTTEXT FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31",
                        connection))
                    {
                        command.Parameters.AddWithValue("@tsuid", tsuid);

                        var status = -1;
                        var percentComplete = 0;
                        var result = string.Empty;
                        var resultText = string.Empty;

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                status = reader.GetInt32(0);
                                if (!reader.IsDBNull(1))
                                {
                                    percentComplete = reader.GetInt32(1);
                                }
                                if (!reader.IsDBNull(2))
                                {
                                    result = reader.GetString(2);
                                }
                                if (!reader.IsDBNull(3))
                                {
                                    resultText = reader.GetString(3);
                                }
                            }
                        }

                        return string.Format(
                            "<SaveStatus Result=\"0\" Status=\"{0}\" PercentComplete=\"{1}\" " +
                            "ErrorResult=\"{2}\" ResultText=\"{3}\"></SaveStatus>",
                            status,
                            percentComplete,
                            result,
                            resultText);
                    }
                }
            }
            catch (Exception ex)
            {
                return "<SaveStatus Result=\"1\">Error: " + ex.Message + "</SaveStatus>";
            }
        }

        public static string CheckApproveStatus(string data, SPWeb oWeb)
        {
            if (oWeb == null)
            {
                throw new ArgumentNullException(nameof(oWeb));
            }

            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(data);

                var timeSheetUid = doc.FirstChild.Attributes["ID"].Value;

                var status = -1;
                var percentComplete = 0;
                var result = string.Empty;
                var resultText = string.Empty;
                var approvalStatus = 0;

                using (var connection = GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                {
                    using (var command = new SqlCommand(
                        "SELECT STATUS,PERCENTCOMPLETE,RESULT,RESULTTEXT FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30",
                        connection))
                    {
                        command.Parameters.AddWithValue("@tsuid", timeSheetUid);

                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                status = reader.GetInt32(0);
                                if (!reader.IsDBNull(1))
                                {
                                    percentComplete = reader.GetInt32(1);
                                }
                                if (!reader.IsDBNull(2))
                                {
                                    result = reader.GetString(2);
                                }
                                if (!reader.IsDBNull(3))
                                {
                                    resultText = reader.GetString(3);
                                }
                            }
                        }
                    }

                    using (var command = new SqlCommand(
                        "SELECT APPROVAL_STATUS FROM TSTIMESHEET where TS_UID=@tsuid",
                        connection))
                    {
                        command.Parameters.AddWithValue("@tsuid", timeSheetUid);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                approvalStatus = reader.GetInt32(0);
                            }
                            reader.Close();
                        }
                    }
                }

                var message = string.Format(
                    "<ApproveStatus Result=\"0\" Status=\"{0}\" PercentComplete=\"{1}\" ErrorResult=\"{2}\" " +
                    "ResultText=\"{3}\" ApprovalStatus=\"{4}\"></ApproveStatus>",
                    status,
                    percentComplete,
                    result,
                    resultText,
                    approvalStatus);
                return message;
            }
            catch (Exception exception)
            {
                return string.Format(
                    "<ApproveStatus Result=\"1\">Error: {0}</ApproveStatus>",
                    exception.Message);
            }
        }

        public static string StopStopWatch(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                string tsuid = doc.FirstChild.Attributes["ID"].Value;
                string userid = "";
                try
                {
                    userid = doc.FirstChild.Attributes["UserId"].Value;
                }
                catch { }

                var dtStart = DateTime.MinValue;
                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                {
                    var user = GetUser(oWeb, userid);

                    using (var command =
                        new SqlCommand("Select STARTED FROM TSSW where TSITEMUID = @id and USER_ID=@userid", connection))
                    {
                        command.Parameters.AddWithValue("@id", tsuid);
                        command.Parameters.AddWithValue("@userid", user.ID);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dtStart = reader.GetDateTime(0);
                            }
                        }
                    }

                    using (var cmd =
                        new SqlCommand("DELETE FROM TSSW where TSITEMUID = @id and USER_ID=@userid", connection))
                    {
                        cmd.Parameters.AddWithValue("@id", tsuid);
                        cmd.Parameters.AddWithValue("@userid", user.ID);
                        cmd.ExecuteNonQuery();
                    }
                }

                if (dtStart != DateTime.MinValue)
                {
                    TimesheetSettings settings = new TimesheetSettings(oWeb);
                    string[] dayDefs = settings.DayDef.Split('|');

                    DateTime dtNow = DateTime.Now;

                    DateTime dtNowRounded = new DateTime(dtNow.Year, dtNow.Month, dtNow.Day, 0, 0, 0);

                    DateTime dtStartRounded = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 0, 0, 0);
                    DateTime dtCounter = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 0, 0, 0);

                    string output = "";

                    while (dtCounter <= dtNowRounded)
                    {
                        if (dayDefs[(int)dtCounter.DayOfWeek * 3].ToLower() == "true")
                        {
                            double minutes = 0;
                            double maxMinutes = double.Parse(dayDefs[(int)dtCounter.DayOfWeek * 3 + 2]) * 60;
                            if (dtStartRounded == dtNowRounded)
                            {
                                TimeSpan ts = dtNow - dtStart;
                                minutes = ts.TotalMinutes;
                            }
                            else if (dtCounter == dtNowRounded)
                            {
                                TimeSpan ts = dtNow - dtCounter;
                                minutes = ts.TotalMinutes;
                            }
                            else
                            {
                                minutes = 24 * 60;
                            }

                            if (minutes > maxMinutes)
                                minutes = maxMinutes;

                            output += "<StopWatchValue Date=\"" + dtCounter.ToString("F") + "\" DateTicks=\"" + dtCounter.Ticks + "\" Minutes=\"" + Math.Floor(minutes) + "\"/>";


                        }
                        dtCounter = dtCounter.AddDays(1);
                    }

                    return "<StopWatch Status=\"0\">" + output + "</StopWatch>";
                }
                else
                {
                    return "<StopWatch Status=\"1\">Could not find stopwatch entry.</StopWatch>";
                }
            }
            catch (Exception ex)
            {
                return "<StopWatch Status=\"1\">Error: " + ex.Message + "</StopWatch>";
            }
        }

        public static string ShowApprovalNotification(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                string userId = "";
                string periodId = "";

                periodId = doc.FirstChild.Attributes["PeriodId"].Value;

                SPUser user = GetUser(oWeb, userId);

                string sql_getUserIDs = string.Empty;
                string sql_getApprovalCount = string.Empty;
                DataTable dtUserID = null;
                int approvalCount = 0;
                string bIsTimeSheetManager = "";
                string bIsProjectManager = "True";

                sql_getUserIDs = string.Format("select SharePointAccountID from LSTResourcepool WHERE (',' + TimesheetManagerID + ',' LIKE '%,{0},%') and Generic=0 ", user.ID);

                var queryExecutor = new QueryExecutor(oWeb);
                dtUserID = queryExecutor.ExecuteReportingDBQuery(sql_getUserIDs, new Dictionary<string, object> { });


                if (dtUserID != null && dtUserID.Rows.Count > 0)
                {
                    StringBuilder sharePointAccountIDs = new StringBuilder();
                    String userIDs;
                    bIsTimeSheetManager = "True";

                    using (var connection =
                        GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                    {
                        foreach (DataRow row in dtUserID.Rows)
                        {
                            sharePointAccountIDs.Append(string.Format("{0},", row["SharePointAccountID"]));
                        }

                        if (sharePointAccountIDs.Length > 0)
                        {
                            userIDs = sharePointAccountIDs.ToString().Substring(0, sharePointAccountIDs.ToString().Length - 1);

                            sql_getApprovalCount = string.Format(
                                "select count(*) as ApprovalCount from TSTIMESHEET where SITE_UID = @siteID " +
                                "and TSUSER_UID in (select TSUSERUID from TSUSER where USER_ID in ({0}) " +
                                "and SUBMITTED = @submitted " +
                                "and APPROVAL_STATUS = @approvalStatus and PERIOD_ID <= @periodId)",
                                userIDs);

                            using (var command = new SqlCommand(sql_getApprovalCount, connection))
                            {
                                command.Parameters.AddWithValue("@siteID", oWeb.Site.ID);
                                command.Parameters.AddWithValue("@userIDs", userIDs);
                                command.Parameters.AddWithValue("@submitted", 1);
                                command.Parameters.AddWithValue("@approvalStatus", 0);
                                command.Parameters.AddWithValue("@periodId", periodId);
                                var obj = command.ExecuteScalar();
                                approvalCount = Convert.ToInt32(obj);
                            }
                        }
                    }
                }
                else
                {
                    bIsTimeSheetManager = "False";
                }

                // Check if the user is in the role. If not disabled 'Project Managers' options from MyTimesheet page.
                SPRoleChecker roleChecker = new SPRoleChecker();
                if (!roleChecker.ContainsRole(oWeb, "Contribute2"))
                {
                    bIsProjectManager = "False";
                }

                return "<ApprovalNotification Status=\"0\" IsTimeSheetManager=\"" + bIsTimeSheetManager + "\" IsProjectManager=\"" + bIsProjectManager + "\">" + approvalCount + "</ApprovalNotification>";
            }
            catch (Exception ex)
            {
                return "<ApprovalNotification Status=\"1\">Error: " + ex.Message + "</ApprovalNotification>";
            }

        }

        public static string ApplyHours(string data, SPWeb oWeb)
        {
            return "";
        }
        private const int TIMESHEET_REJECTION_NOTIFICATION = 18;
        public static string ApproveTimesheets(string data, SPWeb oWeb)
        {
            return ApproveTimesheetsCore(data, oWeb);
        }
        private static string AutoApproveTimesheets(string data, SPWeb oWeb, SqlTransaction transaction)
        {
            return ApproveTimesheetsCore(data, oWeb, transaction);
        }

        public static string ApproveTimesheetsCore(string data, SPWeb oWeb, SqlTransaction transaction = null)
        {
            try
            {
                var doc = new XmlDocument();
                doc.LoadXml(data);

                var timesheetNodes = doc.FirstChild.SelectNodes("//TS");

                var approvalStatus = "1";
                try
                {
                    approvalStatus = doc.FirstChild.Attributes["ApproveStatus"].Value;
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.ToString());
                }

                var outData = new StringBuilder();
                var errors = false;

                if (timesheetNodes.Count <= 0)
                {
                    throw new APIException(900002, "No submitted timesheets were selected");
                }
                else
                {
                    outData.Append("<Approve>");

                    bool liveHours;
                    bool.TryParse(EpmCoreFunctions.getConfigSetting(oWeb.Site.RootWeb, "EPMLiveTSLiveHours"), out liveHours);

                    var status = 3;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (var connection =
                        new SqlConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                        {
                            connection.Open();
                            foreach (XmlNode timesheetNode in timesheetNodes)
                            {
                                try
                                {
                                    using (var command = new SqlCommand(
                                        @"update TSTIMESHEET 
                                          set approval_status=@status,approval_notes=@notes,approval_date=GETDATE()
                                          where ts_uid=@ts_uid",
                                        transaction == null ? connection : transaction.Connection))
                                    {
                                        if (transaction != null)
                                        {
                                            command.Transaction = transaction;
                                        }
                                        command.Parameters.AddWithValue("@ts_uid", timesheetNode.Attributes["id"].Value);
                                        command.Parameters.AddWithValue("@notes", timesheetNode.InnerText);
                                        command.Parameters.AddWithValue("@status", approvalStatus);

                                        command.ExecuteNonQuery();
                                    }

                                    if (!liveHours)
                                    {
                                        using (var command = new SqlCommand(
                                            "SELECT status,jobtype_id FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30",
                                            connection))
                                        {
                                            command.Parameters.AddWithValue("@tsuid", timesheetNode.Attributes["id"].Value);

                                            using (var reader = command.ExecuteReader())
                                            {
                                                if (reader.Read())
                                                {
                                                    status = reader.GetInt32(0);
                                                }
                                            }
                                        }

                                        if (status == 3)
                                        {
                                            using (var command = new SqlCommand(
                                                                    "DELETE FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30",
                                                                    connection))
                                            {
                                                command.Parameters.AddWithValue("@tsuid", timesheetNode.Attributes["id"].Value);
                                                command.ExecuteNonQuery();
                                            }

                                            using (var command =
                                                new SqlCommand(
                                                    "INSERT INTO TSQUEUE (TS_UID,STATUS,JOBTYPE_ID,USERID,JOBDATA) VALUES(@tsuid,0,30,@USERID,@JOBDATA)",
                                                    connection))
                                            {
                                                command.Parameters.AddWithValue("@tsuid", timesheetNode.Attributes["id"].Value);
                                                command.Parameters.AddWithValue("@USERID", oWeb.CurrentUser.ID);
                                                command.Parameters.AddWithValue("@JOBDATA", "");
                                                command.ExecuteNonQuery();
                                            }
                                        }
                                    }

                                    outData.AppendFormat("<TS id='{0}' Status=\"0\"/>", timesheetNode.Attributes["id"].Value);

                                    if (approvalStatus == "2")
                                    {
                                        var userUid = Guid.Empty;
                                        var sharepointaccountid = 0;
                                        var emailTo = string.Empty;
                                        var emailcontent = string.Empty;
                                        var ResourceName = string.Empty;
                                        using (var command =
                                            new SqlCommand(
                                                "Select RESOURCENAME,TSUSER_UID from  TSTIMESHEET where ts_uid=@ts_uid",
                                                connection))
                                        {
                                            command.Parameters.AddWithValue("@ts_uid", timesheetNode.Attributes["id"].Value);
                                            using (var reader = command.ExecuteReader())
                                            {
                                                if (reader.Read())
                                                {
                                                    ResourceName = Convert.ToString(reader["RESOURCENAME"]);
                                                    userUid = Guid.Parse(Convert.ToString(reader["TSUSER_UID"]));

                                                }
                                            }
                                        }

                                        using (var command = new SqlCommand(
                                                                "SELECT USER_ID FROM TSUSER where TSUSERUID=@tsuser_uid",
                                                                connection))
                                        {
                                            command.Parameters.AddWithValue("@tsuser_uid", userUid);
                                            using (var reader = command.ExecuteReader())
                                            {
                                                if (reader.Read())
                                                {
                                                    sharepointaccountid = Convert.ToInt32(reader["USER_ID"]);
                                                }
                                            }
                                        }

                                        //Getting reciepient email address
                                        SPSecurity.RunWithElevatedPrivileges(() =>
                                        {
                                            using (var reportingConnection =
                                                new SqlConnection(
                                                    EpmCoreFunctions.getReportingConnectionString(
                                                        oWeb.Site.WebApplication.Id,
                                                        oWeb.Site.ID)))
                                            {
                                                reportingConnection.Open();
                                                using (var command =
                                                    new SqlCommand(
                                                        "Select Email from LSTResourcepool where SharePointAccountID=@sharepointaccountid",
                                                        reportingConnection))
                                                {
                                                    command.Parameters.AddWithValue("@sharepointaccountid", sharepointaccountid);
                                                    using (var reader = command.ExecuteReader())
                                                    {
                                                        if (reader.Read())
                                                        {
                                                            emailTo = Convert.ToString(reader["Email"]);
                                                        }
                                                    }
                                                }
                                            }
                                        });

                                        //Getting List pf rejected entries
                                        using (var command =
                                            new SqlCommand("Select Title,Project from TSITEM where ts_uid=@ts_uid", connection))
                                        {
                                            command.Parameters.AddWithValue("@ts_uid", timesheetNode.Attributes["id"].Value);
                                            using (var reader = command.ExecuteReader())
                                            {
                                                while (reader.Read())
                                                {
                                                    emailcontent += "<li>" + reader["Title"] + "</li>";
                                                }
                                            }
                                        }

                                        if (!string.IsNullOrWhiteSpace(emailTo))
                                        {
                                            var emaillist = new List<string>();
                                            emaillist.Add(emailTo);
                                            try
                                            {
                                                APIEmail.sendEmail(
                                                    TIMESHEET_REJECTION_NOTIFICATION,
                                                    new Hashtable()
                                                    {
                                                        { "TimesheetUser_Name", ResourceName },
                                                        { "Element_Entries", emailcontent }
                                                    },
                                                    emaillist,
                                                    string.Empty,
                                                    oWeb,
                                                    true);
                                            }
                                            catch (Exception exception)
                                            {
                                                Logger.WriteLog(
                                                    Logger.Category.Medium,
                                                    "TimeSheetAPI Approve TimeSheet",
                                                    exception.ToString());
                                            }
                                        }
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Logger.WriteLog(
                                        Logger.Category.Medium,
                                        "TimeSheetAPI Approve TimeSheet",
                                        exception.ToString());
                                    errors = true;
                                    outData.AppendFormat(
                                        "<TS id='{0}' Status=\"2\">{1}</TS>",
                                        timesheetNode.Attributes["id"].Value,
                                        exception.Message);
                                }
                            }
                            outData.Append("</Approve>");
                        }
                    });
                }
                if (errors)
                {
                    return Response.Failure(90010, outData.ToString());
                }
                else
                {
                    return Response.Success(outData.ToString());
                }
            }
            catch (APIException ex)
            {
                return Response.Failure(ex.ExceptionNumber, string.Format("Error: {0}", ex.Message));
            }
        }

        private static void queuetimesheet()
        {

        }

        public static string GetTimesheetGridLayout(string data, SPWeb web)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                int iGridType = int.Parse(doc.FirstChild.Attributes["GridType"].Value);
                string sPeriod = doc.FirstChild.Attributes["Period"].Value;
                string sGridId = doc.FirstChild.Attributes["GridId"].Value;
                string Editable = "0";

                try
                {
                    Editable = doc.FirstChild.Attributes["Editable"].Value;
                }
                catch { }

                XmlDocument docLayout = new XmlDocument();
                docLayout.LoadXml(Properties.Resources.txtMyTimesheet_GridLayout);
                XmlAttribute attr = docLayout.CreateAttribute("id");
                attr.Value = "TS" + sGridId;

                XmlNode ndCfg = docLayout.FirstChild.SelectSingleNode("//Cfg");
                ndCfg.Attributes.Append(attr);

                attr = docLayout.CreateAttribute("GridEditable");
                attr.Value = Editable;
                ndCfg.Attributes.Append(attr);

                attr = docLayout.CreateAttribute("PeriodId");
                attr.Value = sPeriod;
                ndCfg.Attributes.Append(attr);

                attr = docLayout.CreateAttribute("SaveAndSubmit");
                attr.Value = "false";
                ndCfg.Attributes.Append(attr);


                XmlNode nodeRightCols = docLayout.FirstChild.SelectSingleNode("//RightCols");
                XmlNode ndLeftCols = docLayout.FirstChild.SelectSingleNode("//LeftCols");
                XmlNode ndFooter = docLayout.FirstChild.SelectSingleNode("//Foot/I[@id='-1']");
                XmlNode nodeHeader = docLayout.FirstChild.SelectSingleNode("//Head/Header[@id='Header']");
                XmlNode nodeGroupDef = docLayout.FirstChild.SelectSingleNode("//Def/D[@Name='Group']");
                XmlNode nodeRDef = docLayout.FirstChild.SelectSingleNode("//Def/D[@Name='R']");

                TimesheetSettings settings = new TimesheetSettings(web);

                int RightWidth = 200;
                int MidWidth = 0;
                int TSCellWidth = 60;

                XmlNode ndSW = docLayout.FirstChild.SelectSingleNode("//RightCols/C[@Name='StopWatch']");

                if (settings.AllowStopWatch && iGridType == 0)
                {
                    RightWidth += int.Parse(ndSW.Attributes["Width"].Value);
                }
                else
                {
                    ndSW.Attributes["Visible"].Value = "0";
                }

                var totalColumnCalc = new StringBuilder();

                Dictionary<string, string> viewInfo = new Dictionary<string, string>();

                EPMLiveCore.API.ViewManager views = GetViews(web);

                foreach (KeyValuePair<string, Dictionary<string, string>> key in views.Views)
                {
                    try
                    {
                        if (key.Value["Default"].ToLower() == "true")
                        {
                            attr = docLayout.CreateAttribute("Group");
                            attr.Value = key.Value["Group"];
                            ndCfg.Attributes.Append(attr);

                            attr = docLayout.CreateAttribute("Sort");
                            attr.Value = key.Value["Sort"];
                            ndCfg.Attributes.Append(attr);

                            viewInfo = key.Value;
                        }
                    }
                    catch { }
                }

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    using (var connection =
                        GetOpenedConnection(EpmCoreFunctions.getConnectionString(web.Site.WebApplication.Id)))
                    {
                        DataSet dataSet;
                        using (var command =
                            new SqlCommand("SELECT TSTYPE_ID, TSTYPE_NAME FROM TSTYPE where SITE_UID=@siteid", connection))
                        {
                            command.Parameters.AddWithValue("@siteid", web.Site.ID);

                            dataSet = new DataSet();
                            using (var adapter = new SqlDataAdapter(command))
                            {
                                adapter.Fill(dataSet);
                            }
                        }

                        var calcOrder = new StringBuilder();
                        var starts = GetPeriodDaysArray(connection, settings, web, sPeriod);
                        foreach (DateTime start in starts)
                        {
                            var nodeColumn = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                            var createdAttribute = docLayout.CreateAttribute("Name");
                            createdAttribute.Value = string.Format("P{0}", start.Ticks);
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("Visible");
                            createdAttribute.Value = "1";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("CanHide");
                            createdAttribute.Value = "0";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("CanSort");
                            createdAttribute.Value = "0";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("CanResize");
                            createdAttribute.Value = "0";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("CanEdit");
                            createdAttribute.Value = "1";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("Width");
                            createdAttribute.Value = TSCellWidth.ToString();
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("Align");
                            createdAttribute.Value = "Right";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("Type");
                            createdAttribute.Value = "Text";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("Format");
                            createdAttribute.Value = ",0.00";
                            nodeColumn.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute("EditFormat");
                            createdAttribute.Value = ",0.00";
                            nodeColumn.Attributes.Append(createdAttribute);

                            if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0 || settings.AllowNotes)
                            {
                                var clonedNode = nodeColumn.CloneNode(true);
                                clonedNode.Attributes["Name"].Value =
                                    string.Format("TS{0}", clonedNode.Attributes["Name"].Value);
                                clonedNode.Attributes["Type"].Value = "Text";
                                clonedNode.Attributes["Visible"].Value = "0";

                                nodeRightCols.AppendChild(clonedNode);
                            }

                            nodeRightCols.AppendChild(nodeColumn);

                            //Header
                            createdAttribute = docLayout.CreateAttribute(string.Format("P{0}", start.Ticks));
                            createdAttribute.Value = start.ToString("ddd<br>MMM dd");
                            nodeHeader.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute(string.Format("P{0}Formula", start.Ticks));
                            createdAttribute.Value = "sum()";
                            nodeGroupDef.Attributes.Append(createdAttribute);

                            createdAttribute = docLayout.CreateAttribute(string.Format("P{0}Type", start.Ticks));
                            createdAttribute.Value = "Float";
                            nodeGroupDef.Attributes.Append(createdAttribute);

                            totalColumnCalc.AppendFormat("+P{0}", start.Ticks);

                            calcOrder.AppendFormat(",P{0}", start.Ticks);

                            RightWidth += TSCellWidth;
                        }

                        if (starts.Count > 0)
                        {
                            nodeRDef.Attributes["CalcOrder"].Value =
                                string.Concat(nodeRDef.Attributes["CalcOrder"].Value, calcOrder.ToString());
                            nodeGroupDef.Attributes["CalcOrder"].Value =
                                string.Concat(nodeGroupDef.Attributes["CalcOrder"].Value, calcOrder.ToString());
                        }
                    }

                    if (iGridType == 0)
                    {
                        using (var rsite = new SPSite(web.Site.ID))
                        {
                            using (var rweb = rsite.OpenWeb(web.ID))
                            {
                                var lockedWebGuid = EpmCoreFunctions.getLockedWeb(rweb);
                                if (lockedWebGuid != rweb.ID)
                                {
                                    using (var lockWeb = rsite.OpenWeb(lockedWebGuid))
                                    {
                                        PopulateTimesheetGridLayout(
                                            lockWeb,
                                            ref docLayout,
                                            settings,
                                            ref MidWidth,
                                            viewInfo,
                                            false,
                                            "My Work");
                                    }
                                }
                                else
                                {
                                    PopulateTimesheetGridLayout(
                                        rweb,
                                        ref docLayout,
                                        settings,
                                        ref MidWidth,
                                        viewInfo,
                                        false,
                                        "My Work");
                                }
                            }
                        }
                    }
                });



                XmlNode ndProgressCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                XmlAttribute attr2 = docLayout.CreateAttribute("Name");
                attr2.Value = "Progress";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Visible");
                attr2.Value = "1";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("CanHide");
                attr2.Value = "0";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("CanSort");
                attr2.Value = "0";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Width");
                attr2.Value = "100";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Type");
                attr2.Value = "Float";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Format");
                attr2.Value = "0%";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("HtmlPrefixFormula");
                attr2.Value = "'<div style=\\\"width:50px;border:1px solid gray;float:left;height:12px\\\"><div style=\\\"width:'+(Value>1?50:Math.abs(Value*50))+'px;overflow:hidden;float:left;background:'+(Value>1?'#DD0000':'#4B75FC')+';height:12px\\\">&nbsp;</div></div>'";
                ndProgressCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Formula");
                attr2.Value = "Work>0?((TSOtherHours + TSTotals)/Work):0";
                ndProgressCol.Attributes.Append(attr2);

                nodeRightCols.AppendChild(ndProgressCol);


                XmlNode ndTotalsCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                attr2 = docLayout.CreateAttribute("Name");
                attr2.Value = "TSTotals";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Visible");
                attr2.Value = "1";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("CanHide");
                attr2.Value = "0";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("CanSort");
                attr2.Value = "0";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Width");
                attr2.Value = "50";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Class");
                attr2.Value = "Totals";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Type");
                attr2.Value = "Float";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("CanEdit");
                attr2.Value = "0";
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Formula");
                attr2.Value = totalColumnCalc.ToString().Trim('+');
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Format");
                attr2.Value = ",0.00";
                ndTotalsCol.Attributes.Append(attr2);

                nodeRightCols.AppendChild(ndTotalsCol);

                attr2 = docLayout.CreateAttribute("TSTotals");
                attr2.Value = "Totals";
                ndTotalsCol.Attributes.Append(attr2);

                nodeHeader.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("TSTotalsFormula");
                attr2.Value = totalColumnCalc.ToString().Trim('+');
                nodeGroupDef.Attributes.Append(attr2);




                if (RightWidth > (TSCellWidth * 5) + 300)
                    RightWidth = (TSCellWidth * 5) + 300;

                attr2 = docLayout.CreateAttribute("MinRightWidth");
                attr2.Value = RightWidth.ToString();
                ndCfg.Attributes.Append(attr2);

                if (MidWidth > 300)
                    MidWidth = 300;

                attr2 = docLayout.CreateAttribute("MinMidWidth");
                attr2.Value = MidWidth.ToString();
                ndCfg.Attributes.Append(attr2);


                nodeGroupDef.Attributes["CalcOrder"].Value += ",TSTotals";


                XmlNode ndCols = docLayout.FirstChild.SelectSingleNode("//Cols");
                if (ndCols.ChildNodes.Count > 0)
                {
                    ndFooter.Attributes["MidHtml"].Value = "<div align=\"right\"><b>Total:&nbsp;</b></div>";
                }
                else
                {
                    ndFooter.Attributes["LeftHtml"].Value = "<div align=\"right\"><b>Total:&nbsp;</b></div>";
                }

                if (iGridType == 1)
                {
                    ndFooter.ParentNode.RemoveChild(ndFooter);


                    XmlNode ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "Project";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("GroupEmpty");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Width");
                    attr2.Value = "150";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanSort");
                    attr2.Value = "1";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanHide");
                    attr2.Value = "1";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanEdit");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.InsertAfter(ndNewCol, ndLeftCols.SelectSingleNode("//C[@Name='Title']"));

                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "PMApproval";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Html";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Width");
                    attr2.Value = "40";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanSort");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.PrependChild(ndNewCol);


                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "TMApproval";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Html";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Width");
                    attr2.Value = "40";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanSort");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.PrependChild(ndNewCol);



                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "ApprovalNotes";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanSort");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("CanEdit");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Width");
                    attr2.Value = "30";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Align");
                    attr2.Value = "Center";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Html";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.PrependChild(ndNewCol);


                    attr2 = docLayout.CreateAttribute("ChildPaging");
                    attr2.Value = "3";
                    ndCfg.Attributes.Append(attr2);

                    try
                    {
                        ndCfg.Attributes.Remove(ndCfg.Attributes["Group"]);
                    }
                    catch { }



                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "Work";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Float";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Visible");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.AppendChild(ndNewCol);

                    try
                    {
                        docLayout.FirstChild.SelectSingleNode("//Panel").Attributes["Visible"].Value = "1";
                    }
                    catch { }
                    try
                    {
                        docLayout.FirstChild.SelectSingleNode("//D[@Name='R']").Attributes["CanSelect"].Value = "0";
                    }
                    catch { }

                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "ResId";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Visible");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.AppendChild(ndNewCol);

                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "Submitted";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Visible");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Int";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.AppendChild(ndNewCol);

                    ndNewCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                    attr2 = docLayout.CreateAttribute("Name");
                    attr2.Value = "Approved";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Type");
                    attr2.Value = "Int";
                    ndNewCol.Attributes.Append(attr2);

                    attr2 = docLayout.CreateAttribute("Visible");
                    attr2.Value = "0";
                    ndNewCol.Attributes.Append(attr2);

                    ndLeftCols.AppendChild(ndNewCol);
                }

                return docLayout.OuterXml;
            }
            catch (Exception ex)
            {


                XmlDocument docLayout = new XmlDocument();
                docLayout.LoadXml(Properties.Resources.txtMyTimesheet_GridErrorLayout);

                docLayout.FirstChild.SelectSingleNode("//Foot/I").Attributes["Error"].Value = "Layout Load Error: " + System.Web.HttpUtility.HtmlEncode(ex.Message);

                return docLayout.OuterXml;
            }

        }

        private static string getFormat(SPField oField, XmlDocument oDoc, SPWeb oWeb)
        {
            return FormatHelper.GetFormat(
                oField,
                oDoc,
                oWeb,
                nInfo => $"{nInfo.CurrencySymbol}{nInfo.CurrencyGroupSeparator}0{nInfo.CurrencyDecimalSeparator}00",
                "0\\%;0\\%;0\\%");
        }

        private static void PopulateTimesheetGridLayout(SPWeb web, ref XmlDocument docLayout, TimesheetSettings settings, ref int MidWidth, Dictionary<string, string> viewInfo, bool isWork, string InputList)
        {

            Hashtable arrCols = new Hashtable();
            try
            {
                string[] sCols = viewInfo["Cols"].Split(',');
                foreach (string sCol in sCols)
                {
                    string[] sColInfo = sCol.Split('|');
                    string width = "";
                    try
                    {
                        width = sColInfo[1];
                    }
                    catch { }
                    if (sColInfo[0] != "")
                        arrCols.Add(sColInfo[0], width);
                }
            }
            catch { }
            if (!arrCols.Contains("Title"))
                arrCols.Add("Title", "");

            SPList oLstMyWork = web.Lists.TryGetList(InputList);
            if (oLstMyWork != null)
            {

                XmlNode ndCols = docLayout.FirstChild.SelectSingleNode("//Cols");
                XmlNode ndLeftCols = docLayout.FirstChild.SelectSingleNode("//LeftCols");
                XmlNode ndHeader = docLayout.FirstChild.SelectSingleNode("//Head/Header[@id='Header']");

                foreach (SPField field in oLstMyWork.Fields)
                {

                    if (field.Reorderable && isGoodField(field.InternalName))
                    {
                        int iFieldWidth = iGetFieldWidth(field);
                        string sFieldType = iGetFieldType(field);
                        string GoodFieldname = field.InternalName;
                        if (GoodFieldname.EndsWith("Type"))
                            GoodFieldname += "Field";

                        XmlNode ndCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                        XmlAttribute attr1 = docLayout.CreateAttribute("Name");
                        attr1.Value = GoodFieldname;
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Visible");
                        if (arrCols.Contains(GoodFieldname) || settings.TimesheetFields.Contains(field.InternalName))
                            attr1.Value = "1";
                        else
                            attr1.Value = "0";

                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("GroupEmpty");
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("CanSort");
                        attr1.Value = "1";
                        ndCol.Attributes.Append(attr1);


                        attr1 = docLayout.CreateAttribute("Width");
                        if (arrCols.Contains(GoodFieldname) && arrCols[GoodFieldname].ToString() != "")
                            attr1.Value = arrCols[GoodFieldname].ToString();
                        else
                            attr1.Value = iFieldWidth.ToString();
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Type");
                        attr1.Value = sFieldType;
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Align");
                        attr1.Value = iGetFieldAlign(field);
                        ndCol.Attributes.Append(attr1);


                        XmlDocument oDoc = new XmlDocument();
                        oDoc.LoadXml(field.SchemaXml);
                        string format = getFormat(field, oDoc, web);
                        if (format != "")
                        {
                            attr1 = docLayout.CreateAttribute("Format");
                            attr1.Value = format;
                            ndCol.Attributes.Append(attr1);
                        }


                        if (sFieldType == "Enum")
                        {
                            attr1 = docLayout.CreateAttribute("IconAlign");
                            attr1.Value = "Right";
                            ndCol.Attributes.Append(attr1);

                            iSetEnum(web, field, ref ndCol);

                        }


                        if (isWork)
                        {
                            if (!settings.TimesheetFields.Contains(field.InternalName))
                            {
                                attr1 = docLayout.CreateAttribute("CanHide");
                                attr1.Value = "1";
                                ndCol.Attributes.Append(attr1);

                                attr1 = docLayout.CreateAttribute("CanEdit");
                                attr1.Value = "0";
                                ndCol.Attributes.Append(attr1);

                                ndLeftCols.AppendChild(ndCol);
                            }
                        }
                        else if (settings.TimesheetFields.Contains(field.InternalName))
                        {
                            MidWidth += iFieldWidth;

                            attr1 = docLayout.CreateAttribute("CanHide");
                            attr1.Value = "0";
                            ndCol.Attributes.Append(attr1);

                            attr1 = docLayout.CreateAttribute("CanEdit");
                            attr1.Value = "1";
                            ndCol.Attributes.Append(attr1);

                            ndCols.AppendChild(ndCol);
                        }
                        else
                        {
                            attr1 = docLayout.CreateAttribute("CanHide");
                            attr1.Value = "1";
                            ndCol.Attributes.Append(attr1);

                            attr1 = docLayout.CreateAttribute("CanEdit");
                            attr1.Value = "0";
                            ndCol.Attributes.Append(attr1);

                            ndLeftCols.AppendChild(ndCol);
                        }

                        attr1 = docLayout.CreateAttribute(GoodFieldname);
                        attr1.Value = field.Title;
                        ndHeader.Attributes.Append(attr1);
                    }
                }
            }
            else
            {

            }
        }

        private static void iSetEnum(SPWeb web, SPField field, ref XmlNode ndCol)
        {
            string enums = "";
            string enumkeys = "";

            try
            {
                if (field.Type == SPFieldType.Choice)
                {
                    SPFieldChoice ocField = (SPFieldChoice)field;
                    foreach (string choice in ocField.Choices)
                    {
                        enums += "|" + choice;
                    }
                }
                else if (field.Type == SPFieldType.MultiChoice)
                {
                    SPFieldMultiChoice ocField = (SPFieldMultiChoice)field;
                    foreach (string choice in ocField.Choices)
                    {
                        enums += "|" + choice;
                    }
                }
                else if (field.Type == SPFieldType.Lookup)
                {
                    SPFieldLookup olField = (SPFieldLookup)field;

                    SPList oList = web.Lists[new Guid(olField.LookupList)];

                    foreach (SPListItem li in oList.Items)
                    {
                        enums += "|" + li[olField.LookupField].ToString();
                        enumkeys += "|" + li.ID;
                    }
                }
            }
            catch { }
            if (enums != "")
            {
                XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("Enum");
                attr1.Value = enums;
                ndCol.Attributes.Append(attr1);
            }
            if (enumkeys != "")
            {
                XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("EnumKeys");
                attr1.Value = enumkeys;
                ndCol.Attributes.Append(attr1);
            }
        }

        private static int iGetFieldWidth(SPField field)
        {
            switch (field.Type)
            {
                case SPFieldType.Boolean:
                    return 30;
                case SPFieldType.Number:
                case SPFieldType.Currency:
                case SPFieldType.DateTime:
                    return 80;
            }
            return 150;
        }

        private static string iGetFieldType(SPField field)
        {
            switch (field.Type)
            {
                case SPFieldType.Choice:
                case SPFieldType.MultiChoice:
                    return "Enum";
                case SPFieldType.Lookup:
                    return "Text";
                case SPFieldType.DateTime:
                    return "Date";
                case SPFieldType.Boolean:
                    return "Bool";
                case SPFieldType.Number:
                    if (((SPFieldNumber)field).DisplayFormat == SPNumberFormatTypes.NoDecimal)
                    {
                        return "Int";
                    }
                    else
                    {
                        return "Float";
                    }
                case SPFieldType.Currency:
                    return "Float";
            }
            return "Text";
        }

        private static string iGetFieldAlign(SPField field)
        {
            switch (field.Type)
            {
                case SPFieldType.Number:
                case SPFieldType.Currency:
                case SPFieldType.DateTime:
                    return "Right";
            }
            return "Left";
        }

        private static bool isGoodField(string field)
        {
            switch (field)
            {
                case "Title":
                    return false;
            }
            return true;
        }

        public static string GetTimesheetApprovalsGridPage(string data, SPWeb web, string sPeriod)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(System.Web.HttpUtility.HtmlDecode(data));



            SqlConnection cn = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(SPContext.Current.Web.Site.WebApplication.Id));
                cn.Open();
            });

            XmlNode ndB = doc.FirstChild.SelectSingleNode("//B");


            string tsuid = ndB.Attributes["id"].Value;

            XmlDocument docOut = new XmlDocument();
            docOut.LoadXml("<Grid><Body><B id=\"" + tsuid + "\"/></Body></Grid>");
            ndB = docOut.FirstChild.SelectSingleNode("//B");
            EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(web.Site.ID);

            DataSet dsTS = iiGetTSData(cn, web, sPeriod, new Guid(tsuid), rptData, string.Empty);
            TimesheetSettings settings = new TimesheetSettings(web);
            ArrayList arrPeriods = GetPeriodDaysArray(cn, settings, web, sPeriod);
            ArrayList arrLookups = new ArrayList();
            SPList lstMyWork = web.Site.RootWeb.Lists.TryGetList("My Work");

            if (lstMyWork != null)
            {
                foreach (SPField field in lstMyWork.Fields)
                {
                    if (field.Type == SPFieldType.Lookup)
                    {

                        arrLookups.Add(field.InternalName + "Text");

                    }
                }
            }
            foreach (DataRow dr in dsTS.Tables[2].Rows)
            {
                ndB.AppendChild(CreateTSRow(ref docOut, dsTS, dr, arrLookups, arrPeriods, settings, false, web));
            }



            cn.Close();

            return docOut.OuterXml;
        }

        public static string GetTimesheetApprovalsGrid(string data, SPWeb web)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);
                string Filter = string.Empty;
                string sPeriod = doc.FirstChild.Attributes["Period"].Value;
                string sGridId = doc.FirstChild.Attributes["GridId"].Value;
                if (doc.FirstChild.Attributes["Filter"] != null)
                {
                    Filter = doc.FirstChild.Attributes["Filter"].Value;
                }

                XmlDocument docData = new XmlDocument();
                docData.LoadXml("<Grid><Cfg TimesheetUID=\"\"/><Body><B></B></Body></Grid>");

                XmlNode ndB = docData.SelectSingleNode("//B");

                TimesheetSettings settings = new TimesheetSettings(web);

                EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(web.Site.ID);

                var sql = string.Format(
                    @"SELECT * FROM dbo.LSTResourcePool WHERE (',' + TimesheetManagerID + ',' LIKE '%,{0},%') and Generic=0 ANd (Disabled=0 or Disabled is NULL)",
                    web.CurrentUser.ID);
                DataTable dtMyResources = rptData.ExecuteSql(sql);

                string sResList = "";

                foreach (DataRow dr in dtMyResources.Rows)
                {
                    sResList += ";#" + dr["SharePointAccountID"].ToString();
                }

                sResList = sResList.Trim(';').Trim('#');

                var dataSet = new DataSet();
                ArrayList arrPeriods;
                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(web.Site.WebApplication.Id)))
                {
                    using (var command = new SqlCommand("spTSGetMyApprovals", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@siteid", web.Site.ID);
                        command.Parameters.AddWithValue("@periodid", sPeriod);
                        command.Parameters.AddWithValue("@resources", sResList);
                        using (var dataAdapter = new SqlDataAdapter(command))
                        {
                            dataAdapter.Fill(dataSet);
                        }
                    }

                    arrPeriods = GetPeriodDaysArray(connection, settings, web, sPeriod);
                }

                foreach (DataRow dr in dtMyResources.Rows)
                {
                    DataRow[] drTimesheets = dataSet.Tables[0].Select("USER_ID='" + dr["SharePointAccountId"].ToString() + "'");
                    DataRow drTimesheet = null;
                    string tsuid = "";

                    if (drTimesheets.Length > 0)
                    {
                        drTimesheet = drTimesheets[0];
                        tsuid = drTimesheet["TS_UID"].ToString();
                    }
                    if (Filter == "2")
                    {
                        if (drTimesheet == null || drTimesheet["SUBMITTED"].ToString().ToLower() == "false")
                        {
                            iGetApprovalRow(dr, drTimesheet, dataSet.Tables[1], ref docData, ndB, arrPeriods, tsuid);
                        }
                    }
                    else if (Filter == "3")
                    {
                        if (drTimesheet != null)
                        {
                            if (drTimesheet["SUBMITTED"].ToString().ToLower() == "true" && drTimesheet["APPROVAL_STATUS"].ToString() == "0")
                            {
                                iGetApprovalRow(dr, drTimesheet, dataSet.Tables[1], ref docData, ndB, arrPeriods, tsuid);
                            }
                        }
                    }
                    else
                    {
                        iGetApprovalRow(dr, drTimesheet, dataSet.Tables[1], ref docData, ndB, arrPeriods, tsuid);
                    }
                }

                return docData.OuterXml;
            }
            catch (Exception ex)
            {
                return "<Grid><Body><B><I Title=\"Error: " + ex.Message + "\"/></B></Body></Grid>";
            }
        }

        private static void iGetApprovalRow(DataRow drResource, DataRow drTimesheet, DataTable dtHours, ref XmlDocument docData, XmlNode ndB, ArrayList arrPeriods, string tsuid)
        {
            XmlNode ndRow = docData.CreateNode(XmlNodeType.Element, "I", docData.NamespaceURI);

            XmlAttribute attr1;

            attr1 = docData.CreateAttribute("Def");
            attr1.Value = "Resource";
            ndRow.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("ResId");
            attr1.Value = drResource["SharePointAccountId"].ToString();
            ndRow.Attributes.Append(attr1);

            if (drTimesheet != null)
            {
                attr1 = docData.CreateAttribute("id");
                attr1.Value = drTimesheet["TS_UID"].ToString();
                ndRow.Attributes.Append(attr1);


                //attr1 = docData.CreateAttribute("TMApproval");
                //if (drTimesheet["APPROVAL_STATUS"].ToString() == "1")
                //    attr1.Value = "<span class=\"icon-checkmark-circle-2\" style=\"color:#5BB75B\">";
                //else if (drTimesheet["APPROVAL_STATUS"].ToString() == "2")
                //    attr1.Value = "<span class=\"icon-cancel-circle-2\" style=\"color:#D9534F\">";
                //else
                if (drTimesheet["SUBMITTED"].ToString() == "True")
                {

                    attr1 = docData.CreateAttribute("Submitted");
                    attr1.Value = "1";
                    ndRow.Attributes.Append(attr1);

                    attr1 = docData.CreateAttribute("Approved");
                    attr1.Value = drTimesheet["APPROVAL_STATUS"].ToString();
                    ndRow.Attributes.Append(attr1);
                }
                else
                {
                    attr1 = docData.CreateAttribute("ApprovalNotesCanEdit");
                    attr1.Value = "0";
                    ndRow.Attributes.Append(attr1);

                    attr1 = docData.CreateAttribute("Submitted");
                    attr1.Value = "0";
                    ndRow.Attributes.Append(attr1);

                    attr1 = docData.CreateAttribute("Approved");
                    attr1.Value = "0";
                    ndRow.Attributes.Append(attr1);
                    //attr1 = docData.CreateAttribute("CanSelect");
                    //attr1.Value = "0";

                }
                ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("ApprovalNotes");
                attr1.Value = drTimesheet["APPROVAL_NOTES"].ToString();
                ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("Count");
                attr1.Value = "1";
                ndRow.Attributes.Append(attr1);

                foreach (DateTime dtStart in arrPeriods)
                {
                    DataRow[] drDayHour = dtHours.Select("TS_ITEM_DATE='" + dtStart.ToString() + "' AND TS_UID='" + tsuid + "'");
                    if (drDayHour.Length > 0)
                    {
                        attr1 = docData.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = drDayHour[0]["Hours"].ToString();
                        ndRow.Attributes.Append(attr1);
                    }
                    else
                    {
                        attr1 = docData.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = "0";
                        ndRow.Attributes.Append(attr1);
                    }
                }

            }
            else
            {
                //attr1 = docData.CreateAttribute("CanSelect");
                //attr1.Value = "0";
                //ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("ApprovalNotesCanEdit");
                attr1.Value = "0";
                ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("id");
                attr1.Value = drResource["SharePointAccountId"].ToString();
                ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("Submitted");
                attr1.Value = "0";
                ndRow.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("Approved");
                attr1.Value = "0";
                ndRow.Attributes.Append(attr1);


            }

            attr1 = docData.CreateAttribute("Title");
            attr1.Value = drResource["Title"].ToString();
            ndRow.Attributes.Append(attr1);




            ndB.AppendChild(ndRow);
        }

        public static string GetTimesheetGrid(string data, SPWeb web)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            string sPeriod = doc.FirstChild.Attributes["Period"].Value;
            string sGridId = doc.FirstChild.Attributes["GridId"].Value;
            string sUserId = "";
            try
            {
                sUserId = doc.FirstChild.Attributes["UserId"].Value;
            }
            catch { }
            SPUser user = GetUser(web, sUserId);
            sUserId = user.ID.ToString();
            //string sUsername = EPMLiveCore.CoreFunctions.GetRealUserName(web.CurrentUser.LoginName, web.Site);
            //string sUsername = web.CurrentUser.LoginName;

            XmlDocument docData = new XmlDocument();
            docData.LoadXml("<Grid><Cfg TimesheetUID=\"\"/><Body><B></B></Body></Grid>");

            XmlNode ndB = docData.SelectSingleNode("//B");

            TimesheetSettings settings = new TimesheetSettings(web);


            SPList lstMyWork = web.Site.RootWeb.Lists.TryGetList("My Work");

            ArrayList arrLookups = new ArrayList();

            if (lstMyWork != null)
            {
                foreach (SPField field in lstMyWork.Fields)
                {
                    if (field.Type == SPFieldType.Lookup || field.Type == SPFieldType.User)
                    {

                        arrLookups.Add(field.InternalName + "Text");

                    }
                }
            }


            try
            {
                DataSet dsTS;
                ArrayList arrPeriods;
                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(web.Site.WebApplication.Id)))
                {
                    dsTS = GetTSDataSet(connection, web, user, sPeriod);
                    arrPeriods = GetPeriodDaysArray(connection, settings, web, sPeriod);
                }

                bool bCanEdit = true;

                if (dsTS.Tables[1].Rows[0]["SUBMITTED"].ToString() == "True" || dsTS.Tables[1].Rows[0]["SUBMITTED"].ToString() == "True")
                {
                    bCanEdit = false;
                }


                foreach (DataRow dr in dsTS.Tables[2].Rows)
                {
                    ndB.AppendChild(CreateTSRow(ref docData, dsTS, dr, arrLookups, arrPeriods, settings, bCanEdit, web));
                }

                docData.SelectSingleNode("//Cfg").Attributes["TimesheetUID"].Value = dsTS.Tables[0].Rows[0]["tsuid"].ToString();
            }
            catch (Exception ex)
            {
                docData.LoadXml("<Grid><Body><B></B></Body></Grid>");

                XmlNode ndBod = docData.SelectSingleNode("//B");

                XmlNode ndCol = docData.CreateNode(XmlNodeType.Element, "I", docData.NamespaceURI);
                XmlAttribute attr1 = docData.CreateAttribute("Title");
                attr1.Value = "Data Error: " + ex.Message;
                ndCol.Attributes.Append(attr1);

                ndBod.AppendChild(ndCol);
            }



            return docData.OuterXml;
        }

        private static XmlNode CreateTSRow(ref XmlDocument docData, DataSet dsTS, DataRow dr, ArrayList arrLookups, ArrayList arrPeriods, TimesheetSettings settings, bool bCanEdit, SPWeb web)
        {

            var currenvyCultureInfo = new CultureInfo(1033);
            DataRow result = null;
            try
            {
                result = dsTS.Tables[myworktableid].Rows.Find(new[] { dr["LIST_UID"].ToString(), dr["ITEM_ID"].ToString() });
            }
            catch { }

            XmlNode ndCol = docData.CreateNode(XmlNodeType.Element, "I", docData.NamespaceURI);

            XmlAttribute attr1 = docData.CreateAttribute("UID");
            attr1.Value = dr["TS_ITEM_UID"].ToString();
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("PMApproval");
            if (dr["APPROVAL_STATUS"].ToString() == "1")
                attr1.Value = "<span class=\"icon-checkmark-circle-2\" style=\"color:#5BB75B\">";
            else if (dr["APPROVAL_STATUS"].ToString() == "2")
                attr1.Value = "<span class=\"icon-cancel-circle-2\" style=\"color:#D9534F\">";
            else
                attr1.Value = "";
            ndCol.Attributes.Append(attr1);

            //attr1 = docData.CreateAttribute("ShowLoading");
            //attr1.Value = @"<img id='MTG_Processing_" + dr["ITEM_ID"].ToString() + "' style='display:none;' src='/_layouts/epmlive/images/mywork/loading16.gif'></img>";
            //ndCol.Attributes.Append(attr1);

            if (result != null && Convert.ToBoolean(result["IsDeleted"]))
            {
                attr1 = docData.CreateAttribute("Title");
                attr1.Value = "<span style=\"text-decoration:line-through\" >" + dr["Title"].ToString() + "</span>";
                ndCol.Attributes.Append(attr1);
            }
            else
            {
                attr1 = docData.CreateAttribute("Title");
                attr1.Value = dr["Title"].ToString();
                ndCol.Attributes.Append(attr1);
            }

            attr1 = docData.CreateAttribute("Comments");
            attr1.Value = string.Format("<img class='TS_Comments' src='/_layouts/epmlive/images/mywork/comment.png' alt='Click here to add comments'/>");
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("SiteID");
            attr1.Value = web.Site.ID.ToString();
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("WebID");
            attr1.Value = dr["WEB_UID"].ToString();
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("ListID");
            attr1.Value = dr["LIST_UID"].ToString();
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("ItemID");
            attr1.Value = dr["ITEM_ID"].ToString();
            ndCol.Attributes.Append(attr1);

            attr1 = docData.CreateAttribute("ItemTypeID");
            attr1.Value = dr["ITEM_TYPE"].ToString();
            ndCol.Attributes.Append(attr1);

            //============My Work Fields==============

            if (result == null && dr["ITEM_TYPE"].ToString() == "1")//Regular Work
            {
                attr1 = docData.CreateAttribute("CanEdit");
                attr1.Value = "0";
                ndCol.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("TSEnabled");
                attr1.Value = "0";
                ndCol.Attributes.Append(attr1);

            }
            else if (dr["ITEM_TYPE"].ToString() == "2")//Non Work
            {
                attr1 = docData.CreateAttribute("CanEdit");
                attr1.Value = "1";
                ndCol.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("TSEnabled");
                attr1.Value = "1";
                ndCol.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("WorkTypeField");
                attr1.Value = settings.NonWorkList;
                ndCol.Attributes.Append(attr1);
            }
            else
            {


                foreach (DataColumn dc in dsTS.Tables[myworktableid].Columns)
                {
                    string GoodFieldname = dc.ColumnName;
                    if (GoodFieldname.EndsWith("Type"))
                        GoodFieldname += "Field";

                    if (arrLookups.Contains(GoodFieldname))
                    {
                        GoodFieldname = GoodFieldname.Substring(0, GoodFieldname.Length - 4);
                    }

                    if (isValidMyWorkColumn(GoodFieldname))
                    {
                        attr1 = docData.CreateAttribute(GoodFieldname);
                        if (GoodFieldname == "PercentComplete")
                        {
                            try
                            {
                                attr1.Value = Convert.ToString(Convert.ToDouble(result[dc.ColumnName].ToString()) * 100, currenvyCultureInfo.NumberFormat);
                            }
                            catch { attr1.Value = "0"; }
                        }
                        else
                        {
                            if (dc.DataType == typeof(Double))
                            {
                                try
                                {
                                    attr1.Value = ((double)result[dc.ColumnName]).ToString(currenvyCultureInfo.NumberFormat);
                                }
                                catch { }
                            }
                            else if (dc.DataType == typeof(DateTime))
                            {
                                try
                                {
                                    attr1.Value = DateTime.Parse(result[dc.ColumnName].ToString()).ToString("u");
                                }
                                catch { }
                            }
                            else
                                attr1.Value = result[dc.ColumnName].ToString();
                        }
                        ndCol.Attributes.Append(attr1);

                        if (GoodFieldname == "Timesheet")
                        {
                            string en = "0";

                            if (result[dc.ColumnName].ToString().ToLower() == "true")
                                en = "1";

                            attr1 = docData.CreateAttribute("CanEdit");
                            attr1.Value = en;
                            ndCol.Attributes.Append(attr1);

                            attr1 = docData.CreateAttribute("TSEnabled");
                            attr1.Value = en;
                            ndCol.Attributes.Append(attr1);
                        }
                    }
                }
            }

            //======================COmments==============
            string HasComments = "";

            try
            {
                string scomments = result["CommentCount"].ToString();
                double comments = 0;
                double.TryParse(scomments, out comments);
                if (comments > 0)
                {
                    //if (list.Fields.ContainsFieldWithStaticName("Commenters") && list.Fields.ContainsFieldWithStaticName("CommentersRead"))
                    {
                        ArrayList commenters = new ArrayList();
                        int authorid = 0;
                        try
                        {
                            commenters = new ArrayList(result["Commenters"].ToString().Split(','));
                        }
                        catch { }
                        try
                        {
                            SPFieldUserValue uv = new SPFieldUserValue(web, result["Author"].ToString());
                            authorid = uv.LookupId;
                        }
                        catch { }

                        ArrayList commentersread = new ArrayList();
                        try
                        {
                            commentersread = new ArrayList(result["CommentersRead"].ToString().Split(','));
                        }
                        catch { }
                        if (commentersread.Contains(web.CurrentUser.ID.ToString()))
                        {
                            HasComments += "1";
                        }
                        else
                        {
                            HasComments += "2";
                        }

                    }
                }
            }
            catch { }

            XmlAttribute attrName = docData.CreateAttribute("HasComments");
            attrName.Value = HasComments;
            ndCol.Attributes.Append(attrName);

            //============Timesheet Specific Fields==============
            foreach (string tsField in settings.TimesheetFields)
            {
                string GoodFieldname = tsField;
                if (GoodFieldname.EndsWith("Type"))
                    GoodFieldname += "Field";

                if (isValidMyWorkColumn(GoodFieldname))
                {
                    DataRow[] drTS = dsTS.Tables[5].Select("TS_ITEM_UID='" + dr["TS_ITEM_UID"].ToString() + "' AND ListName='MYTS' AND ColumnName='" + tsField + "'");

                    if (drTS.Length > 0)
                    {
                        attr1 = docData.CreateAttribute(GoodFieldname);
                        attr1.Value = drTS[0]["ColumnValue"].ToString();
                        ndCol.Attributes.Append(attr1);
                    }
                }
            }

            //============Other Work Hours==============
            DataRow[] drTSOther = dsTS.Tables[6].Select("LIST_UID='" + dr["LIST_UID"].ToString() + "' AND ITEM_ID='" + dr["ITEM_ID"].ToString() + "'");

            System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo(1033);
            IFormatProvider culture = new System.Globalization.CultureInfo(cInfo.Name, true);

            if (drTSOther.Length > 0)
            {
                attr1 = docData.CreateAttribute("TSOtherHours");
                attr1.Value = float.Parse(drTSOther[0]["OtherHours"].ToString()).ToString(culture);
                ndCol.Attributes.Append(attr1);
            }

            //============Hours==============
            ProcessHours(ref ndCol, dsTS, settings, dr["TS_ITEM_UID"].ToString(), arrPeriods, culture);
            //============StopWatch==========
            DataRow[] drSW = dsTS.Tables[9].Select("TS_ITEM_UID='" + dr["TS_ITEM_UID"].ToString() + "'");

            if (drSW.Length > 0)
            {
                attr1 = docData.CreateAttribute("StopWatch");
                attr1.Value = ((DateTime)drSW[0]["Started"]).ToString("F");
                ndCol.Attributes.Append(attr1);

                attr1 = docData.CreateAttribute("StopWatchIcon");
                attr1.Value = "/_layouts/epmlive/images/tstimeron.png";
                ndCol.Attributes.Append(attr1);
            }
            else
            {
                attr1 = docData.CreateAttribute("StopWatchIcon");
                attr1.Value = "/_layouts/epmlive/images/tstimeroff.png";
                ndCol.Attributes.Append(attr1);
            }

            if (!bCanEdit)
            {
                ndCol.Attributes["CanEdit"].Value = "0";
            }

            return ndCol;
        }

        internal static ArrayList GetPeriodDaysArray(SqlConnection cn, TimesheetSettings settings, SPWeb web, string sPeriod)
        {
            ArrayList arrPeriods = new ArrayList();

            SqlCommand cmd = new SqlCommand("SELECT period_start,period_end FROM TSPERIOD WHERE SITE_ID=@siteid and PERIOD_ID=@periodid", cn);
            cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
            cmd.Parameters.AddWithValue("@periodid", sPeriod);

            SqlDataReader dr = cmd.ExecuteReader();

            string[] dayDefs = settings.DayDef.Split('|');

            if (dr.Read())
            {
                DateTime dtStart = dr.GetDateTime(0);
                DateTime dtEnd = dr.GetDateTime(1);

                while (dtStart <= dtEnd)
                {
                    if (dayDefs[(int)dtStart.DayOfWeek * 3].ToLower() == "true")
                    {
                        arrPeriods.Add(dtStart);
                    }

                    dtStart = dtStart.AddDays(1);
                }
            }
            dr.Close();

            return arrPeriods;

        }

        private static void ProcessHours(ref XmlNode ndCol, DataSet dsTS, TimesheetSettings settings, string tsitemuid, ArrayList arrPeriods, IFormatProvider culture)
        {
            bool bHasTypes = (dsTS.Tables[7].Rows.Count > 0);

            foreach (DateTime dtStart in arrPeriods)
            {
                DataRow[] drHours = dsTS.Tables[3].Select("TS_ITEM_UID='" + tsitemuid + "' AND TS_ITEM_DATE='" + dtStart.ToString() + "'");
                DataRow[] drNotes = dsTS.Tables[4].Select("TS_ITEM_UID='" + tsitemuid + "' AND TS_ITEM_DATE='" + dtStart.ToString() + "'");

                if (bHasTypes)
                {
                    float totalHours = 0;
                    string hoursString = "";
                    if (bHasTypes)
                    {
                        foreach (DataRow drHour in drHours)
                        {
                            hoursString += "T" + drHour["TS_ITEM_TYPE_ID"].ToString() + ": " + float.Parse(drHour["TS_ITEM_HOURS"].ToString()).ToString(culture) + ",";

                            totalHours += float.Parse(drHour["TS_ITEM_HOURS"].ToString());
                        }
                    }
                    else
                    {
                        if (drHours.Length > 0)
                        {
                            totalHours = float.Parse(drHours[0]["TS_ITEM_HOURS"].ToString());
                        }
                    }

                    if (drNotes.Length > 0)
                    {
                        hoursString += "Notes: \"" + System.Web.HttpUtility.HtmlEncode(drNotes[0]["TS_ITEM_NOTES"].ToString()) + "\",";
                    }

                    hoursString = "{" + hoursString.Trim(',') + "}";

                    XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("P" + dtStart.Ticks);
                    attr1.Value = totalHours.ToString();
                    ndCol.Attributes.Append(attr1);

                    attr1 = ndCol.OwnerDocument.CreateAttribute("TSP" + dtStart.Ticks);
                    attr1.Value = hoursString;
                    ndCol.Attributes.Append(attr1);
                }
                else if (settings.AllowNotes)
                {

                    if (drHours.Length > 0)
                    {
                        XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = float.Parse(drHours[0]["TS_ITEM_HOURS"].ToString()).ToString(culture);
                        ndCol.Attributes.Append(attr1);
                    }
                    else
                    {
                        XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);
                    }

                    if (drNotes.Length > 0)
                    {
                        XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("TSP" + dtStart.Ticks);
                        attr1.Value = drNotes[0]["TS_ITEM_NOTES"].ToString();
                        ndCol.Attributes.Append(attr1);
                    }
                }
                else
                {
                    if (drHours.Length > 0)
                    {
                        XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = float.Parse(drHours[0]["TS_ITEM_HOURS"].ToString()).ToString(culture);
                        ndCol.Attributes.Append(attr1);
                    }
                    else
                    {
                        XmlAttribute attr1 = ndCol.OwnerDocument.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);
                    }
                }

            }
        }

        public static string AutoAddWork(string data, SPWeb oWeb)
        {
            if (oWeb == null)
            {
                throw new ArgumentNullException(nameof(oWeb));
            }

            var docTimesheet = new XmlDocument();
            docTimesheet.LoadXml(data);

            var timeSheetId = docTimesheet.FirstChild.Attributes["ID"].Value;

            using (var connection = GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
            {
                var submitted = false;
                var period = 0;

                using (var command =
                    new SqlCommand("SELECT submitted, period_id FROM TSTIMESHEET where TS_UID=@tsuid ", connection))
                {
                    command.Parameters.AddWithValue("@tsuid", timeSheetId);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            submitted = reader.GetBoolean(0);
                            period = reader.GetInt32(1);
                        }
                    }
                }

                var userId = string.Empty;
                try
                {
                    userId = docTimesheet.FirstChild.Attributes["UserId"].Value;
                }
                catch (Exception exception)
                {
                    Trace.TraceError(exception.ToString());
                }

                DateTime finish;
                DateTime start;
                using (var command = new SqlCommand(
                    "SELECT period_start,period_end FROM TSPERIOD WHERE SITE_ID=@siteid and PERIOD_ID=@periodid",
                    connection))
                {
                    command.Parameters.AddWithValue("@siteid", oWeb.Site.ID);
                    command.Parameters.AddWithValue("@periodid", period);
                    using (var reader = command.ExecuteReader())
                    {
                        finish = DateTime.MinValue;
                        start = DateTime.MaxValue;

                        if (reader.Read())
                        {
                            start = reader.GetDateTime(0);
                            finish = reader.GetDateTime(1);
                        }
                    }
                }

                if (!submitted)
                {
                    var reportData = new EpmWorkReportData(oWeb.Site.ID);
                    var rows = new List<string>();
                    try
                    {
                        if (docTimesheet.FirstChild.Attributes["Rows"] != null)
                        {
                            var tempRows = docTimesheet.FirstChild.Attributes["Rows"].Value.Split(',');
                            foreach (var tempRow in tempRows)
                            {
                                if (!string.IsNullOrWhiteSpace(tempRow) &&
                                    !rows.Contains(tempRow, StringComparer.CurrentCultureIgnoreCase))
                                {
                                    rows.Add(tempRow);
                                }
                            }
                        }

                        var dataSet = new DataSet();
                        using (var command =
                            new SqlCommand("SELECT list_uid,item_id FROM TSITEM WHERE TS_UID=@tsuid", connection))
                        {
                            command.Parameters.AddWithValue("@tsuid", timeSheetId);
                            using (var reader = command.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    var idValue = string.Format("{0}.{1}", reader.GetGuid(0), reader.GetInt32(1));
                                    if (!rows.Contains(idValue))
                                    {
                                        rows.Add(idValue);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Trace.TraceError(exception.ToString());
                    }

                    var user = GetUser(oWeb, userId);

                    var workTable = reportData.ExecuteSql(
                        string.Format(
                        "SELECT * FROM lstmywork where Timesheet=1 and StartDate < '{0}' AND DueDate > '{1}' AND AssignedToID='{2}'",
                        finish.ToString("s"),
                        start.ToString("s"),
                        user.ID));

                    foreach (DataRow rowWork in workTable.Rows)
                    {
                        var workValue = string.Format(
                            "{0}.{1}",
                            rowWork["ListId"],
                            rowWork["ItemId"]);
                        if (!rows.Contains(workValue, StringComparer.CurrentCultureIgnoreCase))
                        {
                            AddWorkItem(rowWork, oWeb.Site, timeSheetId, Guid.NewGuid(), connection);
                        }
                    }

                    return "<AutoAddWork Status=\"0\"></AutoAddWork>";
                }
                else
                {
                    return "<AutoAddWork Status=\"3\">Timesheet is submitted and cannot add work.</AutoAddWork>";
                }
            }
        }

        private static bool isValidMyWorkColumn(string colName)
        {
            switch (colName)
            {
                case "Title":
                case "ID":
                    return false;
            }
            return true;
        }

        internal static SPUser GetUser(SPWeb web, string sUserId)
        {
            if (sUserId == "")
                return web.CurrentUser;
            else
            {
                try
                {
                    SPUser u = web.SiteUsers.GetByID(int.Parse(sUserId));

                    if (iVerifyDelegate(web, u))
                    {
                        return u;
                    }
                    else
                    {
                        return web.CurrentUser;
                    }
                }
                catch (Exception ex)
                {
                    return web.CurrentUser;
                }
            }
        }

        public static string GetTSData(string data, SPWeb web)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);

            string sPeriod = doc.FirstChild.Attributes["Period"].Value;
            string sUserId = "";
            try
            {
                sUserId = doc.FirstChild.Attributes["UserId"].Value;
            }
            catch { }
            SPUser user = GetUser(web, sUserId);

            DataSet dataSet;
            using (var connection =
                GetOpenedConnection(EpmCoreFunctions.getConnectionString(web.Site.WebApplication.Id)))
            {
                dataSet = GetTSDataSet(connection, web, user, sPeriod);
            }

            return dataSet.GetXml();
        }

        private static DataSet GetTSDataSet(SqlConnection connection, SPWeb web, SPUser user, string period)
        {
            var rptData = new CoreReportHelper.MyWorkReportData(web.Site.ID);

            var timesheetId = Guid.Empty;
            using (var command = new SqlCommand(
                @"select TOP 1 TS_UID from TSTIMESHEET 
                    where SITE_UID = @siteid and PERIOD_ID = @period and USERNAME = @username",
                connection))
            {
                command.Parameters.AddWithValue("@siteid", web.Site.ID);
                command.Parameters.AddWithValue("@period", period);
                command.Parameters.AddWithValue("@username", user.LoginName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        timesheetId = reader.GetGuid(0);
                    }
                }
            }

            if (timesheetId == Guid.Empty)
            {
                timesheetId = GenerateTSFromPast(connection, web, user, period, rptData);
            }

            using (var command = new SqlCommand("SPTSSetUser", connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@siteid", web.Site.ID);
                command.Parameters.AddWithValue("@username", user.LoginName);
                command.Parameters.AddWithValue("@name", user.Name);
                command.Parameters.AddWithValue("@userid", user.ID);
                command.ExecuteNonQuery();
            }

            var userId = Guid.Empty;
            using (var cmd = new SqlCommand("SELECT TSUSERUID FROM TSUSER WHERE USER_ID=@uid", connection))
            {
                cmd.Parameters.AddWithValue("@uid", user.ID);

                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        userId = reader.GetGuid(0);
                    }
                    reader.Close();
                }
            }

            using (var command = new SqlCommand("UPDATE TSTIMESHEET SET TSUSER_UID=@uid where TS_UID=@tsuid", connection))
            {
                command.Parameters.AddWithValue("@tsuid", timesheetId);
                command.Parameters.AddWithValue("@uid", userId);
                command.ExecuteNonQuery();
            }

            return iiGetTSData(connection, web, period, timesheetId, rptData, Convert.ToString(user.ID));
        }

        private static DataSet iiGetTSData(SqlConnection cn, SPWeb web, string sPeriod, Guid tsuid, EPMLiveCore.ReportHelper.MyWorkReportData rptData, string userId)
        {
            SqlCommand cmd = new SqlCommand("spTSGetTimesheet", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@tsuid", tsuid);

            DataSet ds = new DataSet();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(ds);

            myworktableid = ds.Tables.Count;

            string sql = string.Format(@"SELECT * FROM dbo.LSTMyWork WHERE [AssignedToID] = -99999 AND [SiteId] = N'{0}'", web.Site.ID);
            DataTable myWorkDataTable = rptData.ExecuteSql(sql);

            ds.Tables.Add(myWorkDataTable);
            ArrayList drAdded = new ArrayList();

            DataColumn dc = new DataColumn("IsDeleted");
            dc.DataType = typeof(bool);
            dc.ReadOnly = false;
            dc.DefaultValue = Convert.ToBoolean(false);
            ds.Tables[myworktableid].Columns.Add(dc);

            string assignedToId = "-99";

            foreach (DataRow drItem in ds.Tables[2].Rows)
            {
                if (!drAdded.Contains(drItem["LIST_UID"].ToString() + "." + drItem["ITEM_ID"].ToString()))
                {
                    if (string.IsNullOrEmpty(Convert.ToString(drItem["ASSIGNEDTOID"])))
                    {
                        assignedToId = "-99";
                    }
                    else
                    {
                        assignedToId = Convert.ToString(drItem["ASSIGNEDTOID"]);
                    }
                    sql = string.Format(@"SELECT * FROM dbo.LSTMyWork WHERE [AssignedToID] = {0} AND [SiteId] = N'{1}' AND LISTID = N'{2}' AND ITEMID=N'{3}'", assignedToId, web.Site.ID, drItem["LIST_UID"].ToString(), drItem["ITEM_ID"].ToString());
                    myWorkDataTable = rptData.ExecuteSql(sql);


                    if (myWorkDataTable.Rows.Count > 0)
                        ds.Tables[myworktableid].Rows.Add(myWorkDataTable.Rows[0].ItemArray);
                    else if (Convert.ToString(drItem["List"]) != "Non Work")
                    {
                        DataTable dtTSItem = new DataTable();

                        SqlCommand cmdTSItem = new SqlCommand("select itm.WEB_UID,itm.LIST_UID, LIST, tm.SITE_UID, itm.ASSIGNEDTOID, itm.ITEM_ID, itm.PROJECT,itm.PROJECT_ID from TSITEM itm inner join TSTIMESHEET tm on tm.TS_UID = itm.TS_UID where itm.ITEM_ID=@ItemID and itm.TS_UID=@TSUID", cn);
                        cmdTSItem.Parameters.AddWithValue("@ItemID", Convert.ToString(drItem["ITEM_ID"]));
                        cmdTSItem.Parameters.AddWithValue("@TSUID", Convert.ToString(drItem["TS_UID"]));

                        SqlDataAdapter daTSItem = new SqlDataAdapter(cmdTSItem);
                        daTSItem.Fill(dtTSItem);

                        if (dtTSItem != null)
                        {
                            string project = EpmCoreFunctions.GetSafeGroupTitle(Convert.ToString(dtTSItem.Rows[0]["PROJECT"]));
                            string projectID = Convert.ToString(dtTSItem.Rows[0]["PROJECT_ID"]) == "" ? "null" : Convert.ToString(dtTSItem.Rows[0]["PROJECT_ID"]);
                            sql = string.Format(@"select '{0}' SiteId,'{1}' WebId,'{2}' ListId,{3} ItemId,{4} ProjectID, '{5}' ProjectText,0 IsAssignment,'{6}' WorkType,'true' IsDeleted ", Convert.ToString(dtTSItem.Rows[0]["SITE_UID"]), Convert.ToString(dtTSItem.Rows[0]["WEB_UID"]), Convert.ToString(dtTSItem.Rows[0]["LIST_UID"]), Convert.ToString(dtTSItem.Rows[0]["ITEM_ID"]), projectID, project, dtTSItem.Rows[0]["LIST"]);
                            myWorkDataTable = rptData.ExecuteSql(sql);

                            if (myWorkDataTable.Rows.Count > 0)
                            {
                                //Old Code  We had issue with Column sequence In select state we manually defined column sequence 
                                //ds.Tables[myworktableid].Rows.Add(myWorkDataTable.Rows[0].ItemArray);

                                //New Code 
                                DataRow dr = ds.Tables[myworktableid].NewRow();
                                foreach (DataColumn item in myWorkDataTable.Columns)
                                {
                                    try
                                    {
                                        dr[item.ColumnName] = myWorkDataTable.Rows[0][item.ColumnName];
                                    }
                                    catch (Exception ex)
                                    {
                                        dr[item.ColumnName] = DBNull.Value;
                                        Logger.WriteLog(Logger.Category.Unexpected, "TimeSheetAPI iiGetTSData", ex.ToString());
                                    }
                                }
                                DataView dv = new DataView(ds.Tables[myworktableid]);
                                dv.RowFilter = string.Format("ITEMID = '{0}' AND LISTID = '{1}'", dr["ITEMID"].ToString(), dr["LISTID"].ToString());
                                if (dv.Count == 0)
                                {
                                    ds.Tables[myworktableid].Rows.Add(dr);
                                }
                            }
                        }
                    }
                    try
                    {
                        if (!drAdded.Contains(drItem["LIST_UID"].ToString() + "." + drItem["ITEM_ID"].ToString()))
                        {
                            drAdded.Add(drItem["LIST_UID"].ToString() + "." + drItem["ITEM_ID"].ToString());
                        }
                    }
                    catch (Exception ex)
                    {

                        Logger.WriteLog(Logger.Category.Unexpected, "TimeSheetAPI iiGetTSData", ex.ToString());
                    }

                }
            }

            TimesheetSettings settings = new TimesheetSettings(web);

            foreach (string sField in settings.TimesheetFields)
            {
                if (sField != "")
                {
                    try
                    {
                        ds.Tables[myworktableid].Columns.Remove(sField);
                    }
                    catch { }
                }
            }

            ds.Tables[myworktableid].PrimaryKey = new[] { ds.Tables[myworktableid].Columns["ListId"], ds.Tables[myworktableid].Columns["ItemId"] };

            return ds;
        }

        private static Guid GenerateTSFromPast(
            SqlConnection connection,
            SPWeb web,
            SPUser user,
            string period,
            CoreReportHelper.MyWorkReportData reportData)
        {
            var timesheetGuid = Guid.NewGuid();
            var copyfromtGuid = Guid.Empty;
            using (var command = new SqlCommand(
                "select top 1 ts_uid from TSTIMESHEET " +
                "where period_id < @period and site_uid=@siteid and username=@username order by period_id desc",
                connection))
            {
                command.Parameters.AddWithValue("@period", period);
                command.Parameters.AddWithValue("@username", user.LoginName);
                command.Parameters.AddWithValue("@siteid", web.Site.ID);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        copyfromtGuid = reader.GetGuid(0);
                    }
                }
            }

            using (var command = new SqlCommand(
                "INSERT INTO TSTIMESHEET (TS_UID, USERNAME, RESOURCENAME, PERIOD_ID, SITE_UID) " +
                "VALUES (@tsuid, @username, @resourcename, @period, @siteid)",
                connection))
            {
                command.Parameters.AddWithValue("@tsuid", timesheetGuid);
                command.Parameters.AddWithValue("@period", period);
                command.Parameters.AddWithValue("@siteid", web.Site.ID);
                command.Parameters.AddWithValue("@username", user.LoginName);
                command.Parameters.AddWithValue("@resourcename", user.Name);
                command.ExecuteNonQuery();
            }

            if (copyfromtGuid != Guid.Empty)
            {
                var itemsSet = new DataSet();
                using (var command = new SqlCommand(
                    "SELECT * FROM TSITEM where TS_UID = @tsuid and item_type = 1",
                    connection))
                {
                    command.Parameters.AddWithValue("@tsuid", copyfromtGuid);
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(itemsSet);
                    }
                }

                foreach (DataRow itemRow in itemsSet.Tables[0].Rows)
                {
                    try
                    {
                        var sql = string.Format(
                            "SELECT * FROM dbo.LSTMyWork WHERE Complete != 1 and Status != 'Completed' " +
                            "AND ([AssignedToID] = -99 or [AssignedToID] = {0}) AND [SiteId] = N'{1}' " +
                            "AND LISTID = N'{2}' AND ITEMID=N'{3}'",
                            user.ID,
                            web.Site.ID,
                            itemRow["LIST_UID"].ToString(),
                            itemRow["ITEM_ID"].ToString());
                        var myWorkDataTable = reportData.ExecuteSql(sql);

                        if (myWorkDataTable.Rows.Count > 0)
                        {
                            if (myWorkDataTable.Rows[0]["Timesheet"].ToString() == bool.TrueString)
                            {
                                using (var command = new SqlCommand(
                                    "INSERT INTO TSITEM " +
                                    "(TS_UID, WEB_UID, LIST_UID, ITEM_ID, ITEM_TYPE, TITLE, PROJECT, PROJECT_ID, LIST, " +
                                    "PROJECT_LIST_UID,AssignedToID) " +
                                    "VALUES (@tsuid, @webuid, @listuid, @itemid, 1, @title, @project, @projectid, @list, " +
                                    "@projectlistuid, @assignedtoid)",
                                    connection))
                                {
                                    command.Parameters.AddWithValue("@tsuid", timesheetGuid);
                                    command.Parameters.AddWithValue("@webuid", itemRow["WEB_UID"].ToString());
                                    command.Parameters.AddWithValue("@listuid", itemRow["LIST_UID"].ToString());
                                    command.Parameters.AddWithValue("@itemid", itemRow["ITEM_ID"].ToString());
                                    command.Parameters.AddWithValue("@title", itemRow["TITLE"].ToString());
                                    command.Parameters.AddWithValue("@assignedtoid", user.ID);

                                    if (string.IsNullOrWhiteSpace(itemRow["PROJECT"].ToString()))
                                    {
                                        command.Parameters.AddWithValue("@project", DBNull.Value);
                                    }
                                    else
                                    {
                                        command.Parameters.AddWithValue(
                                            "@project",
                                            itemRow["PROJECT"].ToString());
                                    }

                                    if (string.IsNullOrWhiteSpace(itemRow["PROJECT_ID"].ToString()))
                                    {
                                        command.Parameters.AddWithValue("@projectid", DBNull.Value);
                                    }
                                    else
                                    {
                                        command.Parameters.AddWithValue(
                                            "@projectid",
                                            itemRow["PROJECT_ID"].ToString());
                                    }

                                    command.Parameters.AddWithValue("@list", itemRow["LIST"].ToString());

                                    if (string.IsNullOrWhiteSpace(itemRow["PROJECT_LIST_UID"].ToString()))
                                    {
                                        command.Parameters.AddWithValue("@projectlistuid", DBNull.Value);
                                    }
                                    else
                                    {
                                        command.Parameters.AddWithValue(
                                            "@projectlistuid",
                                            itemRow["PROJECT_LIST_UID"].ToString());
                                    }

                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Logger.WriteLog(
                            Logger.Category.Unexpected,
                            "TimeSheetAPI GenerateTSFromPast",
                            exception.ToString());
                    }
                }
            }

            return timesheetGuid;
        }

        private static bool iVerifyDelegate(SPWeb web, SPUser u)
        {
            //TODO: Delegates
            DataTable dt = EPMLiveCore.API.APITeam.GetResourcePool("<Data FilterField='SharePointAccount' FilterFieldValue='" + u.ID + ";#" + u.Name + "'/>", web);

            return true;

        }

        public static string GetWork(string data, SPWeb oWeb)
        {
            var docOut = new XmlDocument();
            docOut.LoadXml(Resources.txtMyTimesheetWork_GridLayout);
            var currencyCultureInfo = new CultureInfo(1033);

            try
            {
                bool otherWork;
                DataSet dataSet;
                ArrayList arrLookups;
                string userId;
                Dictionary<string, string> viewInfo;
                TimesheetSettings settings;
                int temp;
                string searchText;
                string searchField;
                string TSID;
                bool nonWork;

                GetAttributes(
                    data,
                    oWeb,
                    out nonWork,
                    out TSID,
                    out searchField,
                    out searchText,
                    out temp,
                    out settings,
                    out viewInfo,
                    out userId,
                    out arrLookups,
                    out dataSet,
                    docOut,
                    out otherWork);

                SPSecurity.RunWithElevatedPrivileges(
                    delegate
                    {
                        PopulateAction(
                            oWeb,
                            TSID,
                            dataSet,
                            nonWork,
                            settings,
                            viewInfo,
                            arrLookups,
                            ref docOut,
                            ref temp,
                            ref searchField,
                            out userId);
                    });

                ProcessNode(
                    oWeb,
                    userId,
                    docOut,
                    otherWork,
                    nonWork,
                    settings,
                    searchField,
                    searchText,
                    dataSet,
                    arrLookups,
                    currencyCultureInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception Suppressed {0}", ex);
                docOut.LoadXml(Resources.txtMyTimesheetWork_GridLayout);

                var nodeBod = docOut.SelectSingleNode("//B");

                var nodeCol = docOut.CreateNode(XmlNodeType.Element, "I", docOut.NamespaceURI);
                var title = docOut.CreateAttribute("Title");
                title.Value = "Data Error: " + ex.Message;
                nodeCol.Attributes.Append(title);

                nodeBod.AppendChild(nodeCol);
            }

            return docOut.OuterXml;
        }

        private static void PopulateAction(
            SPWeb oWeb,
            string TSID,
            DataSet dataSet,
            bool nonWork,
            TimesheetSettings settings,
            Dictionary<string, string> viewInfo,
            ArrayList arrLookups,
            ref XmlDocument docOut,
            ref int temp,
            ref string searchField,
            out string userId)
        {
            userId = GetUserIdFromDb(oWeb, TSID, dataSet);
            GetUserIdFromXml(oWeb, ref userId, docOut);

            if (userId != string.Empty)
            {
                var inputList = "My Work";
                if (nonWork)
                {
                    inputList = settings.NonWorkList;
                }

                using (var spSite = new SPSite(oWeb.Site.ID))
                {
                    using (var spWeb = spSite.OpenWeb(oWeb.ID))
                    {
                        var lWebGuid = EpmCoreFunctions.getLockedWeb(spWeb);
                        PopulateTimeSheetLayout(lWebGuid, spWeb, spSite, ref docOut, settings, viewInfo, inputList, ref temp);
                    }

                    GetSearchField(spSite, ref searchField, arrLookups);
                }
            }
        }

        private static void ProcessNode(
            SPWeb oWeb,
            string userId,
            XmlDocument docOut,
            bool otherWork,
            bool nonWork,
            TimesheetSettings settings,
            string searchField,
            string searchText,
            DataSet dataSet,
            ArrayList arrLookups,
            CultureInfo currencyCultureInfo)
        {
            if (userId != string.Empty)
            {
                var nodeBody = docOut.SelectSingleNode("//Body/B");

                var work = GetWorkDT(oWeb, otherWork, nonWork, userId, settings, searchField, searchText);

                foreach (DataRow dataRow in work.Rows)
                {
                    var ndRow = PopulateAttributes(oWeb, docOut, nonWork, dataRow, settings, dataSet);
                    ProcessFeatures(work, arrLookups, docOut, dataRow, currencyCultureInfo, ndRow);
                    nodeBody.AppendChild(ndRow);
                }
            }
        }

        private static void GetAttributes(
            string data,
            SPWeb oWeb,
            out bool nonWork,
            out string TSID,
            out string searchField,
            out string searchText,
            out int temp,
            out TimesheetSettings settings,
            out Dictionary<string, string> viewInfo,
            out string userId,
            out ArrayList arrLookups,
            out DataSet dataSet,
            XmlDocument docOut,
            out bool otherWork)
        {
            var docIn = new XmlDocument();
            docIn.LoadXml(data);

            TSID = docIn.FirstChild.Attributes["TSID"].Value;

            searchField = string.Empty;
            try
            {
                searchField = docIn.FirstChild.Attributes["SearchField"].Value;
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception Suppressed {0}", ex);
            }

            searchText = string.Empty;
            try
            {
                searchText = HttpUtility.UrlDecode(docIn.FirstChild.Attributes["SearchText"].Value).Trim();
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception Suppressed {0}", ex);
            }

            bool.TryParse(docIn.FirstChild.Attributes["OtherWork"].Value, out otherWork);
            bool.TryParse(docIn.FirstChild.Attributes["NonWork"].Value, out nonWork);

            var nodeCfg = docOut.FirstChild.SelectSingleNode("//Cfg");
            temp = 0;
            settings = new TimesheetSettings(oWeb);
            viewInfo = new Dictionary<string, string>();

            var views = nonWork
                ? GetNonWorkViews(oWeb)
                : GetWorkViews(oWeb);

            AddSorting(views, docOut, nodeCfg, ref viewInfo);

            userId = string.Empty;
            arrLookups = new ArrayList();
            dataSet = new DataSet();
        }

        private static void AddSorting(ViewManager views, XmlDocument docOut, XmlNode ndCfg, ref Dictionary<string, string> viewInfo)
        {
            foreach (var key in views.Views)
            {
                try
                {
                    if (string.Equals(key.Value["Default"], "true", StringComparison.OrdinalIgnoreCase))
                    {
                        var attribute = docOut.CreateAttribute("Group");
                        attribute.Value = key.Value["Group"];
                        ndCfg.Attributes.Append(attribute);

                        attribute = docOut.CreateAttribute("Sort");
                        attribute.Value = key.Value["Sort"];
                        ndCfg.Attributes.Append(attribute);

                        viewInfo = key.Value;
                    }
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Exception Suppressed {0}", ex);
                }
            }
        }

        private static string GetUserIdFromDb(SPWeb oWeb, string TSID, DataSet dataSet)
        {
            var userId = string.Empty;
            using (var connection = GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
            {
                using (var command = new SqlCommand(
                    "SELECT USER_ID, PERIOD_ID FROM dbo.TSTIMESHEET "
                    + "INNER JOIN dbo.TSUSER ON dbo.TSTIMESHEET.TSUSER_UID = dbo.TSUSER.TSUSERUID where TS_UID=@uid",
                    connection))
                {
                    command.Parameters.AddWithValue("@uid", TSID);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            userId = reader.GetInt32(0).ToString();
                        }
                    }
                }

                FillTimesheetItemsDataset(TSID, dataSet, connection);
            }
            return userId;
        }

        private static void GetUserIdFromXml(SPWeb oWeb, ref string userId, XmlDocument docOut)
        {
            if (userId == string.Empty)
            {
                docOut.LoadXml("<Grid><IO Result=\"-1\" Message=\"Could not determine user\"/></Grid>");
            }
            else
            {
                var user = GetUser(oWeb, userId);

                if (!string.Equals(user.ID.ToString(), userId, StringComparison.Ordinal))
                {
                    userId = string.Empty;
                    docOut.LoadXml("<Grid><IO Result=\"-1\" Message=\"User mismatch or access denied\"/></Grid>");
                }
            }
        }

        private static void PopulateTimeSheetLayout(
            Guid lWebGuid,
            SPWeb spWeb,
            SPSite spSite,
            ref XmlDocument docOut,
            TimesheetSettings settings,
            Dictionary<string, string> viewInfo,
            string inputList,
            ref int temp)
        {
            if (lWebGuid != spWeb.ID)
            {
                using (var web = spSite.OpenWeb(lWebGuid))
                {
                    PopulateTimesheetGridLayout(web, ref docOut, settings, ref temp, viewInfo, true, inputList);
                }
            }
            else
            {
                PopulateTimesheetGridLayout(spWeb, ref docOut, settings, ref temp, viewInfo, true, inputList);
            }
        }

        private static void GetSearchField(SPSite spSite, ref string searchField, ArrayList arrLookups)
        {
            try
            {
                var lstMyWork = spSite.RootWeb.Lists.TryGetList("My Work");

                if (lstMyWork != null)
                {
                    var searchFieldStringBuilder = new StringBuilder(searchField);
                    foreach (SPField field in lstMyWork.Fields)
                    {
                        if (field.Type == SPFieldType.Lookup || field.Type == SPFieldType.User)
                        {
                            if (field.InternalName == searchField)
                            {
                                searchFieldStringBuilder.Append("Text");
                            }

                            arrLookups.Add(string.Format("{0}Text", field.InternalName));
                        }
                    }
                    searchField = searchFieldStringBuilder.ToString();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError("Exception Suppressed {0}", ex);
            }
        }

        private static XmlNode PopulateAttributes(
            SPWeb oWeb,
            XmlDocument docOut,
            bool nonWork,
            DataRow dataRow,
            TimesheetSettings settings,
            DataSet dataSet)
        {
            var ndRow = docOut.CreateNode(XmlNodeType.Element, "I", docOut.NamespaceURI);

            var attribute = docOut.CreateAttribute("WorkTypeField");

            attribute.Value = !nonWork
                ? dataRow["WorkType"].ToString()
                : settings.NonWorkList;
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("UID");
            attribute.Value = Guid.NewGuid().ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("Title");
            attribute.Value = dataRow["Title"].ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("SiteID");
            attribute.Value = oWeb.Site.ID.ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("WebID");
            attribute.Value = dataRow["WebID"].ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("ListID");
            attribute.Value = dataRow["ListID"].ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("ItemID");
            attribute.Value = dataRow["ItemID"].ToString();
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("TSEnabled");
            attribute.Value = "1";
            ndRow.Attributes.Append(attribute);

            attribute = docOut.CreateAttribute("ItemTypeID");
            attribute.Value = nonWork
                ? "2"
                : "1";
            ndRow.Attributes.Append(attribute);

            var drCurrent = dataSet.Tables[0]
                .Select("LIST_UID='" + dataRow["listid"] + "' and ITEM_ID='" + dataRow["itemid"] + "'");
            if (drCurrent.Length > 0)
            {
                attribute = docOut.CreateAttribute("Current");
                attribute.Value = "1";
                ndRow.Attributes.Append(attribute);
            }
            return ndRow;
        }

        private static void ProcessFeatures(
            DataTable work,
            ArrayList arrLookups,
            XmlDocument docOut,
            DataRow dataRow,
            CultureInfo currencyCultureInfo,
            XmlNode ndRow)
        {
            XmlAttribute attribute;
            foreach (DataColumn dataColumn in work.Columns)
            {
                var goodFieldName = dataColumn.ColumnName;

                if (goodFieldName.EndsWith("Type"))
                {
                    goodFieldName += goodFieldName;
                }

                if (arrLookups.Contains(goodFieldName))
                {
                    goodFieldName = goodFieldName.Substring(0, goodFieldName.Length - 4);
                }

                if (isValidMyWorkColumn(goodFieldName))
                {
                    attribute = docOut.CreateAttribute(goodFieldName);
                    if (goodFieldName == "PercentComplete" && dataRow[dataColumn.ColumnName] != DBNull.Value)
                    {
                        attribute.Value = Convert.ToString(
                            Convert.ToDouble(dataRow[dataColumn.ColumnName]) * 100,
                            currencyCultureInfo.NumberFormat);
                    }
                    else
                    {
                        if (dataColumn.DataType == typeof(double) && dataRow[dataColumn.ColumnName] != DBNull.Value)
                        {
                            attribute.Value = Convert.ToString(
                                Convert.ToDouble(dataRow[dataColumn.ColumnName]),
                                currencyCultureInfo.NumberFormat);
                        }
                        else
                        {
                            attribute.Value = dataColumn.DataType == typeof(DateTime)
                                && !string.IsNullOrWhiteSpace(Convert.ToString(dataRow[dataColumn.ColumnName]))
                                    ? DateTime.Parse(Convert.ToString(dataRow[dataColumn.ColumnName])).ToString("u")
                                    : Convert.ToString(dataRow[dataColumn.ColumnName]);
                        }
                    }
                    ndRow.Attributes.Append(attribute);
                }
            }
        }

        private static void FillTimesheetItemsDataset(string id, DataSet dataSet, SqlConnection connection)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException(nameof(dataSet));
            }

            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            using (var command = new SqlCommand("SELECT * FROM TSITEM WHERE TS_UID=@tsuid", connection))
            {
                command.Parameters.AddWithValue("@tsuid", id);
                using (var dataAdapter = new SqlDataAdapter(command))
                {
                    dataAdapter.Fill(dataSet);
                }
            }
        }

        private static DataTable GetWorkDT(SPWeb oWeb, bool bOtherWork, bool bNonWork, string userid, TimesheetSettings settings, string SearchField, string SearchText)
        {
            EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(oWeb.Site.ID);

            string sql = "";

            if (!string.IsNullOrEmpty(SearchField) && !string.IsNullOrEmpty(SearchText))
            {
                if (settings.AllowUnassigned)
                {
                    sql = string.Format(@"SELECT * FROM  (SELECT  *,ROW_NUMBER() OVER (PARTITION BY AssignedToID,Title,ID,ProjectID ORDER BY [Modified]) AS 'RANK' FROM LSTMyWork) a WHERE a.[RANK] = 1 AND [AssignedToID] = -99 AND [SiteId] = N'{0}' AND Timesheet=1 AND {1} LIKE '%{2}%'", oWeb.Site.ID, SearchField, SearchText.Replace("'", "''"));
                }
                else
                {
                    sql = string.Format(@"SELECT * FROM  (SELECT  *,ROW_NUMBER() OVER (PARTITION BY AssignedToID,Title,ID,ProjectID ORDER BY [Modified]) AS 'RANK' FROM LSTMyWork) a WHERE a.[RANK] = 1 AND [AssignedToID] = {3} AND [SiteId] = N'{0}' AND Timesheet=1 AND {1} LIKE '%{2}%'", oWeb.Site.ID, SearchField, SearchText.Replace("'", "''"), userid);
                }
            }
            else if (bOtherWork)
            {
                sql = string.Format(@"SELECT * FROM  (SELECT  *,ROW_NUMBER() OVER (PARTITION BY AssignedToID,Title,ID,ProjectID ORDER BY [Modified]) AS 'RANK' FROM LSTMyWork) a WHERE a.[RANK] = 1 AND [AssignedToID] = -99 AND [SiteId] = N'{1}' AND Timesheet=1 AND IsAssignment = 0", userid, oWeb.Site.ID);
            }
            else if (bNonWork)
            {
                SPList list = oWeb.Lists.TryGetList(settings.NonWorkList);

                if (list != null)
                {
                    SPSiteDataQuery q = new SPSiteDataQuery();
                    q.Lists = "<Lists><List ID=\"" + list.ID.ToString() + "\"/></Lists>";
                    foreach (SPField field in list.Fields)
                    {
                        if (field.Reorderable)
                        {
                            q.ViewFields += "<FieldRef Name='" + field.InternalName + "' />";
                        }
                    }
                    q.RowLimit = 10000;

                    DataTable dt = oWeb.GetSiteData(q);

                    dt.Columns["ID"].ColumnName = "ItemId";

                    return dt;
                }
            }
            else
            {
                sql = string.Format(@"SELECT * FROM  (SELECT  *,ROW_NUMBER() OVER (PARTITION BY AssignedToID,Title,ID,ProjectID ORDER BY [Modified]) AS 'RANK' FROM LSTMyWork) a WHERE a.[RANK] = 1 AND [AssignedToID] = {0} AND [SiteId] = N'{1}' AND Timesheet=1", userid, oWeb.Site.ID);
            }



            return rptData.ExecuteSql(sql);

        }

        private static void AddWorkItem(DataRow row, SPSite site, string tsuid, Guid id, SqlConnection cn)
        {
            string webid = row["WebID"].ToString();
            string listid = row["ListID"].ToString();
            string itemid = row["ItemID"].ToString();
            string assignedtoid = row["AssignedToID"].ToString();

            if (webid != "")
            {
                if (listid != "")
                {
                    if (itemid != "")
                    {
                        try
                        {

                            using (SPWeb web = site.OpenWeb(new Guid(webid)))
                            {
                                SPListItem li = null;

                                SPList list = web.Lists[new Guid(listid)];

                                try
                                {
                                    try
                                    {
                                        li = list.GetItemById(int.Parse(itemid));
                                    }
                                    catch { }

                                    if (li != null)
                                    {
                                        int projectid = 0;
                                        string project = "";
                                        string projectlist = "";
                                        try
                                        {
                                            projectlist = ((SPFieldLookup)list.Fields.GetFieldByInternalName("Project")).LookupList;

                                            SPFieldLookupValue lv = new SPFieldLookupValue(li[list.Fields.GetFieldByInternalName("Project").Id].ToString());
                                            projectid = lv.LookupId;
                                            project = lv.LookupValue;
                                        }
                                        catch { }

                                        SqlCommand cmd = new SqlCommand("INSERT INTO TSITEM (TS_UID,TS_ITEM_UID,WEB_UID,LIST_UID,ITEM_ID,ITEM_TYPE,TITLE, PROJECT,PROJECT_ID, LIST,PROJECT_LIST_UID,ASSIGNEDTOID) VALUES(@tsuid,@uid,@webid,@listid,@itemid,1,@title,@project,@projectid,@list,@projectlistid,@assignedtoid)", cn);
                                        cmd.Parameters.AddWithValue("@tsuid", tsuid);
                                        cmd.Parameters.AddWithValue("@uid", id);
                                        cmd.Parameters.AddWithValue("@webid", web.ID);
                                        cmd.Parameters.AddWithValue("@listid", list.ID);
                                        cmd.Parameters.AddWithValue("@itemid", li.ID);
                                        cmd.Parameters.AddWithValue("@title", li["Title"].ToString());
                                        cmd.Parameters.AddWithValue("@list", list.Title);
                                        cmd.Parameters.AddWithValue("@assignedtoid", assignedtoid);
                                        if (projectlist == "")
                                            cmd.Parameters.AddWithValue("@projectlistid", DBNull.Value);
                                        else
                                            cmd.Parameters.AddWithValue("@projectlistid", projectlist);

                                        if (projectid == 0)
                                        {
                                            cmd.Parameters.AddWithValue("@project", DBNull.Value);
                                            cmd.Parameters.AddWithValue("@projectid", DBNull.Value);
                                        }
                                        else
                                        {
                                            cmd.Parameters.AddWithValue("@project", project);
                                            cmd.Parameters.AddWithValue("@projectid", projectid);
                                        }
                                        cmd.ExecuteNonQuery();
                                    }
                                }
                                catch { }
                            }
                        }
                        catch { }
                    }
                }
            }
        }

        public static string AddWork(string data, SPWeb oWeb)
        {

            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                string tsuid = docTimesheet.FirstChild.Attributes["TSUID"].Value;

                Guid id = Guid.NewGuid();

                using (var connection =
                    GetOpenedConnection(EpmCoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                {
                    var rptData = new EpmWorkReportData(oWeb.Site.ID);
                    var sItems = docTimesheet.FirstChild.InnerText.Split(',');

                    foreach (var sItem in sItems)
                    {
                        var itemInfo = sItem.Split('.');
                        var sql = string.Format(
                            @"SELECT top 1 * from dbo.LSTMyWork WHERE itemid={0} and listid='{1}'",
                            itemInfo[1],
                            itemInfo[0]);

                        var dtWork = rptData.ExecuteSql(sql);

                        if (dtWork.Rows.Count > 0)
                        {
                            AddWorkItem(dtWork.Rows[0], oWeb.Site, tsuid, id, connection);
                        }
                    }
                }

                return "<AddWork Status=\"0\"></AddWork>";

            }
            catch (Exception ex)
            {
                return "<AddWork Status=\"1\">Error: " + ex.Message + "</AddWork>";
            }

        }

        private static SqlConnection GetOpenedConnection(string connectionString)
        {
            SqlConnection connection = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
            });
            return connection;
        }
    }
}