﻿using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Xml;
using EPMLiveCore.ReportHelper;
using Microsoft.SharePoint;

namespace EPMLiveCore
{
    public class ReportingData
    {
        public static DataSet GetReportingData(SPWeb web, string list, bool bRollup, string query, string orderby, int page, int pagesize)
        {

            var reportBiz = new ReportBiz(web.Site.ID);

            if (reportBiz.SiteExists())
            {
                var oList = web.Lists[list];
                EPMData data = null;
                SqlCommand sqlCommand = null;

                try
                {
                    data = new EPMData(web.Site.ID);
                    var clientReportingConnection = data.GetClientReportingConnection;
                    sqlCommand = new SqlCommand(
                        "select * from information_schema.parameters where specific_name='spGetReportListData' and parameter_name='@orderby'",
                        clientReportingConnection);

                    var borderby = false;
                    using (var dr = sqlCommand.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            borderby = true;
                        }
                    }

                    sqlCommand.Dispose();
                    sqlCommand = new SqlCommand("spGetReportListData", clientReportingConnection);
                    sqlCommand.CommandType = CommandType.StoredProcedure;
                    sqlCommand.Parameters.AddWithValue("@siteid", web.Site.ID);
                    sqlCommand.Parameters.AddWithValue("@webid", web.ID);
                    sqlCommand.Parameters.AddWithValue("@weburl", web.ServerRelativeUrl);
                    sqlCommand.Parameters.AddWithValue("@userid", web.CurrentUser.ID);
                    sqlCommand.Parameters.AddWithValue("@rollup", bRollup);
                    sqlCommand.Parameters.AddWithValue("@list", list);
                    sqlCommand.Parameters.AddWithValue("@query", query);
                    sqlCommand.Parameters.AddWithValue("@pagesize", pagesize);
                    sqlCommand.Parameters.AddWithValue("@page", page);
                    sqlCommand.Parameters.AddWithValue("@listid", oList.ID);

                    if (borderby)
                    {
                        sqlCommand.Parameters.AddWithValue("@orderby", orderby);
                    }

                    var dataSet = new DataSet();
                    using (var dataAdapter = new SqlDataAdapter(sqlCommand))
                    {
                        dataAdapter.Fill(dataSet);
                    }

                    return dataSet;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                    throw;
                }
                finally
                {
                    if (sqlCommand != null)
                    {
                        sqlCommand.Dispose();
                    }
                    if (data != null)
                    {
                        data.Dispose();
                    }
                }
            }
            else
            {
                throw new Exception("Reporting Not Setup.");
            }
        }

        public static DataTable GetReportingData(SPWeb web, string list, bool bRollup, string query, string orderby)
        {
            return GetReportingData(web, list, bRollup, query, orderby, 0, 0).Tables[0];

        }

        public static string GetReportQuery(SPWeb web, SPList list, string spquery, out string orderby)
        {
            orderby = "";

            if (spquery == "")
                return "";

            XmlDocument doc = new XmlDocument();
            doc.LoadXml("<Query>" + spquery + "</Query>");

            XmlNode ndWhere = doc.FirstChild.SelectSingleNode("//Where");

            XmlNode ndGroupBy = doc.FirstChild.SelectSingleNode("//OrderBy");

            if (ndGroupBy != null)
            {
                foreach (XmlNode nd in ndGroupBy.SelectNodes("FieldRef"))
                {

                    SPField oField = list.Fields.GetFieldByInternalName(nd.Attributes["Name"].Value);
                    if (oField.Type == SPFieldType.Lookup || oField.Type == SPFieldType.User)
                        orderby += "," + nd.Attributes["Name"].Value + "Text";
                    else
                        orderby += "," + nd.Attributes["Name"].Value;
                    try
                    {
                        if (nd.Attributes["Ascending"].Value.ToLower() == "false")
                            orderby += " DESC";
                    }
                    catch
                    {
                    }
                }
            }

            orderby = orderby.Trim(',');

            if (ndWhere == null)
                return "";

            string q = GetReportQueryNode(web, ndWhere.FirstChild, list);



            return q;

        }

