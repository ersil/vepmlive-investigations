﻿using System;
using System.Net;
using System.Web;
using EPMLiveCore;
using EPMLiveCore.Layouts.epmlive;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Drawing.Charts;
using System.Data;
using System.Text;
using System.Linq;
using EPMLiveCore.Jobs.SSRS;

namespace EPMLiveWebParts.Layouts.epmlive
{
    public partial class SSRSNativeReportViewer : LayoutsPageBase
    {
        private string webUrl = string.Empty;
        private string itemUrl = string.Empty;
        private bool isNativeMode = false;

        private string _reportingServicesUrl = EPMLiveCore.CoreFunctions.getWebAppSetting(SPContext.Current.Site.WebApplication.Id, "ReportingServicesURL");

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request["weburl"]))
            {
                webUrl = Request["weburl"];
            }

            if (!string.IsNullOrEmpty(Request["itemurl"]))
            {
                itemUrl = Request["itemurl"];
            }

            if (!string.IsNullOrEmpty(Request["isNativeMode"]))
            {
                bool.TryParse(Request["isNativeMode"], out isNativeMode);
            }
        }
        
        [WebMethod]
        public static string GetRegs()
        {
            var itemUrlRequest = HttpContext.Current.Request.QueryString["itemurl"];
            var reportURL = EPMLiveCore.CoreFunctions.getWebAppSetting(SPContext.Current.Site.WebApplication.Id, "ReportingServicesURL");
            var addresses = $"{$"{reportURL}/Pages/ReportViewer.aspx?{itemUrlRequest}&rs:Command=Render"}|reportbuilder:Action=Edit&ItemPath={itemUrlRequest}&Endpoint={reportURL}";
            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(addresses);
        }

        [WebMethod]
        public static string GetSubscriptions()
        {
            var itemUrlRequest = HttpContext.Current.Request.QueryString["itemurl"];
            var reportURL = EPMLiveCore.CoreFunctions.getWebAppSetting(SPContext.Current.Site.WebApplication.Id, "ReportingServicesURL");
            var addresses = $"{reportURL}/{itemUrlRequest}";

            var rs = ReportingService.GetInstance(SPContext.Current.Site);
            var subsList = rs.ListSubscriptions(itemUrlRequest);

            var ds = new DataSet("Subscriptions");
            System.Data.DataTable table = new System.Data.DataTable("Table");
            table.Columns.Add("Type");
            table.Columns.Add("DeliveryExtension");
            table.Columns.Add("Description");
            table.Columns.Add("Event");
            table.Columns.Add("LastRun");
            table.Columns.Add("SubsID");
            table.Columns.Add("Disabled");


            foreach (EPMLiveCore.SSRS2010.Subscription subsc in subsList)
            {
                if (SPContext.Current.Web.CurrentUser.LoginName.ToUpper().EndsWith(subsc.Owner.ToUpper()))
                    table.Rows.Add(subsc.EventType, subsc.DeliverySettings.Extension, subsc.Description, subsc.Status,
                        subsc.LastExecuted, subsc.SubscriptionID, subsc.Active.DisabledByUser);
            }
            //GetAddSubscriptionsFilters();
            ds.Tables.Add(table);
            return DataSetToJSON(ds);
        }

        [WebMethod]
        public static string GetDeliveryExtensions()
        {
            var rs = ReportingService.GetInstance(SPContext.Current.Site);
            var extensions = rs.ListExtensions("Delivery");
            string html = "<option value=\"choose\">Choose a method of delivery</option>";

            extensions?.ToList().ForEach(x => html += $"<option value\"{x.LocalizedName}\">{x.LocalizedName}</option>");

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(html);
        }

        [WebMethod]
        public static string GetReportParameters()
        {
            var itemUrlRequest = HttpContext.Current.Request.QueryString["itemurl"];
            var reportURL = EPMLiveCore.CoreFunctions.getWebAppSetting(SPContext.Current.Site.WebApplication.Id, "ReportingServicesURL");
            var addresses = $"{reportURL}/{itemUrlRequest}";
            var rs = ReportingService.GetInstance(SPContext.Current.Site);

            //string historyId = null;
            //ParameterValue[] parameterValues = null;
            //DataSourceCredentials[] datasourceCred = null;

            //all the parameters of the report will be filled to out parameter 'itemParameter'
            var itemParameters = rs.GetItemParameters(itemUrlRequest);
            StringBuilder htmlToLoad = new StringBuilder();
            htmlToLoad.Append("<table style=\"width:100%\"><tr><th>Parameter</th><th>Source of Value</th><th>Value/Field</th></tr>");

            //string fieldValueHtml = string.Empty;
            //iterate through parameters
            int i = 0;
            foreach (var ip in itemParameters)
            {
                htmlToLoad.Append("<tr>");

                htmlToLoad.Append($"<td id=\"FieldLabelID{i}\"><p>{ip.Prompt}</p></td>");

                htmlToLoad.Append($"<td><select {(ip.DefaultValues == null ? "disabled" : string.Empty)} id=\"EnterFieldID{i}\" name=\"EnterFieldID{i}\" onchange=\"EnterFieldChange('{i}')\">");
                htmlToLoad.Append($"<option value=\"enter\" {(ip.DefaultValues == null ? "selected" : string.Empty)}>Enter value</option>");
                htmlToLoad.Append($"<option value=\"default\" {(ip.DefaultValues != null ? "selected" : string.Empty)}>Use default value</option>");
                htmlToLoad.Append("</select></td>");

                htmlToLoad.Append("<td>");

                var isDefaultValue = false;
                switch (ip.ParameterTypeName.ToUpper())
                {
                    case "BOOLEAN":
                        htmlToLoad.Append($"<select {(ip.DefaultValues != null ? "disabled" : string.Empty)} {(ip.MultiValue ? "multiple" : string.Empty)} id=\"ValueFieldID{i}\" name=\"ValueFieldID{i}\">");

                        isDefaultValue = (ip.DefaultValues != null && ip.DefaultValues.ToList().Any(x => x != null && x.ToUpper() == "TRUE"));
                        htmlToLoad.Append($"<option value=\"true\" ");
                        htmlToLoad.Append(isDefaultValue ? "selected" : string.Empty);
                        htmlToLoad.Append(">True</option>");

                        isDefaultValue = (ip.DefaultValues != null && ip.DefaultValues.ToList().Any(x => x != null && x.ToUpper() == "FALSE"));
                        htmlToLoad.Append($"<option value=\"false\" ");
                        htmlToLoad.Append(isDefaultValue ? "selected" : string.Empty);
                        htmlToLoad.Append(">False</option>");

                        htmlToLoad.Append("</select>");
                        break;
                    case "STRING":
                    case "INTEGER":
                    case "FLOAT":
                    case "DATETIME":
                        if (ip.ValidValues != null)
                        {
                            htmlToLoad.Append($"<select {(ip.DefaultValues != null ? "disabled" : string.Empty)} {(ip.MultiValue ? "multiple" : string.Empty)} id=\"ValueFieldID{i}\" name=\"ValueFieldID{i}\">");
                            ip.ValidValues?.ToList().ForEach(x =>
                            {
                                isDefaultValue = (ip.DefaultValues != null && ip.DefaultValues.ToList().Any(defValue => defValue != null && defValue.ToUpper() == x.Value));

                                htmlToLoad.Append($"<option value=\"{x.Value}\" ");
                                htmlToLoad.Append(ip.DefaultValues != null && isDefaultValue ? "selected" : string.Empty);
                                htmlToLoad.Append($">{x.Value}</option>");
                            });
                            htmlToLoad.Append("</select>");
                        }
                        else
                        {
                            var defaultValues = string.Empty;
                            ip.DefaultValues?.ToList().ForEach(x => defaultValues += x + "||");
                            htmlToLoad.Append($"<input type=\"{(ip.ParameterTypeName.ToUpper() == "DATETIME" ? "datetime-local" : "text")}\" defaultValue=\"{defaultValues}\"/ id=\"ValueFieldID{i}\" name=\"ValueFieldID{i}\">");
                        }
                        break;
                }

                htmlToLoad.Append("</td>");

                htmlToLoad.Append("</tr>");

                i++;
            }

            htmlToLoad.Append("</table>");
            htmlToLoad.Append($"<div id=\"QtyParamsDiv\"><input type=\"hidden\" id=\"QtyParams\" name=\"QtyParams\" value =\"{ i }\" /></div>");

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(htmlToLoad.ToString());
        }

        [WebMethod]
        public static bool SaveSubscription()
        {
            //ScheduleDefinition sd = new ScheduleDefinition();
            //var rp = sd.Item;
            //rp.
            //ParameterValue parameters = null;
            //parameters.
            //ExtensionSettings extensionSettings = new ExtensionSettings();
            //extensionSettings.Extension = EXTENSION_FILESHARE;
            //extensionSettings.ParameterValues = extensionParams;

            return true;
        }

        [WebMethod]
        public static void EnableDisableSubscription(string subsIdList, bool enable)
        {
            var rs = ReportingService.GetInstance(SPContext.Current.Site);

            if (!string.IsNullOrWhiteSpace(subsIdList))
            {
                var subscriptionsList = subsIdList.Split('|').ToList();
                subscriptionsList?.Where(x => !string.IsNullOrEmpty(x))?.ToList().ForEach(x => {
                    if (enable)
                        rs.EnableSubscription(x);
                    else
                        rs.DisableSubscription(x);
                });
            }
        }

        [WebMethod]
        public static void DeleteSubscription(string subsIdList)
        {
            var rs = ReportingService.GetInstance(SPContext.Current.Site);

            if (!string.IsNullOrWhiteSpace(subsIdList))
            {
                var subscriptionsList = subsIdList.Split('|').ToList();
                subscriptionsList?.Where(x => !string.IsNullOrEmpty(x))?.ToList().ForEach(x => {
                    rs.DeleteSubscription(x);
                });
            }
        }

        public static string DataSetToJSON(DataSet ds)
        {

            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (System.Data.DataTable dt in ds.Tables)
            {
                object[] arr = new object[dt.Rows.Count + 1];

                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                {
                    arr[i] = dt.Rows[i].ItemArray;
                }

                dict.Add(dt.TableName, arr);
            }

            JavaScriptSerializer json = new JavaScriptSerializer();
            return json.Serialize(dict);
        }
    }
}
