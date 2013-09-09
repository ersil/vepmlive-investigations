﻿using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using System.Data.SqlClient;

namespace EPMLiveCore.Layouts.epmlive
{
    public partial class izendareporting : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                string username = CoreFunctions.GetJustUsername(SPContext.Current.Web.CurrentUser.LoginName);
                Guid gAuth = Guid.NewGuid();

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {

                    SqlConnection cn = new SqlConnection(CoreFunctions.getConnectionString(SPContext.Current.Site.WebApplication.Id));
                    cn.Open();

                    SqlCommand cmd = new SqlCommand("spAddAuth", cn);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@authid", gAuth);
                    cmd.Parameters.AddWithValue("@username", username);
                    cmd.Parameters.AddWithValue("@email", SPContext.Current.Web.CurrentUser.Email);
                    cmd.ExecuteNonQuery();


                    cn.Close();

                });

                string b64 = EncodeTo64(CoreFunctions.getWebAppSetting(SPContext.Current.Site.WebApplication.Id, "epmliveapiurl") + "/integration.asmx`" + gAuth);

                string url = "https://reports.epmlive.com/?dbid=" + SPContext.Current.Site.WebApplication.Id + "&siteid=" + SPContext.Current.Site.ID + "&authid=" + b64;

                string rn = "";
                try { rn = Request["rn"].ToString(); }
                catch { }

                if (rn != "")
                    url += "&rn=" + rn;

                Response.Redirect(url + "&InFrame=1");
            }
            catch (Exception ex)
            {
                Response.Write(ex.Message);
            }
        }

        private string EncodeTo64(string toEncode)
        {

            byte[] toEncodeAsBytes

                  = System.Text.ASCIIEncoding.ASCII.GetBytes(toEncode);

            string returnValue

                  = System.Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;

        }
    }
}
