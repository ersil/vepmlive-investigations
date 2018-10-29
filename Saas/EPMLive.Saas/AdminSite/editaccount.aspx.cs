﻿using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Text;
using EPMLive.OnlineLicensing.Api;

namespace AdminSite
{
    public partial class editaccount : System.Web.UI.Page
    {
        private const string AccountIdParameter = "@account_id";

        private const string GetAccountInfoQuery =
            "SELECT ACCOUNT.secondary_owner_id, ACCOUNT.lockusers, ACCOUNT.maxusers, ACCOUNT.crmaccountuid, ACCOUNT.account_id, ACCOUNT.billingType, ACCOUNT.account_ref, ACCOUNT.monthsfree, " +
            "ACCOUNT.dtCreated, ACCOUNT.accountDescription, ACCOUNT.partnerid, ACCOUNT.dedicated, PrimaryOwner.uid, PrimaryOwner.dtcreated AS Expr1, PrimaryOwner.username, PrimaryOwner.isBilled, " +
            "PrimaryOwner.createdBy, PrimaryOwner.tempPassword, PrimaryOwner.firstName, PrimaryOwner.lastName, PrimaryOwner.company, PrimaryOwner.title, PrimaryOwner.department, PrimaryOwner.phone, " +
            "PrimaryOwner.fax, PrimaryOwner.email, PrimaryOwner.activated, PrimaryOwner.registered, PrimaryOwner.enabled, PrimaryOwner.address1, PrimaryOwner.address2, PrimaryOwner.city, PrimaryOwner.state, " +
            "PrimaryOwner.zip, PrimaryOwner.country, PrimaryOwner.region, PrimaryOwner.version, PrimaryOwner.donotemail, PrimaryOwner.lastcontactdate, PrimaryOwner.epmliveadmin, PrimaryOwner.notes, " +
            "PrimaryOwner.publisher, PrimaryOwner.CrmUid, BETA1Survey.project_version, BETA1Survey.sharepoint_version, BETA1Survey.products, BETA1Survey.comments, " +
            "SecondaryOwner.username AS secondaryOwner_username, SecondaryOwner.firstName AS secondaryOwner_firstName, SecondaryOwner.lastName AS secondaryOwner_lastName, " +
            "SecondaryOwner.email AS secondaryOwner_email, SecondaryOwner.company AS secondaryOwner_company, SecondaryOwner.title AS secondaryOwner_title, " +
            "SecondaryOwner.department AS secondaryOwner_department, SecondaryOwner.phone AS secondaryOwner_phone " +
            "FROM USERS AS PrimaryOwner INNER JOIN " +
            "ACCOUNT ON PrimaryOwner.uid = ACCOUNT.owner_id LEFT OUTER JOIN " +
            "USERS AS SecondaryOwner ON ACCOUNT.secondary_owner_id = SecondaryOwner.uid LEFT OUTER JOIN " +
            "BETA1Survey ON PrimaryOwner.uid = BETA1Survey.uid " +
            "WHERE (ACCOUNT.account_id = " + AccountIdParameter + ")";

        protected string strAccountId;
        protected string strTab;
        protected string strUid;

        protected string secondaryOwnerUid;

        protected int TotalTicketUsage;
        protected int TotalTickets;
        protected StringBuilder sbOrders = new StringBuilder();
        protected readonly StringBuilder sbActiveLicenses = new StringBuilder();
        protected readonly StringBuilder sbInactiveLicenses = new StringBuilder();
        protected bool usingNewBilling = true;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                strTab = Request["tab"].ToString();
            }
            catch { strTab = ""; }
            if (strTab == "")
                strTab = "1";

