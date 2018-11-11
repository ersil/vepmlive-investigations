﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using EPMLiveCore.Helpers;
using EPMLiveCore.ReportHelper;
using Microsoft.SharePoint;

namespace EPMLiveCore
{
    public class ItemSecurityEventReceiver : SPItemEventReceiver
    {



        public override void ItemAdded(SPItemEventProperties properties)
        {
            AddTimerJob(properties);
        }



        public override void ItemUpdated(SPItemEventProperties properties)
        {
            //if(System.Diagnostics.Process.GetCurrentProcess().ProcessName != "TimerService")
            AddTimerJob(properties);
        }

        private void AddTimerJob(SPItemEventProperties properties)
        {

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                base.EventFiringEnabled = false;
                GridGanttSettings settings = new GridGanttSettings(properties.List);

                bool isSecure = false;
                try
                {
                    isSecure = settings.BuildTeamSecurity;
                }
                catch { }


                int priority = 1;
                if (isSecure)
                    priority = 0;

                using (var sqlConnection = new SqlConnection(CoreFunctions.getConnectionString(properties.Web.Site.WebApplication.Id)))
                {
                    sqlConnection.Open();
                    using (var sqlCommand = new SqlCommand("INSERT INTO ITEMSEC (SITE_ID, WEB_ID, LIST_ID, ITEM_ID, USER_ID, priority) VALUES (@siteid, @webid, @listid, @itemid, @userid, @priority)", sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@siteid", properties.SiteId);
                        sqlCommand.Parameters.AddWithValue("@webid", properties.Web.ID);
                        sqlCommand.Parameters.AddWithValue("@listid", properties.ListId);
                        sqlCommand.Parameters.AddWithValue("@itemid", properties.ListItemId);
                        sqlCommand.Parameters.AddWithValue("@userid", properties.CurrentUserId);
                        sqlCommand.Parameters.AddWithValue("@priority", priority);
                        sqlCommand.ExecuteNonQuery();
                    }
                }
                SPUser orignalUser = properties.Web.AllUsers.GetByID(properties.CurrentUserId);

                if (isSecure)
                {
                    SPListItem li = properties.ListItem;

                    string safeTitle = !string.IsNullOrEmpty(li.Title) ? GetSafeGroupTitle(li.Title) : string.Empty;

                    properties.Web.AllowUnsafeUpdates = true;

                    SPFieldLookup assignedTo = null;
                    try
                    {
                        assignedTo = properties.List.Fields.GetFieldByInternalName("AssignedTo") as SPFieldLookup;
                    }
                    catch { }

                    object assignedToFv = null;
                    string sAssignedTo = string.Empty;
                    try
                    {
                        assignedToFv = li["AssignedTo"];
                    }
                    catch { }
                    if (assignedToFv != null)
                    {
                        sAssignedTo = assignedToFv.ToString();
                    }

                    if (string.IsNullOrEmpty(sAssignedTo))
                    {
                        li["AssignedTo"] = new SPFieldUserValue(properties.Web, orignalUser.ID, orignalUser.LoginName);

                        try
                        {
                            li.SystemUpdate();
                        }
                        catch (Exception e)
                        {
                        }

                    }
                }
                base.EventFiringEnabled = true;
            });

        }

        private string GetSafeGroupTitle(string grpName)
        {
            return StringHelper.GetSafeString(grpName);
        }

        public override void ItemDeleting(SPItemEventProperties properties)
        {
            base.ItemDeleted(properties);

            SPListItem _listItem = properties.ListItem;

            //EPML-3694: Security permissions lost on "SAP Procure to Pay" project
            //As per proposed solution mentioned in Jira item - "Do not remove groups if title is blank"
            //So, in case of Title is blank we just return from this routine.
            if (string.IsNullOrEmpty(_listItem.Title))
                return;

            List<SPGroup> grps = new List<SPGroup>();
            foreach (SPRoleAssignment ra in _listItem.RoleAssignments)
            {
                if (ra.Member is SPGroup && ra.Member.Name.Contains(_listItem.Title))
                {
                    grps.Add(ra.Member as SPGroup);
                }
            }

            if (_listItem.HasUniqueRoleAssignments)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    using (SPSite es = new SPSite(properties.WebUrl))
                    {
                        using (SPWeb ew = es.OpenWeb())
                        {
                            ew.AllowUnsafeUpdates = true;
                            foreach (SPGroup g in grps)
                            {
                                ew.SiteGroups.Remove(g.Name);
                            }
                            ew.Update();
                        }
                    }
                });
            }

        }

    }
}
