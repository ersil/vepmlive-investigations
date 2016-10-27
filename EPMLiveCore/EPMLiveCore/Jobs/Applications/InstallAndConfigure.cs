﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using EPMLiveCore.Controls.Navigation.Providers;
using EPMLiveCore.Infrastructure;
using Microsoft.SharePoint;
using System.Data;
using System.IO;
using System.Data.SqlClient;

namespace EPMLiveCore.Jobs.Applications
{
    public class InstallAndConfigure : API.BaseJob
    {
        public void execute(SPSite osite, SPWeb oweb, string data)
        {
            if (string.IsNullOrEmpty(strConn))
            {
                strConn = EPMLiveCore.CoreFunctions.getConnectionString(osite.WebApplication.Id);
            }
            SqlConnection cn = new SqlConnection(strConn);
            try
            {
                cn.Open();
                DataTable dtMessages;
                int maxErrorLevel;

                bool bVerify = false;
                int iCommunityId = 0;

                bool.TryParse(DocData.FirstChild.Attributes["Verify"].Value, out bVerify);

                try
                {
                    iCommunityId = int.Parse(DocData.FirstChild.Attributes["Community"].Value);
                }
                catch { }

                SPList oList = oweb.Lists[base.ListUid];
                SPListItem oListItem = oList.GetItemById(base.ItemID);

                base.totalCount = 100;

                API.ApplicationInstaller installer = new API.ApplicationInstaller(oListItem["EXTID"].ToString(), cn, this);
                installer.InstallAndConfigureApp(bVerify, oweb, iCommunityId);

                string retMessage = installer.Message;
                maxErrorLevel = installer.MaxErrorLevel;
                dtMessages = installer.DtMessagesHTML;

                base.sErrors = installer.XmlMessages.OuterXml;

                if (installer.MaxErrorLevel > 1)
                {
                    base.bErrors = true;
                }

                // clear nav cache
                CacheStore.Current.RemoveSafely(oweb.Url, new CacheStoreCategory(oweb).Navigation);
            }
            catch (Exception ex)
            {
                base.bErrors = true;
                base.sErrors += "General Failure: " + ex.Message;
            }
            finally
            {
                if (cn != null)
                {
                    cn.Close();
                }

            }

        }

        public void SetPercent(float percent)
        {
            updateProgress(percent);
        }
    }
}
