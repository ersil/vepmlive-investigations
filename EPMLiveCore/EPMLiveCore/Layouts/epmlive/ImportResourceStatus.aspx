﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=16.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ImportResourceStatus.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.ImportResourceStatus" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
	<link rel="stylesheet" type="text/css" href="/_layouts/epmlive/stylesheets/libraries/bootstrap/css/bootstrap.min.css"/>
	<script src="/_layouts/epmlive/javascripts/libraries/bootstrap.min.js" type="text/javascript"></script>
	
    <style type="text/css">
        .allLog {
            font-weight: bold;
            color: #000000;
            padding-right: 5px;
            cursor: pointer;
        }

        .infoLog {
            font-weight: bold;
            color: #3399FF;
            padding-right: 5px;
            cursor: pointer;
        }

        .warningLog {
            font-weight: bold;
            color: #FF9900;
            padding-right: 5px;
            cursor: pointer;
        }

        .errorLog {
            font-weight: bold;
            color: #FF0000;
            padding-right: 5px;
            cursor: pointer;
        }

        .hero-unit {
            padding: 30px;
            line-height: 1em;
        }

            .hero-unit > p {
                margin: 0;
            }

        textarea[readonly] {
            height: 285px;
            cursor: text;
        }

        #importdetailstable-wrap {
            height: 285px;
            overflow: auto;
        }

        #importdetailslog-wrap {
            height: 250px;
            overflow: auto;
        }

        div, p, th, td {
            font-family: 'Segoe UI', 'Segoe', Tahoma, Helvetica, Arial, sans-serif;
        }

        .container-wrap {
            height: 565px;
        }

        .accordion-inner {
            height: 285px !important;
        }

        .ms-core-overlay {
            overflow: hidden !important;
        }
    </style>

	<!--[if IE]>
	<style type="text/css">
		textarea[readonly] {
			height: 250px;
		}
		
		#resourcetable-wrap {
			height: 250px;
		}

		.container-wrap {
			height: 500px;
		}
	</style>
	<![endif]-->

	<script type="text/javascript">
	    window.epmLive = window.epmLive || {};
	    window.epmLive.jobId = '<%= JobId %>';	    
    </script>
	<script src="/_layouts/epmlive/javascripts/resourceimporter.js<%= Version %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server" Visible="False">
	<asp:Label ID="MissingJobIdErrorLabel" runat="server" Text="No Job ID was provided." ForeColor="Red" Visible="False"></asp:Label>
	<asp:Panel ID="Panel" runat="server" Visible="False" CssClass="container-wrap">
		<div id="epmcontainer" class="container">
			<div class="hero-unit">
				<div class="row-fluid">
					<div class="span11">
						<div class="progress progress-striped" data-bind="css: { active: PercentComplete() < 100 }">
						  <div class="bar" data-bind="style: { width: PercentComplete() + '%' }"></div>
						</div>
					</div>
					<div class="span1" style="font-family: 'Segoe UI', 'Segoe', Tahoma, Helvetica, Arial, sans-serif;" data-bind="text: PercentComplete() + '%'"/>
				</div>
			</div>
			<div class="row-fluid"><p style="font-family: 'Segoe UI', 'Segoe', Tahoma, Helvetica, Arial, sans-serif;" data-bind="text: CurrentProcess()"/></div>
		</div>
		<div class="accordion" id="status">
		  <div class="accordion-group">
			<div class="accordion-heading">
			  <a class="accordion-toggle" data-toggle="collapse" data-parent="#status" href="#details" style="font-family: 'Segoe UI', 'Segoe', Tahoma, Helvetica, Arial, sans-serif;">
				Import Details
			  </a>
			</div>
            <div id="details" class="accordion-body collapse in">
                    <div class="accordion-inner">
                        <div class="row-fluid" id="importdetailstable-wrap">
                            <table class="table table-bordered" id="importdetailstable">
                                <tr>
                                    <td>Total Records</td>
                                    <td data-bind="text: TotalRecords"></td>
                                </tr>
                                <tr>
                                    <td>Processed Records</td>
                                    <td data-bind="text: ProcessedRecords"></td>
                                </tr>
                                <tr>
                                    <td>Success Records</td>
                                    <td data-bind="text: SuccessRecords"></td>
                                </tr>
                                <tr>
                                    <td>Failed Records</td>
                                    <td data-bind="text: FailedRecords"></td>
                                </tr>
                                <tr>
                                    <td>Log Count</td>
                                    <td data-bind="text: log().length"></td>
                                </tr>
                            </table>
                        </div>
                    </div>
                </div>
		  </div>
		  <div class="accordion-group">
			<div class="accordion-heading">
			  <a class="accordion-toggle" data-toggle="collapse" data-parent="#status" href="#log" style="font-family: 'Segoe UI', 'Segoe', Tahoma, Helvetica, Arial, sans-serif;">
				Import Log
			  </a>
			</div>
            <div id="log" class="accordion-body collapse">
                    <div class="accordion-inner">
                        <div class="row-fluid">
                            <div>
                                <a id="lnkall" style="display: none;" class="allLog" onclick="javascript:ImportResourceStatusClient.filterLogByStatus('All');">All</a>
                                <a id="lnkinfo" style="display: none;" class="infoLog" onclick="javascript:ImportResourceStatusClient.filterLogByStatus('Info');">Info</a>
                                <a id="lnkwarning" style="display: none;" class="warningLog" onclick="javascript:ImportResourceStatusClient.filterLogByStatus('Warning');">Warning</a>
                                <a id="lnkerror" style="display: none;" class="errorLog" onclick="javascript:ImportResourceStatusClient.filterLogByStatus('Error');">Error</a>
                            </div>
                            <br />
                            <div class="row-fluid" id="importdetailslog-wrap">

                                <table class="table table-bordered" id="importdetailslog">
                                    <thead>
                                        <tr>
                                            <th width="10">#</th>
                                            <th width="10">Status</th>
                                            <th width="270">Description</th>
                                        </tr>
                                    </thead>
                                    <tbody data-bind="foreach: log ">
                                        <tr>
                                            <td data-bind="text: $index() + 1"></td>
                                            <td data-bind="attr: { 'class': Kind == '0' ? 'infoLog' : Kind == '1' ? 'warningLog' : 'errorLog' }">
                                                <!-- ko if: Kind == '0' -->
                                                Info
                                                <!-- /ko -->
                                                <!-- ko if: Kind == '1' -->
                                                Warning
                                                <!-- /ko -->
                                                <!-- ko if: Kind == '2' -->
                                                Error
                                                <!-- /ko -->
                                            </td>
                                            <td>
                                                <div data-bind="html: Message"></div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
		  </div>
		</div>
		<input type="button" value="Close" onclick="parent.SP.UI.ModalDialog.commonModalDialogClose(parent.SP.UI.DialogResult.OK); return false;" style="float: right; position: relative; top: -5px;">
        <input id="CancelImport" type="button" value="Cancel" onclick="cancelImportResourceJob();" style="float: right; position: relative; top: -5px;">
	</asp:Panel>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Importing Resources
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
Importing Resources
</asp:Content>