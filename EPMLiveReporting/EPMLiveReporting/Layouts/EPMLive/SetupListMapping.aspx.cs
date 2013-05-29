﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Microsoft.SharePoint.Utilities;
using System.Diagnostics;

namespace EPMLiveReportsAdmin.Layouts.EPMLive 
{
    public partial class SetupListMapping : LayoutsPageBase
    {
       protected Panel pnlListSelect;
        //protected InputFormCheckBox chkResource;
        //protected InputFormCheckBoxList cblFields;
        //protected CheckBoxList cblResources;
        //protected CheckBoxList cblAutomatic;
        //protected DropDownList ddlLists;
        //protected HtmlGenericControl pDescription1;
        //protected HtmlGenericControl pDescription2;
        private bool _existing;
        private Guid _existingListId;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Request["ListID"] != null)
            {
                _existing = true;
                _existingListId = new Guid(Request["ListID"]);
            }

            if (!IsPostBack)
            {
                if (_existing)
                {
                    var list = PopulateFieldList();
                    ddlLists.Items.Add(new ListItem(list.Title));
                    ddlLists.Enabled = false;
                }
                else
                    PopulateControls();
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private void PopulateControls()
        {
            SPSite site = SPContext.Current.Site;

            var rb = new ReportBiz(site.ID);
            Collection<string> existingLists = rb.GetMappedLists();

            using (SPWeb web = site.OpenWeb())
            {
                foreach (SPList list in web.Lists)
                {
                    // skip hidden lists
                    if (list.Hidden)
                    {
                        continue;
                    }

                    var item = new ListItem(list.Title, list.ID.ToString());
                    if (existingLists.Contains(list.Title))
                        continue;

                    ddlLists.Items.Add(item);
                }
            }
            if (ddlLists.Items.Count == 0)
            {
                ddlLists.Items.Add(new ListItem("All lists already mapped"));
                ddlLists.SelectedIndex = 0;
                ddlLists.Enabled = false;
                return;
            }
            PopulateFieldList();
        }

        protected void Cancel_Click(object sender, EventArgs e)
        {
            SPUtility.Redirect(SPContext.Current.Web.Url + "/_layouts/epmlive/listmappings.aspx?Source=" + SPContext.Current.Web.Url + "/_layouts/epmlive/settings.aspx", SPRedirectFlags.Static, HttpContext.Current);
        }

        protected void Submit_Click(object sender, EventArgs e)
        {
            var fields = new ListItemCollection();            
            fields = cblAutomatic.Items;

            // Read selected fields
            // Quirk Alert: if we have disabled an item from javascript, it can't be read here on postback.
            // Instead we have to cycle a hidden checkboxlist that contains resource fileds IF chkResource is checked
            foreach (ListItem field in cblFields.Items)
            {
                if (field.Selected && fields.FindByText(field.Text) == null)
                    fields.Add(field);
            }

            // Add all resource fields if chkResource is checked.
            // (Only add if not already in the list)
            if (chkResource.Checked)
                foreach (ListItem resourceField in cblResources.Items)
                {
                    if (fields.FindByText(resourceField.Text) == null)
                        fields.Add(resourceField);
                }

            var rb = new ReportBiz(SPContext.Current.Site.ID);

            if (_existing)
            {
                ListBiz lb = rb.GetListBiz(_existingListId);
                lb.UpdateListMapping(fields);
            }
            else
            {
                var listId = new Guid(ddlLists.SelectedValue);
                rb.CreateListBiz(listId, fields);
            }                      

            try
            {
                //FOREIGN IMPLEMENTATION -- START
                EPMData DAO = new EPMData(SPContext.Current.Site.ID);
                rb.UpdateForeignKeys(DAO);
                DAO.Dispose();
                // -- END
            }
            catch (Exception ex)
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {
                    if (!EventLog.SourceExists("EPMLive Reporting - UpdateForeignKeys"))
                        EventLog.CreateEventSource("EPMLive Reporting - UpdateForeignKeys", "EPM Live");

                    EventLog myLog = new EventLog("EPM Live", ".", "EPMLive Reporting - UpdateForeignKeys");
                    myLog.MaximumKilobytes = 32768;
                    myLog.WriteEntry(ex.Message + ex.StackTrace, EventLogEntryType.Error, 4001);
                });
            }

