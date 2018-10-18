﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using EPMLiveCore.ReportingProxy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.WebPartPages;
using EPMLiveCore.ReportHelper;
using System.Collections.ObjectModel;
using System.Diagnostics;
using EPMLiveCore.Infrastructure;
using EPMLiveCore.Infrastructure.Logging;
using static EPMLiveCore.Infrastructure.Logging.LoggingService;
using Microsoft.SharePoint.Administration;
using static System.Diagnostics.Trace;
using static EPMLiveCore.Layouts.epmlive.LayoutsHelper;

namespace EPMLiveCore.Layouts.epmlive
{
    public partial class gridganttsettings : LayoutsPageBase
    {
        protected TextBox txtRollupLists;
        protected TextBox txtRollupSites;
        protected CheckBox chkExecutive;
        protected CheckBox chkShowViewToolbar;
        protected CheckBox chkHideNewButton;
        protected CheckBox chkUsePopup;
        protected CheckBox chkResTools;
        protected CheckBox chkPerformance;
        protected CheckBox chkAllowEdit;
        protected CheckBox chkEditDefault;
        protected CheckBox chkShowInsert;
        protected CheckBox chkDisableNewRollup;
        protected CheckBox chkEnableRequests;
        protected CheckBox chkUseNewMenu;
        protected TextBox txtNewMenuName;

        protected static string strListName;
        protected string strListId;
        protected string strReportActionUrl = string.Empty;

        protected DataTable dtGroupsPermissions = new DataTable();

        const string REPORT_CHECK_URL = "/_layouts/epmlive/ReportCheckActions.aspx";
        public override string PageToRedirectOnCancel
        {
            get
            {
                return ((Web.ServerRelativeUrl == "/") ? "" : Web.ServerRelativeUrl) + "/_layouts/15/listedit.aspx?List=" + Request["List"];
            }
        }
        private string GetGroupsPermissionsAssignment()
        {
            StringBuilder sbGroupsPermissions = new StringBuilder();

            if (ViewState["dtGroupsPermissions"] != null)
            {
                dtGroupsPermissions = (DataTable)ViewState["dtGroupsPermissions"];

                foreach (DataRow dr in dtGroupsPermissions.Rows)
                {
                    sbGroupsPermissions.Append(dr["GroupsID"] + "~" + dr["PermissionsID"] + "|~|");
                }
            }
            if (sbGroupsPermissions.ToString().Length > 1)
            {
                string strTemp = sbGroupsPermissions.ToString().Substring(0, sbGroupsPermissions.ToString().Length - 3);
                return strTemp;
            }
            else
            {
                return String.Empty;
            }
        }

        protected void lnkGrpPermDelete_OnClick(object sender, EventArgs e)
        {
            LinkButton lnkDelete = (LinkButton)sender;

            if (ViewState["dtGroupsPermissions"] != null)
            {
                dtGroupsPermissions = (DataTable)ViewState["dtGroupsPermissions"];

                foreach (DataRow dr in dtGroupsPermissions.Rows)
                {
                    if ((dr["GroupsID"] + ";" + dr["PermissionsID"]) == lnkDelete.CommandArgument)
                    {
                        dtGroupsPermissions.Rows.Remove(dr);
                        break;
                    }
                }

                GvGroupsPermissions.DataSource = dtGroupsPermissions;
                GvGroupsPermissions.DataBind();

                ViewState["dtGroupsPermissions"] = dtGroupsPermissions;
                Button1.Focus();
            }
        }


        protected void btnGrpPermAdd_OnClick(object sender, EventArgs e)
        {
            HandleGridPermanentAddClickEvent(
                ViewState,
                ref dtGroupsPermissions,
                ddlGroups,
                ddlSPPermissions,
                GvGroupsPermissions,
                Button1);
        }
        protected override void OnPreLoad(EventArgs e)
        {
            base.OnPreLoad(e);

            EPMLiveScriptManager.RegisterScript(Page, new[]
            {
                "GridGanttSettings"
            });
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                strListId = Request["List"];
                strReportActionUrl = SPContext.Current.Web.Url + REPORT_CHECK_URL;
                chkTimesheet.InputAttributes.Add("onclick", "CheckTimesheet();");

                var web = SPContext.Current.Web;
                PopulatePermissionControls(web);
                var arrLookups = new ArrayList();
                var list = web.Lists[new Guid(Request["List"])];
                strListName = list.Title;
                PopulateListItemDropDowns();
                PopulateLinkDropDowns(web, list);
                PopulateInformationControls(list);
                PopulateGridGanttSettings(list, web);
                PopulateViewState();
                var rootWeb = web.Site.RootWeb;
                PopulateTimeSheetCheckBox(rootWeb, list);
                LoadReportControls(rootWeb, list);
            }
        }

        private void PopulatePermissionControls(SPWeb web)
        {
            var oRoleDefinitions = web.Site.RootWeb.RoleDefinitions;

            foreach (SPRoleDefinition oRoleDefinition in oRoleDefinitions)
            {
                ddlSPPermissions.Items.Add(new ListItem(oRoleDefinition.Name, oRoleDefinition.Id.ToString()));
            }

            var collGroups = web.Site.RootWeb.SiteGroups;
            foreach (SPGroup oGroup in collGroups)
            {
                ddlGroups.Items.Add(new ListItem(oGroup.Name, oGroup.ID.ToString()));
            }

            dtGroupsPermissions.Columns.Add("GroupsText");
            dtGroupsPermissions.Columns.Add("GroupsID");
            dtGroupsPermissions.Columns.Add("PermissionsText");
            dtGroupsPermissions.Columns.Add("PermissionsID");
        }

        private void PopulateListItemDropDowns()
        {
            ddlStartDate.Items.Add(new ListItem("< Select Field >", string.Empty));
            ddlDueDate.Items.Add(new ListItem("< Select Field >", string.Empty));
            ddlProgressBar.Items.Add(new ListItem("< Select Field >", string.Empty));
            ddlMilestone.Items.Add(new ListItem("< Select Field >", string.Empty));
            ddlInformation.Items.Add(new ListItem("< Select Field >", string.Empty));
            ddlWBS.Items.Add(new ListItem("< Select Field >", ""));
        }

        private void PopulateLinkDropDowns(SPWeb web, SPList list)
        {
            if (list.Title == CoreFunctions.getConfigSetting(web, "EPMLiveProjectCenter") || list.Title == "Project Center Rollup")
            {
                ddlItemLink.Items.Add(new ListItem("Edit Work Plan", "workplan"));
                ddlItemLink.Items.Add(new ListItem("Task Center", "tasks"));
            }

            SPSecurity.RunWithElevatedPrivileges(
                delegate
                {
                    using (var site = new SPSite(web.Site.ID))
                    {
                        using (var spWeb = site.OpenWeb(web.ID))
                        {
                            var planners = CoreFunctions.getLockConfigSetting(spWeb, "EPMLivePlannerPlanners", false);

                            var anyFound = planners.Split(',')
                                .Where(planner => !string.IsNullOrWhiteSpace(planner))
                                .Select(planner => planner.Split('|'))
                                .Select(
                                    sPlanner => CoreFunctions.getLockConfigSetting(
                                        spWeb,
                                        string.Format("EPMLivePlanner{0}ProjectCenter", sPlanner[0]),
                                        false)).Any(projectCenter => projectCenter == list.Title);

                            if (anyFound)
                            {
                                ddlItemLink.Items.Add(new ListItem("Planner", "planner"));
                            }
                        }
                    }
                });
        }

        private void PopulateInformationControls(SPList list)
        {
            foreach (SPField field in list.Fields)
            {
                if (!field.Hidden && field.Type != SPFieldType.Computed)
                {
                    switch (field.Type)
                    {
                        case SPFieldType.DateTime:
                            ddlStartDate.Items.Add(new ListItem(field.Title, field.InternalName));
                            ddlDueDate.Items.Add(new ListItem(field.Title, field.InternalName));
                            ddlInformation.Items.Add(new ListItem(field.Title, field.InternalName));
                            break;
                        case SPFieldType.Number:
                            ddlProgressBar.Items.Add(new ListItem(field.Title, field.InternalName));
                            ddlInformation.Items.Add(new ListItem(field.Title, field.InternalName));
                            break;
                        case SPFieldType.Text:
                            ddlWBS.Items.Add(new ListItem(field.Title, field.InternalName));
                            ddlInformation.Items.Add(new ListItem(field.Title, field.InternalName));
                            break;
                        case SPFieldType.Boolean:
                            ddlInformation.Items.Add(new ListItem(field.Title, field.InternalName));
                            ddlMilestone.Items.Add(new ListItem(field.Title, field.InternalName));
                            break;
                        default:
                            ddlInformation.Items.Add(new ListItem(field.Title, field.InternalName));
                            break;
                    }
                }
            }
        }

