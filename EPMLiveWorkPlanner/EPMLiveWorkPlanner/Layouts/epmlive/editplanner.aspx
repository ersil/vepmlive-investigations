﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Register TagPrefix="wssuc" TagName="ButtonSection" src="~/_controltemplates/ButtonSection.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormControl" src="~/_controltemplates/InputFormControl.ascx" %>
<%@ Register TagPrefix="wssuc" TagName="InputFormSection" src="~/_controltemplates/InputFormSection.ascx" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=14.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="editplanner.aspx.cs" Inherits="EPMLiveWorkPlanner.editplanner" DynamicMasterPageFile="~masterurl/default.master" %>


<asp:Content ID="ccc" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script language="javascript">
        function getWidth() {
            var winW = 640;
            if (parseInt(navigator.appVersion) > 3) {
                if (navigator.appName == "Netscape") {
                    winW = window.innerWidth;
                    //winH = window.innerHeight;
                }
                if (navigator.appName.indexOf("Microsoft") != -1) {
                    winW = document.body.offsetWidth;
                    //winH = document.body.offsetHeight;
                }
            }
            return winW;
        }
        function cancelAdd() {
            document.getElementById("Field").style.display = "none";
        }
        function addSumField() {

            document.getElementById("Field").style.top = document.getElementById("s4-workspace").scrollTop + 225;
            document.getElementById("Field").style.left = getWidth() / 2 - 125;
            document.getElementById("Field").style.position = "absolute";
            document.getElementById("Field").style.display = "";

            document.getElementById("ctl00_PlaceHolderMain_hdnId").value = "";
            document.getElementById("ctl00_PlaceHolderMain_ddlAddCalculation").value = "";
            document.getElementById("ctl00_PlaceHolderMain_txtAddField").value = "";
            document.getElementById("ctl00_PlaceHolderMain_btnAdd").value = "Add Field";

        }
        function editField(field, calc, id) {
            document.getElementById("Field").style.top = 225;
            document.getElementById("Field").style.left = getWidth() / 2 - 125;
            document.getElementById("Field").style.position = "absolute";
            document.getElementById("Field").style.display = "";

            document.getElementById("ctl00_PlaceHolderMain_hdnId").value = id;
            document.getElementById("ctl00_PlaceHolderMain_ddlAddCalculation").value = calc;
            document.getElementById("ctl00_PlaceHolderMain_txtAddField").value = field;
            document.getElementById("ctl00_PlaceHolderMain_btnAdd").value = "Save Field";
        }


        function removeField() {
            var tList = document.getElementById("<%=ddlListAvailableFields.ClientID %>");
            var sList = document.getElementById("<%=ddlListSelectedFields.ClientID %>");

            var sFields = "";
            var arrSelected = new Array();
            var count = 0;

            for (var i = 0; i < sList.length; i++) {
                if (sList.options[i].selected) {
                    var elOptNew = document.createElement('option');
                    elOptNew.text = sList.options[i].text;
                    elOptNew.value = sList.options[i].value;

                    try {
                        tList.add(elOptNew, null);
                    }
                    catch (ex) {
                        tList.add(elOptNew);
                    }

                    //tList.add(new Option(sList.options[i].text, sList.options[i].value));
                    arrSelected[count++] = sList.options[i].value;
                }
            }
            for (i = 0; i < sList.length; i++) {
                for (x = 0; x < arrSelected.length; x++) {
                    if (sList.options[i].value == arrSelected[x])
                        sList.options[i] = null;
                }
            }
            for (var i = 0; i < sList.length; i++) {
                if (sFields == "")
                    sFields = sList.options[i].value;
                else
                    sFields = sFields + "," + sList.options[i].value;
            }

            document.getElementById("statusfields").value = sFields;
        }

        function addField() {
            var sList = document.getElementById("<%=ddlListAvailableFields.ClientID %>");
            var tList = document.getElementById("<%=ddlListSelectedFields.ClientID %>");

            var sFields = document.getElementById("statusfields").value;

            var arrSelected = new Array();
            var count = 0;

            for (var i = 0; i < sList.length; i++) {
                if (sList.options[i].selected) {
                    var elOptNew = document.createElement('option');
                    elOptNew.text = sList.options[i].text;
                    elOptNew.value = sList.options[i].value;

                    try {
                        tList.add(elOptNew, null);
                    }
                    catch (ex) {
                        tList.add(elOptNew);
                    }

                    //tList.add(new Option(sList.options[i].text, sList.options[i].value));
                    arrSelected[count++] = sList.options[i].value;

                    if (sFields == "")
                        sFields = sList.options[i].value;
                    else
                        sFields = sFields + "," + sList.options[i].value;
                }
            }
            for (i = 0; i < sList.length; i++) {
                for (x = 0; x < arrSelected.length; x++) {
                    if (sList.options[i].value == arrSelected[x])
                        sList.options[i] = null;
                }
            }

            document.getElementById("statusfields").value = sFields;
        }

        function ToggleAll() {
            ToggleOnline();
            ToggleProject();
            ToggleAgile();

        }

        function ToggleOnline() {
            var chkBox = document.getElementById("<%=chkOnlinePlanner.ClientID%>");
            var spn = document.getElementById("spnOnline");

            if (chkBox.checked) {
                spn.style.display = "";
            }
            else {
                spn.style.display = "none";
            }
        }

        function ToggleProject() {
            var chkBox = document.getElementById("<%=chkProjectPlanner.ClientID%>");
            var spn = document.getElementById("spnProject");

            if (chkBox.checked) {
                spn.style.display = "";
            }
            else {
                spn.style.display = "none";
            }
        }

        function ToggleAgile() {
            var chkBox = document.getElementById("<%=chkAgilePlanner.ClientID%>");
            var chkBox2 = document.getElementById("<%=chkProjectPlanner.ClientID%>");
            var spn = document.getElementById("spnAgile");

            if (chkBox.checked) {
                chkBox2.checked = false;
                chkBox2.disabled = true; 
                spn.style.display = "";
            }
            else {
                chkBox2.disabled = false;
                spn.style.display = "none";
            }

            ToggleProject();
        }
    </script>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderId="PlaceHolderPageTitle" runat="server">
	Planner Settings
