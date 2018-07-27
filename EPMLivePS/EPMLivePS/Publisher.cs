using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.SharePoint;
using PSLibrary = Microsoft.Office.Project.Server.Library;

namespace EPMLiveEnterprise
{
    public class Publisher
    {
        private const string DotDelimiter = ".";
        private const string EpmLiveKey = "EPM Live";
        private const string PublisherKey = "Publisher";

        private const string ProjectGuidParam = "@projectguid";
        private const string LogTextParam = "@logtext";
        private const string StatusParam = "@status";
        private const string ProjectNameParam = "@projectname";
        private const string PubTypeParam = "@pubtype";
        private const string TransUidParam = "@transuid";
        private const string WebGuidParam = "@webguid";
        private const string WebUrlParam = "@weburl";

        private EventLog myLog = CreateWindowsEvent(EpmLiveKey, PublisherKey);
        private Guid taskEntity = new Guid(PSLibrary.EntityCollection.Entities.TaskEntity.UniqueId);

        private WebSvcCustomFields.CustomFields pCf;
        private WebSvcResource.Resource pResource;
        private WebSvcProject.Project pService;
        private WebSvcCustomFields.CustomFieldDataSet cfDs;
        private WebSvcLookupTables.LookupTable psiLookupTable;
        private WebSvcStatusing.Statusing Statusing;
        private WebSvcWssInterop.WssInterop pWssInterop;

        private string publishSiteUrl;
        private string projectServerUrl;
        private SPSite mySite;
        private SPWeb mySiteToPublish;
        private Guid projectGuid;
        private Guid mySiteGuid;
        private SqlConnection cn;
        private Hashtable hshCurTasks;
        private ArrayList arrDelTasks;
        private ArrayList arrDelNewTasks;
        private DataTable dtFieldsToPublish;
        private ArrayList arrFieldsToPublish;
        private ArrayList arrPJFieldsToPublish;
        private Hashtable hshTaskCenterFields;
        private Hashtable hshProjectCenterFields;
        private Hashtable hshTaskHierarchy;
        private ProjectWorkspaceSynch workspaceSynch;
        private Microsoft.Office.Project.Server.Library.PSContextInfo contextInfo;
        private Microsoft.Office.Project.Server.Events.ProjectPostPublishEventArgs eventArgs;
        private Guid lastTransUid = new Guid();

        private WebSvcResource.ResourceDataSet rDs;
        private WebSvcCustomFields.CustomFieldDataSet dsFields;
        private WebSvcLookupTables.LookupTableDataSet dsLt;

        private string strTimesheetField = "";

        private StringBuilder sbErrors = new StringBuilder();

        private double pctComplete = 0;

        public Publisher(Microsoft.Office.Project.Server.Library.PSContextInfo c, Microsoft.Office.Project.Server.Events.ProjectPostPublishEventArgs e)
        {
            contextInfo = c;
            eventArgs = e;
            mySiteGuid = contextInfo.SiteGuid;
            myLog.EntryWritten += new EntryWrittenEventHandler(myLog_EntryWritten);
        }

        void myLog_EntryWritten(object sender, EntryWrittenEventArgs e)
        {
            string message = e.Entry.Message;
            sbErrors.Append(message.Replace("\r\n","<br>") + "<br><br><br><br>");
        }

        ~Publisher()
        {
            myLog.Close();
        }

        private static EventLog CreateWindowsEvent(string logName, string source)
        {
            using (var eventLog = new EventLog(logName))
            {
                eventLog.Source = $"{EpmLiveKey} {source}";
                eventLog.MachineName = DotDelimiter;

                return eventLog;
            }
        }

