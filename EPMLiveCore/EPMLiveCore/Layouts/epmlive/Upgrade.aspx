﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Upgrade.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.Upgrade" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<style type="text/css">
	
		.button-green:hover {
			background: #71BF31;
			border: 1px solid #60BA16;
			filter: none;
		}

		.button-green {
			font-weight: bold;
			background: #6CC325 1px;
			background: -webkit-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
			background: -o-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
			background: -ms-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
			background: -moz-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
			border: 1px solid #6ABD3D;
			color: white;
			filter: progid:DXImageTransform.Microsoft.gradient(startColorstr='#9FD870', endColorstr='#6CC325', GradientType=0);
		}

		.button {
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
			font-family: arial, sans-serif !important;
			text-decoration: none !important;
			text-align: center;
		}

		.button {
			color: #FFFFFF !important;
			font-size: 16px !important;
			padding: 8px 15px;
		}

		.button-green-disabled {
			opacity: .5;
			filter: alpha(opacity=50);
			cursor: default;
			background: #6CC325 1px;
			background: -webkit-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
			background: -o-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
			background: -ms-linear-gradient(top, #9FD870 1px, #6CC325 1px, #6ABD3C 100%);
			background: -moz-linear-gradient(top, #9FD870 1px, #98DB62 1px, #6ABD3C 100%);
			border: 1px solid #6ABD3D;
			color: white;
		}

		.upgradeheader {
			font-family: Segoe UI Light, Segoe, Helvetica;
			font-size: 24px;
			padding-bottom: 20px;
		}

		.upgradetext {
			font-family: Segoe UI Light, Segoe, Helvetica;
			font-size: 16px;
		}

	</style>
	
	<script type="text/javascript">
		 window.epmLiveUpgradeEnabled = false;

		 window.allowEpmLiveUpgrade = function() {
			 if (document.getElementById("chkTerms").checked == true) {
				 window.epmLiveUpgradeEnabled = true;
				 document.getElementById('btnUpgrade').className = "button button-green";
			 } else {
				 window.epmLiveUpgradeEnabled = false;
				 document.getElementById('btnUpgrade').className = "button button-green-disabled";
			 }
		 };

		 window.upgradeEpmLive = function() {
			 if (window.epmLiveUpgradeEnabled) {
				 var options = { url: 'PerformUpgrade.aspx', width: 500, height: 250, showClose: true };
				 SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
			 }
		 };
	</script>

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
	<asp:Label ID="lblError" runat="server" Text="" Visible="False"></asp:Label>
	<asp:Panel ID="pnlMain" runat="server">
		<div style="padding:20px">
			<div style="width: 600px;margin-bottom: 50px;">
				<div class="upgradeheader">Welcome the EPM Live Upgrader</div>
				<div class="upgradetext">
					<p>This page will allow you to upgrade this site to the most recent version of EPM Live.  By clicking the "I Agree" checkbox below, you are agreeing that this site will be upgraded to the most recent version of EPM Live and all of its associated functionality. Although EPM Live fully supports upgrades, please understand that your existing business processes may change after the upgrade. We suggest creating a new trial site prior to the upgrade in order to understand the implications of this upgrade.</p>
					<p>Also, please visit our <a href="http://support.epmlive.com" style="color: #438DE8; text-decoration: underline;" target="_blank">Support Community</a> for important upgrade announcements.</p>
				</div>
			</div>

			<div class="controls">
				<p><asp:Label ID="NotRootMessageLabel" runat="server" Text="You are choosing to run the upgrader on a site other than the root site. Are you sure you want to run the upgrader on this site?" Visible="False" ForeColor="Red"></asp:Label></p>
				<label class="checkbox inline">
					<input type="checkbox" id="chkTerms" onclick="javascript:window.allowEpmLiveUpgrade();"/><span>I Agree.</span>
				</label>
			</div>

			<div style="padding-top:10px;">
				<a class="button button-green-disabled" href="javascript:void(0);" id="btnUpgrade" onclick="javascript:window.upgradeEpmLive();">Upgrade Now</a>
			</div>
		</div>
	</asp:Panel>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Upgrade EPM Live
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Upgrade EPM Live
</asp:Content>
