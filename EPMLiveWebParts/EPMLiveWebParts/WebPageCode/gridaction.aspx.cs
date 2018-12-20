using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using EPMLiveCore;
using EPMLiveCore.API;
using EPMLiveCore.API.ProjectArchiver;
using EPMLiveCore.ReportingProxy;
using Microsoft.SharePoint;

namespace EPMLiveWebParts
{
    public partial class gridaction : System.Web.UI.Page
    {
        private const string SuccessMessage = "Success";
        private const string ArhiveRestoreListIdRequestParameter = "listid";
        private const string ArchiveRestoreItemIdRequestParameter = "id";
        private const string ArchiveProjectAction = "archiveproject";
        private const string RestoreProjectAction = "restoreproject";

        protected string data;
        

        private string getMenuItem(string grid, string title, string image, string command, string type)
        {
            string ret = Properties.Resources.txtGridMenus;
            ret = ret.Replace("#title#", title);
            ret = ret.Replace("#image#", image);
            //ret = ret.Replace("#command#", command);
            //ret = ret.Replace("#type#", type);
            ret = ret.Replace("#gridfunc#", "mygrid" + grid + ".menuaction(this, '" + command + "','" + type + "');");
            ret = ret.Replace("#gridmenuhover#", "mygrid" + grid + ".menuhover(this);");
            return ret;
        }

