﻿using System;
using System.IO;
using System.Linq;
using EPMLiveCore.Infrastructure;
using EPMLiveCore.Jobs.EPMLiveUpgrade.Infrastructure;
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

            using (var spSite = new SPSite(siteId))
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
                UpdateUI(siteId, spWeb.ID);
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

        #region Methods (1) 

        // Private Methods (1) 

        private void UpdateLink(string url, SPNavigationNode node)
        {
            try
            {
                LogMessage(string.Format("Node: {0}, URL: {1}", node.Title, url), 3);

                node.Url = url;
                node.Update();

                LogMessage(string.Empty, MessageKind.SUCCESS, 4);
            }
            catch (Exception e)
            {
                LogMessage(e.Message, MessageKind.FAILURE, 3);
            }
        }

        #endregion Methods 

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
                                                int id = Convert.ToInt32(nodeId.Split(':')[0]);

                                                SPNavigationNode node = spWeb.Navigation.GetNodeById(id);
                                                if (node.Title.Equals("My Work"))
                                                {
                                                    UpdateLink(spWeb.Url + "/_layouts/15/epmlive/MyWork.aspx", node);
                                                }
                                                else if (node.Title.Equals("Timesheet"))
                                                {
                                                    UpdateLink(spWeb.Url + "/_layouts/15/epmlive/MyTimesheet.aspx", node);
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

                                    CacheStore.Current.RemoveSafely(spWeb.Url, CacheStoreCategory.Navigation);
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
}