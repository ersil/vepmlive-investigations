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
        
        protected global::System.Web.UI.WebControls.Label lblView;
        
        protected global::System.Web.UI.WebControls.DropDownList ddWorkspaceCenterView;
        
        protected global::Microsoft.SharePoint.WebControls.ScriptBlock workspaceCenterScriptBlock1;
        
        public static implicit operator global::System.Web.UI.TemplateControl(WorkSpaceCenter target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.Label @__BuildControllblView() {
            global::System.Web.UI.WebControls.Label @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.Label();
            this.lblView = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "lblView";
            @__ctrl.Text = "View :";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.ListItem @__BuildControl__control3() {
            global::System.Web.UI.WebControls.ListItem @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.ListItem();
            @__ctrl.Text = "All Items";
            @__ctrl.Value = "AllItems";
            @__ctrl.Selected = true;
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.ListItem @__BuildControl__control4() {
            global::System.Web.UI.WebControls.ListItem @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.ListItem();
            @__ctrl.Text = "My Workspace";
            @__ctrl.Value = "MyWorkspace";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.ListItem @__BuildControl__control5() {
            global::System.Web.UI.WebControls.ListItem @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.ListItem();
            @__ctrl.Text = "My Favorite";
            @__ctrl.Value = "MyFavorite";
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControl__control2(System.Web.UI.WebControls.ListItemCollection @__ctrl) {
            global::System.Web.UI.WebControls.ListItem @__ctrl1;
            @__ctrl1 = this.@__BuildControl__control3();
            @__ctrl.Add(@__ctrl1);
            global::System.Web.UI.WebControls.ListItem @__ctrl2;
            @__ctrl2 = this.@__BuildControl__control4();
            @__ctrl.Add(@__ctrl2);
            global::System.Web.UI.WebControls.ListItem @__ctrl3;
            @__ctrl3 = this.@__BuildControl__control5();
            @__ctrl.Add(@__ctrl3);
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.WebControls.DropDownList @__BuildControlddWorkspaceCenterView() {
            global::System.Web.UI.WebControls.DropDownList @__ctrl;
            @__ctrl = new global::System.Web.UI.WebControls.DropDownList();
            this.ddWorkspaceCenterView = @__ctrl;
            @__ctrl.ApplyStyleSheetSkin(this.Page);
            @__ctrl.ID = "ddWorkspaceCenterView";
            ((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("onchange", "changeView()");
            this.@__BuildControl__control2(@__ctrl.Items);
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::Microsoft.SharePoint.WebControls.ScriptBlock @__BuildControlworkspaceCenterScriptBlock1() {
            global::Microsoft.SharePoint.WebControls.ScriptBlock @__ctrl;
            @__ctrl = new global::Microsoft.SharePoint.WebControls.ScriptBlock();
            this.workspaceCenterScriptBlock1 = @__ctrl;
            @__ctrl.ID = "workspaceCenterScriptBlock1";
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__RenderworkspaceCenterScriptBlock1));
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__RenderworkspaceCenterScriptBlock1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write("\r\n            $(function () {\r\n                var loadWorkspaceCenterGrid = func" +
                    "tion () {\r\n                    window.TreeGrid(\'<treegrid data_url=\"");
                                                 @__w.Write( WebUrl );

            @__w.Write("/_vti_bin/WorkEngine.asmx\" data_timeout=\"0\" data_method=\"Soap\" data_function=\"Exe" +
                    "cute\" data_namespace=\"workengine.com\" data_param_function=\"GetWorkspaceCenterGri" +
                    "dData\" data_param_dataxml=\"AllItems\" layout_url=\"");
                                                                                                                                                                                                                                                                                @__w.Write( WebUrl );

            @__w.Write("/_vti_bin/WorkEngine.asmx\" layout_timeout=\"0\" layout_method=\"Soap\" layout_functio" +
                    "n=\"Execute\" layout_namespace=\"workengine.com\" layout_param_function=\"WorkSpaceCe" +
                    "nterLayout\" suppressmessage=\"3\" ");
                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              @__w.Write( DebugTag );

            @__w.Write("></treegrid>\', \'EPMWorkspaceCenterGrid\');\r\n                };\r\n                Ex" +
                    "ecuteOrDelayUntilScriptLoaded(loadWorkspaceCenterGrid, \'EPMLive.js\');\r\n         " +
                    "   });\r\n          \r\n        ");
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControlTree(global::EPMLiveWebParts.WorkSpaceCenter.WorkSpaceCenter @__ctrl) {
            global::System.Web.UI.WebControls.Label @__ctrl1;
            @__ctrl1 = this.@__BuildControllblView();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
            global::System.Web.UI.WebControls.DropDownList @__ctrl2;
            @__ctrl2 = this.@__BuildControlddWorkspaceCenterView();
            @__parser.AddParsedSubObject(@__ctrl2);
            global::Microsoft.SharePoint.WebControls.ScriptBlock @__ctrl3;
            @__ctrl3 = this.@__BuildControlworkspaceCenterScriptBlock1();
            @__parser.AddParsedSubObject(@__ctrl3);
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write(@"
<script type=""text/javascript"" src=""../_layouts/15/epmlive/TreeGrid/GridE.js""></script>
<style type=""text/css"">
      #EPMWorkspaceCenterGrid {
        margin: 10px auto;
        padding: 5px;
        position: relative;
        display: inline-block;
    }
    #DDLView {
       text-align:right;
       padding-right:7px;
    }


</style>
<script type=""text/javascript"">
    function changeView() {
        $(""#myWorkSpace_Search"").val('');
        var source = Grids[0].Source;
        source.Data.url = '");
                   @__w.Write( WebUrl );

            @__w.Write("/_vti_bin/WorkEngine.asmx\';\r\n        source.Data.Function = \'Execute\';\r\n        s" +
                    "ource.Data.Param.Function = \'GetWorkspaceCenterGridData\';\r\n        source.Data.P" +
                    "aram.Dataxml = $(\"#");
                                @__w.Write(ddWorkspaceCenterView.ClientID);

            @__w.Write(@""").val();
        Grids[""gridWorkSpaceCenter""].Reload(source, null, false);
    }

    $(document).ready(function () {
        $(""#myWorkSpace_Search"").keyup(function (e) {
            var query = $(this).val();
            var count = GetGrids();
            var grid = Grids[""gridWorkSpaceCenter""];
            if (query.length > 0) {
                grid.ChangeFilter('WorkSpace', query.toLowerCase(), 11, 0, 1, null);
            }
            else {
                grid.ChangeFilter('WorkSpace', query.toLowerCase(), 12, 0, 1, null);
            }
            grid.Update();
            grid.Render();
        });
    });
    function createNewWorkspace() {
        var createNewWorkspaceUrl = """);
                             @__w.Write( WebUrl );

            @__w.Write("/_layouts/epmlive/QueueCreateWorkspace.aspx\";\r\n        var options = {\r\n         " +
                    "   url: createNewWorkspaceUrl, width: 1000, height: 600, title: \'Create\', dialog" +
                    "ReturnValueCallback: function (dialogResult, returnValue) {\r\n                if " +
                    "(dialogResult === 1) {\r\n                    toastr.options = {\r\n                " +
                    "        \'private closeButton\': false,\r\n                        \'debug\': false,\r\n" +
                    "                        \'positionClass\': \'toast-top-right\',\r\n                   " +
                    "     \'onclick\': null,\r\n                        \'showDuration\': \'300\',\r\n         " +
                    "               \'hideDuration\': \'1000\',\r\n                        \'timeOut\': \'5000" +
                    "\',\r\n                        \'extendedTimeOut\': \'1000\',\r\n                        " +
                    "\'showEasing\': \'swing\',\r\n                        \'hideEasing\': \'linear\',\r\n       " +
                    "                 \'showMethod\': \'fadeIn\',\r\n                        \'hideMethod\': " +
                    "\'fadeOut\'\r\n                    }\r\n                    toastr.info(\'Your workspac" +
                    "e is being created - we will notify you when it is ready.\');\r\n                }\r" +
                    "\n            }\r\n        };\r\n        SP.UI.ModalDialog.showModalDialog(options);\r" +
                    "\n    }\r\n</script>\r\n<div id=\"EPMWorkspaceCenter\" style=\"width: 100%;\">\r\n    <div " +
                    "id=\"btnNew\">\r\n          <a href=\"javascript:createNewWorkspace();\" class=\"ms-cor" +
                    "e-menu-root ms-textXLarge\">\r\n              <img title=\"Add new\" alt=\"\" src=\"/_la" +
                    "youts/epmlive/images/newitem5.png\"/>\r\n              new item\r\n          </a>\r\n  " +
                    "  </div>\r\n    <div id=\"DDLView\">\r\n        <input type=\"text\" id=\"myWorkSpace_Sea" +
                    "rch\" class=\"ms-cui-tb mwg-watermark\" style=\"width: 100px;\">\r\n        ");
            parameterContainer.Controls[0].RenderControl(@__w);
            @__w.Write("\r\n        ");
            parameterContainer.Controls[1].RenderControl(@__w);
            @__w.Write("\r\n\r\n    </div>\r\n    <div id=\"EPMWorkspaceCenterGrid\" style=\"width: 100%; height: " +
                    "400px;\">\r\n        ");
            parameterContainer.Controls[2].RenderControl(@__w);
            @__w.Write("\r\n    </div>\r\n</div>\r\n");
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
