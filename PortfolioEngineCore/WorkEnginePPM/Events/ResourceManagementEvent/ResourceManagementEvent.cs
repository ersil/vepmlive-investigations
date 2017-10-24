﻿using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.SharePoint;
using WorkEnginePPM.Core.ResourceManagement;
using EPMLiveCore;
using Utilities = WorkEnginePPM.Core.ResourceManagement.Utilities;
using WorkEnginePPM.WebServices.Core;
using System.Threading;
using System.Threading.Tasks;
namespace WorkEnginePPM.Events
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class ResourceManagementEvent : SPItemEventReceiver
    {
        #region Methods (6)

        // Public Methods (4) 

        /// <summary>
        /// An item was added
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            if (!ValidateRequest(properties)) return;

            try
            {
                SPWeb spWeb = properties.Web;

                decimal rate;
                Utilities.AddUpdateResource(Utilities.BuildFieldsTable(properties, false), spWeb, properties.ListId,
                                            out rate, false);

                CalculateResourceAvailabilities(properties.ListItem["EXTID"].ToString(), spWeb);
                UpdateUser(properties);
            }
            catch (Exception exception)
            {
                properties.ErrorMessage = exception.GetBaseException().Message;
                properties.Cancel = true;
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
        }

        /// <summary>
        /// An item is being added.
        /// </summary>
        /// <param name="properties"></param>
        public override void ItemAdding(SPItemEventProperties properties)
        {
            if (!ValidateRequest(properties)) return;

            try
            {
                SPWeb spWeb = properties.Web;

                decimal rate;
                properties.AfterProperties["EXTID"] =
                    Utilities.AddUpdateResource(Utilities.BuildFieldsTable(properties, true), spWeb, properties.ListId,
                                                out rate, true);

                if (rate != 0) properties.AfterProperties["StandardRate"] = rate;
            }
            catch (Exception exception)
            {
                properties.ErrorMessage = exception.GetBaseException().Message;
                properties.Cancel = true;
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
        }

        /// <summary>
        /// An item is being deleted
        /// </summary>
        public override void ItemDeleting(SPItemEventProperties properties)
        {
            if (!ValidateRequest(properties)) return;

            try
            {
                int extId;

                if (!int.TryParse(properties.ListItem["EXTID"] as string, out extId))
                {
                    return;
                }

                bool confirmDelete = false;

                StackFrame[] stackFrames = new StackTrace().GetFrames();
                if (stackFrames != null)
                {
                    if ((from stackFrame in stackFrames
                         select stackFrame.GetMethod()
                             into methodBase
                         where methodBase.Name.Equals("ConfirmDelete")
                         select methodBase.DeclaringType
                                 into declaringType
                         where declaringType != null
                         select declaringType.FullName)
                        .Any(fullName => fullName != null && fullName.Equals("EPMLiveCore.API.ResourceGrid")))
                    {
                        confirmDelete = true;
                    }
                }

                SPWeb spWeb = properties.Web;

                if (!confirmDelete)
                {
                    string deleteResourceCheckMessage;
                    string deleteResourceCheckStatus;

                    if (Utilities.PerformDeleteResourceCheck(extId, properties.ListItem.UniqueId, spWeb,
                                                             out deleteResourceCheckStatus,
                                                             out deleteResourceCheckMessage))
                    {
                        Utilities.DeleteResource(extId, properties.ListItem.UniqueId, spWeb);
                    }
                    else
                    {
                        properties.ErrorMessage = string.Format("{0}|||{1}", deleteResourceCheckStatus,
                                                                deleteResourceCheckMessage);
                        properties.Cancel = true;
                        properties.Status = SPEventReceiverStatus.CancelWithError;
                    }
                }
                else
                {
                    Utilities.DeleteResource(extId, properties.ListItem.UniqueId, spWeb);
                }
            }
            catch (Exception exception)
            {
                properties.ErrorMessage = exception.GetBaseException().Message;
                properties.Cancel = true;
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
        }

        /// <summary>
        /// An item is being updated.
        /// </summary>
        /// <param name="properties">An <see cref="T:Microsoft.SharePoint.SPItemEventProperties"/> object that represents properties of the event handler.</param>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            if (!ValidateRequest(properties)) return;
            if (properties.ListItem == null) return;

            StackFrame[] stackFrames = new StackTrace().GetFrames();

            if (stackFrames != null)
            {
                if (
                    stackFrames.Select(stackFrame => stackFrame.GetMethod().Name).Any(
                        methodName =>
                        methodName.Equals("AddCommentersFields") || methodName.Equals("CreateComment") ||
                        methodName.Equals("UpdateComment") || methodName.Equals("DeleteComment")))
                {
                    properties.Cancel = true;
                    properties.Status = SPEventReceiverStatus.CancelNoError;

                    return;
                }
            }

            try
            {
                SPWeb spWeb = properties.Web;

                decimal rate;
                var extId = Utilities.AddUpdateResource(Utilities.BuildFieldsTable(properties, false), spWeb, properties.ListId, out rate, true);

                if (rate != 0) properties.AfterProperties["StandardRate"] = rate;
                properties.AfterProperties["EXTID"] = extId;
            }
            catch (Exception exception)
            {
                properties.ErrorMessage = exception.GetBaseException().Message;
                properties.Cancel = true;
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            if (!ValidateRequest(properties)) return;

            try
            {
                CalculateResourceAvailabilities(properties.ListItem["EXTID"].ToString(), properties.Web);
                UpdateUser(properties);
            }
            catch (Exception exception)
            {
                properties.ErrorMessage = exception.GetBaseException().Message;
                properties.Cancel = true;
                properties.Status = SPEventReceiverStatus.CancelWithError;
            }
        }

        /// <summary>
        /// Validates the request.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns></returns>
        private bool ValidateRequest(SPItemEventProperties properties)
        {
            return properties.OpenSite().Features[new Guid("158c5682-d839-4248-b780-82b4710ee152")] != null &&
                   properties.List.Title.Equals("Resources");
        }

        private void CalculateResourceAvailabilities(string pfeResourceId, SPWeb spWeb)
        {
            Task.Run(() => {
                using (var resourceManager = new ResourceManager(spWeb))
                {
                    resourceManager.CalculateResourceAvailabilities(int.Parse(pfeResourceId));
                }
            });
        }

        public static void UpdateUser(SPItemEventProperties properties)
        {
            using (SPWeb spWeb = properties.OpenWeb())
            {
                SPFieldUserValue user = new SPFieldUserValue(spWeb, properties.ListItem["SharePointAccount"].ToString());
                if (user.LookupId == spWeb.Site.SystemAccount.ID)
                    return;
                SPList myList = spWeb.Lists["Resources"];

                SPQuery curQry = new SPQuery();
                //Get resource with Sp Account Id       					
                string query = string.Format(@"<Where><Eq><FieldRef Name='SharePointAccount' LookupId='TRUE'/><Value Type='Integer'>{0}</Value></Eq></Where>", user.LookupId);
                curQry.Query = query;
                string UserName = string.Empty;
                if (myList != null && myList.ItemCount > 0)
                {
                    if (myList.Fields.ContainsField("UserHasPermission"))
                    {
                        SPListItemCollection myItems = myList.GetItems(curQry);
                        foreach (SPListItem item in myItems)
                        {
                            if (Convert.ToString(item["Generic"]).ToUpper() == "YES")
                            {
                                return;
                            }

                            int ResourceLevel = Convert.ToInt32(item["ResourceLevel"]);
                            bool ResourceLevelPermission = false;
                            if (ResourceLevel == 2 || ResourceLevel == 3 || item["ResourceLevel"] == null)//null and 2 for full permission 3 for project Manger and null for Developemnt
                                ResourceLevelPermission = true;
                            UserName = CoreFunctions.GetRealUserName(user.User.LoginName);
                            string basePath = CoreFunctions.getConfigSetting(spWeb, "epkbasepath");
                            bool ResourcePlanPermission = EPMLiveCore.Utilities.CheckEditResourcePlanPermission(basePath, UserName) && ResourceLevelPermission;
                            var newitem = EPMLiveCore.Utilities.ReloadListItem(item);
                            newitem["UserHasPermission"] = ResourcePlanPermission;
                            using (var scope = new DisabledItemEventScope())
                            {
                                newitem.SystemUpdate(false);// false will prevent to update version number and save conflict
                            }
                        }
                    }
                }
            }
        }
        #endregion Methods
    }
}
