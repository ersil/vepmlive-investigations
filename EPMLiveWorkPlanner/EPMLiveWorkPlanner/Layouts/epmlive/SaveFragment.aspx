﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaveFragment.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.SaveFragment" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

    <script language="javascript" type="text/javascript">

        $(function () {
            var xDataXml = window.parent.Grids.WorkPlannerGrid.GetXmlData();
            var hdnTaskFragmentXml = document.getElementById('<%=hdnTaskFragmentXml.ClientID%>');
            hdnTaskFragmentXml.value = xDataXml;
        });

        function closeSaveFragmentPopup() {
            window.frameElement.commonModalDialogClose(1, 1);
        }
    </script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <center>
        <asp:ValidationSummary ID="ValidationSummary1" runat="server" ShowSummary="false"></asp:ValidationSummary>
    <table width="100%">
        <tr>
            <td>
                <asp:Label runat="server" ID="lblFragmentName" Text="Fragment Name: " ></asp:Label>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtFragmentName" Text="" TabIndex="0" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="fragmentNameValidate" runat="server" Display="Dynamic" ErrorMessage="Please enter fragment name" ForeColor="Red" ControlToValidate="txtFragmentName" ></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblDescription" Text="Description: "></asp:Label>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtDescription" Text="" Rows="3" TextMode="MultiLine" Width="200px"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblTag" Text="Tag: "></asp:Label>
            </td>
            <td>
                <asp:TextBox runat="server" ID="txtTag" Text="" Width="200px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="tagValidate" runat="server" Display="Dynamic" ErrorMessage="Please enter tag name for this fragment" ControlToValidate="txtTag" ForeColor="Red"  ></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td>
                <asp:Label runat="server" ID="lblScope" Text="Scope: "></asp:Label>
            </td>
            <td>
                <asp:RadioButtonList ID="rdoScope" runat="server" Width="200px">
                    <asp:ListItem Text="Private" Selected="True" Value="true"></asp:ListItem>
                    <asp:ListItem Text="Public" Selected="False" Value="false"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        </table>

        <br />

        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
        <asp:Button ID="btnClose" runat="server" Text="Close" OnClientClick="javascript:return closeSaveFragmentPopup();" />
        </center>

    <asp:HiddenField ID="hdnTaskFragmentXml" runat="server" />

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
    Save Fragment
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server">
    Save Fragment
</asp:Content>
