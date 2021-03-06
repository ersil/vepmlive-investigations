﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using EPMLiveCore.API;
using EPMLiveCore.Infrastructure;
using EPMLiveCore.Jobs.EPMLiveUpgrade.Infrastructure;
using EPMLiveCore.ListDefinitions;
using EPMLiveCore.ReportingProxy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Navigation;

namespace EPMLiveCore.Jobs.EPMLiveUpgrade.Steps.OptIn
{
    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 1.0, Description = "Updating User Interface", IsOptIn = true)]
    internal class UpdateUI55 : UpgradeStep
    {
        #region Constructors (1) 

        public UpdateUI55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        #endregion Constructors 

        #region Methods (3) 

        // Private Methods (3) 

        private void ChangeMasterPage(string masterPage, SPWeb spWeb)
        {
            LogMessage("Changing MasterPage to: " + masterPage, 2);

            string url = (spWeb.ServerRelativeUrl == "/" ? string.Empty : spWeb.ServerRelativeUrl);

            spWeb.MasterUrl = string.Format(url + "/_catalogs/masterpage/{0}.master", masterPage);
            spWeb.CustomMasterUrl = string.Format(url + "/_catalogs/masterpage/{0}.master", masterPage);
            spWeb.Update();
        }

        private void ResetFeature(Guid featureId, string featureName, SPWeb spWeb)
        {
            LogTitle("Feature: " + featureName, 2);

            SPFeature spFeature = spWeb.Features.FirstOrDefault(f => f.DefinitionId == featureId);

            if (spFeature != null)
            {
                LogTitle("Deactivating . . ", 4);
                spWeb.Features.Remove(spFeature.DefinitionId);
            }

            LogTitle("Activating . . ", 4);
            spWeb.Features.Add(featureId);

            LogMessage(string.Empty, MessageKind.SUCCESS, 3);
        }

        private void UpdateUI(Guid siteId, Guid webId)
        {
            SPWebCollection webCollection;

            using (SPSite spSite = new SPSite(siteId))
            {
                using (SPWeb spWeb = spSite.OpenWeb(webId))
                {
                    LogTitle(GetWebInfo(spWeb), 1);

                    try
                    {
                        string masterUrl = spWeb.MasterUrl;
                        string fileName = Path.GetFileName(masterUrl).ToLower();

                        if (!fileName.Equals("uplandv5.master"))
                        {
                            var masterpages = new[]
                            {
                                "epmlive", "epmlivemasterv5blue", "masterv43lightbluetop",
                                "masterv43lightbluews", "wetoplevel",
                                "weworkspace", "weworkspacetopnav"
                            };

                            bool contains = false;
                            foreach (string masterpage in masterpages.Where(mp => (mp + ".master").Equals(fileName)))
                            {
                                contains = true;
                            }

                            if (contains)
                            {
                                ResetFeature(new Guid("046f0200-30e5-4545-b00f-c8c73aef9f0e"), "EPM Live Upland UI",
                                    spWeb);
                                ChangeMasterPage("UplandV5", spWeb);
                            }
                            else
                            {
                                LogMessage("The current default MasterPage is not one of EPM Live MasterPage.",
                                    MessageKind.SKIPPED, 2);
                            }
                        }
                        else
                        {
                            LogMessage("The default MasterPage is already set to UplandV5.", MessageKind.SKIPPED, 2);
                        }

                        SPList spList = spWeb.Lists.TryGetList("Team");
                        if (spList != null)
                        {
                            var settings = new GridGanttSettings(spList) { HideNewButton = true };
                            settings.SaveSettings(spList);
                        }
                    }
                    catch (Exception exception)
                    {
                        LogMessage(exception.Message, MessageKind.FAILURE, 3);
                    }
                    finally
                    {
                        webCollection = spWeb.Webs;
                    }
                }
            }

            if (webCollection == null) return;

            foreach (SPWeb spWeb in webCollection)
            {
                try
                {
                    UpdateUI(siteId, spWeb.ID);
                }
                catch (Exception ex)
                {
                    LogMessage(ex.Message, MessageKind.FAILURE, 3);
                }
                finally {if(spWeb!=null) spWeb.Dispose(); }                
                
            }
        }

        #endregion Methods 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                Guid siteId;
                Guid webId;

                using (var spSite = new SPSite(Web.Site.ID))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        siteId = spSite.ID;
                        webId = spWeb.ID;
                    }
                }

                UpdateUI(siteId, webId);
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 2.0, Description = "Updating Navigation", IsOptIn = true)]
    internal class UpdateNav55 : UpgradeStep
    {
        #region Constructors (1) 

        public UpdateNav55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        #endregion Constructors 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                using (var spSite = new SPSite(Web.Site.ID))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        LogTitle(GetWebInfo(spWeb), 1);

                        SPList spList = spWeb.Lists.TryGetList("Installed Applications");
                        if (spList != null)
                        {
                            var qry = new SPQuery
                            {
                                Query =
                                    @"<Where><Eq><FieldRef Name='Title' /><Value Type='Text'>My Workplace</Value></Eq></Where>",
                                ViewFields = @"<FieldRef Name='ID' />"
                            };

                            SPListItemCollection listItems = spList.GetItems(qry);

                            if (listItems.Count != 0)
                            {
                                try
                                {
                                    LogTitle("Changing My Workplace to Global My Workplace", 2);

                                    SPListItem item = spList.GetItemById(listItems[0].ID);

                                    item["Title"] = "Global My Workplace";
                                    item["HomePage"] = new SPFieldUrlValue
                                    {
                                        Url = spWeb.Url + "/SitePages/GlobalMyWorkplace.aspx",
                                        Description = "Global My Workplace"
                                    };

                                    item.SystemUpdate();

                                    LogTitle("Updating links", 2);

                                    string[] nodes = ((item["QuickLaunch"] ?? string.Empty).ToString()).Split(',');

                                    if (nodes.Any())
                                    {
                                        foreach (string nodeId in nodes)
                                        {
                                            try
                                            {
                                                string message;
                                                MessageKind messageKind;

                                                int id = Convert.ToInt32(nodeId.Split(':')[0]);
                                                SPNavigationNode node = spWeb.Navigation.GetNodeById(id);

                                                if (node == null) continue;

                                                if (node.Title.Equals("My Work"))
                                                {
                                                    string url = spWeb.ServerRelativeUrl +
                                                                 "/_layouts/15/epmlive/MyWork.aspx";

                                                    if (url.ToLower().Equals(node.Url.ToLower())) continue;

                                                    UpgradeUtilities.UpdateNodeLink(url, item.ID, node, spWeb,
                                                        out message, out messageKind);

                                                    LogMessage(message, messageKind, 3);
                                                }
                                                else if (node.Title.Equals("Timesheet"))
                                                {
                                                    string url = spWeb.ServerRelativeUrl +
                                                                 "/_layouts/15/epmlive/MyTimesheet.aspx";

                                                    if (url.ToLower().Equals(node.Url.ToLower())) continue;

                                                    UpgradeUtilities.UpdateNodeLink(url, item.ID, node, spWeb,
                                                        out message, out messageKind);

                                                    LogMessage(message, messageKind, 3);
                                                }
                                            }
                                            catch (Exception e)
                                            {
                                                LogMessage(e.Message, MessageKind.FAILURE, 3);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        LogMessage("No navigation nodes were found.", MessageKind.FAILURE, 3);
                                    }

                                    CacheStore.Current.RemoveSafely(spWeb.Url, new CacheStoreCategory(spWeb).Navigation);
                                }
                                catch (Exception e)
                                {
                                    LogMessage(e.Message, MessageKind.FAILURE, 3);
                                }
                            }
                            else
                            {
                                LogMessage("The My Workplace community was not found.", MessageKind.SKIPPED, 2);
                            }
                        }
                        else
                        {
                            LogMessage("The list Installed Applications does not exists.", MessageKind.FAILURE, 2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 3.0, Description = "Updating List Icons", IsOptIn = true)]
    internal class UpdateListIcon55 : UpgradeStep
    {
        #region Fields (1) 

        private readonly Dictionary<string, string> _listIcons;

        #endregion Fields 

        #region Constructors (1) 

        public UpdateListIcon55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite)
        {
            _listIcons = new Dictionary<string, string>
            {
                {"Project Portfolios", "icon-notebook"},
                {"Project Center", "icon-briefcase-3"},
                {"Task Center", "icon-checkbox-checked"},
                {"Issues", "icon-warning"},
                {"Project Documents", "icon-libreoffice"},
                {"Resources", "icon-user-3"},
                {"Time Off", "icon-calendar-4"},
                {"To Do", "icon-list-5"}
            };
        }

        #endregion Constructors 

        #region Methods (1) 

        // Private Methods (1) 

        private void UpdateListIcon(Guid siteId, Guid webId)
        {
            SPWebCollection webCollection;

            using (var spSite = new SPSite(siteId))
            {
                using (SPWeb spWeb = spSite.OpenWeb(webId))
                {
                    LogTitle(GetWebInfo(spWeb), 1);

                    foreach (var listIcon in _listIcons)
                    {
                        try
                        {
                            SPList spList = spWeb.Lists.TryGetList(listIcon.Key);

                            if (spList != null)
                            {
                                LogTitle(GetListInfo(spList), 2);

                                LogTitle("Icon: " + listIcon.Value, 3);

                                var settings = new GridGanttSettings(spList);
                                if (string.IsNullOrEmpty(settings.ListIcon) || settings.ListIcon.Equals("icon-square"))
                                {
                                    settings.ListIcon = listIcon.Value;
                                    if (settings.SaveSettings(spList))
                                    {
                                        LogMessage(string.Empty, MessageKind.SUCCESS, 4);
                                    }
                                    else
                                    {
                                        LogMessage("Could not save the icon.", MessageKind.FAILURE, 4);
                                    }
                                }
                                else
                                {
                                    LogMessage("The icon is already set to: " + settings.ListIcon, MessageKind.SKIPPED,
                                        4);
                                }
                            }
                            else
                            {
                                LogMessage("The list " + listIcon.Key + " does not exists.", MessageKind.SKIPPED, 3);
                            }
                        }
                        catch (Exception e)
                        {
                            LogMessage(e.Message, MessageKind.FAILURE, 2);
                        }
                    }

                    webCollection = spWeb.Webs;
                }
            }

            if (webCollection == null) return;

            foreach (SPWeb spWeb in webCollection)
            {
                try
                {
                    UpdateListIcon(siteId, spWeb.ID);
                }
                catch (Exception ex)
                { LogMessage(ex.ToString(), MessageKind.FAILURE, 2); }
                finally
                { if (spWeb != null) spWeb.Dispose(); }
            }
        }

        #endregion Methods 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                Guid siteId;
                Guid webId;

                using (var spSite = new SPSite(Web.Site.ID))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        siteId = spSite.ID;
                        webId = spWeb.ID;
                    }
                }

                UpdateListIcon(siteId, webId);
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 4.0, Description = "Using Content DB", IsOptIn = true)]
    internal class UseContentDB55 : UpgradeStep
    {
        #region Constructors (1) 

        public UseContentDB55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        #endregion Constructors 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                using (var spSite = new SPSite(Web.Site.ID))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        LogTitle(GetWebInfo(spWeb), 1);

                        var queryExecutor = new QueryExecutor(spWeb);
                        foreach (Guid listId in queryExecutor.GetMappedListIds())
                        {
                            SPList spList = null;

                            try
                            {
                                spList = spWeb.Lists.GetList(listId, true);
                            }
                            catch { }

                            if (spList == null) continue;

                            LogTitle(GetListInfo(spList), 2);

                            var settings = new GridGanttSettings(spList);

                            string reason = string.Empty;

                            switch ((int)spList.BaseTemplate)
                            {
                                case (int)EPMLiveLists.ProjectCenter:
                                    reason = "List Definition: Project Center.";
                                    break;
                                case (int)EPMLiveLists.TaskCenter:
                                    reason = "List Definition: Task Center.";
                                    break;
                                default:
                                    if (settings.EnableWorkList) reason = "Work List.";
                                    break;
                            }

                            if (!string.IsNullOrEmpty(reason))
                            {
                                settings.EnableContentReporting = true;
                                settings.SaveSettings(spList);

                                LogMessage(reason, MessageKind.SUCCESS, 3);
                            }
                            else
                            {
                                LogMessage("List is not a Project Center, Task Center or a Work list.",
                                    MessageKind.SKIPPED, 3);
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 5.0, Description = "Turning on Create Workspace functionality",
        IsOptIn = true)]
    internal class TurnOnCreateWorkspace55 : UpgradeStep
    {
        #region Constructors (1) 

        public TurnOnCreateWorkspace55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        #endregion Constructors 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                using (var spSite = new SPSite(Web.Site.ID))
                {
                    using (SPWeb spWeb = spSite.OpenWeb())
                    {
                        LogTitle(GetWebInfo(spWeb), 1);

                        SPList spList = spWeb.Lists.TryGetList("Project Center");

                        if (spList != null)
                        {
                            LogTitle(GetListInfo(spList), 2);

                            var settings = new GridGanttSettings(spList) { EnableRequests = true };
                            settings.SaveSettings(spList);

                            LogMessage(string.Empty, MessageKind.SUCCESS, 3);
                        }
                        else
                        {
                            LogMessage("Cannot find the Project Center list.", MessageKind.FAILURE, 2);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 6.0, Description = "Configuring Advance Reporting",
        IsOptIn = true)]
    internal class ConfigureAdvanceReporting55 : UpgradeStep
    {
        #region Fields (2) 

        private const string LIST_NAME = "IzendaReports";
        private readonly string _storeUrl;

        #endregion Fields 

        #region Constructors (1) 

        public ConfigureAdvanceReporting55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite)
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            _storeUrl = "https://store.workengine.com";
        }

        #endregion Constructors 

        #region Methods (1) 

        // Private Methods (1) 

        private void AddGroup(SPWeb spWeb, SPGroup owner, SPUser user, string groupName)
        {
            LogMessage(groupName, 3);

            try
            {
                SPGroup spGroup = null;

                try
                {
                    spGroup = spWeb.SiteGroups.GetByName(groupName);
                }
                catch { }

                if (spGroup == null)
                {
                    spWeb.SiteGroups.Add(groupName, owner, user, null);
                    SPRole roll = spWeb.Roles["Read"];
                    roll.AddGroup(spWeb.SiteGroups[groupName]);

                    try
                    {
                        SPList spList = spWeb.Lists["User Information List"];
                        spList.Items.GetItemById(spWeb.SiteGroups[groupName].ID).SystemUpdate();
                    }
                    catch { }

                    LogMessage(null, MessageKind.SUCCESS, 4);
                }
                else
                {
                    LogMessage("Group already exists.", MessageKind.SKIPPED, 4);
                }
            }
            catch (Exception exception)
            {
                LogMessage(exception.Message, MessageKind.FAILURE, 4);
            }
        }

        #endregion Methods 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var spSite = new SPSite(Web.Site.ID))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb())
                        {
                            LogTitle(GetWebInfo(spWeb), 1);

                            SPList spList = spWeb.Lists.TryGetList(LIST_NAME);

                            if (spList == null)
                            {
                                LogMessage("Downloading new " + LIST_NAME + " list", 2);

                                var catalog =
                                    (SPDocumentLibrary)Web.Site.GetCatalog(SPListTemplateType.ListTemplateCatalog);

                                const string TEMPLATE_NAME = LIST_NAME + " [5.5]";

                                using (var webClient = new WebClient())
                                {
                                    byte[] bytes =
                                        webClient.DownloadData(_storeUrl + "/Upgrade/" + LIST_NAME.ToLower() + ".stp");
                                    SPFile file = catalog.RootFolder.Files.Add(LIST_NAME + "_55.stp", bytes, true);
                                    SPListItem li = file.GetListItem();
                                    li["Title"] = TEMPLATE_NAME;
                                    li.SystemUpdate();
                                }

                                LogMessage("Creating the " + LIST_NAME + " list", 2);

                                SPListTemplateCollection listTemplates = spSite.GetCustomListTemplates(spWeb);
                                SPListTemplate template = listTemplates[TEMPLATE_NAME];

                                spWeb.Lists.Add(LIST_NAME, string.Empty, template);

                                SPList list = spWeb.Lists[LIST_NAME];
                                list.Title = LIST_NAME;
                                list.Hidden = true;
                                list.Update();

                                LogMessage("Adding reporting groups", 2);

                                SPGroup owner = spWeb.SiteGroups["Administrators"];
                                SPUser user = spWeb.CurrentUser;

                                AddGroup(spWeb, owner, user, "Report Viewers");
                                AddGroup(spWeb, owner, user, "Report Writers");

                                LogMessage("Processing reports", 2);

                                string error;
                                Reporting.ProcessIzendaReportsFromList(list, out error);

                                if (!string.IsNullOrEmpty(error))
                                {
                                    LogMessage(error, MessageKind.FAILURE, 3);
                                }
                                else
                                {
                                    LogMessage(null, MessageKind.SUCCESS, 3);
                                }

                                LogMessage("Updating Navigation link", 2);

                                string relativeUrl = spWeb.ServerRelativeUrl.ToLower();
                                string newUrl = relativeUrl + "/_layouts/15/epmlive/reporting/landing.aspx";

                                SPList lst = spWeb.Lists.TryGetList("Installed Applications");
                                if (lst != null)
                                {
                                    var qry = new SPQuery
                                    {
                                        Query = @"<Where><IsNotNull><FieldRef Name='QuickLaunch' /></IsNotNull></Where>",
                                        ViewFields = @"<FieldRef Name='QuickLaunch' />"
                                    };

                                    SPListItemCollection listItems = lst.GetItems(qry);

                                    foreach (SPListItem item in listItems)
                                    {
                                        foreach (SPNavigationNode navNode in
                                            from node in item["QuickLaunch"].ToString().Split(',')
                                            select Convert.ToInt32(node.Split(':')[0])
                                            into i
                                            select spWeb.Navigation.GetNodeById(i)
                                            into navNode
                                            where navNode != null
                                            let url = navNode.Url.ToLower()
                                            where
                                                url.EndsWith(relativeUrl + "/reports.aspx") ||
                                                url.EndsWith(relativeUrl + "/sitepages/report.aspx")
                                            select navNode)
                                        {
                                            string message;
                                            MessageKind messageKind;

                                            UpgradeUtilities.UpdateNodeLink(newUrl, item.ID, navNode, spWeb, out message,
                                                out messageKind);

                                            LogMessage(message, messageKind, 3);
                                        }
                                    }
                                }
                                else
                                {
                                    LogMessage("The list Installed Applications does not exists.", MessageKind.FAILURE,
                                        3);
                                }

                                CacheStore.Current.RemoveSafely(spWeb.Url, new CacheStoreCategory(spWeb).Navigation);
                            }
                            else
                            {
                                LogMessage("Advance reporting is already configured.", MessageKind.SKIPPED, 2);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 2);
            }

            return true;
        }

        #endregion
    }

    [UpgradeStep(Version = EPMLiveVersion.V55, Order = 7.0, Description = "Scheduling Reporting Refresh", IsOptIn = true
        )]
    internal class RefreshReporting55 : UpgradeStep
    {
        #region Constructors (1) 

        public RefreshReporting55(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { }

        #endregion Constructors 

        #region Overrides of UpgradeStep

        public override bool Perform()
        {
            try
            {
                UpgradeUtilities.ScheduleReportingRefresh(Web);
                LogMessage(null, MessageKind.SUCCESS, 1);
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 1);
            }

            return true;
        }

        #endregion
    }
}