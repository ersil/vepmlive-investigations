﻿using System;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Serialization;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;

using System.ComponentModel;
using System.Text;
using System.Collections;
using System.Xml;

using Microsoft.Web.CommandUI;

using System.Text.RegularExpressions;

using System.Data;

using Microsoft.SharePoint.JsonUtilities;

using System.Reflection;
using ReportFiltering;

using System.Data.SqlClient;

namespace EPMLiveWebParts
{
    [ToolboxData("<{0}:GridViewWebpart runat=server></{0}:GridViewWebpart>")]
    [Guid("f816bcc6-82de-4bb6-98aa-408dd07bf62c")]
    [XmlRoot(Namespace = "GridListView")]
    public class GridListView : Microsoft.SharePoint.WebPartPages.WebPart, IWebPartPageComponentProvider
    {
        private IReportID _myProvider;


        private bool inEditMode = false;
        private bool titlefound = false;
        
        private EPMLiveCore.Act act;

        private GridGantUserControl ganttControl;
        
        //PROPS
        string strList;
        private string strView;
        private string strRollupList;
        private string error;
        private bool showMenu = false;

        private bool? boolShowViewToolbar;
        private bool? boolHideNewButton;
        private bool? boolUseDefaults;
        private bool? boolShowInsertRow;
        private bool? boolUseParent;

        private bool? boolAllowEdit;
        private bool? boolEdit;

        private bool? boolPerformance;
        private bool? boolUsePopup;
        private bool? boolShowSearch;
        private bool? boolLockSearch;

        private string strLinkType;
        private string strExecView;
        private bool? boolLockViewContext;
        private string strStart;
        private string strFinish;
        private string strMilestone;
        private string strProgress;
        private string strInformation;
        private string strWBS;
        protected string strDefaultControl;
        private string strMyDefaultControl;
        private string strRollupSites;

        private string strGroup1;
        private string strGroup2;
        private string strExpand;

        private bool bShowSearch = false;
        private bool bLockSearch = false;
        private bool bHasSearchResults = false;
        private string sSearchField = "";
        private string sSearchValue = "";
        private string sSearchType = "";

        private string useParent = "false";

        private bool disableNewButtonModification;
        
        
        private bool BOOLShowViewBar;
        //=======================================

        int activation;

        private ViewToolBar toolbar;

        private SPList list = null;
        private SPView view = null;

        private PeopleEditor peMulti;
        private PeopleEditor peSingle;

        private int filterIndex;

        bool hideNew = false;

        bool allowInsertRow = false;
        bool allowEditToggle = false;
        private bool hasList = false;
        //=========================Global Temp Props===============
        bool useNewMenu = false;
        //bool disableNew = false;
        string newMenuName = "";

        string rollupLists = "";

        string sFullGridId;

        private string sFullParamList = "";

        private string newGridMode = "";
        private int newMenuStyle = 0;
        private bool requestList = false;
        private bool bRollups = false;

        bool EPKEnabled = false;
        string EPKButtons = "";
        string EPKURL = "";
        string EPKView = "";
        string EPKCostView = "";

        string PlannerV2Menu = "";
        string PlannerV2CurPlanner = "";

        bool bIsFormWebpart = false;
        bool bIsLinkedItemView = false;
        string LookupFilterField = "";
        string LookupFilterValue = "";

        bool bAssociatedItems = false;

        bool bUsePopUp = false;

        bool? bContentReporting;

        #region Gantt Properties

