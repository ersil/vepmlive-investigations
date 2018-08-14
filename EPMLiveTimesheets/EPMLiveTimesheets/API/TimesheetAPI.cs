﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.API;
using EPMLiveCore.ReportingProxy;
using Microsoft.SharePoint;
using TimeSheets.Log;
using TimeSheets.Models;
using EpmCoreFunctions = EPMLiveCore.CoreFunctions;

namespace TimeSheets
{
    public class TimesheetAPI
    {
        private static int myworktableid = 6;
        private const string UnSubmitTimesheetTemplate = "<UnSubmitTimesheet Status=\"{0}\">{1}</UnSubmitTimesheet>";

        static TimesheetAPI()
        {
            RoleChecker = new SPRoleChecker();
        }

        public static ISPRoleChecker RoleChecker { get; set; }

        public static string GetTimesheetUpdates(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                string id = docTimesheet.FirstChild.Attributes["ID"].Value;
                ArrayList rows = new ArrayList(docTimesheet.FirstChild.Attributes["Rows"].Value.Split(','));

                XmlDocument docRet = new XmlDocument();
                docRet.LoadXml("<Grid><IO/><Changes/></Grid>");

                XmlNode ndB = docRet.FirstChild.SelectSingleNode("//Changes");

                TimesheetSettings settings = new TimesheetSettings(oWeb);

                ArrayList arrLookups = new ArrayList();

                SPList lstMyWork = oWeb.Site.RootWeb.Lists.TryGetList("My Work");

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

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                SqlCommand cmd = new SqlCommand("SELECT USER_ID, PERIOD_ID FROM         dbo.TSTIMESHEET INNER JOIN dbo.TSUSER ON dbo.TSTIMESHEET.TSUSER_UID = dbo.TSUSER.TSUSERUID where TS_UID=@uid", cn);
                cmd.Parameters.AddWithValue("@uid", id);
                SqlDataReader drTS = cmd.ExecuteReader();

                string sUser = "";
                string sPeriod = "";

                if (drTS.Read())
                {
                    sUser = drTS.GetInt32(0).ToString();
                    sPeriod = drTS.GetInt32(1).ToString();
                }
                drTS.Close();

                if (sUser == "")
                {
                    return "<Grid><IO Result=\"-1\" Message=\"Could not determine user\"/></Grid>";
                }
                else
                {
                    SPUser user = GetUser(oWeb, sUser);

                    if (user.ID.ToString() == sUser)
                    {

                        DataSet dsTS = iGetTSData(cn, oWeb, user, sPeriod);

                        bool bCanEdit = true;

                        if (dsTS.Tables[1].Rows[0]["SUBMITTED"].ToString() == "True" || dsTS.Tables[1].Rows[0]["SUBMITTED"].ToString() == "True")
                        {
                            bCanEdit = false;
                        }

                        ArrayList arrPeriods = GetPeriodDaysArray(cn, settings, oWeb, sPeriod);

                        try
                        {
                            cn.Close();
                        }
                        catch { }

                        foreach (DataRow dr in dsTS.Tables[2].Rows)
                        {
                            if (!rows.Contains(dr["TS_ITEM_UID"].ToString()))
                            {
                                XmlNode nd = CreateTSRow(ref docRet, dsTS, dr, arrLookups, arrPeriods, settings, bCanEdit, oWeb);

                                XmlAttribute attr = docRet.CreateAttribute("Added");
                                attr.Value = "1";
                                nd.Attributes.Append(attr);

                                ndB.AppendChild(nd);
                            }
                        }
                    }
                    else
                    {
                        return "<Grid><IO Result=\"-1\" Message=\"User mismatch or access denied\"/></Grid>";
                    }
                }

                return docRet.OuterXml;
            }
            catch (Exception ex)
            {
                return "<Grid><IO Result=\"-1\" Message=\"" + ex.Message + "\"/></Grid>";
            }
        }

        public static string GetOtherHours(string data, SPWeb oWeb)
        {
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                SqlCommand cmd = new SqlCommand(@"SELECT     SUM(dbo.TSITEMHOURS.TS_ITEM_HOURS) AS Hours
                                                    FROM         dbo.TSITEMHOURS INNER JOIN
                                                    dbo.TSITEM ON dbo.TSITEMHOURS.TS_ITEM_UID = dbo.TSITEM.TS_ITEM_UID where LIST_UID=@listid and ITEM_ID=@itemid", cn);
                cmd.Parameters.AddWithValue("@listid", docTimesheet.FirstChild.Attributes["List"].Value);
                cmd.Parameters.AddWithValue("@itemid", docTimesheet.FirstChild.Attributes["ID"].Value);

                double hours = 0;

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read() && !dr.IsDBNull(0))
                    hours = dr.GetDouble(0);
                dr.Close();

                cn.Close();

                return "<GetOtherHours Status=\"0\">" + hours + "</GetOtherHours>";
            }
            catch (Exception ex)
            {
                return "<GetOtherHours Status=\"1\">" + ex.Message + "</GetOtherHours>";
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

                    var userid = 0;
                    using (var command = new SqlCommand(
                        @"SELECT dbo.TSUSER.USER_ID FROM dbo.TSUSER 
                        INNER JOIN dbo.TSTIMESHEET ON dbo.TSUSER.TSUSERUID = dbo.TSTIMESHEET.TSUSER_UID 
                        WHERE TS_UID=@tsuid",
                        connection))
                    {
                        command.Parameters.AddWithValue("@tsuid", tsuid);
                        using (var reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userid = reader.GetInt32(0);
                            }
                        }
                    }

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

