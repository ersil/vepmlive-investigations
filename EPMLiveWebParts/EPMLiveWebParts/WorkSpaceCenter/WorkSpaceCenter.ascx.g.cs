﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPMLiveWebParts.WorkSpaceCenter {
    using System.Web.UI.WebControls.Expressions;
    using System.Web.UI.HtmlControls;
    using System.Collections;
    using System.Text;
    using System.Web.UI;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;
    using Microsoft.SharePoint.WebPartPages;
    using System.Web.SessionState;
    using System.Configuration;
    using Microsoft.SharePoint;
    using System.Web;
    using System.Web.DynamicData;
    using System.Web.Caching;
    using System.Web.Profile;
    using System.ComponentModel.DataAnnotations;
    using System.Web.UI.WebControls;
    using System.Web.Security;
    using System;
    using Microsoft.SharePoint.Utilities;
    using System.Text.RegularExpressions;
    using System.Collections.Specialized;
    using System.Web.UI.WebControls.WebParts;
    using Microsoft.SharePoint.WebControls;
    
    
    public partial class WorkSpaceCenter {
        
        public static implicit operator global::System.Web.UI.TemplateControl(WorkSpaceCenter target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControlTree(global::EPMLiveWebParts.WorkSpaceCenter.WorkSpaceCenter @__ctrl) {
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write("\r\n\r\n<style type=\"text/css\">\r\n    #EPMWorkspaceCenterGrid {\r\n        margin: 10px " +
                    "auto;\r\n        padding: 5px;\r\n        position: relative;\r\n        display: inli" +
                    "ne-block;\r\n    }\r\n\r\n    .workspacecentercontextmenu {\r\n        list-style: none;" +
                    "\r\n        cursor: pointer;\r\n        position: absolute;\r\n    }\r\n\r\n        .works" +
                    "pacecentercontextmenu .icon-ellipsis-horizontal:after {\r\n            content: \'." +
                    "..\';\r\n            position: relative;\r\n            top: -10px;\r\n            left" +
                    ": 0px;\r\n        }\r\n\r\n        .workspacecentercontextmenu .epm-menu-btn span {\r\n " +
                    "           font-size: 2em;\r\n            color: #0090CA;\r\n            opacity: .6" +
                    ";\r\n        }\r\n\r\n            .workspacecentercontextmenu .epm-menu-btn span:hover" +
                    " {\r\n                opacity: 1;\r\n            }\r\n\r\n        .workspacecentercontex" +
                    "tmenu ul.epm-nav-contextual-menu {\r\n            right: -20px !important;\r\n      " +
                    "      top: -20px !important;\r\n        }\r\n\r\n            .workspacecentercontextme" +
                    "nu ul.epm-nav-contextual-menu li {\r\n                height: 23px;\r\n             " +
                    "   line-height: 23px;\r\n            }\r\n\r\n                .workspacecentercontextm" +
                    "enu ul.epm-nav-contextual-menu li a {\r\n                    text-decoration: none" +
                    ";\r\n                    display: inline-block;\r\n                }\r\n\r\n            " +
                    "    .workspacecentercontextmenu ul.epm-nav-contextual-menu li span.epm-nav-cm-ic" +
                    "on {\r\n                    top: 0px !important;\r\n                }\r\n</style>\r\n\r\n<" +
                    "script type=\"text/javascript\">\r\n\r\n    $(function () {\r\n        ExecuteOrDelayUnt" +
                    "ilScriptLoaded(WorkspaceCenterClient.init, \'EPMLive.js\');\r\n    });\r\n\r\n    var Wo" +
                    "rkspaceCenterClient = (function () {\r\n\r\n        var init = function () {\r\n      " +
                    "      loadWorkspaceCenterGrid();\r\n            toolbarCfg();\r\n        };\r\n\r\n     " +
                    "   var toolbarCfg = function () {\r\n            var cfgs = [\r\n                {\r\n" +
                    "                    \'placement\': \'left\',\r\n                    \'content\': [\r\n    " +
                    "                // invite button\r\n                    {\r\n                       " +
                    " \'controlId\': \'btnNew\',\r\n                        \'controlType\': \'button\',\r\n     " +
                    "                   \'iconClass\': \'fui-plus\',\r\n                        \'title\': \'n" +
                    "ew item\',\r\n                        \'events\': [\r\n                            {\r\n " +
                    "                               \'eventName\': \'click\',\r\n                          " +
                    "      \'function\': function () { createNewWorkspace(); },\r\n                      " +
                    "      }\r\n                        ]\r\n                    }\r\n                    ]" +
                    "\r\n                },\r\n                {\r\n                    \'placement\': \'right" +
                    "\',\r\n                    \'content\': [\r\n                    //search control\r\n    " +
                    "                {\r\n                        \'controlId\': \'genericId\',\r\n          " +
                    "              \'controlType\': \'search\',\r\n                        \'custom\': \'yes\'," +
                    "\r\n                        \'customControlId\': \'\'\r\n                    },\r\n       " +
                    "             //search control\r\n                    {\r\n                        \'c" +
                    "ontrolId\': \'myWorkSpace_Search1\',\r\n                        \'controlType\': \'searc" +
                    "h\',\r\n                        \'custom\': \'no\',\r\n                        \'events\': " +
                    "[{\r\n                            \'eventName\': \'keyup\',\r\n                         " +
                    "   \'function\': function (e) {\r\n                                var query = $(thi" +
                    "s).val();\r\n                                var count = GetGrids();\r\n            " +
                    "                    var grid = Grids[\"gridWorkSpaceCenter\"];\r\n                  " +
                    "              if (query.length > 0) {\r\n                                    grid." +
                    "ChangeFilter(\'WorkSpace\', query.toLowerCase(), 11, 0, 1, null);\r\n               " +
                    "                 }\r\n                                else {\r\n                    " +
                    "                grid.ChangeFilter(\'WorkSpace\', query.toLowerCase(), 12, 0, 1, nu" +
                    "ll);\r\n                                }\r\n                                grid.Up" +
                    "date();\r\n                                grid.Render();\r\n                       " +
                    "     }\r\n                        }\r\n                        ]\r\n                  " +
                    "  },\r\n                    //view control\r\n                    {\r\n               " +
                    "         \'controlId\': \'ddWorkspaceCenterView1\',\r\n                        \'contro" +
                    "lType\': \'dropdown\',\r\n                        \'title\': \'View:\',\r\n                " +
                    "        \'value\': \'All Items\',\r\n                        \'iconClass\': \'none\',\r\n   " +
                    "                     \'sections\': [\r\n                            {\r\n             " +
                    "                   \'heading\': \'none\',\r\n                                \'divider\'" +
                    ": \'yes\',\r\n                                \'options\': [\r\n                        " +
                    "            {\r\n                                        \'iconClass\': \'none\',\r\n   " +
                    "                                     \'text\': \'All Items\',\r\n                     " +
                    "                   \'events\': [\r\n                                            {\r\n " +
                    "                                               \'eventName\': \'click\',\r\n          " +
                    "                                      \'function\': function () { changeView(\"AllI" +
                    "tems\"); }\r\n                                            }\r\n                      " +
                    "                  ]\r\n\r\n                                    },\r\n                 " +
                    "                   {\r\n                                        \'iconClass\': \'none" +
                    "\',\r\n                                        \'text\': \'My Workspace\',\r\n           " +
                    "                             \'events\': [\r\n                                      " +
                    "      {\r\n                                                \'eventName\': \'click\',\r\n" +
                    "                                                \'function\': function () { change" +
                    "View(\"MyWorkspace\"); }\r\n                                            }\r\n         " +
                    "                               ]\r\n                                    },\r\n      " +
                    "                              {\r\n                                        \'iconCl" +
                    "ass\': \'none\',\r\n                                        \'text\': \'My Favorite\',\r\n " +
                    "                                       \'events\': [\r\n                            " +
                    "                {\r\n                                                \'eventName\': " +
                    "\'click\',\r\n                                                \'function\': function (" +
                    ") { changeView(\"MyFavorite\"); }\r\n                                            }\r\n" +
                    "                                        ]\r\n                                    }" +
                    "\r\n\r\n                                ]\r\n                            }\r\n          " +
                    "              ]\r\n                    }\r\n                    ]\r\n                }" +
                    "\r\n            ];\r\n            epmLiveGenericToolBar.generateToolBar(\'WorkSpacece" +
                    "nterToolbarMenu\', cfgs);\r\n        };\r\n\r\n        var changeView = function (curre" +
                    "ntView) {\r\n            var source = Grids[\"gridWorkSpaceCenter\"].Source;\r\n      " +
                    "      source.Data.url = \'");
                       @__w.Write( WebUrl );

            @__w.Write(@"/_vti_bin/WorkEngine.asmx';
            source.Data.Function = 'Execute';
            source.Data.Param.Function = 'GetWorkspaceCenterGridData';
            source.Data.Param.Dataxml = currentView;
            Grids[""gridWorkSpaceCenter""].Reload(source, null, false);
        };

        var createNewWorkspace = function () {
            var createNewWorkspaceUrl = """);
                                 @__w.Write( WebUrl );

            @__w.Write(@"/_layouts/epmlive/QueueCreateWorkspace.aspx"";
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
            window.TreeGrid('<treegrid data_url=""");
                                         @__w.Write( WebUrl );

            @__w.Write("/_vti_bin/WorkEngine.asmx\" data_timeout=\"0\" data_method=\"Soap\" data_function=\"Exe" +
                    "cute\" data_namespace=\"workengine.com\" data_param_function=\"GetWorkspaceCenterGri" +
                    "dData\" data_param_dataxml=\"AllItems\" layout_url=\"");
                                                                                                                                                                                                                                                                        @__w.Write( WebUrl );

            @__w.Write("/_vti_bin/WorkEngine.asmx\" layout_timeout=\"0\" layout_method=\"Soap\" layout_functio" +
                    "n=\"Execute\" layout_namespace=\"workengine.com\" layout_param_function=\"WorkSpaceCe" +
                    "nterLayout\" suppressmessage=\"3\" ");
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                      @__w.Write( DebugTag );

            @__w.Write(@"></treegrid>', 'EPMWorkspaceCenterGrid');
        };

        return {
            init: init
        };

    })();

    Grids.OnRenderFinish = function (grid) {
        if (grid.id == 'gridWorkSpaceCenter') {
            $("".workspacecentercontextmenu"").each(function () {
                window.epmLiveNavigation.addFavoriteWSMenu($(this));
            });
        }
    };
</script>
<div id=""EPMWorkspaceCenter"" style=""width: 100%;"">
    <div id=""WorkSpacecenterToolbarMenu"" style=""width: 99%"">
    </div>
    <div id=""EPMWorkspaceCenterGrid"" style=""width: 100%; height: 400px;"">
    </div>
</div>
");
        }
        
        private void InitializeControl() {
            this.@__BuildControlTree(this);
            this.Load += new global::System.EventHandler(this.Page_Load);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual object Eval(string expression) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        protected virtual string Eval(string expression, string format) {
            return global::System.Web.UI.DataBinder.Eval(this.Page.GetDataItem(), expression, format);
        }
    }
}
