﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint;
using System.Xml;
using System.Data.SqlClient;
using System.Collections;

namespace EPMLiveCore.API
{
    internal class PlatformIntegration
    {
        public static string InstallIntegration(string data, SPWeb web)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                if (web.DoesUserHavePermissions(SPBasePermissions.ManageLists))
                {
                    SPList list = null;
                    try
                    {
                        list = web.Lists[new Guid(doc.FirstChild.Attributes["List"].Value)];
                    }
                    catch { }
                    if (list != null)
                    {
                        const string assemblyName =
                        "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                        const string className = "EPMLiveCore.PlatformIntegrationEvent";

                        bool bAdding = false;
                        bool bUpdating = false;
                        bool bDeleting = false;


                        foreach (SPEventReceiverDefinition ev in list.EventReceivers)
                        {
                            if (ev.Assembly == assemblyName && ev.Class == className)
                            {
                                if (ev.Type == SPEventReceiverType.ItemAdded)
                                    bAdding = true;

                                if (ev.Type == SPEventReceiverType.ItemUpdated)
                                    bUpdating = true;

                                if (ev.Type == SPEventReceiverType.ItemDeleting)
                                    bDeleting = true;
                            }
                        }

                        if (!bAdding)
                            list.EventReceivers.Add(SPEventReceiverType.ItemAdded, assemblyName, className);

                        if (!bUpdating)
                            list.EventReceivers.Add(SPEventReceiverType.ItemUpdated, assemblyName, className);

                        if (!bDeleting)
                            list.EventReceivers.Add(SPEventReceiverType.ItemDeleting, assemblyName, className);

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {

                            SqlConnection cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                            cn.Open();

                            SqlCommand cmd = new SqlCommand("DELETE FROM PLATFORMINTEGRATIONS where PlatformIntegrationId=@id", cn);
                            cmd.Parameters.AddWithValue("@id", doc.FirstChild.Attributes["IntID"].Value);
                            cmd.ExecuteNonQuery();

                            cmd = new SqlCommand("INSERT INTO PLATFORMINTEGRATIONS (PlatformIntegrationId,ListId,IntegrationKey,IntegrationUrl) VALUES (@id,@listid,@key,@url)", cn);
                            cmd.Parameters.AddWithValue("@id", doc.FirstChild.Attributes["IntID"].Value);
                            cmd.Parameters.AddWithValue("@listid", doc.FirstChild.Attributes["List"].Value);
                            cmd.Parameters.AddWithValue("@url", doc.FirstChild.Attributes["APIUrl"].Value);
                            cmd.Parameters.AddWithValue("@key", doc.FirstChild.Attributes["IntKey"].Value);
                            cmd.ExecuteNonQuery();

                            cmd = new SqlCommand("INSERT INTO PLATFORMINTEGRATIONLOG (PlatformIntegrationId, DTLOGGED, MESSAGE, LOGLEVEL) VALUES (@intid, GETDATE(), 'Successfully installed integration', 10)", cn);
                            cmd.Parameters.AddWithValue("@intid", doc.FirstChild.Attributes["IntID"].Value);
                            cmd.ExecuteNonQuery();

     
                            foreach (XmlNode ndControl in doc.FirstChild.SelectSingleNode("Controls").SelectNodes("Control"))
                            {
                                cmd = new SqlCommand("INSERT INTO PLATFORMINTEGRATIONCONTROLS (PlatformIntegrationId, ControlId, DisplayName, Image, Global, ButtonStyle, WindowStyle) VALUES (@PlatformIntegrationId, @ControlId, @DisplayName, @Image, @Global, @ButtonStyle, @WindowStyle)", cn);
                                cmd.Parameters.AddWithValue("@PlatformIntegrationId", doc.FirstChild.Attributes["IntID"].Value);
                                cmd.Parameters.AddWithValue("@ControlId", ndControl.Attributes["Id"].Value);
                                cmd.Parameters.AddWithValue("@DisplayName", ndControl.Attributes["DisplayName"].Value);
                                cmd.Parameters.AddWithValue("@Image", ndControl.Attributes["Image"].Value);
                                cmd.Parameters.AddWithValue("@Global", ndControl.Attributes["Global"].Value);
                                cmd.Parameters.AddWithValue("@ButtonStyle", ndControl.Attributes["ButtonStyle"].Value);
                                cmd.Parameters.AddWithValue("@WindowStyle", ndControl.Attributes["WindowStyle"].Value);

                                cmd.ExecuteNonQuery();
                            }

                            cn.Close();

    
                        });

                    }
                    else
                    {
                        throw new APIException(500002, "List not found");
                    }
                }
                else
                    throw new APIException(500001, "User does not have access to modify lists");
            }
            catch (Exception ex)
            {
                throw new APIException(500000, "General Error: " + ex.Message);
            }
            return "";
        }

        public static string RemoveIntegration(string data, SPWeb web)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(data);

                if (web.DoesUserHavePermissions(SPBasePermissions.ManageLists))
                {
                    SPList list = null;
                    try
                    {
                        list = web.Lists[new Guid(doc.FirstChild.Attributes["List"].Value)];
                    }
                    catch { }
                    if (list != null)
                    {
                        const string assemblyName =
                        "EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5";
                        const string className = "EPMLiveCore.PlatformIntegrationEvent";


                        ArrayList arrEvents = new ArrayList();

                        foreach (SPEventReceiverDefinition ev in list.EventReceivers)
                        {
                            if (ev.Assembly == assemblyName && ev.Class == className)
                            {
                                arrEvents.Add(ev);
                            }
                        }

                        foreach (SPEventReceiverDefinition ev in arrEvents)
                        {
                            ev.Delete();
                        }

                        list.Update();

                        SPSecurity.RunWithElevatedPrivileges(delegate()
                        {
                            using (var connection = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id)))
                            {
                                connection.Open();

                                using (var command = new SqlCommand("DELETE FROM PLATFORMINTEGRATIONLOG where PlatformIntegrationId=@id", connection))
                                {
                                    command.Parameters.AddWithValue("@id", doc.FirstChild.Attributes["IntID"].Value);
                                    command.ExecuteNonQuery();
                                }

                                using (var command = new SqlCommand("DELETE FROM PLATFORMINTEGRATIONCONTROLS where PlatformIntegrationId=@id", connection))
                                {
                                    command.Parameters.AddWithValue("@id", doc.FirstChild.Attributes["IntID"].Value);
                                    command.ExecuteNonQuery();
                                }

                                using (var command = new SqlCommand("DELETE FROM PLATFORMINTEGRATIONS where PlatformIntegrationId=@id", connection))
                                {
                                    command.Parameters.AddWithValue("@id", doc.FirstChild.Attributes["IntID"].Value);
                                    command.ExecuteNonQuery();
                                }
                            }
                        });
                    }
                    else
                    {
                        throw new APIException(500002, "List not found");
                    }
                }
                else
                    throw new APIException(500001, "User does not have access to modify lists");
            }
            catch (Exception ex)
            {
                throw new APIException(500000, "General Error: " + ex.Message);
            }


            return "";
        }
    }
}