        public static string SubmitTimesheet(string data, SPWeb oWeb)
        {
            SqlTransaction transaction = null;
            SqlConnection cn = null;
            try
            {
                XmlDocument docTimesheet = new XmlDocument();
                docTimesheet.LoadXml(data);

                string tsuid = docTimesheet.FirstChild.Attributes["ID"].Value;

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                var cmd = new SqlCommand("SELECT dbo.TSUSER.USER_ID " +
                                        "FROM dbo.TSUSER INNER JOIN dbo.TSTIMESHEET " +
                                        "    ON dbo.TSUSER.TSUSERUID = dbo.TSTIMESHEET.TSUSER_UID " +
                                        "WHERE TS_UID=@tsuid", cn);
                cmd.Parameters.AddWithValue("@tsuid", tsuid);

                var dr = cmd.ExecuteReader();

                int userid = 0;

                try
                {
                    if (dr.Read())
                    {
                        userid = dr.GetInt32(0);
                    }
                }
                finally
                {
                    dr.Close();
                }

                string message = "";

                if (userid != 0)
                {
                    SPUser user = TimesheetAPI.GetUser(oWeb, userid.ToString());

                    if (user.ID != userid)
                    {
                        message = "<SubmitTimesheet Status=\"3\">You do not have access to edit that timesheet.</SubmitTimesheet>";
                    }
                    else
                    {
                        transaction = cn.BeginTransaction();
                        try
                        {
                            cmd = new SqlCommand("Update TSTIMESHEET set submitted=1,LASTMODIFIEDBYU=@uname,LASTMODIFIEDBYN=@name, LastSubmittedByName=@lastSubmittedByName, LastSubmittedByUser=@lastSubmittedByUser where TS_UID=@tsuid", cn, transaction);
                            cmd.Parameters.AddWithValue("@uname", oWeb.CurrentUser.LoginName);
                            cmd.Parameters.AddWithValue("@name", oWeb.CurrentUser.Name);
                            cmd.Parameters.AddWithValue("@tsuid", tsuid);
                            cmd.Parameters.AddWithValue("@lastSubmittedByName", oWeb.CurrentUser.Name);
                            cmd.Parameters.AddWithValue("@lastSubmittedByUser", oWeb.CurrentUser.LoginName);

                            cmd.ExecuteNonQuery();

                            TimesheetSettings settings = new TimesheetSettings(oWeb);

                            if (settings.DisableApprovals)
                            {
                                XmlDocument docRet = new XmlDocument();
                                docRet.LoadXml(AutoApproveTimesheets("<Approve ApproveStatus=\"1\"><TS id=\"" + tsuid + "\"></TS></Approve>", oWeb, transaction));

                                if (docRet.FirstChild.Attributes["Status"].Value == "1")
                                {
                                    throw new Exception(System.Web.HttpUtility.HtmlDecode(docRet.FirstChild.SelectSingleNode("//Error").InnerText));
                                }
                            }

                            transaction.Commit();
                        }
                        catch
                        {
                            if (transaction != null)
                            {
                                transaction.Rollback();
                            }
                            throw;
                        }


                        message = "<SubmitTimesheet Status=\"0\"></SubmitTimesheet>";
                    }
                }
                else
                {
                    message = "<SubmitTimesheet Status=\"2\">Invalid user found for timesheet.</SubmitTimesheet>";
                }

                ProcessFullMeta(oWeb.Site, cn, tsuid);

                cn.Close();

                return message;
            }
            catch (Exception ex)
            {
                return "<SubmitTimesheet Status=\"1\">Error: " + ex.Message + "</SubmitTimesheet>";
            }
            finally
            {
                if(cn != null)
                    cn.Dispose();
            }            
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

            if(ds.Tables.Count > 0)
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
                        SPListItem li = iList.GetItemById(int.Parse(dataRow["ITEM_ID"].ToString()));
                        SharedFunctions.processMeta(iWeb, iList, li, new Guid(dataRow["ts_item_uid"].ToString()), dataRow["project"].ToString(), cn, pList);
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
        public static void CheckNonTeamMemberAllocation(SPWeb oWeb, string tsuid, SqlConnection cn, string data)
        {
            List<TimeSheetItem> timeSheetItems;
            double qtdAllocatedHours = 0;

            timeSheetItems = GetTimeSheetItems(data, cn.ConnectionString);
            qtdAllocatedHours = CalculateAllocatedHours(timeSheetItems, cn.ConnectionString);

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

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                bool submitted = false;

                SqlCommand cmd = new SqlCommand("SELECT submitted FROM TSTIMESHEET where TS_UID=@tsuid ", cn);
                cmd.Parameters.AddWithValue("@tsuid", tsuid);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    submitted = dr.GetBoolean(0);
                }
                dr.Close();

                if (!submitted)
                {
                    int status = 3;

                    cmd = new SqlCommand("SELECT status,jobtype_id FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31", cn);
                    cmd.Parameters.AddWithValue("@tsuid", tsuid);

                    dr = cmd.ExecuteReader();
                    if (dr.Read())
                    {
                        status = dr.GetInt32(0);
                    }
                    dr.Close();

                    if (status == 3)
                    {
                        // [EPMLCID-9648] Begin: Checking if resource is allocating time to a project he/she is not member of
                        if (bool.Parse(EPMLiveCore.CoreFunctions.getConfigSetting(oWeb, "EPMLiveEnableNonTeamNotf")))
                            CheckNonTeamMemberAllocation(oWeb, tsuid, cn, data);

                        cmd = new SqlCommand("DELETE FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31", cn);
                        cmd.Parameters.AddWithValue("@tsuid", tsuid);
                        cmd.ExecuteNonQuery();

                        cmd = new SqlCommand("INSERT INTO TSQUEUE (TS_UID,STATUS,JOBTYPE_ID,USERID,JOBDATA) VALUES(@tsuid,0,31,@USERID,@JOBDATA)", cn);
                        cmd.Parameters.AddWithValue("@tsuid", tsuid);
                        cmd.Parameters.AddWithValue("@USERID", oWeb.CurrentUser.ID);
                        cmd.Parameters.AddWithValue("@JOBDATA", data);
                        cmd.ExecuteNonQuery();

                        cn.Close();

                        return "<SaveTimesheet Status=\"0\">Save Queued</SaveTimesheet>";
                    }
                    else
                    {
                        cn.Close();

                        return "<SaveTimesheet Status=\"2\">Timesheet is already being processed.</SaveTimesheet>";
                    }
                }
                else
                {
                    cn.Close();

                    return "<SaveTimesheet Status=\"3\">Timesheet is submitted and cannot save.</SaveTimesheet>";
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

                string tsuid = doc.FirstChild.Attributes["ID"].Value;
                string userid = "";
                try
                {
                    userid = doc.FirstChild.Attributes["UserId"].Value;
                }
                catch { }

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                SPUser user = GetUser(oWeb, userid);

                SqlCommand cmd = new SqlCommand("SELECT  *   FROM         dbo.TSITEM INNER JOIN dbo.TSTIMESHEET ON dbo.TSITEM.TS_UID = dbo.TSTIMESHEET.TS_UID INNER JOIN dbo.TSSW ON dbo.TSITEM.TS_ITEM_UID = dbo.TSSW.TSITEMUID where USER_ID=@userid and site_uid=@siteid", cn);
                cmd.Parameters.AddWithValue("@userid", user.ID);
                cmd.Parameters.AddWithValue("@siteid", oWeb.Site.ID);

                bool bError = false;
                string sMessage = "";

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    bError = true;
                    sMessage = "Timer already started on another item.";
                }
                dr.Close();

                if (!bError)
                {

                    DateTime dt = DateTime.Now;
                    cmd = new SqlCommand("INSERT INTO TSSW (TSITEMUID, STARTED, USER_ID) VALUES (@tsitemuid, @dt, @userid)", cn);
                    cmd.Parameters.AddWithValue("@tsitemuid", tsuid);
                    cmd.Parameters.AddWithValue("@dt", dt);
                    cmd.Parameters.AddWithValue("@userid", user.ID);
                    cmd.ExecuteNonQuery();

                    sMessage = dt.ToString("F");

                }

                cn.Close();

                if (bError)
                {
                    return "<StopWatch Status=\"1\">Error: " + sMessage + "</StopWatch>";
                }
                else
                {
                    return "<StopWatch Status=\"0\">" + sMessage + "</StopWatch>";
                }

            }
            catch (Exception ex)
            {
                return "<StopWatch Status=\"1\">Error: " + ex.Message + "</StopWatch>";
            }
        }

        public static string CheckSaveStatus(string data, SPWeb oWeb)
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                string tsuid = doc.FirstChild.Attributes["ID"].Value;

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                SqlCommand cmd = new SqlCommand("SELECT STATUS,PERCENTCOMPLETE,RESULT,RESULTTEXT FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=31", cn);
                cmd.Parameters.AddWithValue("@tsuid", tsuid);

                int status = -1;
                int pct = 0;
                string result = "";
                string resulttext = "";

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    status = dr.GetInt32(0);
                    if (!dr.IsDBNull(1))
                        pct = dr.GetInt32(1);
                    if (!dr.IsDBNull(2))
                        result = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        resulttext = dr.GetString(3);
                }
                dr.Close();

                cn.Close();


                return "<SaveStatus Result=\"0\" Status=\"" + status + "\" PercentComplete=\"" + pct + "\" ErrorResult=\"" + result + "\" ResultText=\"" + resulttext + "\"></SaveStatus>";


            }
            catch (Exception ex)
            {
                return "<SaveStatus Result=\"1\">Error: " + ex.Message + "</SaveStatus>";
            }
        }

        public static string CheckApproveStatus(string data, SPWeb oWeb)
        {

            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                string tsuid = doc.FirstChild.Attributes["ID"].Value;

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                SqlCommand cmd = new SqlCommand("SELECT STATUS,PERCENTCOMPLETE,RESULT,RESULTTEXT FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30", cn);
                cmd.Parameters.AddWithValue("@tsuid", tsuid);

                int status = -1;
                int pct = 0;
                string result = "";
                string resulttext = "";

                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    status = dr.GetInt32(0);
                    if (!dr.IsDBNull(1))
                        pct = dr.GetInt32(1);
                    if (!dr.IsDBNull(2))
                        result = dr.GetString(2);
                    if (!dr.IsDBNull(3))
                        resulttext = dr.GetString(3);
                }
                dr.Close();

                cmd = new SqlCommand("SELECT APPROVAL_STATUS FROM TSTIMESHEET where TS_UID=@tsuid", cn);
                cmd.Parameters.AddWithValue("@tsuid", tsuid);
                dr = cmd.ExecuteReader();

                int approvalstatus = 0;

                if (dr.Read())
                {
                    approvalstatus = dr.GetInt32(0);
                }
                dr.Close();

                cn.Close();


                return "<ApproveStatus Result=\"0\" Status=\"" + status + "\" PercentComplete=\"" + pct + "\" ErrorResult=\"" + result + "\" ResultText=\"" + resulttext + "\" ApprovalStatus=\"" + approvalstatus + "\"></ApproveStatus>";


            }
            catch (Exception ex)
            {
                return "<ApproveStatus Result=\"1\">Error: " + ex.Message + "</ApproveStatus>";
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

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });


                SPUser user = GetUser(oWeb, userid);

                DateTime dtStart = DateTime.MinValue;

                SqlCommand cmd = new SqlCommand("Select STARTED FROM TSSW where TSITEMUID = @id and USER_ID=@userid", cn);
                cmd.Parameters.AddWithValue("@id", tsuid);
                cmd.Parameters.AddWithValue("@userid", user.ID);
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    dtStart = dr.GetDateTime(0);
                }
                dr.Close();

                cmd = new SqlCommand("DELETE FROM TSSW where TSITEMUID = @id and USER_ID=@userid", cn);
                cmd.Parameters.AddWithValue("@id", tsuid);
                cmd.Parameters.AddWithValue("@userid", user.ID);
                cmd.ExecuteNonQuery();

                cn.Close();

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
                    SqlConnection cn = null;
                    StringBuilder sharePointAccountIDs = new StringBuilder();
                    String userIDs;
                    bIsTimeSheetManager = "True";

                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                        cn.Open();
                    });

                    foreach (DataRow row in dtUserID.Rows)
                    {
                        sharePointAccountIDs.Append(string.Format("{0},", row["SharePointAccountID"]));
                    }

                    if (sharePointAccountIDs.Length > 0)
                    {
                        userIDs = sharePointAccountIDs.ToString().Substring(0, sharePointAccountIDs.ToString().Length - 1);

                        sql_getApprovalCount = "select count(*) as ApprovalCount from TSTIMESHEET where SITE_UID = @siteID and TSUSER_UID in (select TSUSERUID from TSUSER where USER_ID in (" + @userIDs + ") and SUBMITTED = @submitted and APPROVAL_STATUS = @approvalStatus and PERIOD_ID <= @periodId)";

                        using (SqlCommand cmd = new SqlCommand(sql_getApprovalCount, cn))
                        {
                            cmd.Parameters.AddWithValue("@siteID", oWeb.Site.ID);
                            cmd.Parameters.AddWithValue("@userIDs", userIDs);
                            cmd.Parameters.AddWithValue("@submitted", 1);
                            cmd.Parameters.AddWithValue("@approvalStatus", 0);
                            cmd.Parameters.AddWithValue("@periodId", periodId);
                            var obj = cmd.ExecuteScalar();
                            approvalCount = Convert.ToInt32(obj);
                        }

                        cn.Close();

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
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                //XmlNode ndPeriod = doc.FirstChild.SelectSingleNode("//Period");
                XmlNodeList ndTS = doc.FirstChild.SelectNodes("//TS");

                string ApprovalStatus = "1";
                try
                {
                    ApprovalStatus = doc.FirstChild.Attributes["ApproveStatus"].Value;
                }
                catch { }

                string outData = "";
                bool errors = false;

                //if(ndPeriod == null)
                //{
                //    throw new EPMLiveCore.API.APIException(900001, "No Period Provided");
                //}
                //else 
                if (ndTS.Count <= 0)
                {
                    throw new EPMLiveCore.API.APIException(900002, "No submitted timesheets were selected");
                }
                else
                {

                    //Guid tJob = Guid.NewGuid();

                    //SqlCommand cmd = new SqlCommand("INSERT INTO TIMERJOBS (timerjobuid, siteguid, jobtype, jobname,  scheduletype, jobdata, [key]) VALUES (@timerjobuid, @siteguid, 30, 'Timesheet Approval', 0, @jobdata, @key)", cn);
                    //cmd.Parameters.AddWithValue("@siteguid", oWeb.Site.ID);
                    //cmd.Parameters.AddWithValue("@jobdata", ndTS.InnerText);
                    //cmd.Parameters.AddWithValue("@timerjobuid", tJob);
                    //cmd.Parameters.AddWithValue("@key", ndPeriod.InnerText);
                    //cmd.ExecuteNonQuery();

                    //EPMLiveCore.CoreFunctions.enqueue(tJob, 0);


                    outData = "<Approve>";

                    bool liveHours = false;

                    bool.TryParse(EPMLiveCore.CoreFunctions.getConfigSetting(oWeb.Site.RootWeb, "EPMLiveTSLiveHours"), out liveHours);


                    //string[] tsUids = ndTS.InnerText.Split(',');

                    int status = 3;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        using (SqlConnection cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id)))
                        {
                            cn.Open();
                            foreach (XmlNode TS in ndTS)
                            {
                                //if (tsUid != "")
                                {
                                    try
                                    {
                                        //string[] tsData = tsUid.Split('|');

                                        SqlCommand cmd = new SqlCommand("update TSTIMESHEET set approval_status=@status,approval_notes=@notes,approval_date=GETDATE()"
                                            +"where ts_uid=@ts_uid", transaction == null ? cn : transaction.Connection);
                                        if (transaction != null)
                                        {
                                            cmd.Transaction = transaction;
                                        }
                                        cmd.Parameters.AddWithValue("@ts_uid", TS.Attributes["id"].Value);
                                        cmd.Parameters.AddWithValue("@notes", TS.InnerText);
                                        cmd.Parameters.AddWithValue("@status", ApprovalStatus);
                                                                                  
                                        cmd.ExecuteNonQuery();

                                        if (!liveHours)
                                        {
                                            cmd = new SqlCommand("SELECT status,jobtype_id FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30", cn);
                                            cmd.Parameters.AddWithValue("@tsuid", TS.Attributes["id"].Value);

                                            SqlDataReader dr = cmd.ExecuteReader();
                                            if (dr.Read())
                                            {
                                                status = dr.GetInt32(0);
                                            }
                                            dr.Close();

                                            if (status == 3)
                                            {
                                                cmd = new SqlCommand("DELETE FROM TSQUEUE where TS_UID=@tsuid and JOBTYPE_ID=30", cn);
                                                cmd.Parameters.AddWithValue("@tsuid", TS.Attributes["id"].Value);
                                                cmd.ExecuteNonQuery();

                                                cmd = new SqlCommand("INSERT INTO TSQUEUE (TS_UID,STATUS,JOBTYPE_ID,USERID,JOBDATA) VALUES(@tsuid,0,30,@USERID,@JOBDATA)", cn);
                                                cmd.Parameters.AddWithValue("@tsuid", TS.Attributes["id"].Value);
                                                cmd.Parameters.AddWithValue("@USERID", oWeb.CurrentUser.ID);
                                                // if (tsData.Length > 1)
                                                //    cmd.Parameters.AddWithValue("@JOBDATA", tsData[1]);
                                                //else
                                                cmd.Parameters.AddWithValue("@JOBDATA", "");
                                                cmd.ExecuteNonQuery();

                                            }

                                        }

                                        outData += "<TS id='" + TS.Attributes["id"].Value + "' Status=\"0\"/>";
                                        if (ApprovalStatus == "2")
                                        {
                                            Guid tuseruid = Guid.Empty;
                                            int sharepointaccountid = 0;
                                            string emailto = string.Empty;
                                            string emailcontent = string.Empty;
                                            string ResourceName = "";
                                            cmd = new SqlCommand("Select RESOURCENAME,TSUSER_UID from  TSTIMESHEET where ts_uid=@ts_uid", cn);
                                            cmd.Parameters.AddWithValue("@ts_uid", TS.Attributes["id"].Value);
                                            using (SqlDataReader dr = cmd.ExecuteReader())
                                            {

                                                if (dr.Read())
                                                {
                                                    ResourceName = Convert.ToString(dr["RESOURCENAME"]);
                                                    tuseruid = Guid.Parse(Convert.ToString(dr["TSUSER_UID"]));

                                                }
                                            }
                                            cmd = new SqlCommand("SELECT USER_ID FROM TSUSER where TSUSERUID=@tsuser_uid", cn);
                                            cmd.Parameters.AddWithValue("@tsuser_uid", tuseruid);
                                            using (SqlDataReader dr = cmd.ExecuteReader())
                                            {
                                                if (dr.Read())
                                                {
                                                    sharepointaccountid = Convert.ToInt32(dr["USER_ID"]);
                                                }
                                            }
                                            //Getting reciepient email address
                                            SPSecurity.RunWithElevatedPrivileges(() =>
                                            {
                                                using (SqlConnection rptcon = new SqlConnection(EPMLiveCore.CoreFunctions.getReportingConnectionString(oWeb.Site.WebApplication.Id, oWeb.Site.ID)))
                                                {
                                                    rptcon.Open();
                                                    cmd = new SqlCommand("Select Email from LSTResourcepool where SharePointAccountID=@sharepointaccountid", rptcon);
                                                    cmd.Parameters.AddWithValue("@sharepointaccountid", sharepointaccountid);
                                                    using (SqlDataReader dr = cmd.ExecuteReader())
                                                    {
                                                        if (dr.Read())
                                                        {
                                                            emailto = Convert.ToString(dr["Email"]);
                                                        }
                                                    }
                                                }
                                            });
                                            //Getting List pf rejected entries
                                            cmd = new SqlCommand("Select Title,Project from TSITEM where ts_uid=@ts_uid", cn);
                                            cmd.Parameters.AddWithValue("@ts_uid", TS.Attributes["id"].Value);
                                            using (SqlDataReader dr = cmd.ExecuteReader())
                                            {
                                                while (dr.Read())
                                                {
                                                    emailcontent += "<li>" + dr["Title"] + "</li>";
                                                }
                                            }


                                            if (!string.IsNullOrEmpty(emailto))
                                            {
                                                List<string> emaillist = new List<string>();
                                                emaillist.Add(emailto);
                                                try
                                                {
                                                    APIEmail.sendEmail(TIMESHEET_REJECTION_NOTIFICATION,
                                                           new Hashtable() { { "TimesheetUser_Name", ResourceName },
                                      { "Element_Entries", emailcontent } },
                                                           emaillist, string.Empty, oWeb, true);
                                                }
                                                catch (Exception ex)
                                                {

                                                    Logger.WriteLog(Logger.Category.Medium, "TimeSheetAPI Approve TimeSheet", ex.ToString());
                                                }

                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.WriteLog(Logger.Category.Medium, "TimeSheetAPI Approve TimeSheet", ex.ToString());
                                        errors = true;
                                        outData += "<TS id='" + TS.Attributes["id"].Value + "' Status=\"2\">" + ex.Message + "</TS>";
                                    }
                                }
                            }
                            outData += "</Approve>";
                        }
                    });

                }
                if (errors)
                    return EPMLiveCore.API.Response.Failure(90010, outData);
                else
                    return EPMLiveCore.API.Response.Success(outData);
            }
            catch (EPMLiveCore.API.APIException ex)
            {
                return EPMLiveCore.API.Response.Failure(ex.ExceptionNumber, string.Format("Error: {0}", ex.Message));
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


                XmlNode ndRightCols = docLayout.FirstChild.SelectSingleNode("//RightCols");
                XmlNode ndLeftCols = docLayout.FirstChild.SelectSingleNode("//LeftCols");
                XmlNode ndFooter = docLayout.FirstChild.SelectSingleNode("//Foot/I[@id='-1']");
                XmlNode ndHeader = docLayout.FirstChild.SelectSingleNode("//Head/Header[@id='Header']");
                XmlNode ndGroupDef = docLayout.FirstChild.SelectSingleNode("//Def/D[@Name='Group']");
                XmlNode ndRDef = docLayout.FirstChild.SelectSingleNode("//Def/D[@Name='R']");

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

                string TotalColumnCalc = "";

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
                    SqlConnection cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                    cn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT TSTYPE_ID, TSTYPE_NAME FROM TSTYPE where SITE_UID=@siteid", cn);
                    cmd.Parameters.AddWithValue("@siteid", web.Site.ID);

                    DataSet dsTypes = new DataSet();
                    SqlDataAdapter daTypes = new SqlDataAdapter(cmd);
                    daTypes.Fill(dsTypes);

                    ArrayList arrPeriods = GetPeriodDaysArray(cn, settings, web, sPeriod);

                    foreach (DateTime dtStart in arrPeriods)
                    {
                        //Col
                        XmlNode ndCol = docLayout.CreateNode(XmlNodeType.Element, "C", docLayout.NamespaceURI);
                        XmlAttribute attr1 = docLayout.CreateAttribute("Name");
                        attr1.Value = "P" + dtStart.Ticks;
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Visible");
                        attr1.Value = "1";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("CanHide");
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("CanSort");
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("CanResize");
                        attr1.Value = "0";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("CanEdit");
                        attr1.Value = "1";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Width");
                        attr1.Value = TSCellWidth.ToString();
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Align");
                        attr1.Value = "Right";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Type");
                        attr1.Value = "Text";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("Format");
                        //attr1.Value = "0.##";
                        attr1.Value = ",0.00";
                        ndCol.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("EditFormat");
                        attr1.Value = ",0.00";
                        ndCol.Attributes.Append(attr1);

                        if (dsTypes.Tables[0].Rows.Count > 0 || settings.AllowNotes)
                        {
                            XmlNode ndCol2 = ndCol.CloneNode(true);
                            ndCol2.Attributes["Name"].Value = "TS" + ndCol2.Attributes["Name"].Value;
                            ndCol2.Attributes["Type"].Value = "Text";
                            ndCol2.Attributes["Visible"].Value = "0";

                            ndRightCols.AppendChild(ndCol2);
                        }

                        //if(dsTypes.Tables[0].Rows.Count > 0)
                        //    ndCol.Attributes["CanEdit"].Value = "0";

                        ndRightCols.AppendChild(ndCol);


                        //Header
                        attr1 = docLayout.CreateAttribute("P" + dtStart.Ticks);
                        attr1.Value = dtStart.ToString("ddd<br>MMM dd");
                        ndHeader.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("P" + dtStart.Ticks + "Formula");
                        attr1.Value = "sum()";
                        ndGroupDef.Attributes.Append(attr1);

                        attr1 = docLayout.CreateAttribute("P" + dtStart.Ticks + "Type");
                        attr1.Value = "Float";
                        ndGroupDef.Attributes.Append(attr1);

                        TotalColumnCalc += "+" + "P" + dtStart.Ticks;

                        ndRDef.Attributes["CalcOrder"].Value += "," + "P" + dtStart.Ticks;
                        ndGroupDef.Attributes["CalcOrder"].Value += "," + "P" + dtStart.Ticks;

                        RightWidth += TSCellWidth;
                    }

                    cn.Close();

                    if (iGridType == 0)
                    {
                        using (SPSite rsite = new SPSite(web.Site.ID))
                        {
                            using (SPWeb rweb = rsite.OpenWeb(web.ID))
                            {
                                Guid lWebGuid = EPMLiveCore.CoreFunctions.getLockedWeb(rweb);
                                if (lWebGuid != rweb.ID)
                                {
                                    using (SPWeb lweb = rsite.OpenWeb(lWebGuid))
                                    {
                                        PopulateTimesheetGridLayout(lweb, ref docLayout, settings, ref MidWidth, viewInfo, false, "My Work");
                                    }
                                }
                                else
                                {
                                    PopulateTimesheetGridLayout(rweb, ref docLayout, settings, ref MidWidth, viewInfo, false, "My Work");
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

                ndRightCols.AppendChild(ndProgressCol);


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
                attr2.Value = TotalColumnCalc.Trim('+');
                ndTotalsCol.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("Format");
                attr2.Value = ",0.00";
                ndTotalsCol.Attributes.Append(attr2);

                ndRightCols.AppendChild(ndTotalsCol);

                attr2 = docLayout.CreateAttribute("TSTotals");
                attr2.Value = "Totals";
                ndTotalsCol.Attributes.Append(attr2);

                ndHeader.Attributes.Append(attr2);

                attr2 = docLayout.CreateAttribute("TSTotalsFormula");
                attr2.Value = TotalColumnCalc.Trim('+');
                ndGroupDef.Attributes.Append(attr2);




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


                ndGroupDef.Attributes["CalcOrder"].Value += ",TSTotals";


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
            string format = "";

            switch (oField.Type)
            {
                case SPFieldType.DateTime:
                    try
                    {

                        if (oDoc.FirstChild.Attributes["Format"].Value == "DateOnly")
                        {
                            format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
                        }
                        else
                        {
                            format = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.FullDateTimePattern;
                        }
                    }
                    catch { }
                    break;
                case SPFieldType.Number:
                    if (oDoc.FirstChild.Attributes["Percentage"] != null && oDoc.FirstChild.Attributes["Percentage"].Value.ToLower() == "true")
                    {
                        format = "0\\%;0\\%;0\\%";
                    }
                    else
                    {
                        int decCount = 0;
                        string decimals = "";
                        try
                        {
                            decCount = int.Parse(oDoc.FirstChild.Attributes["Decimals"].Value);
                        }
                        catch { }

                        for (int i = 0; i < decCount; i++)
                        {
                            decimals += "0";
                        }

                        if (decCount > 0)
                            decimals = "." + decimals;

                        format = ",0" + decimals;
                        break;
                    }
                    break;
                case SPFieldType.Currency:
                    SPFieldCurrency c = (SPFieldCurrency)oField;
                    System.Globalization.NumberFormatInfo nInfo = System.Globalization.CultureInfo.GetCultureInfo(c.CurrencyLocaleId).NumberFormat;
                    format = nInfo.CurrencySymbol + nInfo.CurrencyGroupSeparator + "0" + nInfo.CurrencyDecimalSeparator + "00";
                    break;
                case SPFieldType.Calculated:
                    switch (oDoc.FirstChild.Attributes["ResultType"].Value)
                    {
                        case "Currency":
                            format = oWeb.Locale.NumberFormat.CurrencySymbol + ",0.00";
                            break;
                        case "Number":
                            if (oDoc.FirstChild.Attributes["Percentage"] != null && oDoc.FirstChild.Attributes["Percentage"].Value.ToLower() == "true")
                            {
                                format = "0\\%;0\\%;0\\%";
                            }
                            else
                            {
                                int decCount = 0;
                                string decimals = "";
                                try
                                {
                                    decCount = int.Parse(oDoc.FirstChild.Attributes["Decimals"].Value);
                                }
                                catch { }

                                for (int i = 0; i < decCount; i++)
                                {
                                    decimals += "0";
                                }

                                if (decCount > 0)
                                    decimals = "." + decimals;

                                format = ",0" + decimals;
                            }
                            break;
                    };
                    break;

            };

            return format;
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

                string sql = string.Format(@"SELECT * FROM dbo.LSTResourcePool WHERE (',' + TimesheetManagerID + ',' LIKE '%,{0},%') and Generic=0 ANd (Disabled=0 or Disabled is NULL)", web.CurrentUser.ID);
                DataTable dtMyResources = rptData.ExecuteSql(sql);

                string sResList = "";

                foreach (DataRow dr in dtMyResources.Rows)
                {
                    sResList += ";#" + dr["SharePointAccountID"].ToString();
                }
                sResList = sResList.Trim(';').Trim('#');

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                    cn.Open();
                });

                DataSet ds = new DataSet();
                SqlCommand cmd = new SqlCommand("spTSGetMyApprovals", cn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
                cmd.Parameters.AddWithValue("@periodid", sPeriod);
                cmd.Parameters.AddWithValue("@resources", sResList);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                ArrayList arrPeriods = GetPeriodDaysArray(cn, settings, web, sPeriod);

                cn.Close();




                foreach (DataRow dr in dtMyResources.Rows)
                {
                    DataRow[] drTimesheets = ds.Tables[0].Select("USER_ID='" + dr["SharePointAccountId"].ToString() + "'");
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
                            iGetApprovalRow(dr, drTimesheet, ds.Tables[1], ref docData, ndB, arrPeriods, tsuid);
                        }
                    }
                    else if (Filter == "3")
                    {
                        if (drTimesheet != null)
                        {
                            if (drTimesheet["SUBMITTED"].ToString().ToLower() == "true" && drTimesheet["APPROVAL_STATUS"].ToString() == "0")
                            {
                                iGetApprovalRow(dr, drTimesheet, ds.Tables[1], ref docData, ndB, arrPeriods, tsuid);
                            }
                        }
                    }
                    else
                    {
                        iGetApprovalRow(dr, drTimesheet, ds.Tables[1], ref docData, ndB, arrPeriods, tsuid);
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
                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                    cn.Open();
                });


                DataSet dsTS = iGetTSData(cn, web, user, sPeriod);

                ArrayList arrPeriods = GetPeriodDaysArray(cn, settings, web, sPeriod);

                try
                {
                    cn.Close();
                }
                catch { }

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
            XmlDocument docTimesheet = new XmlDocument();
            docTimesheet.LoadXml(data);

            string tsuid = docTimesheet.FirstChild.Attributes["ID"].Value;

            SqlConnection cn = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                cn.Open();
            });

            bool submitted = false;

            int period = 0;

            SqlCommand cmd = new SqlCommand("SELECT submitted, period_id FROM TSTIMESHEET where TS_UID=@tsuid ", cn);
            cmd.Parameters.AddWithValue("@tsuid", tsuid);
            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                submitted = dr.GetBoolean(0);
                period = dr.GetInt32(1);
            }
            dr.Close();

            string sUserId = "";
            try
            {
                sUserId = docTimesheet.FirstChild.Attributes["UserId"].Value;
            }
            catch { }

            cmd = new SqlCommand("SELECT period_start,period_end FROM TSPERIOD WHERE SITE_ID=@siteid and PERIOD_ID=@periodid", cn);
            cmd.Parameters.AddWithValue("@siteid", oWeb.Site.ID);
            cmd.Parameters.AddWithValue("@periodid", period);
            dr = cmd.ExecuteReader();

            DateTime fn = DateTime.MinValue;
            DateTime st = DateTime.MaxValue;

            if (dr.Read())
            {
                st = dr.GetDateTime(0);
                fn = dr.GetDateTime(1);
            }
            dr.Close();

            if (!submitted)
            {
                EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(oWeb.Site.ID);

                ArrayList arrRows = new ArrayList();
                try
                {
                    if (docTimesheet.FirstChild.Attributes["Rows"] != null)
                    {
                        string[] sTempRows = docTimesheet.FirstChild.Attributes["Rows"].Value.ToLower().Split(',');
                        foreach (string sTempRow in sTempRows)
                        {
                            if (sTempRow != "" && !arrRows.Contains(sTempRow))
                                arrRows.Add(sTempRow);
                        }
                    }

                    DataSet dsTs = new DataSet();
                    cmd = new SqlCommand("SELECT list_uid,item_id FROM TSITEM WHERE TS_UID=@tsuid", cn);
                    cmd.Parameters.AddWithValue("@tsuid", tsuid);
                    dr = cmd.ExecuteReader();

                    while (dr.Read())
                    {
                        string s = dr.GetGuid(0).ToString() + "." + dr.GetInt32(1);
                        if (!arrRows.Contains(s))
                            arrRows.Add(s);
                    }
                    dr.Close();

                }
                catch { }

                SPUser user = GetUser(oWeb, sUserId);

                DataTable dtWork = rptData.ExecuteSql("SELECT * FROM lstmywork where Timesheet=1 and StartDate < '" + fn.ToString("s") + "' AND DueDate > '" + st.ToString("s") + "' AND AssignedToID='" + user.ID + "'");

                foreach (DataRow drWork in dtWork.Rows)
                {
                    if (!arrRows.Contains(drWork["ListId"].ToString().ToLower() + "." + drWork["ItemId"].ToString().ToLower()))
                        AddWorkItem(drWork, oWeb.Site, tsuid, Guid.NewGuid(), cn);
                }

                cn.Close();

                return "<AutoAddWork Status=\"0\"></AutoAddWork>";
            }
            else
            {
                cn.Close();

                return "<AutoAddWork Status=\"3\">Timesheet is submitted and cannot add work.</AutoAddWork>";
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
            sUserId = user.ID.ToString();

            SqlConnection cn = null;
            SPSecurity.RunWithElevatedPrivileges(delegate ()
            {
                cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                cn.Open();
            });


            DataSet ds = iGetTSData(cn, web, user, sPeriod);


            try
            {
                cn.Close();
            }
            catch { }

            return ds.GetXml();
        }

        private static DataSet iGetTSData(SqlConnection cn, SPWeb web, SPUser user, string sPeriod)
        {

            EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(web.Site.ID);

            SqlCommand cmd = new SqlCommand("select TOP 1 TS_UID from TSTIMESHEET where SITE_UID = @siteid and PERIOD_ID = @period and USERNAME = @username", cn);
            cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
            cmd.Parameters.AddWithValue("@period", sPeriod);
            cmd.Parameters.AddWithValue("@username", user.LoginName);

            Guid tsuid = Guid.Empty;

            SqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                tsuid = dr.GetGuid(0);
            }
            dr.Close();

            if (tsuid == Guid.Empty)
            {
                tsuid = iGenerateTSFromPast(cn, web, user, sPeriod, rptData);
            }

            cmd = new SqlCommand("SPTSSetUser", cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
            cmd.Parameters.AddWithValue("@username", user.LoginName);
            cmd.Parameters.AddWithValue("@name", user.Name);
            cmd.Parameters.AddWithValue("@userid", user.ID);
            cmd.ExecuteNonQuery();

            cmd = new SqlCommand("SELECT TSUSERUID FROM TSUSER WHERE USER_ID=@uid", cn);
            cmd.Parameters.AddWithValue("@uid", user.ID);
            Guid userid = Guid.Empty;

            dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                userid = dr.GetGuid(0);
            }
            dr.Close();

            cmd = new SqlCommand("UPDATE TSTIMESHEET SET TSUSER_UID=@uid where TS_UID=@tsuid", cn);
            cmd.Parameters.AddWithValue("@tsuid", tsuid);
            cmd.Parameters.AddWithValue("@uid", userid);
            cmd.ExecuteNonQuery();

            return iiGetTSData(cn, web, sPeriod, tsuid, rptData, Convert.ToString(user.ID));
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
                            string project = Regex.Replace(Convert.ToString(dtTSItem.Rows[0]["PROJECT"]), @"(\s+|@|&|'|\(|\)|<|>|#)", " ");
                            string projectID = Convert.ToString(dtTSItem.Rows[0]["PROJECT_ID"]) == "" ? "null" : Convert.ToString(dtTSItem.Rows[0]["PROJECT_ID"]);
                            sql = string.Format(@"select '" + Convert.ToString(dtTSItem.Rows[0]["SITE_UID"]) + "' SiteId,'" + Convert.ToString(dtTSItem.Rows[0]["WEB_UID"]) + "' WebId,'" + Convert.ToString(dtTSItem.Rows[0]["LIST_UID"]) + "' ListId," + Convert.ToString(dtTSItem.Rows[0]["ITEM_ID"]) + " ItemId,null WebUrl,null Commenters,null CommentersRead,null CommentCount,null WorkspaceUrl,null ID,null Title,null _UIVersionString,null Attachments,null ItemChildCountID,null ItemChildCountText,null FolderChildCountID,null FolderChildCountText,null AppAuthorID,null AppAuthorText,null AppEditorID,null AppEditorText," + projectID + " ProjectID, '" + project + "' ProjectText,null AssignedToID,null AssignedToText,null OwnerID,null OwnerText,null Status,null Priority,null Body,null ScheduleStatus,null PercentComplete,null Due,null StartDate,null ActualStart,null DueDate,null ActualFinish,null Work,null ActualWork,null RemainingWork,null TimesheetHours,null RemainingHours,null WorkPercentSpent,null WorkStatus,null taskorder,null TaskHierarchy,null Site,null DaysOverdue,null Complete,null Timesheet,0 IsAssignment,null ContentType,null Modified,null Created,null AuthorID,null AuthorText,null EditorID,null EditorText,'" + dtTSItem.Rows[0]["LIST"] + "' WorkType, null DataSource,'true' IsDeleted ");
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

        private static Guid iGenerateTSFromPast(SqlConnection cn, SPWeb web, SPUser user, string period, EPMLiveCore.ReportHelper.MyWorkReportData rptData)
        {
            Guid tsuid = Guid.NewGuid();

            SqlCommand cmd = new SqlCommand("select top 1 ts_uid from TSTIMESHEET where period_id < @period and site_uid=@siteid and username=@username order by period_id desc", cn);
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@username", user.LoginName);
            cmd.Parameters.AddWithValue("@siteid", web.Site.ID);

            Guid copyfromtsuid = Guid.Empty;

            SqlDataReader dr = cmd.ExecuteReader();

            if (dr.Read())
            {
                copyfromtsuid = dr.GetGuid(0);
            }
            dr.Close();


            cmd = new SqlCommand("INSERT INTO TSTIMESHEET (TS_UID, USERNAME, RESOURCENAME, PERIOD_ID, SITE_UID) VALUES (@tsuid, @username, @resourcename, @period, @siteid)", cn);
            cmd.Parameters.AddWithValue("@tsuid", tsuid);
            cmd.Parameters.AddWithValue("@period", period);
            cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
            cmd.Parameters.AddWithValue("@username", user.LoginName);
            cmd.Parameters.AddWithValue("@resourcename", user.Name);
            cmd.ExecuteNonQuery();

            if (copyfromtsuid != Guid.Empty)
            {



                cmd = new SqlCommand("SELECT * FROM TSITEM where TS_UID = @tsuid and item_type = 1", cn);
                cmd.Parameters.AddWithValue("@tsuid", copyfromtsuid);

                DataSet dsItems = new DataSet();
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dsItems);

                foreach (DataRow drItem in dsItems.Tables[0].Rows)
                {
                    try
                    {
                        string sql = string.Format(@"SELECT * FROM dbo.LSTMyWork WHERE Complete != 1 and Status != 'Completed' AND ([AssignedToID] = -99 or [AssignedToID] = " + user.ID + ") AND [SiteId] = N'{0}' AND LISTID = N'{1}' AND ITEMID=N'{2}'", web.Site.ID, drItem["LIST_UID"].ToString(), drItem["ITEM_ID"].ToString());
                        DataTable myWorkDataTable = rptData.ExecuteSql(sql);

                        if (myWorkDataTable.Rows.Count > 0)
                        {

                            if (myWorkDataTable.Rows[0]["Timesheet"].ToString() == "True")
                            {
                                cmd = new SqlCommand("INSERT INTO TSITEM (TS_UID, WEB_UID, LIST_UID, ITEM_ID, ITEM_TYPE, TITLE, PROJECT, PROJECT_ID, LIST, PROJECT_LIST_UID,AssignedToID) VALUES (@tsuid, @webuid, @listuid, @itemid, 1, @title, @project, @projectid, @list, @projectlistuid, @assignedtoid)", cn);
                                cmd.Parameters.AddWithValue("@tsuid", tsuid);
                                cmd.Parameters.AddWithValue("@webuid", drItem["WEB_UID"].ToString());
                                cmd.Parameters.AddWithValue("@listuid", drItem["LIST_UID"].ToString());
                                cmd.Parameters.AddWithValue("@itemid", drItem["ITEM_ID"].ToString());
                                cmd.Parameters.AddWithValue("@title", drItem["TITLE"].ToString());
                                cmd.Parameters.AddWithValue("@assignedtoid", user.ID);

                                if (drItem["PROJECT"].ToString() == "")
                                    cmd.Parameters.AddWithValue("@project", DBNull.Value);
                                else
                                    cmd.Parameters.AddWithValue("@project", drItem["PROJECT"].ToString());

                                if (drItem["PROJECT_ID"].ToString() == "")
                                    cmd.Parameters.AddWithValue("@projectid", DBNull.Value);
                                else
                                    cmd.Parameters.AddWithValue("@projectid", drItem["PROJECT_ID"].ToString());

                                cmd.Parameters.AddWithValue("@list", drItem["LIST"].ToString());

                                if (drItem["PROJECT_LIST_UID"].ToString() == "")
                                    cmd.Parameters.AddWithValue("@projectlistuid", DBNull.Value);
                                else
                                    cmd.Parameters.AddWithValue("@projectlistuid", drItem["PROJECT_LIST_UID"].ToString());

                                cmd.ExecuteNonQuery();

                            }

                        }
                    }
                    catch { }
                }


            }

            return tsuid;
        }

        private static bool iVerifyDelegate(SPWeb web, SPUser u)
        {
            //TODO: Delegates
            DataTable dt = EPMLiveCore.API.APITeam.GetResourcePool("<Data FilterField='SharePointAccount' FilterFieldValue='" + u.ID + ";#" + u.Name + "'/>", web);

            return true;

        }

        public static string GetWork(string data, SPWeb oWeb)
        {
            XmlDocument docOut = new XmlDocument();
            docOut.LoadXml(Properties.Resources.txtMyTimesheetWork_GridLayout);
            var currenvyCultureInfo = new CultureInfo(1033);

            try
            {

                XmlDocument docIn = new XmlDocument();
                docIn.LoadXml(data);

                bool bOtherWork = false;
                bool bNonWork = false;
                string TSID = docIn.FirstChild.Attributes["TSID"].Value;

                string SearchField = "";
                try
                {
                    SearchField = docIn.FirstChild.Attributes["SearchField"].Value;
                }
                catch { }

                string SearchText = "";
                try
                {
                    SearchText = System.Web.HttpUtility.UrlDecode(docIn.FirstChild.Attributes["SearchText"].Value).Trim();
                }
                catch { }

                try
                {
                    bOtherWork = bool.Parse(docIn.FirstChild.Attributes["OtherWork"].Value);
                }
                catch { }
                try
                {
                    bNonWork = bool.Parse(docIn.FirstChild.Attributes["NonWork"].Value);
                }
                catch { }



                XmlNode ndCfg = docOut.FirstChild.SelectSingleNode("//Cfg");

                int temp = 0;

                TimesheetSettings settings = new TimesheetSettings(oWeb);

                Dictionary<string, string> viewInfo = new Dictionary<string, string>();

                EPMLiveCore.API.ViewManager views = null;

                if (bNonWork)
                    views = GetNonWorkViews(oWeb);
                else
                    views = GetWorkViews(oWeb);

                foreach (KeyValuePair<string, Dictionary<string, string>> key in views.Views)
                {
                    try
                    {
                        if (key.Value["Default"].ToLower() == "true")
                        {
                            XmlAttribute attr = docOut.CreateAttribute("Group");
                            attr.Value = key.Value["Group"];
                            ndCfg.Attributes.Append(attr);

                            attr = docOut.CreateAttribute("Sort");
                            attr.Value = key.Value["Sort"];
                            ndCfg.Attributes.Append(attr);

                            viewInfo = key.Value;
                        }
                    }
                    catch { }
                }

                string sUser = "";

                ArrayList arrLookups = new ArrayList();

                DataSet dsCur = new DataSet();

                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {

                    SqlConnection cn = null;
                    SPSecurity.RunWithElevatedPrivileges(delegate ()
                    {
                        cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                        cn.Open();
                    });

                    SqlCommand cmd = new SqlCommand("SELECT USER_ID, PERIOD_ID FROM         dbo.TSTIMESHEET INNER JOIN dbo.TSUSER ON dbo.TSTIMESHEET.TSUSER_UID = dbo.TSUSER.TSUSERUID where TS_UID=@uid", cn);
                    cmd.Parameters.AddWithValue("@uid", TSID);
                    SqlDataReader drTS = cmd.ExecuteReader();

                    if (drTS.Read())
                    {
                        sUser = drTS.GetInt32(0).ToString();

                    }
                    drTS.Close();

                    cmd = new SqlCommand("SELECT * FROM TSITEM WHERE TS_UID=@tsuid", cn);
                    cmd.Parameters.AddWithValue("@tsuid", TSID);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dsCur);

                    cn.Close();

                    if (sUser == "")
                    {
                        sUser = "";
                        docOut.LoadXml("<Grid><IO Result=\"-1\" Message=\"Could not determine user\"/></Grid>");
                    }
                    else
                    {
                        SPUser user = GetUser(oWeb, sUser);

                        if (user.ID.ToString() != sUser)
                        {
                            sUser = "";
                            docOut.LoadXml("<Grid><IO Result=\"-1\" Message=\"User mismatch or access denied\"/></Grid>");
                        }
                    }


                    if (sUser != "")
                    {
                        string InputList = "My Work";
                        if (bNonWork)
                            InputList = settings.NonWorkList;

                        using (SPSite rsite = new SPSite(oWeb.Site.ID))
                        {
                            using (SPWeb rweb = rsite.OpenWeb(oWeb.ID))
                            {
                                Guid lWebGuid = EPMLiveCore.CoreFunctions.getLockedWeb(rweb);
                                if (lWebGuid != rweb.ID)
                                {
                                    using (SPWeb lweb = rsite.OpenWeb(lWebGuid))
                                    {
                                        PopulateTimesheetGridLayout(lweb, ref docOut, settings, ref temp, viewInfo, true, InputList);
                                    }
                                }
                                else
                                {
                                    PopulateTimesheetGridLayout(rweb, ref docOut, settings, ref temp, viewInfo, true, InputList);
                                }
                            }

                            try
                            {
                                SPList lstMyWork = rsite.RootWeb.Lists.TryGetList("My Work");

                                if (lstMyWork != null)
                                {
                                    foreach (SPField field in lstMyWork.Fields)
                                    {
                                        if (field.Type == SPFieldType.Lookup || field.Type == SPFieldType.User)
                                        {
                                            if (field.InternalName == SearchField)
                                                SearchField += "Text";

                                            arrLookups.Add(field.InternalName + "Text");

                                        }
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                });

                if (sUser != "")
                {


                    XmlNode ndBody = docOut.SelectSingleNode("//Body/B");

                    DataTable work = GetWorkDT(oWeb, bOtherWork, bNonWork, sUser, settings, SearchField, SearchText);

                    foreach (DataRow dr in work.Rows)
                    {

                        XmlNode ndRow = docOut.CreateNode(XmlNodeType.Element, "I", docOut.NamespaceURI);

                        XmlAttribute attr = docOut.CreateAttribute("WorkTypeField");

                        if (!bNonWork)
                            attr.Value = dr["WorkType"].ToString();
                        else
                            attr.Value = settings.NonWorkList;

                        ndRow.Attributes.Append(attr);

                        //if(bNonWork)
                        //{
                        //    attr = docOut.CreateAttribute("CanGroup");
                        //    attr.Value = "0";
                        //    ndRow.Attributes.Append(attr);
                        //}

                        attr = docOut.CreateAttribute("UID");
                        attr.Value = Guid.NewGuid().ToString();
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("Title");
                        attr.Value = dr["Title"].ToString();
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("SiteID");
                        attr.Value = oWeb.Site.ID.ToString();
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("WebID");
                        attr.Value = dr["WebID"].ToString();
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("ListID");
                        attr.Value = dr["ListID"].ToString();
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("ItemID");
                        attr.Value = dr["ItemID"].ToString();
                        ndRow.Attributes.Append(attr);



                        attr = docOut.CreateAttribute("TSEnabled");
                        attr.Value = "1";
                        ndRow.Attributes.Append(attr);

                        attr = docOut.CreateAttribute("ItemTypeID");
                        if (bNonWork)
                            attr.Value = "2";
                        else
                            attr.Value = "1";
                        ndRow.Attributes.Append(attr);

                        DataRow[] drCurrent = dsCur.Tables[0].Select("LIST_UID='" + dr["listid"].ToString() + "' and ITEM_ID='" + dr["itemid"].ToString() + "'");
                        if (drCurrent.Length > 0)
                        {
                            attr = docOut.CreateAttribute("Current");
                            attr.Value = "1";
                            ndRow.Attributes.Append(attr);
                        }

                        foreach (DataColumn dc in work.Columns)
                        {
                            string GoodFieldname = dc.ColumnName;

                            if (GoodFieldname.EndsWith("Type"))
                                GoodFieldname += GoodFieldname;

                            if (arrLookups.Contains(GoodFieldname))
                            {
                                GoodFieldname = GoodFieldname.Substring(0, GoodFieldname.Length - 4);
                            }

                            if (isValidMyWorkColumn(GoodFieldname))
                            {
                                attr = docOut.CreateAttribute(GoodFieldname);
                                if (GoodFieldname == "PercentComplete" && dr[dc.ColumnName] != DBNull.Value)
                                {
                                    attr.Value = Convert.ToString(Convert.ToDouble(dr[dc.ColumnName]) * 100, currenvyCultureInfo.NumberFormat);
                                }
                                else
                                {
                                    if (dc.DataType == typeof(Double) && dr[dc.ColumnName] != DBNull.Value)
                                    {
                                        attr.Value = Convert.ToString(Convert.ToDouble(dr[dc.ColumnName]), currenvyCultureInfo.NumberFormat);
                                    }
                                    else if (dc.DataType == typeof(DateTime) && !String.IsNullOrEmpty(Convert.ToString(dr[dc.ColumnName])))
                                    {
                                        attr.Value = DateTime.Parse(Convert.ToString(dr[dc.ColumnName])).ToString("u");
                                    }
                                    else
                                    {
                                        attr.Value = Convert.ToString(dr[dc.ColumnName]);
                                    }
                                }
                                ndRow.Attributes.Append(attr);
                            }
                        }

                        ndBody.AppendChild(ndRow);
                    }
                }
            }
            catch (Exception ex)
            {
                docOut.LoadXml(Properties.Resources.txtMyTimesheetWork_GridLayout);

                XmlNode ndBod = docOut.SelectSingleNode("//B");

                XmlNode ndCol = docOut.CreateNode(XmlNodeType.Element, "I", docOut.NamespaceURI);
                XmlAttribute attr1 = docOut.CreateAttribute("Title");
                attr1.Value = "Data Error: " + ex.Message;
                ndCol.Attributes.Append(attr1);

                ndBod.AppendChild(ndCol);
            }

            return docOut.OuterXml;
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

                SqlConnection cn = null;
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(oWeb.Site.WebApplication.Id));
                    cn.Open();
                });

                EPMLiveCore.ReportHelper.MyWorkReportData rptData = new EPMLiveCore.ReportHelper.MyWorkReportData(oWeb.Site.ID);

                string[] sItems = docTimesheet.FirstChild.InnerText.Split(',');


                foreach (string sItem in sItems)
                {
                    string[] ItemInfo = sItem.Split('.');

                    string sql = string.Format(@"SELECT top 1 * from dbo.LSTMyWork WHERE itemid={0} and listid='{1}'", ItemInfo[1], ItemInfo[0]);

                    DataTable dtWork = rptData.ExecuteSql(sql);

                    if (dtWork.Rows.Count > 0)
                    {
                        AddWorkItem(dtWork.Rows[0], oWeb.Site, tsuid, id, cn);
                    }
                }
                cn.Close();

                return "<AddWork Status=\"0\"></AddWork>";

            }
            catch (Exception ex)
            {
                return "<AddWork Status=\"1\">Error: " + ex.Message + "</AddWork>";
            }

        }
    }
}
