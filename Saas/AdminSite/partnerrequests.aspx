﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.master" AutoEventWireup="true" CodeBehind="partnerrequests.aspx.cs" Inherits="AdminSite.partnerrequests" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CellPadding="4"
            ForeColor="Black" GridLines="Vertical" Width="100%" OnRowCommand="GridView1_RowCommand" OnRowDataBound="GridView1_RowDataBound" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px">
            <FooterStyle BackColor="#CCCC99" />
            <Columns>
                <asp:BoundField DataField="firstname" HeaderText="First Name" />
                <asp:BoundField DataField="lastname" HeaderText="Last Name" />
                <asp:BoundField DataField="companyname" HeaderText="Company" />
                <asp:TemplateField HeaderText="Process">
                    <ItemTemplate>
                       <asp:LinkButton ID="LinkButton1" 
                         CommandArgument='<%# Eval("partner_id") %>' 
                         CommandName="process" runat="server">
                         Process</asp:LinkButton>
                     </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Delete">
                    <ItemTemplate>
                       <asp:LinkButton ID="LinkButton2" 
                         CommandArgument='<%# Eval("partner_id") %>' 
                         CommandName="del" runat="server">
                         Delete</asp:LinkButton>
                     </ItemTemplate>
                </asp:TemplateField>
            </Columns>
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" HorizontalAlign="Left" />
            <AlternatingRowStyle BackColor="White" />
        </asp:GridView>
        <asp:Panel ID="pnlProcess" runat="server" Visible="false">
            <h3>Process Partner</h3>
            <asp:HiddenField ID="hdnUserId" runat="server" />
            <asp:HiddenField ID="hdnSiteUrl" runat="server" />
            <asp:HiddenField ID="hdnSiteName" runat="server" />
            <asp:HiddenField ID="hdnPartnerId" runat="server" />
            <table>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Name:</b></td>
                    <td><asp:Label ID="lblFirst" runat="Server"></asp:Label> <asp:Label ID="lblLast" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Company:</b></td>
                    <td><asp:Label ID="lblCompany" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>URL:</b></td>
                    <td><asp:Label ID="lblUrl" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Email:</b></td>
                    <td><asp:Label ID="lblEmail" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Has Account:</b></td>
                    <td><asp:Label ID="lblAccount" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Address:</b></td>
                    <td><asp:Label ID="lblAddress" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>City:</b></td>
                    <td><asp:Label ID="lblCity" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>State:</b></td>
                    <td><asp:Label ID="lblState" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Zip:</b></td>
                    <td><asp:Label ID="lblZip" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Country:</b></td>
                    <td><asp:Label ID="lblCountry" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" bgcolor="#eff3fb"><b>Company Information:</b></td>
                </tr>
                <tr>
                    <td colspan="2"><asp:Label ID="lblDescription" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" bgcolor="#eff3fb"><b>Service Information:</b></td>
                </tr>
                <tr>
                    <td colspan="2"><asp:Label ID="lblServices" runat="Server"></asp:Label></td>
                </tr>
                <tr>
                    <td colspan="2" bgcolor="#eff3fb"><b>Partner Type:</b></td>
                </tr>
                <tr>
                    <td colspan="2"><asp:Label ID="lblType" runat="Server"></asp:Label></td>
                </tr>
                <asp:Panel id="pnlSite" runat="server">
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Site URL:</b></td>
                    <td>http://www.epmlive.com/<asp:TextBox ID="txtSiteUrl"  Width="300" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td bgcolor="#eff3fb" width="150"><b>Site Name:</b></td>
                    <td><asp:TextBox ID="txtSiteName" runat="server" Width="300"></asp:TextBox></td>
                </tr>
                </asp:Panel>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="Button1" runat="server" Text="Process" OnClick="Button1_Click" />
                         <asp:Button ID="Button2" runat="server" Text="Cancel" OnClick="Button2_Click" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
</asp:Content>
