﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace EPMLiveCore.Layouts.epmlive
{
    public partial class ManageFragment : LayoutsPageBase
    {
        public bool _isPrivate;
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ScriptLink.Register(Page, "/epmlive/javascripts/libraries/jquery.min.js", false);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    FillMyFragmentsGrid();
                    FillPublicFragmentsGrid();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void FillMyFragmentsGrid()
        {
            SPList plannerFragmentList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments");
            SPQuery qryFilterPlanner = new SPQuery();

            string qryFilter = "<ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='FragmentType' /><FieldRef Name='Author' /></ViewFields><OrderBy><FieldRef Name='Title' /><FieldRef Name='Author' /></OrderBy><Where><And><And><Eq><FieldRef Name='Author' /><Value Type='User'>" + SPContext.Current.Web.CurrentUser.Name + "</Value></Eq><Eq><FieldRef Name='PlannerID' /><Value Type='Text'>" + Convert.ToString(Request["PlannerID"]) + "</Value></Eq></And><Eq><FieldRef Name='FragmentType' /><Value Type='Choice'>Private</Value></Eq></And></Where>";

            if (plannerFragmentList != null)
            {
                qryFilterPlanner.Query = qryFilter;
                SPListItemCollection fragmentItems = plannerFragmentList.GetItems(qryFilterPlanner);
                gridMyFragments.DataSource = fragmentItems.GetDataTable();
                gridMyFragments.DataBind();
            }
        }

        private void FillPublicFragmentsGrid()
        {
            SPList plannerFragmentList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments");
            SPQuery qryFilterPlanner = new SPQuery();
            string qryFilter = string.Empty;

            if (SPContext.Current.Web.CurrentUser.IsSiteAdmin)
                qryFilter = "<ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='FragmentType' /><FieldRef Name='Author' /></ViewFields><OrderBy><FieldRef Name='Title' /><FieldRef Name='Author' /></OrderBy><Where><And><Eq><FieldRef Name='FragmentType' /><Value Type='Choice'>Public</Value></Eq><Eq><FieldRef Name='PlannerID' /><Value Type='Text'>" + Convert.ToString(Request["PlannerID"]) + "</Value></Eq></And></Where>";
            else
                qryFilter = "<ViewFields><FieldRef Name='ID' /><FieldRef Name='Title' /><FieldRef Name='FragmentType' /><FieldRef Name='Author' /></ViewFields><OrderBy><FieldRef Name='Title' /><FieldRef Name='Author' /></OrderBy><Where><And><And><Eq><FieldRef Name='FragmentType' /><Value Type='Choice'>Public</Value></Eq><Eq><FieldRef Name='PlannerID' /><Value Type='Text'>" + Convert.ToString(Request["PlannerID"]) + "</Value></Eq></And><Eq><FieldRef Name='Author' /><Value Type='User'>" + SPContext.Current.Web.CurrentUser.Name + "</Value></Eq></And></Where>";

            if (plannerFragmentList != null)
            {
                qryFilterPlanner.Query = qryFilter;

                SPListItemCollection fragmentItems = plannerFragmentList.GetItems(qryFilterPlanner);
                gridPublicFragments.DataSource = fragmentItems.GetDataTable(); ;
                gridPublicFragments.DataBind();
            }
        }

        private void UpdatingGrid(GridView gridView, int rowInd, out SPListItem outFragment)
        {
            string fragmentName = string.Empty;
            SPList plannerFragmentList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments");
            Label lblId = (Label)gridView.Rows[rowInd].FindControl("lblID");
            SPListItem fragment;
            if (lblId != null)
            {
                fragment = plannerFragmentList.GetItemById(Convert.ToInt32(lblId.Text));
                fragment["Title"] = ((TextBox)gridView.Rows[rowInd].Cells[1].Controls[0]).Text;
                bool isPrivate = ((CheckBox)gridView.Rows[rowInd].FindControl("chkPrivate")).Checked;
                if (isPrivate)
                {
                    fragment["FragmentType"] = "Private";
                }
                else
                {
                    fragment["FragmentType"] = "Public";
                }

                fragment.Update();
                plannerFragmentList.Update();
                gridView.EditIndex = -1;
                outFragment = fragment;
            }
            else
            {
                outFragment = null;
            }
        }

        protected void lnkdelete_Click(object sender, EventArgs e)
        {
            string fragmentName = string.Empty;
            LinkButton lnkbtn = sender as LinkButton;
            //getting particular row linkbutton
            GridViewRow gvrow = lnkbtn.NamingContainer as GridViewRow;
            GridView gv = gvrow.Parent.NamingContainer as GridView;

            SPList plannerFragmentList = SPContext.Current.Web.Lists.TryGetList("PlannerFragments");
            Label lblId = (Label)gv.Rows[gvrow.RowIndex].FindControl("lblID");
            if (lblId != null)
            {
                SPListItem fragment = plannerFragmentList.GetItemById(Convert.ToInt32(lblId.Text));
                fragment.Recycle();
                plannerFragmentList.Update();
                if (gv.ID == "gridMyFragments")
                {
                    FillMyFragmentsGrid();
                }
                else
                {
                    FillPublicFragmentsGrid();
                }
            }
        }

        protected void grid_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //getting username from particular row
                string fragment = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Title"));
                //identifying the control in gridview
                LinkButton lnkDelete = (LinkButton)e.Row.FindControl("lnkdelete");
                string author = Convert.ToString(DataBinder.Eval(e.Row.DataItem, "Author"));
                if (string.Compare(author, SPContext.Current.Web.CurrentUser.Name) != 0)
                {
                    LinkButton lnkEdit = (LinkButton)e.Row.FindControl("lnkEdit");
                    lnkEdit.Visible = false;
                    lnkDelete.Visible = false;
                }
                else
                {
                    //raising javascript confirmationbox whenver user clicks on link button
                    lnkDelete.Attributes.Add("onclick", "javascript:return ConfirmationBox('" + fragment + "')");
                }
            }
        }
    }
}
