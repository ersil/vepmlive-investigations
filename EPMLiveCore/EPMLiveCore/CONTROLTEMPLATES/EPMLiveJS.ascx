﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EPMLiveJS.ascx.cs" Inherits="EPMLiveCore.ControlTemplates.EPMLiveJS" %>

<script type="text/javascript">
    (function() {

        function onJqueryLoaded() {
            $.getScript = function(url, callback, cache) {
                $.ajax({
                    type: 'GET',
                    url: url,
                    success: callback,
                    dataType: 'script',
                    cache: cache
                });
            };

            $.getScript('<%= WebUrl %>/_layouts/15/epmlive/javascripts/libraries/jquery.tmpl.min.js?v=<%= EPMFileVersion %>', function() {
                $.getScript('<%= WebUrl %>/_layouts/15/epmlive/javascripts/libraries/knockout-2.2.1.js?v=<%= EPMFileVersion %>', function() {
                    $.getScript('<%= WebUrl %>/_layouts/15/epmlive/javascripts/epmlive<%= DebugMode ? ".min" : string.Empty %>.js?v=<%= EPMFileVersion %>', function() {
                        epmLive.currentSiteId = '<%= SiteId %>';
                        epmLive.currentSiteUrl = '<%= SiteUrl %>';
                        epmLive.currentWebId = '<%= WebId %>';
                        epmLive.currentWebUrl = '<%= WebUrl %>';
                        epmLive.currentWebFullUrl = '<%= WebFullUrl %>';
                        epmLive.fileVersion = '<%= EPMFileVersion %>';

                        window.SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('EPMLive.js');
                    }, true);
                }, true);
            }, true);

<%--            //EPML-5445
            //Looks like this control is NOT used
            $(function () {
                var supportintegration = '<%= SupportIntegration %>';
                if (supportintegration.toLowerCase() == 'true') {
                    var zendesk = document.createElement('script');
                    zendesk.type = 'text/javascript';
                    zendesk.async = true;
                    zendesk.src = "../../_layouts/15/epmlive/javascripts/libraries/EPMLiveFooter.js";
                    var s = document.getElementsByTagName('script')[0];
                    s.parentNode.insertBefore(zendesk, s);
                }
            });
            //EPML-5445--%>
        }

        ExecuteOrDelayUntilScriptLoaded(onJqueryLoaded, 'jquery.min.js');
    })();
</script>