﻿using System;
using System.Web.UI;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Collections;

namespace WorkEnginePPM
{
    public partial class CostAnalyzerASPX : LayoutsPageBase
    {
        protected string strOutput;

        protected void Page_Load(object sender, EventArgs e)
        {
            EPMLiveCore.Act act = new EPMLiveCore.Act(Web);

            int activation = act.CheckFeatureLicense(EPMLiveCore.ActFeature.PortfolioEngine);
            if (activation != 0)
            {
                strOutput = act.translateStatus(activation);
                LiteralControl lit = new LiteralControl(strOutput.ToString());
                PlaceHolder1.Controls.Add(lit);
                return;
            }

            int i;

            SPList list = Web.Lists[new Guid(Request["listid"])];


            if (HelperFunctions.UseNonActiveXControl("costanalyzer", list) == false)
            {
                if (int.TryParse(Request["view"], out i))
                    strOutput = HelperFunctions.outputEPKControl(Request["epkurl"], "WE_LMRModeler.LMR_WE_Model", "<Params Ticket=\\\"" + Request["dataid"] + "\\\"  ViewID=\\\"" + Request["view"] + "\\\"/>", "true", Page);
                else
                    strOutput = HelperFunctions.outputEPKControl(Request["epkurl"], "WE_LMRModeler.LMR_WE_Model", "<Params Ticket=\\\"" + Request["dataid"] + "\\\"  ViewName=\\\"" + Request["view"] + "\\\"/>", "true", Page);
                LiteralControl lit = new LiteralControl(strOutput.ToString());
                PlaceHolder1.Controls.Add(lit);
            }
            else
            {
                bool useneewone = false;


                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    try
                    {
                        string sListName = list.Title;

                        using (SPSite site = new SPSite(Web.Site.ID))
                        {
                            SPWeb rootWeb = site.RootWeb;

                            string menus = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "epk" + sListName.Replace(" ", "") + "_menus");

                            ArrayList arrMenus = new ArrayList(menus.Split('|'));

                            if (arrMenus.Contains("costanalyzerv2"))
                                useneewone = true;
                        }
                    }
                    catch { }
                });

                if (useneewone)
                {
                    WorkEnginePPM.ControlTemplates.WorkEnginePPM.CostAnalyzerControl ctl = (WorkEnginePPM.ControlTemplates.WorkEnginePPM.CostAnalyzerControl)LoadControl("/_layouts/ppm/CostAnalyzer.ascx");
                    ctl.TicketVal = Request["dataid"];

                    if (int.TryParse(Request["view"], out i))
                        ctl.ViewIDVal = Request["view"];
                    else
                        ctl.ViewNameVal = Request["view"];

                    PlaceHolder1.Controls.Add(ctl);
                }
                else
                {
                    WorkEnginePPM.ControlTemplates.WorkEnginePPM.ModelControl ctl = (WorkEnginePPM.ControlTemplates.WorkEnginePPM.ModelControl)LoadControl("/_layouts/ppm/Model.ascx");

                    ctl.TicketVal = Request["dataid"];

                    if (int.TryParse(Request["view"], out i))
                        ctl.ViewIDVal = Request["view"];
                    else
                        ctl.ViewNameVal = Request["view"];

                    PlaceHolder1.Controls.Add(ctl);
                }
            }
        }
    }
}