        private static string GetReportQueryNode(SPWeb web, XmlNode nd, SPList list)
        {
            if (nd.Name == "And")
            {
                return "(" + GetReportQueryNode(web, nd.FirstChild, list) + " And " + GetReportQueryNode(web, nd.FirstChild.NextSibling, list) + ")";
            }
            else if (nd.Name == "Or")
            {
                return "(" + GetReportQueryNode(web, nd.FirstChild, list) + " Or " + GetReportQueryNode(web, nd.FirstChild.NextSibling, list) + ")";
            }
            else
            {
                string field = nd.SelectSingleNode("FieldRef").Attributes["Name"].Value;
                if (!string.IsNullOrEmpty(field))
                {
                    SPField oField = null;
                    try
                    {
                        oField = list.Fields.GetFieldByInternalName(field);
                    }
                    catch { }

                    if (oField != null)
                    {
                        //Lookups and User type of columns were created in database as ID and Text fields hence direct returning (column name is null) will not work. Fixed this.
                        if (nd.Name == "IsNull" && oField.Type != SPFieldType.Lookup && oField.Type != SPFieldType.User)
                        {
                            return field + " is null";
                        }
                        else
                        {
                            XmlNode ndVals = null;
                            try
                            {
                                ndVals = nd.SelectSingleNode("Values");
                            }
                            catch { }

                            if (ndVals != null)
                            {
                                string vals = "(";

                                if (oField.Type == SPFieldType.Lookup || oField.Type == SPFieldType.User)
                                {
                                    field += "Text";
                                }

                                if (ndVals.SelectNodes("Value").Count > 0)
                                {
                                    foreach (XmlNode ndVal in ndVals.SelectNodes("Value"))
                                    {
                                        vals += field + " = '" + ndVal.InnerText.Replace("'", "''") + "' OR ";
                                    }
                                }
                                else
                                {
                                    vals += field + " = ''";
                                }

                                return vals.Trim(' ').Trim('R').Trim('O') + ")";
                            }
                            else
                            {
                                string val = string.Empty;
                                // Avoiding NULL value when nothing is provided for filter value
                                if (nd.SelectSingleNode("Value") != null)
                                {
                                    val = nd.SelectSingleNode("Value").InnerText;
                                }

                                if (nd.SelectSingleNode("Value") != null && nd.SelectSingleNode("Value").SelectSingleNode("UserID") != null)
                                {
                                    val = web.CurrentUser.ID.ToString();
                                }

                                bool bLookup = false;
                                bool bcontaintoday = false;
                                SPFieldType fieldType = SPFieldType.Text;

                                switch (oField.Type)
                                {
                                    case SPFieldType.Calculated:
                                        {
                                            // Multiply by 100 if show as percentage true
                                            if (((SPFieldCalculated)oField).ShowAsPercentage)
                                            {
                                                val = Convert.ToString(double.Parse(val) * 100);
                                            }
                                        }
                                        break;
                                    case SPFieldType.DateTime:
                                        {
                                            //Initially the code developed use to check the dates based on Like operator also regardless of  sign and offset set through sharepoint
                                            //Corrected the code so that it works with sign and  offsetdays like [Today]-1 or [Today]-20 accordingly
                                            if (string.IsNullOrEmpty(val)
                                                && nd.SelectSingleNode("Value").InnerXml.Contains("Today"))
                                            {
                                                val = nd.SelectSingleNode("Value").InnerXml;
                                                bcontaintoday = true;
                                            }
                                        }
                                        break;
                                    case SPFieldType.Lookup:
                                        {
                                            try
                                            {
                                                bLookup = true;
                                                SPFieldLookup lookupField = oField as SPFieldLookup;
                                                SPList lookupList = lookupField.ParentList.ParentWeb.Lists[new Guid(lookupField.LookupList)];
                                                fieldType = lookupList.Fields.GetFieldByInternalName(lookupField.LookupField.ToString()).Type;
                                                // Check if field type is text / number
                                                if (fieldType.Equals(SPFieldType.Integer) || fieldType.Equals(SPFieldType.Counter))
                                                    field += "ID";
                                                // Need this condition while adding external task
                                                else if (nd.SelectSingleNode("Value") != null && nd.SelectSingleNode("Value").Attributes["Type"] != null
                                                    && (nd.SelectSingleNode("Value").Attributes["Type"].Value.Equals(SPFieldType.Counter.ToString())))
                                                    field += "ID";
                                                else
                                                    field += "Text";
                                            }
                                            catch
                                            {
                                                field += "Text";
                                            }
                                        }
                                        break;
                                    case SPFieldType.User:
                                        {
                                            bLookup = true;
                                            Int32 tempVal = 0;
                                            //If user has entered user id for filter then it will pass ID otherwise pass Text to CAML query / database while quering data and returning results.
                                            if (Int32.TryParse(Convert.ToString(val), out tempVal))
                                                field += "ID";
                                            else
                                                field += "Text";
                                        }
                                        break;
                                }

                                if (val.Contains("Today") && bcontaintoday == true)
                                {
                                    if (val.ToLower() == "[today]" || val.ToLower() == "<today />")
                                    {
                                        val = DateTime.Now.ToString("s");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Double offsetdays = Convert.ToDouble(nd.SelectSingleNode("//Value/Today").Attributes[0].Value);
                                            val = DateTime.Now.AddDays(offsetdays).ToString("s");
                                        }
                                        catch
                                        {
                                            val = DateTime.Now.ToString("s");
                                        }
                                    }
                                }

                                if (nd.Name == "BeginsWith")
                                {
                                    return field + " Like '" + val + "%'";
                                }
                                else if (nd.Name == "Contains")
                                {
                                    return field + " Like '%" + val + "%'";
                                }
                                else if (nd.Name == "Neq")
                                {
                                    if (oField.Type == SPFieldType.DateTime)
                                    {
                                        return "((CONVERT(nvarchar," + field + ", 111) <> CONVERT(nvarchar, CONVERT(DateTime, '" + val + "'), 111)) OR " + field + " IS NULL)";
                                    }
                                    return "(" + field + " <> '" + val + "' OR " + field + " IS NULL)";
                                }
                                else if (nd.Name == "IsNotNull")
                                {
                                    return field + " Is Not Null";
                                }
                                else if (nd.Name == "IsNull")
                                {
                                    return field + " Is Null";
                                }
                                else
                                {
                                    string sign = GetNodeSign(nd.Name);

                                    if (bLookup && sign == "=")
                                    {
                                        return "',' + CAST(" + field + " as varchar(MAX)) + ',' LIKE '%," + val + ",%'";
                                    }
                                    else if (oField.Type.Equals(SPFieldType.DateTime))
                                    {
                                        // Need to use actual sign operator to match only the date part 
                                        return "CONVERT(nvarchar, " + field + ", 111) " + sign + " CONVERT(nvarchar, CONVERT(DateTime, '" + val + "'), 111)";
                                    }
                                    else
                                    {
                                        return field + " " + sign + " '" + val + "'";
                                    }
                                }
                            }
                        }
                    }
                }
                return string.Empty;
            }
        }

        public static string GetNodeSign(string name)
        {
            switch (name.ToLower())
            {
                case "eq":
                    return "=";
                case "neq":
                    return "<>";
                case "gt":
                    return ">";
                case "geq":
                    return ">=";
                case "lt":
                    return "<";
                case "leq":
                    return "<=";
                case "beginswith":
                    return "";
                case "contains":
                    return "";
                default:
                    return "=";
            }
        }

        // TEST //
        public static string ProcessReportFilter(SPList list, SPWeb web, string filterWpId)
        {
            string ret = "";
            ArrayList reportFilterIDs = new ArrayList();
            Guid listid = Guid.Empty;
            string reportFilterField = "";
            int MAX_LOOKUPFILTER = 300;

            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                SqlConnection connection = null;
                SqlCommand command = null;

                try
                {
                    connection = new SqlConnection(CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                    connection.Open();

                    command = new SqlCommand(
                        "SELECT VALUE,listid FROM PERSONALIZATIONS where userid=@userid and [key]=@key and FK=@FK",
                        connection);
                    command.Parameters.AddWithValue("@userid", web.CurrentUser.ID);
                    command.Parameters.AddWithValue("@key", "ReportFilterWebPartSelections");
                    command.Parameters.AddWithValue("@FK", filterWpId);
                    command.ExecuteNonQuery();
                    using (var dataReader = command.ExecuteReader())
                    {
                        if (dataReader.Read())
                        {
                            reportFilterIDs = new ArrayList(dataReader.GetString(0).Split('|')[0].Split(','));
                            listid = dataReader.GetGuid(1);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.ToString());
                }
                finally
                {
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
            });

            if (listid == list.ID)
            {
                reportFilterField = "Title";

                if (reportFilterIDs.Count < MAX_LOOKUPFILTER)
                {

                    ret = "<In><FieldRef Name=\"Title\"/><Values>" + GetReportFilters(reportFilterIDs) + "</Values></In>";

                }
            }
            else if (listid != Guid.Empty)
            {



                foreach (SPField oField in list.Fields)
                {
                    if (oField.Type == SPFieldType.Lookup)
                    {
                        try
                        {
                            SPFieldLookup oLookup = (SPFieldLookup)oField;
                            if (new Guid(oLookup.LookupList) == listid)
                            {
                                reportFilterField = oLookup.InternalName;
                                break;
                            }
                        }
                        catch { }
                    }
                }


                if (reportFilterIDs.Count < MAX_LOOKUPFILTER && reportFilterField != "")
                {
                    ret = "<In><FieldRef Name=\"" + reportFilterField + "\"/><Values>" + GetReportFilters(reportFilterIDs) + "</Values></In>";
                }
            }


            return ret;
        }

        private static string GetReportFilters(ArrayList reportFilterIDs)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in reportFilterIDs)
            {
                sb.Append("<Value Type=\"Text\">");
                sb.Append(System.Web.HttpUtility.HtmlEncode(s));
                sb.Append("</Value>");
            }

            return sb.ToString();
        }
    }
}
