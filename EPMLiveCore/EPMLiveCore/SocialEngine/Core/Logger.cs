﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Xml.Linq;
using Microsoft.SharePoint;

namespace EPMLiveCore.SocialEngine.Core
{
    internal class Logger
    {
        #region Methods (4) 

        // Public Methods (2) 

        public Guid Log(ObjectKind objectKind, ActivityKind activityKind,
            Dictionary<string, object> data, SPWeb spWeb, Exception exception)
        {
            XElement details = GetDetails(data);

            if (exception.InnerException != null)
            {
                details.Add(new XElement("InnerException", new XCData(exception.InnerException.Message)));
            }

            var kind = LogKind.Error;

            var socialEngineException = exception as SocialEngineException;
            if (socialEngineException != null) kind = socialEngineException.LogKind;

            return AddLog(spWeb, exception.Message, exception.StackTrace, details, kind);
        }

        public Guid Log(ObjectKind objectKind, ActivityKind activityKind, Dictionary<string, object> data, SPWeb spWeb,
            string message)
        {
            return AddLog(spWeb, message, null, GetDetails(data), LogKind.Info);
        }

        // Private Methods (2) 

        private static Guid AddLog(SPWeb spWeb, string message, string stackTrace, XElement details, LogKind kind)
        {
            Guid id = Guid.NewGuid();

            const string SQL =
                @"INSERT INTO SS_Logs (Id, Message, StackTrace, Details, Kind, WebId, UserId) VALUES (@Id, @Message, @StackTrace, @Details, @Kind, @WebId, @UserId)";

            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                string cs = CoreFunctions.getReportingConnectionString(spWeb.Site.WebApplication.Id, spWeb.Site.ID);

                using (var sqlConnection = new SqlConnection(cs))
                {
                    using (var sqlCommand = new SqlCommand(SQL, sqlConnection))
                    {
                        sqlCommand.Parameters.AddWithValue("@Id", id);
                        sqlCommand.Parameters.AddWithValue("@Message", message);
                        sqlCommand.Parameters.AddWithValue("@StackTrace", stackTrace);
                        sqlCommand.Parameters.AddWithValue("@Details", details.ToString());
                        sqlCommand.Parameters.AddWithValue("@Kind", kind);
                        sqlCommand.Parameters.AddWithValue("@WebId", spWeb.ID);
                        sqlCommand.Parameters.AddWithValue("@UserId", spWeb.CurrentUser.ID);

                        sqlConnection.Open();
                        sqlCommand.ExecuteNonQuery();
                    }
                }
            });

            return id;
        }

        private static XElement GetDetails(Dictionary<string, object> data)
        {
            var details = new XElement("Details");

            foreach (var d in data)
            {
                details.Add(new XElement(d.Key, new XCData((d.Value ?? string.Empty).ToString())));
            }

            return details;
        }

        #endregion Methods 
    }
}