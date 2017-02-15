﻿using EPMLive.OnlineLicensing.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AdminSite
{
    public partial class renewlicense : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnRenew_Click(object sender, EventArgs e)
        {
            var newExpirationDate = TxtExpirationDate.Value;
            var orderId = "A0CD98F9-7DDD-4F15-886B-761AB7183E4D";

            using (var license = new LicenseManager())
            {
                if (!license.ValidateNewLicenseExtension(Guid.Parse(orderId), DateTime.Parse(newExpirationDate)))
                {
                    ShowErrorMessage("The new license expiration date should be greater than the previous.");
                    return;
                }

                license.RenewLicense(Guid.Parse(orderId), DateTime.Parse(newExpirationDate));

                var script = $@"
                        <script>
                            parent.location.href='editaccount.aspx?account_id={Request["accountId"]}&tab=4';
                        </script>
                    ";

                ClientScript.RegisterStartupScript(this.GetType(), "RenewLicense", script);
            }
        }

        private void ShowErrorMessage(string message)
        {
            errorLabel.InnerText = message;
            errorLabel.Visible = true;
        }
    }
}