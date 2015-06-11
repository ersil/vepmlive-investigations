﻿using EPMLiveCore.API.SPAdmin;
using EPMLiveCore.Jobs.EPMLiveUpgrade.Infrastructure;
using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPMLiveCore.Jobs.EPMLiveUpgrade.Steps
{
    [UpgradeStep(Version = EPMLiveVersion.V568, Order = 1.0, Description = "Update Menu Name/Add new Menu item")]
    internal class AddUpdateMenuItem : UpgradeStep
    {
        private const string SETTINGS_LIST = "EPM Live Settings";
        private SPWeb _spWeb;
        
        #region Constructors
        public AddUpdateMenuItem(SPWeb spWeb, bool isPfeSite) : base(spWeb, isPfeSite) { _spWeb = spWeb; }
        #endregion

        #region Overrides of UpgradeStep
        public override bool Perform()
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(() =>
                {
                    using (var spSite = new SPSite(Web.Site.ID))
                    {
                        using (SPWeb spWeb = spSite.OpenWeb(Web.ID))
                        {
                            if (!spWeb.IsRootWeb)
                            {
                                LogMessage(spWeb.Title + " is not a root web.", MessageKind.SKIPPED, 2);
                                return;
                            }

                            LogMessage("Getting " + SETTINGS_LIST + " list", 2);

                            SPList list = Web.Lists.TryGetList(SETTINGS_LIST);
                            if (list != null)
                            {

                                LogMessage("Updating Job Queue Menu", 2);

                                SPQuery jobQueueQuery = new SPQuery();
                                jobQueueQuery.Query = "<Where><Eq><FieldRef Name='Title'/><Value Type='Text'>Job Queue</Value></Eq></Where>";
                                SPListItemCollection jobListItems = list.GetItems(jobQueueQuery);

                                if (jobListItems != null && jobListItems.Count > 0)
                                {
                                    SPListItem li = jobListItems[0];
                                    li["Title"] = "Portfolio Queue";
                                    li.Update();                                    
                                }

                                LogMessage("Adding Work Queue Menu", 2);

                                var blnItemExists = list.Items.Cast<SPListItem>().Any(item => item.Title.Equals("Work Queue"));

                                if (!blnItemExists)
                                {
                                    SPListItem listItem = list.Items.Add();
                                    listItem["Title"] = "Work Queue";
                                    listItem["URL"] = "/_layouts/epmlive/queuejobs.aspx";
                                    listItem["Description"] = "General EPM Live Queue";
                                    listItem["Category"] = "10) Utilities";
                                    listItem.Update();
                                }

                                LogMessage(null, MessageKind.SUCCESS, 4);
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
}