        [ConnectionConsumer("Report ID Consumer", "ReportIDConsumer")]
        public void ReportIDConsumer(IReportID Provider)
        {
            _myProvider = Provider;
        }


        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropGroup1
        {
            
            get
            {
                return strGroup1;
            }
            set
            {
                strGroup1 = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropGroup2
        {
            get
            {
                return strGroup2;
            }
            set
            {
                strGroup2 = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropExpand
        {
            get
            {
                return strExpand;
            }
            set
            {
                strExpand = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropHideNewButton
        {
            get
            {
                if (boolHideNewButton == null)
                    return false;
                return boolHideNewButton;
            }
            set
            {
                boolHideNewButton = value;
            }
        }

        [Category("Grid Properties")]
        [Personalizable(true)]
        [WebPartStorage(Storage.Personal)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        [XmlElement(ElementName = "MyDefaultControl")]
        public string PropMyDefaultControl
        {
            get
            {
                if (strMyDefaultControl == null)
                {
                    return PropDefaultControl;
                }

                return strMyDefaultControl;
            }
            set
            {
                strMyDefaultControl = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropDefaultControl
        {
            get
            {
                if (strDefaultControl == null)
                    return "Grid";

                return strDefaultControl;
            }
            set
            {
                strDefaultControl = value;
            }
        }


        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropLockViewContext
        {
            get
            {
                if (boolLockViewContext == null)
                    if(SPContext.Current != null && SPContext.Current.List != null && SPContext.Current.ViewContext.View != null)
                        return true;
                    else
                        return false;

                return boolLockViewContext;
            }
            set
            {
                boolLockViewContext = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropExecView
        {
            get
            {
                return strExecView;
            }
            set
            {
                strExecView = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropList
        {
            get
            {
                if (PropLockViewContext.Value == true)
                {
                    try
                    {
                        if(SPContext.Current != null && SPContext.Current.ViewContext.View != null)
                            return SPContext.Current.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url;
                    }
                    catch { }
                    return strList;
                }
                else if (strList == null || strList == "")
                {
                    try
                    {
                        if(SPContext.Current != null && SPContext.Current.ViewContext.View != null)
                            return SPContext.Current.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url;
                    }
                    catch { }
                    return strList;
                }
                else
                    return strList;
            }
            set
            {
                strList = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropView
        {
            get
            {
                if (PropLockViewContext.Value == true)
                {
                    try
                    {
                        return SPContext.Current.ViewContext.View.Title;
                    }
                    catch { }
                    return strView;
                }
                else if (strView == null || strView == "")
                {
                    try
                    {
                        return SPContext.Current.ViewContext.View.Title;
                    }
                    catch { }
                    return strView;
                }
                else
                    return strView;
            }
            set
            {
                strView = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropRollupList
        {
            get
            {
                if (strRollupList == null)
                    return "";
                return strRollupList;
            }
            set
            {
                strRollupList = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropRollupSites
        {
            get
            {
                if (strRollupSites == null)
                    return "";
                return strRollupSites;
            }
            set
            {
                strRollupSites = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropStart
        {
            get
            {
                if (strStart == null)
                {
                    return "StartDate";
                }
                return strStart;
            }
            set
            {
                strStart = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropFinish
        {
            get
            {
                if (strFinish == null)
                {
                    return "DueDate";
                }
                return strFinish;
            }
            set
            {
                strFinish = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropMilestone
        {
            get
            {
                if (strMilestone == null)
                {
                    return "Milestone";
                }
                return strMilestone;
            }
            set
            {
                strMilestone = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropProgress
        {
            get
            {
                if (strProgress == null)
                {
                    return "PercentComplete";
                }
                return strProgress;
            }
            set
            {
                strProgress = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropInformation
        {
            get
            {
                return strInformation;
            }
            set
            {
                strInformation = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropWBS
        {
            get
            {
                if (strWBS == null)
                {
                    return "OutlineNumber";
                }
                return strWBS;
            }
            set
            {
                strWBS = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropShowViewToolbar
        {
            get
            {
                if (boolShowViewToolbar == null)
                    return true;
                return boolShowViewToolbar;
            }
            set
            {
                boolShowViewToolbar = value;
            }
        }
        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public string PropLinkType
        {
            get
            {
                return strLinkType;
            }
            set
            {
                strLinkType = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("PropUseDefaults")]
        [Description("PropUseDefaults")]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool? PropUseDefaults
        {
            get
            {
                if (boolUseDefaults == null)
                {
                    if (PropLinkType != null)
                        return false;
                    else
                        return true;
                }

                return boolUseDefaults.Value;
            }
            set
            {
                boolUseDefaults = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropAllowEdit
        {
            get
            {
                if (boolAllowEdit == null)
                    return false;
                return boolAllowEdit;
            }
            set
            {
                boolAllowEdit = value;
            }
        }


        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Content Reporting")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropContentReporting
        {
            get
            {
                if(bContentReporting == null)
                    return false;
                return bContentReporting;
            }
            set
            {
                bContentReporting = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropUsePopup
        {
            get
            {
                if (boolUsePopup == null)
                    return false;
                return boolUsePopup;
            }
            set
            {
                boolUsePopup = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropPerformance
        {
            get
            {
                if (boolPerformance == null)
                    return false;
                return boolPerformance;
            }
            set
            {
                boolPerformance = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropEdit
        {
            get
            {
                if (boolEdit == null)
                    return false;
                return boolEdit;
            }
            set
            {
                boolEdit = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("Custom String for Toolpart")]
        [Description("Used by the toolpart.")]
        [Browsable(false)]
        public bool? PropShowInsertRow
        {
            get
            {
                if (boolShowInsertRow == null)
                    return false;
                return boolShowInsertRow;
            }
            set
            {
                boolShowInsertRow = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("PropUseParent")]
        [Description("PropUseParent")]
        [Browsable(false)]
        [DefaultValue(true)]
        public bool? PropUseParent
        {
            get
            {
                if(boolUseParent == null)
                {
                    return false;
                }

                return boolUseParent.Value;
            }
            set
            {
                boolUseParent = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("PropShowSearch")]
        [Description("PropShowSearch")]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool? PropShowSearch
        {
            get
            {
                if(boolShowSearch == null)
                {
                    return false;
                }

                return boolShowSearch.Value;
            }
            set
            {
                boolShowSearch = value;
            }
        }

        [Category("Grid Properties")]
        [WebPartStorage(Storage.Shared)]
        [FriendlyNameAttribute("PropLockSearch")]
        [Description("PropLockSearch")]
        [Browsable(false)]
        [DefaultValue(false)]
        public bool? PropLockSearch
        {
            get
            {
                if(boolLockSearch == null)
                {
                    return false;
                }

                return boolLockSearch.Value;
            }
            set
            {
                boolLockSearch = value;
            }
        }
        #endregion

        public GridListView()
        {
            boolUseDefaults = true;
            
        }

        public WebPartContextualInfo WebPartContextualInfo
        {
            
            get
            {
                if(SPContext.Current.ViewContext.View != null && (SPContext.Current.List.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url == PropList || PropList == ""))
                {
                    string webPartPageComponentId = SPRibbon.GetWebPartPageComponentId(this);

                    WebPartContextualInfo info = new WebPartContextualInfo();
                    WebPartRibbonContextualGroup contextualGroup = new WebPartRibbonContextualGroup();
                    WebPartRibbonTab ribbonItemTab = new WebPartRibbonTab();
                    WebPartRibbonTab ribbonListTab = new WebPartRibbonTab();
                    //Create the contextual group object and initialize its values.
                    contextualGroup.Id = "Ribbon.ListContextualGroup";
                    contextualGroup.Command = "ListContextualGroup";
                    contextualGroup.VisibilityContext = "CustomContextualTab" + webPartPageComponentId + ".CustomVisibilityContext";

                    ribbonItemTab.Id = "Ribbon.ListItem";
                    ribbonItemTab.VisibilityContext = "GridViewListItemContextualTab" + webPartPageComponentId + ".CustomVisibilityContext";

                    ribbonListTab.Id = "Ribbon.List";
                    ribbonListTab.VisibilityContext = "GridViewListContextualTab" + webPartPageComponentId + ".CustomVisibilityContext";

                    info.ContextualGroups.Add(contextualGroup);
                    info.Tabs.Add(ribbonItemTab);
                    info.Tabs.Add(ribbonListTab);
                    info.PageComponentId = SPRibbon.GetWebPartPageComponentId(this);

                    return info;
                }
                else
                {
                    return null;
                }
            }
        }

        public string DelayScript
        {
            get
            {
                string url = SPContext.Current.Web.ServerRelativeUrl;
                string webPartPageComponentId = SPRibbon.GetWebPartPageComponentId(this);
                return @"
                <script type=""text/javascript"">
                //<![CDATA[
                            function _addCustomPageComponent()
                            {
                                var _customPageComponent = new ContextualTabWebPart.CustomPageComponent('" + webPartPageComponentId + @"',mygrid" + sFullGridId + ",myDataProcessor" + sFullGridId + @",'" + ((url == "/") ? "" : url) + @"','" + HttpContext.Current.Request.Url.AbsolutePath + @"');

                                var ribbonPageManager = SP.Ribbon.PageManager.get_instance();
                                ribbonPageManager.addPageComponent(_customPageComponent);
                                ribbonPageManager.get_focusManager().requestFocusForComponent(_customPageComponent);
                            }

                            function _registerCustomPageComponent()
                            {
                                SP.SOD.registerSod(""GridViewContextualTabPageComponent.js"", ""\/_layouts\/epmlive\/gridlistribbon.aspx"");
                                SP.SOD.executeFunc(""GridViewContextualTabPageComponent.js"", ""ContextualWebPart.CustomPageComponent"", _addCustomPageComponent);
                            }

                            SP.SOD.executeOrDelayUntilScriptLoaded(_registerCustomPageComponent, ""sp.ribbon.js"");
                //]]>
                </script>";
            }
        }

        private string getPjList(SPWeb web)
        {
            string planner = "";
            try
            {
                Guid lWeb = EPMLiveCore.CoreFunctions.getLockedWeb(web);
                string projectcenter = "";

                if (lWeb != web.ID)
                {
                    using (SPWeb tweb = web.Site.OpenWeb(lWeb))
                    {
                        projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLiveWPProjectCenter");
                    }
                }
                else
                {
                    projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLiveWPProjectCenter");
                }

                if (projectcenter.ToLower() == list.Title.ToLower())
                {
                    foreach (SPListItem li in list.Items)
                    {
                        if (web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)    
                            planner += "<Button Id=\"Ribbon.ListItem.EPMLive.LEditInProject\" Sequence=\"20\" Command=\"LEditInProject\" CommandValueId=\"" + list.ID + "." + li.ID + "\" LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                        else
                            planner += "<Button Id=\"Ribbon.ListItem.EPMLive.EditInPSProject\" Sequence=\"10\" Command=\"LEditInPSProject\"  LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" CommandValueId=\"" + list.ID + "." + li.ID + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/project2007logo.gif\"/>";
                    }
                }
                else
                {
                    SPList pList = web.Lists[projectcenter];
                    foreach (SPListItem li in pList.Items)
                    {
                        if (web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                            planner += "<Button Id=\"Ribbon.ListItem.EPMLive.LEditInProject\" Sequence=\"20\" Command=\"LEditInProject\" CommandValueId=\"" + pList.ID + "." + li.ID + "\" LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                        else
                            planner += "<Button Id=\"Ribbon.ListItem.EPMLive.EditInPSProject\" Sequence=\"10\" Command=\"LEditInPSProject\"  LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" CommandValueId=\"" + pList.ID + "." + li.ID + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/project2007logo.gif\"/>";
                    }
                }
            }
            catch { }
            return planner;
        }

        private string getPlannerList(string plannerName, SPWeb web, string pDisplay)
        {
            string planner = "";
            try
            {
                Guid lWeb = EPMLiveCore.CoreFunctions.getLockedWeb(web);
                string projectcenter = "";
                string taskcenter = "";
                bool enableWP = false;
                if (lWeb != web.ID)
                {
                    using (SPWeb tweb = web.Site.OpenWeb(lWeb))
                    {
                        projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "ProjectCenter");
                        taskcenter = EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "TaskCenter");
                        try
                        {
                            enableWP = bool.Parse(EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "Enable"));
                        }
                        catch { }
                    }
                }
                else
                {
                    projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "ProjectCenter");
                    taskcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "TaskCenter");
                    try
                    {
                        enableWP = bool.Parse(EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "Enable"));
                    }
                    catch { }

                }

                string preid = "";

                if (projectcenter.ToLower() == list.Title.ToLower() && enableWP)
                {
                    if (web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] != null)
                        preid = list.ID + ".";

                    foreach (SPListItem li in list.Items)
                    {
                        planner += "<Button Id=\"Ribbon.ListItem.EPMLive.Planner" + plannerName + "\" Sequence=\"20\" Command=\"LPlanner" + plannerName + "\" CommandValueId=\"" + preid + li.ID + "\" LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                    }
                }
                if (taskcenter.ToLower() == list.Title.ToLower() && enableWP)
                {
                    SPList pList = web.Lists[projectcenter];
                    
                    if (web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] != null)
                        preid = pList.ID + ".";

                    foreach (SPListItem li in pList.Items)
                    {
                        planner += "<Button Id=\"Ribbon.ListItem.EPMLive.Planner" + plannerName + "\" Sequence=\"20\" Command=\"LPlanner" + plannerName + "\" CommandValueId=\"" + preid + li.ID + "\" LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                    }
                }
            }
            catch { }
            return planner;
        }

        private string getEPKPlannerList(SPWeb web)
        {
            string planner = "";
            try
            {
                
                string taskcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web.Site.RootWeb, "EPKTaskCenter");

                if(taskcenter.ToLower() == list.Title.ToLower())
                {
                    SPField pField = list.Fields[EPMLiveCore.CoreFunctions.getConfigSetting(web.Site.RootWeb, "epktaskcenterprojectfield")];

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(pField.SchemaXml);

                    SPList pList = web.Lists[new Guid(doc.FirstChild.Attributes["List"].Value)];

                    foreach(SPListItem li in pList.Items)
                    {
                        planner += "<Button Id=\"Ribbon.ListItem.EPMLive.PlannerEPK\" Sequence=\"20\" Command=\"LPlannerPE\" CommandValueId=\"" + web.ID + "." + pList.ID + "." + li.ID + "\" LabelText=\"" + HttpUtility.HtmlEncode(li.Title) + "\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                    }
                }
            }
            catch { }
            return planner;
        }

        private string getPlanner(string plannerName, SPWeb web, string pDisplay, string image)
        {
            string planner = "";
            try
            {
                Guid lWeb = EPMLiveCore.CoreFunctions.getLockedWeb(web);
                string projectcenter = "";
                string taskcenter = "";
                bool enableWP = false;
                if (lWeb != web.ID)
                {
                    using (SPWeb tweb = web.Site.OpenWeb(lWeb))
                    {
                        projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "ProjectCenter");
                        taskcenter = EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "TaskCenter");
                        try
                        {
                            enableWP = bool.Parse(EPMLiveCore.CoreFunctions.getConfigSetting(tweb, "EPMLive" + plannerName + "Enable"));
                        }
                        catch { }
                    }
                }
                else
                {
                    projectcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "ProjectCenter");
                    taskcenter = EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "TaskCenter");
                    try
                    {
                        enableWP = bool.Parse(EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLive" + plannerName + "Enable"));
                    }
                    catch { }

                }

                if ((projectcenter.ToLower() == list.Title.ToLower()) && enableWP)
                {
                    planner = "<Button Id=\"Ribbon.ListItem.EPMLive.Planner" + plannerName + "\" Sequence=\"95\" Command=\"Planner" + plannerName + "\" LabelText=\"" + pDisplay + "\" TemplateAlias=\"o2\" Image16by16=\"" + image + "\"/>";
                }
            }
            catch { }
            return planner;
        }

        private string getPlanner(string plannerName, SPWeb web, string pDisplay)
        {
            return getPlanner(plannerName, web, pDisplay, "_layouts/images/epmlivelogosmall.gif");
        }

        private string getEPKButtons(SPSite site, Ribbon ribbon1, string language)
        {
            StringBuilder sb = new StringBuilder();

            string menus = "";
            menus = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPK" + list.Title.Replace(" ","") + "_menus");
            if(menus == "")
                menus = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPKMenus");

            ArrayList arrButtons = new ArrayList(menus.Split('|'));

            string noactivex = "";
            noactivex = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPK" + list.Title.Replace(" ", "") + "_nonactivexs");
            if(noactivex == "")
                noactivex = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "epknonactivexs");

            ArrayList arrActivex = new ArrayList(noactivex.Split('|'));


            XmlDocument ribbonExtensions;

            if (arrButtons.Contains("details"))
            {
                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKDetails""
                Sequence=""40""
                Command=""EPKSingleAction""
                CommandValueId=""PI Details""
                Image16by16=""/_layouts/" + language + @"/images/formatmap16x16.png"" Image16by16Top=""0"" Image16by16Left=""-200""
                LabelText=""Details""
                TemplateAlias=""o2""
              />");
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
            }
            if (arrButtons.Contains("costs"))
            {
                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKCosts""
                Sequence=""41""
                Command=""EPKSingleAction""
                CommandValueId=""Costs""
                Image32by32=""/_layouts/epmlive/images/editcosts.png""
                LabelText=""Edit Costs""
                TemplateAlias=""o1""
                />");
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
            }
            if (arrButtons.Contains("workplan"))
            {
                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKWorkPlan""
                Sequence=""42""
                Command=""EPKSingleAction""
                CommandValueId=""Workplan""
                Image16by16=""/_layouts/" + language + @"/images/formatmap16x16.png"" Image16by16Top=""-112"" Image16by16Left=""-128""
                LabelText=""Work Planner""
                TemplateAlias=""o2""
                />");
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
            }
            if (arrButtons.Contains("resplan"))
            {
                if(arrActivex.Contains("resplan"))
                {
                    ribbonExtensions = new XmlDocument();
                    ribbonExtensions.LoadXml(@"<Button
                    Id=""Ribbon.ListItem.Manage.EPKResourcePlanner""
                    Sequence=""43""
                    Command=""EPKMultiAction""
                    CommandValueId=""ResourcePlanner""
                    Image32by32=""/_layouts/" + language + @"/images/formatmap32x32.png"" Image32by32Top=""-352"" Image32by32Left=""-288""
                    LabelText=""Edit Resource Plan""
                    TemplateAlias=""o1""
                    />");
                    ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                }
                else
                {
                    ribbonExtensions = new XmlDocument();
                    ribbonExtensions.LoadXml(@"<Button
                    Id=""Ribbon.ListItem.Manage.EPKResourcePlanner""
                    Sequence=""44""
                    Command=""EPKSingleAction""
                    CommandValueId=""ResourcePlanner""
                    Image32by32=""/_layouts/" + language + @"/images/formatmap32x32.png"" Image32by32Top=""-352"" Image32by32Left=""-288""
                    LabelText=""Edit Resource Plan""
                    TemplateAlias=""o1""
                    />");
                    ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                }
            }
            if (arrButtons.Contains("portfolio"))
            {
                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml(@"<ToggleButton
                Id=""Ribbon.List.ViewFormat.EPKCostView""
                Sequence=""45""
                Command=""EPKCostView""
                Image32by32=""/_layouts/" + language + @"/images/formatmap32x32.png"" Image32by32Top=""-96"" Image32by32Left=""-256""
                LabelText=""Cost View""
                QueryCommand=""QueryEPKCostView""
                TemplateAlias=""o1""
                />");
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.ViewFormat.Controls._children");
            }


            sb.Append("<Group Id=\"Ribbon.ListItem.EPMLiveAnalyze\" Sequence=\"42\" Command=\"EPMLiveAnalyzeGroup\" Description=\"\" Title=\"Analyze\" Template=\"Ribbon.Templates.Flexible2\">");
            sb.Append("<Controls Id=\"Ribbon.ListItem.EPMLiveAnalyze.Controls\">");
            bool hasAction = false;
            if (arrButtons.Contains("costanalyzer"))
            {
                hasAction = true;
                sb.Append(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKCostAnalyzer""
                Sequence=""100""
                Command=""EPKMultiAction""
                CommandValueId=""CostAnalyzer""
                Image16by16=""/_layouts/epmlive/images/costanalyzer16.png""
                Image32by32=""/_layouts/epmlive/images/costanalyzer.png""
                LabelText=""Cost Analyzer""
                TemplateAlias=""o1""
                />");
                hasAction = true;
            }
            if (arrButtons.Contains("resanalyzer"))
            {
                sb.Append(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKResourceAnalyzer""
                Sequence=""101""
                Command=""EPKMultiAction""
                Image16by16=""/_layouts/" + language + @"/images/formatmap16x16.png"" Image16by16Top=""-96"" Image16by16Left=""-192""
                Image32by32=""/_layouts/" + language + @"/images/formatmap32x32.png"" Image32by32Top=""-256"" Image32by32Left=""-192""
                LabelText=""Resource Analyzer""
                TemplateAlias=""o1""
                />");
                hasAction = true;
            }

            if (arrButtons.Contains("modeler"))
            {
                sb.Append(@"<Button
                Id=""Ribbon.ListItem.Manage.EPKModeler""
                Sequence=""102""
                Command=""EPKMultiAction""
                Image16by16=""/_layouts/epmlive/images/target.gif""
                Image32by32=""/_layouts/epmlive/images/target.gif"" Image32by32Top=""8"" Image32by32Left=""8""
                LabelText=""Modeler""
                TemplateAlias=""o1""
                />");
                hasAction = true;
            }

            

            if (hasAction)
            {
                sb.Append("</Controls>");
                sb.Append("</Group>");
                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml(sb.ToString());
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Groups._children");

                ribbonExtensions = new XmlDocument();
                ribbonExtensions.LoadXml("<MaxSize Id=\"Ribbon.ListItem.EPMLiveAnalyze.MaxSize\" Sequence=\"10\" GroupId=\"Ribbon.ListItem.EPMLiveAnalyze\" Size=\"LargeMedium\" />");
                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Scaling._children");
            }

            return sb.ToString();
        }

        private void AddContextualTab()
        {
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                using(SPSite site = new SPSite(SPContext.Current.Web.Url))
                {
                    using(SPWeb web = site.OpenWeb())
                    {
                        EPMLiveCore.GridGanttSettings gSettings = new EPMLiveCore.GridGanttSettings(list);

                        var ribbon = SPRibbon.GetCurrent(Page);
                        if(ribbon != null)
                        {
                            ribbon.Minimized = true;
                            ribbon.CommandUIVisible = true;

                            //string initialTabId = "Ribbon.ListItem";
                            //if (newGridMode == "datasheet")
                            //    initialTabId = "Ribbon.List";
                            //if (!ribbon.IsTabAvailable(initialTabId))
                            //    ribbon.MakeTabAvailable(initialTabId);
                            ribbon.InitialTabId = "Ribbon.ListItem";

                            ribbon.MakeContextualGroupInitiallyVisible("Ribbon.ListContextualTab", "CustomContextualTab" + SPRibbon.GetWebPartPageComponentId(this) + ".CustomVisibilityContext");
                        }


                        //============Clean Up Ribbon
                        if(!list.EnableFolderCreation)
                        {
                            ribbon.TrimById("Ribbon.ListItem.New.NewFolder");
                        }

                        ribbon.TrimById("Ribbon.List.Actions.CreateVisioDiagram");
                        ribbon.TrimById("Ribbon.List.Actions.OpenWithAccess");
                        ribbon.TrimById("Ribbon.List.Actions.ExportToProject");


                        //=========================

                        string language = web.Language.ToString();

                        Microsoft.Web.CommandUI.Ribbon ribbon1 = SPRibbon.GetCurrent(this.Page);
                        XmlDocument ribbonExtensions = new XmlDocument();
                        ribbonExtensions.LoadXml(Properties.Resources.gridribbon.Replace("#language#", language));
                        ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.ViewFormat.Controls._children");

                        if(newGridMode == "gantt")
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml(Properties.Resources.txtGanttRibbon.Replace("#language#", language));
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Groups._children");

                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<MaxSize Id=\"Ribbon.List.EPMLiveGanttView.MaxSize\" Sequence=\"10\" GroupId=\"Ribbon.List.EPMLiveGanttView\" Size=\"LargeLarge\" />");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Scaling._children");

                        }

                        if(newGridMode != "datasheet")
                        {
                            ribbon.TrimById("Ribbon.List.Datasheet");
                        }

                        if(newGridMode == "datasheet" || newGridMode == "grid" || newGridMode == "")
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.List.CustomViews.Filters\" Sequence=\"4\" Command=\"ShowFilters\" Image16by16=\"/_layouts/images/fcofilter.png\" Image32by32=\"/_layouts/images/menufilter.gif\" LabelText=\"Hide/Show Search\" ToolTipTitle=\"Filters\" ToolTipDescription=\"\" TemplateAlias=\"o1\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.CustomViews.Controls._children");
                        }

                        ribbonExtensions = new XmlDocument();
                        ribbonExtensions.LoadXml("<Button Id=\"Ribbon.List.Datasheet.Save\" Sequence=\"10\" Command=\"DatasheetSave\" Image16by16=\"/_layouts/" + language + "/images/formatmap16x16.png\" Image16by16Top=\"-256\" Image16by16Left=\"0\" Image32by32=\"/_layouts/" + language + "/images/formatmap32x32.png\" Image32by32Top=\"-416\" Image32by32Left=\"-256\" LabelText=\"Save Items\" ToolTipTitle=\"Save Items\" ToolTipDescription=\"Save all items in grid.\" TemplateAlias=\"o1\"/>");
                        ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Datasheet.Controls._children");

                        if(newGridMode == "datasheet" || newGridMode == "grid" || newGridMode == "")
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.List.Datasheet.Print\" Sequence=\"10\" Command=\"PrintGrid\" Image16by16=\"/_layouts/epmlive/images/print.gif\" Image32by32=\"/_layouts/epmlive/images/printmenu.gif\" LabelText=\"Print\" ToolTipTitle=\"Print\" ToolTipDescription=\"\" TemplateAlias=\"o1\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Share.Controls._children");
                        }

                        if(newGridMode == "gantt")
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.List.Datasheet.Print\" Sequence=\"10\" Command=\"PrintGantt\" Image16by16=\"/_layouts/epmlive/images/print.gif\" Image32by32=\"/_layouts/epmlive/images/printmenu.gif\" LabelText=\"Print\" ToolTipTitle=\"Print\" ToolTipDescription=\"\" TemplateAlias=\"o1\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Share.Controls._children");
                        }

                        ribbonExtensions = new XmlDocument();
                        ribbonExtensions.LoadXml("<Button Id=\"Ribbon.List.Share.RefreshItems\" Sequence=\"1\" Command=\"RefreshItems\"  Image32by32=\"/_layouts/epmlive/images/refresh.png\" LabelText=\"Refresh Items\" ToolTipTitle=\"Refresh Items\" ToolTipDescription=\"Refresh all items in grid.\" TemplateAlias=\"o1\"/>");
                        ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Actions.Controls._children");

                        string pubPC = EPMLiveCore.CoreFunctions.getLockConfigSetting(web, "EPMLivePublisherProjectCenter", false);
                        string pubTC = EPMLiveCore.CoreFunctions.getLockConfigSetting(web, "EPMLivePublisherTaskCenter", false);
                        bool foundmpp = false;

                        try
                        {
                            SPDocumentLibrary lib = (SPDocumentLibrary)web.Lists["Project Schedules"];
                            if(lib != null)
                            {
                                if(lib.ContentTypesEnabled)
                                {
                                    foreach(SPContentType ct in lib.ContentTypes)
                                    {
                                        string template = ct.DocumentTemplateUrl;
                                        if(template.Substring(template.Length - 3, 3) == "mpp")
                                        {
                                            foundmpp = true;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    string template = lib.DocumentTemplateUrl;
                                    if(template.Substring(template.Length - 3, 3) == "mpp")
                                    {
                                        foundmpp = true;
                                    }
                                }
                            }
                        }
                        catch { }

                        if(PlannerV2Menu == "" && web.Site.Features[new Guid("e6df7606-1541-4bf1-a810-e8e9b11819e3")] == null)
                        {
                            string agile = getPlanner("Agile", web, "Agile Planner");
                            if(agile != "")
                            {
                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml(agile);
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                            }

                            string wp = "";
                            if(web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                                wp = getPlanner("WP", web, "Work Planner");
                            else
                                wp = getPlanner("PSWebApp", web, "Edit in Project Web App", "_layouts/images/project2007logosmall.gif");

                            if(wp != "")
                            {
                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml(wp);
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                            }

                            //string editinproject = "";
                            //string workspace = "";
                            

                            if(pubPC == list.Title)
                            {
                                if(web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                                {
                                    //if (wp != "")
                                    if(foundmpp)
                                    {
                                        //editinproject = "<Button Id=\"Ribbon.ListItem.EPMLive.EditInProject\" Sequence=\"10\" Command=\"EditInProject\" LabelText=\"Edit In Project\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/project2007logo.gif\"/>";
                                        ribbonExtensions = new XmlDocument();
                                        ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.EditInProject\" Sequence=\"90\" Command=\"EditInProject\" LabelText=\"Edit In Project\" TemplateAlias=\"o2\" Image16by16=\"_layouts/images/Project2007LogoSmall.gif\"/>");
                                        ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                                    }
                                }
                                else
                                {
                                    //editinproject = "<Button Id=\"Ribbon.ListItem.EPMLive.EditInPSProject\" Sequence=\"10\" Command=\"EditInPSProject\" LabelText=\"Edit In Project Professional\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/project2007logo.gif\"/>";
                                    ribbonExtensions = new XmlDocument();
                                    ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.EditInPSProject\" Sequence=\"90\" Command=\"EditInPSProject\" LabelText=\"Edit In Project Professional\" TemplateAlias=\"o2\" Image16by16=\"_layouts/images/Project2007LogoSmall.gif\"/>");
                                    ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                                }
                            }
                        }
                        if(bRollups || requestList)
                        {
                            //workspace = "<Button Id=\"Ribbon.ListItem.EPMLive.GoToWorkspace\" Sequence=\"12\" Command=\"GoToWorkspace\" LabelText=\"Go To Workspace\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>";
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.GoToWorkspace\" Sequence=\"12\" Command=\"GoToWorkspace\" LabelText=\"Go To Workspace\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/spgraphic.gif\" Image32by32Top=\"7\" Image32by32Left=\"4\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Actions.Controls._children");
                        }

                        if(requestList)
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.CreateWorkspace\" Sequence=\"13\" Command=\"CreateWorkspace\" LabelText=\"Create Workspace\" TemplateAlias=\"o1\" Image32by32=\"_layouts/images/epmlivelogo.gif\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Actions.Controls._children");

                        }


                        //if (agile != "" || wp != "" || editinproject != "" || workspace != "" || requestList)
                        //{
                        //    StringBuilder sb = new StringBuilder();

                        //    sb.Append("<Group Id=\"Ribbon.ListItem.EPMLive\" Sequence=\"50\" Command=\"ListItemEPMLiveGroup\" Description=\"\" Title=\"WorkEngine\" Template=\"Ribbon.Templates.Flexible2\">");
                        //    sb.Append("<Controls Id=\"Ribbon.ListItem.EPMLive.Controls\">");
                        //    if (editinproject != "")
                        //        sb.Append(editinproject);
                        //    if (wp != "")
                        //        sb.Append(wp);
                        //    if (agile != "")
                        //        sb.Append(agile);
                        //    if (workspace != "")
                        //        sb.Append(workspace);

                        //    sb.Append("</Controls>");
                        //    sb.Append("</Group>");

                        //    ribbonExtensions = new XmlDocument();
                        //    ribbonExtensions.LoadXml(sb.ToString());
                        //    ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Groups._children");

                        //    ribbonExtensions = new XmlDocument();
                        //    ribbonExtensions.LoadXml("<MaxSize Id=\"Ribbon.ListItem.EPMLive.MaxSize\" Sequence=\"10\" GroupId=\"Ribbon.ListItem.EPMLive\" Size=\"LargeLarge\" />");
                        //    ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Scaling._children");
                        //}
                        bool disableProject = false;
                        bool.TryParse(EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLiveDisablePublishing"), out disableProject);
                        bool disablePlan = false;
                        bool.TryParse(EPMLiveCore.CoreFunctions.getConfigSetting(web, "EPMLiveDisablePlanners"), out disablePlan);

                        if(PlannerV2Menu != "")
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml(PlannerV2Menu);
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                        }
                        //SPList cList = web.Lists.TryGetList("Comments");
                        //if(cList != null)
                        {
                            ribbonExtensions = new XmlDocument();
                            ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.EditComments\" Sequence=\"140\" Command=\"EditComments\" LabelText=\"Comments\" TemplateAlias=\"o2\" Image16by16=\"_layouts/epmlive/images/comments16.gif\"/>");
                            ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                        }

                        if(gSettings.BuildTeam && list.Fields.GetFieldByInternalName("AssignedTo") != null)
                        {
                            if(list.DoesUserHavePermissions(SPBasePermissions.EditListItems))
                            {
                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml("<Button Id=\"Ribbon.ListItem.EPMLive.BuildTeam\" Sequence=\"150\" Command=\"BuildTeam\" LabelText=\"Edit Team\" TemplateAlias=\"o2\" Image16by16=\"_layouts/epmlive/images/buildteam16.gif\"/>");
                                ribbon.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                            }
                        }

                        {
                            string pj = "";
                            string lwp = "";
                            string lagile = "";

                            if(!disablePlan)
                            {
                                lagile = getPlannerList("Agile", web, "Agile Planner");

                                if(web.Site.Features[new Guid("91f0c887-2db2-44b2-b15c-47c69809c767")] != null)
                                {
                                    if(web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                                        lwp = getPlannerList("WP", web, "Work Planner");
                                    else
                                        lwp = getPlannerList("PSWebApp", web, "Edit in Project Web App");
                                }

                            }

                            if(!disableProject && (foundmpp && list.Title == pubTC) || web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] != null)
                            {
                                pj = getPjList(web);
                            }

                            string lepk = "";

                            if(!disablePlan && web.Site.Features[new Guid("158c5682-d839-4248-b780-82b4710ee152")] != null && EPMLiveCore.CoreFunctions.getConfigSetting(web.Site.RootWeb, "EPKTaskCenter").ToLower() == list.Title.ToLower())
                            {
                                lepk = getEPKPlannerList(web);
                            }

                            StringBuilder sb = new StringBuilder();

                            if(lwp != "" || lagile != "" || pj != "" || lepk != "")
                            {
                                sb.Append("<Group Id=\"Ribbon.List.EPMLive\" Sequence=\"41\" Command=\"ListEPMLiveGroup\" Description=\"\" Title=\"WorkEngine\" Template=\"Ribbon.Templates.Flexible2\">");
                                sb.Append("<Controls Id=\"Ribbon.List.EPMLive.Controls\">");

                                if(pj != "" && PlannerV2Menu == "")
                                {
                                    if(web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                                    {
                                        sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInProject\" Sequence=\"10\" Command=\"ListEPMLiveEditInProject\" Image32by32=\"_layouts/images/project2007logo.gif\" LabelText=\"Edit In Project\" TemplateAlias=\"o1\">");
                                        sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInProject.Menu\">");
                                        sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInProject.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                        sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInProject.Menu.Scope.Controls\">");
                                        sb.Append(pj);
                                        sb.Append("</Controls>");
                                        sb.Append("</MenuSection>");
                                        sb.Append("</Menu>");
                                        sb.Append("</FlyoutAnchor>");
                                    }
                                    else
                                    {
                                        sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInPSProject\" Sequence=\"10\" Command=\"ListEditInPSProject\" Image32by32=\"_layouts/images/project2007logo.gif\" LabelText=\"Edit In Project Professional\" TemplateAlias=\"o1\">");
                                        sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInPSProject.Menu\">");
                                        sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInPSProject.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                        sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInPSProject.Menu.Scope.Controls\">");
                                        sb.Append(pj);
                                        sb.Append("</Controls>");
                                        sb.Append("</MenuSection>");
                                        sb.Append("</Menu>");
                                        sb.Append("</FlyoutAnchor>");
                                    }
                                }

                                if(lwp != "" && PlannerV2Menu == "")
                                {

                                    if(web.Features[new Guid("ebc3f0dc-533c-4c72-8773-2aaf3eac1055")] == null)
                                    {
                                        sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInWorkplanner\" Sequence=\"11\" Command=\"ListEPMLiveEditWP\" Image32by32=\"_layouts/images/epmlivelogo.gif\" LabelText=\"Edit In Workplanner\" TemplateAlias=\"o1\">");
                                        sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInWorkPlanner.Menu\">");
                                        sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInWorkplanner.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                        sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInWorkplanner.Menu.Scope.Controls\">");
                                        sb.Append(lwp);
                                        sb.Append("</Controls>");
                                        sb.Append("</MenuSection>");
                                        sb.Append("</Menu>");
                                        sb.Append("</FlyoutAnchor>");
                                    }
                                    else
                                    {
                                        sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInWorkplanner\" Sequence=\"11\" Command=\"ListEPMLiveEditPSWebApp\" Image32by32=\"_layouts/images/project2007logo.gif\" LabelText=\"Edit In Project Web App\" TemplateAlias=\"o1\">");
                                        sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInWorkPlanner.Menu\">");
                                        sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInWorkplanner.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                        sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInWorkplanner.Menu.Scope.Controls\">");
                                        sb.Append(lwp);
                                        sb.Append("</Controls>");
                                        sb.Append("</MenuSection>");
                                        sb.Append("</Menu>");
                                        sb.Append("</FlyoutAnchor>");
                                    }
                                }
                                if(lagile != "")
                                {
                                    sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInAgilePlanner\" Sequence=\"11\" Command=\"ListEPMLiveEditAgile\" Image32by32=\"_layouts/images/epmlivelogo.gif\" LabelText=\"Edit In Agile Planner\" TemplateAlias=\"o1\">");
                                    sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInAgilePlanner.Menu\">");
                                    sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInAgilePlanner.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                    sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInAgilePlanner.Menu.Scope.Controls\">");
                                    sb.Append(lagile);
                                    sb.Append("</Controls>");
                                    sb.Append("</MenuSection>");
                                    sb.Append("</Menu>");
                                    sb.Append("</FlyoutAnchor>");
                                }
                                if(lepk != "")
                                {
                                    sb.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.EditInPePlanner\" Sequence=\"12\" Command=\"ListEPMLiveEditPE\" Image32by32=\"/_layouts/" + language + "/images/formatmap32x32.png\" Image32by32Top=\"-384\" Image32by32Left=\"-32\" LabelText=\"Edit In Work Planner\" TemplateAlias=\"o1\">");
                                    sb.Append("<Menu Id=\"Ribbon.List.EPMLive.EditInPePlanner.Menu\">");
                                    sb.Append("<MenuSection Id=\"Ribbon.List.EPMLive.EditInPePlanner.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                    sb.Append("<Controls Id=\"Ribbon.List.EPMLive.EditInPePlanner.Menu.Scope.Controls\">");
                                    sb.Append(lepk);
                                    sb.Append("</Controls>");
                                    sb.Append("</MenuSection>");
                                    sb.Append("</Menu>");
                                    sb.Append("</FlyoutAnchor>");
                                }

                                sb.Append("</Controls>");
                                sb.Append("</Group>");

                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml(sb.ToString());
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Groups._children");

                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml("<MaxSize Id=\"Ribbon.List.EPMLive.MaxSize\" Sequence=\"10\" GroupId=\"Ribbon.List.EPMLive\" Size=\"LargeLarge\" />");
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.List.Scaling._children");
                            }
                        }

                        if(EPKEnabled)
                        {
                            //ribbon.TrimById("Ribbon.ListItem.Manage.EditProperties");

                            getEPKButtons(site, ribbon1, language);

                            //ribbonExtensions = new XmlDocument();
                            //ribbonExtensions.LoadXml(getEPKButtons(site).Replace("#language#",language));
                            //ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");

                        }


                        if(bAssociatedItems)
                        {
                            StringBuilder sbLists = new StringBuilder();

                            foreach(SPList cList in web.Lists)
                            {
                                try
                                {
                                    foreach(SPField field in cList.Fields)
                                    {
                                        if(field.Type == SPFieldType.Lookup)
                                        {
                                            SPFieldLookup fl = (SPFieldLookup)field;

                                            if(fl.LookupList.ToLower() == "{" + list.ID.ToString().ToLower() + "}")
                                            {
                                                EPMLiveCore.GridGanttSettings gSets = new EPMLiveCore.GridGanttSettings(cList);

                                                if(gSets.AssociatedItems)
                                                {
                                                    //sbLists.Append("<Button Id=\"Ribbon.ListItem.EPMLive.LinkedItemsButton\" Sequence=\"20\" Command=\"");
                                                    sbLists.Append("<Button Sequence=\"20\" Command=\"");
                                                    //if(!bRollups)
                                                    sbLists.Append("LinkedItemsButton");
                                                    //else
                                                    //    sbLists.Append("LinkedItemsButtonRollup");
                                                    sbLists.Append("\" Id=\"Ribbon.ListItem.EPMLive.LinkedItemsButton.");
                                                    sbLists.Append(HttpUtility.HtmlEncode(cList.Title));
                                                    sbLists.Append(".");
                                                    sbLists.Append(field.InternalName);
                                                    sbLists.Append("\" LabelText=\"");
                                                    sbLists.Append(HttpUtility.HtmlEncode(cList.Title));
                                                    sbLists.Append("\" TemplateAlias=\"o1\" Image16by16=\"");
                                                    sbLists.Append(cList.ImageUrl);
                                                    sbLists.Append("\"/>");
                                                }
                                                break;
                                            }
                                        }
                                    }
                                }
                                catch { }
                            }

                            if(sbLists.ToString() != "")
                            {
                                StringBuilder sbLinkedItems = new StringBuilder();

                                sbLinkedItems.Append("<Group Id=\"Ribbon.ListItem.EPMLive.Associated\" Sequence=\"41\" Command=\"LinkedItems\" Description=\"\" Title=\"Associated Items\" Template=\"Ribbon.Templates.Flexible2\">");
                                sbLinkedItems.Append("<Controls Id=\"Ribbon.ListItem.EPMLive.Associated.Controls\">");

                                sbLinkedItems.Append(sbLists);

                                sbLinkedItems.Append("</Controls>");
                                sbLinkedItems.Append("</Group>");

                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml(sbLinkedItems.ToString());
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Groups._children");

                                ribbonExtensions = new XmlDocument();
                                ribbonExtensions.LoadXml("<MaxSize Id=\"Ribbon.ListItem.EPMLive.Associated.MaxSize\" Sequence=\"10\" GroupId=\"Ribbon.ListItem.EPMLive.Associated\" Size=\"MediumMedium\" />");
                                ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Scaling._children");



                                
                                //sbLinkedItems.Append("<FlyoutAnchor Id=\"Ribbon.List.EPMLive.LinkedItems\" Sequence=\"39\" Command=\"");

                                ////if(!bRollups)
                                //sbLinkedItems.Append("LinkedItems");
                                ////else
                                ////    sbLinkedItems.Append("LinkedItemsRollup");

                                //sbLinkedItems.Append("\" Image32by32=\"/_layouts/epmlive/images/linkeditems.gif\" LabelText=\"Associated Items\" TemplateAlias=\"o1\">");
                                //sbLinkedItems.Append("<Menu Id=\"Ribbon.List.EPMLive.LinkedItems.Menu\">");
                                //sbLinkedItems.Append("<MenuSection Id=\"Ribbon.List.EPMLive.LinkedItems.Menu.Scope\" Sequence=\"10\" DisplayMode=\"Menu16\">");
                                //sbLinkedItems.Append("<Controls Id=\"Ribbon.List.EPMLive.LinkedItems.Menu.Scope.Controls\">");
                                //sbLinkedItems.Append(sbLists);
                                //sbLinkedItems.Append("</Controls>");
                                //sbLinkedItems.Append("</MenuSection>");
                                //sbLinkedItems.Append("</Menu>");
                                //sbLinkedItems.Append("</FlyoutAnchor>");


                                //ribbonExtensions = new XmlDocument();
                                //ribbonExtensions.LoadXml(sbLinkedItems.ToString());
                                //ribbon1.RegisterDataExtension(ribbonExtensions.FirstChild, "Ribbon.ListItem.Manage.Controls._children");
                            }
                        }
                    }
                }
            });
        }

        protected override void OnPreRender(EventArgs e)
        {
            
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                buildParams();
            });

            if(newGridMode.ToLower() == "gantt")
            {
                ScriptLink.Register(Page, "/_layouts/epmlive/treegrid/GridE.js", false);
                ScriptLink.Register(Page, "/_layouts/epmlive/GanttGrid.js", false);
            }

            base.OnPreRender(e);
            
            string webPartPageComponentId = SPRibbon.GetWebPartPageComponentId(this);

            if (!Page.ClientScript.IsClientScriptBlockRegistered(base.GetType(), "listviewwebpart" + webPartPageComponentId))
                Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "listviewwebpart" + webPartPageComponentId, "<script language=\"javascript\">var mygrid" + sFullGridId + ";var myDataProcessor" + sFullGridId + ";</script>");

            if(!Page.ClientScript.IsClientScriptBlockRegistered(base.GetType(), "listviewwebpart"))
            {
                //TODO: fix JS code

                Page.ClientScript.RegisterClientScriptBlock(base.GetType(), "listviewwebpart", "<script language=\"javascript\">" + Properties.Resources.txtRibbonScripts + "</script>");


            }

            if (SPContext.Current.ViewContext.View != null && list.ID == SPContext.Current.List.ID)
            {

                AddContextualTab();

                ClientScriptManager clientScript = this.Page.ClientScript;

                clientScript.RegisterClientScriptBlock(this.GetType(), "ContextualWebPart", this.DelayScript);
            }


            


            CssRegistration.Register("/_layouts/epmlive/dhtml/xgrid/dhtmlxgrid.css");
            CssRegistration.Register("/_layouts/epmlive/dhtml/xgrid/dhtmlxgrid_skins.css");
            CssRegistration.Register("/_layouts/epmlive/dhtml/calendar/dhtmlxcalendar.css");
            
            CssRegistration.Register("/_layouts/epmlive/dhtml/xcombo/dhtmlxcombo.css");
            CssRegistration.Register("/_layouts/epmlive/modal/modalmain.css");

            //LiteralControl javascriptRef = new LiteralControl("<script type='text/javascript' src='/_layouts/epmlive/DHTML/xgrid/dhtmlxcommon.js'></script>");
            //Page.Header.Controls.Add(javascriptRef);
            //javascriptRef = new LiteralControl("<script type='text/javascript' src='/_layouts/epmlive/DHTML/xgrid/dhtmlxgrid.js'></script>");
            //Page.Header.Controls.Add(javascriptRef); 

            //ScriptManager.RegisterClientScriptInclude(Page, this.GetType(), "dhtmlxcommon.js", "");
            //ScriptManager.RegisterClientScriptInclude(Page, this.GetType(), "dhtmlxgrid.js", "/_layouts/epmlive/DHTML/xgrid/dhtmlxgrid.js");
            
        }

        protected override void OnLoad(EventArgs e)
        {
            sFullGridId = this.ZoneIndex + this.ZoneID;
        }

        protected override void CreateChildControls()
        {

            
            if(SPContext.Current.ViewContext.View != null)
            {
                try
                {
                    typeof(ListTitleViewSelectorMenu).GetField("m_wpSingleInit", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Page.FindControl("ctl00$PlaceHolderPageTitleInTitleArea$ctl01$ctl00").Controls[1], true);
                }
                catch { }
                try
                {
                    typeof(ListTitleViewSelectorMenu).GetField("m_wpSingle", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(Page.FindControl("ctl00$PlaceHolderPageTitleInTitleArea$ctl01$ctl00").Controls[1], true);
                }
                catch { }
            }

            try
            {


                if(PropLinkType == null)
                    boolUseDefaults = true;

                EnsureChildControls();
                
                SPWeb web = SPContext.Current.Web;

                act = new EPMLiveCore.Act(web);
                activation = act.CheckFeatureLicense(EPMLiveCore.ActFeature.WebParts);
                //activation = EPMLiveCore.CoreFunctions.getFeatureLicenseStatus(0);
                if(activation != 0)
                    return;


                toolbar = new ViewToolBar();
                toolbar.EnableViewState = false;
                

                //if (!Page.IsPostBack)
                {
                    try
                    {
                        list = web.GetListFromUrl(PropList);
                        view = list.Views[PropView];
                    }
                    catch { }

                    if(PropUseDefaults.Value && list != null)
                    {
                        EPMLiveCore.GridGanttSettings gSettings = new EPMLiveCore.GridGanttSettings(list);
                        BOOLShowViewBar = gSettings.ShowViewToolbar;
                    }
                    else
                    {
                        BOOLShowViewBar = PropShowViewToolbar.Value;
                    }


                }

                string switchto = "";
                try
                {
                    switchto = Page.Request["gridmode"];
                }
                catch { }
                if(switchto != "" && switchto != null && this.ID == Page.Request["webpartid"])
                {
                    if(switchto == "gantt")
                        PropMyDefaultControl = "Gantt";
                    else if(switchto == "grid")
                        PropMyDefaultControl = "Grid";
                    else if(switchto == "cost")
                        PropMyDefaultControl = "Cost";

                    newGridMode = switchto.ToLower();

                    SetPersonalizationDirty();

                }
                else
                    newGridMode = PropMyDefaultControl.ToLower();

                buildParams();

                if(BOOLShowViewBar && view != null && list != null)
                {
                    if(PropMyDefaultControl == "Gantt")
                        toolbar.TemplateName = "GanttViewToolBar";

                    SPContext context = SPContext.GetContext(this.Context, view.ID, list.ID, web);
                    toolbar.RenderContext = context;

                    Controls.Add(toolbar);

                    foreach(Control control in toolbar.Controls)
                    {
                        processControls(control, list.Title, list.ID.ToString(), view.ID.ToString(), PropMyDefaultControl, this.ID, sFullGridId, hideNew);
                    }
                }

                //if(newGridMode.ToLower() == "gantt")
                //{

                //    ganttControl = (GridGantUserControl)Page.LoadControl(@"~/_CONTROLTEMPLATES/GridGantUserControl.ascx");
                //    ganttControl.ganttParams = sFullParamList;
                //    Controls.Add(ganttControl);

                //}

            }
            catch (Exception ex)
            {
                error = "Error (CreateChildControls): " + ex.Message.ToString() + ex.StackTrace;
            }
            peMulti = new PeopleEditor();
            peMulti.MultiSelect = true;
            peMulti.ID = "userpicker";
            this.Controls.Add(peMulti);

            peSingle = new PeopleEditor();
            peSingle.MultiSelect = false;
            peSingle.ID = "userpickersingle";
            this.Controls.Add(peSingle);

        }


        private void processControls(Control parentControl, string listname, string listid, string viewid, string defaultcontrol, string webpartid, string ZoneIndex, bool hideNew)
        {
            foreach (Control childControl in parentControl.Controls)
            {
                if(childControl.ToString() == "System.Web.UI.WebControls.HyperLink")
                {
                    if(childControl.ID == "imgZoomIn")
                    {
                        HyperLink hl = (HyperLink)childControl;
                        hl.NavigateUrl = "Javascript:GanttZoomIn('" + sFullGridId + "');";
                    }
                    if(childControl.ID == "imgZoomOut")
                    {
                        HyperLink hl = (HyperLink)childControl;
                        hl.NavigateUrl = "Javascript:GanttZoomOut('" + sFullGridId + "');";
                    }
                    if(childControl.ID == "hlGanttScrollTo")
                    {
                        HyperLink hl = (HyperLink)childControl;
                        hl.NavigateUrl = "Javascript:GanttScrollTo('" + sFullGridId + "');";
                    }
                }
                if (childControl.ToString() == "System.Web.UI.WebControls.Label")
                {
                    if (childControl.ID == "lblFilter")
                    {
                        //System.Web.UI.WebControls.HyperLink hl = (System.Web.UI.WebControls.HyperLink)childControl;
                        //hl.NavigateUrl = "Javascript:switchFilter" + ZoneIndex + "('" + hl.ClientID + "');";

                        try
                        {
                            System.Web.UI.WebControls.Label lblFilterText = (System.Web.UI.WebControls.Label)parentControl.FindControl("lblFilterText");
                            System.Web.UI.WebControls.Label lblLink = (System.Web.UI.WebControls.Label)childControl;
                            //lblLink.Text = "<a onclick=\"Javascript:switchFilter" + sFullGridId + "('" + lblFilterText.ClientID + "');\">";
                            lblLink.Text = "<a onclick=\"Javascript:mygrid" + sFullGridId + ".toggleSearch();\">";
                            
                            filterIndex = parentControl.Parent.Controls.IndexOf(parentControl);


                        }
                        catch { }
                    }
                }
                if (childControl.ToString().ToUpper() == "MICROSOFT.SHAREPOINT.WEBCONTROLS.NEWMENU")
                {
                    if (hideNew)
                    {
                        childControl.Visible = false;
                    }
                    else if (useNewMenu)
                    {

                        if(hasList)
                        {
                            NewMenu menu = (NewMenu)childControl;
                            try
                            {
                                Microsoft.SharePoint.WebControls.MenuItemTemplate mnu = menu.GetMenuItem("New0");
                                mnu.ClientOnClickNavigateUrl = "javascript:newAppPopup('" + list.ID.ToString().ToUpper() + @"');";
                                string txt = mnu.Text;
                                int spaceloc = txt.IndexOf(" ");
                                txt = txt.Substring(0, spaceloc);
                                if(newMenuName == "")
                                    mnu.Text = txt + " " + listname;
                                else
                                    mnu.Text = txt + " " + newMenuName;
                                menu.MenuControl.ClientOnClickScript = "javascript:newAppPopup('" + list.ID.ToString().ToUpper() + @"');";
                            }
                            catch { }
                        }
                        else
                        {
                            NewMenu menu = (NewMenu)childControl;
                            try
                            {
                                Microsoft.SharePoint.WebControls.MenuItemTemplate mnu = menu.GetMenuItem("New0");
                                mnu.ClientOnClickNavigateUrl = SPContext.Current.Web.Url + "/_layouts/epmlive/newapp.aspx?List=" + listid;
                                string txt = mnu.Text;
                                int spaceloc = txt.IndexOf(" ");
                                txt = txt.Substring(0, spaceloc);
                                if(newMenuName == "")
                                    mnu.Text = txt + " " + listname;
                                else
                                    mnu.Text = txt + " " + newMenuName;
                                menu.MenuControl.ClientOnClickScript = "location.href='" + mnu.ClientOnClickNavigateUrl + "'";
                            }
                            catch { }
                        }
                    }
                    else if(listname != "Project Center" && listname != "Project Center Rollup" && rollupLists != "" && !disableNewButtonModification)
                    {
                        NewMenu menu = (NewMenu)childControl;
                        try
                        {
                            menu.GetMenuItem("New0").Visible = false;
                            menu.MenuControl.NavigateUrl = "";
                            menu.MenuControl.ClientOnClickScript = "";
                            string[] arrrolluplists = rollupLists.Split(',');
                            foreach(string rolluplist in arrrolluplists)
                            {
                                string[] arrrolluplist = rolluplist.Split('|');
                                string image = "";
                                try
                                {
                                    image = "/_layouts/images/" + arrrolluplist[1];
                                }
                                catch { }
                                menu.AddMenuItem("New1", "New " + arrrolluplist[0].Trim() + " Item", "", "", SPContext.Current.Web.Url + "/_layouts/epmlive/newitem.aspx?List=" + arrrolluplist[0] + "&Source=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()), "");
                            }
                        }
                        catch { }
                    }
                    else
                    {
                        NewMenu menu = (NewMenu)childControl;
                        MenuItemTemplate template = menu.GetMenuItem("New0");
                        try
                        {
                            //if(!bUsePopUp)
                            {
                                if(template.ClientOnClickScript.StartsWith("javascript:NewItem2("))
                                {
                                    template.ClientOnClickScript = "newItem" + sFullGridId + "(" + bUsePopUp.ToString().ToLower() + ");";
                                    menu.MenuControl.ClientOnClickScript = "newItem" + sFullGridId + "(" + bUsePopUp.ToString().ToLower() + ");";
                                }
                            }
                        }catch{}
                    }
                }

                if (childControl.ToString().ToUpper() == "MICROSOFT.SHAREPOINT.WEBCONTROLS.ACTIONSMENU")
                {
                    ActionsMenu menu = (ActionsMenu)childControl;
                    if (rollupLists != "")
                    {
                        try { menu.GetMenuItem("EditInGridButton").Visible = false; }
                        catch { }
                        try { menu.GetMenuItem("ExportToDatabase").Visible = false; }
                        catch { }
                        if (rollupLists != "")
                        {
                            try
                            {
                                menu.GetMenuItem("ViewRSS").Visible = false;
                            }
                            catch { }
                            try
                            {
                                menu.GetMenuItem("SubscribeButton").Visible = false;
                            }
                            catch { }
                        }
                        try
                        {
                            SPWeb web = SPContext.Current.Web;
                            MenuItemTemplate mnu = menu.GetMenuItem("ExportToSpreadsheet");
                            mnu.ClientOnClickScript = "location.href='" + web.ServerRelativeUrl + "/_layouts/epmlive/rollupexport.aspx?List=" + listid + "&View=" + viewid + "&Lists=" + HttpUtility.UrlEncode(rollupLists.Replace(",", ";")) + "'";
                        }
                        catch { }
                    }

                    string url = HttpContext.Current.Request.Url.AbsolutePath;
                    if (url.Contains("?"))
                        url += "&";
                    else
                        url += "?";
                    
                    string mainurl = url + "webpartid=" + webpartid;

                    if(!String.IsNullOrEmpty(Page.Request["lookupfield"]))
                        mainurl += "&lookupfield=" + Page.Request["lookupfield"];

                    if(!String.IsNullOrEmpty(Page.Request["LookupFieldList"]))
                        mainurl += "&LookupFieldList=" + Page.Request["LookupFieldList"];

                    if (defaultcontrol.ToLower() == "grid")
                    {
                        menu.AddMenuItem("PrintGrid", "Print Grid", "/_layouts/epmlive/images/printmenu.GIF", "View printable list.", "", "printgrid" + ZoneIndex + "()");

                        menu.AddMenuItem("ViewInGantt", "View In Gantt", "/_layouts/epmlive/images/menuganttview.GIF", "View list using EPM Live Gantt.", mainurl + "&gridmode=gantt", "");

                        if (allowEditToggle)
                        {
                            if (inEditMode)
                            {
                                menu.AddMenuItem("SwitchToView", "Switch To View Mode", "/_layouts/epmlive/images/menugridview.GIF", "View list using EPM Live Grid.", mainurl + "&gridmode=grid", "");
                            }
                            else
                            {
                                menu.AddMenuItem("SwitchToEdit", "Switch To Edit Mode", "/_layouts/images/menudatasheet.gif", "Edit list using the grid view.", mainurl + "&gridmode=datasheet", "");
                            }
                        }
                    }
                    else if (defaultcontrol.ToLower() == "gantt")
                    {
                        menu.AddMenuItem("ViewInGrid", "View In Grid", "/_layouts/epmlive/images/menugridview.GIF", "View list using EPM Live Grid.", mainurl + "&gridmode=grid", "");
                    }
                }

                processControls(childControl, listname, listid, viewid, defaultcontrol, webpartid, ZoneIndex, hideNew);
            }
        }

        protected override void RenderWebPart(HtmlTextWriter output)
        {
            
            output.Write(error);
            try
            {
                HideListView();

                if (activation != 0)
                {
                    output.Write(act.translateStatus(activation));
                    return;
                }

                SPWeb web = SPContext.Current.Web;
                {

                    //buildParams();
                    //string switchto = "";
                    
                    

                    if (view != null)
                    {
                        showMenu = view.ViewFields.Exists("LinkTitle") || view.ViewFields.Exists("LinkFilename");
                    }

                    EPMLiveCore.GridGanttSettings gSettings = new EPMLiveCore.GridGanttSettings(list);

                    if (PropUseDefaults.Value && list != null)
                    {

                        BOOLShowViewBar = gSettings.ShowViewToolbar;
                    }
                    else
                    {
                        BOOLShowViewBar = PropShowViewToolbar.Value;
                        //hideNew = PropHideNewButton.Value;
                        //rollupLists = PropRollupList;
                        //inEditMode = PropEdit.Value;
                        //allowInsertRow = PropShowInsertRow.Value;
                        //allowEditToggle = PropAllowEdit.Value;
                    }

                    //try
                    //{
                    //    useNewMenu = bool.Parse(props[17]);
                    //}
                    //catch { }
                    //try
                    //{
                    //    disableNewButtonModification = bool.Parse(props[16]);
                    //}
                    //catch { }
                    //try
                    //{
                    //    newMenuName = props[18];
                    //}
                    //catch { }

                    //string webpartid = "";
                    //try
                    //{
                    //    // switchto = Page.Request["switchto"];
                    //    webpartid = Page.Request["webpartid"].ToString();
                    //    newGridMode = Page.Request["gridmode"].ToString();
                    //}
                    //catch { }
                    //if (webpartid == this.ID)
                    //{
                    //    try
                    //    {
                    //        if (newGridMode == "datasheet")
                    //            inEditMode = true;
                    //        else if (newGridMode != "")
                    //            inEditMode = false;
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        error = "Error Saving Personalization: " + ex.Message;
                    //    }
                    //}
                    if (BOOLShowViewBar && view != null && list != null)
                    {
                        this.Controls.Add(toolbar);
                        if (PropMyDefaultControl == "Gantt")
                            toolbar.TemplateName = "GanttViewToolBar";
                        else
                            toolbar.TemplateName = "GridViewToolBar";
                    }


                    //if (rollupLists != "")
                    //    allowInsertRow = false;
                    //if (inEditMode)
                    //    newGridMode = "datasheet";

                    //================================================================
                    web.Site.CatchAccessDeniedException = false;
                    //EnsureChildControls();
                    output.WriteLine("<style>");
                    output.WriteLine(".ms-menutoolbar {display:;}");
                    output.WriteLine("</style>");

                    if (list != null && view != null)
                    {
                        if (BOOLShowViewBar)
                        {
                            SPContext context = SPContext.GetContext(this.Context, view.ID, list.ID, web);
                            toolbar.RenderContext = context;

                            Controls.Add(toolbar);

                            //if (PropRollupList != "")
                            try
                            {
                                //foreach (Control control in toolbar.Controls)
                                //{
                                //    processControls(control, list.Title, list.ID.ToString(), view.ID.ToString(), PropMyDefaultControl, this.ID, sFullGridId, hideNew);
                                //}
                                if (inEditMode && PropMyDefaultControl != "Gantt")
                                {
                                    Panel pnl = new Panel();
                                    pnl.Controls.Add(new LiteralControl("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"ms-toolbar\" valign=\"center\" nowrap><div class=\"ms-buttoninactivehover\" onmouseover=\"this.className='ms-buttonactivehover'\" onmouseout=\"this.className='ms-buttoninactivehover'\" onclick=\"javascript:saveGrid" + sFullGridId + "();\">"));
                                    pnl.Controls.Add(new LiteralControl("<a><img src=\"_layouts/images/SAVEITEM.GIF\" border=\"0\" align=\"absmiddle\"> Save Data</a>"));
                                    pnl.Controls.Add(new LiteralControl("</div></td></tr></table>"));

                                    toolbar.Controls[0].Controls[1].Controls[0].Controls.AddAt(toolbar.Controls[0].Controls[1].Controls[0].Controls.Count - 1, pnl);
                                }
                                if (PropMyDefaultControl.ToLower() == "grid")
                                {
                                    Panel pnl = new Panel();
                                    pnl.Controls.Add(new LiteralControl("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"ms-toolbar\" style=\"height:100%\" valign=\"center\" nowrap><div class=\"ms-buttoninactivehover\" onmouseover=\"this.className='ms-buttonactivehover'\" onmouseout=\"this.className='ms-buttoninactivehover'\" onclick=\"javascript:mygrid" + sFullGridId + ".toggleSearch();\">"));
                                    //pnl.Controls.Add(new LiteralControl("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\"><tr><td class=\"ms-toolbar\" style=\"height:100%\" valign=\"center\" nowrap><div class=\"ms-buttoninactivehover\" onmouseover=\"this.className='ms-buttonactivehover'\" onmouseout=\"this.className='ms-buttoninactivehover'\" onclick=\"javascript:mygrid" + sFullGridId + ".switchFilter('fA" + sFullGridId + "');\">"));
                                    pnl.Controls.Add(new LiteralControl("<a id=\"fA" + sFullGridId + "\"><img src=\"/_layouts/epmlive/images/gridfilter.gif\" border=\"0\" align=\"absmiddle\" > Show Search</a>"));
                                    pnl.Controls.Add(new LiteralControl("</div></td></tr></table>"));

                                    toolbar.Controls[0].Controls[1].Controls[0].Controls.AddAt(toolbar.Controls[0].Controls[1].Controls[0].Controls.Count - 1, pnl);
                                }
                                //<asp:Panel runat="server" id="pnlFilter"><table border="0" cellpadding="0" cellspacing="0"><tr><td class="ms-toolbar" nowrap="true"><div class="ms-buttoninactivehover" onmouseover="this.className='ms-buttonactivehover'" onmouseout="this.className='ms-buttoninactivehover'"><asp:Label id="lblFilter" runat="server"/><img align='absmiddle' alt="" src="/_layouts/images/filter.gif" style='border-width:0px;'>&nbsp;<asp:Label id="lblFilterText" runat="server" Text="Show Filters"/></a></div></td></tr></table></asp:Panel>
                                
                                toolbar.RenderControl(output);
                            }
                            catch { }
                        }

                        if(hasList)
                        {
                            output.WriteLine("<script language=\"javascript\">");
                            output.WriteLine("function newAppPopup(list, itemid){");
                            output.WriteLine("    if(itemid)");
                            output.WriteLine("      var layoutsUrl = SP.Utilities.Utility.getLayoutsPageUrl('EPMLive/CreateNewWorkspace.aspx?list=' + list + '&type=site&CopyFrom=' + itemid);");
                            output.WriteLine("    else");
                            output.WriteLine("      var layoutsUrl = SP.Utilities.Utility.getLayoutsPageUrl('EPMLive/CreateNewWorkspace.aspx?list=' + list + '&type=site');");
                            output.WriteLine("    var urlBuilder = new SP.Utilities.UrlBuilder(layoutsUrl);");
                            output.WriteLine("    var tUrl = urlBuilder.get_url();");

                            output.WriteLine("    var options = { url: tUrl, title: 'Create', allowMaximize: false, width: 800, height: 600, dialogReturnValueCallback : Function.createDelegate(null, HandleCreateNewWorkspaceCreate) };");

                            output.WriteLine("    SP.UI.ModalDialog.showModalDialog(options);");

                            output.WriteLine("    function HandleCreateNewWorkspaceCreate(result, value) {");
                            output.WriteLine("        if (result == '1') {");
                            output.WriteLine("            window.location.href = value;");
                            output.WriteLine("        }");
                            output.WriteLine("    }");
                            output.WriteLine("}");
                            output.WriteLine("</script>");
                        }

                        if (newGridMode != "")
                        {
                            switch (newGridMode)
                            {
                                case "datasheet":
                                case "grid":
                                    renderGrid(output, web);
                                    addNewButton(output, web);
                                    break;
                                case "gantt":
                                    renderGantt(output, web);
                                    addNewButton(output, web);
                                    break;
                                case "cost":
                                    renderCost(output, web);
                                    break;
                            };
                        }
                        else
                        {
                            newGridMode = "grid";
                            renderGrid(output, web);
                            addNewButton(output, web);
                        }

                        //if (PropMyDefaultControl.ToLower() == "grid")
                        //    renderGrid(output, web);
                        //else if (PropMyDefaultControl.ToLower() == "gantt")
                        //    renderGantt(output, web);
                    }

                    

                    
                }
            }
            catch (Exception ex)
            {
                output.Write("Error (Render WebPart): " + ex.Message);
            }

        }
        private void HideListView()
        {
            if(SPContext.Current.ViewContext.View != null)
            {
                foreach(System.Web.UI.WebControls.WebParts.WebPart wp in WebPartManager.WebParts)
                {
                    try
                    {
                        if(wp.ToString() == "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart" || wp.ToString() == "Microsoft.SharePoint.WebPartPages.ListViewWebPart")
                        {
                            Microsoft.SharePoint.WebPartPages.XsltListViewWebPart wp2 = (Microsoft.SharePoint.WebPartPages.XsltListViewWebPart)wp;
                            wp2.XmlDefinition = wp2.XmlDefinition.Replace("<Toolbar Type=\"Standard\"/>", "<Toolbar Type=\"Standard\" ShowAlways=\"TRUE\"/>");
                            wp.Visible = false;
                            break;
                        }
                    }
                    catch { }
                }
            }
        }

        private string postItems(SPWeb web, out bool error)
        {
            SPWeb rootWeb = web.Site.RootWeb;

            string username = EPMLiveCore.CoreFunctions.GetCleanUserName(web.CurrentUser.LoginName, web.Site);
            string basePath = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "epkbasepath");
            string ppmId = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "ppmpid");
            string ppmCompany = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "ppmcompany");
            string ppmDbConn = EPMLiveCore.CoreFunctions.getConfigSetting(rootWeb, "ppmdbconn");

            if(basePath != "")
            {

                string ids = "";


                XmlDocument xmlQuery = new XmlDocument();
                xmlQuery.LoadXml("<Query>" + view.Query + "</Query>");

                XmlNode ndWhere = xmlQuery.SelectSingleNode("//Where");
                string query = "";

                if(ndWhere != null)
                {
                    query = ndWhere.OuterXml;
                }

                if(rollupLists != "")
                {
                    foreach(string rlist in rollupLists.Split(','))
                    {
                        DataTable dt = EPMLiveCore.CoreFunctions.getSiteItems(web, view, query, Page.Request["FilterField1"], "", rlist, new string[0]);

                        foreach(DataRow dr in dt.Rows)
                        {
                            ids += "," + dr["WEBID"].ToString().Trim("{}".ToCharArray()) + "." + dr["LISTID"].ToString().Trim("{}".ToCharArray()) + "." + dr["ID"].ToString();
                        }
                    }
                }
                else
                {
                    DataTable dt = EPMLiveCore.CoreFunctions.getSiteItems(web, view, query, Page.Request["FilterField1"], "", "", new string[0]);

                    foreach(DataRow dr in dt.Rows)
                    {
                        ids += "," + dr["WEBID"].ToString().Trim("{}".ToCharArray()) + "." + dr["LISTID"].ToString().Trim("{}".ToCharArray()) + "." + dr["ID"].ToString();
                    }
                }

                ids = ids.Trim(',');

                
                
                string ret = "";

                SPSecurity.RunWithElevatedPrivileges(delegate(){
                    PortfolioEngineCore.WEIntegration.WEIntegration integration = new PortfolioEngineCore.WEIntegration.WEIntegration(basePath, username, ppmId, ppmCompany, ppmDbConn);

                    ret = integration.DisplayProjects("<Display>" + ids + "</Display>");
                });

                XmlDocument doc = new XmlDocument();
                doc.LoadXml(ret);

                string status = doc.SelectSingleNode("//STATUS").InnerText;
                if(status == "0")
                {
                    error = false;
                    return doc.SelectSingleNode("//DisplayProjects").Attributes["SessionId"].Value;
                }
                else
                {
                    error = true;
                    return "Error: " + doc.SelectSingleNode("//Error").InnerText;
                }
            }
            error = true;
            return "Error: EPK basePath not defined.";
        }

        private void renderCost(HtmlTextWriter output, SPWeb web)
        {
            bool postErrors = false;
            string ret = postItems(web, out postErrors);

            if(!postErrors)
            {
                int i;
                if(int.TryParse(EPKView, out i))
                    output.WriteLine(WorkEnginePPM.HelperFunctions.outputEPKControl(EPKURL, "WE_LMRPort.LMR_WE", "<Params Ticket=\\\"" + ret + "\\\" ViewID=\\\"" + EPKView + "\\\"/>", "true", Page));
                else
                    output.WriteLine(WorkEnginePPM.HelperFunctions.outputEPKControl(EPKURL, "WE_LMRPort.LMR_WE", "<Params Ticket=\\\"" + ret + "\\\" ViewName=\\\"" + EPKView + "\\\"/>", "true", Page));
            }
            else
                output.Write(ret);

            output.WriteLine("<script language=\"javascript\">");
            output.WriteLine("mygrid" + sFullGridId + " = new Object();");
            output.WriteLine("var myDataProcessor" + sFullGridId + " = new Object();");

            addGridProperties(output, web);

            output.WriteLine(@"mygrid" + sFullGridId + @".getSelectedRowId = function(){return 0;};");
            output.WriteLine(@"mygrid" + sFullGridId + @".getUserData = function(rowid, key){ return """"; };");
            output.WriteLine(@"mygrid" + sFullGridId + @".getCheckedIds = function(){return """";};");

            //output.WriteLine("mygrid" + sFullGridId + @".getGlobalCommands = function($arr){return fnGetGlobalCommands($arr);};");
            //output.WriteLine("mygrid" + sFullGridId + @".canHandleCommand = function(this.$Grid, commandId){return fnCanHandleCommand(this.$Grid, commandId);};");
            //output.WriteLine("mygrid" + sFullGridId + @".addFocusedCommands = function($arr){return fnFocusedCommands($arr);};");
            //output.WriteLine("mygrid" + sFullGridId + @".JSGridEvents = function(name){return fnJSGridEvents(name);};");
            output.WriteLine("</script>");
        }

        private void renderGantt(HtmlTextWriter output, SPWeb web)
        {
            //ganttControl.RenderControl(output);


            output.Write("<script type=\"text/javascript\" src=\"_layouts/epmlive/modal/modal.js\"></script> ");


            int iHeight = 0;

            string suffix = "";

            if(!string.IsNullOrEmpty(this.Height))
            {
                MatchCollection mc = Regex.Matches(this.Height, @"^\d+");

                iHeight = int.Parse(mc[0].Value);

                try
                {
                    suffix = Regex.Matches(this.Height.Substring(mc[0].Value.Length), @"\w+")[0].Value;
                }
                catch { }
            }

            if(BOOLShowViewBar)
                iHeight -= 45;

            if((!hideNew && SPContext.Current.ViewContext.View != null) || (BOOLShowViewBar && !hideNew) || (!hideNew && bIsFormWebpart))
            {
                iHeight -= 30;
            }

            string sHeight = "";
            if(iHeight != 0)
                sHeight = "height: " + iHeight.ToString() + suffix;

            output.WriteLine("<div id=\"griddiv" + sFullGridId + "\" style=\"width:100%;" + sHeight + "\"><treegrid Data_Url=\"" + web.Url + "/_layouts/epmlive/getganttitems.aspx?data=" + sFullParamList + "\" Debug=\"\"/></div>");

            output.Write("<div  width=\"100%\" id=\"loadinggrid" + this.ID + "\" align=\"center\">");
            output.Write("<img src=\"_layouts/images/gears_anv4.gif\" style=\"vertical-align: middle;\"/> Loading Items...");
            output.Write("</div>");

            output.WriteLine("<script language=\"javascript\">");
            output.WriteLine("Grids.OnRenderFinish = function(grid){grid.ActionZoomFit();};");
            output.WriteLine("Grids.OnReady = function(grid, start)  { document.getElementById('loadinggrid" + this.ID + "').style.display = 'none'; }");
            output.WriteLine("Grids.OnRenderFinish = function(grid)  { clickTab(); }");  
            

            output.WriteLine("ArrGantts.push('GanttGrid" + sFullGridId + "');");
            output.WriteLine("mygrid" + sFullGridId + " = new Object();");
            output.WriteLine("var myDataProcessor" + sFullGridId + " = new Object();");

            output.WriteLine("mygrid" + sFullGridId + "._singleItemUrl = \"" + web.Url + "/_layouts/epmlive/getsingleitem.aspx?data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "\";");

            //CUSTOM PROPERTIES

            addGridProperties(output, web);

            output.WriteLine("mygrid" + sFullGridId + @".setRowHidden = function(rowid){
                                                                            var grid = Grids.GanttGrid" + sFullGridId + @";
                                                                            grid.HideRow(grid.GetRowById(rowid), true, false);
                                                                            };");

            output.WriteLine("mygrid" + sFullGridId + @".getTitles = function(){
                                                                        var titles = """";
                                                                        try{
                                                                        var grid = Grids.GanttGrid" + sFullGridId + @";
                                                                        var sRows = grid.GetSelRows();
                                                                        var fRow = grid.FRow;

                                                                        for(var sRow in sRows)
                                                                        {
                                                                            var row = sRows[sRow];
                                                                            if(row.itemid != """")
                                                                            {
                                                                                titles += "","" + row.Title;
                                                                            }
                                                                        }

                                                                        if(fRow)
                                                                            titles += "","" + fRow.Title;

                                                                        if(titles != """" && titles[0] == ',')
                                                                            titles = titles.substring(1);
                                                                        return titles;
                                                                        }catch(e){}
                                                                        };");


            output.WriteLine("mygrid" + sFullGridId + @".getCheckedIds = function(){

                                                                                    var ids = """";
                                                                                    try{
                                                                                    var grid = Grids.GanttGrid" + sFullGridId + @";

                                                                                    var row = grid.FRow;
                                                                                    //for(var sRow in sRows)
                                                                                    {
                                                                                        //var row = sRows[sRow];
    
                                                                                        if(row.itemid != """")
                                                                                        {
                                                                                            ids += "","" + row.webid + ""."" + row.listid + ""."" + row.itemid;
                                                                                        }
                                                                                    }

                                                                                    if(ids != """" && ids[0] == ',')
                                                                                        ids = ids.substring(1);
                                                                                    return ids;
                                                                                    }catch(e){return """";}
                                                                                    };");


            output.WriteLine("mygrid" + sFullGridId + @".getCheckedRowIds = function()
                                                        {
                                                            try{
                                                            var grid = Grids.GanttGrid" + sFullGridId + @";
                                                            return grid.FRow.id;
                                                            }catch(e){}
                                                        };");

            output.WriteLine("mygrid" + sFullGridId + @".getSelectedRowId = function()
                                                        {
                                                            try{
                                                            var grid = Grids.GanttGrid" + sFullGridId + @";
                                                            return grid.FRow.id;
                                                            }catch(e){}
                                                        };");
            output.WriteLine("mygrid" + sFullGridId + @".getUserData = function(rowid, key){  
                                                                    try
                                                                    {
                                                                    var grid = Grids.GanttGrid" + sFullGridId + @";
                                                                    var row = grid.GetRowById(rowid);
                                                                    return grid.GetValue(row, key);
                                                                    }catch(e){}
                                                                    };");
            //output.WriteLine("mygrid" + sFullGridId + @".getGlobalCommands = function($arr){return fnGetGlobalCommands($arr);};");
            //output.WriteLine("mygrid" + sFullGridId + @".canHandleCommand = function(this.$Grid, commandId){return fnCanHandleCommand(this.$Grid, commandId);};");
            //output.WriteLine("mygrid" + sFullGridId + @".addFocusedCommands = function($arr){return fnFocusedCommands($arr);};");
            //output.WriteLine("mygrid" + sFullGridId + @".JSGridEvents = function(name){return fnJSGridEvents(name);};");
                                    output.WriteLine(@"function newItem" + sFullGridId  + @"(usepopup)
                                    {
	                                    var wurl = mygrid" + sFullGridId + @"._newitemurl;

	                                    if(usepopup)
	                                    {
		                                    function NewItemCallback(dialogResult, returnValue){if(dialogResult){window.location.href=window.location.href;}}

		                                    var options = { url: wurl, width: 700, dialogReturnValueCallback:NewItemCallback };

		                                    SP.UI.ModalDialog.showModalDialog(options);
	                                    }
	                                    else
	                                    {
		                                    location.href = wurl;
	                                    }
                                    }");

            output.WriteLine("</script>");

            output.Write(Properties.Resources.txtGridSaving.Replace("#gridid#", sFullGridId).Replace("#siteurl#", web.Url));




            output.Write("<script>");
            output.WriteLine("function maximizeWindow(){try{");
            output.WriteLine("var currentDialog = SP.UI.ModalDialog.get_childDialog();");
            output.WriteLine("if(currentDialog != null){");
            output.WriteLine("if(!currentDialog.$S_0){");
            output.WriteLine("currentDialog.$z();");
            output.WriteLine("}}}catch(e){}");


            output.WriteLine("}");

            output.WriteLine("ExecuteOrDelayUntilScriptLoaded(maximizeWindow, 'sp.ui.dialog.js');");

            output.Write("initmb();");

            output.WriteLine("function clickTab(){");
            output.WriteLine("try{");
            output.WriteLine("var wp = document.getElementById('MSOZoneCell_WebPart" + this.Qualifier + "');");
            output.WriteLine("fireEvent(wp, 'mouseup');");
            output.WriteLine("setTimeout(\"clickbrowse()\",1000);");
            output.WriteLine("}catch(e){}");
            output.WriteLine("}");

            output.WriteLine("function clickbrowse(){");
            output.WriteLine("try{");
            output.WriteLine("var wp2 = document.getElementById('Ribbon.Read-title').firstChild;");
            output.WriteLine("fireEvent(wp2, 'click');");
            output.WriteLine("}catch(e){}");
            output.WriteLine("}");
                
        //output.Write("SP.SOD.executeOrDelayUntilScriptLoaded(clickTab, \"GridViewContextualTabPageComponent.js\");");


        output.Write("setTimeout(\"clickTab()\", 100);");

            output.Write("</script>");

        }

        private string typeoption(string id, string display)
        {
            string ret = "<option value=\"" + id + "\" ";
            if(sSearchType == id)
                ret += "selected";
            ret += ">" + display + "</option>";
            return ret;
        }


        private void renderGrid(HtmlTextWriter output, SPWeb web)
        {
            
            output.Write("<style>");
            output.Write(".ms-usereditor { width:200px; }");
            output.Write(".grid_hover { border: 10px solid #91CDF2; background-color: #F2FAFF } ");
            //output.Write(".rowselected { background-color: #000000; }");
            output.Write("</style>");

            

            //========================Multiple People==============================
           // if (inEditMode)
            {
                output.Write("<div id=\"peoplegrid" + this.ID + "\" style=\"display:none; border: 1px solid #808080; padding: 3px; background-color: #F9F9F9; width=200px;\">");
                output.Write("<div id=\"peoplecheck" + sFullGridId + "\" style=\"overflow: auto; width: 200px; height: 100px;  background-color: #FFFFFF; border: 1px solid #808080; margin-top:2px; padding:3px;\" class=\"ms-descriptiontext\">");
                output.Write("</div>");

                output.Write("<table border=\"0\" width=\"100%\"><tr><td>");
                    output.Write("<a onclick=\"javascript:viewChecks" + sFullGridId + "('" + sFullGridId + "');\"><img id=\"peoplecheckimg" + sFullGridId + "\" src=\"_layouts/images/TPMAX1.GIF\" border=\"0\"></a><br>");
                    output.Write("</td><td align=\"right\">");
                    output.Write("<font class=\"ms-descriptiontext\"><a onclick=\"javascript:mygrid" + sFullGridId + ".editStop();\">Close</a></font>");
                output.Write("</td></tr></table>");
                output.Write("<div id=\"divPe" + sFullGridId + "\" style=\"display:none;\">");
                peMulti.RenderControl(output);
                output.Write("</div>");
                output.Write("</div>");

                //===============================Single================================
                output.Write("<div id=\"peoplesinglegrid" + this.ID + "\" style=\"display:none; border: 1px solid #808080; padding: 3px; background-color: #F9F9F9; width=200px;\">");
                output.Write("<div id=\"peoplechecksingle" + sFullGridId + "\" style=\"width: 200px; height: 100px;  background-color: #FFFFFF; border: 1px solid #808080; margin-top:2px; padding:0px;\" class=\"ms-descriptiontext\">");
                output.Write("<select size=\"6\" onclick=\"changeUser" + sFullGridId + "(this);\" id=\"peoplecheckselect" + sFullGridId + "\"  style=\"width:100%;height:100%\"><option>test</option></select>");
                output.Write("</div>");
                
                output.Write("<table border=\"0\" width=\"200\"><tr><td>");
                output.Write("<a onclick=\"javascript:viewDropDown" + sFullGridId + "('" + sFullGridId + "');\"><img id=\"peoplechecksingleimg" + sFullGridId + "\" src=\"_layouts/images/TPMAX1.GIF\" border=\"0\"></a><br>");
                output.Write("</td><td align=\"right\">");
                output.Write("<font class=\"ms-descriptiontext\"><a onclick=\"javascript:mygrid" + sFullGridId + ".editStop();\">Close</a></font>");
                output.Write("</td></tr></table>");

                output.Write("<div id=\"divPes" + sFullGridId + "\" style=\"display:none;\">");
                peSingle.RenderControl(output);
                output.Write("</div>");
                output.Write("</div>");
                //==============================MultiChoice==============================
                output.Write("<div id=\"multichoicegrid" + this.ID + "\" style=\"display:none; border: 1px solid #808080; padding: 3px; background-color: #F9F9F9; width:200px;height:180px\">");
                output.Write("<div id=\"multichoiceinner" + sFullGridId + "\" style=\"width: 195px; height: 160px;  background-color: #FFFFFF; border: 1px solid #808080; margin-top:2px; padding:0px;overflow:auto\" class=\"ms-descriptiontext\">");
                output.Write("test");
                output.Write("</div>");
                output.Write("<table border=\"0\" width=\"200\"><tr><td align=\"right\">");
                output.Write("<font class=\"ms-descriptiontext\"><a onclick=\"javascript:mygrid" + sFullGridId + ".editStop();\">Close</a></font>");
                output.Write("</td></tr></table>");
                output.Write("</div>");


            }

            output.Write("<div id=\"gridmenu" + sFullGridId + "\" style=\"position:absolute;left:0px;display:none;\" class=\"ms-MenuUIPopupBody ms-MenuUIPopupScreen\">");
            output.Write("<div class=\"ms-MenuUIPopupInner\" style=\"overflow: visible;\" isInner=\"true\">");
            output.Write("<div class=\"ms-MenuUI\" style=\"width: 177px;\">");
            output.Write("<ul class=\"ms-MenuUIUL\" style=\"width: 173px;\">");
            //output.Write(Properties.Resources.txtGridMenus.Replace("#gridfunc#", "mygrid" + sFullGridId + ".menuaction(this);"));
            output.Write("</ul>");
            output.Write("</div>");
            output.Write("</div>");
            output.Write("</div>");

            //===================================

            /*output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/xgrid/dhtmlxgrid.css\"/>");
            output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/xgrid/dhtmlxgrid_skins.css\"/>");
            output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/calendar/dhtmlxcalendar.css\"/>");
            output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/skins/dhtmlxmenu_dhx_blue.css\">");
            //output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/xmenu/context.css\">");
            output.Write("<link rel=\"STYLESHEET\" type=\"text/css\" href=\"/_layouts/epmlive/dhtml/xcombo/dhtmlxcombo.css\">");
            */
            output.Write("<script>_css_prefix=\"/_layouts/epmlive/DHTML/xgrid/\"; _js_prefix=\"/_layouts/epmlive/DHTML/xgrid/\"; </script>");

            //ScriptLink.Register(Page, "/_layouts/epmlive/DHTML/xgrid/dhtmlxcommon.js", false);
            //ScriptLink.Register(Page, "/_layouts/epmlive/DHTML/xgrid/dhtmlxgrid.js", false);

            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/dhtmlxcommon.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/dhtmlxgrid.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/dhtmlxgridcell.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xtreegrid/dhtmlxtreegrid.js\"></script>");
            
            output.Write("<script src=\"_layouts/epmlive/DHTML/xtreegrid/ext/dhtmlxtreegrid_filter.js\"></script>");

            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/ext/dhtmlxgrid_post.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/ext/dhtmlxgrid_nxml.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/ext/dhtmlxgrid_filter.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/ext/dhtmlxgrid_math.js\"></script>");
            output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/ext/dhtmlxgrid_srnd.js\"></script>");
            
            //output.Write("<script src=\"/_layouts/epmlive/DHTML/dhtmlxajax.js\"></script>");
            
            //if (inEditMode)
            {

                output.Write("<script src=\"/_layouts/epmlive/DHTML/xgrid/excells/dhtmlxgrid_excell_calendar.js\"></script>");
                output.Write("<script src=\"/_layouts/epmlive/DHTML/xgrid/excells/dhtmlxgrid_excell_combo.js\"></script>");
                output.Write("<script src=\"_layouts/epmlive/DHTML/xgrid/excells/dhtmlxgrid_excell_dhxcalendar.js\"></script>");
                output.Write("<script src=\"_layouts/epmlive/DHTML/xcombo/dhtmlxcombo.js\"></script>");
                output.Write("<script src=\"_layouts/epmlive/DHTML/calendar/dhtmlxcalendar.js\"></script>");
                output.Write("<script src=\"_layouts/epmlive/DHTML/xdataproc/dhtmlxdataprocessor.js\"></script>");
            }
            
            output.Write("<script type=\"text/javascript\" src=\"_layouts/epmlive/modal/modal.js\"></script> ");

            //output.Write("<script language=\"JavaScript\" src=\"_layouts/epmlive/dhtml/xmenu/dhtmlxprotobar.js\"></script>");
            output.Write("<script language=\"JavaScript\" src=\"_layouts/epmlive/dhtml/xmenu/dhtmlxmenu.js\"></script>");
            output.Write("<script language=\"JavaScript\" src=\"_layouts/epmlive/dhtml/xmenu/ext/dhtmlxmenu_ext.js\"></script>");
            //output.Write("<script language=\"JavaScript\" src=\"_layouts/epmlive/dhtml/xmenu/dhtmlxmenubar_cp.js\"></script>");

            output.Write("<style>.menuTable{background-color:#ffffff;}.secondMenuTable{background-color:#ffffff;}.contextMenuover, .contextMenudown{background-color:#9ac2e5;}.contextMenuover td{color:#000000;} </style>");
            output.Write("<script src=\"_layouts/epmlive/epmlivegrid.js\"></script>");

            //output.Write("<a href=\"javascript:mygrid.adjustColumnSize(2);\">CLick</a>");

            //if(bShowSearch)
            {
                string fieldlist = "";
                string jsonfields = "";
                SortedList slFields = new SortedList();

                if(sSearchField == "")
                    sSearchField = "Title";

                if(sSearchType == "")
                    sSearchType = "1";

                foreach(SPField field in list.Fields)
                {
                    if(field.Reorderable && !field.Hidden)
                    {
                        slFields.Add(field.Title, field.InternalName);

                        
                        if(field.Type == SPFieldType.Choice)
                        {
                            jsonfields += field.InternalName + ": ["; 
                            SPFieldChoice c = (SPFieldChoice)field;
                            foreach(string choice in c.Choices)
                            {
                                jsonfields += "\"" + choice + "\",";
                            }
                            jsonfields = jsonfields.TrimEnd(',') + "],";
                        }
                        if(field.Type == SPFieldType.Boolean)
                        {
                            jsonfields += field.InternalName + ": [ \"Yes\", \"No\" ],";
                        }
                    }
                }

                foreach(DictionaryEntry de in slFields)
                {
                    if(sSearchField == de.Value.ToString())
                        fieldlist += "<option value=\"" + de.Value + "\" selected>" + de.Key.ToString() + "</option>";
                    else 
                        fieldlist += "<option value=\"" + de.Value + "\">" + de.Key.ToString() + "</option>";
                }

                output.WriteLine("<script language=\"javascript\">");
                output.WriteLine("var searchfields" + sFullGridId + " = {" + jsonfields.TrimEnd(',') + "};");
                output.WriteLine("function switchsearch" + sFullGridId + "()");
                output.WriteLine("{");
                output.WriteLine("var searcher = document.getElementById('search" + sFullGridId + "');");
                output.WriteLine("var searchtext = document.getElementById('searchtext" + sFullGridId + "');");
                output.WriteLine("var searchchoice = document.getElementById('searchchoice" + sFullGridId + "');");
                output.WriteLine("var searchtypechoice = document.getElementById('searchtype" + sFullGridId + "');");
                output.WriteLine("var searchfield = searcher.options[searcher.selectedIndex].value;");
                output.WriteLine("var sList = searchfields" + sFullGridId + "[searchfield];");
                output.WriteLine("if(sList){");
                output.WriteLine("searchtext.style.display='none';");
                output.WriteLine("searchchoice.style.display='';");
                output.WriteLine("searchchoice.options.length = 0;");
                output.WriteLine("searchtypechoice.options[2].selected = true;");
                output.WriteLine("searchtypechoice.disabled = true;");
                output.WriteLine("for(var i=0; i < sList.length; i++) {     var d = sList[i];     searchchoice.options.add(new Option(d, d)); if(d=='" + sSearchValue + "'){searchchoice.options[searchchoice.options.length-1].selected = true;} } ");

                output.WriteLine("}else{");
                output.WriteLine("searchtext.style.display='';");
                output.WriteLine("searchchoice.style.display='none';");
                output.WriteLine("searchtypechoice.disabled = false;");
                output.WriteLine("}");
                output.WriteLine("}");

                output.WriteLine("function unSearch" + sFullGridId + "(){");
                output.WriteLine("var gri = document.getElementById('grid" + this.ID + "');");
                output.WriteLine("gri.style.display = 'none';");
                output.WriteLine("var loader = document.getElementById('loadinggrid" + this.ID + "');");
                output.WriteLine("loader.style.display = '';");
                output.WriteLine("var unsearch = document.getElementById('unsearch" + sFullGridId + "');");
                output.WriteLine("unsearch.style.display=\"none\";");
                output.WriteLine("var searchtext = document.getElementById('searchtext" + sFullGridId + "');");
                output.WriteLine("searchtext.value = '';");

                output.WriteLine("loadX" + sFullGridId + "();");
                
                output.WriteLine("}");

                output.WriteLine("function doSearch" + sFullGridId + "(){");
                output.WriteLine("var gri = document.getElementById('grid" + this.ID + "');");
                output.WriteLine("gri.style.display = 'none';");
                output.WriteLine("var loader = document.getElementById('loadinggrid" + this.ID + "');");
                output.WriteLine("loader.style.display = '';");
                //output.WriteLine("var searchbut = document.getElementById('searchbutton" + sFullGridId + "');");
                //output.WriteLine("searchbut.disabled = true;");
                output.WriteLine("var searcher = document.getElementById('search" + sFullGridId + "');");

                output.WriteLine("var unsearch = document.getElementById('unsearch" + sFullGridId + "');");
                output.WriteLine("unsearch.style.display=\"table-cell\";");
                output.WriteLine("var searchchoice = document.getElementById('searchchoice" + sFullGridId + "');");
                output.WriteLine("var searchtext = document.getElementById('searchtext" + sFullGridId + "');");
                output.WriteLine("var searchtypechoice = document.getElementById('searchtype" + sFullGridId + "');");
                output.WriteLine("var searchfield = searcher.options[searcher.selectedIndex].value;");
                output.WriteLine("var searchtype = searchtypechoice.options[searchtypechoice.selectedIndex].value;");
                output.WriteLine("var sList = searchfields" + sFullGridId + "[searchfield];");
                output.WriteLine("var searchvalue = \"\";");
                output.WriteLine("if(sList){");
                output.WriteLine("searchvalue = searchchoice.options[searchchoice.selectedIndex].value;");
                output.WriteLine("}else{");
                output.WriteLine("searchvalue = searchtext.value;");
                output.WriteLine("}");

                if(bLockSearch)
                {
                    System.Collections.Specialized.NameValueCollection nv = Page.Request.QueryString;
                    StringBuilder sbUrl = new StringBuilder();

                    string fListId = list.ID.ToString("N");

                    foreach(string key in nv.AllKeys)
                    {
                        if(key != fListId + "_searchvalue" && key != fListId + "_searchfield" && key != fListId + "_searchtype")
                        {
                            sbUrl.Append("&");
                            sbUrl.Append(key);
                            sbUrl.Append("=");
                            sbUrl.Append(HttpUtility.UrlEncode(nv[key]));
                        }
                    }

                    string urlParams = sbUrl.ToString().TrimStart('&');
                    if(!String.IsNullOrEmpty(urlParams))
                        urlParams = "?" + urlParams;

                    string curUrl = Page.Request.Url.ToString();

                    try
                    {
                        curUrl = curUrl.Remove(curUrl.IndexOf("?"));
                    }
                    catch { }

                    output.WriteLine("var url = '" + curUrl + urlParams + "';");
                    output.WriteLine("if(url.indexOf('?') > 0){url = url + '&';}else{url = url + '?';}");
                    output.WriteLine("url = url + '" + fListId + "_searchfield=' + searchfield + '&" + fListId + "_searchvalue=' + searchvalue + '&" + fListId + "_searchtype=' + searchtype;");
                    output.WriteLine("location.href= url;");

                }
                else
                {
                    
                    output.WriteLine("loadX" + sFullGridId + "(searcher.options[searcher.selectedIndex].value, searchvalue, searchtype);");
                }

                output.WriteLine("}");

                output.WriteLine("function enablesearcher" + sFullGridId + "(){");
                //output.WriteLine("var searchbut = document.getElementById('searchbutton" + sFullGridId + "');");
                //output.WriteLine("searchbut.disabled = false;");
                output.WriteLine("}");

                output.WriteLine("</script>");

                output.Write("<div id=\"search" + this.ID + "\" style=\"width:100%; height:25px; padding:10px");
                if(!bShowSearch)
                    output.Write(";display:none");
                output.WriteLine("\" class=\"ms-listviewtable\"><table><tr><td>");
                output.Write("Search: ");
                output.Write("<select id=\"search" + sFullGridId + "\" onChange=\"switchsearch" + sFullGridId + "();\" style=\"border: 1px solid #ABABAB; height:20px\">");
                output.WriteLine(fieldlist);
                output.Write("</select>&nbsp;&nbsp;");

                output.WriteLine("<select id=\"searchtype" + sFullGridId + "\" style=\"border: 1px solid #ABABAB; height:20px\">");
                output.WriteLine(typeoption("1", "Contains"));
                output.WriteLine(typeoption("8", "Begins With"));
                output.WriteLine(typeoption("2", "Equals"));
                output.WriteLine(typeoption("3", "Does Not Equal"));
                output.WriteLine(typeoption("4", "Greater Than"));
                output.WriteLine(typeoption("5", "Greater Than or Equal"));
                output.WriteLine(typeoption("6", "Less Than"));
                output.WriteLine(typeoption("7", "Less Than or Equal"));

                output.WriteLine("</select>&nbsp;&nbsp;");
                output.WriteLine("</td><td>");
                output.WriteLine(@"<div style=""border: 1px solid #ABABAB;width: 150px;height: 20px;"">
                    <div id=""unsearch" + sFullGridId + @""" class="""" style=""padding-left:4px;display: none;min-width: 12px;align:top"">
                        <img alt=""Clear Search"" src=""/_layouts/epmlive/images/unsearch.png"" style=""padding-bottom:2px"" onclick=""unSearch" + sFullGridId + @"()""/>
                    </div>
                    <div style=""display: table-cell"">
                        <input type=""text"" id=""searchtext" + sFullGridId + @""" class="""" style=""border: 0px; width:100%; height:15px; font-family: 'Segoe UI','Segoe',Tahoma,Helvetica,Arial,sans-serif;font-size:13px""/>
                        <select id=""searchchoice" + sFullGridId + @""" style=""border: 0px; width:100%"">
                        </select>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    </div>
                    <div class="""" style=""padding-right:2px; padding-left:5px;padding-top:2px;display: table-cell;min-width: 10px;cursor:pointer"">
                        <img onclick=""doSearch" + sFullGridId + @"()"" src=""/_layouts/epmlive/images/find_icon.png""/>
                    </div>
                </div>");

//                output.Write("<input type=\"button\" id=\"searchbutton" + sFullGridId + "\" onclick=\"doSearch" + sFullGridId + "()\" value=\"Search\">");
                output.WriteLine("</td></tr></table>");
                output.WriteLine("</div>");

                output.WriteLine("<script language=\"javascript\">switchsearch" + sFullGridId + "();</script>");

            }

            output.Write("<div id=\"grid" + this.ID + "\" style=\"width:100%;display:none;\" class=\"ms-listviewtable\"></div>\r\n\r\n");
            output.Write("<div id=\"errordiv" + sFullGridId + "\" style=\"font-color:red;font-size:11px;\"></div>");

            
            output.Write("<div  width=\"100%\" id=\"loadinggrid" + this.ID + "\" align=\"center\"");
            if(bShowSearch && !bHasSearchResults){
                output.Write(" style=\"display:none\"");
            }
            output.Write(">");
            output.Write("<img src=\"_layouts/images/gears_anv4.gif\" style=\"vertical-align: middle;\"/> Loading Items...");
            output.Write("</div>");


            output.Write("<script language=\"javascript\">");
     

            output.Write("function viewChecks" + sFullGridId + "(btn)");
            output.Write("{");
            output.Write("var pp = document.getElementById('divPe' + btn);");
            output.Write("var ppi = document.getElementById('peoplecheckimg' + btn);");
            output.Write("if(pp.style.display == \"none\"){");
            output.Write("pp.style.display = \"\";\r\n");
            output.Write("ppi.src = \"_layouts/images/TPMIN1.GIF\";\r\n");
            output.Write("}");
            output.Write("else{");
            output.Write("pp.style.display = \"none\";");
            output.Write("ppi.src = \"_layouts/images/TPMAX1.GIF\";\r\n");
            output.Write("}");
            output.Write("}");

            output.Write("function viewDropDown" + sFullGridId + "(btn)");
            output.Write("{");
            output.Write("var pp = document.getElementById('divPes' + btn);");
            output.Write("var ppi = document.getElementById('peoplechecksingleimg' + btn);");
            output.Write("if(pp.style.display == \"none\"){");
            output.Write("pp.style.display = \"\";\r\n");
            output.Write("ppi.src = \"_layouts/images/TPMIN1.GIF\";\r\n");
            output.Write("}");
            output.Write("else{");
            output.Write("pp.style.display = \"none\";");
            output.Write("ppi.src = \"_layouts/images/TPMAX1.GIF\";\r\n");
            output.Write("}");
            output.Write("}");

            output.Write("function gridfilter" + sFullGridId + "(value){");
            output.Write("var vals = value.split('|');");
            output.Write("mygrid" + sFullGridId + ".filterBy(vals[0],vals[1]);");
            output.Write("}");

            output.Write("function setSize" + sFullGridId + "(){mygrid" + sFullGridId + "._askRealRows();}");

            output.Write("function printgrid" + sFullGridId + "() {var temp = mygrid" + sFullGridId + ".hdr.rows[2];var parent = temp.parentNode;parent.removeChild(temp,true);mygrid" + sFullGridId + ".printView();parent.appendChild(temp);}");

            output.Write("function switchFilterLoad" + sFullGridId + "(){mygrid" + sFullGridId + ".switchFilter();}");
            output.Write("</script>");
            
            output.WriteLine("<script>");
            
            output.WriteLine("var mygrid" + sFullGridId + "Hidden = false;");
            output.WriteLine("var listid" + sFullGridId + " = '" + list.ID.ToString() + "';");

            output.WriteLine("mygrid" + sFullGridId + " = new dhtmlXGridObject('grid" + this.ID + "');");

            //CUSTOM PROPERTIES
            output.WriteLine("mygrid" + sFullGridId + ".setImagePath(\"_layouts/epmlive/dhtml/xgrid/imgs/\");");


            output.WriteLine("mygrid" + sFullGridId + @".getTitles = function(){
                            var lookups = """";
            mygrid" + sFullGridId + @".forEachRow(function (id) {
			    var c = this.cells(id, 0);
			    if (c.isCheckbox() && !c.cell.firstChild.disabled && c.cell.firstChild.checked)
                {
                    var title = this.getUserData(id,""title"");
        	        var itemid = this.getUserData(id,""itemid"");
                    if(itemid != """")
                    {
                        lookups += "","" + title;
                    }
                }
		    });

            if(lookups == """")
            {
                var rowId = mygrid" + sFullGridId + @".getSelectedRowId();
                lookups = "","" + mygrid" + sFullGridId + @".getUserData(rowId,""title"");
            }

            if(lookups != """" && lookups[0] == ',')
                lookups = lookups.substring(1);
            return lookups;
                }
                ");


            output.WriteLine("mygrid" + sFullGridId + ".getCheckedRowIds = function(){");
            output.WriteLine(@"
                var ids = """";
                this.forEachRow(function (id) {
			        var c = this.cells(id, 0);
			        if (c.isCheckbox() && !c.cell.firstChild.disabled && c.cell.firstChild.checked)
                    {
                        ids += "","" + id;
                    }
		        });

                if(ids != """" && ids[0] == ',')
                    ids = ids.substring(1);

                if(ids == """")
                    ids = this.getSelectedRowId();

                return ids;
            };");

            output.WriteLine("mygrid" + sFullGridId + ".getCheckedIds = function(){");
            output.WriteLine(@"
                var ids = """";
                this.forEachRow(function (id) {
			        var c = this.cells(id, 0);
			        if (c.isCheckbox() && !c.cell.firstChild.disabled && c.cell.firstChild.checked)
                    {
                        var listid = this.getUserData(id,""listid"");
        	            var itemid = this.getUserData(id,""itemid"");
        	            var webid = this.getUserData(id,""webid"");

                        if (this._useparent) {
                            var parentid = this.getUserData(id, ""parentitemid"");
                            if (parentid && parentid != """") {
                                var newids = parentid.toString().split(""."");
                                webid = newids[0];
                                listid = newids[1];
                                itemid = newids[2];
                            }
                        }

                        if(itemid != """")
                        {
                            ids += "","" + webid + ""."" + listid + ""."" + itemid;
                        }
                    }
		        });

                if(ids == """")
                {
                    var rowId = this.getSelectedRowId();
                    var listid = this.getUserData(rowId,""listid"");
        	        var itemid = this.getUserData(rowId,""itemid"");
        	        var webid = this.getUserData(rowId,""webid"");
                    if (this._useparent) {
                        var parentid = this.getUserData(rowId, ""parentitemid"");
                        if (parentid && parentid != """") {
                            var newids = parentid.toString().split(""."");
                            webid = newids[0];
                            listid = newids[1];
                            itemid = newids[2];
                        }
                    }

                    if(itemid != """")
                    {
                        ids = webid + ""."" + listid + ""."" + itemid;
                    }
                }

                if(ids != """" && ids[0] == ',')
                    ids = ids.substring(1);

                return ids;
            };");

            output.WriteLine("mygrid" + sFullGridId + ".getCheckedItems = function(){");
            output.WriteLine(@"
                var ids = """";
                this.forEachRow(function (id) {
			        var c = this.cells(id, 0);
			        if (c.isCheckbox() && !c.cell.firstChild.disabled && c.cell.firstChild.checked)
                    {
                        var listid = this.getUserData(id,""listid"");
        	            var itemid = this.getUserData(id,""itemid"");
        	            var webid = this.getUserData(id,""webid"");
                        if(itemid != """")
                        {
                            ids += "","" + itemid;
                        }
                    }
		        });

                if(ids == """")
                {
                    var rowId = this.getSelectedRowId();
                    var listid = this.getUserData(rowId,""listid"");
        	        var itemid = this.getUserData(rowId,""itemid"");
        	        var webid = this.getUserData(rowId,""webid"");
                    if(itemid != """")
                    {
                        ids = itemid;
                    }
                }

                if(ids != """" && ids[0] == ',')
                    ids = ids.substring(1);

                return ids;
            };");

            output.WriteLine("mygrid" + sFullGridId + ".toggleSearch = function(){");
            output.WriteLine("var searchDiv = document.getElementById('search" + this.ID + "');");
            output.WriteLine("if(searchDiv.style.display == 'none'){");
            output.WriteLine("searchDiv.style.display = '';");
            output.WriteLine("}else{");
            output.WriteLine("searchDiv.style.display = 'none';");
            output.WriteLine("}");
            output.WriteLine("};");

            output.Write("mygrid" + sFullGridId + ".switchFilter = function(){try{");
            output.Write("var input1 = mygrid" + sFullGridId + ".hdr.rows[2];");
            output.Write("if(mygrid" + sFullGridId + "Hidden == false){");
            output.Write("input1.style.display = \"none\";");
            output.Write("mygrid" + sFullGridId + "Hidden = true;");
            //output.Write("if(hlink != null){document.getElementById(hlink).innerHTML=\"<img src='/_layouts/images/filter.gif' border='0' align='absmiddle' >&nbsp;Show Filters\";}");
            output.Write("}else{");
            output.Write("input1.style.display = \"\";");
            output.Write("mygrid" + sFullGridId + "Hidden = false;");
            //output.Write("if(hlink != null){document.getElementById(hlink).innerHTML=\"<img src='/_layouts/images/filter.gif' border='0' align='absmiddle' >&nbsp;Hide Filters\";}");
            output.Write("}");
            output.Write("mygrid" + sFullGridId + ".setSizes();");
            output.Write("}catch(e){}};");
            output.Write("mygrid" + sFullGridId + "._enableCMenus = true;");

            addGridProperties(output, web);
            ////////////////////////////////////////////////////////////////////////

            if (inEditMode)
            {
                output.WriteLine("mygrid" + sFullGridId + ".setSkin(\"editgrid\");");
            }
            else
            {
                output.WriteLine("mygrid" + sFullGridId + ".setSkin(\"modern\");");
            }

            //output.Write("mygrid" + sFullGridId + ".enableRowsHover(true,\"grid_hover\");");

            output.WriteLine("mygrid" + sFullGridId + ".enableAlterCss(\"ms-itmhover\", \"ms-itmhover\");");

            if (this.Height == "")
            {
                output.WriteLine("mygrid" + sFullGridId + ".enableAutoHeight(true);");
            }
            else
            {
                MatchCollection mc = Regex.Matches(this.Height, "\\d*");
                string h = "100";
                if (mc.Count > 0)
                {
                    h = mc[0].Value;
                    try
                    {
                        h = (int.Parse(h) - 30).ToString();
                    }catch{}
                }
                output.WriteLine("mygrid" + sFullGridId + ".enableAutoHeight(true," + h + ",true);");
            }
            output.WriteLine("mygrid" + sFullGridId + ".enableSmartRendering(true);");
            output.Write("mygrid" + sFullGridId + ".enableSmartXMLParsing(true);");

            output.Write("mygrid" + sFullGridId + ".setImageSize(1,1);");
            output.Write("mygrid" + sFullGridId + "._editmode = " + inEditMode.ToString().ToLower() + ";");
            output.Write("mygrid" + sFullGridId + ".enableEditEvents(true,false,false);");
            //output.Write("mygrid" + sFullGridId + ".enableMultiline(true);");
            output.Write("document.onclick=function(){mygrid" + sFullGridId + "._curHover=false;mygrid" + sFullGridId + ".clearmenus();};");
            //output.Write("mygrid" + sFullGridId + ".enableDistributedParsing(true, 100, 100);");

            System.Globalization.CultureInfo cInfo = new System.Globalization.CultureInfo(web.Locale.LCID);
            string []dtFormat = cInfo.DateTimeFormat.ShortDatePattern.ToLower().Split(cInfo.DateTimeFormat.DateSeparator.ToCharArray());
            string dtFormatComplete = "";
            foreach (string s in dtFormat)
            {
                if (s.Contains("m"))
                    dtFormatComplete += cInfo.DateTimeFormat.DateSeparator + "%m";
                else if (s.Contains("d"))
                    dtFormatComplete += cInfo.DateTimeFormat.DateSeparator + "%d";
                else if (s.Contains("y"))
                    dtFormatComplete += cInfo.DateTimeFormat.DateSeparator + "%Y";
            }
            if(dtFormatComplete.Length > 1)
                dtFormatComplete = dtFormatComplete.Substring(1);
            else
                dtFormatComplete = "%m/%d/%Y";
            output.Write("mygrid" + sFullGridId + ".setDateFormat(\"" + dtFormatComplete + "\");");
            output.Write("mygrid" + sFullGridId + ".attachEvent(\"onXLE\",clearLoader);");
            output.Write("mygrid" + sFullGridId + ".attachEvent(\"onXLE\",switchFilterLoad" + sFullGridId + ");");
            output.Write("mygrid" + sFullGridId + ".attachEvent(\"onXLE\",enablesearcher" + sFullGridId + ");");

            output.Write("mygrid" + sFullGridId + ".attachEvent(\"onRowSelect\",selectRow" + sFullGridId + ");");

            try
            {
                if (view.TabularView && SPContext.Current.ViewContext.View != null)
                {

                    output.WriteLine("mygrid" + sFullGridId + ".hovered = \"\";");

                    int index = 0;

                    if (inEditMode)
                        index = 1;

                    output.WriteLine(@"
                    mygrid" + sFullGridId + @".attachEvent(""onMouseOver"", function(id,ind){
                        if(mygrid" + sFullGridId + @".hovered != id)
                        {
                            var c = this.cells(id, " + index + @");
			                if (c.isCheckbox())
                            {
                                c.cell.firstChild.style.display="""";

                                try
                                {
                                    var c = this.cells(mygrid" + sFullGridId + @".hovered, " + index + @");
                                    if(c && !c.cell.firstChild.checked)
                                        c.cell.firstChild.style.display=""none"";
                                }catch(e){}
                                mygrid" + sFullGridId + @".hovered = id;
                            }
                        }
                    });");
                }
            }
            catch { }

            if (inEditMode)
            {
                output.Write("mygrid" + sFullGridId + ".attachEvent(\"onEditCell\",onEditCell" + sFullGridId + ");");

                if (allowInsertRow)
                    output.Write("mygrid" + sFullGridId + ".attachEvent(\"onXLE\",setDefaults" + sFullGridId + ");");
                output.Write("mygrid" + sFullGridId + ".attachEvent(\"onKeyPress\",onKeyPress" + sFullGridId + ");");
            }
            //output.Write("mygrid" + sFullGridId + ".attachEvent(\"onBeforeContextMenu\",onShowMenu);");

            //output.Write("mygrid" + sFullGridId + ".attachHeader(\" ,#text_filter,#select_filter, ,#cspan,#cspan,#cspan,#cspan\");");
            //output.Write("mygrid.enableEditEvents(false, true, true);");



            //if (showMenu)
            //{
            //    output.Write("var aMenu=new dhtmlXMenuObject();");
            //    output.Write("aMenu.setIconsPath(\"/_layouts/epmlive/images/\");");
            //    output.Write("aMenu.renderAsContextMenu();");
            //    output.Write("aMenu.loadXML(\"/_layouts/epmlive/gridmenu.xml\");");
            //    //if(inEditMode)
            //        output.Write("aMenu.attachEvent('onClick',onButtonClick" + sFullGridId + ");");
            //    //else
            //    //    output.Write("aMenu.setContextMenuHandler(onButtonClick);");
            //    output.Write("mygrid" + sFullGridId + ".enableContextMenu(aMenu);");
            //}
            output.Write("mygrid" + sFullGridId + "._singleItemUrl = \"" + web.Url + "/_layouts/epmlive/getsingleitem.aspx?data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "\";");
            output.Write("mygrid" + sFullGridId + ".init();");
            output.Write("singleItemUrl" + sFullGridId + " = \"" + web.Url + "/_layouts/epmlive/getsingleitem.aspx?data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "\";");
            output.Write("singleEditItemUrl" + sFullGridId + " = \"" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/getedititem.aspx?data=" + sFullParamList + "\";");
            output.Write("inEditMode" + sFullGridId + " = " + inEditMode.ToString().ToLower() + ";");
            
            //if (inEditMode)
            //{
                output.Write("myDataProcessor" + sFullGridId + " = new dataProcessor(\"" + web.Url + "/_layouts/epmlive/savegrid.aspx?columns=" + getViewFields() + "&data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "\");");
                output.Write("myDataProcessor" + sFullGridId + ".setUpdateMode(\"off\");");
                output.Write("myDataProcessor" + sFullGridId + ".defineAction(\"error100\", myErrorHandler" + sFullGridId + ");");
                output.Write("myDataProcessor" + sFullGridId + ".defineAction(\"error500\", error500" + sFullGridId + ");");
                output.Write("myDataProcessor" + sFullGridId + ".defineAction(\"alldatareturned\", alldatareturned" + sFullGridId + ");");
                output.Write("myDataProcessor" + sFullGridId + ".setTransactionMode(\"POST\", true);");
                //if(inEditMode)    
                    output.Write("myDataProcessor" + sFullGridId + ".init(mygrid" + sFullGridId + ");");

                if (!titlefound)
                {
                    output.Write("mygrid" + sFullGridId + ".enableTreeCellEdit(false);");
                }

                output.Write("function viewError" + sFullGridId + "(id){");
                output.Write("document.getElementById('dlgErrorText" + sFullGridId + "').innerText=mygrid" + sFullGridId + ".getUserData(id,\"lastError\");");
                output.Write("sm('dlgError" + sFullGridId + "',250,130);");
                output.Write("}");

                output.Write("function myErrorHandler" + sFullGridId + "(obj){");
                output.Write("      var id = obj.getAttribute(\"sid\");");
                if (inEditMode)
                {
                    output.Write("      mygrid" + sFullGridId + ".cells(id,0).setValue(\"<a href=\\\"javascript:viewError" + sFullGridId + "('\" + id + \"');\\\"><img src='/_layouts/images/EXCLAIM.GIF' border='0'></a>\");");
                }
                else
                {
                    output.Write("var EditCol = getEditCol" + sFullGridId + "();");
                    output.Write("if(EditCol != null){");

                    output.Write("      mygrid" + sFullGridId + ".cells(id,EditCol).setValue(\"<a href=\\\"javascript:viewError" + sFullGridId + "('\" + id + \"');\\\"><img src='/_layouts/images/EXCLAIM.GIF' border='0'></a> <a onclick=\\\"javascript:saveEdit" + sFullGridId + "('\" + id + \"');\\\" style=\\\"cursor:hand\\\"><img src=\\\"/_layouts/images/saveitem.gif\\\" border=\\\"0\\\" alt=\\\"Save\\\"></a> <a onclick=\\\"javascript:cancelEditRow" + sFullGridId + "('\" + id + \"');\\\" style=\\\"cursor:hand\\\"><img src=\\\"/_layouts/images/close.gif\\\" border=\\\"0\\\" alt=\\\"Cancel\\\"></a>\");");

                    output.Write("}");
                }
                output.Write("      mygrid" + sFullGridId + ".setUserData(id,\"lastError\",obj.firstChild.data);");
                output.Write("      return true;");
                output.Write("}");

                output.Write("function alldatareturned" + sFullGridId + "(obj){");
                output.Write("      setRowValues" + sFullGridId + "(obj.firstChild);");
                output.Write("}");

                output.Write("function error500" + sFullGridId + "(obj){");
                output.Write("      alert(obj.firstChild.data);");
                output.Write("      myDataProcessor" + sFullGridId + ".stopOnError = true; ");
                output.Write("      hm('dlgSaving" + sFullGridId + "');");
                output.Write("      return false;");
                output.Write("}");

                output.Write("myDataProcessor" + sFullGridId + ".setOnAfterUpdate(function(id,action,newid){");
                output.Write("if(action != \"error100\" && action != \"delete\"){mygrid" + sFullGridId + ".cells(id,0).setValue(\"\");mygrid" + sFullGridId + ".cells(id,0).setBgColor('#F0F0F0');}");
                output.Write("if(myDataProcessor" + sFullGridId + ".getSyncState())");
                output.Write("{");
                output.Write("    hm('dlgSaving" + sFullGridId + "');");
                output.Write("    myDataProcessor" + sFullGridId + ".updatedRows = new Array(0);");
                output.Write("}");
                output.Write("});");

                string cExcells = Properties.Resources.txtEditGridJS;
                cExcells = cExcells.Replace("#gridid#", sFullGridId);
                cExcells = cExcells.Replace("#peid#", peMulti.ClientID);
                cExcells = cExcells.Replace("#peuid#", peMulti.UniqueID);
                cExcells = cExcells.Replace("#pesid#", peSingle.ClientID);
                cExcells = cExcells.Replace("#pesuid#", peSingle.UniqueID);

                output.Write(cExcells);

                

                output.Write("window.onbeforeunload=leavePage" + sFullGridId + ";");

                string minValues = "\"\"";
                string maxValues = "\"\"";

                foreach (string f in view.ViewFields)
                {
                    XmlDocument doc = new XmlDocument();
                    SPField field = list.Fields.GetFieldByInternalName(f);
                    doc.LoadXml(field.SchemaXml);
                    //===============Min Values==================
                    string min = "";
                    try
                    {
                        min = doc.ChildNodes[0].Attributes["Min"].Value;
                        if (doc.ChildNodes[0].OuterXml.Contains("Percentage=\"TRUE\""))
                            min = (double.Parse(min) * 100).ToString();
                    }
                    catch { }
                    minValues += ",\"" + min + "\"";

                    //===============Max Values==================
                    string max = "";
                    try
                    {
                        max = doc.ChildNodes[0].Attributes["Max"].Value;
                        if (doc.ChildNodes[0].OuterXml.Contains("Percentage=\"TRUE\""))
                            max = (double.Parse(max) * 100).ToString();
                    }
                    catch { }
                    maxValues += ",\"" + max + "\"";
                }
                output.Write("minValues" + sFullGridId + " = [" + minValues + "];");
                output.Write("maxValues" + sFullGridId + " = [" + maxValues + "];");
            //}
            //else
            //{
            //    string cExcells = Properties.Resources.txtNonEditJs;
            //    cExcells = cExcells.Replace("#gridid#", sFullGridId);
            //    cExcells = cExcells.Replace("#peid#", peMulti.ClientID);
            //    cExcells = cExcells.Replace("#peuid#", peMulti.UniqueID);
            //    cExcells = cExcells.Replace("#pesid#", peSingle.ClientID);
            //    cExcells = cExcells.Replace("#pesuid#", peSingle.UniqueID);

            //    output.Write(cExcells);
            //}


            
            output.Write(Properties.Resources.txtInlineEdit.Replace("#gridid#", sFullGridId));

            output.WriteLine("function loadX" + sFullGridId + "(searchfield, searchvalue, searchtype){");
            
            output.WriteLine("var ribbontestintervall = null; ");

            //output.WriteLine("_ribbonStartInit(\"Ribbon.ListContextualTab\" , false , null );");

            output.WriteLine("mygridwidth" + sFullGridId + " = document.getElementById('WebPart" + this.Qualifier + "').offsetWidth - 25;");

            output.WriteLine("if(searchfield){");
            output.WriteLine("searchvalue = escape(searchvalue);");
            output.WriteLine("mygrid" + sFullGridId + ".post(\"" + web.Url + "/_layouts/epmlive/getgriditems.aspx\",\"data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "&insertrow=" + allowInsertRow + "&width=\" + mygridwidth" + sFullGridId + " + \"&searchfield=\" + searchfield + \"&searchvalue=\" + searchvalue + \"&searchtype=\" + searchtype + \"&source=" + System.Web.HttpUtility.UrlEncode(System.Web.HttpContext.Current.Request.Url.ToString()) + "\");");
            //output.WriteLine("mygrid" + sFullGridId + ".post(\"" + web.Url + "/_layouts/epmlive/getgriditems.aspx\",\"data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "&insertrow=" + allowInsertRow + "&width=\" + mygridwidth" + sFullGridId + " + \"&source=" + System.Web.HttpUtility.UrlEncode(System.Web.HttpContext.Current.Request.Url.ToString()) + "\");");
            output.WriteLine("}else{");
            output.WriteLine("mygrid" + sFullGridId + ".post(\"" + web.Url + "/_layouts/epmlive/getgriditems.aspx\",\"data=" + sFullParamList + "&edit=" + inEditMode.ToString().ToLower() + "&insertrow=" + allowInsertRow + "&width=\" + mygridwidth" + sFullGridId + " + \"&source=" + System.Web.HttpUtility.UrlEncode(System.Web.HttpContext.Current.Request.Url.ToString()) + "\");");
            output.WriteLine("}");

            output.WriteLine("}");

            if(SPContext.Current.ViewContext.View != null)
            {
                if(newGridMode == "datasheet")
                {
                    output.WriteLine("function clickTab(){");
                    output.WriteLine("try{");
                    output.WriteLine("var wp = document.getElementById('MSOZoneCell_WebPart" + this.Qualifier + "');");
                    output.WriteLine("fireEvent(wp, 'mouseup');");
                    output.WriteLine("setTimeout(\"clickbrowse()\",100);");
                    output.WriteLine("}catch(e){}");
                    output.WriteLine("}");

                    output.WriteLine("function clickbrowse(){");
                    output.WriteLine("try{");
                    output.WriteLine("var wp2 = document.getElementById('Ribbon.List-title').firstChild;");
                    output.WriteLine("fireEvent(wp2, 'click');");
                    output.WriteLine("}catch(e){}");
                    output.WriteLine("}");
                }
                else
                {
                    output.WriteLine("function clickTab(){");
                    output.WriteLine("try{");
                    output.WriteLine("var wp = document.getElementById('MSOZoneCell_WebPart" + this.Qualifier + "');");
                    output.WriteLine("fireEvent(wp, 'mouseup');");
                    output.WriteLine("setTimeout(\"clickbrowse()\",1000);");
                    output.WriteLine("}catch(e){}");
                    output.WriteLine("}");

                    output.WriteLine("function clickbrowse(){");
                    output.WriteLine("try{");
                    output.WriteLine("var wp2 = document.getElementById('Ribbon.Read-title').firstChild;");
                    output.WriteLine("fireEvent(wp2, 'click');");
                    output.WriteLine("}catch(e){}");
                    output.WriteLine("}");
                }
                //output.Write("ExecuteOrDelayUntilScriptLoaded(clickTab, \"sp.ribbon.js\");");


                output.Write("setTimeout(\"clickTab()\", 100);");

            }
            else
            {
                output.Write("function clickRow(){");
                output.Write("}");
            }

            if(SPContext.Current.ViewContext.View != null && !String.IsNullOrEmpty(Page.Request["IsDlg"]))
            {
                output.WriteLine("function maximizeWindow(){try{");
                output.WriteLine("var currentDialog = SP.UI.ModalDialog.get_childDialog();");
                output.WriteLine("if(currentDialog != null){");
                output.WriteLine("if(!currentDialog.$S_0){");
                output.WriteLine("currentDialog.$z();");
                output.WriteLine("}}}catch(e){}");

                if(bShowSearch)
                {
                    if(bHasSearchResults)
                        output.WriteLine("setTimeout(\"loadX" + sFullGridId + "('" + sSearchField + "','" + sSearchValue + "','" + sSearchType + "'\"), 100);");
                }
                else
                    output.WriteLine("setTimeout(\"loadX" + sFullGridId + "()\", 100);");

                output.WriteLine("}");

                output.WriteLine("ExecuteOrDelayUntilScriptLoaded(maximizeWindow, 'sp.ui.dialog.js');");
                //output.WriteLine("ExecuteOrDelayUntilScriptLoaded(, 'sp.ui.dialog.js');");

            }
            else
            {
                if(bShowSearch)
                {
                    if(bHasSearchResults)
                        output.Write("_spBodyOnLoadFunctionNames.push(\"loadX" + sFullGridId + "('" + sSearchField + "','" + sSearchValue + "','" + sSearchType + "')\");");
                }
                else
                    output.WriteLine("ExecuteOrDelayUntilScriptLoaded(loadX" + sFullGridId + ", 'sp.ui.dialog.js');");
                //output.Write("_spBodyOnLoadFunctionNames.push(\"loadX" + sFullGridId + "\");");
            }
            
            output.Write("</script>");

            //if (inEditMode)
            {
                output.Write(Properties.Resources.txtGridSaving.Replace("#gridid#", sFullGridId).Replace("#siteurl#",web.Url));
                
 
                output.Write("<script>");
                output.Write("initmb();");
                output.Write("</script>");
                //output.Write("<a href=\"javascript:alert(mygrid" + sFullGridId + ".getSelectedRowId());\">getdata</a>");
            }
            
        }

        static string EncodeNonAsciiCharacters(string value)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                if (c > 127 || c==47)
                {
                    // This character is too big for ASCII 
                    string encodedValue = "\\u" + ((int)c).ToString("x4");
                    sb.Append(encodedValue);
                }
                else
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        private void addNewButton(HtmlTextWriter output, SPWeb web)
        {
            if((!hideNew && SPContext.Current.ViewContext.View != null) || (BOOLShowViewBar && !hideNew) || (!hideNew && bIsFormWebpart))
            {
                output.WriteLine(@"<table width=""100%"">");

                switch(newMenuStyle)
                {
                        
                    case 0:
                        output.WriteLine(@"<tr><TD style=""PADDING-BOTTOM: 3px"" class=ms-addnew><SPAN style=""POSITION: relative; WIDTH: 10px; DISPLAY: inline-block; HEIGHT: 10px; OVERFLOW: hidden"" class=s4-clust><IMG style=""POSITION: absolute; TOP: -128px !important; LEFT: 0px !important"" alt="""" src=""/_layouts/images/fgimg.png""></SPAN>&nbsp;");
                        output.WriteLine(@"<A id=idHomePageNewItem class=ms-addnew onclick='newItem" + sFullGridId + @"(" + bUsePopUp.ToString().ToLower() + @");' href=""javascript:void(0);"" target=_self>Add new item</A>");
                        output.WriteLine(@"</td></tr>");
                        break;
                    case 1:
                        if(rollupLists != "")
                        {
                            string[] tRollupLists = rollupLists.ToString().Split(',');

                            if(tRollupLists.Length > 1)
                            {
                                output.WriteLine(@"<style media=""all"" type=""text/css"">@import ""/_layouts/epmlive/addmenu.css"";</style>");
                                output.WriteLine(@"<tr><TD style=""PADDING-BOTTOM: 3px"" class=ms-addnew>");
                                output.WriteLine(@"<ul class=""addmenu"">");
                                output.WriteLine(@"<li class=""top""><a target=""_self"" class=""top_link ms-addnew""><span><img style=""top: 2px;"" src=""/_layouts/images/newrowheader.png"" valign=""middle"">Add New Item</span></a>");
                                output.WriteLine(@"<ul class=""sub"">");

                                for(int i = 0; i < tRollupLists.Length; i++)
                                {
                                    string[] tRlist = tRollupLists[i].Split('|');
                                    output.WriteLine(@"<li><a href=""" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + @"/_layouts/epmlive/newitem.aspx?List=" + tRlist[0] + @""" target=""_self"">" + tRlist[0] + "</a></li>");
                                }

                                output.WriteLine(@"</ul>");
                                output.WriteLine(@"</li>");
                                output.WriteLine(@"</ul>");
                                output.WriteLine(@"</td></tr>");
                            }
                            else
                            {
                                string[] tRlist = tRollupLists[0].Split('|');
                                output.WriteLine(@"<tr><TD style=""PADDING-BOTTOM: 3px"" class=ms-addnew><SPAN style=""POSITION: relative; WIDTH: 10px; DISPLAY: inline-block; HEIGHT: 10px; OVERFLOW: hidden"" class=s4-clust><IMG style=""POSITION: absolute; TOP: -128px !important; LEFT: 0px !important"" alt="""" src=""/_layouts/images/fgimg.png""></SPAN>&nbsp;");
                                output.WriteLine(@"<A id=idHomePageNewItem class=ms-addnew href=""" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + @"/_layouts/epmlive/newitem.aspx?List=" + tRlist[0] + @""" target=_self>Add new " + tRlist[0] + " item</A>");
                                output.WriteLine(@"</td></tr>");
                            }
                        }

                        break;
                    case 2:

                        if(hasList)
                        {
                            output.WriteLine(@"<tr><TD style=""PADDING-BOTTOM: 3px"" class=ms-addnew><SPAN style=""POSITION: relative; WIDTH: 10px; DISPLAY: inline-block; HEIGHT: 10px; OVERFLOW: hidden"" class=s4-clust><IMG style=""POSITION: absolute; TOP: -128px !important; LEFT: 0px !important"" alt="""" src=""/_layouts/images/fgimg.png""></SPAN>&nbsp;");
                            output.WriteLine(@"<A id=idHomePageNewItem class=ms-addnew href=""javascript:newAppPopup('" + list.ID.ToString().ToUpper() + @"');"">Add new " + newMenuName + "</A>");
                            output.WriteLine(@"</td></tr>");
                        }
                        else
                        {
                            output.WriteLine(@"<tr><TD style=""PADDING-BOTTOM: 3px"" class=ms-addnew><SPAN style=""POSITION: relative; WIDTH: 10px; DISPLAY: inline-block; HEIGHT: 10px; OVERFLOW: hidden"" class=s4-clust><IMG style=""POSITION: absolute; TOP: -128px !important; LEFT: 0px !important"" alt="""" src=""/_layouts/images/fgimg.png""></SPAN>&nbsp;");
                            output.WriteLine(@"<A id=idHomePageNewItem class=ms-addnew href=""" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + @"/_layouts/epmlive/newapp.aspx?List={" + list.ID.ToString().ToUpper() + @"}"" target=_self>Add new " + newMenuName + "</A>");
                            output.WriteLine(@"</td></tr>");
                        }
                        break;
                }

                output.WriteLine(@"</table>");
                
            }
        }

        private void addGridProperties(HtmlTextWriter output, SPWeb web)
        {
            output.WriteLine("mygrid" + sFullGridId + "._epkview = '" + EPKView + "';");
            output.WriteLine("mygrid" + sFullGridId + "._useparent = " + useParent + ";");
            output.WriteLine("mygrid" + sFullGridId + "._curPlanner = '" + PlannerV2CurPlanner.Trim() + "';");
            output.WriteLine("mygrid" + sFullGridId + "._epkurl = '" + EPKURL + "';");
            output.WriteLine("mygrid" + sFullGridId + "._epkcostview = '" + EPKCostView + "';");
            output.WriteLine("mygrid" + sFullGridId + ".addFocusedCommands = function($arr){");
            //foreach (SPUserCustomAction ca in list.UserCustomActions)
            //{
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(ca.CommandUIExtension);

            //    output.WriteLine("Array.add($arr, '" + doc.FirstChild.FirstChild.FirstChild.FirstChild.Attributes["Command"].Value + "');");
            //}
            output.WriteLine("return $arr;}");
            output.WriteLine("mygrid" + sFullGridId + ".getGlobalCommands = function($arr){return $arr;}");
            output.WriteLine("mygrid" + sFullGridId + ".canHandleCommand = function($Grid, commandId){");
            //foreach (SPUserCustomAction ca in list.UserCustomActions)
            //{
            //    XmlDocument doc = new XmlDocument();
            //    doc.LoadXml(ca.CommandUIExtension);

            //    output.WriteLine("if(commandId==='" + doc.FirstChild.FirstChild.FirstChild.FirstChild.Attributes["Command"].Value + "') return true;");
            //}
            output.WriteLine("return false;}");
            output.WriteLine("mygrid" + sFullGridId + ".handleCommand = function($Grid, commandId, properites){if(typeof(properties) != 'undefined')return properties;}");
            output.WriteLine("mygrid" + sFullGridId + "._webpartid = '" + this.ID + "';");

            if(rollupLists == "" && ((uint)list.BaseTemplate == 10702 || (uint)list.BaseTemplate == 107 || (uint)list.BaseTemplate == 10115))
            {
                output.WriteLine("var objStsSync = GetStssyncData('tasks','Client', '', '/_layouts/images/menu');");
                string listurl = EncodeNonAsciiCharacters(System.IO.Path.GetDirectoryName(list.DefaultViewUrl).Replace("\\","/"));
                output.WriteLine("mygrid" + sFullGridId + "._outlookexport = \"javaScript:ExportHailStorm('tasks','" + EncodeNonAsciiCharacters(web.Url) + "','{" + list.ID.ToString().ToUpper() + "}','" + web.Title + "','" + list.Title + "','" + listurl + "','','" + listurl + "')\";");
            }
            else
                output.WriteLine("mygrid" + sFullGridId + "._outlookexport = '';");
            
            output.WriteLine("mygrid" + sFullGridId + "._gridMode = '" + newGridMode + "';");
            

            string image = "/_layouts/" + web.Language + "/images/formatmap32x32.png";
            string top = "-160";
            string left = "-384";



            output.WriteLine("mygrid" + sFullGridId + "._hasTemplateList = " + hasList.ToString().ToLower() + ";");

            switch (newMenuStyle)
            {
                case 0:
                    output.WriteLine("mygrid" + sFullGridId + "._newmenumode = " + newMenuStyle + ";");
                    if(bIsFormWebpart || bIsLinkedItemView)
                    {
                        if(bUsePopUp)
                            output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + list.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl + "?LookupField=" + LookupFilterField + "&LookupValue=" + LookupFilterValue + "';");
                        else
                            output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + list.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl + "?LookupField=" + LookupFilterField + "&LookupValue=" + LookupFilterValue + "&Source=" + HttpUtility.UrlEncode(HttpContext.Current.Request.Url.ToString()) + "';");
                        
                    }
                    else
                    {
                        output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + list.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl + "';");
                    }
                    output.WriteLine("mygrid" + sFullGridId + "._newfolder = " + list.EnableFolderCreation.ToString().ToLower() + ";");
                    if (list.ContentTypesEnabled)
                    {
                        string newButton = "";
                        foreach (SPContentType ct in list.ContentTypes)
                        {
                            if (ct.Name != "Folder")
                            {
                                top = "-160";
                                left = "-384";
                                if (ct.Id.IsChildOf(new SPContentTypeId("0x0120")))
                                {
                                    top = "-448";
                                    left = "-320";
                                }
                                newButton += @"<Button Id='Ribbon.ListItem.New.NewListItem." + ct.Id + "' Command='NewItemCT' CommandValueId='" + ct.Id + "' LabelText='" + ct.Name + "' Description='" + ct.Description + "' Image32by32='" + image + "' CommandType='OptionSelection' Image32by32Top='" + top + "' Image32by32Left='" + left + "'/>";
                            }
                        }
                        output.WriteLine("mygrid" + sFullGridId + "._newmenu = \"" + newButton + "\";");
                    }
                    else
                    {
                        output.WriteLine("mygrid" + sFullGridId + @"._newmenu = ""<Button Id=\'Ribbon.ListItem.New.NewListItem\' Command=\'NewItem\' CommandValueId=\'NewListItem\' LabelText=\'New Item\' Description=\'Create a New Item\' Image32by32=\'" + image + @"\' CommandType=\'OptionSelection\' Image32by32Top=\'" + top + @"\' Image32by32Left=\'" + left + @"\'/>""");
                    }
                    break;
                case 1:
                    output.WriteLine("mygrid" + sFullGridId + "._newmenumode = " + newMenuStyle + ";");
                    output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/newitem.aspx';");
                    output.WriteLine("mygrid" + sFullGridId + "._newfolder = false;");
                    try
                    {
                        string newButton = "";
                        if (rollupLists != "")
                        {
                            string[] tRollupLists = rollupLists.ToString().Split(',');
                            for (int i = 0; i < tRollupLists.Length; i++)
                            {
                                string[] tRlist = tRollupLists[i].Split('|');
                                newButton += "<Button Id='Ribbon.ListItem.New.NewListItem." + tRlist[0] + "' Command='NewItemRollup' CommandValueId='" + tRlist[0] + "' LabelText='" + tRlist[0] + " Item' Description='Create a new " + tRlist[0] + " item.' Image32by32='" + image + "' CommandType='OptionSelection' Image32by32Top='" + top + "' Image32by32Left='" + left + "'/>";
                            }
                        }
                        output.WriteLine("mygrid" + sFullGridId + "._newmenu = \"" + newButton + "\";");
                    }
                    catch { }
                    break;
                case 2:

                    if(hasList)
                    {
                        output.WriteLine("mygrid" + sFullGridId + "._newmenumode = 3;");
                        output.WriteLine("mygrid" + sFullGridId + @"._newmenu = ""<Button Id=\'Ribbon.ListItem.New.NewListItem." + list.ID + @"\' Command=\'NewItemApp2\' CommandValueId=\'" + newMenuName + @"\' LabelText=\'" + newMenuName + @"\' Description=\'Create a new " + newMenuName + @" item.\' Image32by32=\'" + image + @"\' CommandType=\'OptionSelection\' Image32by32Top=\'" + top + @"\' Image32by32Left=\'" + left + @"\'/>""");
                        output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/newapp.aspx';");
                        output.WriteLine("mygrid" + sFullGridId + "._newfolder = false;");
                        output.WriteLine("mygrid" + sFullGridId + "._defaultct = '" + list.ID + "';");
                    }
                    else
                    {
                        output.WriteLine("mygrid" + sFullGridId + "._newmenumode = " + newMenuStyle + ";");
                        output.WriteLine("mygrid" + sFullGridId + @"._newmenu = ""<Button Id=\'Ribbon.ListItem.New.NewListItem." + list.ID + @"\' Command=\'NewItemApp\' CommandValueId=\'" + newMenuName + @"\' LabelText=\'" + newMenuName + @"\' Description=\'Create a new " + newMenuName + @" item.\' Image32by32=\'" + image + @"\' CommandType=\'OptionSelection\' Image32by32Top=\'" + top + @"\' Image32by32Left=\'" + left + @"\'/>""");
                        output.WriteLine("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/newapp.aspx';");
                        output.WriteLine("mygrid" + sFullGridId + "._newfolder = false;");
                        output.WriteLine("mygrid" + sFullGridId + "._defaultct = '" + list.ID + "';");
                    }
                    
                    break;
            };
            try
            {
                output.WriteLine("mygrid" + sFullGridId + "._modifylist = " + list.DoesUserHavePermissions(SPBasePermissions.ManageLists).ToString().ToLower() + ";");
            }
            catch
            {
                output.WriteLine("mygrid" + sFullGridId + "._modifylist = false;");
            }
            try
            {
                output.WriteLine("mygrid" + sFullGridId + "._listperms = " + list.DoesUserHavePermissions(SPBasePermissions.ManagePermissions).ToString().ToLower() + ";");
            }
            catch
            {
                output.WriteLine("mygrid" + sFullGridId + "._listperms = false;");
            }
            output.WriteLine("mygrid" + sFullGridId + "._shownewmenu = " + (!hideNew).ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._allowedit = " + allowEditToggle.ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._listid = '" + HttpUtility.UrlEncode(list.ID.ToString()).ToUpper() + "';");
            output.WriteLine("mygrid" + sFullGridId + "._webid = '" + HttpUtility.UrlEncode(web.ID.ToString()).ToUpper() + "';");
            output.WriteLine("mygrid" + sFullGridId + "._listname = '" + HttpUtility.UrlEncode(list.Title) + "';");
            output.WriteLine("mygrid" + sFullGridId + "._viewid = '" + HttpUtility.UrlEncode(view.ID.ToString()).ToUpper() + "';");
            output.WriteLine("mygrid" + sFullGridId + "._viewurl = '" + web.Url.Replace(" ", "%20") + "/" + view.Url.Replace(" ", "%20") + "';");
            output.WriteLine("mygrid" + sFullGridId + "._siteurl = '" + web.Site.ServerRelativeUrl + "';");
            output.WriteLine("mygrid" + sFullGridId + "._webrelurl = '" + (web.ServerRelativeUrl == "/" ? "" : web.ServerRelativeUrl) + "';");
            output.WriteLine("mygrid" + sFullGridId + "._viewname = '" + view.Title + "';");
            output.WriteLine("mygrid" + sFullGridId + "._basetype = '" + list.BaseType + "';");
            output.WriteLine("mygrid" + sFullGridId + "._templatetype = '" + (int)list.BaseTemplate + "';");
            output.WriteLine("mygrid" + sFullGridId + "._newrow = " + (inEditMode && allowInsertRow).ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._rolluplists = '" + rollupLists.Replace(",", ";").Replace("|", ",") + "';");
            output.WriteLine("mygrid" + sFullGridId + "._brollups = " + bRollups.ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._requestList = " + requestList.ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._usepopup = " + bUsePopUp.ToString().ToLower() + ";");
            output.WriteLine("mygrid" + sFullGridId + "._lookupfield = '" + Page.Request["lookupfield"] + "';");
            output.WriteLine("mygrid" + sFullGridId + "._lookupfieldlist = '" + Page.Request["LookupFieldList"] + "';");
            
            
            StringBuilder sbExcel = new StringBuilder();

            sbExcel.Append(HttpUtility.UrlEncode(((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl)).Replace("%", @"\u00"));
            sbExcel.Append(@"\u002f_vti_bin\u002fowssvr.dll?CS=65001\u0026Using=_layouts\u002fquery.iqy\u0026List=");
            sbExcel.Append(@"\u00257B");
            sbExcel.Append(list.ID.ToString().ToUpper().Replace("-", @"\u00252D").ToString());
            sbExcel.Append(@"\u00257D");
            sbExcel.Append(@"\u0026View=\u00257B");
            sbExcel.Append(view.ID.ToString().ToUpper().Replace("-", @"\u00252D").ToString());
            sbExcel.Append(@"\u00257D");
            sbExcel.Append(@"\u0026RootFolder=");
            sbExcel.Append(System.IO.Path.GetDirectoryName(view.ServerRelativeUrl).Replace(@"\", @"\u00252F"));
            sbExcel.Append(@"\u0026CacheControl=1");

            output.WriteLine("mygrid" + sFullGridId + "._excell = '" + sbExcel + "';");

            output.WriteLine("mygrid" + sFullGridId + "._gridid = '" + sFullGridId + "';");
            StringBuilder sbForms = new StringBuilder();
            foreach (SPForm form in list.Forms)
            {
                switch (form.Type)
                {
                    case PAGETYPE.PAGE_DISPLAYFORM:
                        sbForms.Append("<Button Id='Ribbon.List.Settings.EditDefaultForms.Menu.MS.EditDefaultFormDisplay' CommandValueId='" + form.ServerRelativeUrl + "' Command='EditDefaultForm' Image16by16='/_layouts/" + web.Language + "/images/formatmap16x16.png' Image16by16Top='-176' Image16by16Left='-16' Image32by32='/_layouts/" + web.Language + "/images/formatmap32x32.png' Image32by32Top='-256' Image32by32Left='-320' LabelText='Default Display Form'/>");
                        break;
                    case PAGETYPE.PAGE_EDITFORM:
                        sbForms.Append("<Button Id='Ribbon.List.Settings.EditDefaultForms.Menu.MS.EditDefaultFormDisplay' CommandValueId='" + form.ServerRelativeUrl + "' Command='EditDefaultForm' Image16by16='/_layouts/" + web.Language + "/images/formatmap16x16.png' Image16by16Top='-32' Image16by16Left='-80' Image32by32='/_layouts/" + web.Language + "/images/formatmap32x32.png' Image32by32Top='-96' Image32by32Left='-448' LabelText='Default Edit Form'/>");
                        break;
                    case PAGETYPE.PAGE_NEWFORM:
                        sbForms.Append("<Button Id='Ribbon.List.Settings.EditDefaultForms.Menu.MS.EditDefaultFormDisplay' CommandValueId='" + form.ServerRelativeUrl + "' Command='EditDefaultForm' Image16by16='/_layouts/" + web.Language + "/images/formatmap16x16.png' Image16by16Top='-128' Image16by16Left='-224' Image32by32='/_layouts/" + web.Language + "/images/formatmap32x32.png' Image32by32Top='-128' Image32by32Left='-96' LabelText='Default New Form'/>");
                        break;
                };

            }
            output.WriteLine("mygrid" + sFullGridId + "._formmenus = \"" + sbForms + "\";");
        }
        
        private string getViewFields()
        {
            string fields = "";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Query>" + view.Query + "</Query>");
            XmlNode ndGroup = doc.SelectSingleNode("//GroupBy");

            foreach (string f in view.ViewFields)
            {
                SPField field = getRealField(list.Fields.GetFieldByInternalName(f));
                if (field.InternalName == "Title" || field.InternalName == "URL" || field.InternalName == "FileLeafRef")
                {
                    titlefound = true;
                }
                fields += "\n" + field.InternalName;
            }
            if (!titlefound && (ndGroup != null || getAdditionalGroupings() != "|"))
                fields = "\n_Title_" + fields;
            if (fields.Length > 1)
                fields = fields.Substring(1);

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(fields);

            return Convert.ToBase64String(toEncodeAsBytes);
        }

        private SPField getRealField(SPField field)
        {
            try
            {
                if (field.Type == SPFieldType.Computed)
                {
                    {
                        XmlDocument fieldXml = new XmlDocument();
                        fieldXml.LoadXml(field.SchemaXml);

                        string parentField = "";
                        try
                        {
                            parentField = fieldXml.FirstChild.Attributes["DisplayNameSrcField"].Value;
                        }
                        catch { }
                        if (parentField != "")
                        {
                            try
                            {
                                field = field.ParentList.Fields.GetFieldByInternalName(parentField);
                            }
                            catch { }
                        }
                    }
                }
            }
            catch { }
            return field;
        }

        private string getAdditionalGroupings()
        {
            return PropGroup1 + "|" + PropGroup2;
        }

        private void appendParam(string param, string val)
        {
            sFullParamList += "\n" + param + "\t" + val;
        }

        private int GetLinkedItemInfo(SPWeb web)
        {
            int retItem = 0;

            if(!string.IsNullOrEmpty(Page.Request["LookupFieldList"]))
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    //using (SPSite s = SPContext.Current.Site)
                    {
                        SqlConnection cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                        cn.Open();

                        try
                        {
                            SqlCommand cmd = new SqlCommand("SELECT VALUE FROM PERSONALIZATIONS where userid=@userid and [key]=@key and listid=@listid", cn);
                            cmd.Parameters.AddWithValue("@userid", web.CurrentUser.ID);
                            cmd.Parameters.AddWithValue("@key", "LIP");
                            cmd.Parameters.AddWithValue("@listid", Page.Request["LookupFieldList"]);
                            cmd.ExecuteNonQuery();
                            SqlDataReader dr = cmd.ExecuteReader();
                            if(dr.Read())
                            {
                                ArrayList lookupFilterIDs = new ArrayList(dr.GetString(0).Split(','));

                                if(lookupFilterIDs.Count == 1 && !string.IsNullOrEmpty(lookupFilterIDs[0].ToString()))
                                {
                                    SPQuery query = new SPQuery();
                                    query.Query = "<Where><Eq><FieldRef Name=\"Title\"/><Value Type=\"Text\">" + lookupFilterIDs[0].ToString() + "</Value></Eq></Where>";

                                    SPList templist = web.Lists[new Guid(Page.Request["LookupFieldList"])];

                                    SPListItemCollection lic = templist.GetItems(query);
                                    if(lic.Count == 1)
                                    {
                                        retItem = lic[0].ID;
                                    }
                                }
                            }
                            dr.Close();
                        }
                        catch { }
                        
                        cn.Close();
                    }
                });
            }
            return retItem;
        }

        private void buildParams()
        {
            sFullParamList = "";
            //bool useNew = false; 
            appendParam("List", PropList);
            appendParam("View", PropView);
            appendParam("FilterField", Page.Request["FilterField1"]);
            appendParam("FilterValue", Page.Request["FilterValue1"]);
            appendParam("LookupField", Page.Request["lookupfield"]);
            appendParam("LookupFieldList", Page.Request["LookupFieldList"]);
            appendParam("GridName", sFullGridId);
            appendParam("AGroups", getAdditionalGroupings());
            if(!string.IsNullOrEmpty(Page.Request["lookupfield"]) && !string.IsNullOrEmpty(Page.Request["LookupFieldList"]))
                appendParam("Expand", "10");
            else
                appendParam("Expand", PropExpand);

            if(_myProvider != null)
                appendParam("ReportID", _myProvider.ReportID.Substring(2).Replace("_","-"));

            SPWeb web = SPContext.Current.Web;
            try
            {
                list = web.GetListFromUrl(PropList);
                view = list.Views[PropView];
            }
            catch { }


            if(SPContext.Current.ListItem != null && list != null)
            {
                foreach(SPField field in list.Fields)
                {
                    if(field.Type == SPFieldType.Lookup)
                    {
                        SPFieldLookup oLookup = (SPFieldLookup)field;
                        try
                        {
                            if(new Guid(oLookup.LookupList) == SPContext.Current.List.ID)
                            {
                                bIsFormWebpart = true;
                                appendParam("LookupFilterField", field.InternalName);
                                appendParam("LookupFilterValue", SPContext.Current.ListItem.ID.ToString());
                                LookupFilterField = field.InternalName;
                                LookupFilterValue = SPContext.Current.ListItem.ID.ToString();
                                break;
                            }
                        }
                        catch { }
                    }
                }
            }

            int litem = GetLinkedItemInfo(web);
            if(litem != 0)
            {
                bIsLinkedItemView = true;
                LookupFilterField = Page.Request["lookupfield"];
                LookupFilterValue = litem.ToString();
            }

            EPMLiveCore.GridGanttSettings gSettings = new EPMLiveCore.GridGanttSettings(list);

            bAssociatedItems = gSettings.AssociatedItems;

            if (PropUseDefaults.Value)
            {
                //if (props.Length >= 12)
                {
                    appendParam("Start", gSettings.StartDate);
                    appendParam("Finish", gSettings.DueDate);
                    appendParam("Percent", gSettings.Progress);
                    appendParam("WBS", gSettings.WBS);
                    appendParam("Milestone", gSettings.Milestone);
                    appendParam("Executive", (gSettings.Executive ? "on" : ""));
                    appendParam("Info", gSettings.Information);
                    appendParam("LType", gSettings.ItemLink);
                    if(!bIsFormWebpart)
                    {
                        appendParam("RLists", gSettings.RollupLists);
                        rollupLists = gSettings.RollupLists;
                        if(gSettings.RollupLists != "")
                            bRollups = true;

                        appendParam("RSites", gSettings.RollupSites.Replace("\r\n", ","));
                    }
                    else
                    {
                        appendParam("RLists", "");
                        appendParam("RSites", "");
                    }
                    
                    //appendParam("WBS", props[10]);
                    
                    appendParam("HideNew", gSettings.HideNewButton.ToString());
                    hideNew = gSettings.HideNewButton;
                    
                    if(!gSettings.EnableContentReporting)
                        appendParam("UsePerf", gSettings.Performance.ToString());
                    else
                        appendParam("UsePerf", "false");
                    
                    appendParam("AllowEdit", gSettings.AllowEdit.ToString());
                    allowEditToggle = gSettings.AllowEdit;

                    appendParam("EditDefault", gSettings.EditDefault.ToString());
                    inEditMode = gSettings.EditDefault;

                    appendParam("ShowInsert", gSettings.ShowInsert.ToString());
                    allowInsertRow = gSettings.ShowInsert;

                    appendParam("DisableNew", gSettings.DisableNewItemMod.ToString());
                    disableNewButtonModification = gSettings.DisableNewItemMod;

                    appendParam("UseNew", gSettings.UseNewMenu.ToString());
                    useNewMenu = gSettings.UseNewMenu;

                    appendParam("NewName", gSettings.NewMenuName);
                    newMenuName = gSettings.NewMenuName;

                    appendParam("UsePopup", gSettings.UsePopup.ToString());
                    bUsePopUp = gSettings.UsePopup;

                    appendParam("Requests", gSettings.EnableRequests.ToString());
                    requestList = gSettings.EnableRequests;

                    useParent = gSettings.UseParent.ToString().ToLower();

                    appendParam("UseReporting", gSettings.EnableContentReporting.ToString());

                    bShowSearch = gSettings.Search;
                    bLockSearch = gSettings.LockSearch;
                }
            }
            else
            {
                //data = PropList + "\n" + PropView + "\n" + PropWBS + "\n" + PropExecView + "\n" + PropLinkType + "\n" + PropRollupList.Replace(",", "|").Replace("\r\n", ",") + "\n" + Page.Request["FilterField1"] + "\n" + Page.Request["FilterValue1"] + "\n" + PropRollupSites.Replace("\r\n", ",") + "\n" + sFullGridId + "\n" + getAdditionalGroupings() + "\n" + PropExpand + "\n" + PropPerformance.Value + "\n" + PropUsePopup + "\n" + PropHideNewButton;

                appendParam("Start", PropStart);
                appendParam("Finish", PropFinish);
                appendParam("Percent", PropProgress);
                appendParam("WBS", PropWBS);
                appendParam("Milestone", PropMilestone);
                appendParam("Executive", PropExecView);
                appendParam("Info", PropInformation);
                appendParam("LType", PropLinkType);
                if(!bIsFormWebpart)
                {

                    appendParam("RLists", PropRollupList.Replace(",", "|").Replace("\r\n", ","));
                    rollupLists = PropRollupList.Replace(",", "|").Replace("\r\n", ",");
                    if(PropRollupList != "")
                        bRollups = true;

                    appendParam("RSites", PropRollupSites.Replace("\r\n", ","));
                }
                else
                {
                    appendParam("RLists", "");
                    appendParam("RSites", "");
                }
                appendParam("HideNew", PropHideNewButton.Value.ToString());
                appendParam("UsePerf", PropPerformance.Value.ToString());
                appendParam("AllowEdit", PropAllowEdit.Value.ToString());
                appendParam("EditDefault", PropEdit.Value.ToString());
                appendParam("ShowInsert", PropShowInsertRow.Value.ToString());
                //appendParam("DisableNew", Prop);
                //appendParam("UseNew", props[17]);
                //appendParam("NewName", props[18]);
                appendParam("UsePopup", PropUsePopup.Value.ToString());
                //appendParam("Requests", props[20]);
                bUsePopUp = PropUsePopup.Value;

                inEditMode = PropEdit.Value;
                allowInsertRow = PropShowInsertRow.Value;
                allowEditToggle = PropAllowEdit.Value;
                hideNew = PropHideNewButton.Value;
                useParent = PropUseParent.Value.ToString().ToLower();


                appendParam("UseNew", gSettings.UseNewMenu.ToString());
                useNewMenu = gSettings.UseNewMenu;

                appendParam("NewName", gSettings.NewMenuName);
                newMenuName = gSettings.NewMenuName;

                appendParam("Requests", gSettings.EnableRequests.ToString());
                requestList = gSettings.EnableRequests;


                try
                {
                    bShowSearch = PropShowSearch.Value;
                }
                catch { }
                try
                {
                    bLockSearch = PropLockSearch.Value;
                }
                catch { }

            }

            appendParam("UseParent", useParent);
            
            //General Always
            try
            {
                if(SPContext.Current.ViewContext.View != null)
                    appendParam("ShowCheckboxes", view.TabularView.ToString());
            }
            catch { }

            
            sFullParamList = sFullParamList.Substring(1);

            byte[] toEncodeAsBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(sFullParamList);

            sFullParamList = Convert.ToBase64String(toEncodeAsBytes);


           

            try
            {
                sSearchField = Page.Request[list.ID.ToString("N") + "_searchfield"].ToString();
                sSearchValue = Page.Request[list.ID.ToString("N") + "_searchvalue"].ToString();
                sSearchType = Page.Request[list.ID.ToString("N") + "_searchtype"].ToString();

                if(!String.IsNullOrEmpty(sSearchField))
                    bHasSearchResults = true;
            }
            catch { }



            SPSecurity.RunWithElevatedPrivileges(delegate()
            {

                using(SPSite site = new SPSite(web.Site.ID))
                {
                    using(SPWeb w = site.OpenWeb(web.ID))
                    {
                        if(useNewMenu)
                        {
                            newMenuStyle = 2;
                        }
                        else if(bRollups && !disableNewButtonModification)
                        {
                            if(w.Webs.Count > 0)
                                newMenuStyle = 1;
                        }
            
                        string webpartid = "";
                        try
                        {
                            // switchto = Page.Request["switchto"];
                            webpartid = Page.Request["webpartid"].ToString();
                            if(webpartid == this.ID)
                            {
                                newGridMode = Page.Request["gridmode"].ToString();
                            }
                        }
                        catch { }
                        if(webpartid == this.ID)
                        {
                            try
                            {
                                if(newGridMode == "datasheet")
                                    inEditMode = true;
                                else if(newGridMode != "")
                                    inEditMode = false;
                            }
                            catch(Exception ex)
                            {
                                error = "Error Saving Personalization: " + ex.Message;
                            }
                        }

                        if(rollupLists != "")
                            allowInsertRow = false;
                        if(inEditMode)
                            newGridMode = "datasheet";
                        //case 0:
                        //            output.Write("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + list.Forms[PAGETYPE.PAGE_NEWFORM].ServerRelativeUrl + "';");
                        //            break;
                        //        case 1:
                        //            output.Write("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/newitem.aspx';");
                        //            break;
                        //        case 2:
                        //            output.Write("mygrid" + sFullGridId + "._newitemurl = '" + ((web.ServerRelativeUrl == "/") ? "" : web.ServerRelativeUrl) + "/_layouts/epmlive/newapp.aspx';");
                        //            break;
                        try
                        {

                            if(web.Site.Features[new Guid("e6df7606-1541-4bf1-a810-e8e9b11819e3")] != null)
                            {
                                System.Collections.Generic.Dictionary<string, EPMLiveCore.PlannerDefinition> pList = EPMLiveCore.CoreFunctions.GetPlannerList(web, null);

                                int bPlanner = 0;

                                foreach(System.Collections.Generic.KeyValuePair<string, EPMLiveCore.PlannerDefinition> de in pList)
                                {
                                    string id = (string)de.Key;
                                    EPMLiveCore.PlannerDefinition p = (EPMLiveCore.PlannerDefinition)de.Value;

                                    if(String.Equals(p.commandPrefix, list.Title, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        bPlanner = 1;
                                        break;
                                    }
                                    if(String.Equals(p.command, list.Title, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        PlannerV2CurPlanner = id;
                                        bPlanner = 2;
                                        break;
                                    }
                                }

                                if(bPlanner == 1)
                                {
                                    PlannerV2Menu = "<Button Id=\"Ribbon.ListItem.EPMLive.Planner\" Sequence=\"33\" Command=\"EPMLivePlanner\" LabelText=\"Edit Plan\" TemplateAlias=\"o1\" Image32by32=\"_layouts/epmlive/images/planner32.png\"/>";
                                }

                                else if(bPlanner == 2)
                                {
                                    PlannerV2Menu = "<Button Id=\"Ribbon.ListItem.EPMLive.Planner\" Sequence=\"33\" Command=\"TaskPlanner\" LabelText=\"Edit Plan\" TemplateAlias=\"o1\" Image32by32=\"_layouts/epmlive/images/planner32.png\"/>";
                                }

                                /*string planners = EPMLiveCore.CoreFunctions.getLockConfigSetting(w, "EPMLivePlannerPlanners", false);

                                foreach(string planner in planners.Split(','))
                                {
                                    if(!String.IsNullOrEmpty(planner))
                                    {
                                        string[] sPlanner = planner.Split('|');
                                        string tc = EPMLiveCore.CoreFunctions.getLockConfigSetting(w, "EPMLivePlanner" + sPlanner[0] + "TaskCenter", false);
                                        string pc = EPMLiveCore.CoreFunctions.getLockConfigSetting(w, "EPMLivePlanner" + sPlanner[0] + "ProjectCenter", false);

                                        if(String.Equals(list.Title, tc, StringComparison.CurrentCultureIgnoreCase))
                                        {
                                            PlannerV2Menu = "<Button Id=\"Ribbon.ListItem.EPMLive.Planner\" Sequence=\"33\" Command=\"TaskPlanner\" LabelText=\"Edit Plan\" TemplateAlias=\"o1\" Image32by32=\"_layouts/epmlive/images/planner32.png\"/>";
                                            PlannerV2CurPlanner = sPlanner[0];
                                            break;
                                        }

                                        if(String.Equals(list.Title, pc, StringComparison.CurrentCultureIgnoreCase))
                                        {

                                            //PlannerV2Menu = "<FlyoutAnchor Id=\"Ribbon.List.EPMLive.Planner\" Sequence=\"30\" PopulateDynamically=\"true\" PopulateOnlyOnce=\"false\" PopulateQueryCommand=\"ListEPMLivePlannerPopulate\" Command=\"ListEPMLivePlanner\" Image32by32=\"_layouts/epmlive/images/planner32.png\" LabelText=\"Edit Plan\" TemplateAlias=\"o1\"></FlyoutAnchor>";
                                            PlannerV2Menu = "<Button Id=\"Ribbon.ListItem.EPMLive.Planner\" Sequence=\"33\" Command=\"EPMLivePlanner\" LabelText=\"Edit Plan\" TemplateAlias=\"o1\" Image32by32=\"_layouts/epmlive/images/planner32.png\"/>";
                                            break;
                                        }
                                    }
                                }*/
                            }
                            EPKView = HttpUtility.UrlEncode(view.Title);
                            SPWeb rweb = site.RootWeb;
                            {
                                if(web.Site.Features[new Guid("158c5682-d839-4248-b780-82b4710ee152")] != null)
                                {
                                    EPKURL = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKURL");
                                    ArrayList arr = new ArrayList(EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKLists").ToLower().Split(','));
                                    if(arr.Contains(list.Title.ToLower()))
                                    {
                                        EPKEnabled = true;
                                        EPKButtons = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKButtons");
                                        EPKURL = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKURL");
                                        string[] sEPKViews = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKViews").Split('|');

                                        foreach(string sEPKView in sEPKViews)
                                        {
                                            string[] sEPKViewMap = sEPKView.Split(',');
                                            if(sEPKViewMap[0].ToLower() == view.Title.ToLower())
                                                EPKView = sEPKViewMap[1];
                                        }

                                        //if(EPKView == "")
                                        //    EPKView = view.Title;

                                        //EPKView = HttpUtility.UrlEncode(EPKView);

                                        EPKCostView = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPK" + list.Title.Replace(" ", "") + "_costview");
                                        if(EPKCostView == "")
                                        {
                                            string[] sEPKCostViews = EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKCostViews").Split('|');

                                            foreach(string sEPKView in sEPKCostViews)
                                            {
                                                string[] sEPKViewMap = sEPKView.Split(',');
                                                if(sEPKViewMap[0].ToLower() == view.Title.ToLower())
                                                    EPKCostView = sEPKViewMap[1];
                                            }
                                        }
                                        EPKCostView = HttpUtility.UrlEncode(EPKCostView);
                                    }
                                }
                            }


                            string _templateResourceUrl = EPMLiveCore.CoreFunctions.getConfigSetting(w, "EPMLiveTemplateGalleryURL", true, false);

                            try
                            {
                                using(SPSite tsite = new SPSite(_templateResourceUrl))
                                {
                                    using(SPWeb tweb = tsite.OpenWeb())
                                    {
                                        SPList tlist = tweb.Lists.TryGetList("Template Gallery");
                                        if(tlist != null)
                                            hasList = true;
                                    }
                                }
                            }
                            catch { }


                        }
                        catch { }
                    }
                }
            });

            if(Page.Request["IsDlg"] == "1")
                bUsePopUp = true;

            if(!string.IsNullOrEmpty(Page.Request["LookupFieldList"]))
            {
                bLockSearch = false;
                bShowSearch = false;
            }
        }

        

        public override ToolPart[] GetToolParts()
        {
            ToolPart[] toolparts = new ToolPart[3];
            toolparts[0] = new GridListViewToolpart();
            toolparts[1] = new WebPartToolPart();
            toolparts[2] = new CustomPropertyToolPart();


            return toolparts;
        }
    }
}
