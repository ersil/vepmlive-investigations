﻿using EPMLiveIntegration;
using System;
using System.Collections.Generic;
using System.Data;

namespace UplandIntegrations.Tfs
{
    public class TfsIntegrator : IIntegrator, IIntegratorControls
    {
        #region Interface Methods
        public bool InstallIntegration(WebProperties WebProps, IntegrationLog Log, out string Message, string IntegrationKey, string APIUrl)
        {
            try
            {
                Message = "";
                CheckWebProps(WebProps, true);
                if (string.IsNullOrEmpty(APIUrl))
                {
                    Message = "APIUrl is required for integration. Please contact administrator.";
                    return false;
                }
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                    tfsService.InstallEvent((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], IntegrationKey, APIUrl);
                }
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
                Log.LogMessage("Install integration. " + ex.Message, IntegrationLogType.Error);
                return false;
            }
            return true;
        }
        public bool RemoveIntegration(WebProperties WebProps, IntegrationLog Log, out string Message, string IntegrationKey)
        {
            try
            {
                Message = "";
                CheckWebProps(WebProps, true);
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                    tfsService.RemoveEvent((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], Convert.ToString(WebProps.Properties["ServerUrl"]), IntegrationKey);
                }
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
                Log.LogMessage("Remove integration. " + ex.Message, IntegrationLogType.Error);
                return false;
            }
            return true;
        }

        public List<IntegrationControl> GetControls(WebProperties WebProps, IntegrationLog Log)
        {
            throw new NotImplementedException();
        }
        public List<ColumnProperty> GetColumns(WebProperties WebProps, IntegrationLog Log, string ListName)
        {
            CheckWebProps(WebProps, true);
            List<ColumnProperty> columnPropertyList = new List<ColumnProperty>();
            using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
            {
                DataTable dataTable = new DataTable();
                String objectName = (string)WebProps.Properties["Object"];
                String teamProjectCollection = (string)WebProps.Properties["TeamProjectCollection"];
                tfsService.GetObjectItems(teamProjectCollection, objectName, dataTable, DateTime.Now.AddYears(-1), true);
                foreach (DataColumn item in dataTable.Columns)
                {
                    ColumnProperty prop = new ColumnProperty();
                    prop.ColumnName = item.ColumnName;
                    prop.DiplayName = item.ColumnName;
                    prop.DefaultListColumn = GetDefaultColumn(objectName, item.ColumnName);
                    columnPropertyList.Add(prop);
                }
                dataTable.Dispose();
                dataTable = null;
            }
            return columnPropertyList;
        }
        public Dictionary<string, string> GetDropDownValues(WebProperties WebProps, IntegrationLog log, string Property, string ParentPropertyValue)
        {
            Dictionary<string, string> props = new Dictionary<string, string>();
            if (Property == "TeamProjectCollection")
            {
                CheckWebProps(WebProps, false);
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                    DataTable collection = new DataTable();
                    tfsService.GetObjectItems(null, Property, collection, DateTime.Now, false);
                    foreach (DataRow dataRow in collection.Rows)
                    {
                        props.Add(Convert.ToString(dataRow["DisplayName"]), Convert.ToString(dataRow["DisplayName"]));
                    }
                }
            }
            else if (Property == "Object")
            {
                props.Add(TfsType.Bug.ToString(), TfsType.Bug.ToString());
                props.Add(TfsType.Change_Request.ToString(), TfsType.Change_Request.ToString().Replace("_", " "));
                props.Add(TfsType.Feature.ToString(), TfsType.Feature.ToString());
                props.Add(TfsType.Issue.ToString(), TfsType.Issue.ToString());
                props.Add(TfsType.Product_Backlog_Item.ToString(), TfsType.Product_Backlog_Item.ToString().Replace("_", " "));
                props.Add(TfsType.Projects.ToString(), TfsType.Projects.ToString());
                props.Add(TfsType.Requirement.ToString(), TfsType.Requirement.ToString());
                props.Add(TfsType.Risk.ToString(), TfsType.Risk.ToString());
                props.Add(TfsType.Shared_Steps.ToString(), TfsType.Shared_Steps.ToString().Replace("_", " "));
                props.Add(TfsType.Task.ToString(), TfsType.Task.ToString());
                props.Add(TfsType.Test_Case.ToString(), TfsType.Test_Case.ToString().Replace("_", " "));
                props.Add(TfsType.User_Story.ToString(), TfsType.User_Story.ToString().Replace("_", " "));
            }
            else if (Property == "UseBasicAuthCredential")
            {
                props.Add("true", "true");
                props.Add("false", "false");
            }
            return props;
        }
        public bool TestConnection(WebProperties WebProps, IntegrationLog Log, out string Message)
        {
            try
            {
                Message = "";
                CheckWebProps(WebProps, false);
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                }
                return true;
            }
            catch (Exception ex)
            {
                Message = "Error: " + ex.Message;
                Log.LogMessage("Test connection. " + ex.Message, IntegrationLogType.Error);
                return false;
            }
        }

        public TransactionTable DeleteItems(WebProperties WebProps, DataTable Items, IntegrationLog Log)
        {
            CheckWebProps(WebProps, true);
            TransactionTable transactionTable = new TransactionTable();
            using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
            {
                foreach (DataRow item in Items.Rows)
                {
                    string curId = item["ID"].ToString();
                    string spId = item["SPID"].ToString();
                    try
                    {
                        tfsService.DeleteObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], Convert.ToInt32(curId));
                        transactionTable.AddRow(spId, curId, TransactionType.DELETE);
                    }
                    catch (Exception ex)
                    {
                        transactionTable.AddRow(spId, curId, TransactionType.FAILED);
                        Log.LogMessage("Delete items. " + ex.Message, IntegrationLogType.Warning);
                    }
                }
            }
            //Log.LogMessage("Delete items. Not supported in TFS", IntegrationLogType.Error);
            return transactionTable;
        }
        public TransactionTable UpdateItems(WebProperties WebProps, DataTable Items, IntegrationLog Log)
        {
            CheckWebProps(WebProps, true);
            TransactionTable transactionTable = new TransactionTable();
            using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
            {
                foreach (DataRow item in Items.Rows)
                {
                    string curId = item["ID"].ToString();
                    string spId = item["SPID"].ToString();
                    try
                    {
                        if (string.IsNullOrEmpty(curId))
                        {
                            curId = tfsService.CreateObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], item, Items.Columns).ToString();
                            transactionTable.AddRow(spId, curId, TransactionType.INSERT);
                        }
                        else
                        {
                            DataTable getItemDataTable = new DataTable();
                            tfsService.GetObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], getItemDataTable, curId, false);

                            if (getItemDataTable.Rows.Count == 0)
                            {
                                curId = tfsService.CreateObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], item, Items.Columns).ToString();
                                transactionTable.AddRow(spId, curId, TransactionType.INSERT);
                            }
                            else
                            {
                                tfsService.UpdateObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], item, Convert.ToInt32(curId), Items.Columns);
                                transactionTable.AddRow(spId, curId, TransactionType.UPDATE);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        transactionTable.AddRow(spId, curId, TransactionType.FAILED);
                        Log.LogMessage("Update items. " + ex.Message, IntegrationLogType.Warning);
                    }
                }
            }
            return transactionTable;
        }
        public DataTable GetItem(WebProperties WebProps, IntegrationLog log, string ItemID, DataTable Items)
        {
            try
            {
                CheckWebProps(WebProps, true);
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                    tfsService.GetObjectItem((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], Items, ItemID, false);
                }
            }
            catch (Exception ex)
            {
                log.LogMessage("Get item. " + ex.Message, IntegrationLogType.Error);
            }
            return Items;
        }
        public DataTable PullData(WebProperties WebProps, IntegrationLog log, DataTable Items, DateTime LastSynchDate)
        {
            try
            {
                CheckWebProps(WebProps, true);
                using (TfsService tfsService = new TfsService(WebProps.Properties["ServerUrl"].ToString(), WebProps.Properties["Username"].ToString(), WebProps.Properties["Password"].ToString(), Convert.ToBoolean(WebProps.Properties["UseBasicAuthCredential"].ToString())))
                {
                    tfsService.GetObjectItems((string)WebProps.Properties["TeamProjectCollection"], (string)WebProps.Properties["Object"], Items, LastSynchDate, false);
                }
            }
            catch (Exception ex)
            {
                log.LogMessage("Pull data. " + ex.Message, IntegrationLogType.Error);
            }
            return Items;
        }
        #endregion

        #region Private Methods

        private void CheckWebProps(WebProperties WebProps, Boolean checkOtherWebProperties)
        {
            if (string.IsNullOrEmpty(Convert.ToString(WebProps.Properties["ServerUrl"]))) throw new Exception("Please provide the serverurl.");
            if (string.IsNullOrEmpty(Convert.ToString(WebProps.Properties["Username"]))) throw new Exception("Please provide the username.");
            if (string.IsNullOrEmpty(Convert.ToString(WebProps.Properties["Password"]))) throw new Exception("Please provide the password.");

            if (checkOtherWebProperties)
            {
                if (string.IsNullOrEmpty(Convert.ToString(WebProps.Properties["TeamProjectCollection"]))) throw new Exception("Please provide the project collection.");
                if (string.IsNullOrEmpty(Convert.ToString(WebProps.Properties["Object"]))) throw new Exception("Please provide the object.");
            }
        }
        private string GetDefaultColumn(string objectName, string columnName)
        {
            return string.Empty;
        }

        #endregion

        public string GetControlCode(WebProperties WebProps, IntegrationLog Log, string ItemID, string Control)
        {
            return string.Empty;
        }

        public List<string> GetEmbeddedItemControls(WebProperties WebProps, IntegrationLog Log)
        {
            return new List<string>();
        }

        public List<IntegrationControl> GetPageButtons(WebProperties WebProps, IntegrationLog Log, bool GlobalButtons)
        {
            if (!GlobalButtons)
            {
                CheckWebProps(WebProps, true);
                TfsType tfsType = (TfsType)Enum.Parse(typeof(TfsType), Convert.ToString(WebProps.Properties["Object"]));
                switch (tfsType)
                {
                    case TfsType.Bug:
                    case TfsType.Change_Request:
                    case TfsType.Feature:
                    case TfsType.Issue:
                    case TfsType.Product_Backlog_Item:
                    case TfsType.Requirement:
                    case TfsType.Risk:
                    case TfsType.Shared_Steps:
                    case TfsType.Task:
                    case TfsType.Test_Case:
                    case TfsType.User_Story:
                        return new List<IntegrationControl>
                        {
                            new IntegrationControl
                            {
                                Control = "TF_ViewWorkItem",
                                Title = "View Work Item",
                                Image = "tf_viewworkitem.png",
                                Window = IntegrationControlWindowStyle.FullWindow
                            }
                    
                        };
                        break;
                    default:
                        break;
                }
            }
            return new List<IntegrationControl>();
        }

        public string GetURL(WebProperties webProps, IntegrationLog log, string control, string itemId)
        {
            try
            {
                CheckWebProps(webProps, true);
                switch (control)
                {
                    case "TF_ViewWorkItem":
                        using (TfsService tfsService = new TfsService(webProps.Properties["ServerUrl"].ToString(), webProps.Properties["Username"].ToString(), webProps.Properties["Password"].ToString(), Convert.ToBoolean(webProps.Properties["UseBasicAuthCredential"].ToString())))
                        {
                            return tfsService.GetWorkItemURL((string)webProps.Properties["TeamProjectCollection"], (string)webProps.Properties["Object"], itemId);
                        }
                        break;
                }
                throw new Exception(control + " is not a valid tfs control.");
            }
            catch (Exception exception)
            {
                log.LogMessage(exception.Message, IntegrationLogType.Error);
            }

            return string.Empty;
        }

        public string GetProxyResult(WebProperties WebProps, IntegrationLog Log, string ItemID, string Control, string Property)
        {
            return string.Empty;
        }
    }
}