        private void PopulateGridGanttSettings(SPList list, SPWeb web)
        {
            var gSettings = new GridGanttSettings(list);

            PopulateTextBox(gSettings);

            var defaultTemps = GetAvailableDefaultTemps();
            var showAutoCreate = defaultTemps.Count > 1;

            chkAutoCreate.Checked = gSettings.EnableAutoCreation;
            ifcAutoCreate.Visible = showAutoCreate;

            PopulateDropDownLists(defaultTemps, gSettings, showAutoCreate, list);
            PopulateListIcon(gSettings);
            PopulateCheckBoxes(gSettings);
            PopulatePermissionControls(gSettings, web);
        }

        private void PopulateTextBox(GridGanttSettings gSettings)
        {
            txtRollupLists.Text = gSettings.RollupLists.Replace(",", "\r\n").Replace("|", ",");
            txtRollupSites.Text = gSettings.RollupSites.Replace(",", "\r\n");
            txtNewMenuName.Text = gSettings.NewMenuName;
            txtRequestList.Text = gSettings.RequestList;
        }

        private void PopulateDropDownLists(Dictionary<string, int> defaultTemps, GridGanttSettings gSettings, bool showAutoCreate, SPList list)
        {
            ddlStartDate.SelectedValue = gSettings.StartDate;
            ddlDueDate.SelectedValue = gSettings.DueDate;
            ddlProgressBar.SelectedValue = gSettings.Progress;
            ddlWBS.SelectedValue = gSettings.WBS;
            ddlMilestone.SelectedValue = gSettings.Milestone;
            ddlInformation.SelectedValue = gSettings.Information;
            ddlItemLink.SelectedValue = gSettings.ItemLink;
            ddlRibbonBehavior.SelectedValue = gSettings.RibbonBehavior;
            ddlAutoCreateTemplate.DataSource = defaultTemps;
            ddlAutoCreateTemplate.DataTextField = "Key";
            ddlAutoCreateTemplate.DataValueField = "Value";
            ddlAutoCreateTemplate.DataBind();
            ddlAutoCreateTemplate.SelectedValue = gSettings.AutoCreationTemplateId;
            ifcAutoCreateTemplate.Visible = showAutoCreate;
            ddlAutoCreateTemplate.Enabled = showAutoCreate && chkAutoCreate.Checked;

            PopulateParentSiteLookupDropDown(list, gSettings);
        }

        private void PopulateParentSiteLookupDropDown(SPList list, GridGanttSettings gSettings)
        {
            ddlParentSiteLookup.DataSource = GetAvailableParentSiteLookups(list, gSettings);
            ddlParentSiteLookup.DataTextField = "Key";
            ddlParentSiteLookup.DataValueField = "Value";
            ddlParentSiteLookup.DataBind();
            ddlParentSiteLookup.SelectedValue = gSettings.WorkspaceParentSiteLookup;
        }

        private void PopulateListIcon(GridGanttSettings gSettings)
        {
            spnListIcon.CssClass = string.IsNullOrEmpty(gSettings.ListIcon)
                ? "icon-square"
                : gSettings.ListIcon;
            hdnListIcon.Value = string.IsNullOrEmpty(gSettings.ListIcon)
                ? "icon-square"
                : gSettings.ListIcon;
        }

        private void PopulateCheckBoxes(GridGanttSettings gSettings)
        {
            chkExecutive.Checked = gSettings.Executive;
            chkUsePopup.Checked = gSettings.UsePopup;
            chkShowViewToolbar.Checked = gSettings.ShowViewToolbar;
            chkHideNewButton.Checked = gSettings.HideNewButton;
            chkPerformance.Checked = gSettings.Performance;
            chkAllowEdit.Checked = gSettings.AllowEdit;
            chkEditDefault.Checked = gSettings.EditDefault;
            chkShowInsert.Checked = gSettings.ShowInsert;
            chkDisableNewRollup.Checked = gSettings.DisableNewItemMod;
            chkUseNewMenu.Checked = gSettings.UseNewMenu;
            chkEnableRequests.Checked = gSettings.EnableRequests;
            chkWorkListFeat.Checked = gSettings.EnableWorkList;
            chkFancyForms.Checked = gSettings.EnableFancyForms;
            chkEmails.Checked = gSettings.SendEmails;
            chkDeleteRequest.Checked = gSettings.DeleteRequest;
            chkUseParent.Checked = gSettings.UseParent;
            chkSearch.Checked = gSettings.Search;
            chkLockSearch.Checked = gSettings.LockSearch;
            chkAssociatedItems.Checked = gSettings.AssociatedItems;
            chkEnableTeam.Checked = gSettings.BuildTeam;
            chkContentReporting.Checked = gSettings.EnableContentReporting;
            chkResTools.Checked = gSettings.EnableResourcePlan;
            chkDisplayRedirect.Checked = gSettings.DisplayFormRedirect;
            chkDisableThumbnails.Checked = gSettings.DisableThumbnails;

            SectionEmail.Visible = true;

            chkEnableTeamSecurity.Checked = gSettings.BuildTeamSecurity;
        }

        private void PopulatePermissionControls(GridGanttSettings gSettings, SPWeb web)
        {
            if (gSettings.BuildTeamPermissions.Length > 1)
            {
                var strOuter = gSettings.BuildTeamPermissions.Split(
                    new[]
                    {
                        "|~|"
                    },
                    StringSplitOptions.None);
                ProcessPermissionStrings(web, strOuter, dtGroupsPermissions);
            }

            GvGroupsPermissions.DataSource = dtGroupsPermissions;
            GvGroupsPermissions.DataBind();
        }

        private void PopulateViewState()
        {
            if (ViewState["dtGroupsPermissions"] == null)
            {
                ViewState.Add("dtGroupsPermissions", dtGroupsPermissions);
            }
        }

        private void PopulateTimeSheetCheckBox(SPWeb rootWeb, SPList list)
        {
            var lists = new ArrayList(CoreFunctions.getConfigSetting(rootWeb, "EPMLiveTSLists").Replace("\r\n", "\n").Split('\n'));

            if (lists.Contains(list.Title))
            {
                chkTimesheet.Checked = true;
            }
        }

        private void LoadReportControls(SPWeb rootWeb, SPList list)
        {
            var isReportingV2Enabled = false;
            try
            {
                isReportingV2Enabled = Convert.ToBoolean(CoreFunctions.getConfigSetting(rootWeb, "ReportingV2"));
            }
            catch (Exception ex)
            {
                TraceError("Exception Suppressed {0}", ex);
            }

            var reportBiz = new ReportBiz(SPContext.Current.Site.ID, SPContext.Current.Web.ID, isReportingV2Enabled);
            if (reportBiz.SiteExists())
            {
                ifsEnableReporting.Visible = true;

                var mappedLists = reportBiz.GetMappedLists();
                var mappedListIds = reportBiz.GetMappedListsIds();
                var isMapped = mappedLists.Contains(list.Title);
                var isMaster = mappedListIds.Contains(list.ID.ToString().ToLower());

                if (!isReportingV2Enabled)
                {
                    cbEnableReporting.Checked = isMapped;
                    cbEnableReporting.Enabled = isMaster || !isMapped;
                }
                else
                {
                    if (isMapped)
                    {
                        if (isMaster)
                        {
                            cbEnableReporting.Checked = true;
                            cbEnableReporting.Enabled = false;
                        }
                        else
                        {
                            cbEnableReporting.Checked = false;
                            cbEnableReporting.Enabled = true;
                        }
                    }
                    else
                    {
                        cbEnableReporting.Checked = false;
                        cbEnableReporting.Enabled = true;
                    }
                }

                SetButtonEventsVisibility(list, isMapped);
            }
        }

        private void SetButtonEventsVisibility(SPList list, bool isMapped)
        {
            var events = GetListEvents(
                list,
                "EPMLiveReportsAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b90e532f481cf050",
                "EPMLiveReportsAdmin.ListEvents",
                new List<SPEventReceiverType>
                {
                    SPEventReceiverType.ItemAdded,
                    SPEventReceiverType.ItemUpdated,
                    SPEventReceiverType.ItemDeleting
                });

            var hasEvents = events.Count > 0;
            btnAddEvt.Visible = isMapped && !hasEvents;
        }

