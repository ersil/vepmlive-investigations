﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using System.Web;
using System.Web.UI.WebControls;

namespace EPMLiveCore
{
    [ToolboxData("<{0}:BtnCreateNew runat=server></{0}:BtnCreateNew>")]
    public class BtnCreateNew : WebControl, INamingContainer
    {   
        private const string CREATE_NEW_ASYNC_DATA_PAGE = "/_layouts/epmlive/CreateNewAjaxDataHost.aspx?";
        private string _createNewDataHostPageUrl = string.Empty;

        public BtnCreateNew() { }

        protected override void Render(HtmlTextWriter writer)
        {
            bool doNotRender = false;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using (SPSite tSite = new SPSite(SPContext.Current.Web.Url))
                {
                    using (SPWeb tWeb = tSite.OpenWeb())
                    {
                        string setting = CoreFunctions.getConfigSetting(tWeb, "EPMLiveDisableCreateNew");
                        doNotRender = !string.IsNullOrEmpty(setting) ? bool.Parse(setting) : false;
                    }
                }
            });

            if (doNotRender)
            {
                return;
            }

            SPSite site = SPContext.Current.Site;
            SPWeb web = SPContext.Current.Web;
            _createNewDataHostPageUrl = site.MakeFullUrl(web.ServerRelativeUrl) + CREATE_NEW_ASYNC_DATA_PAGE;

            if (web.Lists.Cast<SPList>().Where(l => !l.Hidden).Count() > 0)
            {
                // render script and variables
                writer.Write("<script type=\"text/javascript\"> var createNewAsyncUrl = \"" + _createNewDataHostPageUrl + "\"; </script>");
                writer.Write("<script type=\"text/javascript\" src=\"" + site.MakeFullUrl(web.ServerRelativeUrl) + "/_layouts/epmlive/BtnCreateNew.js\"></script>");



                writer.Write("<A id=lnkCreateNew class=ms-socialNotif onclick=\"ToggleCreateNewMenu();return false;\" href=\"javascript:;\"><SPAN onlick=\"document.getElementById('lnkCreateNew').click()\"><SPAN onlick=\"document.getElementById('lnkCreateNew').click()\" style=\"POSITION: relative; WIDTH: 32px; DISPLAY: inline-block; HEIGHT: 32px; OVERFLOW: hidden\" ><IMG style=\"BORDER-BOTTOM: 0px; POSITION: absolute; BORDER-LEFT: 0px; BORDER-TOP: 0px; BORDER-RIGHT: 0px; LEFT: 0px !important\" src=\"/_layouts/epmlive/images/plus.png\"></SPAN><SPAN class=ms-socialNotif-text>Create New</SPAN></SPAN></A>");

                // render button
                // =====================================================
                /*writer.Write("<a id=\"lnkCreateNew\" style=\"height: 15px;" +
                                                            "color: #1f497d;" +
                                                            "padding-top: 4px;" +
                                                            "padding-right: 10px;" +
                                                            "padding-bottom: 4px;" +
                                                            "padding-left: 10px;" +
                                                            "vertical-align: middle;" +
                                                            "border-top-color: transparent;" +
                                                            "border-right-color: transparent;" +
                                                            "border-bottom-color: transparent;" +
                                                            "border-left-color: transparent;" +
                                                            "border-top-width: 1px;" +
                                                            "border-right-width: 1px;" +
                                                            "border-bottom-width: 1px;" +
                                                            "border-left-width: 1px;" +
                                                            "border-top-style: solid;" +
                                                            "border-right-style: solid;" +
                                                            "border-bottom-style: solid;" +
                                                            "border-left-style: solid;" +
                                                            "display: inline-block;" +
                                                            "white-space: nowrap;\"" +
                                                            "onmouseover=\"this.style.color = '#4F81BD';\"" +
                                                            "onmouseout=\"this.style.color = '#1f497d';\"" +
                                                            "href=\"#\"" +
                                                            "onclick=\"ToggleCreateNewMenu();return false;\">");
                writer.Write("<span onlick=\"document.getElementById('lnkCreateNew').click()\" style=\"background-image:url('/_layouts/images/menu-down.gif'); background-position: right center; background-repeat: no-repeat; padding-right:10px;\">");
                writer.Write("<span onlick=\"document.getElementById('lnkCreateNew').click()\" id=\"spnCreateNewText\" class=\"menu-item-text\" >+ Create New Item</span>");
                writer.Write("</span>");
                writer.Write("</a>");*/

                // html for actual menu
                // ===================================================
                writer.Write("<div id=\"divCreateNewMenu\" style=\"z-index: 103; position: absolute; display:none; max-height:450px; overflow-x:hidden;overflow-y:auto;\" dir=\"ltr\" class=\"ms-MenuUIPopupBody ms-MenuUIPopupScreen\" title=\"\" ismenu=\"true\" level=\"0\" _backgroundframeid=\"msomenuid6\" flipped=\"false\" leftforbackiframe=\"834\" topforbackiframe=\"29\">");
                writer.Write("<div style=\"overflow: visible\" class=\"ms-MenuUIPopupInner\" isinner=\"true\">");
                writer.Write("<div id=\"divCreateNewMenuAsync\" class=\"ms-MenuUI\" style=\"width:254px\">");
                // loading div
                // ========================================
                writer.Write("<div id=\"divCreateNewLoading\" style=\"width: 100%; text-align: center;padding-top:5px;padding-bottom:5px;\">");
                writer.Write("<img src=\"/_layouts/15/images/gears_anv4.gif\" style=\"vertical-align: middle\" />");
                writer.Write("Loading...");
                writer.Write("</div>");
                // =========================================
                writer.Write("</div>");
                writer.Write("</div>");
                writer.Write("</div>");
            }
        }
    }
}
