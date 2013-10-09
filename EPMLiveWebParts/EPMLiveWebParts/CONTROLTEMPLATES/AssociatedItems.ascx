﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AssociatedItems.ascx.cs" Inherits="EPMLiveWebParts.AssociatedItems.AssociatedItems" %>

<style type="text/css">
    .slidingDiv {
        width: 100%;
        padding: 20px;
        border: 1px thin black;
        -webkit-box-shadow: 0px 3px 20px rgba(50, 50, 50, 0.8);
        -moz-box-shadow: 0px 3px 20px rgba(50, 50, 50, 0.8);
        box-shadow: 0px 3px 20px rgba(50, 50, 50, 0.8);
        position: absolute;
        width: 200px;
        background-color: white;
    }

    .slidingDivClose {
        float: right;
        font-size: large;
    }

    .slidingDivHeader {
        float: left;
        color: black;
        font-size: large;
    }

    .slidingDivAdd {
        float: right;
    }

    .listMainDiv {
        float: left;
        padding: 5px;
        margin-right: 5px;
    }

    .pipeSeperator {
        float: left;
        font-size: large;
    }
</style>

<script type="text/javascript">

    $(function () {
        fillWebPartData();
    });

    function fillWebPartData() {
        if (dataXml != '') {
            EPMLiveCore.WorkEngineAPI.Execute("GetAssociatedItems", dataXml, function (response) {
                var divHTML = response.toString().replace("<Result Status=\"0\">", "").replace("</Result>", "");
                $("#<%=associatedItemsDiv.ClientID%>").html("");
                $("#<%=associatedItemsDiv.ClientID%>").html(divHTML);
                $(".slidingDiv").hide();

                $(".listMainDiv").mouseover(function () {
                    $(".slidingDiv").hide();
                    $(this).find(".slidingDiv").show();
                });
                $(".slidingDiv").mouseover(function () {
                    $(this).show();
                });
                $("#<%=associatedItemsDiv.ClientID%>").mouseout(function () {
                    $(".slidingDiv").hide();
                });
            });
        }
    }

    function showItemUrl(weburl) {
        var options = { url: weburl, showMaximized: true, dialogReturnValueCallback: function (dialogResult) { fillWebPartData(); } };
        SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
    }

    function showItemPopup(siteurl, webid, listid, itemid) {
        showSharePointPopup(siteurl + '/_layouts/epmlive/gridaction.aspx?action=getcontextmenus&webid=' + webid +
            '&listid=' + listid + '&ID=' + itemid, null, false, true, null, {
                gridId: "myDiv",
                rowId: "myDiv",
                col: "myDiv"
            }, 300, 400);
    }

    function emptyFunction() {
    }

    function showSharePointPopup(url, title, allowMaximize, showClose, func, funcParams, width, height) {
        if (allowMaximize == null) allowMaximize = true;
        if (showClose == null) showClose = true;
        if (func == null) func = emptyFunction;

        var options;

        if (width !== undefined && height !== undefined) {
            options = {
                title: title,
                allowMaximize: allowMaximize,
                showClose: showClose,
                url: url,
                dialogReturnValueCallback: Function.createCallback(Function.createDelegate(null, func), funcParams),
                width: width,
                height: height
            };
        } else {
            options = { title: title, allowMaximize: allowMaximize, showClose: showClose, url: url, dialogReturnValueCallback: Function.createCallback(Function.createDelegate(null, func), funcParams) };
        }

        SP.UI.ModalDialog.showModalDialog(options);
    }

</script>

<div id="associatedItemsDiv" runat="server" style="float: left;"></div>
<asp:Label ID="lblError" runat="server" Text="This page is not a display form page. Please add this webpart on a display form page." ForeColor="Red"></asp:Label>
