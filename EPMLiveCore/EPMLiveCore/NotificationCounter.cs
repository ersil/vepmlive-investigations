﻿using System.Web.UI;
using System.Web.UI.WebControls;
using Microsoft.SharePoint;

namespace EPMLiveCore
{
    [ToolboxData("<{0}:NotificationCounter runat=server></{0}:NotificationCounter>")]
    public class NotificationCounter : WebControl, INamingContainer
    {
        #region Methods (1) 

        // Protected Methods (1) 

        /// <summary>
        /// Renders the contents of the control to the specified writer. This method is used primarily by control developers.
        /// </summary>
        /// <param name="writer">A <see cref="T:System.Web.UI.HtmlTextWriter"/> that represents the output stream to render HTML content on the client.</param>
        protected override void RenderContents(HtmlTextWriter writer)
        {
            string url = SPContext.Current.Web.SafeServerRelativeUrl();
            
            writer.Write(@"<link href=""{0}/_layouts/epmlive/stylesheets/notifications.css"" rel=""stylesheet"" type=""text/css"" />", url);

            writer.Write(@"
                <div id=""EPMLiveNotificationCounter"" class=""ms-qatbutton"" 
                    data-bind=""style: { background: epmLiveNotifications.notificationCounterColor(), fontWeight: epmLiveNotifications.notificationCounterFontWeight() }"" 
                    style=""background: #666666;"">
                    <span id=""EPMLiveNotificationCount"" data-bind=""text: epmLiveNotifications.totalNewNotifications() > 99 ? '99+' : epmLiveNotifications.totalNewNotifications()"">&nbsp;</span>
                </div>");

            writer.Write(@"<div id=""EPMLiveNotificationCounterScreen"" class=""ms-qatbutton"" style=""height: 27px; width: 30px; top: -12px; left: -28px;"">&nbsp;</div>");
        }

        #endregion Methods 
    }
}
