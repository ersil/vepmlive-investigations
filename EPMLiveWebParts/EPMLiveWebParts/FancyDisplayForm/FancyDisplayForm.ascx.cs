﻿using EPMLiveCore;
using EPMLiveCore.ReportingProxy;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls.WebParts;

namespace EPMLiveWebParts.FancyDisplayForm
{
    [ToolboxItemAttribute(false)]
    public partial class FancyDisplayForm : WebPart
    {
        #region Variables and Constants

        const int QUICK_DETAILS_SECTION_ONE_FIELD_COUNT = 7;
        const int QUICK_DETAILS_SECTION_TOTAL_FIELD_COUNT = 14;
        const int NARRATIVE_DETAILS_SECTION_FIELD_COUNT = 2;
        const int PEOPLE_DETAILS_SECTION_FIELD_COUNT = 5;
        const int DATE_DETAILS_SECTION_FIELD_COUNT = 2;
        const int NUMBER_OF_PEOPLE_PER_FIELD_DISPLAY = 3;

        int quickDetailsFieldsCount = 0;
        int narrativeDetailsFieldsCount = 0;
        int peopleDetailsFieldsCount = 0;
        int datesDetailsFieldsCount = 0;
        int associatedItemCount = 0;
        int itemId = 0;

        StringBuilder sbQuickDetailsContent = new StringBuilder();
        StringBuilder sbQuickDetailsShowAllRegion = new StringBuilder();

        StringBuilder sbNarrativeDetailsContent = new StringBuilder();
        StringBuilder sbNarrativeDetailsShowAllRegion = new StringBuilder();

        StringBuilder sbPeopleDetailsContent = new StringBuilder();
        StringBuilder sbPeopleDetailsShowAllRegion = new StringBuilder();

        StringBuilder sbDateDetailsContent = new StringBuilder();
        StringBuilder sbDateDetailsShowAllRegion = new StringBuilder();

        StringBuilder sbItemDetailsContent = new StringBuilder();
        StringBuilder sbItemDetailsShowAllRegion = new StringBuilder();

        StringBuilder sbFancyDispFormContent = new StringBuilder();

        #endregion

        #region Constructor

        public FancyDisplayForm()
        {

        }

        #endregion

        #region Web Part Events

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeControl();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ScriptLink.Register(Page, "/epmlive/javascripts/libraries/jquery.min.js", false);
        }

