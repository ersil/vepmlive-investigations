﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using System.Data.SqlClient;
using System.Data;

namespace EPMLiveCore.Jobs
{
    public class ResourcePlan : API.BaseJob
    {
        private DataTable dtResLink;
        private DataTable dtResInfo;

        public void execute(SPSite site, SPWeb web, string data)
        {
            queuetype = 1;

            if(!initJob(site))
                return;

            string resPlanLists = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPMLiveResPlannerLists");
            if(resPlanLists.Trim() != "")
            {

                string sFixLists = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPMLiveFixLists");

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    cn.Open();
                });

                SqlCommand cmd = new SqlCommand("DELETE FROM RESINFO where siteid=@siteid", cn);
                cmd.Parameters.AddWithValue("@siteid", site.ID);
                cmd.ExecuteNonQuery();

                cmd = new SqlCommand("DELETE FROM RESLINK where siteid=@siteid or siteid in (select siteid from reslink where weburl=@weburl)", cn);
                cmd.Parameters.AddWithValue("@siteid", site.ID);
                cmd.Parameters.AddWithValue("@weburl", site.ServerRelativeUrl);
                cmd.ExecuteNonQuery();

                cn.Close();

                buildResPlanInfo();

                int hours = 0;
                string workdays = " ";
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    int startHour = site.RootWeb.RegionalSettings.WorkDayStartHour / 60;
                    int endHour = site.RootWeb.RegionalSettings.WorkDayEndHour / 60;
                    hours = endHour - startHour - 1;

                    int work = site.RootWeb.RegionalSettings.WorkDays;
                    for(byte x = 0; x < 7; x++)
                    {
                        workdays = ((((work >> x) & 0x01) == 0x01) ? "" : "," + (7 - x)) + workdays;
                    }
                });

                if(workdays.Length > 1)
                    workdays = workdays.Substring(1);

                float webCount = 0;
                base.totalCount = site.AllWebs.Count;

                foreach(SPWeb w in site.AllWebs)
                {
                    try
                    {
                        sErrors += "<br>Processing Web: " + w.Title + " (" + w.ServerRelativeUrl + ")";
                        processResPlan(w, resPlanLists, site.ID, hours, workdays);

                    }
                    catch { }
                    w.Close();
                    w.Dispose();

                    updateProgress(webCount++);
                }

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    cn.Open();
                });
                storeResPlanInfo();
                cn.Close();
            }

            finishJob();
        }

        private void processResPlan(SPWeb web, string resPlanLists, Guid siteId, int hours, string workdays)
        {
            string resurl = getResUrl(EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLiveResourceURL"));

            if(resurl.Trim() != "")
            {

                dtResLink.Rows.Add(new object[] { web.ServerRelativeUrl, resurl, siteId, workdays, hours });

                if(resPlanLists.Trim().Length > 0)
                {
                    string[] arLists = resPlanLists.Replace("\r\n", "\n").Split('\n');

                    foreach(string sList in arLists)
                    {
                        if(sList.Trim().Length > 0)
                        {
                            sErrors += "<br>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Processing: " + sList;
                            try
                            {
                                SPList list = web.Lists[sList];
                                SPQuery query = new SPQuery();
                                query.Query = "<Where><And><And><And><IsNotNull><FieldRef Name=\"StartDate\"/></IsNotNull><IsNotNull><FieldRef Name=\"DueDate\"/></IsNotNull></And><IsNotNull><FieldRef Name=\"Work\"/></IsNotNull></And><IsNotNull><FieldRef Name=\"AssignedTo\"/></IsNotNull></And></Where>";

                                foreach(SPListItem li in list.GetItems(query))
                                {
                                    string project = "";
                                    string assignedTo = "";

                                    try
                                    {
                                        project = li[list.Fields.GetFieldByInternalName("Project").Id].ToString();
                                        SPFieldLookupValue lv = new SPFieldLookupValue(project);
                                        project = lv.LookupValue;
                                    }
                                    catch { }

                                    try
                                    {
                                        assignedTo = li[list.Fields.GetFieldByInternalName("AssignedTo").Id].ToString();
                                    }
                                    catch { }

                                    SPFieldUserValueCollection uvc = new SPFieldUserValueCollection(web, assignedTo);
                                    foreach(SPFieldUserValue uv in uvc)
                                    {
                                        float work = 0;
                                        try
                                        {
                                            work = float.Parse(li[list.Fields.GetFieldByInternalName("Work").Id].ToString());
                                            work = work / uvc.Count;
                                        }
                                        catch { }
                                        dtResInfo.Rows.Add(new object[] { project, li.Title, uv.LookupValue, li[list.Fields.GetFieldByInternalName("StartDate").Id].ToString(), li[list.Fields.GetFieldByInternalName("DueDate").Id].ToString(), sList, li[list.Fields.GetFieldByInternalName("Status").Id].ToString(), work, siteId });
                                    }


                                }
                            }
                            catch(Exception ex)
                            {
                                if(ex.Message != "Value does not fall within the expected range.")
                                {
                                    bErrors = true;
                                    sErrors += "...<font color=\"red\">Error: " + ex.Message + "</font>";
                                }
                            }
                        }
                    }
                }
            }
        }

        private void buildResPlanInfo()
        {
            dtResInfo = new DataTable();
            dtResInfo.Columns.Add("Project");
            dtResInfo.Columns.Add("Title");
            dtResInfo.Columns.Add("AssignedTo");
            dtResInfo.Columns.Add("StartDate", typeof(DateTime));
            dtResInfo.Columns.Add("DueDate", typeof(DateTime));
            dtResInfo.Columns.Add("ItemType");
            dtResInfo.Columns.Add("Status");
            dtResInfo.Columns.Add("Work");
            dtResInfo.Columns.Add("SiteId", typeof(Guid));

            dtResLink = new DataTable();
            dtResLink.Columns.Add("weburl");
            dtResLink.Columns.Add("resurl");
            dtResLink.Columns.Add("siteid", typeof(Guid));
            dtResLink.Columns.Add("nonworkdays");
            dtResLink.Columns.Add("workhours");
        }

        private void storeResPlanInfo()
        {
            if(cn.State == ConnectionState.Open)
            {

                using(SqlBulkCopy sbc = new SqlBulkCopy(cn))
                {
                    sbc.DestinationTableName = "RESINFO";
                    // Number Of Records Processed In One Go 
                    int iRowCount = dtResInfo.Rows.Count;
                    if(iRowCount > 500)
                    {
                        iRowCount = 500;
                    }

                    sbc.BatchSize = iRowCount;
                    sbc.NotifyAfter = dtResInfo.Rows.Count;
                    sbc.WriteToServer(dtResInfo);
                    sbc.Close();

                }

                using(SqlBulkCopy sbc = new SqlBulkCopy(cn))
                {
                    sbc.DestinationTableName = "RESLINK";
                    // Number Of Records Processed In One Go 
                    int iRowCount = dtResLink.Rows.Count;
                    if(iRowCount > 500)
                    {
                        iRowCount = 500;
                    }

                    sbc.BatchSize = iRowCount;
                    sbc.NotifyAfter = dtResLink.Rows.Count;
                    sbc.WriteToServer(dtResLink);
                    sbc.Close();

                }
            }
        }

        private string getResUrl(string resUrl)
        {
            try
            {
                //resUrl = resUrl.Substring(resUrl.IndexOf("/", resUrl.IndexOf("//")+2));
                if(resUrl.StartsWith("/"))
                    return resUrl;

                System.Uri u = new Uri(resUrl);
                resUrl = u.AbsolutePath;
            }
            catch { }

            return resUrl;
        }
    }
}
