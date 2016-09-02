﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections.Generic;
using System.Xml;
using System.Reflection;
using System.Data.SqlClient;
using System.Web;

namespace EPMLiveCore.Layouts.epmlive
{
    public partial class SaveFragment : LayoutsPageBase
    {
        private void SavePlannerFragments(SPList plannerFragmentList)
        {
            SPSite site = SPContext.Current.Site;
            SPWeb web = SPContext.Current.Web;

            SPSecurity.RunWithElevatedPrivileges(delegate()
               {
                   using (SPSite currentSite = new SPSite(site.ID))
                   {
                       using (SPWeb currentWeb = currentSite.OpenWeb(web.ID))
                       {
                           currentWeb.AllowUnsafeUpdates = true;
                           SPListItem plannerFragmentItem = null;

                           int itemId = Convert.ToInt32(Request["ID"]);
                           if (itemId > 0)
                           {
                               plannerFragmentItem = plannerFragmentList.GetItemById(itemId);
                               plannerFragmentItem["Title"] = txtFragmentName.Text;
                               plannerFragmentItem["Description"] = txtDescription.Text;
                               plannerFragmentItem["FragmentType"] = (chkPrivate.Checked) ? "Private" : "Public";
                           }
                           else
                           {
                               plannerFragmentItem = plannerFragmentList.Items.Add();
                               plannerFragmentItem["Title"] = txtFragmentName.Text;
                               plannerFragmentItem["Description"] = txtDescription.Text;
                               plannerFragmentItem["FragmentType"] = (chkPrivate.Checked) ? "Private" : "Public";
                               plannerFragmentItem["FragmentXML"] = hdnTaskFragmentXml.Value;
                               plannerFragmentItem["PlannerID"] = Convert.ToString(Request["PlannerID"]);
                           }

                           plannerFragmentItem.Update();
                       }
                   }
               });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    bool doesUserHasManageWebPermission = false;
                    SPList plannerFragmentsList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments"); ;
                    if (plannerFragmentsList != null)
                        doesUserHasManageWebPermission = plannerFragmentsList.DoesUserHavePermissions(SPContext.Current.Web.CurrentUser, SPBasePermissions.ManageWeb);
                    chkPrivate.Enabled = doesUserHasManageWebPermission;

                    try
                    {
                        int itemId = Convert.ToInt32(Request["ID"]);
                        if (itemId > 0)
                        {
                            //Load item
                            SPListItem fragmentItem = plannerFragmentsList.GetItemById(itemId);
                            txtFragmentName.Text = Convert.ToString(fragmentItem["Title"]);
                            txtDescription.Text = Convert.ToString(fragmentItem["Description"]);
                            if (Convert.ToString(fragmentItem["FragmentType"]).Equals("private", StringComparison.CurrentCultureIgnoreCase))
                                chkPrivate.Checked = true;
                            else
                                chkPrivate.Checked = false;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SPList plannerFragmentList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments");
                if (plannerFragmentList != null)
                {
                    SavePlannerFragments(plannerFragmentList);
                    
                    int itemId = Convert.ToInt32(Request["ID"]);
                    if (itemId > 0)
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeSaveFragmentPopup", "<script language='javascript' type='text/javascript'>closeSaveFragmentPopup('Fragment " + txtFragmentName.Text + " updated successfully');</script>");
                    else
                        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "closeSaveFragmentPopup", "<script language='javascript' type='text/javascript'>closeSaveFragmentPopup('Fragment " + txtFragmentName.Text + " saved successfully');</script>");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