            if (!IsPostBack)
                fillGrid();
        }

        private string ordertabletop(bool isPE)
        {
            StringBuilder sbT = new StringBuilder();
            sbT.Append("<table width=\"100%\" border=\"0\" cellpadding=\"3\" style=\"border: #dedfde 1px solid; background-color: white; width: 100%;\" ><tr style=\"background-color: #6b696b; color: white; font-weight: bold;\"><td>Billing System</td><td>Ref #</td><td>Exp/Renewal Date</td><td>Users</td><td>Storage</td>");
            if (isPE)
                sbT.Append("<td>Projects</td>");

            sbT.Append("<td>Actions</td></tr>");

            return sbT.ToString();
        }

        private string ordertablebottom()
        {
            return "</table>";
        }

        private void fillGrid()
        {

            strAccountId = Request["account_id"];

            SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["epmlive"].ToString());
            cn.Open();

            var sqlCommand = new SqlCommand(GetAccountInfoQuery, cn) { CommandType = CommandType.Text };
            sqlCommand.Parameters.AddWithValue(AccountIdParameter, strAccountId);

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            da.Fill(ds);

            DataRow dr = ds.Tables[0].Rows[0];

            lblUsername.Text = dr["username"].ToString();
            lblEditEmail.Text = dr["email"].ToString();
            txtEditCompany.Text = dr["company"].ToString();
            txtEditDepartment.Text = dr["department"].ToString();
            txtEditFirstName.Text = dr["firstName"].ToString();
            txtEditLastName.Text = dr["lastName"].ToString();
            txtEditPhone.Text = dr["phone"].ToString();
            txtEditTitle.Text = dr["title"].ToString();

            secondaryOwnerUserNameField.Text = dr["secondaryOwner_username"].ToString();
            secondaryOwnerEmailField.Text = dr["secondaryOwner_email"].ToString();
            secondaryOwnerCompanyField.Text = dr["secondaryOwner_company"].ToString();
            secondaryOwnerDepartmentField.Text = dr["secondaryOwner_department"].ToString();
            secondaryOwnerFirstNameField.Text = dr["secondaryOwner_firstName"].ToString();
            secondaryOwnerLastNameField.Text = dr["secondaryOwner_lastName"].ToString();
            secondaryOwnerPhoneField.Text = dr["secondaryOwner_phone"].ToString();
            secondaryOwnerTitleField.Text = dr["secondaryOwner_title"].ToString();

            bool lockusers;
            bool.TryParse(dr["lockusers"].ToString(), out lockusers);
            chkLockUsers.Checked = lockusers;


            chkDedicated.Checked = dr["dedicated"].ToString() == "True";


            txtDesc.Text = dr["accountdescription"].ToString();
            txtPartnerId.Text = dr["partnerid"].ToString();

            lblAccountRef.Text = dr["account_ref"].ToString();

            txtMonthsFree.Text = dr["monthsfree"].ToString();
            txtTrialUsers.Text = dr["maxusers"].ToString();

            DateTime dtCreated = Convert.ToDateTime("1/1/1900");
            int months = 1;
            int.TryParse(dr["monthsfree"].ToString(), out months);

            try
            {
                dtCreated = Convert.ToDateTime(dr["dtcreated"].ToString());
            }
            catch { }

            lblExpiration.Text = dtCreated.AddMonths(months).ToShortDateString();

            ddlBillingType.SelectedValue = dr["billingType"].ToString();

            sqlCommand = new SqlCommand("Select * from CONTRACTLEVEL_TITLES", cn);
            sqlCommand.CommandType = CommandType.Text;

            FillLicensesTab();

            //======================================================================


            sqlCommand = new SqlCommand("SELECT * from vwOwnedSites where account_id='" + Request["account_id"] + "'", cn);
            sqlCommand.CommandType = CommandType.Text;
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            gvSitesOwn.DataSource = ds;
            gvSitesOwn.DataBind();

            sqlCommand = new SqlCommand("SELECT sitename,siteurl,timecreated from ACCOUNT_EXTERNAL_SITES where account_id='" + Request["account_id"] + "'", cn);
            sqlCommand.CommandType = CommandType.Text;
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            gvExternalSites.DataSource = ds;
            gvExternalSites.DataBind();

            sqlCommand = new SqlCommand("SELECT * from vwSitesIAmIn where uid='" + dr["uid"] + "'", cn);
            sqlCommand.CommandType = CommandType.Text;
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            gvSites.DataSource = ds;
            gvSites.DataBind();


            sqlCommand = new SqlCommand("SELECT * from vwAccountUsers where account_id='" + Request["account_id"] + "' and deleted is null", cn);
            sqlCommand.CommandType = CommandType.Text;
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            gvUsers.DataSource = ds;
            gvUsers.DataBind();

            TotalTicketUsage = ds.Tables[0].Rows.Count;

            sqlCommand = new SqlCommand("SELECT sum(quantity) from orders where account_ref='" + dr["account_ref"] + "' and version = 6", cn);
            sqlCommand.CommandType = CommandType.Text;
            SqlDataReader drTickets = sqlCommand.ExecuteReader();

            if (drTickets.Read() && !drTickets.IsDBNull(0))
                TotalTickets = drTickets.GetInt32(0);
            else
                TotalTickets = 0;

            drTickets.Close();


            sqlCommand = new SqlCommand("select * from enterprise_sites where owner=@owner", cn);
            sqlCommand.Parameters.AddWithValue("@owner", dr["uid"].ToString());
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            foreach (DataRow drSite in ds.Tables[0].Rows)
            {

                sqlCommand = new SqlCommand("spGetEnterpriseUsers", cn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@euid", drSite["uid"].ToString());
                da = new SqlDataAdapter(sqlCommand);
                ds = new DataSet();
                da.Fill(ds);

                Label lbl = new Label();
                lbl.Text = "<br /><b>Project Server Users (" + drSite["url"].ToString() + ")</b><br />";
                pnlEntUsers.Controls.Add(lbl);

                GridView gv = new GridView();
                gv.Width = new Unit(96, UnitType.Percentage);
                gv.RowStyle.HorizontalAlign = HorizontalAlign.Left;
                gv.AutoGenerateColumns = false;
                gv.CellPadding = 4;
                gv.ForeColor = System.Drawing.Color.Black;
                gv.GridLines = GridLines.Vertical;
                gv.BackColor = System.Drawing.Color.White;
                gv.BorderColor = System.Drawing.ColorTranslator.FromHtml("#DEDFDE");
                gv.BorderWidth = new Unit(1, UnitType.Pixel);
                gv.FooterStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#CCCC99");
                gv.RowStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#F7F7DE");
                gv.HeaderStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#6B696B");
                gv.HeaderStyle.Font.Bold = true;
                gv.HeaderStyle.ForeColor = System.Drawing.Color.White;
                gv.AlternatingRowStyle.BackColor = System.Drawing.Color.White;

                BoundField bf = new BoundField();
                bf.DataField = "name";
                bf.HeaderText = "Name";
                gv.Columns.Add(bf);

                BoundField bf2 = new BoundField();
                bf2.DataField = "email";
                bf2.HeaderText = "Email";
                gv.Columns.Add(bf2);

                BoundField bf3 = new BoundField();
                bf3.DataField = "username";
                bf3.HeaderText = "Username";
                gv.Columns.Add(bf3);

                BoundField bf4 = new BoundField();
                bf4.DataField = "dtadded";
                bf4.HeaderText = "Added";
                bf4.DataFormatString = "{0:d}";
                gv.Columns.Add(bf3);

                TemplateField tf = new TemplateField();
                tf.HeaderText = "View";
                tf.ItemTemplate = new DynamicGridViewURLTemplate("uid", DataControlRowType.DataRow);
                gv.Columns.Add(tf);

                //gv.DataSource = ds.Tables[0];
                //gv.DataBind();

                //pnlEntUsers.Controls.Add(gv);

            }

            ddlBillingType.Enabled = false;


            sqlCommand = new SqlCommand("SP_GetAccountLog", cn);
            sqlCommand.CommandType = CommandType.StoredProcedure;
            sqlCommand.Parameters.AddWithValue("@accountid", Request["account_id"]);
            da = new SqlDataAdapter(sqlCommand);
            ds = new DataSet();
            da.Fill(ds);

            gvLog.DataSource = ds.Tables[0];
            gvLog.DataBind();


            strUid = dr["uid"].ToString();

            secondaryOwnerUid = string.Empty;
            if (!dr.IsNull("secondary_owner_id") && (Guid) dr["secondary_owner_id"] != Guid.Empty)
            {
                secondaryOwnerUid = dr["secondary_owner_id"].ToString();
            }

            cn.Close();
        }

        /// <summary>
        /// Fills the licenses tab with active licenses
        /// </summary>
        private void FillLicensesTab()
        {
            using (var licenseManager = new LicenseManager())
            {
                var accountManager = new AccountManager();
                var accountRef = accountManager.GetAccountReference(Guid.Parse(Request["account_id"]));
                GridViewActiveLicenses.DataSource = licenseManager.GetAllActiveLicenses(accountRef);
                GridViewActiveLicenses.DataBind();
            }
        }

        protected void gvTickets_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["epmlive"].ToString());

                cn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM ACCOUNT_TICKETS WHERE account_id='" + Request["account_id"] + "' and ticket_number='" + e.CommandArgument + "'", cn);
                cmd.ExecuteNonQuery();

                cn.Close();

                Response.Redirect("editaccount.aspx?account_id=" + Request["account_id"] + "&tab=8");

            }
        }

        protected MSCRM.salesorder getOrder(MSCRM.CrmService msCrm, Guid orderid)
        {
            MSCRM.ColumnSet cols = new MSCRM.ColumnSet();

            cols.Attributes = new string[] { "name", "salesorderid", "customerid", "new_ordertype", "ordernumber", "new_monthstobill", "new_recurring" };

            MSCRM.salesorder c = (MSCRM.salesorder)msCrm.Retrieve(MSCRM.EntityName.salesorder.ToString(), orderid, cols);

            return c;
        }

        protected MSCRM.BusinessEntity[] findInvoices(MSCRM.CrmService msCrm, string accountid)
        {
            try
            {
                // Create the ColumnSet that indicates the properties to be retrieved.
                MSCRM.ColumnSet cols = new MSCRM.ColumnSet();

                // Set the properties of the ColumnSet.
                cols.Attributes = new string[] { "name", "totalamount", "invoicenumber", "createdon", "salesorderid", "new_plimusreference" };

                MSCRM.ConditionExpression condition = new MSCRM.ConditionExpression();
                condition.AttributeName = "customerid";
                condition.Operator = MSCRM.ConditionOperator.Equal;
                condition.Values = new string[] { accountid };

                // Create the FilterExpression.
                MSCRM.FilterExpression filter = new MSCRM.FilterExpression();
                filter.FilterOperator = MSCRM.LogicalOperator.And;
                filter.Conditions = new MSCRM.ConditionExpression[] { condition };

                // Create the QueryExpression object.
                MSCRM.QueryExpression query = new MSCRM.QueryExpression();

                // Set the properties of the QueryExpression object.
                query.EntityName = MSCRM.EntityName.invoice.ToString();
                query.ColumnSet = cols;
                query.Criteria = filter;

                // Retrieve the contacts.
                MSCRM.BusinessEntityCollection invoices = msCrm.RetrieveMultiple(query);

                return invoices.BusinessEntities;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                Response.Write("Error Finding Orders: " + ex.Message + ex.Detail.OuterXml);
            }
            return null;
        }

        protected MSCRM.account getAccount(MSCRM.CrmService msCrm, Guid accountid)
        {
            MSCRM.ColumnSet cols = new MSCRM.ColumnSet();

            cols.Attributes = new string[] { "name", "primarycontactid" };

            MSCRM.account c = (MSCRM.account)msCrm.Retrieve(MSCRM.EntityName.account.ToString(), accountid, cols);

            return c;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pnlEditFailure.Visible = false;

                SqlConnection cn = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["epmlive"].ToString());
                cn.Open();

                SqlCommand cmdUpdateUser;

                //cmdUpdateUser = new SqlCommand("UPDATE USERS set firstName=@first, lastName=@last, company=@company, title=@title, department=@department,phone=@phone WHERE uid like '" + Request["uid"] + "'", cn);
                //cmdUpdateUser.CommandType = CommandType.Text;
                //cmdUpdateUser.Parameters.AddWithValue("@first", txtEditFirstName.Text);
                //cmdUpdateUser.Parameters.AddWithValue("@last", txtEditLastName.Text);
                //cmdUpdateUser.Parameters.AddWithValue("@company", txtEditCompany.Text);
                //cmdUpdateUser.Parameters.AddWithValue("@title", txtEditTitle.Text);
                //cmdUpdateUser.Parameters.AddWithValue("@department", txtEditDepartment.Text);
                //cmdUpdateUser.Parameters.AddWithValue("@phone", txtEditPhone.Text);

                //cmdUpdateUser.ExecuteNonQuery();

                cmdUpdateUser = new SqlCommand("UPDATE ACCOUNT set monthsfree=@monthsfree,maxusers=@maxusers,partnerid=@partnerid,dedicated=@dedicated,lockusers=@lockusers,accountdescription=@accountdescription WHERE account_ref = " + lblAccountRef.Text, cn);
                cmdUpdateUser.CommandType = CommandType.Text;
                cmdUpdateUser.Parameters.AddWithValue("@monthsfree", txtMonthsFree.Text);
                cmdUpdateUser.Parameters.AddWithValue("@maxusers", txtTrialUsers.Text);
                cmdUpdateUser.Parameters.AddWithValue("@dedicated", chkDedicated.Checked);
                cmdUpdateUser.Parameters.AddWithValue("@lockusers", chkLockUsers.Checked);
                cmdUpdateUser.Parameters.AddWithValue("@accountdescription", txtDesc.Text);

                if (txtPartnerId.Text != "")
                    cmdUpdateUser.Parameters.AddWithValue("@partnerid", txtPartnerId.Text);
                else
                    cmdUpdateUser.Parameters.AddWithValue("@partnerid", DBNull.Value);

                cmdUpdateUser.ExecuteNonQuery();




                cn.Close();

                pnlEditSuccess.Visible = true;

                Response.Redirect("editaccount.aspx?account_id=" + Request["account_id"]);
            }
            catch (Exception ex)
            {
                pnlEditFailure.Visible = true;
                lblError.Text = ex.Message.ToString() + "<br>" + ex.StackTrace;
            }

        }
        protected void btnCancel_Click(object sender, EventArgs e)
        {

        }

        public class DynamicGridViewURLTemplate : ITemplate
        {

            string _uid;

            DataControlRowType _rowType;

            public DynamicGridViewURLTemplate(string uid, DataControlRowType RowType)
            {

                _rowType = RowType;

                _uid = uid;

            }

            public void InstantiateIn(System.Web.UI.Control container)
            {

                switch (_rowType)
                {

                    case DataControlRowType.Header:

                        Literal lc = new Literal();

                        lc.Text = "<b>View</b>";

                        container.Controls.Add(lc);

                        break;

                    case DataControlRowType.DataRow:

                        HyperLink hpl = new HyperLink();

                        hpl.DataBinding += new EventHandler(this.hpl_DataBind);

                        container.Controls.Add(hpl);

                        break;

                    default:

                        break;

                }

            }



            private void hpl_DataBind(Object sender, EventArgs e)
            {

                HyperLink hpl = (HyperLink)sender;

                GridViewRow row = (GridViewRow)hpl.NamingContainer;

                hpl.NavigateUrl = "edituser.aspx?uid=" + DataBinder.Eval(row.DataItem, _uid).ToString();

                hpl.Text = "View User";

            }
        }
    }
}