﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web;

namespace EPMLiveCore.Layouts.epmlive.applications
{
    public partial class AddCommunity : LayoutsPageBase
    {
        protected string CommId = "";

        protected void Page_Load(object sender, EventArgs e)
        {

            Act act = new Act(Web);

            if(act.CheckFeatureLicense(ActFeature.AppsAndCommunities) != 0)
                Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/applications/noact.aspx", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);

        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {

                CommId = API.Applications.CreateCommunity(txtName.Text, Web).ToString();

                pnlDone.Visible = true;
                pnlMain.Visible = false;
                lblError.Visible = false;
            }
            catch(Exception ex)
            {
                lblError.Text = "Error: " + ex.Message + "<br>";
                lblError.Visible = true;
            }
        }
    }
}
