﻿using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using PortfolioEngineCore;
using WorkEnginePPM.Layouts.ppm;

namespace WorkEnginePPM
{

    public partial class RPCustomFieldsHandler : IHttpHandler, System.Web.SessionState.IRequiresSessionState
    {
        #region IHttpHandler Members

        public bool IsReusable
        {
            get { return false; }
        }

        public void ProcessRequest(HttpContext context)
        {
            string s = "";
            const string this_class = "WorkEnginePPM.RPCustomFieldsHandler";
            StreamReader sr = new StreamReader(context.Request.InputStream);
            string sRequest = sr.ReadToEnd();
            try
            {
                context.Server.ScriptTimeout = 86400;
                s = WebAdmin.CheckRequest(context, this_class, sRequest);
                if (s == "")
                {
                    CStruct xPageRequest = new CStruct();
                    if (xPageRequest.LoadXML(sRequest))
                    {
                        string sFunction = xPageRequest.GetStringAttr("function");
                        string sRequestContext = xPageRequest.GetStringAttr("context");
                        try
                        {
                            Assembly assemblyInstance = Assembly.GetExecutingAssembly();
                            Type thisClass = assemblyInstance.GetType(this_class, true, true);
                            MethodInfo m = thisClass.GetMethod(sFunction);
                            CStruct xData = xPageRequest.GetSubStruct("data");
                            object result = m.Invoke(null, new object[] { context, sRequestContext, xData });
                            s = WebAdmin.BuildReply(this_class, sFunction, sRequestContext, result.ToString());
                        }
                        catch (Exception ex)
                        {
                            s = WebAdmin.BuildReply(this_class, sFunction, sRequestContext, WebAdmin.FormatError("exception", this_class + ".ProcessRequest", ex.Message, "1"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                s = WebAdmin.BuildReply(this_class, this_class + ".ProcessRequest", sRequest, WebAdmin.FormatError("exception", this_class + ".ProcessRequest", ex.Message, "1"));
            }
            context.Response.ContentType = "text/xml; charset=utf-8";
            context.Response.Write(CStruct.ConvertXMLToJSON(s));
        }

        public static string RPCustomFieldsRequest(HttpContext Context, string sRequestContext, CStruct xData)
        {
            var reply = string.Empty;
            try
            {
                reply = CustomFieldHelper.CreateCustomFieldRequest(
                    Context,
                    sRequestContext,
                    xData,
                    ReadCustomFieldInfo,
                    UpdateCustomFieldInfo,
                    DeleteCustomFieldInfo);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                reply = WebAdmin.FormatError("exception", "RPCustomFields.CustomfieldRequest", ex.Message);
            }

            return reply;
        }

        private static void InitializeColumns(_TGrid grid)
        {
            CustomFieldHelper.InitializeColumns(grid);
        }

        private static string ReadLookup(DBAccess dbAccess, int lookupuId)
        {
            return CustomFieldHelper.ReadLookup(dbAccess, lookupuId);
        }

        private static string ReadCustomFieldInfo(DBAccess dba, int nFieldId)
        {
            string sReply = "";
            CStruct xCustomfield = new CStruct();
            xCustomfield.Initialize("customfield");
            DataTable dt;
            dbaResourcePlanning.SelectCustomField(dba, nFieldId, out dt);
            int lookupuid = 0;
            if (dt.Rows.Count == 1)
            {
                DataRow row = dt.Rows[0];
                xCustomfield.CreateInt("FA_FIELD_ID", DBAccess.ReadIntValue(row["FA_FIELD_ID"]));
                xCustomfield.CreateString("FA_NAME", DBAccess.ReadStringValue(row["FA_NAME"], ""));
                lookupuid = DBAccess.ReadIntValue(row["FA_LOOKUP_UID"]);
                xCustomfield.CreateInt("FA_LOOKUP_UID", lookupuid);
                xCustomfield.CreateInt("FA_LOOKUPONLY", DBAccess.ReadIntValue(row["FA_LOOKUPONLY"]));
                xCustomfield.CreateInt("FA_LEAFONLY", DBAccess.ReadIntValue(row["FA_LEAFONLY"]));
                xCustomfield.CreateInt("FA_USEFULLNAME", DBAccess.ReadIntValue(row["FA_USEFULLNAME"]));
            }
            else
            {
                xCustomfield.CreateInt("FA_FIELD_ID", nFieldId);
                xCustomfield.CreateString("FA_NAME", "");
                xCustomfield.CreateInt("FA_LOOKUP_UID", 0);
                xCustomfield.CreateInt("FA_LOOKUPONLY", 0);
                xCustomfield.CreateInt("FA_LEAFONLY", 0);
                xCustomfield.CreateInt("FA_USEFULLNAME", 0);
            }

            //if (lookupuid > 0)
            {
                dbaGeneral.SelectLookup(dba, lookupuid, out dt);

                _TGrid tg = new _TGrid();
                InitializeColumns(tg);
                tg.SetDataTable(dt);
                string tgridData = "";
                tg.Build(out tgridData);
                xCustomfield.CreateString("tgridData", tgridData);

            }

            dbaGeneral.SelectLookups(dba, out dt);
            

            dba.Close();

            sReply = xCustomfield.XML();
            return sReply;
        }
        private static string UpdateCustomFieldInfo(HttpContext Context, CStruct xData)
        {
            string sReply = "";
            string sBaseInfo = WebAdmin.BuildBaseInfo(Context);
            DataAccess da = new DataAccess(sBaseInfo);
            DBAccess dba = da.dba;
            if (dba.Open() == StatusEnum.rsSuccess)
            {
                int nFieldId = xData.GetIntAttr("FA_FIELD_ID");
                string sFieldName = xData.GetStringAttr("FA_NAME");
                string sFieldDesc = xData.GetStringAttr("FA_DESC");
                int nLookupID = xData.GetIntAttr("FA_LOOKUP_UID");
                int nLeafOnly = xData.GetIntAttr("FA_LEAFONLY");
                int nUseFullName = xData.GetIntAttr("FA_USEFULLNAME");
                try
                {
                    if (dbaResourcePlanning.UpdateCustomFieldInfo(dba, nFieldId, sFieldName, sFieldDesc, nLookupID, nLeafOnly, nUseFullName, out sReply) != StatusEnum.rsSuccess)
                    {
                        if (sReply.Length == 0) sReply = WebAdmin.FormatError("exception", "RPCustomFields.UpdateCustomFieldInfo", dba.StatusText);
                    }
                    else
                    {
                        sReply = ReadCustomFieldInfo(dba, nFieldId);
                    }
                }
                catch (Exception ex)
                {
                    sReply = WebAdmin.FormatError("exception", "RPCustomFields.UpdateCustomFieldInfo", ex.Message);
                }
                dba.Close();
            }
            return sReply;
        }

        private static string DeleteCustomFieldInfo(HttpContext Context, CStruct xData)
        {
            string sReply = "";
            string sBaseInfo = WebAdmin.BuildBaseInfo(Context);
            DataAccess da = new DataAccess(sBaseInfo);
            DBAccess dba = da.dba;
            if (dba.Open() == StatusEnum.rsSuccess)
            {
                int nFieldId = xData.GetIntAttr("FA_FIELD_ID");
                try
                {
                    if (dbaResourcePlanning.DeleteCustomField(dba, nFieldId, out sReply) != StatusEnum.rsSuccess)
                    {
                        if (sReply.Length == 0) sReply = WebAdmin.FormatError("exception", "RPCustomFields.DeleteCustomFieldInfo", dba.StatusText);
                    }
                    else
                    {
                        sReply = ReadCustomFieldInfo(dba, nFieldId);
                    }
                }
                catch (Exception ex)
                {
                    sReply = WebAdmin.FormatError("exception", "RPCustomFields.DeleteCustomFieldInfo", ex.Message);
                }
                dba.Close();
            }
            return sReply;
        }

        #endregion
    }
}
