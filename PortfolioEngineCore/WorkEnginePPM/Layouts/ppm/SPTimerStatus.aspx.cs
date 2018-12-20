﻿using System;
using System.Data;
using System.Xml;
using System.Text;

using Microsoft.SharePoint.WebControls;

namespace WorkEnginePPM
{
    public partial class SPTimerStatus : LayoutsPageBase
    {
        private string url = "";
        private DataTable dtFields = new DataTable();

        protected void Page_Load(object sender, EventArgs e)
        {
            //this.Master.InitializePage();
            //this.Master.SetTitle("Timer Status");

            if (!IsPostBack)
            {
                //url = LMR_IF.getSiteUrl();
                //if (url != "")
                //{
                    //pnlNoUrl.Visible = false;
                    //lblUrl.Text = url;
                    populateStatus();
                //}
                //else
                //{
                //    pnlMain.Visible = false;
                //}
            }
        }

        private static bool ExecuteProcess(string sContext, string sXMLRequest, out XmlNode xNode)
        {
            return SPHelper.ExecuteProcess(sContext, sXMLRequest, out xNode);
        }

        private static bool ExecuteProcessEx(string sURL, string sContext, string sXMLRequest, out XmlNode xNode)
        {
            return SPHelper.ExecuteProcessEx(sContext, sXMLRequest, out xNode);
        }

        private void populateStatus()
        {
            XmlNode ndSettings = null;
            if (ExecuteProcess("GetSettings", "<Settings Key=\"EPK\" Schedule=\"10\"/>", out ndSettings) == false)
            {
                lblGeneralError.Text = "Error populateStatus : GetSettings";
                lblGeneralError.Visible = true;
                return;
            }
            if (ndSettings.Attributes["Status"].Value == "0")
            {
                XmlNode ndSchedule = ndSettings.SelectSingleNode("Schedule");
                if (ndSchedule != null)
                {
                    string[] sSchedule = ndSchedule.InnerText.Split('|');
                    if (sSchedule.Length > 2)
                        ddlTime.SelectedValue = sSchedule[2];
                }

                SPHelper.PopulateStatus(lblGeneralError, lblLastRun, lblLastRunResult, lblStatus);
            }
            else
            {
                lblGeneralError.Text = "Error Getting Settings: " + ndSettings.SelectSingleNode("Error").InnerText;
                lblGeneralError.Visible = true;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder("<Settings Key=\"EPK\" Schedule=\"10\">");
            sb.Append("<Schedule>");
            sb.Append("EPK Synch|10|");
            sb.Append(ddlTime.SelectedValue);
            sb.Append("|2|");
            sb.Append("</Schedule>");
            sb.Append("</Settings>");

            XmlNode ndSettings = null;
            if (ExecuteProcess("SetSettings", sb.ToString(), out ndSettings) == false)
            {
                lblGeneralError.Text = "Error btnSave_Click : SetSettings";
                lblGeneralError.Visible = true;
                return;
            }
            if (ndSettings.Attributes["Status"].Value == "0")
            {
                //Response.Redirect("SPTimerStatus.aspx");
                string url = Request.UrlReferrer.OriginalString;
                Response.Redirect(url);
            }
            else
            {
                lblGeneralError.Text = "Error Setting Timer: " + ndSettings.InnerText;
                lblGeneralError.Visible = true;
            }
        }

        //protected void btnCancel_Click(object sender, EventArgs e)
        //{
        //    Response.Redirect("adminpages.aspx");
        //}

        protected void btnRunNow_Click(object sender, EventArgs e)
        {
            XmlNode ndStatus = null;
            if (ExecuteProcess("RunTimer", "<Settings Key=\"EPK\" Schedule=\"10\"/>", out ndStatus) == false)
            {
                lblGeneralError.Text = "Error btnRunNow_Click : RunTimer";
                lblGeneralError.Visible = true;
                return;
            }
            if (ndStatus.Attributes["Status"].Value == "0")
            {
                //Response.Redirect("SPTimerStatus.aspx");   this kind of stmnt worked in PfE web site but bot in SP
                string url = Request.UrlReferrer.OriginalString;
                Response.Redirect(url);
            }
            else
            {
                lblGeneralError.Text = "Error Running Timer: " + ndStatus.SelectSingleNode("Error").InnerText;
                lblGeneralError.Visible = true;
            }
        }

    }
}