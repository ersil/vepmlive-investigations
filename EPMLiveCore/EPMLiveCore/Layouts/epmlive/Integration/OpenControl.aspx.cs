﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace EPMLiveCore.Layouts.epmlive.Integration
{
    public partial class OpenControl : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string intlistid = Request["INTID"];
            string intuid = Request["ID"];
            string control = Request["Control"];


            if (intuid != "")
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    API.Integration.IntegrationCore core = new API.Integration.IntegrationCore(Web.Site.ID, Web.ID);
                    SPListItem li = core.GetListItemFromExternalID(intlistid, intuid);
                    if (li != null)
                    {
                        RedirectTo(li, control);
                    }
                    else
                    {
                        lblError.Text = "Could not find item";
                    }
                });
            }
            else
            {
                RedirectTo(control);
            }
           
        }

        private void RedirectTo(string control)
        {
            switch (control)
            {
                case "mywork":
                    Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/mywork.aspx?isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    break;
                case "workplan":
                    Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/workplanner.aspx?CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    break;
                case "costplan":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/costs.aspx?CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "resplan":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/rpeditor.aspx?CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "costanalyzer":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/costanalyzer.aspx?CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "resanalyzer":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/rpanalyzer.aspx?CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                default:
                    lblError.Text = "Could not load control: " + control;
                    break;
            }
        }

        private void RedirectTo(SPListItem li, string control)
        {
            switch(control)
            {
                case "workspace":
                    try
                    {
                         
                        string url = li["WorkspaceUrl"].ToString();
                        if (url != "")
                        {
                            SPFieldUrlValue val = new SPFieldUrlValue(url);
                            Response.Redirect(val.Url);
                        }
                    }
                    catch { }

                    Microsoft.SharePoint.Utilities.SPUtility.Redirect("EPMLive/QueueCreateWorkspace.aspx?itemid=" + li.ID + "&listid=" + li.ParentList.ID + "&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);

                    break;
                case "workplan":
                    Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/workplanner.aspx?ID=" + li.ID + "&listid=" + li.ParentList.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    break;
                case "costplan":
                    {
                        SPWeb rweb = li.ParentList.ParentWeb.Site.RootWeb;
                        string EPKCostView = CoreFunctions.getConfigSetting(rweb, "EPK" + li.ParentList.Title.Replace(" ", "") + "_costview");
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/costs.aspx?itemid=" + li.ParentList.ParentWeb.ID + "." + li.ParentList.ID + "." + li.ID + "&epkurl=&view=" + EPKCostView + "&listid=" + li.ParentList.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "resplan":
                    {
                        SPWeb rweb = li.ParentList.ParentWeb.Site.RootWeb;
                        string EPKCostView = CoreFunctions.getConfigSetting(rweb, "EPK" + li.ParentList.Title.Replace(" ", "") + "_costview");
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("ppm/rpeditor.aspx?itemid=" + li.ParentList.ParentWeb.ID + "." + li.ParentList.ID + "." + li.ID + "&epkurl=&view=" + EPKCostView + "&listid=" + li.ParentList.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "team":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/People.aspx?ListId=" + li.ParentList.ID + "&id=" + li.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "comments":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/comments.aspx?ListId=" + li.ParentList.ID + "&itemid=" + li.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "worklog":
                    {
                        Microsoft.SharePoint.Utilities.SPUtility.Redirect("epmlive/worklog.aspx?ListId=" + li.ParentList.ID + "&itemid=" + li.ID + "&CloseMethod=3&isdlg=1", Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, System.Web.HttpContext.Current);
                    }
                    break;
                case "projectinfo":
                    {
                        string url = li.ParentList.Forms[PAGETYPE.PAGE_EDITFORM].ServerRelativeUrl + "?ID=" + li.ID + "&CloseMethod=3&isdlg=1";
                        Response.Redirect(url);
                    }
                    break;
                default:
                    lblError.Text = "Could not load control: " + control;
                    break;
            }
        }
    }
}
