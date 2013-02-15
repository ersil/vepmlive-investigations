﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Security.Principal;
using System.ComponentModel;
using EPMLiveReportsAdmin.Properties;
using System.Data.SqlClient;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace EPMLiveReportsAdmin.Layouts.EPMLive 
{
    public partial class AllSnapshots : LayoutsPageBase, IPostBackEventHandler
    {
        EPMData _DAO;
        protected SPGridView grdVwSnapshots;

        protected void Page_Init(object sender, EventArgs e)
        {
            _DAO = new EPMData(SPContext.Current.Web.Site.ID);
            LoadSnapshots();
        }

        protected void LoadSnapshots()
        {
            _DAO.Command = "SELECT Enabled as [Active], Title as [Report Title], PeriodDate as [Reporting Period], DateArchived as [Snapshot Date], ListNames as [Lists],periodid,siteid FROM RPTPeriods";
            DataTable dt = _DAO.GetTable(_DAO.GetClientReportingConnection);
            SPBoundField gridColumn;

            if (!IsPostBack)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    gridColumn = new SPBoundField();
                    gridColumn.HeaderText = column.ColumnName;
                    gridColumn.DataField = column.ColumnName;

                    if (column.ColumnName.ToLower().EndsWith("id"))
                    {
                        gridColumn.Visible = false;
                    }
                    grdVwSnapshots.Columns.Add(gridColumn);
                }
                grdVwSnapshots.DataSource = dt;
                grdVwSnapshots.DataBind();
            }
        }

        public void RaisePostBackEvent(string snapshotuid)
        {
            SPUtility.Redirect("epmlive/Snapshot.aspx?uid=" + snapshotuid, SPRedirectFlags.RelativeToLayoutsPage, HttpContext.Current);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Expires = -1;
            MaintainScrollPositionOnPostBack = true;
        }

        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            _DAO.Dispose();
        }
    }
}
