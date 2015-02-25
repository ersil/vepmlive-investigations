﻿using EPMLiveCore.Jobs.EPMLiveUpgrade.Infrastructure;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMLiveCore.Jobs.EPMLiveUpgrade.Steps
{
    [UpgradeStep(Version = EPMLiveVersion.V567, Order = 1.0, Description = "Adding Resource Pool events, Updating Description(Body) field with base content types")]
    internal class AddResourcePoolEvents : UpgradeStep
    {
        public AddResourcePoolEvents(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        public override bool Perform()
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                try
                {
                    using (var workEngineAPI = new WorkEngineAPI())
                    {
                        var log = System.Web.HttpUtility.HtmlEncode(workEngineAPI.Execute("AddRemoveFeatureEvents",
                            @"<AddRemoveFeatureEvents><Data><Feature Name=""PFEResourceManagement"" Operation=""ADD""/></Data></AddRemoveFeatureEvents>"));

                        LogMessage(log, 2);
                        LogMessage("Resource Pool Events processed", MessageKind.SUCCESS, 4);
                    }
                }
                catch (Exception exception)
                {
                    string message = exception.InnerException != null
                        ? exception.InnerException.Message
                        : exception.Message;

                    LogMessage(message, MessageKind.FAILURE, 4);
                }


                try
                {
                    Web.AllowUnsafeUpdates = true;
                    foreach (SPList list in Web.Lists)
                    {
                        if (!list.Hidden)
                        {
                            foreach (SPContentType contentType in list.ContentTypes)
                            {
                                try
                                {
                                    if (contentType != null && (contentType.Name == "Item" || contentType.Name == "Task"))
                                    {
                                        SPField fldDescription = list.Fields.GetFieldByInternalName("Body");
                                        if (fldDescription != null)
                                        {
                                            SPFieldLink fld = new SPFieldLink(fldDescription);
                                            if (fld != null)
                                            {
                                                if (contentType.FieldLinks[fld.Id] == null)
                                                {
                                                    contentType.FieldLinks.Delete(fld.Name);
                                                    contentType.Update();
                                                    contentType.FieldLinks.Add(fld);
                                                    contentType.Update();
                                                    list.Update();
                                                    LogMessage(string.Format("Update processed with List: {0} , ContentType: {1} , Field: {2}", list.Title, contentType.Name, fld.DisplayName), MessageKind.SUCCESS, 4);
                                                }
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    string message = exception.InnerException != null
                        ? exception.InnerException.Message
                        : exception.Message;

                    LogMessage(message, MessageKind.FAILURE, 4);
                }

            });

            return true;
        }
    }
}
