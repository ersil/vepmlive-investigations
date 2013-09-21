﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="EPMLiveSignals.Layouts.EPMLive.Chat" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">
    <script src="/signalr/hubs" type="text/javascript"></script>
    
    <SharePoint:StyleBlock runat="server">
        .container {
            background-color: #99CCFF;
            border: thick solid #808080;
            padding: 20px;
            margin: 20px;
        }
    </SharePoint:StyleBlock>
    
    <SharePoint:ScriptBlock runat="server">
        (function() {
            'use strict';
            
            function initChat() {
                $(function () {
                    // Declare a proxy to reference the hub. 
                    var chat = $.connection.chatHub;
                    
                    // Create a function that the hub can call to broadcast messages.
                    chat.client.broadcastMessage = function (name, message) {
                        // Html encode display name and message. 
                        var encodedName = $('<div />').text(name).html();
                        var encodedMsg = $('<div />').text(message).html();
                        
                        // Add the message to the page. 
                        $('#discussion').append('<li><strong>' + encodedName + '</strong>:&nbsp;&nbsp;' + encodedMsg + '</li>');
                    };
                    
                    // Get the user name and store it to prepend to messages.
                    $('#displayname').val(prompt('Enter your name:', ''));
                    
                    // Set initial focus to message input box.  
                    $('#message').focus();
                    
                    // Start the connection.
                    $.connection.hub.start().done(function () {
                        $('#sendmessage').click(function () {
                            // Call the Send method on the hub. 
                            chat.server.send($('#displayname').val(), $('#message').val());
                            
                            // Clear text box and reset focus for next comment. 
                            $('#message').val('').focus();
                        });
                    });
                });
            }

            ExecuteOrDelayUntilScriptLoaded(initChat, 'jquery.min.js');
        })();
    </SharePoint:ScriptBlock>
</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <div class="container">
        <input type="text" id="message" />
        <input type="button" id="sendmessage" value="Send" />
        <input type="hidden" id="displayname" />
        <ul id="discussion"></ul>
    </div>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
EPM Live Chat
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
EPM Live Chat
</asp:Content>