        private Dictionary<string, int> GetAvailableDefaultTemps()
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            result.Add("None", -1);
            SPQuery getAllItemsQuery = new SPQuery();
            getAllItemsQuery.Query = "<Where><Eq><FieldRef Name='Active' /><Value Type='Bool'>True</Value></Eq></Where><OrderBy><FieldRef Name=\"Title0\" Ascending=\"True\" /></OrderBy>";
            SPList tmpGalList = SPContext.Current.Web.Lists.TryGetList("Template Gallery");
            if (tmpGalList != null)
            {
                SPListItemCollection templates = tmpGalList.GetItems(getAllItemsQuery);
                foreach (SPListItem template in templates)
                {
                    // the templatetype is not matching, skip this template
                    if (template[tmpGalList.Fields.GetFieldByInternalName("TemplateType").Id] != null &&
                        template[tmpGalList.Fields.GetFieldByInternalName("TemplateType").Id].ToString()
                            .Trim()
                            .Equals("workspace", StringComparison.CurrentCultureIgnoreCase))
                    {
                        result.Add(template["Description"].ToString(), template.ID);
                    }
                }
            }

            return result;
        }

        private Dictionary<string, string> GetAvailableParentSiteLookups(SPList list, GridGanttSettings gSettings)
        {
            var result = new Dictionary<string, string> { { "None", "None" } };
            string[] LookupArray = gSettings.Lookups.Split('|');
            foreach (string sLookup in LookupArray)
            {
                if (sLookup != "")
                {
                    string[] sLookupInfo = sLookup.Split('^');
                    string fldName = sLookupInfo[0];
                    SPField fld = null;
                    if (!string.IsNullOrEmpty(fldName))
                    {
                        try
                        {
                            fld = list.Fields.GetFieldByInternalName(fldName);
                        }
                        catch { }
                    }
                    if (fld != null)
                    {
                        result.Add(fldName, fldName);
                    }
                }
            }

            return result;
        }

        private List<SPEventReceiverDefinition> GetListEvents(SPList list, string assemblyName, string className, List<SPEventReceiverType> types)
        {
            List<SPEventReceiverDefinition> evts = new List<SPEventReceiverDefinition>();

            try
            {
                evts = (from e in list.EventReceivers.OfType<SPEventReceiverDefinition>()
                        where e.Assembly.Equals(assemblyName, StringComparison.CurrentCultureIgnoreCase) &&
                              e.Class.Equals(className, StringComparison.CurrentCultureIgnoreCase) &&
                              types.Contains(e.Type)
                        select e).ToList<SPEventReceiverDefinition>();
            }
            catch
            {

            }

            return evts;
        }

        private void AddGridGanttToViews(SPList list)
        {
            //string siteUrl = SPContext.Current.Site.Url;
            //SPWeb web = SPContext.Current.Web;
            //SPList list = web.Lists[new Guid(Request["List"])];

            foreach (SPView v in list.Views)
            {
                if (v.Type.Equals("HTML", StringComparison.CurrentCultureIgnoreCase))
                {
                    using (SPLimitedWebPartManager wpMgr = Web.GetLimitedWebPartManager(v.ServerRelativeUrl, PersonalizationScope.Shared))
                    {
                        bool wpAlreadyExists = false;

                        foreach (System.Web.UI.WebControls.WebParts.WebPart wp in wpMgr.WebParts)
                        {
                            //if (wp.GetType().Equals(typeof(GridListView)))
                            if (WebPartsHelper.WebPartsReflector.IsWebPartGridListView(wp))
                            {
                                wpAlreadyExists = true;
                                break;
                            }
                        }

                        if (!wpAlreadyExists)
                        {
                            var wpGridListView = WebPartsHelper.WebPartsReflector.CreateGridListViewWebPart();
                            //GridListView wpGridListView = new GridListView();
                            wpMgr.AddWebPart(wpGridListView, "Main", 0);
                            wpMgr.SaveChanges(wpGridListView);
                            Web.Update();
                        }
                    }
                }
            }

        }

        private void RemoveEventHandlers()
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[new Guid(Request["List"])];
            web.AllowUnsafeUpdates = true;
            string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
            string className = "EPMLiveCore.StatusingEvent";

            SPEventReceiverDefinitionCollection eventColl = list.EventReceivers;
            List<SPEventReceiverDefinition> listsToDelete = new List<SPEventReceiverDefinition>();

            foreach (SPEventReceiverDefinition eventDef in eventColl)
            {
                if (eventDef.Assembly.Equals(assemblyName) &&
                    eventDef.Class.Equals(className))
                {
                    if (eventDef.Type.Equals(SPEventReceiverType.ItemAdding) ||
                        eventDef.Type.Equals(SPEventReceiverType.ItemUpdating) ||
                        eventDef.Type.Equals(SPEventReceiverType.ItemDeleted))
                    {
                        listsToDelete.Add(eventDef);
                    }
                }
            }

            if (listsToDelete.Count > 0)
            {
                foreach (SPEventReceiverDefinition eventDel in listsToDelete)
                {
                    eventDel.Delete();
                }
            }

            list.Update();

        }

        private void AddFieldsAndFixLookupFields(SPList list)
        {
            var siteId = list.ParentWeb.Site.ID;
            var webId = list.ParentWeb.ID;
            var listId = list.ID;

            var spUserToken = list.ParentWeb.CurrentUser.UserToken;

            Task.Factory.StartNew(() =>
            {
                using (var spSite = new SPSite(siteId, spUserToken))
                {
                    using (var spWeb = spSite.OpenWeb(webId))
                    {
                        string fieldName = string.Empty;

                        try
                        {
                            list = spWeb.Lists.GetList(listId, true);

                            // add startdate field
                            if (!FieldExistsInList(list, "StartDate"))
                            {
                                fieldName = "StartDate";

                                string fldSDIntName = list.Fields.Add("StartDate", SPFieldType.DateTime, false);
                                SPFieldDateTime fldSD =
                                    list.Fields.GetFieldByInternalName(fldSDIntName) as SPFieldDateTime;
                                fldSD.Title = "Start Date";
                                fldSD.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                                fldSD.Sealed = false;
                                fldSD.AllowDeletion = false;
                                fldSD.Description = "Enter the estimated start date for this item.";
                                fldSD.Update();
                            }

                            // add DueDate field
                            if (!FieldExistsInList(list, "DueDate"))
                            {
                                fieldName = "DueDate";

                                string fldDDIntName = list.Fields.Add("DueDate", SPFieldType.DateTime, false);
                                SPFieldDateTime fldDD =
                                    list.Fields.GetFieldByInternalName(fldDDIntName) as SPFieldDateTime;
                                fldDD.Title = "Due Date";
                                fldDD.DisplayFormat = SPDateTimeFieldFormatType.DateOnly;
                                fldDD.Sealed = false;
                                fldDD.AllowDeletion = false;
                                fldDD.Description = "Enter the estimated due date for this item.";
                                fldDD.Update();
                            }

                            // add AssignedTo field
                            if (!FieldExistsInList(list, "AssignedTo"))
                            {
                                fieldName = "AssignedTo";

                                string fldATIntName = list.Fields.Add("AssignedTo", SPFieldType.User, false);
                                SPFieldUser fldAT = list.Fields.GetFieldByInternalName(fldATIntName) as SPFieldUser;
                                fldAT.Title = "Assigned To";
                                fldAT.AllowMultipleValues = true;
                                fldAT.Sealed = false;
                                fldAT.AllowDeletion = false;
                                fldAT.Description =
                                    "Enter the resource(s) assigned to this item.  You can enter their email address, username, or use the address book.";
                                fldAT.Update();
                            }

                            // add Priority field
                            if (!FieldExistsInList(list, "Priority"))
                            {
                                fieldName = "Priority";

                                StringCollection choices = new StringCollection();
                                choices.AddRange(new string[] { "(1) High", "(2) Normal", "(3) Low" });
                                string fldPIntName = list.Fields.Add("Priority", SPFieldType.Choice, false, false,
                                    choices);
                                SPFieldChoice fldP = list.Fields.GetFieldByInternalName(fldPIntName) as SPFieldChoice;
                                fldP.Title = "Priority";
                                fldP.DefaultValue = "(1) High";
                                fldP.Sealed = false;
                                fldP.AllowDeletion = false;
                                fldP.Update();
                            }

                            // add status field
                            if (!FieldExistsInList(list, "Status"))
                            {
                                fieldName = "Status";

                                StringCollection choices = new StringCollection();
                                choices.AddRange(new string[] { "Not Started", "In Progress", "Completed", "Deferred", "Waiting on someone else" });
                                string fldStatusIntName = list.Fields.Add("Status", SPFieldType.Choice, false, false,
                                    choices);
                                SPFieldChoice fldStatus =
                                    list.Fields.GetFieldByInternalName(fldStatusIntName) as SPFieldChoice;
                                fldStatus.DefaultValue = "Not Started";
                                fldStatus.Sealed = true;
                                fldStatus.AllowDeletion = false;
                                fldStatus.ReadOnlyField = true;
                                fldStatus.Update();
                            }

                            // add Work field
                            if (!FieldExistsInList(list, "Work"))
                            {
                                fieldName = "Work";

                                string fldWIntName = list.Fields.Add("Work", SPFieldType.Number, false);
                                SPFieldNumber fldW = list.Fields.GetFieldByInternalName(fldWIntName) as SPFieldNumber;
                                fldW.Title = "Work";
                                fldW.DisplayFormat = SPNumberFormatTypes.TwoDecimals;
                                fldW.Description = "Enter the estimated work (in hours) for this item.";
                                fldW.Update();
                            }

                            // add percent complete field
                            if (!FieldExistsInList(list, "PercentComplete"))
                            {
                                fieldName = "PercentComplete";

                                string fldPCIntName = list.Fields.Add("PercentComplete", SPFieldType.Number, false);
                                SPFieldNumber fldPC = list.Fields.GetFieldByInternalName(fldPCIntName) as SPFieldNumber;
                                fldPC.Title = "% Complete";
                                fldPC.ShowAsPercentage = true;
                                fldPC.DisplayFormat = SPNumberFormatTypes.NoDecimal;
                                fldPC.MinimumValue = 0;
                                fldPC.MaximumValue = 1;
                                fldPC.Sealed = false;
                                fldPC.AllowDeletion = false;
                                fldPC.Update();
                            }

                            // add body field
                            if (!FieldExistsInList(list, "Body"))
                            {
                                fieldName = "Body";

                                string fldBIntName = list.Fields.Add("Body", SPFieldType.Note, false);
                                SPFieldMultiLineText fldB =
                                    list.Fields.GetFieldByInternalName(fldBIntName) as SPFieldMultiLineText;
                                fldB.Title = "Description";
                                fldB.RichText = true;
                                fldB.Sealed = false;
                                fldB.AllowDeletion = false;
                                fldB.Update();
                            }

                            // add commentcount field
                            if (!FieldExistsInList(list, "CommentCount"))
                            {
                                fieldName = "CommentCount";

                                string fldCCIntName = list.Fields.Add("CommentCount", SPFieldType.Number, false);
                                SPFieldNumber fldCC = list.Fields.GetFieldByInternalName(fldCCIntName) as SPFieldNumber;
                                fldCC.MinimumValue = 0;
                                fldCC.Sealed = false;
                                fldCC.AllowDeletion = false;
                                fldCC.Hidden = true;
                                fldCC.Update();
                            }

                            // add complete field
                            if (!FieldExistsInList(list, "Complete"))
                            {
                                fieldName = "Complete";

                                string fldCompleteIntName = list.Fields.Add("Complete", SPFieldType.Boolean, false);
                                SPFieldBoolean fldComplete =
                                    list.Fields.GetFieldByInternalName(fldCompleteIntName) as SPFieldBoolean;
                                fldComplete.Sealed = false;
                                fldComplete.ShowInEditForm = true;
                                fldComplete.ShowInNewForm = false;
                                fldComplete.AllowDeletion = false;
                                fldComplete.DefaultValue = "0";
                                fldComplete.Update();
                            }

                            // add DaysOverdue field
                            if (!FieldExistsInList(list, "DaysOverdue"))
                            {
                                fieldName = "DaysOverdue";

                                string fldDOIntName = list.Fields.Add("DaysOverdue", SPFieldType.Calculated, false);
                                SPFieldCalculated fldDO =
                                    list.Fields.GetFieldByInternalName(fldDOIntName) as SPFieldCalculated;
                                fldDO.OutputType = SPFieldType.Number;
                                fldDO.Title = "Days Overdue";
                                fldDO.Sealed = false;
                                fldDO.ShowInEditForm = false;
                                fldDO.AllowDeletion = false;
                                fldDO.Formula =
                                    "=IF([Due Date]<>\"\",IF(Status<>\"Completed\",IF([Due Date]<Today,Today-[Due Date],0),0),0)";
                                fldDO.Update();
                            }

                            // add ScheduleStatus field
                            if (!FieldExistsInList(list, "ScheduleStatus"))
                            {
                                fieldName = "ScheduleStatus";

                                string fldSSIntName = list.Fields.Add("ScheduleStatus", SPFieldType.Calculated, false);
                                SPFieldCalculated fldSS =
                                    list.Fields.GetFieldByInternalName(fldSSIntName) as SPFieldCalculated;
                                fldSS.Title = "Schedule Status";
                                fldSS.OutputType = SPFieldType.Text;
                                fldSS.Sealed = false;
                                fldSS.ShowInEditForm = false;
                                fldSS.ShowInNewForm = false;
                                fldSS.AllowDeletion = false;
                                fldSS.Formula =
                                    "=IF(Status=\"Completed\",\"checkmark.GIF\",IF([Days Overdue]>=30,\"RED.GIF\",IF([Days Overdue]>0,\"YELLOW.GIF\",\"GREEN.GIF\")))";
                                fldSS.Update();
                            }

                            // add TodayYear field
                            if (!FieldExistsInList(list, "TodayYear"))
                            {
                                fieldName = "TodayYear";

                                string fldTYIntName = list.Fields.Add("TodayYear", SPFieldType.Calculated, false);
                                SPFieldCalculated fldTY =
                                    list.Fields.GetFieldByInternalName(fldTYIntName) as SPFieldCalculated;
                                fldTY.Title = "Today Year";
                                fldTY.OutputType = SPFieldType.Number;
                                fldTY.Sealed = false;
                                fldTY.ShowInEditForm = false;
                                fldTY.ShowInNewForm = false;
                                fldTY.ShowInDisplayForm = false;
                                fldTY.AllowDeletion = false;
                                fldTY.Formula = "=YEAR(Today)";
                                fldTY.Update();
                            }

                            // add TodayWeek field
                            if (!FieldExistsInList(list, "TodayWeek"))
                            {
                                fieldName = "TodayWeek";

                                string fldTWIntName = list.Fields.Add("TodayWeek", SPFieldType.Calculated, false);
                                SPFieldCalculated fldTW =
                                    list.Fields.GetFieldByInternalName(fldTWIntName) as SPFieldCalculated;
                                fldTW.Title = "Today Week";
                                fldTW.OutputType = SPFieldType.Number;
                                fldTW.Sealed = false;
                                fldTW.ShowInEditForm = false;
                                fldTW.ShowInNewForm = false;
                                fldTW.ShowInDisplayForm = false;
                                fldTW.AllowDeletion = false;
                                fldTW.Formula =
                                    "=IF(Today<>\"\",INT((Today-DATE(YEAR(Today),1,1)+(TEXT(WEEKDAY(DATE(YEAR(Today),1,1)),\"d\")))/7)+1,0)";
                                fldTW.Update();
                            }

                            // add Year field
                            if (!FieldExistsInList(list, "Year"))
                            {
                                fieldName = "Year";

                                string fldYIntName = list.Fields.Add("Year", SPFieldType.Calculated, false);
                                SPFieldCalculated fldY =
                                    list.Fields.GetFieldByInternalName(fldYIntName) as SPFieldCalculated;
                                fldY.Title = "Year";
                                fldY.OutputType = SPFieldType.Number;
                                fldY.Sealed = false;
                                fldY.ShowInEditForm = false;
                                fldY.ShowInNewForm = false;
                                fldY.ShowInDisplayForm = false;
                                fldY.AllowDeletion = false;
                                fldY.Formula = "=IF([Due Date]<>\"\",YEAR([Due Date]))";
                                fldY.Update();
                            }

                            // add Week field
                            if (!FieldExistsInList(list, "Week"))
                            {
                                fieldName = "Week";

                                string fldWIntName = list.Fields.Add("Week", SPFieldType.Calculated, false);
                                SPFieldCalculated fldW =
                                    list.Fields.GetFieldByInternalName(fldWIntName) as SPFieldCalculated;
                                fldW.Title = "Week";
                                fldW.OutputType = SPFieldType.Number;
                                fldW.Sealed = false;
                                fldW.ShowInEditForm = false;
                                fldW.ShowInNewForm = false;
                                fldW.ShowInDisplayForm = false;
                                fldW.AllowDeletion = false;
                                fldW.Formula =
                                    "=IF([Due Date]<>\"\",INT(([Due Date]-DATE(YEAR([Due Date]),1,1)+(TEXT(WEEKDAY(DATE(YEAR([Due Date]),1,1)),\"d\")))/7)+1,0)";
                                fldW.Update();
                            }

                            // add Due field
                            if (!FieldExistsInList(list, "Due"))
                            {
                                fieldName = "Due";

                                string fldDIntName = list.Fields.Add("Due", SPFieldType.Calculated, false);
                                SPFieldCalculated fldD =
                                    list.Fields.GetFieldByInternalName(fldDIntName) as SPFieldCalculated;
                                fldD.Title = "Due";
                                fldD.OutputType = SPFieldType.Text;
                                fldD.Sealed = false;
                                fldD.ShowInEditForm = false;
                                fldD.ShowInNewForm = false;
                                fldD.AllowDeletion = false;
                                fldD.Formula =
                                    "=IF([Status]=\"Completed\",\"NA\",IF([Due Date]=\"\",\"No Due Date\",IF([Due Date]<Today,\"(1) Overdue\",IF([Due Date]=Today,\"(2) Due Today\",IF([Due Date]=Today+1,\"(3) Due Tomorrow\",IF(Week=[Today Week],IF(Year=[Today Year],\"(4) Due This Week\",\"(7) Future\"),IF(Week=[Today Week]+1,IF(Year=[Today Year],\"(5) Due Next Week\",\"(7) Future\"),IF(MONTH([Due Date])=MONTH(Today),IF(Year=[Today Year],\"(6) Due This Month\",\"(7) Future\"),\"(7) Future\"))))))))";
                                fldD.Update();
                            }

                            // add Commenters field
                            if (!FieldExistsInList(list, "Commenters"))
                            {
                                fieldName = "Commenters";

                                string fldCIntName = list.Fields.Add("Commenters", SPFieldType.Note, false);
                                SPFieldMultiLineText fldC =
                                    list.Fields.GetFieldByInternalName(fldCIntName) as SPFieldMultiLineText;
                                fldC.Title = "Commenters";
                                fldC.Hidden = true;
                                fldC.Sealed = false;
                                fldC.AllowDeletion = false;
                                fldC.Update();
                            }

                            // add CommentersRead field
                            if (!FieldExistsInList(list, "CommentersRead"))
                            {
                                fieldName = "CommentersRead";

                                string fldCRIntName = list.Fields.Add("CommentersRead", SPFieldType.Note, false);
                                SPFieldMultiLineText fldCR =
                                    list.Fields.GetFieldByInternalName(fldCRIntName) as SPFieldMultiLineText;
                                fldCR.Title = "Commenters Read";
                                fldCR.Hidden = true;
                                fldCR.Sealed = false;
                                fldCR.AllowDeletion = false;
                                fldCR.Update();
                            }

                            // remove Today field
                            if (FieldExistsInList(list, "Today"))
                            {
                                try
                                {
                                    list.Fields.Delete("Today");
                                }
                                catch { }
                            }

                            list.Update();

                            FixLookupFields(list);
                        }
                        catch (Exception exception)
                        {
                            var logger = new ReportingLogger(list.ParentWeb);
                            logger.Log(list,
                                "Problem adding one or more field or fixing a lookup field when enabling work features for the list.",
                                string.Format("Field: {0}, Exception: {1}", fieldName, exception.Message), 2, 1,
                                string.Empty);
                        }
                    }
                }
            });
        }

        private void AddEventHandlers()
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[new Guid(Request["List"])];
            web.AllowUnsafeUpdates = true;
            string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
            string className = "EPMLiveCore.StatusingEvent";

            SPEventReceiverDefinitionCollection eventColl = list.EventReceivers;
            List<SPEventReceiverDefinition> listsToDelete = GetListEvents(list, assemblyName, className, new List<SPEventReceiverType> { SPEventReceiverType.ItemAdding, SPEventReceiverType.ItemUpdating, SPEventReceiverType.ItemDeleted });

            if (listsToDelete.Count > 0)
            {
                foreach (SPEventReceiverDefinition eventDel in listsToDelete)
                {
                    eventDel.Delete();
                }
            }

            list.EventReceivers.Add(SPEventReceiverType.ItemAdding, assemblyName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemUpdating, assemblyName, className);
            list.EventReceivers.Add(SPEventReceiverType.ItemDeleted, assemblyName, className);

            List<SPEventReceiverDefinition> newEvts = GetListEvents(list, assemblyName, className, new List<SPEventReceiverType> { SPEventReceiverType.ItemAdding, SPEventReceiverType.ItemUpdating, SPEventReceiverType.ItemDeleted });
            foreach (SPEventReceiverDefinition evt in newEvts)
            {
                evt.SequenceNumber = 11000;
                evt.Update();
            }

            list.Update();
        }

        private void FixLookupFields(SPList list)
        {
            var web = list.ParentWeb;
            web.AllowUnsafeUpdates = true;

            for (int i = 0; i < list.Fields.Count; i++)
            {
                if (!(list.Fields[i] is SPFieldLookup))
                {
                    continue;
                }

                bool originalListExists = false;
                bool listEqualToInternalNameExists = false;
                SPList tempLookupList = null;

                try
                {
                    string lookupListGuidString = (list.Fields[i] as SPFieldLookup).LookupList;
                    if (!string.IsNullOrEmpty(lookupListGuidString))
                    {
                        tempLookupList = web.Lists[new Guid(lookupListGuidString)];
                    }
                }
                catch
                { }

                originalListExists = (tempLookupList != null);

                // if can't find list by guid, find it by field internal name
                if (!originalListExists)
                {
                    tempLookupList = web.Lists.TryGetList(list.Fields[i].InternalName);
                    listEqualToInternalNameExists = (tempLookupList != null);
                    if (!listEqualToInternalNameExists && list.Fields[i].InternalName.Equals("Project", StringComparison.CurrentCultureIgnoreCase))
                    {
                        tempLookupList = web.Lists.TryGetList("Project Center");
                    }

                    if (tempLookupList != null)
                    {
                        UpdateLookupReferences((list.Fields[i] as SPFieldLookup), web, tempLookupList);
                    }
                }

            }

        }

        private void EnableWorkengineListFeatures(SPList list)
        {
            AddGridGanttToViews(list);
            AddEventHandlers();
            AddRemoveMyWorkReportingEvents("ADD");
            AddFieldsAndFixLookupFields(list);
        }

        private void AddRemoveMyWorkReportingEvents(string operation)
        {
            var dataElement = new XElement("Data");

            const string assembly =
                "EPMLiveReportsAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b90e532f481cf050";

            const string className = "EPMLiveReportsAdmin.MyWorkListEvents";

            var spEventReceiverTypes = new[]
                                               {
                                                   SPEventReceiverType.ItemAdded,
                                                   SPEventReceiverType.ItemUpdated,
                                                   SPEventReceiverType.ItemDeleting
                                               };

            AddEventReceiverElement(operation, className, assembly, spEventReceiverTypes, new Guid(Request["List"]),
                                    ref dataElement);

            WorkEngineAPI.EventReceiverManager(new XElement("EventReceiverManager", dataElement).ToString(), Web);
        }

        private void AddEventReceiverElement(string operation, string className, string assembly,
                                             IEnumerable<SPEventReceiverType> spEventReceiverTypes, Guid listId,
                                             ref XElement dataElement)
        {
            foreach (SPEventReceiverType spEventReceiverType in spEventReceiverTypes)
            {
                dataElement.Add(new XElement("EventReceiver", new XAttribute("SiteId", Web.Site.ID),
                                             new XAttribute("WebId", Web.ID), new XAttribute("ListId", listId),
                                             new XAttribute("Type", (int)spEventReceiverType),
                                             new XAttribute("Operation", operation),
                                             new XAttribute("Assembly", assembly),
                                             new XAttribute("ClassName", className),
                                             new XAttribute("DataId", listId)));
            }
        }

        private void RemoveWorkengineListFeatures()
        {
            RemoveEventHandlers();
            AddRemoveMyWorkReportingEvents("REMOVE");
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[new Guid(Request["List"])];
            var gSettings = new GridGanttSettings(list);

            gSettings.StartDate = ddlStartDate.SelectedValue;
            gSettings.DueDate = ddlDueDate.SelectedValue;
            gSettings.Progress = ddlProgressBar.SelectedValue;
            gSettings.WBS = ddlWBS.SelectedValue;
            gSettings.Milestone = ddlMilestone.SelectedValue;
            gSettings.Executive = chkExecutive.Checked;
            gSettings.Information = ddlInformation.SelectedValue;
            gSettings.ItemLink = ddlItemLink.SelectedValue;
            gSettings.RibbonBehavior = ddlRibbonBehavior.SelectedValue;
            gSettings.RollupLists = txtRollupLists.Text.Replace(",", "|").Replace("\r\n", ",");
            gSettings.RollupSites = txtRollupSites.Text.Replace("\r\n", ",");
            gSettings.ShowViewToolbar = chkShowViewToolbar.Checked;
            gSettings.HideNewButton = chkHideNewButton.Checked;
            gSettings.Performance = chkPerformance.Checked;
            gSettings.AllowEdit = chkAllowEdit.Checked;
            gSettings.EditDefault = chkEditDefault.Checked;
            gSettings.ShowInsert = chkShowInsert.Checked;
            gSettings.DisableNewItemMod = chkDisableNewRollup.Checked;
            gSettings.UseNewMenu = chkUseNewMenu.Checked;
            gSettings.NewMenuName = txtNewMenuName.Text;
            gSettings.UsePopup = chkUsePopup.Checked;
            gSettings.EnableRequests = chkEnableRequests.Checked;
            gSettings.EnableAutoCreation = chkAutoCreate.Checked;
            gSettings.AutoCreationTemplateId = ddlAutoCreateTemplate.SelectedValue;
            gSettings.WorkspaceParentSiteLookup = ddlParentSiteLookup.SelectedValue;
            gSettings.ListIcon = hdnListIcon.Value;
            gSettings.EnableWorkList = chkWorkListFeat.Checked;
            gSettings.EnableFancyForms = chkFancyForms.Checked;
            gSettings.SendEmails = chkEmails.Checked;
            gSettings.DeleteRequest = chkDeleteRequest.Checked;
            gSettings.RequestList = txtRequestList.Text;
            gSettings.UseParent = chkUseParent.Checked;
            gSettings.Search = chkSearch.Checked;
            gSettings.LockSearch = chkLockSearch.Checked;
            gSettings.AssociatedItems = chkAssociatedItems.Checked;
            gSettings.DisplayFormRedirect = chkDisplayRedirect.Checked;
            gSettings.EnableResourcePlan = chkResTools.Checked;
            gSettings.BuildTeam = chkEnableTeam.Checked;
            gSettings.BuildTeamSecurity = chkEnableTeamSecurity.Checked;
            gSettings.BuildTeamPermissions = GetGroupsPermissionsAssignment();
            gSettings.EnableContentReporting = chkContentReporting.Checked;
            gSettings.DisableThumbnails = chkDisableThumbnails.Checked;
            gSettings.SaveSettings(list);

            //if ((uint)list.BaseTemplate == 10115 || (uint)list.BaseTemplate == 10701 || (uint)list.BaseTemplate == 10702)
            {
                if (chkEmails.Checked)
                    API.APIEmail.InstallAssignedToEvent(list);
                else
                    API.APIEmail.UnInstallAssignedToEvent(list);
                //list.EnableAssignToEmail = chkEmails.Checked;
                //list.Update();
                //CoreFunctions.setListSetting(list, "AssignedToEmail", chkEmails.Checked.ToString());
            }

            if (gSettings.BuildTeamSecurity)
            {
                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemSecurityEventReceiver";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] {
                        SPEventReceiverType.ItemAdded,
                        SPEventReceiverType.ItemUpdated,
                        SPEventReceiverType.ItemDeleting
                    });

                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                list.EventReceivers.Add(SPEventReceiverType.ItemAdded, assemblyName, className);
                list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, assemblyName, className);
                list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, assemblyName, className);

                var newEvts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] {
                        SPEventReceiverType.ItemAdded,
                        SPEventReceiverType.ItemUpdated,
                        SPEventReceiverType.ItemDeleting
                    });

                foreach (SPEventReceiverDefinition evt in newEvts)
                {
                    evt.SequenceNumber = 11000;
                    evt.Update();
                }

                list.Update();
            }
            else
            {
                bool hasSecFld = false;
                string lookups = gSettings.Lookups;
                if (!string.IsNullOrEmpty(lookups))
                {
                    string[] settings = lookups.Split('|');
                    foreach (string s in settings)
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            string[] vals = s.Split('^');

                            bool isSec = false;
                            try
                            {
                                isSec = bool.Parse(vals[4]);
                            }
                            catch { }
                            if (isSec)
                            {
                                hasSecFld = true;
                                break;
                            }
                        }
                    }
                }

                if (!hasSecFld)
                {
                    string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                    string className = "EPMLiveCore.ItemSecurityEventReceiver";

                    var evts = CoreFunctions.GetListEvents(
                        list,
                        assemblyName,
                        className,
                        new [] {
                            SPEventReceiverType.ItemAdded,
                            SPEventReceiverType.ItemUpdated,
                            SPEventReceiverType.ItemDeleting
                        });

                    foreach (SPEventReceiverDefinition evt in evts)
                    {
                        evt.Delete();
                    }
                    list.Update();
                }
            }



            if (chkEnableTeam.Checked && !chkEnableTeamSecurity.Checked)
            {
                API.ListCommands.EnableTeamFeatures(list);

                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemEnableTeamEvent";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] { SPEventReceiverType.ItemAdded }
                );
                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                SPEventReceiverDefinition evtAdded = list.EventReceivers.Add();
                evtAdded.Type = SPEventReceiverType.ItemAdded;
                evtAdded.Assembly = assemblyName;
                evtAdded.Class = className;
                evtAdded.SequenceNumber = 11000;
                evtAdded.Update();
                list.Update();
            }
            else if (chkEnableTeam.Checked || chkEnableTeamSecurity.Checked && list.BaseTemplate == SPListTemplateType.DocumentLibrary)
            {
                //EPML-4257 : In  document library if you have enable team and enable team security on the library will not load
                API.ListCommands.EnableTeamFeatures(list);

                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemEnableTeamEvent";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] { SPEventReceiverType.ItemAdded }
                );
                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                SPEventReceiverDefinition evtAdded = list.EventReceivers.Add();
                evtAdded.Type = SPEventReceiverType.ItemAdded;
                evtAdded.Assembly = assemblyName;
                evtAdded.Class = className;
                evtAdded.SequenceNumber = 11000;
                evtAdded.Update();
                list.Update();
            }
            else
            {
                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemEnableTeamEvent";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] { SPEventReceiverType.ItemAdded }
                );
                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                list.Update();
            }

            // attached workspace auto creation event
            if (chkAutoCreate.Checked)
            {
                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemWorkspaceEventReceiver";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new [] { SPEventReceiverType.ItemAdded }
                );
                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                SPEventReceiverDefinition evtAdded = list.EventReceivers.Add();
                evtAdded.Type = SPEventReceiverType.ItemAdded;
                evtAdded.Assembly = assemblyName;
                evtAdded.Class = className;
                evtAdded.Update();
                list.Update();
            }
            else
            {
                string assemblyName = "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                string className = "EPMLiveCore.ItemWorkspaceEventReceiver";

                var evts = CoreFunctions.GetListEvents(
                    list,
                    assemblyName,
                    className,
                    new List<SPEventReceiverType> { SPEventReceiverType.ItemAdded }
                );
                foreach (SPEventReceiverDefinition evt in evts)
                {
                    evt.Delete();
                }

                list.Update();
            }

            if (chkTimesheet.Checked)
                API.ListCommands.EnableTimesheets(list, web);
            else
                API.ListCommands.DisableTimesheets(list, web);

            if (chkFancyForms.Checked)
                EnableFancyForms(list);
            else
                DisableFancyForms(list);

            if (chkAssociatedItems.Checked)
            {
                AddGridGanttToViews(list);
            }

            if (chkWorkListFeat.Checked)
            {
                EnableWorkengineListFeatures(list);
            }
            else
            {
                RemoveWorkengineListFeatures();
            }

            if (ifsEnableReporting.Visible && cbEnableReporting.Checked)
            {
                ReportBiz reportBiz = new ReportBiz(SPContext.Current.Site.ID);
                if (reportBiz.SiteExists())
                {
                    Collection<string> mappedLists = reportBiz.GetMappedListsIds();
                    if (!mappedLists.Contains(list.ID.ToString().ToLower()))
                    {
                        var fields = new ListItemCollection();
                        fields = GetListFields(list);
                        reportBiz.CreateListBiz(list.ID, SPContext.Current.Web.ID, fields);

                        //reportBiz.CreateListBiz(list.ID);

                        try
                        {
                            //FOREIGN IMPLEMENTATION -- START
                            EPMData DAO = new EPMData(SPContext.Current.Site.ID);
                            reportBiz.UpdateForeignKeys(DAO);
                            DAO.Dispose();
                            // -- END
                        }
                        catch (Exception ex)
                        {
                            SPSecurity.RunWithElevatedPrivileges(delegate ()
                            {
                                if (!EventLog.SourceExists("EPMLive Reporting - UpdateForeignKeys"))
                                    EventLog.CreateEventSource("EPMLive Reporting - UpdateForeignKeys", "EPM Live");

                                using (var myLog = new EventLog("EPM Live", ".", "EPMLive Reporting - UpdateForeignKeys"))
                                {
                                    const int MaximumKilobytes = 32768;
                                    myLog.MaximumKilobytes = MaximumKilobytes;
                                    const int EventId = 4001;
                                    myLog.WriteEntry(ex.Message + ex.StackTrace, EventLogEntryType.Error, EventId);
                                }
                            });
                        }
                    }
                }
            }
            else if (ifsEnableReporting.Visible && !cbEnableReporting.Checked)
            {

                var reportBiz = new ReportBiz(SPContext.Current.Site.ID);
                Guid listId = new Guid(Request["List"]);
                EPMData _DAO = new EPMData(SPContext.Current.Site.ID);
                _DAO.Command = "SELECT TableName FROM RPTList WHERE RPTListID=@RPTListID";
                _DAO.AddParam("@RPTListID", Request["List"]);
                string sTableName = string.Empty;
                try
                {
                    sTableName = _DAO.ExecuteScalar(_DAO.GetClientReportingConnection).ToString();
                }
                catch { }
                DataTable refTables = new DataTable();

                if (!string.IsNullOrEmpty(sTableName))
                {
                    refTables = reportBiz.GetReferencingTables(_DAO, sTableName);
                    if (refTables.Rows.Count == 0)
                    {
                        reportBiz.GetListBiz(new Guid(Request["List"])).Delete();
                    }
                    else
                    {
                        //SPUtility.Redirect("epmlive/ListMappings.aspx?delete=true&id=" + param + "&name=" + sTableName, SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);
                        ShowAlert();
                    }
                }
            }

            SetListIcon(gSettings.ListIcon);

            Infrastructure.CacheStore.Current.RemoveCategory("GridSettings-" + list.ID);

            Microsoft.SharePoint.Utilities.SPUtility.Redirect("listedit.aspx?List=" + Request["List"], Microsoft.SharePoint.Utilities.SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);
        }

        private void SetListIcon(string icon)
        {
            try
            {
                var reportBiz = new ReportBiz(SPContext.Current.Site.ID);
                Guid listId = new Guid(Request["List"]);
                EPMData _DAO = new EPMData(SPContext.Current.Site.ID);
                _DAO.Command = "SELECT COUNT(Id) FROM ReportListIds WHERE Id = @ListID";
                _DAO.AddParam("@ListID", Convert.ToString(Request["List"]));
                object rowsAffected = _DAO.ExecuteScalar(_DAO.GetClientReportingConnection).ToString();

                if (Convert.ToInt32(rowsAffected) == 0)
                    _DAO.Command = "INSERT INTO ReportListIds(Id, ListIcon) VALUES (@ID, @ListIcon)";
                else
                    _DAO.Command = "UPDATE ReportListIds SET ListIcon = @ListIcon WHERE Id = @ID";

                _DAO.AddParam("@ID", Convert.ToString(Request["List"]));
                _DAO.AddParam("@ListIcon", icon);

                rowsAffected = _DAO.ExecuteNonQuery(_DAO.GetClientReportingConnection).ToString();
            }
            catch { }
        }

        private ListItemCollection GetListFields(SPList spList)
        {
            var fields = new ListItemCollection();
            ListItem listField;
            var ListDefaults = new ListBiz(spList.ID, spList.ParentWeb.Site.ID);

            foreach (SPField field in spList.Fields)
            {
                try
                {
                    //Adding contenttype field specifically.
                    if (field.InternalName.ToLower() == "contenttype")
                    {
                        listField = new ListItem();
                        listField.Text = field.Title;
                        listField.Value = field.Id.ToString();
                        fields.Add(listField);
                        continue;
                    }

                    //Adding contenttype field specifically.
                    if (field.InternalName.ToLower() == "extid")
                    {
                        listField = new ListItem();
                        listField.Text = field.Title;
                        listField.Value = field.Id.ToString();
                        fields.Add(listField);
                        continue;
                    }

                    if (!field.Hidden
                        && field.Type != SPFieldType.Computed
                        )
                    {
                        listField = new ListItem();
                        listField.Text = field.Title;
                        listField.Value = field.Id.ToString();
                        fields.Add(listField);
                    }
                }
                catch (Exception ex) { }
            }

            return fields;
        }

        private void DisableFancyForms(SPList list)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SPWeb web = SPContext.Current.Web;

                    web.AllowUnsafeUpdates = true;

                    bool isFancyFormWPExist = false;
                    var rootFolder = list.RootFolder;

                    var dispFormUrl = string.Format("{0}/{1}/DispForm.aspx", web.ServerRelativeUrl, rootFolder.Url);
                    var dispForm = web.GetFile(dispFormUrl);

                    if (dispForm != null)
                    {
                        //Delete webparts from existing list
                        using (var wpm = dispForm.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared))
                        {
                            try
                            {
                                if (wpm.WebParts != null)
                                {
                                    for (int i = 0; i < wpm.WebParts.Count; i++)
                                    {
                                        try
                                        {
                                            if ((!string.IsNullOrEmpty(wpm.WebParts[i].Title)) && (wpm.WebParts[i].Title.Equals("Fancy Display Form", StringComparison.InvariantCultureIgnoreCase)))
                                            {
                                                isFancyFormWPExist = true;
                                                //wpm.CloseWebPart(wpm.WebParts[i]);
                                                //wpm.SaveChanges(wpm.WebParts[i]);
                                                wpm.WebParts[i].AllowHide = true;
                                                wpm.WebParts[i].Hidden = true;
                                                //wpm.WebParts[i].Visible = false;
                                                wpm.SaveChanges(wpm.WebParts[i]);
                                            }
                                            else if (wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart" ||
                                                wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" ||
                                                wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.ListViewWebPart" ||
                                                wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.DataFormWebPart")
                                            {
                                                //wpm.OpenWebPart(wpm.WebParts[i]);
                                                //wpm.SaveChanges(wpm.WebParts[i]);
                                                wpm.WebParts[i].AllowHide = false;
                                                wpm.WebParts[i].Hidden = false;
                                                //wpm.WebParts[i].Visible = true;
                                                wpm.SaveChanges(wpm.WebParts[i]);
                                            }
                                        }
                                        catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                                    }
                                }
                            }
                            catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                            finally { if (wpm != null) wpm.Web.Dispose(); }

                        }
                    }
                    else
                    {
                        /* create a new NewForm */
                        dispForm = rootFolder.Files.Add(dispFormUrl, SPTemplateFileType.FormPage);
                    }

                    if (!isFancyFormWPExist)
                    {
                        using (var wpm = dispForm.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared))
                        {
                            try
                            {
                                //EPMLiveWebParts.FancyDisplayForm fancyDispFormWebPart = new EPMLiveWebParts.FancyDisplayForm();
                                var fancyDispFormWebPart = WebPartsHelper.WebPartsReflector.CreateFancyDisplayFormWebPart();
                                fancyDispFormWebPart.Title = "Fancy Display Form";
                                fancyDispFormWebPart.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
                                fancyDispFormWebPart.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
                                fancyDispFormWebPart.AllowHide = true;
                                fancyDispFormWebPart.Hidden = true;
                                //wpm.WebParts[i].Visible = false;
                                wpm.AddWebPart(fancyDispFormWebPart, "Main", 0);
                                wpm.SaveChanges(fancyDispFormWebPart);
                            }
                            catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                            finally { if (wpm != null) wpm.Web.Dispose(); }

                        }
                    }

                });
            }
            catch { }
        }

        private void EnableFancyForms(SPList list)
        {
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate ()
                {
                    SPWeb web = SPContext.Current.Web;

                    web.AllowUnsafeUpdates = true;

                    bool isFancyFormWPExist = false;
                    var rootFolder = list.RootFolder;

                    var dispFormUrl = string.Format("{0}/{1}/DispForm.aspx", web.ServerRelativeUrl, rootFolder.Url);
                    var dispForm = web.GetFile(dispFormUrl);

                    if (dispForm != null)
                    {
                        //Delete webparts from existing list
                        using (var wpm = dispForm.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared))
                        {

                            if (wpm.WebParts != null)
                            {
                                try
                                {
                                    for (int i = 0; i < wpm.WebParts.Count; i++)
                                    {
                                        try
                                        {
                                            if ((!string.IsNullOrEmpty(wpm.WebParts[i].Title)) && (wpm.WebParts[i].Title.Equals("Fancy Display Form", StringComparison.InvariantCultureIgnoreCase)) && !isFancyFormWPExist)
                                            {
                                                isFancyFormWPExist = true;
                                                wpm.WebParts[i].AllowHide = false;
                                                wpm.WebParts[i].Hidden = false;
                                                //wpm.WebParts[i].Visible = true;
                                                wpm.SaveChanges(wpm.WebParts[i]);
                                                //wpm.OpenWebPart(wpm.WebParts[i]);
                                                //wpm.SaveChanges(wpm.WebParts[i]);
                                            }
                                            else if (wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.XsltListViewWebPart" ||
                                                    wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.ListFormWebPart" ||
                                                    wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.ListViewWebPart" ||
                                                    wpm.WebParts[i].ToString() == "Microsoft.SharePoint.WebPartPages.DataFormWebPart")
                                            {
                                                wpm.WebParts[i].AllowHide = true;
                                                wpm.WebParts[i].Hidden = true;
                                                //wpm.WebParts[i].Visible = false;
                                                wpm.SaveChanges(wpm.WebParts[i]);
                                                //wpm.CloseWebPart(wpm.WebParts[i]);
                                                //wpm.SaveChanges(wpm.WebParts[i]);
                                            }
                                        }
                                        catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                                    }
                                }
                                catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                                finally { if (wpm != null) wpm.Web.Dispose(); }

                            }
                        }

                    }
                    else
                    {
                        /* create a new NewForm */
                        dispForm = rootFolder.Files.Add(dispFormUrl, SPTemplateFileType.FormPage);
                    }

                    if (!isFancyFormWPExist)
                    {
                        using (var wpm = dispForm.GetLimitedWebPartManager(System.Web.UI.WebControls.WebParts.PersonalizationScope.Shared))
                        {
                            try
                            {
                                //EPMLiveWebParts.FancyDisplayForm fancyDispFormWebPart = new EPMLiveWebParts.FancyDisplayForm();
                                var fancyDispFormWebPart = WebPartsHelper.WebPartsReflector.CreateFancyDisplayFormWebPart();
                                fancyDispFormWebPart.Title = "Fancy Display Form";
                                fancyDispFormWebPart.ChromeState = System.Web.UI.WebControls.WebParts.PartChromeState.Normal;
                                fancyDispFormWebPart.ChromeType = System.Web.UI.WebControls.WebParts.PartChromeType.None;
                                fancyDispFormWebPart.AllowHide = false;
                                fancyDispFormWebPart.Hidden = false;
                                fancyDispFormWebPart.Visible = true;

                                wpm.AddWebPart(fancyDispFormWebPart, "Main", 0);
                            }
                            catch (Exception ex) { LoggingService.WriteTrace(Area.EPMLiveCore, Categories.EPMLiveCore.Event, TraceSeverity.Medium, ex.ToString()); }
                            finally { if (wpm != null) wpm.Web.Dispose(); }
                        }
                    }

                });
            }
            catch { }
        }

        protected void AddReportEvent(object sender, EventArgs e)
        {
            SPWeb web = SPContext.Current.Web;
            SPList list = web.Lists[new Guid(Request["List"])];
            string sAssembly = "EPMLiveReportsAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b90e532f481cf050";
            string sClass = "EPMLiveReportsAdmin.ListEvents";
            // remove existing event receivers first
            List<SPEventReceiverDefinition> evts = GetListEvents(list,
                                                                 sAssembly,
                                                                 sClass,
                                                                 new List<SPEventReceiverType> { SPEventReceiverType.ItemAdded,
                                                                                                                             SPEventReceiverType.ItemUpdated,
                                                                                                                             SPEventReceiverType.ItemDeleting });

            foreach (SPEventReceiverDefinition ev in evts)
            {
                ev.Delete();
            }

            // then add event receivers
            list.EventReceivers.Add(SPEventReceiverType.ItemAdded, sAssembly, sClass);
            list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, sAssembly, sClass);
            list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, sAssembly, sClass);

            List<SPEventReceiverDefinition> newEvts = GetListEvents(list,
                                                                sAssembly,
                                                                sClass,
                                                                new List<SPEventReceiverType> { SPEventReceiverType.ItemAdded,
                                                                                                                             SPEventReceiverType.ItemUpdated,
                                                                                                                             SPEventReceiverType.ItemDeleting });
            foreach (SPEventReceiverDefinition ev in newEvts)
            {
                ev.SequenceNumber = 11000;
                ev.Update();
            }

            list.Update();
        }

        void ShowAlert()
        {
            var reportBiz = new ReportBiz(SPContext.Current.Site.ID);
            Guid listId = new Guid(Request.QueryString["List"]);
            EPMData _DAO = new EPMData(SPContext.Current.Site.ID);
            _DAO.Command = "SELECT TableName FROM RPTList WHERE RPTListID=@RPTListID";
            _DAO.AddParam("@RPTListID", listId);

            string sTableName = _DAO.ExecuteScalar(_DAO.GetClientReportingConnection).ToString();
            DataTable refTables = reportBiz.GetReferencingTables(_DAO, sTableName);
            string sLists = GetRefLists(refTables, _DAO);

            string sAlert = "<script language='javascript' type='text/javascript'> alert('Before this list can be deleted the following lists must be deleted first: " + sLists + "); </script>";
            ClientScript.RegisterClientScriptBlock(GetType(), "EPMLiveWarning", sAlert);
        }

        private string GetRefLists(DataTable refTables, EPMData dao)
        {
            string sRefLists = string.Empty;
            foreach (DataRow table in refTables.Rows)
            {
                if (!table["oObjName"].ToString().ToLower().Contains("snapshot"))
                {
                    sRefLists = sRefLists + dao.GetListName(table["oObjName"].ToString()) + ",";
                }
            }

            sRefLists = sRefLists.Remove(sRefLists.LastIndexOf(","));
            return sRefLists;
        }

        private void EnableSiteFeature()
        {

        }

        public void ClearLookupReferences(SPFieldLookup lookupField)
        {
            lookupField.SchemaXml = ReplaceXmlAttributeValue(lookupField.SchemaXml, "List", "");
            lookupField.SchemaXml = ReplaceXmlAttributeValue(lookupField.SchemaXml, "WebId", "");
            lookupField.Update(true);
        }

        public void UpdateLookupReferences(SPFieldLookup lookupField, SPWeb web, SPList list)
        {
            if (string.IsNullOrEmpty(lookupField.LookupList))
            {
                lookupField.LookupWebId = web.ID;
                lookupField.LookupList = list.ID.ToString();
            }
            else
            {
                lookupField.SchemaXml = ReplaceXmlAttributeValue(lookupField.SchemaXml, "List", list.ID.ToString());
                lookupField.SchemaXml = ReplaceXmlAttributeValue(lookupField.SchemaXml, "WebId", web.ID.ToString());
            }

            lookupField.Update(true);
        }

        public string ReplaceXmlAttributeValue(string xml, string attributeName, string value)
        {
            string retVal = string.Empty;
            if (string.IsNullOrEmpty(xml))
            {
                throw new ArgumentNullException("xml");
            }

            int indexOfAttributeName = xml.IndexOf(attributeName, StringComparison.CurrentCultureIgnoreCase);
            if (!indexOfAttributeName.Equals(-1))
            {
                int indexOfAttibuteValueBegin = xml.IndexOf('"', indexOfAttributeName);
                int indexOfAttributeValueEnd = xml.IndexOf('"', indexOfAttibuteValueBegin + 1);
                retVal = xml.Substring(0, indexOfAttibuteValueBegin + 1) + value + xml.Substring(indexOfAttributeValueEnd);
            }

            if (string.IsNullOrEmpty(retVal))
            {
                retVal = xml;
            }

            return retVal;
        }

        public bool FieldExistsInList(SPList list, string fldInternalName)
        {
            SPField testField = null;
            try
            {
                testField = list.Fields.GetFieldByInternalName(fldInternalName);
            }
            catch
            {
                testField = null;
            }
            return (testField != null);
        }
    }
}
