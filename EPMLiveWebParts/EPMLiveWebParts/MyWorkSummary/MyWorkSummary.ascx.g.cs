﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18052
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPMLiveWebParts.MyWorkSummary {
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
    
    
    public partial class MyWorkSummary {
        
        protected global::System.Web.UI.HtmlControls.HtmlGenericControl myWorkSummaryItemsDiv;
        
        public static implicit operator global::System.Web.UI.TemplateControl(MyWorkSummary target) 
        {
            return target == null ? null : target.TemplateControl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private global::System.Web.UI.HtmlControls.HtmlGenericControl @__BuildControlmyWorkSummaryItemsDiv() {
            global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl;
            @__ctrl = new global::System.Web.UI.HtmlControls.HtmlGenericControl("div");
            this.myWorkSummaryItemsDiv = @__ctrl;
            @__ctrl.ID = "myWorkSummaryItemsDiv";
            ((System.Web.UI.IAttributeAccessor)(@__ctrl)).SetAttribute("style", "float: left;");
            return @__ctrl;
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__BuildControlTree(global::EPMLiveWebParts.MyWorkSummary.MyWorkSummary @__ctrl) {
            global::System.Web.UI.HtmlControls.HtmlGenericControl @__ctrl1;
            @__ctrl1 = this.@__BuildControlmyWorkSummaryItemsDiv();
            System.Web.UI.IParserAccessor @__parser = ((System.Web.UI.IParserAccessor)(@__ctrl));
            @__parser.AddParsedSubObject(@__ctrl1);
            @__ctrl.SetRenderMethodDelegate(new System.Web.UI.RenderMethod(this.@__Render__control1));
        }
        
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Never)]
        private void @__Render__control1(System.Web.UI.HtmlTextWriter @__w, System.Web.UI.Control parameterContainer) {
            @__w.Write(@"
<style type=""text/css"">
    .pipeSeperator {
        float: left;
        font-size: large;
    }

    .listMainDiv {
        float: left;
        padding: 5px;
        margin-right: 5px;
    }
</style>


<script type=""text/javascript"">

    $(function () {
        fillMyWorkSummaryData();
    });

    function fillMyWorkSummaryData() {
        if (dataXmlMyWorkSummary != '') {

            $(""#");
        @__w.Write(myWorkSummaryItemsDiv.ClientID);

            @__w.Write(@""").hide();
            $('#myWorkSummaryLoadDiv').show();

            EPMLiveCore.WorkEngineAPI.Execute(""GetMyWorkSummary"", dataXmlMyWorkSummary, function (response) {
                var divHTML = response.toString().replace(""<Result Status=\""0\"">"", """").replace(""</Result>"", """");
                $(""#");
            @__w.Write(myWorkSummaryItemsDiv.ClientID);

            @__w.Write("\").html(\"\");\r\n                $(\"#");
            @__w.Write(myWorkSummaryItemsDiv.ClientID);

            @__w.Write("\").html(divHTML);\r\n\r\n                $(\'#myWorkSummaryLoadDiv\').hide();\r\n        " +
                    "        $(\"#");
            @__w.Write(myWorkSummaryItemsDiv.ClientID);

            @__w.Write(@""").show();

            });
        }
    }

    function displayMyWorkItemsByFilter() {
        alert(""This functionality is under development, sorry for inconvenience."");
    }

</script>

<div id=""myWorkSummaryLoadDiv"" style=""align-content: center"">
    <img src=""../_layouts/15/epmlive/images/mywork/loading16.gif"" />
</div>
");
            parameterContainer.Controls[0].RenderControl(@__w);
            @__w.Write("\r\n\r\n");
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
