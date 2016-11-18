﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Admin.master" AutoEventWireup="true" CodeBehind="newzuoraorder.aspx.cs" Inherits="AdminSite.newzuoraorder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolderMain" runat="server">

<link rel="STYLESHEET" type="text/css" href="modal/modal.css">
<script  src="modal/modal.js"></script>

<div id="divreloading" class="dialog">
    Recalculating...
</div>

<asp:Label ID="lblError" runat="server" ForeColor="Red" Visible="false"></asp:Label>
<a href="editaccount.aspx?account_id=<%=Request["Account_id"] %>">&lt;&lt; Back To Account</a>
<hr />
<font color="red"><b>Do not use this page to place orders for the new pricing model (Professional/Enterprise)</b></font>
<h4>Core Pricing</h4>
Select Product: <asp:DropDownList ID="ddlProduct" runat="server" AutoPostBack="true" >
    <asp:ListItem Text="WorkEngine" Value="adc217cc-ed78-4066-8743-5297148859c4"></asp:ListItem>
    <asp:ListItem Text="PortfolioEngine" Value="a7ddc49d-1a28-4443-8ba1-fa907b0bbea1"></asp:ListItem>
    <asp:ListItem Text="WorkEngine 2007" Value="15437d9b-80f4-40cc-a656-ce5d7daa157e"></asp:ListItem>
</asp:DropDownList>
<br /><br />
Select Version: <asp:DropDownList ID="ddlVersion" runat="server" 
        AutoPostBack="true">
    <asp:ListItem Text="Standard" Value="Standard"></asp:ListItem>
    <asp:ListItem Text="Professional" Value="Professional"></asp:ListItem>
    <asp:ListItem Text="Ultimate" Value="Ultimate"></asp:ListItem>
</asp:DropDownList>
<br /><br />
Select Contract Terms: <asp:DropDownList id="ddlContract" runat="server" AutoPostBack="true">
    <asp:ListItem Text="Yearly with Yearly Payment" Value=".9"></asp:ListItem>
    <asp:ListItem Text="Yearly Contract" Value="1"></asp:ListItem>
    <asp:ListItem Text="Quarterly Contract" Value="1.1111111111111"></asp:ListItem>
    <asp:ListItem Text="Monthly Contract" Value="1.25"></asp:ListItem>
</asp:DropDownList>
<br /><br />
Select Billing Cycle: <asp:DropDownList id="ddlBilling" runat="server" AutoPostBack="true">
    <asp:ListItem Text="Yearly" Value="12"></asp:ListItem>
    <asp:ListItem Text="Semi-Annual" Value="6"></asp:ListItem>
    <asp:ListItem Text="Quarterly" Value="3"></asp:ListItem>
    <asp:ListItem Text="Monthly" Value="1"></asp:ListItem>
</asp:DropDownList>
<br /><br />

User Count: <asp:TextBox ID="txtUsers" runat="server" AutoPostBack="true" Columns="5" Text="25"></asp:TextBox>
<br /><br />
<asp:CheckBox ID="chkDoNotTier" runat="server" AutoPostBack="true" /> Do Not Tier Pricing
<br /><br />
PO #: <asp:TextBox ID="txtPO" runat="server"></asp:TextBox>
<br /><br />
Effective Date: 
    <asp:Calendar ID="Calendar1" runat="server"></asp:Calendar>
<br />
Enter Discount %: <asp:TextBox ID="txtDiscount" runat="server" 
        AutoPostBack="true" Width="34px">0</asp:TextBox>
        <br /><br />
<hr />
<h4>Additional Recurring Charges:</h4>

<br />

    <asp:GridView ID="gvAdditional" runat="server" AutoGenerateColumns="False" 
        EnableModelValidation="True" >
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Item Name" HeaderStyle-Width="500">
<HeaderStyle Width="500px"></HeaderStyle>
            </asp:BoundField>
            <asp:BoundField DataField="price" HeaderText="Price"  HeaderStyle-Width="100" 
                DataFormatString="{0:c}">
<HeaderStyle Width="100px"></HeaderStyle>
            </asp:BoundField>
            <asp:TemplateField HeaderText="Quantity"  HeaderStyle-Width="100">
                <ItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='0' Width="34px" AutoPostBack="true"></asp:TextBox>
                    <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Bind("ID") %>' />
                    <asp:HiddenField ID="HiddenField2" runat="server" Value='<%# Bind("price") %>'/>
                </ItemTemplate>

<HeaderStyle Width="100px"></HeaderStyle>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
    
    <br />
