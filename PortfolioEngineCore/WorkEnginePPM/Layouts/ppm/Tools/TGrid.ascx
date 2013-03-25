﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %> 
<%@ Register Tagprefix="WebPartPages" Namespace="Microsoft.SharePoint.WebPartPages" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TGrid.ascx.cs" Inherits="WorkEnginePPM.TGrid" %>

<asp:ScriptManagerProxy ID="sm1" runat="server">
    <Services>
    </Services>
</asp:ScriptManagerProxy>


<asp:HiddenField ID="hiddenTableData" runat="server"></asp:HiddenField>
<div id="<%=ClientID%>_treegrid_div" style="width:300px;height:300px;overflow:hidden;"></div>
<script type="text/javascript">
    try {
        var params = {};
        params.ClientID = '<%=ClientID%>';
        params.treegrid_div = "<%=ClientID%>_treegrid_div";
        params.tg_id = 'tg_<%=UID%>';
        params.tg_uid = '<%=UID%>';
        params.Data = document.getElementById('<%=hiddenTableData.ClientID%>').value;
        window['<%=UID%>'] = new TGrid(params);
        document.getElementById('<%=hiddenTableData.ClientID%>').value = "";
    }
    catch (e) {
        alert("TGrid ascx Initialization : " + e.toString());
    }

</script>
