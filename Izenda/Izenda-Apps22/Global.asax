<%@ Application Language="C#" %>
<%@ Import Namespace="Izenda.AdHoc" %>

<script RunAt="server">

    [Serializable]
    public class CustomAdHocConfig : DatabaseAdHocConfig
    {
        private string sitecollectionid=string.Empty;
        public CustomAdHocConfig(string _sitecollectionid)
        {
            if (string.IsNullOrEmpty(_sitecollectionid))
            {
                if (HttpContext.Current.Session["CurrentTenant"] != null)
                {
                    sitecollectionid = HttpContext.Current.Session["CurrentTenant"].ToString();
                }
            }
            else
            {
                sitecollectionid = _sitecollectionid;
                HttpContext.Current.Session["CurrentTenant"] = _sitecollectionid;
            }
        }

        public void InitializeReporting()
        {
            HttpSessionState session = HttpContext.Current.Session;
            if (session == null)
                return;



            AdHocSettings.LicenseKey = ConfigurationManager.AppSettings["IzendaLicenseKey"];
            AdHocSettings.GenerateThumbnails = true;
            AdHocSettings.DashboardViewer = "Dashboards.aspx";
            AdHocSettings.AdHocConfig = new CustomAdHocConfig(sitecollectionid);
            AdHocSettings.ChartLimit = 100;
            AdHocSettings.Formats.Add(DateTime.Now.ToString("dd/MM/yyyy"), "{0:dd/MM/yyyy}");
            AdHocSettings.Formats.Add(DateTime.Now.ToString("dd-MM-yyyy"), "{0:dd-MM-yyyy}");
            AdHocSettings.Formats.Add(DateTime.Now.ToString("MM-dd-yyyy"), "{0:MM-dd-yyyy}");
            AdHocSettings.ChartingEngine = ChartingEngine.HtmlChart;
        }

        public override void ProcessDataSet(System.Data.DataSet ds, string reportPart)
        {
            if (!string.IsNullOrEmpty(reportPart))
            {
                System.Collections.Generic.List<string> col = new System.Collections.Generic.List<string>();

                foreach (System.Data.DataRow dataRow in ds.Tables[0].Rows)
                {
                    foreach (System.Data.DataColumn item in ds.Tables[0].Columns)
                    {
                        TypeCode _typeCode = Type.GetTypeCode(item.DataType);

                        switch (_typeCode)
                        {
                            case TypeCode.Byte:
                            case TypeCode.SByte:
                            case TypeCode.Int16:
                            case TypeCode.UInt16:
                            case TypeCode.Int32:
                            case TypeCode.UInt32:
                            case TypeCode.Int64:
                            case TypeCode.UInt64:
                            case TypeCode.Single:
                            case TypeCode.Double:
                            case TypeCode.Decimal:
                                // Console.WriteLine("Numeric");
                                break;
                            case TypeCode.Boolean:
                                // Console.WriteLine("Bool");
                                break;
                            case TypeCode.DateTime:
                                break;
                            case TypeCode.String:
                                if (dataRow[item.ColumnName] != DBNull.Value)
                                    dataRow[item.ColumnName] = dataRow[item.ColumnName].ToString().Replace(";#", ";");
                                break;
                            case TypeCode.Empty:
                                //  Console.WriteLine("Null");
                                break;
                            default:    // TypeCode.DBNull, TypeCode.Char and TypeCode.Object
                                        // Console.WriteLine("Unknown");
                                break;
                        }

                    }

                }
            }
            base.ProcessDataSet(ds, reportPart);
        }

        public override void PreExecuteReportSet(ReportSet reportSet)
        {
            base.PreExecuteReportSet(reportSet);
        }

        public override void ConfigureSettings()
        {
            AdHocSettings.RequireLogin = false;
            AdHocSettings.LoginUrl = "Login.aspx";
            AdHocSettings.ShowModifyButton = true;
            AdHocSettings.ShowDesignLinks = true;

            AdHocSettings.CurrentUserIsAdmin = false;
            AdHocSettings.CurrentUserIsGlobalAdministrator = false;


            //AdHocSettings.DataCacheInterval=0;
            AdHocSettings.SharedWithValues = new string[] { "Report Writers", "Report Viewers" };
            AdHocSettings.DefaultSharingRights = "View Only";
            AdHocSettings.DefaultVisibilityForNonAdmins = "Report Viewers";
            AdHocSettings.GenerateThumbnails = true;

            if (HttpContext.Current.Session[sitecollectionid+"Role"] != null)
                AdHocSettings.CurrentUserRoles = new string[] { HttpContext.Current.Session[sitecollectionid+"Role"].ToString() };

            if (HttpContext.Current.Session[sitecollectionid+"webid"] != null)
                AdHocSettings.CurrentUserTenantId = HttpContext.Current.Session[sitecollectionid+"webid"].ToString();

            AdHocSettings.CurrentUserIsAdmin = false;
            AdHocSettings.CurrentUserIsGlobalAdministrator = false;

            if (HttpContext.Current.Session[sitecollectionid+"ConnectionString"] != null)
            {
                AdHocSettings.SqlServerConnectionString = HttpContext.Current.Session[sitecollectionid+"ConnectionString"].ToString();
                if (HttpContext.Current.Session[sitecollectionid+"StorageConnectionString"] != null)
                    SavedReportsDriver = new Izenda.AdHoc.Database.MSSQLDriver(HttpContext.Current.Session[sitecollectionid+"StorageConnectionString"].ToString());
            }

            if (HttpContext.Current.Session["UserName"] != null)
                AdHocSettings.CurrentUserName = HttpContext.Current.Session["UserName"].ToString();

            AdHocSettings.CurrentUserIsAdmin = false;
            AdHocSettings.CurrentUserIsGlobalAdministrator = false;
            AdHocSettings.ShowModifyButton = false;
            AdHocSettings.ShowDesignLinks = false;

            string pagename = System.IO.Path.GetFileName(HttpContext.Current.Request.PhysicalPath).ToLower();
            if (pagename != "default" && pagename != "timeout")
            {
                if (HttpContext.Current.Session["UserName"] == null)
                {
                    HttpContext.Current.Response.Redirect("timeout.aspx");
                }
            }
        }

        public override void PostLogin()
        {
            AdHocSettings.GenerateThumbnails = true;

            if (HttpContext.Current.Session[sitecollectionid+"Role"] != null)
                AdHocSettings.CurrentUserRoles = new string[] { HttpContext.Current.Session[sitecollectionid+"Role"].ToString() };

            if (HttpContext.Current.Session[sitecollectionid+"webid"] != null)
                AdHocSettings.CurrentUserTenantId = HttpContext.Current.Session[sitecollectionid+"webid"].ToString();

            AdHocSettings.CurrentUserIsAdmin = false;
            AdHocSettings.CurrentUserIsGlobalAdministrator = false;

            if (HttpContext.Current.Session["UserName"] != null)
                AdHocSettings.CurrentUserName = HttpContext.Current.Session["UserName"].ToString();

            if (HttpContext.Current.Session[sitecollectionid+"ConnectionString"] != null)
                AdHocSettings.SqlServerConnectionString = HttpContext.Current.Session[sitecollectionid+"ConnectionString"].ToString();

            if (HttpContext.Current.Session[sitecollectionid+"StorageConnectionString"] != null)
                SavedReportsDriver = new Izenda.AdHoc.Database.MSSQLDriver(HttpContext.Current.Session[sitecollectionid+"StorageConnectionString"].ToString());

            AdHocSettings.CurrentUserIsAdmin = false;
            AdHocSettings.CurrentUserIsGlobalAdministrator = false;
            AdHocSettings.ShowModifyButton = false;
            AdHocSettings.ShowDesignLinks = false;
        }
    }

</script>
