﻿using System.Web.UI;
using System.Web.UI.WebControls;
using EPMLiveCore.CONTROLTEMPLATES;
using Microsoft.SharePoint.WebControls;

namespace EPMLiveCore
{
    [MdsCompliant(true)]
    [ToolboxData("<{0}:NotificationCounterV2 runat=server></{0}:NotificationCounterV2>")]
    public class NotificationCounterV2 : WebControl, INamingContainer
    {
        private const string ASCX_PATH = @"~/_controltemplates/EPMLiveNotificationCounterV2.ascx";

        protected override void CreateChildControls()
        {
            var control = (EPMLiveNotificationCounterV2) Page.LoadControl(ASCX_PATH);
            Controls.Add(control);
        }
    }
}