        protected override void CreateChildControls()
        {
            SPWeb oWeb = SPContext.Current.Web;

            string serviceUrl = ((SPContext.Current.Web.ServerRelativeUrl == "/")
                                     ? ""
                                     : SPContext.Current.Web.ServerRelativeUrl) + "/_vti_bin/Workengine.asmx";

            ScriptManager scriptManager = ScriptManager.GetCurrent(Page);

            if (scriptManager != null)
            {
                scriptManager.Services.Add(new ServiceReference(serviceUrl));
            }
            else
            {
                scriptManager = new ScriptManager();
                scriptManager.Services.Add(new ServiceReference(serviceUrl));

                Page.Form.Controls.Add(scriptManager);
            }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            StringBuilder FancyDispFormDataxml = new StringBuilder();
            itemId = Convert.ToInt32(Page.Request["ID"]);

            if (itemId > 0)
            {
                LoadFancyFormData();
                ManageHTMLOpeningClosing();
                ShowHideDivs();
                divFancyDisplayForm.Visible = true;
                btnCancel1.Visible = true;
                btnCancel2.Visible = true;
            }
            else
            {
                divFancyDisplayForm.Visible = false;
                btnCancel1.Visible = false;
                btnCancel2.Visible = false;
                return;
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            if (Page.Request["Source"] != null)
                Page.Response.Redirect(Convert.ToString(Page.Request["Source"]));
            else
                Page.Response.Redirect(SPContext.Current.Web.Url);
        }

        #endregion

        #region Main Methods

        private void LoadFancyFormData()
        {
            SPList list = null;
            SPListItem item = null;
            Dictionary<string, Dictionary<string, string>> fieldProperties = null;
            string displaySettings = string.Empty;

            try
            {
                list = SPContext.Current.List;
                item = SPContext.Current.ListItem;
            }
            catch { }

            if (list != null && item != null)
            {
                lblItemTitle.Text = Convert.ToString(item["Title"]);
                GridGanttSettings gSettings = new GridGanttSettings(list);

                if (gSettings.DisplaySettings != "")
                    fieldProperties = ListDisplayUtils.ConvertFromString(gSettings.DisplaySettings);

                EPMLiveCore.ListDisplaySettingIterator dispIterator = new ListDisplaySettingIterator();

                if (fieldProperties != null)
                {
                    foreach (SPField field in item.Fields)
                    {
                        try
                        {
                            if (!field.Hidden && fieldProperties.ContainsKey(field.InternalName) && "title" != field.InternalName.ToString().ToLower())
                            {
                                string display = fieldProperties[field.InternalName]["Display"];
                                display = display.Split(";".ToCharArray())[0].ToLower(); //always;;;;;

                                if (display.Equals("always", StringComparison.InvariantCultureIgnoreCase))
                                {
                                    switch (field.Type)
                                    {
                                        case SPFieldType.DateTime:
                                            FillDateDetailsSection(field.Title, field.GetFieldValueAsHtml(item[field.Title]));
                                            break;
                                        case SPFieldType.User:
                                            FillPeopleDetailsSection(field.Title, Convert.ToString(item[field.Title]));
                                            break;
                                        case SPFieldType.Note:
                                            FillNarrativeDetailsSection(field.Title, field.GetFieldValueAsHtml(item[field.Title]));
                                            break;
                                        default:
                                            FillQuickDetailsSection(field, item);
                                            break;
                                    }
                                }
                                else if (displaySettings.Split(";".ToCharArray())[0].ToLower().Equals("where"))
                                {
                                    string where = displaySettings.Split(";".ToCharArray())[1];
                                    string conditionField = "";
                                    string condition = "";
                                    string group = "";
                                    string valueCondition = "";
                                    if (where.Equals("[Me]"))
                                    {
                                        condition = displaySettings.Split(";".ToCharArray())[2];
                                        group = displaySettings.Split(";".ToCharArray())[3];
                                    }
                                    else // [Fielthi
                                    {
                                        conditionField = displaySettings.Split(";".ToCharArray())[2];
                                        condition = displaySettings.Split(";".ToCharArray())[3];
                                        valueCondition = displaySettings.Split(";".ToCharArray())[4];
                                    }

                                    if (EditableFieldDisplay.RenderField(field, where, conditionField, condition, group, valueCondition, item))
                                    {
                                        switch (field.Type)
                                        {
                                            case SPFieldType.DateTime:
                                                FillDateDetailsSection(field.Title, field.GetFieldValueAsText(item[field.Title]));
                                                break;
                                            case SPFieldType.User:
                                                FillPeopleDetailsSection(field.Title, Convert.ToString(item[field.Title]));
                                                break;
                                            case SPFieldType.Note:
                                                FillNarrativeDetailsSection(field.Title, field.GetFieldValueAsText(item[field.Title]));
                                                break;
                                            default:
                                                FillQuickDetailsSection(field, item);
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                    }
                }
                else
                {
                    foreach (SPField field in item.Fields)
                    {
                        if (!field.Hidden && field.ShowInDisplayForm == true)
                        {
                            switch (field.Type)
                            {
                                case SPFieldType.DateTime:
                                    FillDateDetailsSection(field.Title, field.GetFieldValueAsHtml(item[field.Title]));
                                    break;
                                case SPFieldType.User:
                                    FillPeopleDetailsSection(field.Title, Convert.ToString(item[field.Title]));
                                    break;
                                case SPFieldType.Note:
                                    FillNarrativeDetailsSection(field.Title, field.GetFieldValueAsHtml(item[field.Title]));
                                    break;
                                default:
                                    FillQuickDetailsSection(field, item);
                                    break;
                            }
                        }
                    }
                }

                FillItemDetailsSection(item);

                FillAssociatedItemsSection(item);
            }
        }

        private void ManageHTMLOpeningClosing()
        {
            #region Quick Details Section

            if (quickDetailsFieldsCount <= QUICK_DETAILS_SECTION_ONE_FIELD_COUNT)
                sbQuickDetailsContent.Append("</table></td></tr></table>");
            else if (quickDetailsFieldsCount <= QUICK_DETAILS_SECTION_TOTAL_FIELD_COUNT)
                sbQuickDetailsContent.Append("</table></td></tr></table>");
            else
                sbQuickDetailsShowAllRegion.Append("</table>");

            #endregion

            #region Narrative Details Section

            if (narrativeDetailsFieldsCount <= NARRATIVE_DETAILS_SECTION_FIELD_COUNT)
                sbNarrativeDetailsContent.Append("</table></td></tr></table>");
            else
                sbNarrativeDetailsShowAllRegion.Append("</table>");

            #endregion

            #region People Section

            #endregion

            #region Dates Section

            if (datesDetailsFieldsCount <= DATE_DETAILS_SECTION_FIELD_COUNT)
                sbDateDetailsContent.Append("</table></td></tr></table>");
            else
                sbDateDetailsShowAllRegion.Append("</table>");

            #endregion

            #region Item Metadata Section

            #endregion
        }

        private void ShowHideDivs()
        {
            #region Show/Hide Section When 0 Records found

            if (quickDetailsFieldsCount == 0)
                divQuickDetailsParent.Visible = false;
            else
                divQuickDetailsParent.Visible = true;

            if (narrativeDetailsFieldsCount == 0)
                divNarrativeDetailsParent.Visible = false;
            else
                divNarrativeDetailsParent.Visible = true;

            if (peopleDetailsFieldsCount == 0)
                divPeopleDetailsParent.Visible = false;
            else
                divPeopleDetailsParent.Visible = true;

            if (datesDetailsFieldsCount == 0)
                divDateDetailsParent.Visible = false;
            else
                divDateDetailsParent.Visible = true;

            #endregion

            #region Show/Hide Quick Details Section

            if (quickDetailsFieldsCount > 0)
            {
                divQuickDetailsContent.Visible = true;
                divQuickDetailsContent.InnerHtml = sbQuickDetailsContent.ToString();
            }
            else
            {
                divQuickDetailsContent.Visible = false;
            }

            if (quickDetailsFieldsCount > QUICK_DETAILS_SECTION_TOTAL_FIELD_COUNT)
            {
                divShowQuickDetailsHeader.Visible = true;
                divShowQuickDetailsContent.InnerHtml = sbQuickDetailsShowAllRegion.ToString();
            }
            else
            {
                divShowQuickDetailsHeader.Visible = false;
            }

            #endregion

            #region Show/Hide Narrative Details Section

            if (narrativeDetailsFieldsCount > 0)
            {
                divNarrativeDetailsContent.Visible = true;
                divNarrativeDetailsContent.InnerHtml = sbNarrativeDetailsContent.ToString();
            }
            else
            {
                divNarrativeDetailsContent.Visible = false;
            }

            if (narrativeDetailsFieldsCount > NARRATIVE_DETAILS_SECTION_FIELD_COUNT)
            {
                divShowNarrativeDetailsHeader.Visible = true;
                divShowNarrativeDetailsContent.InnerHtml = sbNarrativeDetailsShowAllRegion.ToString();
            }
            else
            {
                divShowNarrativeDetailsHeader.Visible = false;
            }

            #endregion

            #region Show/Hide People Section

            if (peopleDetailsFieldsCount > 0)
            {
                divPeopleContent.Visible = true;
                divPeopleContent.InnerHtml = sbPeopleDetailsContent.ToString();
            }
            else
            {
                divPeopleContent.Visible = false;
            }

            if (peopleDetailsFieldsCount > PEOPLE_DETAILS_SECTION_FIELD_COUNT)
            {
                divPeopleShowAllHeader.Visible = true;
                divPeopleShowAllContent.InnerHtml = sbPeopleDetailsShowAllRegion.ToString();
            }
            else
            {
                divPeopleShowAllHeader.Visible = false;
            }

            #endregion

            #region Show/Hide Dates Section

            if (datesDetailsFieldsCount > 0)
            {
                divDatesContent.Visible = true;
                divDatesContent.InnerHtml = sbDateDetailsContent.ToString();
            }
            else
            {
                divDatesContent.Visible = false;
            }

            if (datesDetailsFieldsCount > DATE_DETAILS_SECTION_FIELD_COUNT)
            {
                divDatesShowAllHeader.Visible = true;
                divDatesShowAllContent.InnerHtml = sbDateDetailsShowAllRegion.ToString();
            }
            else
            {
                divDatesShowAllHeader.Visible = false;
            }

            #endregion

            #region Show/Hide Item Metadata Section

            divItemDetailParent.InnerHtml = sbItemDetailsContent.ToString();

            #endregion

            #region Show/Hide Associated Items Section

            if (associatedItemCount > 0)
            {
                divFancyDispFormParent.Visible = true;
                divFancyDispFormContent.InnerHtml = sbFancyDispFormContent.ToString();
            }
            else
                divFancyDispFormParent.Visible = false;

            #endregion
        }

        #endregion

        #region Fill Methods

        private void FillQuickDetailsSection(SPField field, SPListItem item)
        {
            string fieldValue = field.GetFieldValueAsHtml(item[field.Title]);

            if (quickDetailsFieldsCount == 0)
            {
                sbQuickDetailsContent.Append("<table style='width:100%'><tr><td><table class='fancy-col-table'>");
            }
            else if (quickDetailsFieldsCount == QUICK_DETAILS_SECTION_ONE_FIELD_COUNT)
            {
                sbQuickDetailsContent.Append("</table></td><td style='vertical-align: top'><table class='fancy-col-table'>");
            }
            else if (quickDetailsFieldsCount == QUICK_DETAILS_SECTION_TOTAL_FIELD_COUNT)
            {
                sbQuickDetailsContent.Append("</table></td></tr></table>");
                sbQuickDetailsShowAllRegion.Append("<table class='fancy-col-table'>");
            }

            if (quickDetailsFieldsCount < QUICK_DETAILS_SECTION_TOTAL_FIELD_COUNT)
            {
                sbQuickDetailsContent.Append("<tr>");
                sbQuickDetailsContent.Append("<td>" + field.Title + "</td>");
                if (field.Type == SPFieldType.Calculated)
                {
                    if (field.InternalName.ToLower().Equals("due"))
                        sbQuickDetailsContent.Append("<td>" + CoreFunctions.GetDueField(item) + "</td>");
                    else if (field.InternalName.ToLower().Equals("daysoverdue"))
                        sbQuickDetailsContent.Append("<td>" + CoreFunctions.GetDaysOverdueField(item) + "</td>");
                    else if (field.InternalName.ToLower().Equals("schedulestatus"))
                        sbQuickDetailsContent.Append("<td><img src='/_layouts/images/" + CoreFunctions.GetScheduleStatusField(item) + "' /></td>");
                    else if (fieldValue.ToLower().Contains(".gif") || fieldValue.ToLower().Contains(".jpg") || fieldValue.ToLower().Contains(".png"))
                        sbQuickDetailsContent.Append("<td><img src=\"" + SPContext.Current.Web.Url + "/_layouts/images/" + fieldValue + "\" /></td>");
                    else
                        sbQuickDetailsContent.Append("<td>" + fieldValue + "</td>");
                }
                else if (field.Type == SPFieldType.Boolean)
                {
                    sbQuickDetailsContent.Append("<td>" + fieldValue + "</td>");
                }
                else
                {
                    sbQuickDetailsContent.Append("<td>" + fieldValue + "</td>");
                }
                sbQuickDetailsContent.Append("</tr>");
            }
            else
            {
                sbQuickDetailsShowAllRegion.Append("<tr>");
                sbQuickDetailsShowAllRegion.Append("<td>" + field.Title + "</td>");
                if (field.Type == SPFieldType.Calculated)
                {
                    if (field.InternalName.ToLower().Equals("due"))
                        sbQuickDetailsShowAllRegion.Append("<td>" + CoreFunctions.GetDueField(item) + "</td>");
                    else if (field.InternalName.ToLower().Equals("daysoverdue"))
                        sbQuickDetailsShowAllRegion.Append("<td>" + CoreFunctions.GetDaysOverdueField(item) + "</td>");
                    else if (field.InternalName.ToLower().Equals("schedulestatus"))
                        sbQuickDetailsShowAllRegion.Append("<td><img src='/_layouts/images/" + CoreFunctions.GetScheduleStatusField(item) + "' /></td>");
                    else if (fieldValue.ToLower().Contains(".gif") || fieldValue.ToLower().Contains(".jpg") || fieldValue.ToLower().Contains(".png"))
                        sbQuickDetailsShowAllRegion.Append("<td><img src=\"" + SPContext.Current.Web.Url + "/_layouts/images/" + fieldValue + "\"  /></td>");
                    else
                        sbQuickDetailsShowAllRegion.Append("<td>" + fieldValue + "</td>");
                }
                else if (field.Type == SPFieldType.Boolean)
                {
                    sbQuickDetailsShowAllRegion.Append("<td>" + fieldValue + "</td>");
                }
                else
                {
                    sbQuickDetailsShowAllRegion.Append("<td>" + fieldValue + "</td>");
                }
                sbQuickDetailsShowAllRegion.Append("</tr>");
            }
            quickDetailsFieldsCount++;
        }

        private void FillNarrativeDetailsSection(string fieldName, string fieldValue)
        {
            if (narrativeDetailsFieldsCount == 0)
            {
                sbNarrativeDetailsContent.Append("<table style='width:100%;'><tr><td><table class='fancy-col-table'>");
            }
            else if (narrativeDetailsFieldsCount == NARRATIVE_DETAILS_SECTION_FIELD_COUNT)
            {
                sbNarrativeDetailsContent.Append("</table></td></tr></table>");
                sbNarrativeDetailsShowAllRegion.Append("<table class='fancy-col-table'>");
            }

            if (narrativeDetailsFieldsCount < NARRATIVE_DETAILS_SECTION_FIELD_COUNT)
            {
                sbNarrativeDetailsContent.Append("<tr>");
                sbNarrativeDetailsContent.Append("<td>" + fieldName + "</td>");
                sbNarrativeDetailsContent.Append("<td>" + fieldValue + "</td>");
                sbNarrativeDetailsContent.Append("</tr>");
            }
            else
            {
                sbNarrativeDetailsShowAllRegion.Append("<tr>");
                sbNarrativeDetailsShowAllRegion.Append("<td>" + fieldName + "</td>");
                sbNarrativeDetailsShowAllRegion.Append("<td>" + fieldValue + "</td>");
                sbNarrativeDetailsShowAllRegion.Append("</tr>");
            }
            narrativeDetailsFieldsCount++;
        }

        private void FillPeopleDetailsSection(string fieldName, string fieldValue)
        {
            DataTable dtUserDetails = new DataTable();
            string[] userSeperator = new string[] { ";#" };
            string userIds = string.Empty;
            string imageUrl = string.Empty;
            string userName = string.Empty;
            string userID = string.Empty;

            if (!string.IsNullOrEmpty(fieldValue) && fieldValue.Contains(";#"))
            {
                int i = 0; //13;#Bhavdip Shah;#48;#Dhaval Upadyay;#2;#WSS - Retrieve only user id...
                string[] userDetails = fieldValue.Split(userSeperator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string user in userDetails)
                {
                    if (i % 2 == 0)
                        userIds += user + ",";
                    i++;
                }

                if (!string.IsNullOrEmpty(userIds))
                {
                    userIds = userIds.Substring(0, userIds.Length - 1);
                    try
                    {
                        string sqlUserDetails = "SELECT * FROM LSTUserInformationList WHERE ID IN(" + userIds + ")";
                        var queryExecutor = new QueryExecutor(SPContext.Current.Web);
                        dtUserDetails = queryExecutor.ExecuteReportingDBQuery(sqlUserDetails, new Dictionary<string, object> { });
                    }
                    catch { }

                    if (dtUserDetails != null && dtUserDetails.Rows.Count > 0)
                    {
                        if (peopleDetailsFieldsCount == 0)
                        {
                            sbPeopleDetailsContent.Append("<table style='width: 100%'><tr><td><table class='fancy-col-table'>");
                        }
                        else if (peopleDetailsFieldsCount == PEOPLE_DETAILS_SECTION_FIELD_COUNT)
                        {
                            sbPeopleDetailsContent.Append("</table></td></tr></table>");
                            sbPeopleDetailsShowAllRegion.Append("<table class='fancy-col-table'>");
                        }

                        if (peopleDetailsFieldsCount < PEOPLE_DETAILS_SECTION_FIELD_COUNT)
                        {
                            sbPeopleDetailsContent.Append("<tr>");
                            sbPeopleDetailsContent.Append("<td>" + fieldName + "</td>");
                        }
                        else
                        {
                            sbPeopleDetailsShowAllRegion.Append("<tr>");
                            sbPeopleDetailsShowAllRegion.Append("<td>" + fieldName + "</td>");
                        }

                        for (int user = 0; user < dtUserDetails.Rows.Count; user++)
                        {
                            imageUrl = Convert.ToString(dtUserDetails.Rows[user]["Picture"]);
                            userName = Convert.ToString(dtUserDetails.Rows[user]["Title"]);
                            userID = Convert.ToString(dtUserDetails.Rows[user]["ID"]);

                            if (peopleDetailsFieldsCount < PEOPLE_DETAILS_SECTION_FIELD_COUNT)
                            {
                                sbPeopleDetailsContent.Append("<td style='text-align: right'>");

                                if (!string.IsNullOrEmpty(imageUrl))
                                    sbPeopleDetailsContent.Append("<img alt='" + userName + "' src='" + imageUrl.Remove(imageUrl.IndexOf(", ")) + "' class='dispFormUserImage' /></td>");
                                else
                                    sbPeopleDetailsContent.Append("<img alt='" + userName + "' src='../_layouts/15/images/User.png' class='dispFormUserImage' /></td>");

                                sbPeopleDetailsContent.Append("<td style='vertical-align: middle'>");
                                sbPeopleDetailsContent.Append("<a class='ms-subtleLink' onclick='GoToLinkOrDialogNewWindow(this);return false;' href='" + SPContext.Current.Web.Url + "/_layouts/15/userdisp.aspx?ID=" + userID + "'>" + userName + "</a>" + "&nbsp;&nbsp;</td>");

                                if ((user + 1) == NUMBER_OF_PEOPLE_PER_FIELD_DISPLAY)
                                {
                                    sbPeopleDetailsContent.Append("<td class='dispFormExpandMore'>(more...)</td>");
                                    sbPeopleDetailsContent.Append("</tr>");
                                    sbPeopleDetailsContent.Append("<tr class='ShowMoreRow' style='display: none'>");
                                    sbPeopleDetailsContent.Append("<td>&nbsp;</td>");
                                }
                            }
                            else
                            {
                                sbPeopleDetailsShowAllRegion.Append("<td style='text-align: right'>");

                                if (!string.IsNullOrEmpty(imageUrl))
                                    sbPeopleDetailsShowAllRegion.Append("<img alt='" + userName + "' src='" + imageUrl.Remove(imageUrl.IndexOf(", ")) + "' class='dispFormUserImage' /></td>");
                                else
                                    sbPeopleDetailsShowAllRegion.Append("<img alt='" + userName + "' src='../_layouts/15/images/User.png' class='dispFormUserImage' /></td>");

                                sbPeopleDetailsShowAllRegion.Append("<td style='vertical-align: middle'>");
                                sbPeopleDetailsShowAllRegion.Append("<a class'ms-subtleLink' onclick='GoToLinkOrDialogNewWindow(this);return false;' href='" + SPContext.Current.Web.Url + "/_layouts/15/userdisp.aspx?ID=" + userID + "'>" + userName + "</a>" + "&nbsp;&nbsp;</td>");

                                if ((user + 1) == NUMBER_OF_PEOPLE_PER_FIELD_DISPLAY)
                                {
                                    sbPeopleDetailsShowAllRegion.Append("<td class='dispFormExpandMore'>(more...)</td>");
                                    sbPeopleDetailsShowAllRegion.Append("</tr>");
                                    sbPeopleDetailsShowAllRegion.Append("<tr class='ShowMoreRow' style='display: none'>");
                                    sbPeopleDetailsShowAllRegion.Append("<td>&nbsp;</td>");
                                }
                            }
                        }

                        if (peopleDetailsFieldsCount < PEOPLE_DETAILS_SECTION_FIELD_COUNT)
                            sbPeopleDetailsContent.Append("</tr>");
                        else
                            sbPeopleDetailsShowAllRegion.Append("</tr>");

                        peopleDetailsFieldsCount++;
                    }
                }
            }
        }

        private void FillDateDetailsSection(string fieldName, string fieldValue)
        {
            if (datesDetailsFieldsCount == 0)
            {
                sbDateDetailsContent.Append("<table style='width:100%;'><tr><td><table class='fancy-col-table'>");
            }
            else if (datesDetailsFieldsCount == DATE_DETAILS_SECTION_FIELD_COUNT)
            {
                sbDateDetailsContent.Append("</table></td></tr></table>");
                sbDateDetailsShowAllRegion.Append("<table class='fancy-col-table'>");
            }

            if (datesDetailsFieldsCount < DATE_DETAILS_SECTION_FIELD_COUNT)
            {
                sbDateDetailsContent.Append("<tr>");
                sbDateDetailsContent.Append("<td>" + fieldName + "</td>");
                sbDateDetailsContent.Append("<td>" + fieldValue + "</td>");
                sbDateDetailsContent.Append("</tr>");
            }
            else
            {
                sbDateDetailsShowAllRegion.Append("<tr>");
                sbDateDetailsShowAllRegion.Append("<td>" + fieldName + "</td>");
                sbDateDetailsShowAllRegion.Append("<td>" + fieldValue + "</td>");
                sbDateDetailsShowAllRegion.Append("</tr>");
            }
            datesDetailsFieldsCount++;
        }

        private void FillItemDetailsSection(SPListItem item)
        {

            sbItemDetailsContent.Append("<table style='width:100%'>");
            sbItemDetailsContent.Append("<tr colspan='2'>");
            sbItemDetailsContent.Append("<td>Created " + ((DateTime)(item[SPBuiltInFieldId.Created])).ToFriendlyDateAndTime() + " by " + ((SPField)item.Fields[SPBuiltInFieldId.Author]).GetFieldValueAsHtml(item[SPBuiltInFieldId.Author]) + "</td>");
            sbItemDetailsContent.Append("</tr>");
            sbItemDetailsContent.Append("<tr colspan='2'>");
            sbItemDetailsContent.Append("<td>Last modified " + ((DateTime)(item[SPBuiltInFieldId.Modified])).ToFriendlyDateAndTime() + " by " + ((SPField)item.Fields[SPBuiltInFieldId.Editor]).GetFieldValueAsHtml(item[SPBuiltInFieldId.Editor]) + "</td>");
            sbItemDetailsContent.Append("</tr>");
            sbItemDetailsContent.Append("</table>");
        }

        private void FillAssociatedItemsSection(SPListItem item)
        {
            string sqlLists = string.Empty;
            StringBuilder sqlGetListHeaders = new StringBuilder();
            ArrayList arrAssociatedLists = null;
            string projectLinkedField = "Project";
            DataTable dtListName = null;
            string sourceUrl = string.Empty;

            SPSite spSite = SPContext.Current.Site;
            SPWeb spWeb = SPContext.Current.Web;
            SPList list = SPContext.Current.List;

            if (Page.Request["Source"] != null)
                sourceUrl = Convert.ToString(Page.Request["Source"]);
            else
                sourceUrl = SPContext.Current.Web.Url;

            arrAssociatedLists = EPMLiveCore.API.ListCommands.GetAssociatedLists(list);

            if (arrAssociatedLists != null && arrAssociatedLists.Count > 0)
            {
                associatedItemCount = arrAssociatedLists.Count;

                #region Prepare DataTable for Associated Items

                //Prepare query to load list whether it is exists in Reporting database or not?!!
                sqlGetListHeaders.Append("SELECT * FROM (");
                foreach (EPMLiveCore.API.AssociatedListInfo associatedListItem in arrAssociatedLists)
                    sqlGetListHeaders.Append("SELECT '" + associatedListItem.Title + "' ListName UNION ");

                sqlLists = sqlGetListHeaders.ToString().Substring(0, sqlGetListHeaders.ToString().Length - 6); //Removing last UNION keyword
                sqlLists = sqlLists + ") AssociatedItemList LEFT OUTER JOIN RPTList ON AssociatedItemList.ListName = RPTList.ListName";

                #endregion

                try
                {
                    var queryExecutor = new QueryExecutor(spWeb);
                    dtListName = queryExecutor.ExecuteReportingDBQuery(sqlLists, new Dictionary<string, object> { });
                }
                catch { }

                #region Load Associated Items (Count)

                #endregion

                if (dtListName != null && dtListName.Rows.Count > 0)
                {
                    sbFancyDispFormContent.Append("<table style='width:100%'><tr><td><table class='fancy-col-table'>");

                    for (int i = 0; i < dtListName.Rows.Count; i++)
                    {
                        string rptListId = Convert.ToString(dtListName.Rows[i]["RPTListID"]);
                        string listName = Convert.ToString(dtListName.Rows[i]["ListName"]);
                        string tableName = Convert.ToString(dtListName.Rows[i]["TableName"]);

                        //Checking if rptListId is Null or not: If Null then use Object Model to load count, database call otherwise
                        if (!string.IsNullOrEmpty(rptListId))
                        {
                            #region Retrieve associated list count from database

                            foreach (EPMLiveCore.API.AssociatedListInfo associatedListItemInfo in arrAssociatedLists)
                            {
                                if (associatedListItemInfo.ListId.ToString().Equals(rptListId, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    projectLinkedField = associatedListItemInfo.LinkedField;
                                    break;
                                }
                            }

                            string sql = string.Empty;
                            sql = "SELECT COUNT(*) AS 'Count' FROM " + tableName + " WHERE CONVERT(NVARCHAR(MAX), " + projectLinkedField + "ID) LIKE N'%" + Convert.ToString(SPContext.Current.ItemId) + "%' AND ListId = '" + rptListId + "'";

                            DataTable dtListItemCount = null;
                            try
                            {
                                // Wait for 3 seconds just in-case if reporting database is not updated.
                                Thread.Sleep(3000);
                                var queryExecutor = new QueryExecutor(spWeb);
                                dtListItemCount = queryExecutor.ExecuteReportingDBQuery(sql, new Dictionary<string, object> { });
                            }
                            catch { }

                            int FancyDispFormCount = 0;
                            if (dtListItemCount != null && dtListItemCount.Rows.Count > 0)
                            {
                                FancyDispFormCount = Convert.ToInt32(dtListItemCount.Rows[0]["Count"]);
                            }

                            sbFancyDispFormContent.Append("<tr>");
                            sbFancyDispFormContent.Append("<td>" + listName + "</td>");
                            sbFancyDispFormContent.Append("<td class='pill-cell'><span class='badge'><div id='div_" + rptListId + "' class='listMainDiv'>" + Convert.ToString(FancyDispFormCount) + "</span></a>");

                            #endregion
                        }
                        else
                        {
                            #region Retrieve associated list count using SharePoint Object Model

                            //SharePoint Object Model to Load Record Count
                            SPList otherList = spWeb.Lists[listName];
                            SPQuery query = new SPQuery();
                            rptListId = otherList.ID.ToString();

                            foreach (EPMLiveCore.API.AssociatedListInfo associatedItem in arrAssociatedLists)
                            {
                                if (associatedItem.ListId.ToString().Equals(rptListId, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    projectLinkedField = associatedItem.LinkedField;
                                    break;
                                }
                            }

                            query.Query = "<Where><Eq><FieldRef Name='" + projectLinkedField + "' LookupId='True' /><Value Type='Lookup'>" + item.ID + "</Value></Eq></Where>";
                            Int64 itemCount = otherList.GetItems(query).Count;

                            sbFancyDispFormContent.Append("<tr>");
                            sbFancyDispFormContent.Append("<td>" + listName + "</td>");
                            sbFancyDispFormContent.Append("<td class='pill-cell'><span class='badge'><div id='div_" + otherList.ID.ToString() + "' class='listMainDiv'>" + Convert.ToString(itemCount) + "</span></a>");

                            #endregion
                        }

                        #region Load Top-5 Items (Using SharePoint Object Model)

                        //Load Top-5 Items...
                        SPList projectAssociatedList = spWeb.Lists[listName];
                        SPQuery qryFancyDispForm = new SPQuery();
                        qryFancyDispForm.Query = "<Where><Eq><FieldRef Name='" + projectLinkedField + "' LookupId='True' /><Value Type='Lookup'>" + item.ID + "</Value></Eq></Where><QueryOptions></QueryOptions><OrderBy><FieldRef Name='Modified' Ascending='FALSE' /></OrderBy>";
                        qryFancyDispForm.RowLimit = 5;
                        SPListItemCollection top5FancyDispForm = projectAssociatedList.GetItems(qryFancyDispForm);

                        sbFancyDispFormContent.Append("<div id='div_items_" + rptListId + "'  class='slidingDiv'>");
                        sbFancyDispFormContent.Append("<div class='slidingDivHeader'>" + listName + "</div>");

                        if (!EPMLiveCore.API.ListCommands.GetGridGanttSettings(projectAssociatedList).HideNewButton)
                        {
                            string newFormUrl = projectAssociatedList.DefaultNewFormUrl + "?LookupField=" + projectLinkedField + "&LookupValue=" + item.ID;
                            sbFancyDispFormContent.Append("<div class='slidingDivAdd'><a href='#' onclick=\"javascript:FancyDispFormClient.showNewForm('" + newFormUrl + "');return false;\"><img title='Add new " + listName + "' alt='' src='/_layouts/epmlive/images/newitem5.png' class='ms-core-menu-buttonIcon'></img></a></div>");
                        }

                        sbFancyDispFormContent.Append("<br/>");

                        sbFancyDispFormContent.Append("<div style='clear:both;'></div>");
                        sbFancyDispFormContent.Append("<table class='fancy-col-table' style='color:black; font-weight:normal;'>");

                        foreach (SPListItem currentListItem in top5FancyDispForm)
                        {
                            sbFancyDispFormContent.Append("<tr>");

                            if (currentListItem.Title != null && currentListItem.Title.TrimEnd().Length > 25)
                            {
                                sbFancyDispFormContent.Append("<td><a href='#' alt='" + currentListItem.Title + "' title='" + currentListItem.Title + "' onclick=\"javascript:FancyDispFormClient.showNewForm('" + projectAssociatedList.DefaultDisplayFormUrl + "?ID=" + currentListItem.ID + "&Source=" + sourceUrl + "');return false;\">" + currentListItem.Title.Substring(0, 25) + "..." + "</a></td>");
                            }
                            else
                            {
                                sbFancyDispFormContent.Append("<td><a href='#' alt='" + currentListItem.Title + "' title='" + currentListItem.Title + "' onclick=\"javascript:FancyDispFormClient.showNewForm('" + projectAssociatedList.DefaultDisplayFormUrl + "?ID=" + currentListItem.ID + "&Source=" + sourceUrl + "');return false;\">" + currentListItem.Title + "</a></td>");
                            }

                            sbFancyDispFormContent.Append("<td>");

                            sbFancyDispFormContent.Append("<li class='fancyDisplayFormAssociatedItemsContextMenu'>");
                            sbFancyDispFormContent.Append("<a data-itemid='" + currentListItem.ID + "' data-listid='" + projectAssociatedList.ID.ToString() + "' data-webid='" + spWeb.ID + "' data-siteid='" + spSite.ID + "'>");
                            sbFancyDispFormContent.Append("</a>");
                            sbFancyDispFormContent.Append("</li>");
                            sbFancyDispFormContent.Append("</td>");

                            sbFancyDispFormContent.Append("</tr>");
                        }

                        sbFancyDispFormContent.Append("</table>");
                        sbFancyDispFormContent.Append("<br/>");
                        sbFancyDispFormContent.Append("<br/>");

                        qryFancyDispForm = new SPQuery();
                        qryFancyDispForm.Query = "<Where><Eq><FieldRef Name='" + projectLinkedField + "' LookupId='True' /><Value Type='Lookup'>" + item.ID + "</Value></Eq></Where><QueryOptions></QueryOptions><OrderBy><FieldRef Name='Modified' Ascending='FALSE' /></OrderBy>";
                        SPListItemCollection otherFancyDispForm = projectAssociatedList.GetItems(qryFancyDispForm);

                        if (otherFancyDispForm.Count > 5)
                        {
                            string viewAllItemsUrl = spWeb.Url + "/_layouts/epmlive/gridaction.aspx?action=linkeditemspost&listid=" + list.ID + "&lookups=" + item.Title + "&field=" + projectLinkedField + "&LookupFieldList=" + projectAssociatedList.ID.ToString();// +"&Source=" + sourceUrl;
                            sbFancyDispFormContent.Append("<a href='#' onclick=\"javascript:FancyDispFormClient.showItemUrl('" + viewAllItemsUrl + "');return false;\">View All " + projectAssociatedList.Title + "</a>");
                        }

                        sbFancyDispFormContent.Append("</div>"); //div_items_listGuid - Div Ends..
                        sbFancyDispFormContent.Append("</div></div>"); //div_listGUID - Div Ends..
                        sbFancyDispFormContent.Append("</td></tr>");

                        #endregion
                    }
                }
                sbFancyDispFormContent.Append("</table></td></tr></table>");
                //} //Report Admin - if ends...
            }
        }

        #endregion
    }
}
