﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Columns.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.Integration.Columns" DynamicMasterPageFile="~masterurl/default.master" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Register Tagprefix="wssawc" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %> 


<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">

<%=PageHead %>

        <table width="100%">    

            <wssuc:InputFormSection ID="InputFormSection2" Title="Id Field"
	            Description=""
	            runat="server">
	            <Template_Description>
	                This field comes from the integration source and maps the integration item to a SharePoint Item. This is required for all integration modes.
	            </Template_Description>
	            <Template_InputFormControls>
		            <wssuc:InputFormControl ID="InputFormControl2" LabelText="Select Field" runat="server">
			                <Template_Control>
			                    <asp:DropDownList ID="ddlIDColumn" runat="server"></asp:DropDownList>
			                </Template_Control>
		            </wssuc:InputFormControl>
	            </Template_InputFormControls>
            </wssuc:InputFormSection>

            <wssuc:InputFormSection ID="InputFormSection3" Title="SharePoint Field"
	            Description=""
	            runat="server">
	            <Template_Description>
	                This column maps the SharePoint ID into the Integration Item. This is optional and required if you want the integration item to have information about SharePoint.
	            </Template_Description>
	            <Template_InputFormControls>
		            <wssuc:InputFormControl ID="InputFormControl3" LabelText="" runat="server">
			                <Template_Control>
			                    <asp:DropDownList ID="ddlSPColumn" runat="server">
                                    <asp:ListItem Text="--Select Column--" Value=""></asp:ListItem>
                                </asp:DropDownList>
			                </Template_Control>
		            </wssuc:InputFormControl>
	            </Template_InputFormControls>
            </wssuc:InputFormSection>

            <wssuc:InputFormSection ID="InputFormSectionMatch" Title="Item Matching"
	            Description=""
	            runat="server">
	            <Template_Description>
	                Use this feature to enable mapping items from an external source to an item in your list. Select the field from each environment you would like to compare when mapping data.
	            </Template_Description>
	            <Template_InputFormControls>
		            <wssuc:InputFormControl ID="InputFormControlMatch" LabelText="List Field" runat="server">
			                <Template_Control>
			                    <asp:DropDownList ID="ddlSharePointMatch" runat="server">
                                    <asp:ListItem Text="--Select Column--" Value=""></asp:ListItem>
                                </asp:DropDownList>
			                </Template_Control>
		            </wssuc:InputFormControl>
                    <wssuc:InputFormControl ID="InputFormControlMatch2" LabelText="Integration Field" runat="server">
			                <Template_Control>
			                    <asp:DropDownList ID="ddlIntegrationMatch" runat="server">
                                    <asp:ListItem Text="--Select Column--" Value=""></asp:ListItem>
                                </asp:DropDownList>
			                </Template_Control>
		            </wssuc:InputFormControl>
	            </Template_InputFormControls>
            </wssuc:InputFormSection>

            <wssuc:InputFormSection ID="InputFormSection4" Title="Column Mapping"
	            Description=""
	            runat="server">
	            <Template_Description>
	                Column Mapping. Select the Integration Columns you would like to map to these list columns.
	            </Template_Description>
	            <Template_InputFormControls>
		            <wssuc:InputFormControl ID="InputFormControl4" LabelText="" runat="server">
			                <Template_Control>
			                    <asp:Panel ID="pnlColumns" runat="server">
                                    
                                </asp:Panel>
			                </Template_Control>
		            </wssuc:InputFormControl>
	            </Template_InputFormControls>
            </wssuc:InputFormSection>

            <wssuc:ButtonSection ID="ButtonSection1" runat="server">
		        <Template_Buttons>
			        <asp:PlaceHolder ID="PlaceHolder1" runat="server">
				        <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="Button1_Click" Text="Save Settings" id="Button1" accesskey="" Width="150"/>
			        </asp:PlaceHolder>
		        </Template_Buttons>
	        </wssuc:ButtonSection>
        </table>

</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Integration Columns
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Integration Columns
</asp:Content>