<hr />
<h4>Additional Products:</h4>
<asp:GridView ID="gvProducts" runat="server" AutoGenerateColumns="False" 
        EnableModelValidation="True" >
        <Columns>
            <asp:BoundField DataField="Name" HeaderText="Item Name" HeaderStyle-Width="440">
            </asp:BoundField>
            <asp:BoundField DataField="price" HeaderText="Price"  HeaderStyle-Width="80" 
                DataFormatString="{0:c}">
            </asp:BoundField>
            <asp:BoundField DataField="Type" HeaderText="Item Type" HeaderStyle-Width="100">
            </asp:BoundField>
            <asp:TemplateField HeaderText="Quantity"  HeaderStyle-Width="80">
                <ItemTemplate>
                    <asp:TextBox ID="TextBox1" runat="server" Text='0' Width="34px" AutoPostBack="true"></asp:TextBox>
                    <asp:HiddenField ID="HiddenField1" runat="server" Value='<%# Bind("ID") %>' />
                    <asp:HiddenField ID="HiddenFieldPRPID" runat="server" Value='<%# Bind("PRPID") %>' />
                    <asp:HiddenField ID="HiddenField2" runat="server" Value='<%# Bind("price") %>'/>
                </ItemTemplate>

<HeaderStyle Width="100px"></HeaderStyle>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
<br />
<hr />
<h4>Billing Info:</h4>
<table border="0" cellpadding="1" cellspacing="2">
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>First Name:</td>
                            <td><asp:TextBox ID="txtEditFirstName" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9">Last Name:</td>
                            <td><asp:TextBox ID="txtEditLastName" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>E-Mail:</td>
                            <td><asp:TextBox ID="txtEditEmail" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>Phone:</td>
                            <td><asp:TextBox ID="txtEditPhone" runat="server"></asp:TextBox></td>
                        </tr>
                         <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>Address1:</td>
                            <td><asp:TextBox ID="txtAddress1" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>Address2:</td>
                            <td><asp:TextBox ID="txtAddress2" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>City:</td>
                            <td><asp:TextBox ID="txtCity" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td class="inputtitle" bgcolor="#c9c9c9">Country:</td>
                            <td class="inputbox">
                                <asp:DropDownList ID="ddlCountry" runat="server" class="textbox">
                                    <asp:ListItem Value="" Text="--Select--"></asp:ListItem>
                                    <asp:ListItem Value="US" Text="United States"></asp:ListItem>
                                    <asp:ListItem Value="CA" Text="Canada"></asp:ListItem>

                                    <asp:ListItem Value="AF" Text="Afghanistan"></asp:ListItem>
                                    <asp:ListItem Value="AX" Text="Aland Islands"></asp:ListItem>
                                    <asp:ListItem Value="AL" Text="Albania"></asp:ListItem>
                                    <asp:ListItem Value="DZ" Text="Algeria"></asp:ListItem>
                                    <asp:ListItem Value="AS" Text="American Samoa"></asp:ListItem>
                                    <asp:ListItem Value="AD" Text="Andorra"></asp:ListItem>
                                    <asp:ListItem Value="AO" Text="Angola"></asp:ListItem>
                                    <asp:ListItem Value="AI" Text="Anguilla"></asp:ListItem>
                                    <asp:ListItem Value="AQ" Text="Antarctica"></asp:ListItem>
                                    <asp:ListItem Value="AG" Text="Antigua and Barbuda"></asp:ListItem>
                                    <asp:ListItem Value="AR" Text="Argentina"></asp:ListItem>
                                    <asp:ListItem Value="AM" Text="Armenia"></asp:ListItem>
                                    <asp:ListItem Value="AW" Text="Aruba"></asp:ListItem>
                                    <asp:ListItem Value="AU" Text="Australia"></asp:ListItem>
                                    <asp:ListItem Value="AT" Text="Austria"></asp:ListItem>
                                    <asp:ListItem Value="AZ" Text="Azerbaijan"></asp:ListItem>
                                    <asp:ListItem Value="BS" Text="Bahamas"></asp:ListItem>
                                    <asp:ListItem Value="BH" Text="Bahrain"></asp:ListItem>
                                    <asp:ListItem Value="BD" Text="Bangladesh"></asp:ListItem>
                                    <asp:ListItem Value="BB" Text="Barbados"></asp:ListItem>
                                    <asp:ListItem Value="BY" Text="Belarus"></asp:ListItem>
                                    <asp:ListItem Value="BE" Text="Belgium"></asp:ListItem>
                                    <asp:ListItem Value="BZ" Text="Belize"></asp:ListItem>
                                    <asp:ListItem Value="BJ" Text="Benin"></asp:ListItem>
                                    <asp:ListItem Value="BM" Text="Bermuda"></asp:ListItem>
                                    <asp:ListItem Value="BT" Text="Bhutan"></asp:ListItem>
                                    <asp:ListItem Value="BO" Text="Bolivia"></asp:ListItem>
                                    <asp:ListItem Value="BQ" Text="Bonaire, Saint Eustatius and Saba"></asp:ListItem>
                                    <asp:ListItem Value="BA" Text="Bosnia and Herzegovina"></asp:ListItem>
                                    <asp:ListItem Value="BW" Text="Botswana"></asp:ListItem>
                                    <asp:ListItem Value="BV" Text="Bouvet Island"></asp:ListItem>
                                    <asp:ListItem Value="BR" Text="Brazil"></asp:ListItem>
                                    <asp:ListItem Value="IO" Text="British Indian Ocean Territory"></asp:ListItem>
                                    <asp:ListItem Value="BN" Text="Brunei Darussalam"></asp:ListItem>
                                    <asp:ListItem Value="BG" Text="Bulgaria"></asp:ListItem>
                                    <asp:ListItem Value="BF" Text="Burkina Faso"></asp:ListItem>
                                    <asp:ListItem Value="BI" Text="Burundi"></asp:ListItem>
                                    <asp:ListItem Value="KH" Text="Cambodia"></asp:ListItem>
                                    <asp:ListItem Value="CM" Text="Cameroon"></asp:ListItem>
                        
                                    <asp:ListItem Value="CV" Text="Cape Verde"></asp:ListItem>
                                    <asp:ListItem Value="KY" Text="Cayman Islands"></asp:ListItem>
                                    <asp:ListItem Value="CF" Text="Central African Republic"></asp:ListItem>
                                    <asp:ListItem Value="TD" Text="Chad"></asp:ListItem>
                                    <asp:ListItem Value="CL" Text="Chile"></asp:ListItem>
                                    <asp:ListItem Value="CN" Text="China"></asp:ListItem>
                                    <asp:ListItem Value="CX" Text="Christmas Island"></asp:ListItem>
                                    <asp:ListItem Value="CC" Text="Cocos (Keeling) Islands"></asp:ListItem>
                                    <asp:ListItem Value="CO" Text="Colombia"></asp:ListItem>
                                    <asp:ListItem Value="KM" Text="Comoros"></asp:ListItem>
                                    <asp:ListItem Value="CG" Text="Congo"></asp:ListItem>
                                    <asp:ListItem Value="CD" Text="Congo, the Democratic Republic of the"></asp:ListItem>
                                    <asp:ListItem Value="CK" Text="Cook Islands"></asp:ListItem>
                                    <asp:ListItem Value="CR" Text="Costa Rica"></asp:ListItem>
                                    <asp:ListItem Value="CI" Text="Cote d'Ivoire"></asp:ListItem>
                                    <asp:ListItem Value="HR" Text="Croatia"></asp:ListItem>
                                    <asp:ListItem Value="CU" Text="Cuba"></asp:ListItem>
                                    <asp:ListItem Value="CW" Text="Curaçao (Cura�ao)"></asp:ListItem>
                                    <asp:ListItem Value="CY" Text="Cyprus"></asp:ListItem>
                                    <asp:ListItem Value="CZ" Text="Czech Republic"></asp:ListItem>
                                    <asp:ListItem Value="DK" Text="Denmark"></asp:ListItem>
                                    <asp:ListItem Value="DJ" Text="Djibouti"></asp:ListItem>
                                    <asp:ListItem Value="DM" Text="Dominica"></asp:ListItem>
                                    <asp:ListItem Value="DO" Text="Dominican Republic"></asp:ListItem>
                                    <asp:ListItem Value="EC" Text="Ecuador"></asp:ListItem>
                                    <asp:ListItem Value="EG" Text="Egypt"></asp:ListItem>
                                    <asp:ListItem Value="SV" Text="El Salvador"></asp:ListItem>
                                    <asp:ListItem Value="GQ" Text="Equatorial Guinea"></asp:ListItem>
                                    <asp:ListItem Value="ER" Text="Eritrea"></asp:ListItem>
                                    <asp:ListItem Value="EE" Text="Estonia"></asp:ListItem>
                                    <asp:ListItem Value="ET" Text="Ethiopia"></asp:ListItem>
                                    <asp:ListItem Value="FK" Text="Falkland Islands (Malvinas)"></asp:ListItem>
                                    <asp:ListItem Value="FO" Text="Faroe Islands"></asp:ListItem>
                                    <asp:ListItem Value="FJ" Text="Fiji"></asp:ListItem>
                                    <asp:ListItem Value="FI" Text="Finland"></asp:ListItem>
                                    <asp:ListItem Value="FR" Text="France"></asp:ListItem>
                                    <asp:ListItem Value="GF" Text="French Guiana"></asp:ListItem>
                                    <asp:ListItem Value="PF" Text="French Polynesia"></asp:ListItem>
                                    <asp:ListItem Value="TF" Text="French Southern Territories"></asp:ListItem>
                                    <asp:ListItem Value="GA" Text="Gabon"></asp:ListItem>
                                    <asp:ListItem Value="GM" Text="Gambia"></asp:ListItem>
                                    <asp:ListItem Value="GE" Text="Georgia"></asp:ListItem>
                                    <asp:ListItem Value="DE" Text="Germany"></asp:ListItem>
                                    <asp:ListItem Value="GH" Text="Ghana"></asp:ListItem>
                                    <asp:ListItem Value="GI" Text="Gibraltar"></asp:ListItem>
                                    <asp:ListItem Value="GR" Text="Greece"></asp:ListItem>
                                    <asp:ListItem Value="GL" Text="Greenland"></asp:ListItem>
                                    <asp:ListItem Value="GD" Text="Grenada"></asp:ListItem>
                                    <asp:ListItem Value="GP" Text="Guadeloupe"></asp:ListItem>
                                    <asp:ListItem Value="GU" Text="Guam"></asp:ListItem>
                                    <asp:ListItem Value="GT" Text="Guatemala"></asp:ListItem>
                                    <asp:ListItem Value="GG" Text="Guernsey"></asp:ListItem>
                                    <asp:ListItem Value="GN" Text="Guinea"></asp:ListItem>
                                    <asp:ListItem Value="GW" Text="Guinea-Bissau"></asp:ListItem>
                                    <asp:ListItem Value="GY" Text="Guyana"></asp:ListItem>
                                    <asp:ListItem Value="HT" Text="Haiti"></asp:ListItem>
                                    <asp:ListItem Value="HM" Text="Heard and McDonald Islands"></asp:ListItem>
                                    <asp:ListItem Value="VA" Text="Holy See (Vatican City State)"></asp:ListItem>
                                    <asp:ListItem Value="HN" Text="Honduras"></asp:ListItem>
                                    <asp:ListItem Value="HK" Text="Hong Kong"></asp:ListItem>
                                    <asp:ListItem Value="HU" Text="Hungary"></asp:ListItem>
                                    <asp:ListItem Value="IS" Text="Iceland"></asp:ListItem>
                                    <asp:ListItem Value="IN" Text="India"></asp:ListItem>
                                    <asp:ListItem Value="ID" Text="Indonesia"></asp:ListItem>
                                    <asp:ListItem Value="IR" Text="Iran, Islamic Republic of"></asp:ListItem>
                                    <asp:ListItem Value="IQ" Text="Iraq"></asp:ListItem>
                                    <asp:ListItem Value="IE" Text="Ireland"></asp:ListItem>
                                    <asp:ListItem Value="IM" Text="Isle of Man"></asp:ListItem>
                                    <asp:ListItem Value="IL" Text="Israel"></asp:ListItem>
                                    <asp:ListItem Value="IT" Text="Italy"></asp:ListItem>
                                    <asp:ListItem Value="JM" Text="Jamaica"></asp:ListItem>
                                    <asp:ListItem Value="JP" Text="Japan"></asp:ListItem>
                                    <asp:ListItem Value="JE" Text="Jersey"></asp:ListItem>
                                    <asp:ListItem Value="JO" Text="Jordan"></asp:ListItem>
                                    <asp:ListItem Value="KZ" Text="Kazakhstan"></asp:ListItem>
                                    <asp:ListItem Value="KE" Text="Kenya"></asp:ListItem>
                                    <asp:ListItem Value="KI" Text="Kiribati"></asp:ListItem>
                                    <asp:ListItem Value="KP" Text="Korea, Democratic People's Republic of"></asp:ListItem>
                                    <asp:ListItem Value="KR" Text="Korea, Republic of"></asp:ListItem>
                                    <asp:ListItem Value="KW" Text="Kuwait"></asp:ListItem>
                                    <asp:ListItem Value="KG" Text="Kyrgyzstan"></asp:ListItem>
                                    <asp:ListItem Value="LA" Text="Lao People's Democratic Republic"></asp:ListItem>
                                    <asp:ListItem Value="LV" Text="Latvia"></asp:ListItem>
                                    <asp:ListItem Value="LB" Text="Lebanon"></asp:ListItem>
                                    <asp:ListItem Value="LS" Text="Lesotho"></asp:ListItem>
                                    <asp:ListItem Value="LR" Text="Liberia"></asp:ListItem>
                                    <asp:ListItem Value="LY" Text="Libyan Arab Jamahiriya"></asp:ListItem>
                                    <asp:ListItem Value="LI" Text="Liechtenstein"></asp:ListItem>
                                    <asp:ListItem Value="LT" Text="Lithuania"></asp:ListItem>
                                    <asp:ListItem Value="LU" Text="Luxembourg"></asp:ListItem>
                                    <asp:ListItem Value="MO" Text="Macao"></asp:ListItem>
                                    <asp:ListItem Value="MK" Text="Macedonia, the former Yugoslav Republic of"></asp:ListItem>
                                    <asp:ListItem Value="MG" Text="Madagascar"></asp:ListItem>
                                    <asp:ListItem Value="MW" Text="Malawi"></asp:ListItem>
                                    <asp:ListItem Value="MY" Text="Malaysia"></asp:ListItem>
                                    <asp:ListItem Value="MV" Text="Maldives"></asp:ListItem>
                                    <asp:ListItem Value="ML" Text="Mali"></asp:ListItem>
                                    <asp:ListItem Value="MT" Text="Malta"></asp:ListItem>
                                    <asp:ListItem Value="MH" Text="Marshall Islands"></asp:ListItem>
                                    <asp:ListItem Value="MQ" Text="Martinique"></asp:ListItem>
                                    <asp:ListItem Value="MR" Text="Mauritania"></asp:ListItem>
                                    <asp:ListItem Value="MU" Text="Mauritius"></asp:ListItem>
                                    <asp:ListItem Value="YT" Text="Mayotte"></asp:ListItem>
                                    <asp:ListItem Value="MX" Text="Mexico"></asp:ListItem>
                                    <asp:ListItem Value="FM" Text="Micronesia, Federated States of"></asp:ListItem>
                                    <asp:ListItem Value="MD" Text="Moldova, Republic of"></asp:ListItem>
                                    <asp:ListItem Value="MC" Text="Monaco"></asp:ListItem>
                                    <asp:ListItem Value="MN" Text="Mongolia"></asp:ListItem>
                                    <asp:ListItem Value="ME" Text="Montenegro"></asp:ListItem>
                                    <asp:ListItem Value="MS" Text="Montserrat"></asp:ListItem>
                                    <asp:ListItem Value="MA" Text="Morocco"></asp:ListItem>
                                    <asp:ListItem Value="MZ" Text="Mozambique"></asp:ListItem>
                                    <asp:ListItem Value="MM" Text="Myanmar"></asp:ListItem>
                                    <asp:ListItem Value="NA" Text="Namibia"></asp:ListItem>
                                    <asp:ListItem Value="NR" Text="Nauru"></asp:ListItem>
                                    <asp:ListItem Value="NP" Text="Nepal"></asp:ListItem>
                                    <asp:ListItem Value="NL" Text="Netherlands"></asp:ListItem>
                                    <asp:ListItem Value="NC" Text="New Caledonia"></asp:ListItem>
                                    <asp:ListItem Value="NZ" Text="New Zealand"></asp:ListItem>
                                    <asp:ListItem Value="NI" Text="Nicaragua"></asp:ListItem>
                                    <asp:ListItem Value="NE" Text="Niger"></asp:ListItem>
                                    <asp:ListItem Value="NG" Text="Nigeria"></asp:ListItem>
                                    <asp:ListItem Value="NU" Text="Niue"></asp:ListItem>
                                    <asp:ListItem Value="NF" Text="Norfolk Island"></asp:ListItem>
                                    <asp:ListItem Value="MP" Text="Northern Mariana Islands"></asp:ListItem>
                                    <asp:ListItem Value="NO" Text="Norway"></asp:ListItem>
                                    <asp:ListItem Value="OM" Text="Oman"></asp:ListItem>
                                    <asp:ListItem Value="PK" Text="Pakistan"></asp:ListItem>
                                    <asp:ListItem Value="PW" Text="Palau"></asp:ListItem>
                                    <asp:ListItem Value="PS" Text="Palestinian Territory, Occupied"></asp:ListItem>
                                    <asp:ListItem Value="PA" Text="Panama"></asp:ListItem>
                                    <asp:ListItem Value="PG" Text="Papua New Guinea"></asp:ListItem>
                                    <asp:ListItem Value="PY" Text="Paraguay"></asp:ListItem>
                                    <asp:ListItem Value="PE" Text="Peru"></asp:ListItem>
                                    <asp:ListItem Value="PH" Text="Philippines"></asp:ListItem>
                                    <asp:ListItem Value="PN" Text="Pitcairn"></asp:ListItem>
                                    <asp:ListItem Value="PL" Text="Poland"></asp:ListItem>
                                    <asp:ListItem Value="PT" Text="Portugal"></asp:ListItem>
                                    <asp:ListItem Value="PR" Text="Puerto Rico"></asp:ListItem>
                                    <asp:ListItem Value="QA" Text="Qatar"></asp:ListItem>
                                    <asp:ListItem Value="RE" Text="Reunion"></asp:ListItem>
                                    <asp:ListItem Value="RO" Text="Romania"></asp:ListItem>
                                    <asp:ListItem Value="RU" Text="Russian Federation"></asp:ListItem>
                                    <asp:ListItem Value="RW" Text="Rwanda"></asp:ListItem>
                                    <asp:ListItem Value="BL" Text="Saint Barthelemy"></asp:ListItem>
                                    <asp:ListItem Value="SH" Text="Saint Helena"></asp:ListItem>
                                    <asp:ListItem Value="KN" Text="Saint Kitts and Nevis"></asp:ListItem>
                                    <asp:ListItem Value="LC" Text="Saint Lucia"></asp:ListItem>
                                    <asp:ListItem Value="MF" Text="Saint Martin (French part)"></asp:ListItem>
                                    <asp:ListItem Value="PM" Text="Saint Pierre and Miquelon"></asp:ListItem>
                                    <asp:ListItem Value="VC" Text="Saint Vincent and the Grenadines"></asp:ListItem>
                                    <asp:ListItem Value="WS" Text="Samoa"></asp:ListItem>
                                    <asp:ListItem Value="SM" Text="San Marino"></asp:ListItem>
                                    <asp:ListItem Value="ST" Text="Sao Tome and Principe"></asp:ListItem>
                                    <asp:ListItem Value="SA" Text="Saudi Arabia"></asp:ListItem>
                                    <asp:ListItem Value="SN" Text="Senegal"></asp:ListItem>
                                    <asp:ListItem Value="RS" Text="Serbia"></asp:ListItem>
                                    <asp:ListItem Value="SC" Text="Seychelles"></asp:ListItem>
                                    <asp:ListItem Value="SL" Text="Sierra Leone"></asp:ListItem>
                                    <asp:ListItem Value="SG" Text="Singapore"></asp:ListItem>
                                    <asp:ListItem Value="SX" Text="Sint Maarten"></asp:ListItem>
                                    <asp:ListItem Value="SK" Text="Slovakia"></asp:ListItem>
                                    <asp:ListItem Value="SI" Text="Slovenia"></asp:ListItem>
                                    <asp:ListItem Value="SB" Text="Solomon Islands"></asp:ListItem>
                                    <asp:ListItem Value="SO" Text="Somalia"></asp:ListItem>
                                    <asp:ListItem Value="ZA" Text="South Africa"></asp:ListItem>
                                    <asp:ListItem Value="GS" Text="South Georgia and the South Sandwich Islands"></asp:ListItem>
                                    <asp:ListItem Value="ES" Text="Spain"></asp:ListItem>
                                    <asp:ListItem Value="LK" Text="Sri Lanka"></asp:ListItem>
                                    <asp:ListItem Value="SD" Text="Sudan"></asp:ListItem>
                                    <asp:ListItem Value="SR" Text="Suriname"></asp:ListItem>
                                    <asp:ListItem Value="SJ" Text="Svalbard and Jan Mayen"></asp:ListItem>
                                    <asp:ListItem Value="SZ" Text="Swaziland"></asp:ListItem>
                                    <asp:ListItem Value="SE" Text="Sweden"></asp:ListItem>
                                    <asp:ListItem Value="CH" Text="Switzerland"></asp:ListItem>
                                    <asp:ListItem Value="SY" Text="Syrian Arab Republic"></asp:ListItem>
                                    <asp:ListItem Value="TW" Text="Taiwan"></asp:ListItem>
                                    <asp:ListItem Value="TJ" Text="Tajikistan"></asp:ListItem>
                                    <asp:ListItem Value="TZ" Text="Tanzania, United Republic of"></asp:ListItem>
                                    <asp:ListItem Value="TH" Text="Thailand"></asp:ListItem>
                                    <asp:ListItem Value="TL" Text="Timor-Leste"></asp:ListItem>
                                    <asp:ListItem Value="TG" Text="Togo"></asp:ListItem>
                                    <asp:ListItem Value="TK" Text="Tokelau"></asp:ListItem>
                                    <asp:ListItem Value="TO" Text="Tonga"></asp:ListItem>
                                    <asp:ListItem Value="TT" Text="Trinidad and Tobago"></asp:ListItem>
                                    <asp:ListItem Value="TN" Text="Tunisia"></asp:ListItem>
                                    <asp:ListItem Value="TR" Text="Turkey"></asp:ListItem>
                                    <asp:ListItem Value="TM" Text="Turkmenistan"></asp:ListItem>
                                    <asp:ListItem Value="TC" Text="Turks and Caicos Islands"></asp:ListItem>
                                    <asp:ListItem Value="TV" Text="Tuvalu"></asp:ListItem>
                                    <asp:ListItem Value="UG" Text="Uganda"></asp:ListItem>
                                    <asp:ListItem Value="UA" Text="Ukraine"></asp:ListItem>
                                    <asp:ListItem Value="AE" Text="United Arab Emirates"></asp:ListItem>
                                    <asp:ListItem Value="GB" Text="United Kingdom"></asp:ListItem>
                        
                                    <asp:ListItem Value="UM" Text="United States Minor Outlying Islands"></asp:ListItem>
                                    <asp:ListItem Value="UY" Text="Uruguay"></asp:ListItem>
                                    <asp:ListItem Value="UZ" Text="Uzbekistan"></asp:ListItem>
                                    <asp:ListItem Value="VU" Text="Vanuatu"></asp:ListItem>
                                    <asp:ListItem Value="VE" Text="Venezuela"></asp:ListItem>
                                    <asp:ListItem Value="VN" Text="Viet Nam"></asp:ListItem>
                                    <asp:ListItem Value="VG" Text="Virgin Islands, British"></asp:ListItem>
                                    <asp:ListItem Value="VI" Text="Virgin Islands, U.S."></asp:ListItem>
                                    <asp:ListItem Value="WF" Text="Wallis and Futuna"></asp:ListItem>
                                    <asp:ListItem Value="EH" Text="Western Sahara"></asp:ListItem>
                                    <asp:ListItem Value="YE" Text="Yemen"></asp:ListItem>
                                    <asp:ListItem Value="ZM" Text="Zambia"></asp:ListItem>
                                    <asp:ListItem Value="ZW" Text="Zimbabwe"></asp:ListItem>

                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr id="trState">
                            <td class="inputtitle" bgcolor="#c9c9c9">State/Province:</td>
                            <td class="inputbox">
                                <asp:DropDownList ID="ddlProvince" runat="server" class="textbox">
                                    <asp:ListItem Value="AB" Text="Alberta"></asp:ListItem>
                                    <asp:ListItem Value="BC" Text="British Columbia"></asp:ListItem>
                                    <asp:ListItem Value="MB" Text="Manitoba"></asp:ListItem>
                                    <asp:ListItem Value="NB" Text="New Brunswick"></asp:ListItem>
                                    <asp:ListItem Value="NL" Text="Newfoundland and Labrador"></asp:ListItem>
                                    <asp:ListItem Value="NS" Text="Nova Scotia"></asp:ListItem>
                                    <asp:ListItem Value="NT" Text="Northwest Territories"></asp:ListItem>
                                    <asp:ListItem Value="NU" Text="Nunavut"></asp:ListItem>
                                    <asp:ListItem Value="ON" Text="Ontario"></asp:ListItem>
                                    <asp:ListItem Value="PE" Text="Prince Edward Island"></asp:ListItem>
                                    <asp:ListItem Value="QC" Text="Quebec"></asp:ListItem>
                                    <asp:ListItem Value="SK" Text="Saskatchewan"></asp:ListItem>
                                    <asp:ListItem Value="YT" Text="Yukon"></asp:ListItem>
                                </asp:DropDownList>

                                <asp:DropDownList ID="ddlState" runat="server" class="textbox">
                                    <asp:ListItem Value="AA" Text="Armed Forces America"></asp:ListItem>
                                    <asp:ListItem Value="AE" Text="Armed Forces"></asp:ListItem>
                                    <asp:ListItem Value="AP" Text="Armed Forces Pacific"></asp:ListItem>
                                    <asp:ListItem Value="AK" Text="Alaska"></asp:ListItem>
                                    <asp:ListItem Value="AL" Text="Alabama"></asp:ListItem>
                                    <asp:ListItem Value="AR" Text="Arkansas"></asp:ListItem>
                                    <asp:ListItem Value="AZ" Text="Arizona"></asp:ListItem>
                                    <asp:ListItem Value="CA" Text="California"></asp:ListItem>
                                    <asp:ListItem Value="CO" Text="Colorado"></asp:ListItem>
                                    <asp:ListItem Value="CT" Text="Connecticut"></asp:ListItem>
                                    <asp:ListItem Value="DC" Text="District of Columbia"></asp:ListItem>
                                    <asp:ListItem Value="DE" Text="Delaware"></asp:ListItem>
                                    <asp:ListItem Value="FL" Text="Florida"></asp:ListItem>
                                    <asp:ListItem Value="GA" Text="Georgia"></asp:ListItem>
                                    <asp:ListItem Value="GU" Text="Guam"></asp:ListItem>
                                    <asp:ListItem Value="HI" Text="Hawaii"></asp:ListItem>
                                    <asp:ListItem Value="IA" Text="Iowa"></asp:ListItem>
                                    <asp:ListItem Value="ID" Text="Idaho"></asp:ListItem>
                                    <asp:ListItem Value="IL" Text="Illinois"></asp:ListItem>
                                    <asp:ListItem Value="IN" Text="Indiana"></asp:ListItem>
                                    <asp:ListItem Value="KS" Text="Kansas"></asp:ListItem>
                                    <asp:ListItem Value="KY" Text="Kentucky"></asp:ListItem>
                                    <asp:ListItem Value="LA" Text="Louisiana"></asp:ListItem>
                                    <asp:ListItem Value="MA" Text="Massachusetts"></asp:ListItem>
                                    <asp:ListItem Value="MD" Text="Maryland"></asp:ListItem>
                                    <asp:ListItem Value="ME" Text="Maine"></asp:ListItem>
                                    <asp:ListItem Value="MI" Text="Michigan"></asp:ListItem>
                                    <asp:ListItem Value="MN" Text="Minnesota"></asp:ListItem>
                                    <asp:ListItem Value="MO" Text="Missouri"></asp:ListItem>
                                    <asp:ListItem Value="MS" Text="Mississippi"></asp:ListItem>
                                    <asp:ListItem Value="MT" Text="Montana"></asp:ListItem>
                                    <asp:ListItem Value="NC" Text="North Carolina"></asp:ListItem>
                                    <asp:ListItem Value="ND" Text="North Dakota"></asp:ListItem>
                                    <asp:ListItem Value="NE" Text="Nebraska"></asp:ListItem>
                                    <asp:ListItem Value="NH" Text="New Hampshire"></asp:ListItem>
                                    <asp:ListItem Value="NJ" Text="New Jersey"></asp:ListItem>
                                    <asp:ListItem Value="NM" Text="New Mexico"></asp:ListItem>
                                    <asp:ListItem Value="NV" Text="Nevada"></asp:ListItem>
                                    <asp:ListItem Value="NY" Text="New York"></asp:ListItem>
                                    <asp:ListItem Value="OH" Text="Ohio"></asp:ListItem>
                                    <asp:ListItem Value="OK" Text="Oklahoma"></asp:ListItem>
                                    <asp:ListItem Value="OR" Text="Oregon"></asp:ListItem>
                                    <asp:ListItem Value="PA" Text="Pennsylvania"></asp:ListItem>
                                    <asp:ListItem Value="PR" Text="Puerto Rico"></asp:ListItem>
                                    <asp:ListItem Value="RI" Text="Rhode Island"></asp:ListItem>
                                    <asp:ListItem Value="SC" Text="South Carolina"></asp:ListItem>
                                    <asp:ListItem Value="SD" Text="South Dakota"></asp:ListItem>
                                    <asp:ListItem Value="TN" Text="Tennessee"></asp:ListItem>
                                    <asp:ListItem Value="TX" Text="Texas"></asp:ListItem>
                                    <asp:ListItem Value="UT" Text="Utah"></asp:ListItem>
                                    <asp:ListItem Value="VA" Text="Virginia"></asp:ListItem>
                                    <asp:ListItem Value="VI" Text="Virgin Islands"></asp:ListItem>
                                    <asp:ListItem Value="VT" Text="Vermont"></asp:ListItem>
                                    <asp:ListItem Value="WA" Text="Washington"></asp:ListItem>
                                    <asp:ListItem Value="WI" Text="Wisconsin"></asp:ListItem>
                                    <asp:ListItem Value="WV" Text="West Virginia"></asp:ListItem>
                                    <asp:ListItem Value="WY" Text="Wyoming"></asp:ListItem>

                                </asp:DropDownList>
                
                            </td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>Zip:</td>
                            <td><asp:TextBox ID="txtZip" runat="server"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td width="100" bgcolor="#c9c9c9" nowrap>Invoice Template:</td>
                            <td><asp:DropDownList ID="ddlInvoiceTemplate" runat="server">
                                <asp:ListItem Selected="True" Text="EPM Live Default" Value="4028e487336cf73001338b20f72b50ff"></asp:ListItem>
                                <asp:ListItem Selected="False" Text="EPM Live Bank Info" Value="4028e48833fd603f0134392b727b46ef"></asp:ListItem>
                                <asp:ListItem Selected="False" Text="LMR Solutions" Value="4028e48733fd62b60134392d12da27f8"></asp:ListItem>
                            </asp:DropDownList></td>
                        </tr>
                    </table>
                    <script language="javascript">
                        function checkCountry() {
                            var country = document.getElementById("<%=ddlCountry.ClientID%>").value;
                            if (country == "US") {
                                document.getElementById("trState").style.display = "";
                                document.getElementById("<%=ddlState.ClientID%>").style.display = "";
                                document.getElementById("<%=ddlProvince.ClientID%>").style.display = "none";
                            }
                            else if (country == "CA") {
                                document.getElementById("trState").style.display = "";
                                document.getElementById("<%=ddlState.ClientID%>").style.display = "none";
                                document.getElementById("<%=ddlProvince.ClientID%>").style.display = "";
                            }
                            else {
                                document.getElementById("trState").style.display = "none";
                            }
                        }

                        checkCountry();
</script>

<hr />
<asp:Button ID="btnCreate" runat="server" Text="Create Order" onclick="btnCreate_Click" CssClass="searchbutton" style="width: 100px" />

<br /><br /><br />
<hr style="border: 2px solid black" />
<font style="line-height:16px; padding-top:10px;">
<asp:Label ID="lblOutput" runat="server"></asp:Label>
</font>
<script language="javascript">
    initmb();
</script>
</asp:Content>