        public void doPublish()
        {
            try
            {
                DateTime dtStart = DateTime.Now;

                arrFieldsToPublish = new ArrayList();
                arrPJFieldsToPublish = new ArrayList();
                hshCurTasks = new Hashtable();
                arrDelTasks = new ArrayList();
                hshTaskCenterFields = new Hashtable();
                hshProjectCenterFields = new Hashtable();
                arrDelNewTasks = new ArrayList();

                mySite = new SPSite(mySiteGuid);

                cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(mySite.WebApplication.Id));
                cn.Open();

                ExecuteCommandWithProjectGuid(
                    cn, "UPDATE publishercheck set percentcomplete=2,laststatusdate=getdate() where projectguid=@projectguid", eventArgs.ProjectGuid);

                InitializeTimesheetField();

                InsertPublisherCheck(cn, eventArgs.ProjectGuid, mySite.RootWeb, mySite.Url);

                SqlCommand cmd;
                SqlDataReader dr;

                cmd = new SqlCommand("select pubType,weburl,transuid from publishercheck where projectguid=@projectguid", cn);
                cmd.Parameters.AddWithValue(ProjectGuidParam, eventArgs.ProjectGuid);
                dr = cmd.ExecuteReader();

                if (dr.Read())
                {
                    int pubType = dr.GetInt32(0);
                    publishSiteUrl = System.Web.HttpUtility.UrlDecode(dr.GetString(1));


                    if (!dr.IsDBNull(2))
                        lastTransUid = dr.GetGuid(2);

                    dr.Close();

                    if (publishSiteUrl == "")
                    {
                        publishSiteUrl = getProjectWss(mySite.Url, eventArgs.ProjectGuid);

                        using (var command = new SqlCommand("UPDATE publishercheck set weburl=@weburl where projectguid=@projectguid", cn))
                        {
                            command.Parameters.AddWithValue(ProjectGuidParam, eventArgs.ProjectGuid);
                            command.Parameters.AddWithValue(WebUrlParam, publishSiteUrl);
                            command.ExecuteNonQuery();
                        }
                    }

                    projectServerUrl = mySite.Url;
                    projectGuid = eventArgs.ProjectGuid;

                    hshCurTasks = new Hashtable();
                    hshTaskHierarchy = new Hashtable();
                    //int indSlash = publishSiteUrl.IndexOf("/", 9);
                    //publishSiteUrl = publishSiteUrl.Substring(indSlash);
                    //mySiteToPublish = mySite.OpenWeb(publishSiteUrl);
                    using (SPSite pubSite = new SPSite(publishSiteUrl))
                    {
                        mySiteToPublish = pubSite.OpenWeb();

                        //System.Web.HttpUtility.UrlDecode(publishSiteUrl).ToLower().Replace(System.Web.HttpUtility.UrlDecode(mySite.Url.ToLower()), "").Substring(1));

                        pCf = new WebSvcCustomFields.CustomFields();
                        pCf.Url = mySite.Url + "/_vti_bin/psi/customfields.asmx";
                        pCf.UseDefaultCredentials = true;
                        cfDs = new WebSvcCustomFields.CustomFieldDataSet();

                        psiLookupTable = new WebSvcLookupTables.LookupTable();
                        psiLookupTable.Url = mySite.Url + "/_vti_bin/psi/lookuptable.asmx";
                        psiLookupTable.UseDefaultCredentials = true;

                        pResource = new WebSvcResource.Resource();
                        pResource.Url = mySite.Url + "/_vti_bin/psi/resource.asmx";
                        pResource.UseDefaultCredentials = true;

                        pService = new WebSvcProject.Project();
                        pService.Url = mySite.Url + "/_vti_bin/psi/project.asmx";
                        pService.UseDefaultCredentials = true;

                        Statusing = new WebSvcStatusing.Statusing();
                        Statusing.Url = mySite.Url + "/_vti_bin/psi/statusing.asmx";
                        Statusing.UseDefaultCredentials = true;

                        pWssInterop = new WebSvcWssInterop.WssInterop();
                        pWssInterop.Url = mySite.Url + "/_vti_bin/psi/wssinterop.asmx";
                        pWssInterop.UseDefaultCredentials = true;

                        linkProjectWss();

                        Guid trackingGuid = Guid.NewGuid();
                        string lcid = "1033";
                        StatusingDerived.SetImpersonationContext(true, contextInfo.UserName, contextInfo.UserGuid, trackingGuid, contextInfo.SiteGuid, lcid);

                        workspaceSynch = new EPMLiveEnterprise.ProjectWorkspaceSynch(mySiteGuid, publishSiteUrl, projectGuid, contextInfo.UserName);
                        workspaceSynch.setUpGroups();
                        workspaceSynch.processTaskCenter();
                        workspaceSynch.processProjectCenter();
                        workspaceSynch.processResources();

                        ExecuteCommandWithProjectGuid(
                            cn, "UPDATE publishercheck set percentcomplete=5,laststatusdate=getdate() where projectguid=@projectguid", eventArgs.ProjectGuid);

                        loadFields();

                        if (loadCurrentTasks(eventArgs.ProjectName))
                        {
                            Guid newTransUid = Guid.NewGuid();

                            using (var command = new SqlCommand("SELECT * from CUSTOMFIELDS where visible=1", cn))
                            {
                                using (var dataAdapter = new SqlDataAdapter(command))
                                {
                                    var dataSet = new DataSet();
                                    dataAdapter.Fill(dataSet);
                                    dtFieldsToPublish = dataSet.Tables[0];
                                }
                            }

                            WebSvcProject.ProjectDataSet pDs = new WebSvcProject.ProjectDataSet();
                            SPSecurity.RunWithElevatedPrivileges(delegate ()
                            {
                                pDs = pService.ReadProject(projectGuid, WebSvcProject.DataStoreEnum.PublishedStore);
                                rDs = pResource.ReadResources("", false);
                                dsFields = pCf.ReadCustomFieldsByEntity(new Guid(PSLibrary.EntityCollection.Entities.TaskEntity.UniqueId));
                                dsLt = psiLookupTable.ReadLookupTables("", false, 0);
                            });

                            //SPUser user = mySiteToPublish.AllUsers[getResourceUsername(pDs.Project[0].ProjectOwnerID)];
                            //SPUserToken token = user.UserToken;

                            //mySite = new SPSite(mySite.ID, token);
                            int projectId = publishProjectCenter(pDs);
                            if (projectId != 0)
                            {
                                publishTasks(projectId, pubType, newTransUid, lastTransUid);
                                AssignGroupsToTasks(projectId, pubType, pDs);
                            }

                            ProcessPFEWork(projectId);

                            int status = 2;

                            DateTime dtFinish = DateTime.Now;
                            TimeSpan ts = dtFinish - dtStart;

                            if (sbErrors.ToString() != "")
                            {
                                status = 3;
                                sbErrors.Append("<br><br>");
                            }

                            sbErrors.Append("Publishing Time: " + ts.TotalSeconds.ToString("#.0") + " seconds.");

                            const string UpdatePublisherCheck =
                                "update publishercheck set webguid=@webguid,logtext=@logtext, checkbit=0,transuid=@transuid,status=@status,percentcomplete=0,laststatusdate=getdate() where projectguid=@projectguid";

                            using (var command = new SqlCommand(UpdatePublisherCheck, cn))
                            {
                                command.Parameters.AddWithValue(ProjectGuidParam, eventArgs.ProjectGuid);
                                command.Parameters.AddWithValue(WebGuidParam, mySiteToPublish.ID);
                                command.Parameters.AddWithValue(TransUidParam, newTransUid);
                                command.Parameters.AddWithValue(StatusParam, status);
                                command.Parameters.AddWithValue(LogTextParam, sbErrors.ToString());
                                command.ExecuteNonQuery();
                            }

                            mySiteToPublish.Close();
                        }
                    }
                }
                else
                    dr.Close();
                cn.Close();
            }
            catch (System.Web.Services.Protocols.SoapException ex1)
            {
                myLog.WriteEntry("Soap Error: " + ex1.Message + ex1.Detail, EventLogEntryType.Error, 302);
                if (cn != null && cn.State == ConnectionState.Open)
                {
                    UpdatePublisherCheck(cn, eventArgs.ProjectGuid, "Soap Error", $"{ex1.Message}{ex1.StackTrace}");
                }
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 300);
                if (cn != null && cn.State == ConnectionState.Open)
                {
                    UpdatePublisherCheck(cn, eventArgs.ProjectGuid, "Error", $"{ex.Message}{ex.StackTrace}");
                }
            }
        }

        private void InitializeTimesheetField()
        {
            using (var command = new SqlCommand("SELECT config_value FROM ECONFIG where config_name='TimesheetField'", cn))
            {
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        strTimesheetField = reader.GetString(0);
                    }
                }
            }
        }

        private void InsertPublisherCheck(SqlConnection connection, Guid guid, SPWeb rootWeb, string url)
        {
            using (var command = new SqlCommand("select pubType,weburl,transuid from publishercheck where projectguid=@projectguid", connection))
            {
                command.Parameters.AddWithValue(ProjectGuidParam, guid);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.Read())
                    {
                        var pubtype = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "EPMLivePub-Type");

                        if (!string.IsNullOrWhiteSpace(pubtype))
                        {
                            var wssUrl = getProjectWss(url, guid);

                            if (!string.IsNullOrWhiteSpace(wssUrl))
                            {
                                ExecuteNonQueryOnPublisherCheck(cn, pubtype, wssUrl);
                            }
                        }
                    }
                }
            }
        }

        private void ExecuteNonQueryOnPublisherCheck(SqlConnection connection, string pubtype, string wssUrl)
        {
            const string InsertCommand =
                "INSERT INTO publishercheck (projectguid,checkbit,pubType,weburl, projectname,percentcomplete,status,laststatusdate) VALUES (@projectguid,1,@pubtype,@weburl,@projectname,2,1,GETDATE())";

            using (var sqlCommand = new SqlCommand(InsertCommand, connection))
            {
                sqlCommand.Parameters.AddWithValue(ProjectGuidParam, eventArgs.ProjectGuid);
                sqlCommand.Parameters.AddWithValue(PubTypeParam, pubtype);
                sqlCommand.Parameters.AddWithValue(WebUrlParam, wssUrl);
                sqlCommand.Parameters.AddWithValue(ProjectNameParam, eventArgs.ProjectName);
                sqlCommand.ExecuteNonQuery();
            }
        }

        private void ExecuteCommandWithProjectGuid(SqlConnection connection, string commandText, Guid guid)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue(ProjectGuidParam, guid);
                command.ExecuteNonQuery();
            }
        }

        private void UpdatePublisherCheck(SqlConnection connection, Guid guid, string errorType, string message)
        {
            const string commandText =
                "update publishercheck set logtext=@logtext, checkbit=0,status=4,percentcomplete=0,laststatusdate=getdate() where projectguid=@projectguid";

            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue(ProjectGuidParam, guid);
                command.Parameters.AddWithValue(LogTextParam, $"{errorType}: {message}");
                command.ExecuteNonQuery();
            }
        }

        private void linkProjectWss()
        {
            WebSvcWssInterop.WssSettingsDataSet dsCurrentWssInfo = new WebSvcWssInterop.WssSettingsDataSet();
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                dsCurrentWssInfo = pWssInterop.ReadWssSettings();
            });
            WebSvcWssInterop.WssSettingsDataSet.WssAdminRow adminRow = dsCurrentWssInfo.WssAdmin[0];

            Guid serverUid = adminRow.WADMIN_CURRENT_STS_SERVER_UID;

            string sUrl = publishSiteUrl;
            int slash = sUrl.IndexOf("/", 9);
            sUrl = sUrl.Substring(slash + 1);

            try
            {
                try
                {
                    //pService.UpdateProjectWorkspaceAddress(projectGuid, sUrl, serverUid);
                }
                catch
                {
                    
                    SqlConnection cn = new SqlConnection(EPMLiveHelperClasses.getProjectServerPublishedConnectionString(mySiteGuid));
                    cn.Open();

                    SqlCommand cmd = new SqlCommand("UPDATE MSP_PROJECTS set WPROJ_STS_SUBWEB_NAME=@web,WSTS_SERVER_UID=@sts,PROJ_READ_COUNT=0 where PROJ_UID=@projuid", cn);
                    cmd.Parameters.AddWithValue("@web", sUrl);
                    cmd.Parameters.AddWithValue("@sts", serverUid);
                    cmd.Parameters.AddWithValue("@projuid", projectGuid);
                    //cmd.ExecuteNonQuery();

                    cn.Close();

                }

            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error in linkProjectWss(): " + ex.Message + ex.StackTrace + ex.InnerException, EventLogEntryType.Error, 315);
            }
        }


        private string getProjectWss(string pwaUrl, Guid projectUID)
        {


            try
            {
                WebSvcWssInterop.WssInterop wssInterop = new WebSvcWssInterop.WssInterop();
                wssInterop.Url = pwaUrl + "/_vti_bin/psi/wssinterop.asmx";
                wssInterop.Credentials = System.Net.CredentialCache.DefaultCredentials;

                WebSvcWssInterop.ProjectWSSInfoDataSet ds = new WebSvcWssInterop.ProjectWSSInfoDataSet();

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    ds = wssInterop.ReadWssData(projectUID);
                });

                if (ds.ProjWssInfo.Count > 0)
                {
                    return ds.ProjWssInfo[0].PROJECT_WORKSPACE_URL;
                }
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error in getProjectWss(): " + ex.Message + ex.StackTrace + ex.InnerException, EventLogEntryType.Error, 315);
            }
            return "";
        }

        private bool canDelete(string fieldname)
        {
            if (fieldname == "LastPublished" || fieldname == "ContentType" || fieldname == "Modified" || fieldname == "Publisher_x0020_Approval_x0020_C" || fieldname == "Created" || fieldname == "taskorder" || fieldname == "Title" || fieldname == "WBS" || fieldname == "Publisher_x0020_Approval_x0020_S" || fieldname == "_UIVersionString")
                return false;
            return true;
        }

        private void setPubPercent(double total, double count)
        {
            double pct = count / total * 95 + 5;
            if (pct > pctComplete + 10)
            {
                SqlCommand cmd = new SqlCommand("UPDATE publishercheck set percentcomplete=@pct,laststatusdate=getdate() where projectguid=@projectguid", cn);
                cmd.Parameters.AddWithValue("@projectguid", eventArgs.ProjectGuid);
                cmd.Parameters.AddWithValue("@pct", pct);
                cmd.ExecuteNonQuery();
                pctComplete = pct;
            }
        }

        private void publishTasks(int projectId, int pubType, Guid newTransUid, Guid lastTransUid)
        {
            
            try
            {
                WebSvcProject.ProjectDataSet pDs = null;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    pDs = pService.ReadProject(projectGuid, WebSvcProject.DataStoreEnum.PublishedStore);
                });

                int counter = 0;
                StringBuilder sbBatch = new StringBuilder();

                ArrayList arrProcessedTasks = new ArrayList();

                SPList taskCenter = mySiteToPublish.Lists["Task Center"];

                double taskCount = pDs.Task.Count;
                double taskDoneCount = 0;

                if (pubType == 1)
                {
                    taskCount += pDs.Assignment.Count;

                    foreach (WebSvcProject.ProjectDataSet.AssignmentRow assn in pDs.Assignment)
                    {
                        counter++;

                        arrProcessedTasks.Add(assn.TASK_UID.ToString());
                        SPListItem listItem;
                        if (!IsAssignedTaskSaved(assn.ASSN_UID))
                        {
                            if (hshCurTasks.Contains(assn.TASK_UID.ToString().ToUpper() + "." + assn.ASSN_UID.ToString().ToUpper()))
                            {
                                Guid LIUid = (Guid)hshCurTasks[assn.TASK_UID.ToString().ToUpper() + "." + assn.ASSN_UID.ToString().ToUpper()];
                                listItem = taskCenter.GetItemByUniqueId(LIUid);

                                hshCurTasks.Remove(assn.TASK_UID.ToString().ToUpper() + "." + assn.ASSN_UID.ToString().ToUpper());
                            }
                            else
                            {
                                listItem = taskCenter.Items.Add();
                            }

                            listItem[listItem.Fields.GetFieldByInternalName("IsAssignment").Id] = "1";
                            listItem["Project"] = projectId;

                            processAssignment(assn, pDs, listItem);
                        }
                        else
                        {
                            hshCurTasks.Remove(assn.TASK_UID.ToString().ToUpper() + "." + assn.ASSN_UID.ToString().ToUpper());
                        }
                        taskDoneCount++;
                        setPubPercent(taskCount, taskDoneCount);
                    }
                }

                foreach (WebSvcProject.ProjectDataSet.TaskRow tr in pDs.Task)
                {
                    if (tr.TASK_ID != 0 && !tr.IsTASK_NAMENull() && !tr.TASK_IS_SUBPROJ)
                    {
                        //if (!arrProcessedTasks.Contains(tr.TASK_UID.ToString()) || pubType == 1)
                        {
                            try
                            {
                                SPListItem listItem;

                                if (hshCurTasks.Contains(tr.TASK_UID.ToString().ToUpper()))
                                {
                                    Guid LIUid = (Guid)hshCurTasks[tr.TASK_UID.ToString().ToUpper()];
                                    listItem = taskCenter.GetItemByUniqueId(LIUid);

                                    hshCurTasks.Remove(tr.TASK_UID.ToString().ToUpper());
                                }
                                else
                                {
                                    //sbBatch.Append("<Method ID='" + counter + "' Cmd='New'>");
                                    listItem = taskCenter.Items.Add();
                                }
                                

                                listItem["Project"] = projectId;
                                if (pubType == 1)
                                {
                                    listItem["AssignedTo"] = "";
                                    if (listItem.ParentList.Fields.ContainsField("ResourceNames"))
                                    {
                                        string resources = "";
                                        foreach (WebSvcProject.ProjectDataSet.AssignmentRow assn in pDs.Assignment.Select("TASK_UID='" + tr.TASK_UID + "'"))
                                        {
                                            resources += ", " + getResourceName(assn.RES_UID, pDs);
                                        }

                                        if (resources.Length > 2)
                                            resources = resources.Substring(2);
                                        listItem[listItem.ParentList.Fields.GetFieldByInternalName("ResourceNames").Id] = resources;
                                    }
                                }
                                if (pubType == 2)//Task Based Publishing With Assignments
                                {
                                    string assns = "";
                                    string resources = "";
                                    foreach (WebSvcProject.ProjectDataSet.AssignmentRow assn in pDs.Assignment.Select("TASK_UID='" + tr.TASK_UID + "'"))
                                    {
                                        resources += ", " + getResourceName(assn.RES_UID, pDs);
                                        int resId = getResourceWssId(assn.RES_UID_OWNER);
                                        if(resId != 0)
                                            assns = assns + ";#" + resId + ";#" + getResourceName(assn.RES_UID_OWNER, pDs);
                                    }
                                    if (assns.Length > 2)
                                        assns = assns.Substring(2);

                                    if (resources.Length > 2)
                                        resources = resources.Substring(2);

                                    listItem["AssignedTo"] = assns;
                                    if (listItem.ParentList.Fields.ContainsField("ResourceNames"))
                                        listItem[listItem.ParentList.Fields.GetFieldByInternalName("ResourceNames").Id] = resources;
                                }
                                else if (pubType == 3)//Task Based Publishing Without Assignments
                                {
                                    int assn = workspaceSynch.getResourceIdForTask(tr.TASK_UID, pDs);
                                    if (assn != 0)
                                        listItem["AssignedTo"] = assn;
                                    else
                                        listItem["AssignedTo"] = "";
                                }
                                
                                bool process = true;

                                try
                                {
                                    if (pubType == 2 || pubType == 3 && lastTransUid != new Guid())
                                        if (listItem["transuid"].ToString() != lastTransUid.ToString())
                                            process = false;
                                }
                                catch { }

                                if (process)
                                {
                                    listItem[listItem.Fields.GetFieldByInternalName("TaskHierarchy").Id] = getHierarchy(pDs, tr.TASK_PARENT_UID);
                                    listItem[listItem.Fields.GetFieldByInternalName("IsAssignment").Id] = "0";
                                    listItem["transuid"] = newTransUid.ToString();
                                    listItem[taskCenter.Fields.GetFieldByInternalName("Title").Id] = tr.TASK_NAME;
                                    if(!tr.IsTASK_WBSNull())
                                        listItem[taskCenter.Fields.GetFieldByInternalName("WBS").Id] = tr.TASK_WBS;
                                    listItem[taskCenter.Fields.GetFieldByInternalName("taskuid").Id] = tr.TASK_UID;
                                    listItem[taskCenter.Fields.GetFieldByInternalName("taskorder").Id] = tr.TASK_ID;
                                    listItem["Summary"] = tr.TASK_IS_SUMMARY.ToString();
                                    if (!tr.IsTASK_NOTESNull())
                                        listItem[taskCenter.Fields.GetFieldByInternalName("Notes").Id] = tr.TASK_NOTES;
                                    listItem[taskCenter.Fields.GetFieldByInternalName("LastPublished").Id] = DateTime.Now; //lp.Year.ToString() + "-" + lp.Month.ToString() + "-" + lp.Day.ToString() + " " + lp.Hour.ToString() + ":" + lp.Minute.ToString() + ":" + lp.Second.ToString();
                                    //listItem.Update();
                                    //listItem = taskCenter.Items.GetItemById(listItem.ID);
                                    processTask(tr, pDs, arrFieldsToPublish, listItem, hshTaskCenterFields, tr.TASK_UID.ToString(), pubType);
                                }
                            }
                            catch (Exception ex)
                            {
                                myLog.WriteEntry("Error in publishTasks(" + tr.TASK_NAME + ") updating list item: " + ex.Message + ex.StackTrace + ex.InnerException, EventLogEntryType.Error, 315);
                            }
                        }
                    }
                    taskDoneCount++;
                    setPubPercent(taskCount, taskDoneCount);
                }

                foreach (DictionaryEntry e in hshCurTasks)
                {
                    SPListItem li = taskCenter.GetItemByUniqueId(new Guid(e.Value.ToString()));
                    li.Delete();
                }

                foreach (Guid liguid in arrDelNewTasks)
                {
                    SPListItem li = taskCenter.GetItemByUniqueId(liguid);
                    li.Delete();
                }

            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error in publishTasks(): " + ex.Message + ex.StackTrace + ex.InnerException, EventLogEntryType.Error, 315);
            }
        }

        private int publishProjectCenter(WebSvcProject.ProjectDataSet pDs)
        {
            try
            {
                SPList lProjectCenter = mySiteToPublish.Lists["Project Center"];
                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>" + pDs.Project[0].PROJ_NAME + "</Value></Eq></Where>";

                SPListItem listItem;

                if (lProjectCenter.GetItems(query).Count > 0)
                {
                    listItem = lProjectCenter.GetItems(query)[0];
                    try
                    {
                        if (listItem["IsProjectServer"].ToString() != "True")
                        {
                            return 0;                                                        
                        }
                    }catch{}
                }
                else
                {
                    listItem = lProjectCenter.Items.Add();
                    listItem["IsProjectServer"] = "1";
                }

                WebSvcProject.ProjectDataSet.TaskRow taskRow = (WebSvcProject.ProjectDataSet.TaskRow)pDs.Task.Select("TASK_ID=0")[0];

                listItem[lProjectCenter.Fields.GetFieldByInternalName("Title").Id] = pDs.Project[0].PROJ_NAME;
                listItem["projectguid"] = pDs.Project[0].PROJ_UID.ToString();
                listItem["PublishToPWA"] = "0";
                listItem["SharePointProject"] = "0";
                listItem["IsProjectServer"] = "1";
                listItem["ProjectServerURL"] = projectServerUrl;
                try
                {
                    listItem[lProjectCenter.Fields.GetFieldByInternalName("AssignedTo").Id] = getResourceWssId(pDs.Project[0].ProjectOwnerID);
                }
                catch { }
                processProjectCenterFields(pDs, arrPJFieldsToPublish, ref listItem);
                processTask(taskRow, pDs, arrPJFieldsToPublish, listItem, hshProjectCenterFields, "", 2);

                hshTaskHierarchy.Add(taskRow.TASK_UID, "");

                return listItem.ID;
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error in publishProjectCenter(): " + ex.Message + ex.StackTrace + ex.InnerException, EventLogEntryType.Error, 315);
            }
            return 0;
        }

        /// <summary>
        /// This method used to insert the data into EPMLive PFE.
        /// </summary>
        /// <param name="projectId">project id</param>
        private void ProcessPFEWork(int projectId)
        {
            try
            {
                WebSvcProject.ProjectDataSet pDs = null;
                string lstName = "Project Center";

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    pDs = pService.ReadProject(projectGuid, WebSvcProject.DataStoreEnum.PublishedStore);
                });

                string str = "<UpdateScheduledWork><Params Worktype=\"1\"/><Data><Project ID='" + projectId + "' List='" + lstName + "'>";

                foreach (WebSvcProject.ProjectDataSet.AssignmentRow assn in pDs.Assignment)
                {
                    Int64 resId = 0;
                    resId = getResourceResId(assn.RES_UID_OWNER);
                    if (resId > 0)
                    {
                        str += "<Resource Id=\"" + resId + "\">";

                        DateTime assnSTDate = assn.ASSN_START_DATE;
                        DateTime assnFDate = assn.ASSN_FINISH_DATE;
                        var interavl = (assnFDate - assnSTDate).Days;
                        var hour = assn.ASSN_WORK;
                        DateTime loopStDate = assnSTDate;

                        while (loopStDate <= assnFDate)
                        {
                            var indvHour = ((hour / (interavl + 1)) / assn.ASSN_UNITS) / 6;  // logically devide the hours to each day (Right now it equally seprate hours to each day).
                            str += "<Work Date=\"" + DateTime.Parse(loopStDate.ToString()).ToString("s") + "\" Hours=\"" + indvHour + "\"/>";
                            loopStDate = loopStDate.AddDays(1);
                        }

                        str += "</Resource>";
                    }
                }

                str += "</Project></Data></UpdateScheduledWork>";

                //calling portfolioengineAPI
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    EPMLivePortfolioEngine.PortfolioEngineAPI pfe = new EPMLivePortfolioEngine.PortfolioEngineAPI();
                    pfe.Url = mySiteToPublish.Url + "/_vti_bin/portfolioengine.asmx";
                    pfe.UseDefaultCredentials = true;
                    string ret = pfe.Execute("UpdateScheduledWork", str);
                });
            }
            catch (Exception ex) 
            {
                //Need to change event number accordingly.
                myLog.WriteEntry("Error in ProcessPFEWork " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 305);
            }
        }

        private void AssignGroupsToTasks(int projectId, int pubType, WebSvcProject.ProjectDataSet pDs)
        {
            SPGroup addMemGrp = null; SPGroup addVisitGrp = null;            
            string strProjAssignTo = string.Empty;

            try
            {

                mySiteToPublish.AllowUnsafeUpdates = true;
                SPList lstPC = mySiteToPublish.Lists["Project Center"];
                SPListItem lstPCItem;                
                lstPCItem = lstPC.GetItemById(projectId);

                strProjAssignTo = (Convert.ToString(lstPCItem["AssignedTo"]) != string.Empty ? Convert.ToString(lstPCItem["AssignedTo"]) : "");

                if (lstPCItem != null && lstPCItem.HasUniqueRoleAssignments == true)
                {
                    foreach (SPRoleAssignment role in lstPCItem.RoleAssignments)
                    {
                        try
                        {
                            if (role.Member.GetType() == typeof(SPGroup))
                            {
                                SPGroup tempGrp = (SPGroup)role.Member;

                                if (tempGrp.Name.Contains("Member"))
                                {
                                    addMemGrp = (SPGroup)role.Member;
                                }

                                if (tempGrp.Name.Contains("Visitor"))
                                {
                                    addVisitGrp = (SPGroup)role.Member;
                                }
                            }
                        }
                        catch { }
                    }    
                    foreach (WebSvcProject.ProjectDataSet.ProjectResourceRow pr in pDs.ProjectResource)
                    {
                        try
                        {                                
                            if (!pr.IsWRES_ACCOUNTNull())
                            {
                                string email = "";
                                try
                                {
                                    email = pr.WRES_EMAIL;
                                }
                                catch { }

                                var assignRes = from assgRow in pDs.Assignment.AsEnumerable()
                                                where assgRow.Field<Guid>("Res_UID") == pr.RES_UID
                                                select assgRow;

                                if (assignRes.Count() > 0)
                                    addMemGrp.AddUser(pr.WRES_ACCOUNT, email, pr.RES_NAME, "");                                        
                                else
                                    addVisitGrp.AddUser(pr.WRES_ACCOUNT, email, pr.RES_NAME, "");

                                if (!strProjAssignTo.Contains(";#" + (mySiteToPublish.AllUsers[pr.WRES_ACCOUNT].ID + ";#" + getResourceName(pr.RES_UID, pDs))))
                                    strProjAssignTo += ";#" + (mySiteToPublish.AllUsers[pr.WRES_ACCOUNT].ID + ";#" + getResourceName(pr.RES_UID, pDs));
                                                                         
                            }                                
                        }
                        catch { }
                    }                    
                    addVisitGrp.Update();
                    addMemGrp.Update();
                    lstPCItem["AssignedTo"] = strProjAssignTo;
                    lstPCItem.Update();
                }
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error in AssignGroupsToTasks " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 305);
            }
        }

        private void loadFields()
        {
            try
            {
                try
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        cfDs = pCf.ReadCustomFieldsByEntity(taskEntity);
                    });
                }
                catch (Exception ex)
                {
                    myLog.WriteEntry("Error in publishProject() Reading Custom Fields: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 320);
                }

                SPList taskCenter = mySiteToPublish.Lists["Task Center"];
                foreach (SPField spField in taskCenter.Fields)
                {
                    hshTaskCenterFields.Add(spField.InternalName, spField.Id);
                    if (isValidField(spField))
                    {
                        SqlCommand cmd = new SqlCommand("SELECT fieldname,wssfieldname,coalesce(assnfieldname,'') as assnfieldname,fieldcategory,fieldtype,multiplier FROM CUSTOMFIELDS where wssfieldname like @wssfieldname", cn);
                        cmd.Parameters.AddWithValue("@wssfieldname", spField.InternalName);
                        SqlDataReader drField = cmd.ExecuteReader();
                        string rolldown = "false";
                        if (drField.Read())
                        {
                            try
                            {
                                if (drField.GetInt32(3) == 3)
                                {
                                    WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[] dr = (WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[])cfDs.CustomFields.Select("MD_PROP_ID=" + drField.GetString(0));
                                    if (dr.Length > 0)
                                    {
                                        rolldown = dr[0].MD_PROP_ROLLDOWN_TO_ASSN.ToString().ToLower();
                                    }
                                }
                            }
                            catch { }
                            arrFieldsToPublish.Add(drField.GetString(0) + "#" + spField.InternalName + "#" + drField.GetString(2) + "#" + drField.GetInt32(3) + "#" + drField.GetString(4) + "#" + drField.GetInt32(5) + "#" + spField.Id + "#" + rolldown);
                        }
                        else
                        {
                            if (spField.InternalName.Length > 3)
                            {
                                string fieldName = spField.InternalName.Substring(3);
                                int temp = 0;
                                if (int.TryParse(fieldName,out temp))
                                {
                                    WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[] dr = (WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[])cfDs.CustomFields.Select("MD_PROP_ID=" + fieldName);
                                    if (dr.Length > 0)
                                    {
                                        if(dr[0].IsMD_LOOKUP_TABLE_UIDNull())
                                            arrFieldsToPublish.Add(dr[0].MD_PROP_ID + "#" + spField.InternalName + "#" + dr[0].MD_PROP_UID_SECONDARY + "#3#" + getType(((PSLibrary.PropertyType)dr[0].MD_PROP_TYPE_ENUM).ToString()) + "#1#" + spField.Id + "#" + rolldown); 
                                        else
                                            arrFieldsToPublish.Add(dr[0].MD_PROP_ID + "#" + spField.InternalName + "#" + dr[0].MD_PROP_UID_SECONDARY + "#3#CHOICE#1#" + spField.Id + "#" + rolldown);
                                    }
                                }
                            }
                        }
                        drField.Close();
                    }
                }

                SPList projectCenter = mySiteToPublish.Lists["Project Center"];
                foreach (SPField spField in projectCenter.Fields)
                {
                    hshProjectCenterFields.Add(spField.InternalName, spField.Id);
                    if (isValidField(spField))
                    {
                        string sFieldName = spField.InternalName;

                        if (sFieldName == "Start")
                            sFieldName = "StartDate";
                        if (sFieldName == "Finish")
                            sFieldName = "DueDate";

                        SqlCommand cmd = new SqlCommand("SELECT fieldname,wssfieldname,coalesce(assnfieldname,'') as assnfieldname,fieldcategory,fieldtype,multiplier FROM CUSTOMFIELDS where wssfieldname like @wssfieldname", cn);
                        cmd.Parameters.AddWithValue("@wssfieldname", sFieldName);
                        SqlDataReader drField = cmd.ExecuteReader();
                        if (drField.Read())
                        {
                            arrPJFieldsToPublish.Add(drField.GetString(0) + "#" + spField.InternalName + "#" + drField.GetString(2) + "#" + drField.GetInt32(3) + "#" + drField.GetString(4) + "#" + drField.GetInt32(5) + "#" + spField.Id);
                        }
                        else
                        {
                            if (spField.InternalName.Length > 3)
                            {
                                string fieldName = spField.InternalName.Substring(3);
                                int temp = 0;
                                if (int.TryParse(fieldName, out temp))
                                {
                                    WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[] dr = (WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[])cfDs.CustomFields.Select("MD_PROP_ID=" + fieldName);
                                    if (dr.Length > 0)
                                    {
                                        if (dr[0].IsMD_LOOKUP_TABLE_UIDNull())
                                            arrPJFieldsToPublish.Add(dr[0].MD_PROP_ID + "#" + spField.InternalName + "#" + dr[0].MD_PROP_UID_SECONDARY + "#3#" + getType(((PSLibrary.PropertyType)dr[0].MD_PROP_TYPE_ENUM).ToString()) + "#1#" + spField.Id);
                                        else
                                            arrPJFieldsToPublish.Add(dr[0].MD_PROP_ID + "#" + spField.InternalName + "#" + dr[0].MD_PROP_UID_SECONDARY + "#3#CHOICE#1#" + spField.Id);
                                    }
                                }
                            }
                        }
                        drField.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error reading fields\r\n\r\n" + ex.Message + ex.StackTrace, EventLogEntryType.Error, 334);
            }
        }

        private string getType(string type)
        {
            string xml = "";
            switch (type)
            {
                case "NumberEng96":
                    xml = "NUMBER";
                    break;
                case "CostEng96":
                    xml = "CURRENCY";
                    break;
                case "StringEng96":
                    xml = "TEXT";
                    break;
                case "YesNoEng96":
                    xml = "BOOLEAN";
                    break;
                case "DurationEng96":
                    xml = "DURATION";
                    break;
                case "StartDateEng96":
                    xml = "DATETIME";
                    break;
                case "CHOICE":
                    xml = "CHOICE";
                    break;
            }
            return xml;
        }

        private bool loadCurrentTasks(string projectName)
        {
            try
            {
                SPList taskCenter = mySiteToPublish.Lists["Task Center"];

                SPQuery query = new SPQuery();
                query.Query = "<Where><Eq><FieldRef Name=\"Project\"/><Value Type=\"Text\"><![CDATA[" + projectName + "]]></Value></Eq></Where>";

                foreach (SPListItem li in taskCenter.GetItems(query))
                {
                    string taskuid = "";
                    string transuid = "";
                    try
                    {
                        taskuid = li["taskuid"].ToString();
                    }
                    catch { }
                    try
                    {
                        transuid = li["transuid"].ToString();
                    }
                    catch { }
                    if (taskuid != "")
                        hshCurTasks.Add(taskuid.ToUpper(), li.UniqueId);
                    else
                    {
                        if (lastTransUid.ToString().ToLower() == transuid.ToLower())
                        {
                            arrDelNewTasks.Add(li.UniqueId);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Could not load current tasks, Task Center list may not exist or be configured correctly at (" + publishSiteUrl + "):\r\n\r\n " + ex.Message + ex.StackTrace, EventLogEntryType.Warning, 210);
                return false;
            }
        }

        private bool isValidField(SPField spField)
        {
            if (spField.Hidden || spField.ReadOnlyField)
                return false;

            if (spField.Type == SPFieldType.Attachments || spField.Type == SPFieldType.Calculated || spField.Type == SPFieldType.User)
                return false;

            switch (spField.InternalName)
            {
                case "taskuid":
                case "taskorder":
                case "Publisher_x0020_Approval_x0020_C":
                case "Publisher_x0020_Approval_x0020_S":
                case "LastPublished":
                case "WBS":
                case "Notes":
                case "Status":
                case "Priority":
                case "Title":
                case "AssignedTo":
                    return false;
            }

            return true;
        }

        private string multiplyField(string fieldVal, string multiplier)
        {
            try
            {
                float newVal = float.Parse(fieldVal) / float.Parse(multiplier);
                return newVal.ToString();
            }
            catch { }
            return fieldVal;
        }

        private void processAssignment(WebSvcProject.ProjectDataSet.AssignmentRow assn, WebSvcProject.ProjectDataSet pDs, SPListItem listItem)
        {
            

            //StringBuilder sb = new StringBuilder();
            WebSvcProject.ProjectDataSet.TaskRow taskRow = (WebSvcProject.ProjectDataSet.TaskRow)pDs.Task.Select("TASK_UID='" + assn.TASK_UID + "'")[0];

            try
            {

                DateTime lp = DateTime.Now;
                listItem[listItem.Fields.GetFieldByInternalName("TaskHierarchy").Id] = getHierarchy(pDs, taskRow.TASK_PARENT_UID);
                listItem[listItem.Fields.GetFieldByInternalName("IsAssignment").Id] = "1";
                listItem[listItem.Fields.GetFieldByInternalName("Title").Id] = taskRow.TASK_NAME;
                listItem[listItem.Fields.GetFieldByInternalName("WBS").Id] = taskRow.TASK_WBS;
                listItem[listItem.Fields.GetFieldByInternalName("taskuid").Id] = taskRow.TASK_UID + "." + assn.ASSN_UID;
                listItem[listItem.Fields.GetFieldByInternalName("taskorder").Id] = taskRow.TASK_ID;
                if (!assn.IsASSN_NOTESNull())
                    listItem[listItem.Fields.GetFieldByInternalName("Notes").Id] = assn.ASSN_NOTES;
                listItem[listItem.Fields.GetFieldByInternalName("LastPublished").Id] = DateTime.Now; //lp.Year.ToString() + "-" + lp.Month.ToString() + "-" + lp.Day.ToString() + " " + lp.Hour.ToString() + ":" + lp.Minute.ToString() + ":" + lp.Second.ToString();
                int resId = getResourceWssId(assn.RES_UID_OWNER);
                if (resId != 0)
                    listItem[listItem.Fields.GetFieldByInternalName("AssignedTo").Id] = resId;
                listItem["Summary"] = taskRow.TASK_IS_SUMMARY.ToString();

                foreach (string sField in arrFieldsToPublish)
                {
                    
                        string[] sFieldSplit = sField.Split('#');
                        string fieldName = sFieldSplit[0];
                        string wssFieldName = sFieldSplit[1];
                        string assnFieldName = sFieldSplit[2];
                        string fieldCategory = sFieldSplit[3];
                        string fieldType = sFieldSplit[4];
                        string multiplier = sFieldSplit[5];
                        string rolldown = sFieldSplit[7];
                    try
                    {
                        if (fieldName == "TASK_RESNAMES")
                        {
                            listItem[listItem.Fields.GetFieldByInternalName("ResourceNames").Id] = getResourceName(assn.RES_UID, pDs);
                        }
                        else if (fieldName == "TASK_PCT_COMP")
                        {

                            float pct = assn.ASSN_PCT_WORK_COMPLETE;
                            pct = pct / (float)100.00;
                            listItem[listItem.Fields.GetFieldByInternalName("PercentComplete").Id] = pct;
                            //sb.Append("<Field Name='" + wssFieldName + "'>" + pct + "</Field>");
                        }
                        else
                        {
                            if (fieldCategory == "3")
                            {
                                string fieldData = null;
                                DataRow[] drAssn = pDs.AssignmentCustomFields.Select("ASSN_UID='" + assn.ASSN_UID.ToString() + "' AND MD_PROP_UID='" + assnFieldName + "'");
                                if (drAssn.Length >= 1)
                                {
                                    switch (fieldType)
                                    {
                                        case "DATETIME":
                                            fieldData = drAssn[0]["DATE_VALUE"].ToString();
                                            //fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                            break;
                                        case "DURATION":
                                            fieldData = drAssn[0]["DUR_VALUE"].ToString();
                                            try
                                            {
                                                fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                            }
                                            catch { }
                                            break;
                                        case "NUMBER":
                                            fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                            break;
                                        case "CURRENCY":
                                            fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                            try
                                            {
                                                fieldData = (float.Parse(fieldData) / 100).ToString();
                                            }
                                            catch { }
                                            break;
                                        case "BOOLEAN":
                                            fieldData = drAssn[0]["FLAG_VALUE"].ToString();
                                            break;
                                        case "TEXT":
                                            fieldData = drAssn[0]["TEXT_VALUE"].ToString();
                                            break;
                                        case "CHOICE":
                                            fieldData = drAssn[0]["CODE_VALUE"].ToString();
                                            fieldData = getLookupValue(fieldName, fieldData);
                                            break;
                                    }

                                    //sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                                }
                                else if (rolldown == "true")
                                {
                                    DataRow[] drTask = pDs.TaskCustomFields.Select("TASK_UID='" + taskRow.TASK_UID.ToString() + "' AND MD_PROP_ID='" + fieldName + "'");
                                    if (drTask.Length >= 1)
                                    {
                                        switch (fieldType)
                                        {
                                            case "DATETIME":
                                                fieldData = drTask[0]["DATE_VALUE"].ToString();
                                                //fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                                break;
                                            case "DURATION":
                                                fieldData = drTask[0]["DUR_VALUE"].ToString();
                                                try
                                                {
                                                    fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                                }
                                                catch { }
                                                break;
                                            case "NUMBER":
                                                fieldData = drTask[0]["NUM_VALUE"].ToString();
                                                break;
                                            case "CURRENCY":
                                                fieldData = drTask[0]["NUM_VALUE"].ToString();
                                                try
                                                {
                                                    fieldData = (float.Parse(fieldData) / 100).ToString();
                                                }
                                                catch { }
                                                break;
                                            case "BOOLEAN":
                                                fieldData = drTask[0]["FLAG_VALUE"].ToString();
                                                break;
                                            case "TEXT":
                                                fieldData = drTask[0]["TEXT_VALUE"].ToString();
                                                break;
                                            case "CHOICE":
                                                fieldData = drTask[0]["CODE_VALUE"].ToString();
                                                fieldData = getLookupValue(fieldName, fieldData);
                                                break;
                                        }
                                    }
                                }
                                listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                            }
                            else if (fieldCategory == "2")
                            {
                                DataRow[] drAssn = pDs.AssignmentCustomFields.Select("ASSN_UID='" + assn.ASSN_UID.ToString() + "' AND MD_PROP_ID='" + assnFieldName + "'");
                                if (drAssn.Length >= 1)
                                {

                                    string fieldData = "";
                                    switch (fieldType)
                                    {
                                        case "DATETIME":
                                            fieldData = drAssn[0]["DATE_VALUE"].ToString();
                                            //fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                            break;
                                        case "DURATION":
                                            fieldData = drAssn[0]["DUR_VALUE"].ToString();
                                            try
                                            {
                                                fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                            }
                                            catch { }
                                            break;
                                        case "NUMBER":
                                            fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                            break;
                                        case "CURRENCY":
                                            fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                            try
                                            {
                                                fieldData = (float.Parse(fieldData) / 100).ToString();
                                            }
                                            catch { }
                                            break;
                                        case "BOOLEAN":
                                            fieldData = drAssn[0]["FLAG_VALUE"].ToString();
                                            break;
                                        case "TEXT":
                                            fieldData = drAssn[0]["TEXT_VALUE"].ToString();
                                            break;
                                        case "CHOICE":
                                            fieldData = drAssn[0]["CODE_VALUE"].ToString();
                                            fieldData = getLookupValue(fieldName, fieldData);
                                            break;
                                    }
                                    listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                    //sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                                }
                            }
                            else if (fieldCategory == "1")
                            {
                                if (fieldName == "TASK_PREDECESSORS")
                                {
                                    string preds = "";
                                    DataRow[] drPreds = pDs.Dependency.Select("LINK_SUCC_UID='" + taskRow.TASK_UID.ToString() + "'");
                                    foreach (DataRow drPred in drPreds)
                                    {
                                        WebSvcProject.ProjectDataSet.TaskRow[] drTask = (WebSvcProject.ProjectDataSet.TaskRow[])pDs.Task.Select("TASK_UID='" + drPred["LINK_PRED_UID"] + "'");
                                        if (drTask.Length > 0)
                                        {
                                            preds += "," + drTask[0].TASK_ID;
                                            switch (drPred["LINK_TYPE"].ToString())
                                            {
                                                case "0":
                                                    preds += "FF";
                                                    break;
                                                case "2":
                                                    preds += "SF";
                                                    break;
                                                case "3":
                                                    preds += "SS";
                                                    break;
                                            };
                                        }
                                    }
                                    if (preds.Length > 1)
                                        preds = preds.Substring(1);
                                    listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = preds;
                                }
                                else
                                {
                                    string fieldData = "";
                                    try
                                    {
                                        fieldData = assn[assnFieldName].ToString();
                                    }
                                    catch
                                    {
                                        fieldData = taskRow[fieldName].ToString();
                                    }
                                    if (fieldType == "DATETIME")
                                    {
                                        if (fieldData.Trim() != "")
                                            listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                    }
                                    else
                                    {
                                        if (multiplier != "1")
                                        {
                                            fieldData = multiplyField(fieldData, multiplier);
                                        }
                                        listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        myLog.WriteEntry("Error processing Assignment (" + taskRow.TASK_NAME + ") Field (" + fieldName + "): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 330);
                    }
                }
                if (strTimesheetField != "")
                {
                    if (listItem.Fields.ContainsField("Timesheet"))
                    {
                        DataRow[] drAssn = pDs.TaskCustomFields.Select("TASK_UID='" + taskRow.TASK_UID.ToString() + "' AND MD_PROP_UID='" + strTimesheetField + "'");
                        if (drAssn.Length > 0)
                            listItem["Timesheet"] = drAssn[0]["FLAG_VALUE"].ToString();
                        else
                            listItem["Timesheet"] = 0;
                    }
                }
                listItem.Update();
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error processing Assignment (" + taskRow.TASK_NAME + "): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 330);

            }
            //return sb.ToString();
        }

        private void processProjectCenterFields(WebSvcProject.ProjectDataSet pDs, ArrayList arrFToPublish, ref SPListItem listItem)
        {
            //StringBuilder sb = new StringBuilder();
            try
            {

                foreach (string sField in arrFToPublish)
                {
                    string[] sFieldSplit = sField.Split('#');
                    string fieldName = sFieldSplit[0];
                    string wssFieldName = sFieldSplit[1];
                    string fieldCategory = sFieldSplit[3];
                    string fieldType = sFieldSplit[4];
                    string multiplier = sFieldSplit[5];

                    if (fieldCategory == "4")
                    {
                        DataRow[] drAssn = pDs.ProjectCustomFields.Select("MD_PROP_ID='" + fieldName + "'");
                        if (drAssn.Length >= 1)
                        {

                            string fieldData = "";
                            switch (fieldType)
                            {
                                case "DATETIME":
                                    fieldData = drAssn[0]["DATE_VALUE"].ToString();
                                    fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                    break;
                                case "DURATION":
                                    fieldData = drAssn[0]["DUR_VALUE"].ToString();
                                    try
                                    {
                                        fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                    }
                                    catch { }
                                    break;
                                case "NUMBER":
                                    fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                    break;
                                case "CURRENCY":
                                    fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                    try
                                    {
                                        fieldData = (float.Parse(fieldData) / 100).ToString();
                                    }
                                    catch { }
                                    break;
                                case "BOOLEAN":
                                    fieldData = drAssn[0]["FLAG_VALUE"].ToString();
                                    break;
                                case "TEXT":
                                    fieldData = drAssn[0]["TEXT_VALUE"].ToString();
                                    break;
                                case "CHOICE":
                                    fieldData = drAssn[0]["CODE_VALUE"].ToString();
                                    fieldData = getLookupValue(fieldName, fieldData);
                                    break;
                            }
                            listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                            // sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error processing Project Center Enterprise Fields: " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 330);

            }
            listItem.Update();
        }

        private string getHierarchy(WebSvcProject.ProjectDataSet pDs, Guid taskuid)
        {
            if (hshTaskHierarchy.Contains(taskuid))
                return hshTaskHierarchy[taskuid].ToString();

            WebSvcProject.ProjectDataSet.TaskRow taskRow = (WebSvcProject.ProjectDataSet.TaskRow)pDs.Task.Select("TASK_UID='" + taskuid + "'")[0];
            
            string hierarchy = getHierarchy(pDs, taskRow.TASK_PARENT_UID) + " > " + taskRow.TASK_NAME;

            if (hierarchy[1] == '>')
                hierarchy = hierarchy.Substring(3);

            hshTaskHierarchy.Add(taskRow.TASK_UID, hierarchy);

            return hierarchy;
        }

        private void processTask(WebSvcProject.ProjectDataSet.TaskRow taskRow, WebSvcProject.ProjectDataSet pDs, ArrayList arrFToPublish, SPListItem listItem, Hashtable hshFields1, string taskuid, int pubType)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (string sField in arrFToPublish)
                {
                    string[] sFieldSplit = sField.Split('#');
                    string fieldName = sFieldSplit[0];
                    string wssFieldName = sFieldSplit[1];
                    string fieldCategory = sFieldSplit[3];
                    string fieldType = sFieldSplit[4];
                    string multiplier = sFieldSplit[5];
                    string fieldData = null;

                    try
                    {
                        if (fieldCategory == "3")
                        {
                            DataRow[] drAssn = pDs.TaskCustomFields.Select("TASK_UID='" + taskRow.TASK_UID.ToString() + "' AND MD_PROP_ID='" + fieldName + "'");
                            if (drAssn.Length >= 1)
                            {
                                switch (fieldType)
                                {
                                    case "DATETIME":
                                        fieldData = drAssn[0]["DATE_VALUE"].ToString();
                                        //fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                        break;
                                    case "DURATION":
                                        fieldData = drAssn[0]["DUR_VALUE"].ToString();
                                        try
                                        {
                                            fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                        }
                                        catch { }
                                        break;
                                    case "NUMBER":
                                        fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                        break;
                                    case "CURRENCY":
                                        fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                        try
                                        {
                                            fieldData = (float.Parse(fieldData) / 100).ToString();
                                        }
                                        catch { }
                                        break;
                                    case "BOOLEAN":
                                        fieldData = drAssn[0]["FLAG_VALUE"].ToString();
                                        break;
                                    case "TEXT":
                                        fieldData = drAssn[0]["TEXT_VALUE"].ToString();
                                        break;
                                    case "CHOICE":
                                        fieldData = drAssn[0]["CODE_VALUE"].ToString();
                                        fieldData = getLookupValue(fieldName, fieldData);
                                        break;
                                }
                                //listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                //sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                            }
                            listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                        }
                        else if (fieldCategory == "2")
                        {
                            DataRow[] drAssn = pDs.TaskCustomFields.Select("TASK_UID='" + taskRow.TASK_UID.ToString() + "' AND MD_PROP_ID='" + fieldName + "'");
                            if (drAssn.Length >= 1)
                            {

                                switch (fieldType)
                                {
                                    case "DATETIME":
                                        fieldData = drAssn[0]["DATE_VALUE"].ToString();
                                        fieldData = DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString();
                                        break;
                                    case "DURATION":
                                        fieldData = drAssn[0]["DUR_VALUE"].ToString();
                                        try
                                        {
                                            fieldData = (float.Parse(fieldData) / 4800.0).ToString();
                                        }
                                        catch { }
                                        break;
                                    case "NUMBER":
                                        fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                        break;
                                    case "CURRENCY":
                                        fieldData = drAssn[0]["NUM_VALUE"].ToString();
                                        try
                                        {
                                            fieldData = (float.Parse(fieldData) / 100).ToString();
                                        }
                                        catch { }
                                        break;
                                    case "BOOLEAN":
                                        fieldData = drAssn[0]["FLAG_VALUE"].ToString();
                                        break;
                                    case "TEXT":
                                        fieldData = drAssn[0]["TEXT_VALUE"].ToString();
                                        break;
                                    case "CHOICE":
                                        fieldData = drAssn[0]["CODE_VALUE"].ToString();
                                        fieldData = getLookupValue(fieldName, fieldData);
                                        break;
                                }
                                listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                //sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                            }
                        }
                        else if (fieldCategory == "1")
                        {
                            if (fieldName == "TASK_RESNAMES")
                            {

                            }
                            else if (fieldName == "TASK_PREDECESSORS")
                            {
                                string preds = "";
                                DataRow[] drPreds = pDs.Dependency.Select("LINK_SUCC_UID='" + taskRow.TASK_UID.ToString() + "'");
                                foreach (DataRow drPred in drPreds)
                                {
                                    WebSvcProject.ProjectDataSet.TaskRow[] drTask = (WebSvcProject.ProjectDataSet.TaskRow[])pDs.Task.Select("TASK_UID='" + drPred["LINK_PRED_UID"] + "'");
                                    if (drTask.Length > 0)
                                    {
                                        preds += "," + drTask[0].TASK_ID;

                                        switch (drPred["LINK_TYPE"].ToString())
                                        {
                                            case "0":
                                                preds += "FF";
                                                break;
                                            case "2":
                                                preds += "SF";
                                                break;
                                            case "3":
                                                preds += "SS";
                                                break;
                                        };
                                    }
                                }
                                if (preds.Length > 1)
                                    preds = preds.Substring(1);
                                listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = preds;
                            }
                            else
                            {
                                try
                                {
                                    fieldData = taskRow[fieldName].ToString();
                                }
                                catch
                                {

                                }
                                if (fieldType == "DATETIME")
                                {
                                    if (fieldData.Trim() != "")
                                    {
                                        //fieldData = .ToString("Z");

                                        listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = DateTime.Parse(fieldData);
                                    }
                                    else
                                        listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = null;
                                    //sb.Append("<Field Name='" + wssFieldName + "'>" + DateTime.Parse(fieldData).Year.ToString() + "-" + DateTime.Parse(fieldData).Month.ToString() + "-" + DateTime.Parse(fieldData).Day.ToString() + " " + DateTime.Parse(fieldData).Hour.ToString() + ":" + DateTime.Parse(fieldData).Minute.ToString() + ":" + DateTime.Parse(fieldData).Second.ToString() + "</Field>");
                                }
                                else
                                {
                                    if (multiplier != "1")
                                    {
                                        fieldData = multiplyField(fieldData, multiplier);
                                    }
                                    listItem[listItem.Fields.GetFieldByInternalName(wssFieldName).Id] = fieldData;
                                    //sb.Append("<Field Name='" + wssFieldName + "'>" + fieldData + "</Field>");
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        myLog.WriteEntry("Error setting field (" + fieldName + ") fieldValue (" + fieldData + "): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 335);
                    }
                }
                if (taskuid != "")
                    listItem["taskuid"] = taskuid;

                if (strTimesheetField != "")
                {
                    if (listItem.Fields.ContainsField("Timesheet"))
                    {
                        if (pubType == 1)
                        {
                            listItem["Timesheet"] = 0;
                        }
                        else
                        {
                            DataRow[] drAssn = pDs.TaskCustomFields.Select("TASK_UID='" + taskRow.TASK_UID.ToString() + "' AND MD_PROP_UID='" + strTimesheetField + "'");
                            if (drAssn.Length > 0)
                                listItem["Timesheet"] = drAssn[0]["FLAG_VALUE"].ToString();
                            else
                                listItem["Timesheet"] = 0;
                        }
                    }
                }
                listItem.Update();
            }
            catch (SPException ex1)
            {
                myLog.WriteEntry("SPException: Error processing Task (" + taskRow.TASK_NAME + "): " + ex1.Message + ex1.StackTrace, EventLogEntryType.Error, 331);
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error processing Task (" + taskRow.TASK_NAME + "): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 330);
            }
            
            //return sb.ToString();
        }

        private int getResourceWssId(Guid RES_GUID)
        {
            DataRow[] drRes = rDs.Resources.Select("RES_UID='" + RES_GUID + "'");
            if (drRes.Length > 0)
            {
                if (drRes[0]["RES_IS_WINDOWS_USER"].ToString() == "True")
                {
                    string username = drRes[0]["WRES_ACCOUNT"].ToString();
                    try
                    {
                        return mySiteToPublish.AllUsers[username].ID;
                    }
                    catch
                    {
                        return 0;
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// this method used to return resource Id.
        /// </summary>
        /// <param name="RES_GUID"></param>
        /// <returns></returns>
        private Int64 getResourceResId(Guid RES_GUID)
        {
            DataRow[] drRes = rDs.Resources.Select("RES_UID='" + RES_GUID + "'");
            if (drRes.Length > 0)
            {
                if (drRes[0]["RES_IS_WINDOWS_USER"].ToString() == "True")
                {
                    Int64 ResId = Convert.ToInt64(drRes[0]["Res_ID"]);
                    return ResId;
                }
            }
            return 0;
        }

        private string getResourceName(Guid RES_GUID, WebSvcProject.ProjectDataSet pDs)
        {
            DataRow[] drRes = rDs.Resources.Select("RES_UID='" + RES_GUID + "'");
            if (drRes.Length > 0)
            {
                if (drRes[0]["RES_IS_WINDOWS_USER"].ToString() == "True")
                {
                    string username = drRes[0]["WRES_ACCOUNT"].ToString();
                    try
                    {
                        return mySiteToPublish.AllUsers[username].Name;
                    }
                    catch
                    {
                        return "";
                    }
                }
                else
                {
                    return Convert.ToString(drRes[0]["RES_NAME"]);
                }
            }
            else
            {
                DataRow[] dr = pDs.ProjectResource.Select("RES_UID='" + RES_GUID + "'");
                if (dr.Length > 0)
                    return dr[0]["RES_NAME"].ToString();
            }
            return "";
        }

        private string getResourceUsername(Guid RES_GUID)
        {
            DataRow[] drRes = rDs.Resources.Select("RES_UID='" + RES_GUID + "'");
            if (drRes.Length > 0)
            {
                if (drRes[0]["RES_IS_WINDOWS_USER"].ToString() == "True")
                {
                    return drRes[0]["WRES_ACCOUNT"].ToString();
                }
            }
            return "";
        }

        private string getLookupValue(string fieldName, string lv_id)
        {
            try
            {

                WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow []ds = (WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow [])dsFields.CustomFields.Select("MD_PROP_ID=" + fieldName);

                if (ds.Length <= 0)
                {
                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {
                        dsFields = pCf.ReadCustomFieldsByEntity(new Guid(PSLibrary.EntityCollection.Entities.ProjectEntity.UniqueId));
                    });
                    ds = (WebSvcCustomFields.CustomFieldDataSet.CustomFieldsRow[])dsFields.CustomFields.Select("MD_PROP_ID=" + fieldName);

                }

                WebSvcLookupTables.LookupTableDataSet.LookupTableTreesRow[] tr = (WebSvcLookupTables.LookupTableDataSet.LookupTableTreesRow[])dsLt.LookupTableTrees.Select("LT_STRUCT_UID = '" + lv_id + "'");

                switch ((PSLibrary.PSDataType)ds[0].MD_PROP_TYPE_ENUM)
                {
                    case PSLibrary.PSDataType.STRING:
                        return tr[0].LT_VALUE_TEXT;
                    case PSLibrary.PSDataType.NUMBER:
                        return tr[0].LT_VALUE_NUM.ToString();
                }
                return "";
            }
            catch (Exception ex)
            {
                myLog.WriteEntry("Error Reading Lookup Table for field (" + fieldName + "): " + ex.Message + ex.StackTrace, EventLogEntryType.Error, 330);
            }
            return "";
        }

        public bool IsAssignedTaskSaved(Guid assnid)
        {
            try
            {
                StatusingDerived status = new StatusingDerived();
                status.Url = mySite.Url + "/_vti_bin/psi/statusing.asmx";
                WebSvcStatusing.StatusApprovalTransactionDetailsDataSet sa2Ds = status.ReadStatusApprovalDetails(assnid);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
