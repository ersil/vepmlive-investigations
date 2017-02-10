﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.master" AutoEventWireup="true" CodeBehind="editaccount.aspx.cs" Inherits="AdminSite.editaccount" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

    <link rel="STYLESHEET" type="text/css" href="modal/modal.css">
    <link rel="STYLESHEET" type="text/css" href="dhtmlxtab/dhtmlxtabbar.css" />
    <style>
        input[type=button] {
            background: #605D58;
            border: 0px;
            padding: 4px;
            color: #FFF;
            width: 65px;
            cursor: pointer;
        }

        #AddLicenseButton {
            width: 100px;
        }
    </style>

    <script src="dhtmlxtab/dhtmlxcommon.js"></script>
    <script src="dhtmlxtab/dhtmlxtabbar.js"></script>
    <script src="dhtmlxtab/dhtmlxtabbar_start.js"></script>
    <script src="modal/modal.js"></script>
    <script>
        function viewCRMAccount(account) {
            var h = screen.height - 200;
            var w = screen.width - 300;
            window.open('http://crm.epmlive.com/EPMLive/sfa/accts/edit.aspx?id=%7b' + account + '%7d', 'account', 'status=false,toolbar=false,scrollbar=false,width=' + w + ',height=' + h + ',top=100,left=150');
        }
        function viewCRMInvoice(invoice) {
            var h = screen.height - 200;
            var w = screen.width - 300;
            window.open('http://crm.epmlive.com/EPMLive/sfa/invoice/edit.aspx?id=%7b' + invoice + '%7d', 'invoice', 'status=false,toolbar=false,scrollbar=false,width=' + w + ',height=' + h + ',top=100,left=150');
        }
        function viewCRMOrder(order) {
            var h = screen.height - 200;
            var w = screen.width - 300;
            window.open('http://crm.epmlive.com/EPMLive/sfa/salesorder/edit.aspx?id=%7b' + order + '%7d', 'invoice', 'status=false,toolbar=false,scrollbar=false,width=' + w + ',height=' + h + ',top=100,left=150');
        }
        function addTicket(key_id) {
            var ticket = prompt("Enter Ticket Number", "");
            if (ticket != "") {
                var url = "addticket.aspx?account_id=<%=Request["account_id"] %>&ticket=" + ticket;
            location.href = url;
        }
    }
    function changeowner(key_id) {
        sm('divresetkey', 400, 100);
        var url = "changeowner.aspx?account_id=<%=Request["account_id"] %>";
        document.getElementById("iframeresetkey").src = url;
        document.getElementById("iframeresetkey").style.height = 90;

    }
    function closereset() {
        hm('divresetkey');
    }


    function AddNewLicense() {
        var accountId = '<%=Request["account_id"] %>'
        var url = 'newlicense.aspx?accountId=' + accountId
        ShowModal('modalLicenseManagement', 'iframeAddLicense', url)
    };

    function CloseAddLicenseModal()
    {
        HideModal('modalLicenseManagement')
    }

    function ShowModal(div, iframe, url) {
        document.getElementById(iframe).src = url;
        sm(div, 500, 450);
    }

    function HideModal(modal) {
        hm(modal);
    }

    </script>
    <div id="a_tabbar" class="dhtmlxTabBar" select="a<%=strTab %>" imgpath="dhtmlxtab/imgs/" style="width: 100%; height: 500px;" skincolors="#FCFBFC,#F4F3EE">
        <div id="a1" name="Account Information" width="140px">
            <asp:Panel ID="pnlEditSuccess" runat="server" Width="100%" Visible="false">
                <font color="green"><b>User Saved Successfully!</b></font>
            </asp:Panel>
            <asp:Panel ID="pnlEditFailure" runat="server" Width="100%" Visible="false">
                <font color="red"><b>User Failed to Save:<br /><br />
            <asp:Label ID="lblError" runat="server"></asp:Label>
            </b></font>
            </asp:Panel>
            <table border="0" cellpadding="1" cellspacing="2" style="margin: 10px">
                <tr>
                    <td colspan="2"><b>Account Information</b></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Account #:</td>
                    <td>
                        <asp:Label ID="lblAccountRef" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Account Description:</td>
                    <td>
                        <asp:TextBox ID="txtDesc" runat="server" Width="100"></asp:TextBox></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Months Free:</td>
                    <td>
                        <asp:TextBox ID="txtMonthsFree" runat="server" Width="50"></asp:TextBox></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Trial Users:</td>
                    <td>
                        <asp:TextBox ID="txtTrialUsers" runat="server" Width="50"></asp:TextBox></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Linked Partner Id:</td>
                    <td>
                        <asp:TextBox ID="txtPartnerId" runat="server" Width="50"></asp:TextBox>
                        <a href="http://www.workengine.com/partners/Lists/Partners/PartnerIds.aspx" target="_blank">[Find an ID]</a></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Dedicated:</td>
                    <td>
                        <asp:CheckBox ID="chkDedicated" runat="server"></asp:CheckBox>
                        (Check this if users should not receive system upgrade notices due to being on a dedicated system)</td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Expiration Date:</td>
                    <td>
                        <asp:Label ID="lblExpiration" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Linked CRM Account:</td>
                    <td>
                        <asp:Label ID="lblLink" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Linked Zuora Account:</td>
                    <td>
                        <asp:Label ID="lblZuoraLink" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Lock Users:</td>
                    <td>
                        <asp:CheckBox ID="chkLockUsers" runat="server" />
                        Select this option to prevent the owner from purchasing additional users.</td>
                </tr>
                <tr style="display: none">
                    <td width="100" bgcolor="#c9c9c9" nowrap>Billing Type:</td>
                    <td>
                        <asp:DropDownList ID="ddlBillingType" runat="server" Enabled="false">
                            <asp:ListItem Text="--Select--" Value=""></asp:ListItem>
                            <asp:ListItem Text="Credit Card" Value="1"></asp:ListItem>
                            <asp:ListItem Text="Monthly Recurring PO" Value="2"></asp:ListItem>
                            <asp:ListItem Text="Other Recurring PO" Value="3"></asp:ListItem>
                        </asp:DropDownList>
                    </td>
                </tr>

                <tr>
                    <td colspan="2"><b>User Information</b> <a href="edituser.aspx?uid=<%=strUid %>">[Edit]</a></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Username:</td>
                    <td>
                        <asp:Label ID="lblUsername" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>First Name:</td>
                    <td>
                        <asp:Label ID="txtEditFirstName" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9">Last Name:</td>
                    <td>
                        <asp:Label ID="txtEditLastName" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>E-Mail:</td>
                    <td>
                        <asp:Label ID="lblEditEmail" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Company:</td>
                    <td>
                        <asp:Label ID="txtEditCompany" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Title:</td>
                    <td>
                        <asp:Label ID="txtEditTitle" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Department:</td>
                    <td>
                        <asp:Label ID="txtEditDepartment" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td width="100" bgcolor="#c9c9c9" nowrap>Phone:</td>
                    <td>
                        <asp:Label ID="txtEditPhone" runat="server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2">
                        <br />
                        <input type="button" value="Change Owner" class="searchbutton" onclick="changeowner();" style="width: 100px">
                        <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" CssClass="searchbutton" />
                        <asp:Button ID="btnCancel" runat="server" Text="Cancel" OnClick="btnCancel_Click" CssClass="searchbutton" />
                    </td>
                </tr>
            </table>
        </div>
        <div id="a2" name="Sites" width="90px">
            <div style="padding: 5px">
                <b>2007 Sites Owned by User</b>
                <asp:GridView Width="95%" ID="gvSitesOwn" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:TemplateField HeaderText="Site" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="http://my.epmlive.com/<%# Eval("FullURL") %>" target="_blank"><%# Eval("Title") %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="timecreated" HeaderText="Created">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>
                <br />
                <b>2010 Sites Owned by User</b> <a href="linksite.aspx?account_id=<%=Request["account_id"] %>">[Link Site]</a>
                <asp:GridView Width="95%" ID="gvExternalSites" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:TemplateField HeaderText="Site" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="<%# Eval("siteurl") %>" target="_blank"><%# Eval("sitename") %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="timecreated" HeaderText="Created">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>
                <br />
                <b>Sites User Belongs To</b>
                <asp:GridView Width="95%" ID="gvSites" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:TemplateField HeaderText="Site" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="http://my.epmlive.com/<%# Eval("FullURL") %>" target="_blank"><%# Eval("Title") %></a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="timecreated" HeaderText="Created">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
        <div id="a3" name="Orders" width="90px">


            <div style="padding: 5px">

                <%=sbOrders.ToString()%>
                <br />
                <%if (usingNewBilling)
                    {%>
                <a href="newzuoraorder.aspx?account_id=<%=strAccountId %>">[Add Zuora Order]</a>
                <%} %>
                <hr />
                <b>Old Order System</b>
                <asp:Panel ID="pnlAddOrder" runat="server"><a href="addorder.aspx?account_id=<%=strAccountId %>">[Add Order]</a></asp:Panel>
                <asp:GridView Width="100%" ID="GridView2" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:BoundField DataField="version" HeaderText="Type">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="dtcreated" HeaderText="Order Date">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="expiration" HeaderText="Expiration">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:BoundField DataField="quantity" HeaderText="Quantity">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:BoundField DataField="ref" HeaderText="Ref #">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:BoundField DataField="type" HeaderText="Order Location">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="View CRM Order" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="javascript:viewCRMOrder('<%# Eval("order_id") %>');" style="display: <%# Eval("linked") %>">View Order</a> <a href="updateorder.aspx?account_id=<%=strAccountId %>&order_id=<%# Eval("order_id") %>&r_order_id=<%# Eval("r_order_id") %>" style="display: <%# Eval("linked") %>">[Update Order]</a>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Delete" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="editaccount.aspx?account_id=<%=strAccountId%>&delorder=<%# Eval("r_order_id") %>&tab=3">Delete</a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>
            </div>
        </div>
        <div id="a4" name="Licenses" width="90px">
            <div style="padding: 5px">
                <br />

                <b>Active Licenses</b>
                <br />
                <br />
                <asp:GridView Width="96%" RowStyle-HorizontalAlign="Left" ID="GridViewActiveLicenses" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>

                        <asp:BoundField DataField="product" HeaderText="Product">
                            <ItemStyle HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="features" HeaderText="Features" HtmlEncode="false">
                            <ItemStyle HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="expirationdate" HeaderText="Expiration Date">
                            <ItemStyle HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <input type="button" value="View" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <input type="button" value="Extend" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <input type="button" value="Renew" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <input type="button" value="Rebuke" />
                                <%--<asp:LinkButton ID="DeleteLicenses" runat="server" CausesValidation="false" CommandName="DeleteLicense" Text="Delete" OnClientClick="return confirm('Are you certain you want to delete this item?');"></asp:LinkButton>--%>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>

                <br />

                <input type="button" id="AddLicenseButton" value="New License" onclick="AddNewLicense()" />

            </div>
        </div>
        <div id="a5" name="Users" width="90px">
            <div style="padding: 5px">
                <b>SharePoint Users</b>
                <asp:GridView Width="96%" RowStyle-HorizontalAlign="Left" ID="gvUsers" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:BoundField DataField="name" HeaderText="Name">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="email" HeaderText="Email">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="username" HeaderText="Username">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataFormatString="{0:d}" DataField="dtadded" HeaderText="Added">
                            <ItemStyle Font-Size="X-Small" />
                        </asp:BoundField>
                        <asp:TemplateField HeaderText="View" ItemStyle-HorizontalAlign="Left">
                            <ItemTemplate>
                                <a href="edituser.aspx?uid=<%# Eval("uid") %>">View User</a>
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>


                <asp:Panel ID="pnlEntUsers" runat="server">
                </asp:Panel>

            </div>
        </div>
        <div id="a7" name="Activity Log" width="90px">
            <div style="padding: 5px">
                <asp:GridView Width="96%" RowStyle-HorizontalAlign="Left" ID="gvLog" runat="server" AutoGenerateColumns="False" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderWidth="1px">
                    <Columns>
                        <asp:BoundField DataField="dtposted" HeaderText="Date">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="fullname" HeaderText="User">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                        <asp:BoundField DataField="message" HeaderText="Message">
                            <ItemStyle Font-Size="X-Small" HorizontalAlign="left" />
                        </asp:BoundField>
                    </Columns>
                    <FooterStyle BackColor="#CCCC99" />
                    <RowStyle BackColor="#F7F7DE" HorizontalAlign="Center" />
                    <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
                    <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
                    <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
                    <AlternatingRowStyle BackColor="White" HorizontalAlign="Center" />
                </asp:GridView>

            </div>
        </div>

    </div>
    <div id="divresetkey" class="dialog">
        <iframe id="iframeresetkey" width="100%" height="100" frameborder="0"></iframe>
    </div>

    <div id="modalLicenseManagement" class="dialog">
        <iframe id="iframeAddLicense" width="100%" height="450" frameborder="0"></iframe>
    </div>
    <script language="javascript">
        initmb();
    </script>
</asp:Content>
