﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Log.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.Integration.Log" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
<style>
    .logrow
    {
        border: 0px;
        padding: 5px;
        padding-left: 15px;
    }
    .logheader
    {
        border: 0px;
        padding: 5px;
        text-align: left;
        padding-left: 15px;
    }
</style>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
<div style="padding: 10px">
    View Level: <asp:DropDownList ID="ddlLevel" runat="server" AutoPostBack="true">
    <asp:ListItem Value="1" Text="View All"></asp:ListItem>
    <asp:ListItem Value="2" Text="Warnings and Errors"></asp:ListItem>
    <asp:ListItem Value="3" Text="Errors Only" Selected="True"></asp:ListItem>
    </asp:DropDownList> &nbsp;<a href="IntegrationList.aspx?LIST=<%=Request["LIST"] %>">Back To Integrations</a>
    <br /><br />
    <asp:GridView ID="gvLog" runat="server" AutoGenerateColumns="false" HeaderStyle-HorizontalAlign="Left" RowStyle-BorderStyle="None" RowStyle-CssClass="logrow" BorderStyle="None">
        <Columns>
            <asp:ImageField DataImageUrlField="LOGTYPE" DataImageUrlFormatString="../images/integration/status{0}.gif" ItemStyle-CssClass="logrow" HeaderStyle-CssClass="logheader"></asp:ImageField>
            <asp:BoundField DataField="DTLOGGED" HeaderText="Date"  ItemStyle-CssClass="logrow" HeaderStyle-CssClass="logheader"/>
            <asp:BoundField DataField="LOGTEXT" HeaderText="Message" ItemStyle-CssClass="logrow" HeaderStyle-CssClass="logheader"/>
        
        </Columns>
    </asp:GridView>
</div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Integration Log
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Integration Log
</asp:Content>
