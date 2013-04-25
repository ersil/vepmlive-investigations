﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MyWorkControl.ascx.cs" Inherits="EPMLiveWebParts.CONTROLTEMPLATES.MyWork.MyWorkControl" %>

<div id="EPMAllWork">

    <style type="text/css">
        .EPMLiveMyWorkGroup {
            border-bottom: 1px solid #ABABAB;
        }
        
        .EPMLiveMyWorkGroupTitle {
            padding-top: 14px;
            font-weight: bold;
            font-weight: bold;
            height: 22px;
            padding-bottom: 3px;
        }
        
        .EPMLiveMyWorkGroupTitle > a {
            color: #444444 !important;
            text-decoration: none !important;
        }
        
        .EPMLiveMyWorkGroupPrefix {
            color: #777777;
        }
        
        .EPMLiveMyWorkGroupPostfix {
            font-weight: normal;
        }
    </style>

    <script type="text/javascript">
        function initializeEPMLoader() {
            window.myWorkLoader = SP.UI.ModalDialog.showWaitScreenWithNoClose(SP.Res.dialogLoading15);
        }

        SP.SOD.executeOrDelayUntilScriptLoaded(initializeEPMLoader, "sp.js");
    </script>
    
    <div id="MWG_Header" style="display: <%= ShowToolbar ? "block" : "none" %>">
        <h2>My Work</h2>
        <div id="MWG_Pivot">
            <div id="MWG_Pivot_Active" class="mwg-menuitem selected" data-filter="active">Active</div>
            <div id="MWG_Pivot_Completed" class="mwg-menuitem" data-filter="completed">Completed</div>
            <div id="MWG_Pivot_Selector" class="mwg-menuitem selector">...</div>
            <div id="MWG_PivotMenu_wrap" class="mwg-menuitem">
                <div id="MWG_PivotMenu">
                    <ul>
                        <li class="mwg-mi-clear" data-filter="clear">Clear Filter</li>
                        <li class="mwg-smi">
                            <span>Work Types</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_WorkType_Menu"></ul>
                                </div>
                            </div>
                        </li>
                        <li class="mwg-smi">
                            <span>Created By</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_CreatedBy_Menu"></ul>
                                </div>
                            </div>
                        </li>
                        <li class="mwg-smi">
                            <span>Priority</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_Priority_Menu"></ul>
                                </div>
                            </div>
                        </li>
                        <li class="mwg-smi">
                            <span>Status</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_Status_Menu"></ul>
                                </div>
                            </div>
                        </li>
                        <li class="mwg-smi">
                            <span>Flag</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_Flag_Menu"></ul>
                                </div>
                            </div>
                        </li>
                        <li class="mwg-smi">
                            <span>Due</span>
                            <div class="mwg-submenu-wrap">
                                <div class="mwg-submenu">
                                    <ul id="MWG_Due_Menu"></ul>
                                </div>
                            </div>
                        </li>
                    </ul>
                    <div id="MWG_PivotClose">Close</div>
                </div>
            </div>
            <div id="MWG_Pivot_Search" class="mwg-menuitem"><input id="MWG_Search" type="text"/></div>
        </div>
    </div>
    
    <div id="EPMMyWorkGrid" style="width:100%;height:800px;">

        <script type="text/javascript">
            $(function () {
                var loadMyWorkGrid = function () {
                    window.TreeGrid('<treegrid Data_Url="<%= WebUrl %>/_vti_bin/WorkEngine.asmx" Data_Timeout="0" Data_Method="Soap" Data_Function="Execute" Data_Namespace="workengine.com" Data_Param_Function="GetMyWorkGridData" Data_Param_Dataxml="<%= NonCompleteQuery.Replace(Environment.NewLine, string.Empty) %>" Layout_Url="<%= WebUrl %>/_vti_bin/WorkEngine.asmx" Layout_Timeout="0" Layout_Method="Soap" Layout_Function="Execute" Layout_Namespace="workengine.com" Layout_Param_Function="GetMyWorkGridLayout" Layout_Param_Dataxml="<%= NonCompleteQuery.Replace(Environment.NewLine, string.Empty) %>" SuppressMessage="3" <%= DebugTag %>></treegrid>', 'EPMMyWorkGrid');
                };

                ExecuteOrDelayUntilScriptLoaded(loadMyWorkGrid, 'EPMLive.js');
            });
        </script>

    </div>

    <div id="MWG_SaveRowHtml" style="display:none;">
      <div class="MWG_SaveRow" onClick="saveRow('gridId','rowId')">&nbsp;</div>
      <div class="MWG_CancelRowEdit" onClick="cancelRowEdit('gridId','rowId')">&nbsp;</div>
    </div>

    <div id="MWG_SaveView" style="display:none;">
      <div style="width:200px;padding:10px;"> Name:&nbsp;
        <input id="MWG_ViewName" type="text" />
        <br />
        <input id="MWG_DefaultView" type="checkbox" />
        Default View<br />
        <input id="MWG_PersonalView" type="checkbox" <%= UserHasDesignerPermission ? string.Empty : @"checked=""checked""" %> <%= UserHasDesignerPermission ? string.Empty : @"disabled=""disabled""" %> />
        Personal View<br />
        <br />
        <input type="button" style="float:left;width:90px;margin-right:5px;" value="OK" onClick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, MyWorkGrid.getSavingViewInfo(this)); return false;" class="ms-ButtonHeightWidth" target="_self" />
        <input type="button" style="float:left;width:90px;" value="Cancel" onClick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancel clicked'); return false;" class="ms-ButtonHeightWidth" target="_self" />
      </div>
    </div>

    <div id="MWG_RenameView" style="display:none;">
      <div style="width:250px;padding:10px;">Current Name:&nbsp;<span id="MWG_CurrentView"></span> <br />
        <br />
        <div style="float:left;">New Name:&nbsp;</div>
        <input id="MWG_ViewNewName" type="text" style="float:left;" />
        <br />
        <br />
        <br />
        <input type="button" style="float:left;width:90px;margin-right:5px;" value="OK" onClick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.OK, this.parentNode.children[4].value); return false;" class="ms-ButtonHeightWidth" target="_self" />
        <input type="button" style="float:left;width:90px;" value="Cancel" onClick="SP.UI.ModalDialog.commonModalDialogClose(SP.UI.DialogResult.cancel, 'Cancel clicked'); return false;" class="ms-ButtonHeightWidth" target="_self" />
      </div>
    </div>

    <script type="text/javascript">
        var siteUrl = '<%= WebUrl %>', mwWebPartHeight = <%= !string.IsNullOrEmpty(WebPartHeight) ? WebPartHeight : "0" %>, allowGlobalViewCreation = <%= UserHasDesignerPermission ? "true" : "false" %>, myWorkWebPartSelectionCheckBox = 'SelectionCbx<%= WebPartPageComponentId %>', masterView = '<%= DefaultGlobalView %>', nonCompleteQuery = '<%= NonCompleteQuery.Replace(Environment.NewLine, string.Empty) %>', completeQuery = '<%= CompleteQuery.Replace(Environment.NewLine, string.Empty) %>', newItemLists = [<%= NewItemButtonLists %>], myWorkWebPartId = '<%= WebPartId %>', myWorkWebPartQualifier = '<%= Qualifier %>', selectMyWorkWebPart = <%= TotalWebPartCount == 1 ? "true" : "false" %>, userId = <%= UserId %>, requestedView = '<%= RequestedView != null ? Convert.ToBase64String(Encoding.ASCII.GetBytes(RequestedView)) : string.Empty %>', requestedViewType = '<%= RequestedViewType != null ? Convert.ToBase64String(Encoding.ASCII.GetBytes(RequestedViewType)) : string.Empty %>', currentWebId = '<%= WebId %>', workFilters = { daysAgoEnabled: <%= DaysAgoEnabled.ToString().ToLower() %>, daysAgo: <%= DaysAgo %>, daysAfterEnabled: <%= DaysAfterEnabled.ToString().ToLower() %>, daysAfter: <%= DaysAfter %> }, mwNewItemIndicator = { enabled: <%= IsNewItemIndicatorEnabled.ToString().ToLower() %>, days: <%= NewItemIndicatorDays %> };
    </script>

</div>