            SPUtility.Redirect("epmlive/ListMappings.aspx", SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);
        }

        protected void ddlLists_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateFieldList();
        }

        private SPList PopulateFieldList()
        {

            //Reset checkboxlists
            cblFields.Items.Clear();
            cblAutomatic.Items.Clear();
            cblResources.Items.Clear();

            Guid listId = _existing
                              ? _existingListId
                              : new Guid(ddlLists.SelectedValue);

            SPList selectedList = null;
            var mappedFields = new List<string>();
            bool resList = false;

            using (SPWeb web = SPContext.Current.Site.OpenWeb())
            {
                try
                {
                    selectedList = web.Lists[listId];
                }
                catch { }
            }

            if (selectedList == null)
            {
                foreach (SPWeb w in SPContext.Current.Site.AllWebs)
                {
                    try
                    {
                        selectedList = w.Lists[listId];
                    }
                    catch { }

                    w.Close();

                    if (selectedList != null)
                    {
                        break;
                    }
                }
            }

            // Get existing mappings if they exist
            if (_existing)
            {
                var rb = new ReportBiz(SPContext.Current.Site.ID);
                ListBiz list = rb.GetListBiz(_existingListId);
                mappedFields = list.GetMappedFieldsStrings();
                resList = list.ResourceList;
                chkResource.Checked = list.ResourceList;
                chkResource.Enabled = true;
            }

            var required = ListBiz.RequiredResourceFields;
            var automatic = ListBiz.AutomaticFields;

            // Get all fields in List - and sort them
            var fields = new List<SPField>();
            int matches = 0;
            foreach (SPField field in selectedList.Fields)
            {
                if (required.Contains(field.InternalName))
                    matches++;
                fields.Add(field);
            }
            fields.Sort((f1, f2) => f1.Title.CompareTo(f2.Title));

            // Determine if this list has necessary fields to be a resource list
            bool resourceCandidate = (required.Count == matches);

            chkResource.Enabled = resourceCandidate;

            if (!_existing)
                chkResource.Checked = resourceCandidate;

            // Create Checkbox List
            //      automatic: always selected
            //      required: optional but necessary for resource reporting
           
            int insertReq = 0; // Position in list to insert required items (top)
            int insertChecked = 0;
            //var resourceItems = new ListItemCollection();
            foreach (SPField field in fields)
            {
                var item = new ListItem(string.Format("{0}", field.Title), field.Id.ToString());
                if (field.InternalName == "Title")
                {
                    item.Enabled = false;
                    item.Selected = true;
                    cblFields.Items.Insert(0, item);
                    item.Text = string.Format("<span style='font-weight:bold'>{0}</span>", item.Text);
                    insertReq++;
                    insertChecked++;
                }

                // Save automatic fields so we can pick them up later.                
                if (automatic.Contains(field.InternalName))
                {
                    cblAutomatic.Items.Add(item);
                    continue;
                }

                if (automatic.Contains(field.InternalName))
                {
                    cblAutomatic.Items.Add(item);
                    continue;
                }

                if (field.InternalName.ToLower() == "contenttype")
                {
                    cblFields.Items.Add(item);
                    continue;
                }

                if (field.Hidden
                    || field.Type == SPFieldType.Computed
                    || automatic.Contains(field.InternalName) // Change: don't show automatic fields
                    )
                    continue;

                if (mappedFields.Contains(field.InternalName))
                    item.Selected = true;
                if (resourceCandidate && required.Contains(field.InternalName))
                {
                    if (!_existing)
                        item.Selected = true;
                    item.Text = string.Format("<span style='font-weight:bold'>{0}</span>", item.Text);
                    cblFields.Items.Insert(insertReq++, item);
                    // Set hidden CBL for reading values on postback
                    cblResources.Items.Add(item);
                    insertChecked++;
                }
                else
                {
                    if (item.Selected)
                        cblFields.Items.Insert(insertChecked++, item);
                    else
                        cblFields.Items.Add(item);
                }
            }

            // Create javascript magic
            if (resourceCandidate)
                WriteJS(cblFields);

            pDescription1.Visible = resourceCandidate;
            pDescription2.Visible = resList;

            return selectedList;
        }

        /// <summary>
        /// Create javascript function to toggle resource checkboxes
        /// </summary>
        /// <remarks>
        /// Assumes 4 checkboxes from 1-4, not 0. First checkbox is always 'Title'.
        /// </remarks>
        private void WriteJS(InputFormCheckBoxList cbl)
        {
            chkResource.InputAttributes.Add("onclick", "Toggle()");

            var cdef = "";
            var cdis = "";
            var cena = "";
            var cchk = "";
            for (int j = 1; j < 5; j++)
            {
                cdef += string.Format(" var c{0} = document.getElementById('{1}_{0}'); ", j, cbl.ClientID);
                cchk += string.Format(" c{0}.checked = true;", j);
                cdis += string.Format(" c{0}.disabled = true; c{0}.parentNode.disabled = true;", j);
                cena += string.Format(" c{0}.disabled = false; c{0}.parentNode.disabled = false;", j);
            }
            var script = string.Format(
            @"
            <script type='text/javascript'>
            function Toggle() {{
                var master = document.getElementById('{0}');
                {1}     
                if (master.checked) {{
                    {2}
                    {3}
                }}
                else {{
                    {4}
                }};
            }}

            Toggle();

            </script>
            ", chkResource.ClientID, cdef, cchk, cdis, cena);

            ClientScript.RegisterStartupScript(typeof(Page), "EPMReportsScript", script);
        }
    }    
}
