﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="EPMLiveJS.ascx.cs" Inherits="EPMLiveCore.ControlTemplates.EPMLiveJS" %>

<script src="<%= WebUrl %>/_layouts/epmlive/javascripts/libraries/jquery.min.js" type="text/javascript"></script>

<script type="text/javascript">
    function initializeEPMLiveJS() {
        $.getScript = function (url, callback, cache) {
            $.ajax({
                type: 'GET',
                url: url,
                success: callback,
                dataType: 'script',
                cache: cache
            });
        };

        $.getScript('<%= WebUrl %>/_layouts/epmlive/javascripts/libraries/jquery.tmpl.min.js', function () {
            $.getScript('<%= WebUrl %>/_layouts/epmlive/javascripts/libraries/jquery-ui.min.js', function () {
                $.getScript('<%= WebUrl %>/_layouts/epmlive/javascripts/libraries/knockout-2.2.1.js', function () {
                    $.getScript('<%= WebUrl %>/_layouts/epmlive/xml2json.js', function () {
                        $.getScript('<%= WebUrl %>/_layouts/epmlive/MD5.js', function () {
                            $.getScript('<%= WebUrl %>/_layouts/epmlive/javascripts/jquery.multiselect.min.js', function () {
                                $.getScript('<%= WebUrl %>/_layouts/epmlive/javascripts/EPMLive.js', function () {
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
                    }, true);
                }, true);
            }, true);
        }, true);
    }

    ExecuteOrDelayUntilScriptLoaded(initializeEPMLiveJS, 'jquery.min.js');
</script>
