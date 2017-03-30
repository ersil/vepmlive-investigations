﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="v2licensing.aspx.cs" Inherits="EPMLiveAccountManagement.Layouts.epmlive.v2licensing" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<style>

    .button-green:hover 
{
background: #71BF31;
border: 1px solid #60BA16;
filter: none;
}

.button-green
{
font-weight:bold;
background: #6CC325 1px;
background: -webkit-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
background: -o-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
background: -ms-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
background: -moz-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
border: 1px solid #6ABD3D;
color: white;
filter: progid:DXImageTransform.Microsoft.gradient( startColorstr='#9FD870', endColorstr='#6CC325',GradientType=0 );
}

.button 
{
font-size: 18px;
-moz-border-radius: 5px;
-webkit-border-radius: 5px;
-o-border-radius: 5px;
-ms-border-radius: 5px;
-khtml-border-radius: 5px;
display: inline-block;
line-height: 24px;
margin-bottom: 4px;
font-weight: normal;
text-shadow: none;
padding: 8px 10px 1px 10px;
white-space: nowrap;
cursor: pointer;
font-family: arial, sans-serif;
text-decoration: none !important;
text-align: center;
}

</style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<div style="padding: 10px;line-height: 20px;">

<font style="font-family: Helvetica, Verdana, sans-serif; font-size: 20px;"><asp:Label ID="lblVersion" runat="server"/></font><br />

    Account #: <%=sAccount %>
    <%=sTrial %>
    <br /><br />
    <font style="font-family: Helvetica, Verdana, sans-serif; font-size: 14px; font-weight: bold">Licenses:</font><br />
    <%=sPurchased %><br />
    <%=sInUse %>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Licensing
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Licensing
</asp:Content>