</asp:Content>
<asp:Content ID="Content1" contentplaceholderid="PlaceHolderPageTitleInTitleArea" runat="server">
	Planner Settings
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderId="PlaceHolderPageDescription" runat="server">
    
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderId="PlaceHolderMain" runat="server">
<input type="hidden" name="statusfields" id="statusfields" value="<%=statusfields%>"/>
        <table class="ms-toolbar" width="100%" cellpadding="3" style="height:10px">
            <tr><td class="ms-linksectionheader"><h3 class="ms-standardheader">General Settings</h3></td></tr>
        </table>
        <table cellpadding="0" cellspacing="0" width="100%">
            <wssuc:InputFormSection Title="Planner Name"
                Description=""
                runat="server">
                <Template_Description>
                    Enter a name for the planner
                </Template_Description>
                <Template_InputFormControls>
                    <wssuc:InputFormControl LabelText="Name" runat="server">
                        <Template_Control>
        	                <asp:TextBox ID="txtPlannerName" runat="server"></asp:TextBox>
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>
            </wssuc:InputFormSection>
            <wssuc:InputFormSection Title="Planner Description"
                Description=""
                runat="server">
                <Template_Description>
                    Enter a description for the planner
                </Template_Description>
                <Template_InputFormControls>
                    <wssuc:InputFormControl LabelText="Description" runat="server">
                        <Template_Control>
        	                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Height="50" Width="200"></asp:TextBox>
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>
            </wssuc:InputFormSection>
            <wssuc:InputFormSection Title="Planner Icon"
                Description=""
                runat="server">
                <Template_Description>
                    Enter url for an icon that will display when selected this planner.
                </Template_Description>
                <Template_InputFormControls>
                    <wssuc:InputFormControl LabelText="Icon" runat="server">
                        <Template_Control>
        	                <asp:TextBox ID="txtIcon" runat="server"></asp:TextBox>
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>
            </wssuc:InputFormSection>
            <wssuc:InputFormSection Title="Planner Availability"
                Description=""
                runat="server">
                <Template_Description>
                    Select how this planner will be used.
                </Template_Description>
                <Template_InputFormControls>
                    <wssuc:InputFormControl LabelText="Planner Availability" runat="server">
                        <Template_Control>
        	                <asp:CheckBox ID="chkOnlinePlanner" runat ="server"/> Online Planner<br />
                            <asp:CheckBox ID="chkAgilePlanner" runat ="server"/> Use Agile Functions<br />
                            <asp:CheckBox ID="chkProjectPlanner" runat ="server"/> Microsoft Project<br />
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>
            </wssuc:InputFormSection>
            <wssuc:InputFormSection Title="Disable Child Parent Capability"
                Description=""
                runat="server">
                <Template_Description>
                    Select this option to turn off the Parent Child capability.
                </Template_Description>
                <Template_InputFormControls>
                    <wssuc:InputFormControl LabelText="Disable Child Parent" runat="server">
                        <Template_Control>
        	                <asp:CheckBox ID="chkDisableParentChild" runat ="server"/> Disable
                        </Template_Control>
                    </wssuc:InputFormControl>
                </Template_InputFormControls>
            </wssuc:InputFormSection>
            <wssuc:InputFormSection Title="Source List"
	                Description=""
	                runat="server">
	                <Template_Description>
	                    Use this option to specify the Source List.
	                </Template_Description>
	                <Template_InputFormControls>
		                <wssuc:InputFormControl LabelText="" runat="server">
			                 <Template_Control>
			                    <asp:DropDownList ID="ddlProjectCenter" runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddlProjectCenter_SelectedIndexChanged">
			                    
			                    </asp:DropDownList>
			                 </Template_Control>
		                </wssuc:InputFormControl>
	                </Template_InputFormControls>
            </wssuc:InputFormSection>
                
            <wssuc:InputFormSection Title="Task List"
	                Description=""
	                runat="server">
	                <Template_Description>
	                    Use this option to specify the Task List.
	                </Template_Description>
	                <Template_InputFormControls>
		                <wssuc:InputFormControl LabelText="" runat="server">
			                 <Template_Control>
			                    <asp:DropDownList ID="ddlTaskCenter" runat="Server" AutoPostBack="True" OnSelectedIndexChanged="ddlTaskCenter_SelectedIndexChanged">
			                    
			                    </asp:DropDownList>
			                 </Template_Control>
		                </wssuc:InputFormControl>
	                </Template_InputFormControls>
                </wssuc:InputFormSection>

                <wssuc:InputFormSection Title="Lookup Field"
	                Description=""
	                runat="server">
	                <Template_Description>
	                    Use this option to specify the Field that will be used when looking up to the selected source list
	                </Template_Description>
	                <Template_InputFormControls>
		                <wssuc:InputFormControl LabelText="" runat="server">
			                 <Template_Control>
			                    <asp:DropDownList ID="ddlTaskCenterPJField" runat="Server">
			                    
			                    </asp:DropDownList>
			                 </Template_Control>
		                </wssuc:InputFormControl>
	                </Template_InputFormControls>
                </wssuc:InputFormSection>

        </table>

        <span id="spnOnline">
        <table class="ms-toolbar" width="100%" cellpadding="3" style="height:10px">
            <tr><td class="ms-linksectionheader"><h3 class="ms-standardheader">Online Planner Settings</h3></td></tr>
        </table>

        <table width="100%" class="ms-settingsframe ms-listedit">
            <tr>
                <td>
                
                    <table width="100%" cellpadding="3" style="height:10px">
                        <TR height=10><TD style="PADDING-BOTTOM: 4px; PADDING-LEFT: 4px; PADDING-RIGHT: 4px; PADDING-TOP: 4px" id=TD2 class=ms-linksectionheader colSpan=2>
                        <H3 class=ms-standardheader>Summary Row Field Calculations</H3></TD></TR>
                        <tr>
                            <td>
                                <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" Width="100%"
                                OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound" GridLines="None" CellPadding="0" CellSpacing="0">
                                    <Columns>
                                        <asp:BoundField DataField="field" HeaderText="Field" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-CssClass="ms-vb2"/>
                                        <asp:BoundField DataField="calc" HeaderText="Calculation Type" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-CssClass="ms-vb2"/>
                                        <asp:TemplateField HeaderText="" ItemStyle-VerticalAlign="top" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-CssClass="ms-vb2">
                                             <ItemTemplate>
                                                <asp:HyperLink ID="HyperLink1" runat="server" Text="Edit"></asp:HyperLink>
                                             </ItemTemplate>
                                             </asp:TemplateField>
                                             <asp:TemplateField HeaderText="" ItemStyle-VerticalAlign="top" HeaderStyle-CssClass="ms-vh2-nofilter" ItemStyle-CssClass="ms-vb2">
                                             <ItemTemplate>
                                               <asp:LinkButton ID="LinkButton1" 
                                                 CommandArgument='<%# Eval("item_id") %>' 
                                                 CommandName="Del" runat="server">
                                                 Delete</asp:LinkButton>
                                             </ItemTemplate>
                                             </asp:TemplateField>
                                    </Columns>
                                    <RowStyle CssClass="ms-vb2"/>
                                    <AlternatingRowStyle CssClass="ms-alternating"/>
                                </asp:GridView>
                            </td>
                        </tr>
                        <tr>
					            <td>
                                    <table>
                                        <tr>
                                            <td valign=top style="padding-top:5px;" class="ms-descriptiontext" >
					                        <img src="/_layouts/images/setrect.gif" alt="">&nbsp;
					                        </td>
					                        <td nowrap class="ms-descriptiontext" style="padding-top:7px;padding-left: 3px;">
					                        <a href="Javascript:void(0);" onclick="Javascript:addSumField();">Add Field</a>
					                        </td>
                                        </tr>
                                    </table>
                                </td>
			            </tr>
                    </table>

                    <div id="Field" style="display:none;">
                        <asp:HiddenField ID="hdnId" runat="server" />
                        <table border="0" bgcolor="FFFFFF" style="border: solid 1px black " width="250" cellpadding="5" cellspacing="0">
                            <tr>
                                <td class="ms-vb2">Field:</td>
                                <td class="ms-vb2"><asp:TextBox ID="txtAddField" runat="server"></asp:TextBox></td>
                            </tr>
                            <tr>
                                <td class="ms-vb2">Calculation:</td>
                                <td class="ms-vb2">
                                    <asp:DropDownList ID="ddlAddCalculation" runat="server">
                                        <asp:ListItem Value="Average" Text="Average"></asp:ListItem>
                                        <asp:ListItem Value="Sum" Text="Sum"></asp:ListItem>
                                        <asp:ListItem Value="Min" Text="Minimum"></asp:ListItem>
                                        <asp:ListItem Value="Max" Text="Maximum"></asp:ListItem>
                                        <asp:ListItem Value="RollDown" Text="Folder Roll Down"></asp:ListItem>
                                        <asp:ListItem Value="Disallow" Text="Disable On Summary Tasks"></asp:ListItem>
                                    </asp:DropDownList>
                                </td>
                            </tr>
                            <tr>
                                <td></td>
                                <td><asp:Button ID="btnAdd" runat="server" Text="Add Field" OnClick="btnAdd_Click"></asp:Button> <asp:Button ID="Button1" runat="server" Text="Cancel" OnClientClick="Javascript:cancelAdd()"></asp:Button></td>
                            </tr>
                        </table>
                    </div>

                    <table cellpadding="0" cellspacing="0" width="100%">
                            <TR height=10><TD style="PADDING-BOTTOM: 4px; PADDING-LEFT: 4px; PADDING-RIGHT: 4px; PADDING-TOP: 4px" id=TD1 class=ms-linksectionheader colSpan=2>
                            <H3 class=ms-standardheader>Additional Settings</H3></TD></TR>

                            <wssuc:InputFormSection Title="Enable Folders"
	                            Description=""
	                            runat="server" Visible="false">
	                            <Template_Description>
	                                Select this option to allow the use of folders in the Planner.
	                            </Template_Description>
	                            <Template_InputFormControls>
		                            <wssuc:InputFormControl LabelText="" runat="server">
			                             <Template_Control>
			                                <asp:CheckBox ID="chkUseFolders" runat="server" /> Use Folders
			                             </Template_Control>
		                            </wssuc:InputFormControl>
	                            </Template_InputFormControls>
                            </wssuc:InputFormSection>

                            <wssuc:InputFormSection Title="Default Task Type"
	                            Description=""
	                            runat="server">
	                            <Template_Description>
                                    <ul>
	                                    <li>Shared - This function splits work between resources. When publishing, only 1 item total is published per task</li>
                                        <li>Individual - This function adds work from each assignment to the task level. When publishing, 1 item per assignment is published.</li>
                                    </ul>
	                            </Template_Description>
	                            <Template_InputFormControls>
		                            <wssuc:InputFormControl LabelText="" runat="server">
			                             <Template_Control>
			                                <asp:DropDownList ID="ddTaskType" runat="server">
                                                <asp:ListItem Text="Shared" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Individual" Value="1"></asp:ListItem>
                                            </asp:DropDownList>
			                             </Template_Control>
		                            </wssuc:InputFormControl>
	                            </Template_InputFormControls>
                            </wssuc:InputFormSection>

                            <wssuc:InputFormSection Title="Calculate Work"
	                            Description=""
	                            runat="server">
	                            <Template_Description>
                                    When you assign a resource to a task this will calculate the work for that assignment based on the duration of the task.
	                            </Template_Description>
	                            <Template_InputFormControls>
		                            <wssuc:InputFormControl LabelText="" runat="server">
			                             <Template_Control>
			                               <asp:CheckBox runat="server" ID="chkCalcWork" /> Calculate Work
			                             </Template_Control>
		                            </wssuc:InputFormControl>
	                            </Template_InputFormControls>
                            </wssuc:InputFormSection>
                    
                            <wssuc:InputFormSection Title="Lunch Break"
	                            Description=""
	                            runat="server">
	                            <Template_Description>
                                    Use this to determine the lunch break time. <br /><br />
                                    
                                    <b>You currently have your work set from <%=workstart %>:00 to <%=workend %>:00 which is <%=workhours %> hours.</b>
                                    <br /><br />
                                    In order for your work to calculate properly you may want to set this to a duration of 1 hour.
	                            </Template_Description>
	                            <Template_InputFormControls>
		                            <wssuc:InputFormControl LabelText="" runat="server">
			                             <Template_Control>
			                                <asp:DropDownList id="ddlLunchStart" runat="server">
                                            </asp:DropDownList> - 
                                            <asp:DropDownList id="ddlLunchEnd" runat="server">
                                            </asp:DropDownList>
			                             </Template_Control>
		                            </wssuc:InputFormControl>
	                            </Template_InputFormControls>
                            </wssuc:InputFormSection>

                            <wssuc:InputFormSection Title="Calculate Costs"
	                            Description=""
	                            runat="server">
	                            <Template_Description>
                                    This option will automatically calculate both cost and actual cost based on the rate of individual resources.
	                            </Template_Description>
	                            <Template_InputFormControls>
		                            <wssuc:InputFormControl LabelText="" runat="server">
			                             <Template_Control>
			                               <asp:CheckBox runat="server" ID="chkCalcCost" /> Calculate Costs
			                             </Template_Control>
		                            </wssuc:InputFormControl>
	                            </Template_InputFormControls>
                            </wssuc:InputFormSection>

                        <wssuc:InputFormSection Title="Select Statusing Method"
	                        Description=""
	                        runat="server">
	                        <Template_Description>
	                            <ul>
                                    <li><b>Manual</b> - All updated information will get pulled into work planner and no additional calculations will happen.</li>
                                    <li><b>Actual/Remaining Work</b> - Use this method to accept updates as actual and remaining work. This will also update the % complete based off the work specified.</li>
                                    <li><b>Remaining Work</b> - Use this method to accept updates as remaining work only. This will also update the % complete based off work and remaining work.</li>
                                    <li><b>Status Field</b> - Use this method to accept updates using the status choice field. This will set % complete to 0/50/100.</li>
                                    <li><b>Complete Field</b> - Use this method to accept updates as a complete field checkbox. This will set % complete to 0/100.</li>
                                    <li><b>% Complete</b> - Use this method to accept updates as a % complete field input.</li>
                                </ul>
	                        </Template_Description>
	                        <Template_InputFormControls>
		                        <wssuc:InputFormControl LabelText="" runat="server">
			                            <Template_Control>
			                            <asp:DropDownList ID="ddlStatusMethod" runat="Server">
			                                <asp:ListItem Text="Manual" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="Actual/Remaining Work" Value="1"></asp:ListItem>
                                            <asp:ListItem Text="Remaining Work" Value="2"></asp:ListItem>
                                            <asp:ListItem Text="Status Field" Value="3"></asp:ListItem>
                                            <asp:ListItem Text="Complete Field" Value="4"></asp:ListItem>
                                            <asp:ListItem Text="% Complete" Value="5"></asp:ListItem>
			                            </asp:DropDownList>
			                            </Template_Control>
		                        </wssuc:InputFormControl>
	                        </Template_InputFormControls>
                        </wssuc:InputFormSection>
        

                    <wssuc:InputFormSection Title="Additional Fields"
	                        Description=""
	                        runat="server" Visible="true">
	                        <Template_Description>
	                            Select the fields you would like to include in statusing. 
	                        </Template_Description>
	                        <Template_InputFormControls>
		                        <wssuc:InputFormControl LabelText="" runat="server">
			                            <Template_Control>
			                                <table cellpadding="5">
                                            <tr>
                                                <td class="ms-authoringcontrols">
                                                    Available Fields:<br />
                                                    <asp:ListBox runat="server" ID="ddlListAvailableFields" SelectionMode="Multiple" Rows="10" Width="200" CssClass="ms-input"></asp:ListBox>
                                                </td>
                                                <td>
                                                    <input type="button" value=" &gt; " onclick="addField();" /><br /><br />
                                                    <input type="button" value=" &lt; " onclick="removeField();"/>
                                                </td>
                                                <td class="ms-authoringcontrols">
                                                    Selected Fields:<br />
                                                    <asp:ListBox runat="server" ID="ddlListSelectedFields" SelectionMode="Multiple" Rows="10" Width="200" CssClass="ms-input"></asp:ListBox>
                                                </td>
                                            </tr>
                                        </table>
			                            </Template_Control>
		                        </wssuc:InputFormControl>
	                        </Template_InputFormControls>
                        </wssuc:InputFormSection>
                    </table>
                </td>
            </tr>
        </table>
                
        </span>

        <span id="spnAgile">
            <table class="ms-toolbar" width="100%" cellpadding="3" style="height:10px">
                <tr><td class="ms-linksectionheader"><h3 class="ms-standardheader">Agile Settings</h3></td></tr>
            </table>
            <table cellpadding="0" cellspacing="0" width="100%">
            
                <wssuc:InputFormSection Title="Iteration Content Type"
	                Description=""
	                runat="server">
	                <Template_Description>
	                    Select the content type that will be used for the iteration rows.
	                </Template_Description>
	                <Template_InputFormControls>
		                <wssuc:InputFormControl LabelText="" runat="server">
			                 <Template_Control>
			                    <asp:DropDownList ID="ddlAgileContentType" runat="Server">
			                        
			                    </asp:DropDownList>
			                 </Template_Control>
		                </wssuc:InputFormControl>
	                </Template_InputFormControls>
                </wssuc:InputFormSection>
            </table>
        </span>

        <span id="spnProject">
            <table class="ms-toolbar" width="100%" cellpadding="3" style="height:10px">
                <tr><td class="ms-linksectionheader"><h3 class="ms-standardheader">Microsoft Project Settings</h3></td></tr>
            </table>

            <table cellpadding="0" cellspacing="0" width="100%">
            
                <wssuc:InputFormSection Title="Lock Project Publisher"
		            Description=""
		            runat="server">
		            <Template_Description>
		                When publishing, this option will force the Project Publisher to use the settings defined on this page.
		            </Template_Description>
		            <Template_InputFormControls>
			            <wssuc:InputFormControl LabelText="" runat="server">
				                <Template_Control>
                                <asp:CheckBox ID="chkLockPublisher" runat="server" Text="Lock"/>
				                </Template_Control>
			            </wssuc:InputFormControl>
		            </Template_InputFormControls>
	            </wssuc:InputFormSection>

                <wssuc:InputFormSection Title="Publishing Type"
		                Description=""
		                runat="server">
		                <Template_Description>
		                    Select the Publishing Type that will be used.
		                </Template_Description>
		                <Template_InputFormControls>
			                <wssuc:InputFormControl LabelText="" runat="server">
				                 <Template_Control>
				                    <asp:DropDownList ID="ddlPubType" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Assignment Based" Value="1"></asp:ListItem>
				                        <asp:ListItem Text="Task Based" Value="2"></asp:ListItem>
				                        <asp:ListItem Text="Task Based w/o Assignment (Project Server Only)" Value="3"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
		                </Template_InputFormControls>
	                </wssuc:InputFormSection>

	                <wssuc:InputFormSection Title="Publishing Options"
		                Description=""
		                runat="server">
		                <Template_Description>
		                    
		                </Template_Description>
		                <Template_InputFormControls>
			                <wssuc:InputFormControl LabelText="Publish Summary Tasks" runat="server">
				                 <Template_Control>
				                    <asp:DropDownList ID="ddlSummary" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
				                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
			                <wssuc:InputFormControl LabelText="Publish Time-Phased Data" runat="server">
				                 <Template_Control>
				                    <asp:DropDownList ID="ddlTimePhased" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
				                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
		                </Template_InputFormControls>
	                </wssuc:InputFormSection>
	                
	                <wssuc:InputFormSection Title="Field Settings"
		                Description=""
		                runat="server">
		                <Template_Description>
		                    These field settings are used to control the Project fields that are used to interact with SharePoint.
		                    <br /><br />
		                    The Publish Status Field is used to store the date/time the task was last published in order to determine if a task has been updated.
		                    <br /><br />
		                    The Resource Link Field is used to store the link for the resource from the Resource Pool in SharePoint.
		                </Template_Description>
		                <Template_InputFormControls>
			                <wssuc:InputFormControl LabelText="Publish Status Field" runat="server">
				                 <Template_Control>
				                    <asp:DropDownList ID="ddlPubStatus" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Text1" Value="Text1"></asp:ListItem>
				                        <asp:ListItem Text="Text2" Value="Text2"></asp:ListItem>
				                        <asp:ListItem Text="Text3" Value="Text3"></asp:ListItem>
				                        <asp:ListItem Text="Text4" Value="Text4"></asp:ListItem>
				                        <asp:ListItem Text="Text5" Value="Text5"></asp:ListItem>
				                        <asp:ListItem Text="Text6" Value="Text6"></asp:ListItem>
				                        <asp:ListItem Text="Text7" Value="Text7"></asp:ListItem>
				                        <asp:ListItem Text="Text8" Value="Text8"></asp:ListItem>
				                        <asp:ListItem Text="Text9" Value="Text9"></asp:ListItem>
				                        <asp:ListItem Text="Text10" Value="Text10"></asp:ListItem>
				                        <asp:ListItem Text="Text11" Value="Text11"></asp:ListItem>
				                        <asp:ListItem Text="Text12" Value="Text12"></asp:ListItem>
				                        <asp:ListItem Text="Text13" Value="Text13"></asp:ListItem>
				                        <asp:ListItem Text="Text14" Value="Text14"></asp:ListItem>
				                        <asp:ListItem Text="Text15" Value="Text15"></asp:ListItem>
				                        <asp:ListItem Text="Text16" Value="Text16"></asp:ListItem>
				                        <asp:ListItem Text="Text17" Value="Text17"></asp:ListItem>
				                        <asp:ListItem Text="Text18" Value="Text18"></asp:ListItem>
				                        <asp:ListItem Text="Text19" Value="Text19"></asp:ListItem>
				                        <asp:ListItem Text="Text20" Value="Text20"></asp:ListItem>
				                        <asp:ListItem Text="Text21" Value="Text21"></asp:ListItem>
				                        <asp:ListItem Text="Text22" Value="Text22"></asp:ListItem>
				                        <asp:ListItem Text="Text23" Value="Text23"></asp:ListItem>
				                        <asp:ListItem Text="Text24" Value="Text24"></asp:ListItem>
				                        <asp:ListItem Text="Text25" Value="Text25"></asp:ListItem>
				                        <asp:ListItem Text="Text26" Value="Text26"></asp:ListItem>
				                        <asp:ListItem Text="Text27" Value="Text27"></asp:ListItem>
				                        <asp:ListItem Text="Text28" Value="Text28"></asp:ListItem>
				                        <asp:ListItem Text="Text29" Value="Text29"></asp:ListItem>
				                        <asp:ListItem Text="Text30" Value="Text30"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
			                <wssuc:InputFormControl LabelText="Resource Link Field" runat="server">
				                 <Template_Control>
				                    <asp:DropDownList ID="ddlResourceLink" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Number1" Value="Number1"></asp:ListItem>
				                        <asp:ListItem Text="Number2" Value="Number2"></asp:ListItem>
				                        <asp:ListItem Text="Number3" Value="Number3"></asp:ListItem>
				                        <asp:ListItem Text="Number4" Value="Number4"></asp:ListItem>
				                        <asp:ListItem Text="Number5" Value="Number5"></asp:ListItem>
				                        <asp:ListItem Text="Number6" Value="Number6"></asp:ListItem>
				                        <asp:ListItem Text="Number7" Value="Number7"></asp:ListItem>
				                        <asp:ListItem Text="Number8" Value="Number8"></asp:ListItem>
				                        <asp:ListItem Text="Number9" Value="Number9"></asp:ListItem>
				                        <asp:ListItem Text="Number10" Value="Number10"></asp:ListItem>
				                        <asp:ListItem Text="Number11" Value="Number11"></asp:ListItem>
				                        <asp:ListItem Text="Number12" Value="Number12"></asp:ListItem>
				                        <asp:ListItem Text="Number13" Value="Number13"></asp:ListItem>
				                        <asp:ListItem Text="Number14" Value="Number14"></asp:ListItem>
				                        <asp:ListItem Text="Number15" Value="Number15"></asp:ListItem>
				                        <asp:ListItem Text="Number16" Value="Number16"></asp:ListItem>
				                        <asp:ListItem Text="Number17" Value="Number17"></asp:ListItem>
				                        <asp:ListItem Text="Number18" Value="Number18"></asp:ListItem>
				                        <asp:ListItem Text="Number19" Value="Number19"></asp:ListItem>
				                        <asp:ListItem Text="Number20" Value="Number20"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
		                </Template_InputFormControls>
	                </wssuc:InputFormSection>

	                <wssuc:InputFormSection Title="Synch Fields on Open"
		                Description=""
		                runat="server">
		                <Template_Description>
		                    Use this option to force publisher to synchronize choice fields on open of a project.
		                </Template_Description>
		                <Template_InputFormControls>
			                <wssuc:InputFormControl LabelText="" runat="server">
				                 <Template_Control>
                                    <asp:DropDownList ID="ddlSynchFields" runat="server">
				                        <asp:ListItem Text="Not Set" Value=""></asp:ListItem>
				                        <asp:ListItem Text="Yes" Value="True"></asp:ListItem>
				                        <asp:ListItem Text="No" Value="False"></asp:ListItem>
				                    </asp:DropDownList>
				                 </Template_Control>
			                </wssuc:InputFormControl>
		                </Template_InputFormControls>
	                </wssuc:InputFormSection>

	                <wssuc:InputFormSection Title="Use Resource Pool"
		                Description=""
		                runat="server" Visible="false">
		                <Template_Description>
		                    Use this option to use the EPM Live resource pool with project publisher
		                </Template_Description>
		                <Template_InputFormControls>
			                <wssuc:InputFormControl LabelText="Use Resource Pool" runat="server">
				                 <Template_Control>
                                    <asp:CheckBox ID="chkUseRes" runat="server" />
				                 </Template_Control>
			                </wssuc:InputFormControl>
		                </Template_InputFormControls>
	                </wssuc:InputFormSection>
            </table>
        </span>

        <table cellpadding="0" cellspacing="0" width="100%">
            <wssuc:ButtonSection runat="server">
                <Template_Buttons>
                    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
	                    <asp:Button UseSubmitBehavior="false" runat="server" class="ms-ButtonHeightWidth" OnClick="Button1_Click" Text="Save Settings" id="Button2" accesskey="" Width="150"/>
                    </asp:PlaceHolder>
                </Template_Buttons>
            </wssuc:ButtonSection>
        </table>
        
        <script language="javascript">
            ToggleAll();
        </script>
</asp:Content>
