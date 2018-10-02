﻿using System;
using System.Data.SqlClient;
using Microsoft.SharePoint;

namespace EPMLiveCore.API
{
    /// <summary>
    ///     static class that triages the
    ///     workspace creation logic
    /// </summary>
    public static class WorkspaceTimerjobAgent
    {
        private const string SecKey = "waitingOnSecurity";
        private const string Completed = "completed";

        public static string AddCreateWorkspaceJob(string xmlData)
        {
            return CreateJob(xmlData, false);
        }

        public static string AddCreateWorkspaceJobAndWait(string xmlData)
        {
            string result = string.Empty;
            var mgr = new XMLDataManager(xmlData);
            var sid = new Guid(mgr.GetPropVal("SiteId"));
            var wid = new Guid(mgr.GetPropVal("WebId"));
            Guid listguid = Guid.Empty;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemListGuid")))
            {
                listguid = new Guid(mgr.GetPropVal("AttachedItemListGuid"));
            }
            int itemid = -1;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemId")))
            {
                itemid = int.Parse(mgr.GetPropVal("AttachedItemId"));
            }
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var site = new SPSite(sid))
                {
                    using (SPWeb web = site.OpenWeb(wid))
                    {
                        using (var con = new SqlConnection(CoreFunctions.getConnectionString(site.WebApplication.Id)))
                        {
                            con.Open();
                            Guid timerjobguid = Guid.NewGuid();
                            var cmd = new SqlCommand
                            {
                                CommandText =
                                    "INSERT INTO TIMERJOBS (timerjobuid, siteguid, jobtype, jobname, scheduletype, webguid, listguid, itemid, jobdata, [key]) VALUES (@timerjobuid,@siteguid, 100, 'Create Workspace', 0, @webguid, @listguid, @itemid, @jobdata, @key)",
                                Connection = con
                            };

                            cmd.Parameters.Add(new SqlParameter("@timerjobuid", timerjobguid));
                            cmd.Parameters.Add(new SqlParameter("@siteguid", site.ID.ToString()));
                            cmd.Parameters.Add(new SqlParameter("@webguid", web.ID.ToString()));
                            cmd.Parameters.Add(new SqlParameter("@listguid", listguid.ToString()));
                            cmd.Parameters.Add(new SqlParameter("@itemid", itemid.ToString()));
                            cmd.Parameters.Add(new SqlParameter("@jobdata", xmlData));
                            cmd.Parameters.Add(new SqlParameter("@key", SecKey));
                            cmd.ExecuteNonQuery();

                            result = "Successfully queued create workspace job.";
                        }
                    }
                }
            });

            return result;
        }

        public static string QueueWorkspaceJobOnHoldForSecurity(string xmlData)
        {
            string result = string.Empty;
            var mgr = new XMLDataManager(xmlData);
            var sid = new Guid(mgr.GetPropVal("SiteId"));
            var wid = new Guid(mgr.GetPropVal("WebId"));
            Guid listguid = Guid.Empty;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemListGuid")))
            {
                listguid = new Guid(mgr.GetPropVal("AttachedItemListGuid"));
            }
            int itemid = -1;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemId")))
            {
                itemid = int.Parse(mgr.GetPropVal("AttachedItemId"));
            }
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var s = new SPSite(sid))
                {
                    using (SPWeb w = s.OpenWeb(wid))
                    {
                        using (var con = new SqlConnection(CoreFunctions.getConnectionString(s.WebApplication.Id)))
                        {
                            Guid wsJobUid = Guid.Empty;
                            con.Open();
                            var cmd = new SqlCommand
                            {
                                Connection = con,
                                CommandText = string.Format("SELECT timerjobuid FROM TIMERJOBS WHERE webguid='{0}' AND listguid='{1}' AND itemid={2} AND [key]='{3}'", w.ID, listguid, itemid, SecKey)
                            };
                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                try
                                {
                                    wsJobUid = new Guid(rdr[0].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (wsJobUid != Guid.Empty)
                            {
                                using (var update = new SqlCommand
                                {
                                    Connection = con,
                                    CommandText = string.Format("UPDATE TIMERJOBS SET [key] = '{1}' WHERE timerjobuid='{0}'", wsJobUid, Completed)
                                })
                                {
                                    update.ExecuteNonQuery();
                                }
                                CoreFunctions.enqueue(wsJobUid, 0, s);
                            }
                        }
                    }
                }
            });

            return result;
        }

        /// <summary>
        ///     see if a splistitem has a createworkspace job
        ///     in timerjob table. Queue it if it does.
        /// </summary>
        /// <param name="sid"></param>
        /// <param name="wid"></param>
        /// <param name="listguid"></param>
        /// <param name="itemid"></param>
        /// <returns></returns>
        public static string QueueWorkspaceJobOnHoldForSecurity(Guid sid, Guid wid, Guid listguid, int itemid)
        {   
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var s = new SPSite(sid))
                {
                    using (SPWeb w = s.OpenWeb(wid))
                    {
                        using (var con = new SqlConnection(CoreFunctions.getConnectionString(s.WebApplication.Id)))
                        {
                            Guid wsJobUid = Guid.Empty;
                            con.Open();
                            var cmd =
                                new SqlCommand
                                {
                                    CommandText =
                                        string.Format(
                                            "SELECT timerjobuid FROM TIMERJOBS WHERE webguid='{0}' AND listguid='{1}' AND itemid={2} AND [key]='" +
                                            SecKey + "'", w.ID, listguid, itemid),
                                    Connection = con
                                };
                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                try
                                {
                                    wsJobUid = new Guid(rdr[0].ToString());
                                }
                                catch
                                {
                                }
                            }
                            rdr.Close();

                            if (wsJobUid != Guid.Empty)
                            {
                                using (var update = new SqlCommand
                                {
                                    Connection = con,
                                    CommandText = string.Format("UPDATE TIMERJOBS SET [key] = '{1}' WHERE timerjobuid='{0}'", wsJobUid, Completed)
                                })
                                {
                                    update.ExecuteNonQuery();
                                }
                                CoreFunctions.enqueue(wsJobUid, 0, s);
                            }
                        }
                    }
                }
            });

            return "success";
        }

        public static string QueueCreateWorkspace(string xmlData)
        {
            var mgr = new XMLDataManager(xmlData);
            var sid = new Guid(mgr.GetPropVal("SiteId"));
            var wid = new Guid(mgr.GetPropVal("WebId"));
            Guid listguid = Guid.Empty;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemListGuid")))
            {
                listguid = new Guid(mgr.GetPropVal("AttachedItemListGuid"));
            }
            int itemid = -1;
            if (!string.IsNullOrEmpty(mgr.GetPropVal("AttachedItemId")))
            {
                itemid = int.Parse(mgr.GetPropVal("AttachedItemId"));
            }
            return QueueCreateWorkspace(sid, wid, listguid, itemid);
        }

        public static string QueueCreateWorkspace(Guid sid, Guid wid, Guid listguid, int itemid)
        {
            SPSecurity.RunWithElevatedPrivileges(() =>
            {
                using (var s = new SPSite(sid))
                {
                    using (SPWeb w = s.OpenWeb(wid))
                    {
                        using (var con = new SqlConnection(CoreFunctions.getConnectionString(s.WebApplication.Id)))
                        {
                            Guid wsJobUid = Guid.Empty;
                            con.Open();
                            var cmd = new SqlCommand
                            {
                                Connection = con,
                                CommandText =
                                    string.Format(
                                        "SELECT timerjobuid FROM TIMERJOBS WHERE webguid='{0}' AND listguid='{1}' AND itemid={2}",
                                        w.ID, listguid, itemid)
                            };
                            SqlDataReader rdr = cmd.ExecuteReader();

                            while (rdr.Read())
                            {
                                try
                                {
                                    wsJobUid = new Guid(rdr[0].ToString());
                                }
                                catch
                                {
                                }
                            }

                            if (wsJobUid != Guid.Empty)
                            {
                                CoreFunctions.enqueue(wsJobUid, 0, s);
                            }
                        }
                    }
                }
            });

            return "success";
        }

        public static string AddAndQueueCreateWorkspaceJob(string xmlData)
        {
            return CreateJob(xmlData, true);
        }

        private static string CreateJob(string xmlData, bool enqueue)
        {
            var result = "success";
            var xmlDataManager = new XMLDataManager(xmlData);
            var siteId = new Guid(xmlDataManager.GetPropVal("SiteId"));
            var webId = new Guid(xmlDataManager.GetPropVal("WebId"));
            var listGuid = Guid.Empty;
            var attachedItemListGuid = xmlDataManager.GetPropVal("AttachedItemListGuid");
            if (!string.IsNullOrWhiteSpace(attachedItemListGuid))
            {
                listGuid = new Guid(attachedItemListGuid);
            }
            var itemId = -1;
            var attachedItemId = xmlDataManager.GetPropVal("AttachedItemId");
            if (!string.IsNullOrWhiteSpace(attachedItemId))
            {
                itemId = int.Parse(attachedItemId);
            }
            SPSecurity.RunWithElevatedPrivileges(
                () =>
                {
                    using (var site = new SPSite(siteId))
                    {
                        using (var web = site.OpenWeb(webId))
                        {
                            using (var con = new SqlConnection(CoreFunctions.getConnectionString(site.WebApplication.Id)))
                            {
                                con.Open();
                                var timerJobGuid = Guid.NewGuid();
                                var cmd = new SqlCommand
                                {
                                    CommandText =
                                        "INSERT INTO TIMERJOBS (timerjobuid, siteguid, jobtype, jobname, scheduletype, webguid, listguid, itemid, jobdata) VALUES (@timerjobuid,@siteguid, 100, 'Create Workspace', 0, @webguid, @listguid, @itemid, @jobdata)",
                                    Connection = con
                                };

                                cmd.Parameters.Add(new SqlParameter("@timerjobuid", timerJobGuid));
                                cmd.Parameters.Add(new SqlParameter("@siteguid", site.ID.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@webguid", web.ID.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@listguid", listGuid.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@itemid", itemId.ToString()));
                                cmd.Parameters.Add(new SqlParameter("@jobdata", xmlData));
                                cmd.ExecuteNonQuery();

                                if (enqueue)
                                {
                                    CoreFunctions.enqueue(timerJobGuid, 0, site);
                                    result = "Successfully queued create workspace job.";
                                }
                            }
                        }
                    }
                });

            return result;
        }
    }
}