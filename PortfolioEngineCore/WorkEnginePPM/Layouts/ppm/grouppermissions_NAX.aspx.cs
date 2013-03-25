﻿using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.SharePoint.WebControls;
using PortfolioEngineCore;
using WorkEnginePPM.ControlTemplates.WorkEnginePPM;

namespace WorkEnginePPM.Layouts.ppm
{
    public partial class grouppermissions_NAX : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            DBAccess dba = null;
            try
            {
                DGrid dg = dgrid1;
                string sBaseInfo = WebAdmin.BuildBaseInfo(Context);
                DataAccess da = new DataAccess(sBaseInfo, SecurityLevels.BaseAdmin);
                dba = da.dba;

                if (dba.Open() == StatusEnum.rsSuccess)
                {
                    dg.AddColumn(title: "ID", width: 50, name: "GROUP_ID", isId: true, hidden: true);
                    dg.AddColumn(title: "Name", width: 200, name: "GROUP_NAME");
                    dg.AddColumn(title: "Description", width: 360, name: "GROUP_NOTES");

                    DataTable dt;
                    dbaGroups.SelectGroups(dba, out dt);

                    dba.Close();
                    dg.SetDataTable(dt);
                    dg.Render();
                }
            }
            catch (PFEException pex)
            {
                if (pex.ExceptionNumber == 9999)
                    Response.Redirect(this.Site.Url + "/_layouts/ppm/NoPerm.aspx?requesturl='" + Request.RawUrl + "'");
                lblGeneralError.Text = "PFE Exception : " + pex.ExceptionNumber.ToString() + " - " + pex.Message;
                lblGeneralError.Visible = true;
            }
            catch (Exception ex)
            {
                lblGeneralError.Text = ex.Message;
                lblGeneralError.Visible = true;
            }
            finally
            {
                if (dba != null)
                    dba.Close();
            }
        }
    }
}
