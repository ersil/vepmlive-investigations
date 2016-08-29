﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register TagPrefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="WorkSpaceCenter.ascx.cs" Inherits="EPMLiveWebParts.WorkSpaceCenter.WorkSpaceCenter" %>

<style type="text/css">
    #EPMWorkspaceCenterGrid {
        margin: 10px auto;
        padding: 5px;
        position: relative;
        display: inline-block;
    }

    .workspacecentercontextmenu {
        list-style: none;
        cursor: pointer;
        position: absolute;
    }

        .workspacecentercontextmenu .icon-ellipsis-horizontal:after {
            content: '...';
            position: relative;
            top: -10px;
            left: 0px;
        }

        .workspacecentercontextmenu .epm-menu-btn span {
            font-size: 2em;
            color: #0090CA;
            opacity: .6;
        }

            .workspacecentercontextmenu .epm-menu-btn span:hover {
                opacity: 1;
            }

        .workspacecentercontextmenu ul.epm-nav-contextual-menu {
            right: -20px !important;
            top: -20px !important;
        }

            .workspacecentercontextmenu ul.epm-nav-contextual-menu li {
                height: 23px;
                line-height: 23px;
            }

                .workspacecentercontextmenu ul.epm-nav-contextual-menu li a {
                    text-decoration: none;
                    display: inline-block;
                }

                .workspacecentercontextmenu ul.epm-nav-contextual-menu li span.epm-nav-cm-icon {
                    top: 0px !important;
                }
</style>

<script type="text/javascript">

    $(function () {
        ExecuteOrDelayUntilScriptLoaded(WorkspaceCenterClient.init, 'EPMLive.js');
    });

    var WorkspaceCenterClient = (function () {

        var init = function () {
            loadWorkspaceCenterGrid();
            toolbarCfg();
        };

        var toolbarCfg = function () {
            var cfgs = [
                {
                    'placement': 'left',
                    'content': [
                    // invite button
                    {
                        'controlId': 'btnNew',
                        'controlType': 'button',
                        'iconClass': 'fui-plus',
                        'title': 'new item',
                        'events': [
                            {
                                'eventName': 'click',
                                'function': function () { createNewWorkspace(); },
                            }
                        ]
                    }
                    ]
                },
                {
                    'placement': 'right',
                    'content': [
                    //search control
                    {
                        'controlId': 'genericId',
                        'controlType': 'search',
                        'custom': 'yes',
                        'customControlId': ''
                    },
                    //search control
                    {
                        'controlId': 'myWorkSpace_Search1',
                        'controlType': 'search',
                        'custom': 'yes',
                        'events': [{
                            'eventName': 'keyup',
                            'function': function (e) {
                                var query = $(this).val();
                                var count = GetGrids();
                                var grid = Grids["gridWorkSpaceCenter"];
                                if (query.length > 0) {
                                    grid.ChangeFilter('WorkSpace', query, 11, 0, 1, null);
                                }
                                else {
                                    grid.ChangeFilter('WorkSpace', query, 12, 0, 1, null);
                                }
                                grid.Update();
                                grid.Render();
                            }
                        }
                        ]
                    },
                    //view control
                    {
                        'controlId': 'ddWorkspaceCenterView1',
                        'controlType': 'dropdown',
                        'title': 'View:',
                        'value': 'All Items',
                        'iconClass': 'none',
                        'sections': [
                            {
                                'heading': 'none',
                                'divider': 'yes',
                                'options': [
                                    {
                                        'iconClass': 'none',
                                        'text': 'All Items',
                                        'events': [
                                            {
                                                'eventName': 'click',
                                                'function': function () { changeView("AllItems"); }
                                            }
                                        ]

                                    },
                                    {
                                        'iconClass': 'none',
                                        'text': 'My Workspace',
                                        'events': [
                                            {
                                                'eventName': 'click',
                                                'function': function () { changeView("MyWorkspace"); }
                                            }
                                        ]
                                    },
                                    {
                                        'iconClass': 'none',
                                        'text': 'My Favorite',
                                        'events': [
                                            {
                                                'eventName': 'click',
                                                'function': function () { changeView("MyFavorite"); }
                                            }
                                        ]
                                    }

                                ]
                            }
                        ]
                    }
                    ]
                }
            ];
            epmLiveGenericToolBar.generateToolBar('WorkSpacecenterToolbarMenu', cfgs);
        };

        var changeView = function (currentView) {
            EPM.UI.Loader.current().startLoading({ id: 'EPMWorkspaceCenterLoadingDiv' });
            var source = Grids["gridWorkSpaceCenter"].Source;
            source.Data.url = '<%= WebUrl %>/_vti_bin/WorkEngine.asmx';
            source.Data.Function = 'Execute';
            source.Data.Param.Function = 'GetWorkspaceCenterGridData';
            source.Data.Param.Dataxml = currentView;
            Grids["gridWorkSpaceCenter"].Reload(source, null, false);
        };

        var createNewWorkspace = function () {
            var createNewWorkspaceUrl = "<%= WebUrl %>/_layouts/epmlive/QueueCreateWorkspace.aspx";
            var options = {
                url: createNewWorkspaceUrl, width: 1000, height: 600, title: 'Create', dialogReturnValueCallback: function (dialogResult, returnValue) {
                    if (dialogResult === 1) {
                        toastr.options = {
                            'private closeButton': false,
                            'debug': false,
                            'positionClass': 'toast-top-right',
                            'onclick': null,
                            'showDuration': '300',
                            'hideDuration': '1000',
                            'timeOut': '5000',
                            'extendedTimeOut': '1000',
                            'showEasing': 'swing',
                            'hideEasing': 'linear',
                            'showMethod': 'fadeIn',
                            'hideMethod': 'fadeOut'
                        }
                        toastr.info('Your workspace is being created - we will notify you when it is ready.');
                    }
                }
            };
            SP.UI.ModalDialog.showModalDialog(options);
        };

        var loadWorkspaceCenterGrid = function () {
            EPM.UI.Loader.current().startLoading({ id: 'EPMWorkspaceCenterLoadingDiv' });
            window.TreeGrid('<treegrid data_url="<%= WebUrl %>/_vti_bin/WorkEngine.asmx" data_timeout="0" data_method="Soap" data_function="Execute" data_namespace="workengine.com" data_param_function="GetWorkspaceCenterGridData" data_param_dataxml="AllItems" layout_url="<%= WebUrl %>/_vti_bin/WorkEngine.asmx" layout_timeout="0" layout_method="Soap" layout_function="Execute" layout_namespace="workengine.com" layout_param_function="WorkSpaceCenterLayout" suppressmessage="3" <%= DebugTag %>></treegrid>', 'EPMWorkspaceCenterGrid');
        };

        return {
            init: init
        };

    })();

    Grids.OnRenderFinish = function (grid) {
        if (grid.id == 'gridWorkSpaceCenter') {
            EPM.UI.Loader.current().stopLoading('EPMWorkspaceCenterLoadingDiv');
            $(".workspacecentercontextmenu").each(function () {
                window.epmLiveNavigation.addFavoriteWSMenu($(this));
            });
        }
    };
</script>
<div id="EPMWorkspaceCenter">
    <div id="EPMWorkspaceCenterLoadingDiv">
        <div id="WorkSpacecenterToolbarMenu" style="width: 99%">
        </div>
        <div id="EPMWorkspaceCenterGrid" style="width: 100%; height: 400px;">
        </div>
    </div>
</div>