        private string getmenus(SPWeb web)
        {

            bool bUsePopup = false;
            bool.TryParse(Request["popups"], out bUsePopup);


            StringBuilder sb = new StringBuilder();

            Dictionary<string, string> items = new Dictionary<string, string>();
            int counter = 1;
            try
            {
                SPList list = web.Lists[new Guid(Request["listid"])];

                SPListItem li = list.GetItemById(int.Parse(Request["ID"]));

                EPMLiveCore.GridGanttSettings gSettings = new EPMLiveCore.GridGanttSettings(list);

                bool isFav = IsFav(list, li, web, gSettings);


                if (list.DoesUserHavePermissions(SPBasePermissions.ViewListItems))
                {
                    if (bUsePopup)
                        items.Add("View Item", getMenuItem(Request["grid"], "View Item", "/_layouts/images/blank.gif", "view", ""));
                    else
                        items.Add("View Item", getMenuItem(Request["grid"], "View Item", "/_layouts/images/blank.gif", "view", "1"));
                }

                if (list.DoesUserHavePermissions(SPBasePermissions.EditListItems))
                {
                    if (bUsePopup)
                        items.Add("Edit Item", getMenuItem(Request["grid"], "Edit Item", "/_layouts/images/edititem.gif", "edit", ""));
                    else
                        items.Add("Edit Item", getMenuItem(Request["grid"], "Edit Item", "/_layouts/images/edititem.gif", "edit", "1"));
                }

                if (list.DoesUserHavePermissions(SPBasePermissions.ViewListItems))
                {
                    if (web.Features[new Guid("046f0200-30e5-4545-b00f-c8c73aef9f0e")] != null)
                    {
                        if (!isFav)
                            items.Add("Add Favorite", getMenuItem(Request["grid"], "Add Favorite", "/_layouts/images/blank.gif", "AddFavorite", ""));
                        else
                            items.Add("Remove Favorite", getMenuItem(Request["grid"], "Remove Favorite", "/_layouts/images/blank.gif", "RemoveFavorite", ""));
                    }
                }

                items.Add("SEP" + (counter++).ToString(), "");

                if (list.EnableVersioning)
                    if (list.DoesUserHavePermissions(SPBasePermissions.ViewVersions))
                        items.Add("View Versions", getMenuItem(Request["grid"], "View Versions", "/_layouts/images/versions.gif", "version", ""));

                if (list.EnableModeration)
                    if (list.DoesUserHavePermissions(SPBasePermissions.ApproveItems))
                        items.Add("Approve Item", getMenuItem(Request["grid"], "Approve Item", "/_layouts/images/apprj.gif", "approve", ""));

                if (list.WorkflowAssociations.Count > 0)
                    items.Add("Workflows", getMenuItem(Request["grid"], "Workflows", "/_layouts/images/workflows.gif", "workflows", "1"));

                items.Add("SEP" + (counter++).ToString(), "");

                if (list.DoesUserHavePermissions(SPBasePermissions.ManagePermissions))
                    items.Add("Permissions", getMenuItem(Request["grid"], "Permissions", "/_layouts/images/permissions16.png", "perms", "1"));

                if (list.DoesUserHavePermissions(SPBasePermissions.DeleteListItems))
                    items.Add("Delete Item", getMenuItem(Request["grid"], "Delete Item", "/_layouts/images/delitem.gif", "delete", "99"));

                items.Add("SEP" + (counter++).ToString(), "");



                Dictionary<string, EPMLiveCore.PlannerDefinition> pList = EPMLiveCore.CoreFunctions.GetPlannerList(web, null);

                int bPlanner = 0;

                foreach (KeyValuePair<string, EPMLiveCore.PlannerDefinition> de in pList)
                {
                    string id = (string)de.Key;
                    EPMLiveCore.PlannerDefinition p = (EPMLiveCore.PlannerDefinition)de.Value;

                    if (String.Equals(p.commandPrefix, li.ParentList.Title, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bPlanner = 1;
                        break;
                    }
                    if (String.Equals(p.command, li.ParentList.Title, StringComparison.InvariantCultureIgnoreCase))
                    {
                        bPlanner = 2;
                        break;
                    }
                }

                if (bPlanner == 1)
                {
                    items.Add("Edit Plan", getMenuItem(Request["grid"], "Edit Plan", "/_layouts/epmlive/images/planner16.png", "gotoplanner", "1"));
                }
                else if (bPlanner == 2)
                {
                    items.Add("Edit Plan", getMenuItem(Request["grid"], "Edit Plan", "/_layouts/epmlive/images/planner16.png", "GoToTaskPlanner", "1"));
                }

                if (li.DoesUserHavePermissions(SPBasePermissions.EditListItems))
                    items.Add("Comments", getMenuItem(Request["grid"], "Comments", "/_layouts/epmlive/images/comments16.gif", "comments", "5"));

                if (li.DoesUserHavePermissions(SPBasePermissions.EditListItems) && gSettings.BuildTeam)
                    items.Add("BuildTeam", getMenuItem(Request["grid"], "Edit Team", "/_layouts/epmlive/images/buildteam16.gif", "buildteam", "6"));

                bool gotoshown = false;

                if (Request["rollups"] == "true")
                {
                    if (li.Web.ID != SPContext.Current.Web.ID)
                    {
                        gotoshown = true;
                        items.Add("Go To Workspace", getMenuItem(Request["grid"], "Go To Workspace", "/_layouts/images/STSICON.gif", "workspace", "1"));
                    }
                }

                if (Request["requestlist"] == "true" && !gotoshown)
                {
                    try
                    {
                        string tempurl = li["WorkspaceUrl"].ToString();
                        if (tempurl != "")
                            items.Add("Go To Workspace", getMenuItem(Request["grid"], "Go To Workspace", "/_layouts/images/STSICON.gif", "workspace", "1"));
                    }
                    catch { }
                }

                if (Request["requestlist"] == "true")
                {
                    string childitem = "";
                    string wsurl = "";
                    try
                    {
                        childitem = li["ChildItem"].ToString();
                    }
                    catch { }
                    try
                    {
                        wsurl = li["WorkspaceUrl"].ToString();
                    }
                    catch { }

                    if ((li.ModerationInformation == null || li.ModerationInformation.Status == SPModerationStatusType.Approved)
                        && web.ID == SPContext.Current.Web.ID
                        && childitem == ""
                        && wsurl == ""
                        && li.DoesUserHavePermissions(web.CurrentUser, SPBasePermissions.EditListItems)
                        && web.DoesUserHavePermissions(web.CurrentUser.LoginName, SPBasePermissions.ManageSubwebs))
                    {
                        items.Add("Create Workspace", getMenuItem(Request["grid"], "Create Workspace", "/_layouts/images/STSICON.gif", "createworkspace", "1"));
                    }
                }



                if (web.Site.Features[new Guid("158c5682-d839-4248-b780-82b4710ee152")] != null)
                {
                    SPWeb rweb = web.Site.RootWeb;

                    ArrayList arr = new ArrayList(EPMLiveCore.CoreFunctions.getConfigSetting(rweb, "EPKLists").ToLower().Split(','));
                    if (arr.Contains(list.Title.ToLower()))
                    {
                        items.Add("SEP" + (counter++).ToString(), "");

                        string menus = EPMLiveCore.CoreFunctions.getConfigSetting(web.Site.RootWeb, "EPK" + list.Title.Replace(" ", "") + "_menus");
                        if (menus == "")
                            menus = EPMLiveCore.CoreFunctions.getConfigSetting(web.Site.RootWeb, "EPKMenus");

                        ArrayList arrButtons = new ArrayList(menus.Split('|'));

                        if (arrButtons.Contains("details"))
                        {
                            items.Add("PI Details", getMenuItem(Request["grid"], "PI Details", "/_layouts/images/edititem.gif", "epkcommand:Details", ""));
                        }
                        if (arrButtons.Contains("costs"))
                        {
                            items.Add("Edit Costs", getMenuItem(Request["grid"], "Edit Costs", "/_layouts/epmlive/images/editcosts16.png", "epkcommand:Costs", "6"));
                        }
                        if (arrButtons.Contains("workplan"))
                        {
                            items.Add("Work Planner", getMenuItem(Request["grid"], "Work Planner", "/_layouts/epmlive/images/workitems.gif", "epkcommand:workplan", "6"));
                        }
                        if (arrButtons.Contains("resplan"))
                        {
                            items.Add("Edit Resource Plan", getMenuItem(Request["grid"], "Edit Resource Plan", "/_layouts/epmlive/images/resplan.gif", "epkcommand:rpeditor", "6"));
                        }
                    }
                }

                bool hassep = false;

                foreach (KeyValuePair<string, string> de in items)
                {
                    if (de.Key.ToString().IndexOf("SEP") == 0)
                    {
                        hassep = true;
                    }
                    else
                    {
                        if (hassep)
                        {
                            hassep = false;
                            sb.Append(@"<LI id=sep class=ms-MenuUIULItem type=""separator""><DIV class=ms-MenuUISeparator type=""separator"">&nbsp;</DIV></LI>");
                        }
                        sb.Append(de.Value);
                    }
                }

            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
            return sb.ToString();
        }

        private bool IsFav(SPList l, SPListItem li, SPWeb web, GridGanttSettings gSettings)
        {
            var result = false;
            var queryCheckFavStatus_Item =
                   @"IF EXISTS (SELECT 1 FROM FRF WHERE [SITE_ID]=@siteid AND [WEB_ID]=@webid AND [LIST_ID]=@listid AND [ITEM_ID]=@itemid AND [USER_ID]=@userid AND [Type]=" + Convert.ToInt32(AnalyticsType.Favorite) + @")
                    BEGIN
	                    SELECT 'true'
                    END
                    ELSE
                    BEGIN
                        SELECT 'false'
                    END";

            var qParams =
            new Dictionary<string, object>
            {
                {"@siteid", web.Site.ID},
                {"@webid", web.ID},
                {"@listid", l.ID},
                {"@itemid", li.ID},
                {"@userid", web.CurrentUser.ID},
                {"@icon", gSettings.ListIcon},
                {"@title", li.Title},
            };

            var qExec = new QueryExecutor(SPContext.Current.Web);
            var table = qExec.ExecuteEpmLiveQuery(queryCheckFavStatus_Item, qParams);

            if (table != null)
            {
                try
                {
                    result = bool.Parse(table.Rows[0][0].ToString());
                }
                catch { }
            }

            return result;
        }

        private string getplannerlist(SPWeb web, SPListItem li)
        {
            StringBuilder sb = new StringBuilder();

            Dictionary<string, EPMLiveCore.PlannerDefinition> pList = EPMLiveCore.CoreFunctions.GetPlannerList(web, li);

            foreach (KeyValuePair<string, EPMLiveCore.PlannerDefinition> de in pList)
            {
                string id = (string)de.Key;
                EPMLiveCore.PlannerDefinition p = (EPMLiveCore.PlannerDefinition)de.Value;

                sb.Append("^");
                sb.Append(id);
                sb.Append("|");
                sb.Append(p.title);
                sb.Append("|");
                sb.Append(p.description);
                sb.Append("|");
                sb.Append(p.command);
                sb.Append("|");
                sb.Append(p.image);
            }

            return sb.ToString().Trim('^');
        }

        protected void linkeditemspost(SPWeb web)
        {
            SqlConnection cn = null;
            SPSecurity.RunWithElevatedPrivileges(delegate()
            {
                //using (SPSite s = SPContext.Current.Site)
                {
                    cn = new SqlConnection(EPMLiveCore.CoreFunctions.getConnectionString(web.Site.WebApplication.Id));
                    cn.Open();

                    SqlCommand cmd = new SqlCommand("DELETE FROM PERSONALIZATIONS where userid=@userid and [key]=@key and listid=@listid", cn);
                    cmd.Parameters.AddWithValue("@userid", web.CurrentUser.ID);
                    cmd.Parameters.AddWithValue("@key", "LIP");
                    cmd.Parameters.AddWithValue("@listid", Request["listid"]);
                    cmd.ExecuteNonQuery();

                    cmd = new SqlCommand("INSERT INTO PERSONALIZATIONS (userid, [key], value,siteid,webid,listid,ItemId) VALUES (@userid,@key,@value,@siteid,@webid,@listid,@ItemId)", cn);
                    cmd.Parameters.AddWithValue("@userid", web.CurrentUser.ID);
                    cmd.Parameters.AddWithValue("@key", "LIP");
                    cmd.Parameters.AddWithValue("@siteid", web.Site.ID);
                    cmd.Parameters.AddWithValue("@webid", web.ID);
                    cmd.Parameters.AddWithValue("@listid", Request["listid"]);
                    cmd.Parameters.AddWithValue("@value", Request["lookups"]);
                    if (Request["lookupid"] != null)
                    {
                        cmd.Parameters.AddWithValue("@ItemId", Request["lookupid"]);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@ItemId", DBNull.Value);
                    }
                    cmd.ExecuteNonQuery();

                    cn.Close();
                }
            });
        }

        private SPWeb GetWeb(SPSite site)
        {
            if (Request["webid"] != null && Request["webid"] != "")
                return site.OpenWeb(new Guid(Request["webid"]));
            else
                return SPContext.Current.Web;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string url = "";
            string script = "";
            try
            {
                SPSite site = SPContext.Current.Site;
                site.CatchAccessDeniedException = false;
                {
                    SPWeb w;

                    switch (Request["action"].ToLower())
                    {
                        case "buildteam":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/buildteam.aspx?listid=" + Request["listid"] + "&id=" + Request["id"];
                            w.Close();
                            break;
                        case "linkeditemspost":
                            w = SPContext.Current.Web;
                            {
                                linkeditemspost(w);
                                data = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "|" + Request["field"] + "|" + Request["listid"] + "|" + Request["LookupFieldList"];
                            }
                            break;
                        case "associateditems":
                        case "fancydispform":
                            w = SPContext.Current.Web;
                            {
                                SPList list = w.Lists[new Guid(Request["list"])];
                                url = list.DefaultViewUrl + "?lookupfield=" + Request["field"] + "&LookupFieldList=" + Request["LookupFieldList"];
                            }
                            break;
                        case "deleteitemattachment":
                            w = SPContext.Current.Web;
                            {
                                try
                                {
                                    w.AllowUnsafeUpdates = true;
                                    Guid listId = new Guid(Request["listid"]);
                                    Int32 itemId = Convert.ToInt32(Request["itemid"]);
                                    string fileName = Convert.ToString(Request["fname"]);

                                    SPList list = w.Lists[listId];
                                    SPListItem item = list.GetItemById(itemId);
                                    SPAttachmentCollection allAttachments = item.Attachments;
                                    if (allAttachments != null)
                                    {
                                        allAttachments.RecycleNow(fileName);
                                    }

                                    data = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "|" + listId + "|" + itemId + "|" + fileName;
                                }
                                catch { }
                            }
                            break;
                        case "linkeditems":
                            w = SPContext.Current.Web;
                            {
                                SPList list = w.Lists[HttpUtility.HtmlDecode(Request["list"])];
                                url = list.DefaultViewUrl + "?lookupfield=" + Request["field"] + "&LookupFieldList=" + Request["LookupFieldList"];
                            }
                            break;
                        case "getribbonplanners":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                SPListItem li = list.GetItemById(int.Parse(Request["itemid"]));
                                data = getplannerlist(w, li);
                            }
                            w.Close();
                            break;
                        case "editview":
                            w = GetWeb(site);
                            try
                            {
                                GridViewSession gvs = new GridViewSession(Guid.Empty);
                            }
                            catch { }
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/15/ViewEdit.aspx?List=" + Request["List"] + "&View=" + Request["view"];
                            break;
                        case "createworkspace":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/requestworkspace.aspx?id=" + Request["ID"] + "&list=" + Request["listId"];
                            w.Close();
                            break;
                        //case "postepkmultipage":
                        //    {
                        //        string epkurl = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPKURL");

                        //        if(epkurl != "")
                        //        {
                        //            EPKIntegration.Integration epkInt = new EPKIntegration.Integration();
                        //            epkInt.Url = epkurl + "/services/integration.asmx";
                        //            epkInt.UseDefaultCredentials = true;

                        //            string ret = epkInt.ExecuteProcess("DisplayProjects", "<Projects>" + Request["IDs"].ToLower() + "</Projects>");
                        //            XmlDocument doc = new XmlDocument();
                        //            doc.LoadXml(ret);

                        //            string status = doc.SelectSingleNode("//STATUS").InnerText;
                        //            if(status == "0")
                        //            {
                        //                data = doc.SelectSingleNode("//DisplayProjects").Attributes["SessionId"].Value;
                        //            }
                        //            else
                        //            {
                        //                data = "Post Error: " + doc.SelectSingleNode("//Error").InnerText;
                        //            }
                        //        }
                        //    }
                        //    break;
                        case "epkmultipage":
                            {
                                string epkurl = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPKURL");

                                if (epkurl != "")
                                {
                                    //EPKIntegration.Integration epkInt = new EPKIntegration.Integration();
                                    //epkInt.Url = epkurl + "/services/integration.asmx";
                                    //epkInt.UseDefaultCredentials = true;

                                    //string ret = epkInt.ExecuteProcess("DisplayProjects", "<Projects>" + Request["IDs"].ToLower() + "</Projects>");

                                    //XmlDocument doc = new XmlDocument();
                                    //doc.LoadXml(ret);
                                    url = ((site.ServerRelativeUrl == "/") ? "" : site.ServerRelativeUrl) + "/_layouts/ppm/" + Request["epkcontrol"] + ".aspx?dataid=" + Request["ticket"] + "&epkurl=" + HttpUtility.UrlEncode(epkurl) + "&view=" + Request["view"];


                                    /*string status = doc.SelectSingleNode("//STATUS").InnerText;
                                    if(status == "0")
                                    {
                                        url = ((site.ServerRelativeUrl == "/") ? "" : site.ServerRelativeUrl) + "/_layouts/ppm/" + Request["epkcontrol"] + ".aspx?dataid=" + doc.SelectSingleNode("//DisplayProjects").Attributes["SessionId"].Value + "&epkurl=" + HttpUtility.UrlEncode(epkurl) + "&view=" + Request["view"];
                                    }
                                    else
                                    {
                                        data = "Post Error: " + doc.SelectSingleNode("//Error").InnerText;
                                    }*/
                                }
                            }
                            break;
                        case "epksinglepage":
                            url = site.ServerRelativeUrl + "/_layouts/ppm/" + Request["epkcontrol"] + ".aspx?itemid=" + Request["itemid"] + "&listid=" + Request["listid"];
                            break;
                        case "epkcommand":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];

                                string view = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPK" + list.Title.Replace(" ", "") + "_costview");
                                if (view == "")
                                {
                                    string[] sEPKCostViews = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPKCostViews").Split('|');
                                    foreach (string sEPKView in sEPKCostViews)
                                    {
                                        string[] sEPKViewMap = sEPKView.Split(',');
                                        if (sEPKViewMap[0].ToLower() == Request["view"].ToLower())
                                            view = sEPKViewMap[1];
                                    }
                                    if (view == "")
                                    {
                                        string sviewtitle = list.DefaultView.Title;

                                        foreach (string sEPKView in sEPKCostViews)
                                        {
                                            string[] sEPKViewMap = sEPKView.Split(',');
                                            if (sEPKViewMap[0].ToLower() == sviewtitle.ToLower())
                                                view = sEPKViewMap[1];
                                        }
                                    }
                                }
                                url = (w.ServerRelativeUrl == "/" ? "" : w.ServerRelativeUrl);

                                w.Close();

                                string epkurl1 = EPMLiveCore.CoreFunctions.getConfigSetting(site.RootWeb, "EPKURL");
                                url += "/_layouts/ppm/" + Request["subaction"] + ".aspx?listid=" + Request["listid"] + "&itemid=" + Request["webid"] + "." + Request["listid"] + "." + Request["id"] + "&epkurl=" + HttpUtility.UrlEncode(epkurl1) + "&view=" + view;
                            }
                            break;
                        case "gotoplanner":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/workplanner.aspx?listid=" + Request["listid"] + "&ID=" + Request["id"];
                            w.Close();
                            break;
                        case "gotoplannerpc":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/workplanner.aspx?listid=" + Request["listid"] + "&ID=" + Request["id"] + "&PCSelected=true";
                            w.Close();
                            break;
                        case "gototaskplanner":
                            w = GetWeb(site);

                            if (String.IsNullOrEmpty(Request["listid"]))
                            {
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/workplanner.aspx";
                            }
                            else
                            {
                                SPList olist = w.Lists[new Guid(Request["listid"])];

                                SPListItem oli = olist.GetItemById(int.Parse(Request["id"]));

                                try
                                {
                                    SPFieldLookupValue lv = new SPFieldLookupValue(oli["Project"].ToString());

                                    XmlDocument doc = new XmlDocument();
                                    doc.LoadXml(olist.Fields.GetFieldByInternalName("Project").SchemaXml);

                                    url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/workplanner.aspx?listid=" + doc.FirstChild.Attributes["List"].Value + "&ID=" + lv.LookupId + "&tasklistid=" + Request["listid"];
                                }
                                catch { }
                            }
                            w.Close();
                            break;
                        case "editinproject":
                        case "getproject":
                            w = GetWeb(site);

                            //data = "<script language=\"javascript\">";
                            //data += "javascript:window.open('" + w.ServerRelativeUrl + "/_layouts/epmlive/getproject.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"] + "','', config='height=100,width=200, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, directories=no, status=no');";
                            //data += "location.href = '" + Request["Source"] + "';";
                            //data += "</script>";
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/getproject.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"];
                            w.Close();
                            break;
                        case "editinpsproject":
                        case "getpsproject":
                            w = GetWeb(site);

                            //data = "<script language=\"javascript\">";
                            //data += "javascript:window.open('" + w.ServerRelativeUrl + "/_layouts/epmlive/getproject.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"] + "','', config='height=100,width=200, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, directories=no, status=no');";
                            //data += "location.href = '" + Request["Source"] + "';";
                            //data += "</script>";
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/getpsproject.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"];
                            w.Close();
                            break;
                        case "getpswebapp":
                            w = GetWeb(site);

                            //data = "<script language=\"javascript\">";
                            //data += "javascript:window.open('" + w.ServerRelativeUrl + "/_layouts/epmlive/getproject.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"] + "','', config='height=100,width=200, toolbar=no, menubar=no, scrollbars=no, resizable=no,location=no, directories=no, status=no');";
                            //data += "location.href = '" + Request["Source"] + "';";
                            //data += "</script>";
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/getpswebapp.aspx?listID=" + Request["listId"] + "&ID=" + Request["ID"];
                            w.Close();
                            break;
                        case "workspace":
                            w = GetWeb(site);
                            try
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                SPListItem li = list.GetItemById(int.Parse(Request["ID"]));
                                url = li["WorkspaceUrl"].ToString().Split(',')[0]; ;
                            }
                            catch { }

                            if (url == "")
                                url = w.ServerRelativeUrl;

                            w.Close();
                            break;
                        case "planner":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/workplanner.aspx?listID=" + Request["listId"] + "&ID=" + Request["id"];
                            w.Close();
                            break;
                        case "plannerwp":
                        case "workplan":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/tasks.aspx?ID=" + Request["id"];
                            w.Close();
                            break;
                        case "planneragile":
                        case "agile":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/agile/tasks.aspx?ID=" + Request["id"];
                            w.Close();
                            break;
                        case "tasks":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/Lists/Task Center?FilterField1=" + Request["FilterField1"] + "&FilterValue1=" + Request["FilterValue1"];
                            w.Close();
                            break;
                        case "version":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                SPListItem li = list.GetItemById(int.Parse(Request["ID"]));
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/versions.aspx?List=" + Request["listid"] + "&ID=" + Request["ID"] + "&Filename=" + HttpUtility.UrlEncode(li.Url);
                            }
                            w.Close();
                            break;
                        case "edit":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/" + list.Forms[PAGETYPE.PAGE_EDITFORM].Url + "?ID=" + Request["ID"];
                            }
                            w.Close();
                            break;
                        case "view":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/" + list.Forms[PAGETYPE.PAGE_DISPLAYFORM].Url + "?ID=" + Request["ID"];
                            }
                            w.Close();
                            break;
                        case "delete":
                            w = GetWeb(site);
                            w.AllowUnsafeUpdates = true;
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];

                                if (list.Title.ToLower().Equals("resources"))
                                {
                                    w.AllowUnsafeUpdates = true;
                                    SPListItem item = list.GetItemById(int.Parse(Request["ID"]));
                                    int extId;
                                    string deleteResourceCheckStatus = string.Empty;
                                    string deleteResourceCheckMessage = string.Empty;

                                    if (!int.TryParse(item["EXTID"] as string, out extId))
                                    {
                                        return;
                                    }

                                    var args = new object[] { extId, item.UniqueId, w, deleteResourceCheckMessage, deleteResourceCheckStatus };
                                    Assembly assembly = Assembly.Load("WorkEnginePPM, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5");
                                    Type type = assembly.GetType("WorkEnginePPM.Core.ResourceManagement.Utilities", true, true);
                                    type.GetMethod("PerformDeleteResourceCheck", BindingFlags.Public | BindingFlags.Static).Invoke(null, args);

                                    deleteResourceCheckStatus = Convert.ToString(args[3]);
                                    deleteResourceCheckMessage = Convert.ToString(args[4]);

                                    if (!string.IsNullOrEmpty(deleteResourceCheckStatus) && deleteResourceCheckStatus.ToLower().Equals("no"))
                                    {
                                        ClientScript.RegisterStartupScript(this.GetType(), "canceldelete", "javascript:alert('hi');");
                                        throw new Exception(deleteResourceCheckMessage);
                                    }
                                    else
                                    {
                                        list.ParentWeb.AllowUnsafeUpdates = true;
                                        list.GetItemById(int.Parse(Request["ID"])).Recycle();
                                        //url = Request["Source"];
                                        data = "Success";
                                    }
                                }
                                else
                                {
                                    list.ParentWeb.AllowUnsafeUpdates = true;
                                    list.GetItemById(int.Parse(Request["ID"])).Recycle();
                                    //url = Request["Source"];
                                    data = "Success";
                                }
                            }
                            w.Close();
                            break;
                        case "deletemulti":

                            string[] items = Request["Items"].Split(',');
                            foreach (string item in items)
                            {
                                if (item != "")
                                {
                                    try
                                    {
                                        string[] sItemInfo = item.Split('.');
                                        using (SPWeb web = site.OpenWeb(new Guid(sItemInfo[0])))
                                        {
                                            web.AllowUnsafeUpdates = true;
                                            {
                                                SPList list = web.Lists[new Guid(sItemInfo[1])];
                                                list.GetItemById(int.Parse(sItemInfo[2])).Recycle();
                                                //url = Request["Source"];
                                                data = "Success";
                                            }
                                        }
                                    }
                                    catch { }
                                }
                            }
                            break;
                        case "subscribe":
                            w = GetWeb(site);
                            {
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/subnew.aspx?List=" + Request["listid"] + "&ID=" + Request["ID"];
                            }
                            w.Close();
                            break;
                        case "approve":
                            w = GetWeb(site);
                            {
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/approve.aspx?List=" + Request["listid"] + "&ID=" + Request["ID"];
                            }
                            w.Close();
                            break;
                        case "attachfile":
                            w = GetWeb(site);
                            {
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/attachfilemulti.aspx?ListId=" + Request["listid"] + "&ItemId=" + Request["ID"];
                            }
                            w.Close();
                            break;
                        case "viewedit2":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/listedititem.aspx?ListId=" + Request["listid"] + "&ID=" + Request["ID"] + "&Mode=" + Request["mode"] + "&Source=" + HttpUtility.UrlEncode(w.ServerRelativeUrl) + "%2F%5Flayouts%2Fepmlive%2Flistedititem%2Easpx%3Fclose%3D1%26ListId%3D" + Request["listid"] + "%26ID%3D" + Request["ID"] + "&gridid=" + Request["gridid"] + "&siteid=" + w.Site.ID + "&webid=" + w.ID + "&rowid=" + Request["rowid"];
                            }
                            w.Close();
                            break;
                        case "perms":
                            w = GetWeb(site);
                            {
                                SPList list = w.Lists[new Guid(Request["listid"])];
                                url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/user.aspx?obj=" + Request["listid"] + "," + Request["id"] + ",LISTITEM&LIST=" + Request["listid"] + "&Source=" + HttpUtility.UrlEncode(w.ServerRelativeUrl) + "%2F%5Flayouts%2Fepmlive%2Flistedititem%2Easpx%3Fclose%3D1%26ListId%3D" + Request["listid"] + "%26ID%3D" + Request["ID"] + "&gridid=" + Request["gridid"] + "&siteid=" + w.Site.ID + "&webid=" + w.ID + "&rowid=" + Request["rowid"];
                            }
                            w.Close();
                            break;
                        case "workflows":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/Workflow.aspx?ID=" + Request["id"] + "&List={" + Request["listid"] + "}";
                            w.Close();
                            break;
                        case "comments":
                            w = GetWeb(site);
                            url = ((w.ServerRelativeUrl == "/") ? "" : w.ServerRelativeUrl) + "/_layouts/epmlive/comments.aspx?itemID=" + Request["id"] + "&Listid=" + Request["listid"];
                            w.Close();
                            break;
                        case "getcontextmenus":
                            try
                            {
                                w = GetWeb(site);
                                {
                                    data = getmenus(w);
                                }
                            }
                            catch { }
                            break;
                        case ArchiveProjectAction:
                            data = ArchiveRestoreProject(site, true);
                            break;
                        case RestoreProjectAction:
                            data = ArchiveRestoreProject(site, false);
                            break;
                        case "errormessage":
                            data = Request["message"];
                            break;
                        default:
                            data = "Unknown Action";
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                data = "General Error: " + ex.Message;
                //ClientScript.RegisterStartupScript(this.GetType(), "closeWindow", "<script language=\"javascript\">javascript:alert('hi'); return false;</script>");
            }
            if (url != "")
            {
                url = url.Substring(0, 10) + url.Substring(10).Replace("//", "/");

                string source;

                if ((System.Web.HttpUtility.UrlDecode(Request["source"]) != null
                    && System.Web.HttpUtility.UrlDecode(Request["source"]).Equals(Request["source"]))
                    || (!string.IsNullOrEmpty(Request["source"]) && Request["source"].ToLower().Contains("fd_item_")))
                {
                    source = System.Web.HttpUtility.UrlEncode(Request["source"]).Replace("+", "%20");
                }
                else
                {
                    source = Request["source"];
                }

                if (source != null && source != "")
                {
                    if (url.Contains("?"))
                        url += "&Source=" + source.Replace("#", "");
                    else
                        url += "?Source=" + source.Replace("#", "");
                }
                try
                {
                    //EPML-5263, EPML-5224 - Code Change: Rather then using Equals, at some of the places, it returns value for isDlg as isDlg=1,1. Hence, used Contains which worked in all the cases.
                    if (Request["action"].ToLower() != "workspace" && (Request["isDlg"] != null && Convert.ToString(Request["isDlg"]).Contains("1") || HttpContext.Current.Request.UrlReferrer.OriginalString.ToLower().Contains("&isdlg=1")))
                        url += "&IsDlg=1";
                }
                catch { }
                Response.Redirect(url);
            }
            if (script != "")
            {

            }
        }

        private string ArchiveRestoreProject(SPSite site, bool archive)
        {
            if (site == null)
            {
                throw new ArgumentNullException(nameof(site));
            }

            using (var web = GetWeb(site))
            {
                SPGroup group = web.Groups.GetByName("Administrators");

                if (!web.IsCurrentUserMemberOfGroup(group.ID) && !web.CurrentUser.IsSiteAdmin)
                    throw new UnauthorizedAccessException();

                var list = web.Lists[new Guid(Request[ArhiveRestoreListIdRequestParameter])];
                var listItem = list.GetItemById(int.Parse(Request[ArchiveRestoreItemIdRequestParameter]));
                var service = new ProjectArchiverService();

                if (archive)
                {
                    service.ArchiveProject(listItem);
                }
                else
                {
                    service.RestoreProject(listItem);
                }
                return SuccessMessage;
            }
        }
    }
}