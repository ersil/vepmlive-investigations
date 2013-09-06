﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Import Namespace="Microsoft.SharePoint.ApplicationPages" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="Utilities" Namespace="Microsoft.SharePoint.Utilities" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="asp" Namespace="System.Web.UI" Assembly="System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" %>
<%@ Import Namespace="Microsoft.SharePoint" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IconPicker.aspx.cs" Inherits="EPMLiveCore.Layouts.epmlive.IconPicker" DynamicMasterPageFile="~masterurl/default.master" %>

<asp:Content ID="PageHead" ContentPlaceHolderID="PlaceHolderAdditionalPageHead" runat="server">

</asp:Content>

<asp:Content ID="Main" ContentPlaceHolderID="PlaceHolderMain" runat="server">
    <link rel="stylesheet" href="style.css" />
    <div class="w-main centered">
	
	<div class="clear"></div>
	<section class="mtm clearfix" id="glyphs">
	<header>
		<h1>Available Icons</h1>
		<p>Click the icon you want to use from the list below.</p>
	</header>
	<span class="box1">
		<span aria-hidden="true" class="icon-home"></span>
		&nbsp;icon-home
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-2"></span>
		&nbsp;icon-home-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-3"></span>
		&nbsp;icon-home-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-4"></span>
		&nbsp;icon-home-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-5"></span>
		&nbsp;icon-home-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-6"></span>
		&nbsp;icon-home-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-7"></span>
		&nbsp;icon-home-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-8"></span>
		&nbsp;icon-home-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-9"></span>
		&nbsp;icon-home-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-10"></span>
		&nbsp;icon-home-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-11"></span>
		&nbsp;icon-home-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-office"></span>
		&nbsp;icon-office
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-newspaper"></span>
		&nbsp;icon-newspaper
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil"></span>
		&nbsp;icon-pencil
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil-2"></span>
		&nbsp;icon-pencil-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil-3"></span>
		&nbsp;icon-pencil-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil-4"></span>
		&nbsp;icon-pencil-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil-5"></span>
		&nbsp;icon-pencil-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pencil-6"></span>
		&nbsp;icon-pencil-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quill"></span>
		&nbsp;icon-quill
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quill-2"></span>
		&nbsp;icon-quill-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quill-3"></span>
		&nbsp;icon-quill-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pen"></span>
		&nbsp;icon-pen
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pen-2"></span>
		&nbsp;icon-pen-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pen-3"></span>
		&nbsp;icon-pen-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pen-4"></span>
		&nbsp;icon-pen-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-safari"></span>
		&nbsp;icon-safari
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-opera"></span>
		&nbsp;icon-opera
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-IE"></span>
		&nbsp;icon-IE
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-firefox"></span>
		&nbsp;icon-firefox
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-chrome"></span>
		&nbsp;icon-chrome
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-css3"></span>
		&nbsp;icon-css3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-html5"></span>
		&nbsp;icon-html5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-html5-2"></span>
		&nbsp;icon-html5-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-css"></span>
		&nbsp;icon-file-css
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-xml"></span>
		&nbsp;icon-file-xml
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-powerpoint"></span>
		&nbsp;icon-file-powerpoint
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-zip"></span>
		&nbsp;icon-file-zip
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-excel"></span>
		&nbsp;icon-file-excel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-word"></span>
		&nbsp;icon-file-word
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-openoffice"></span>
		&nbsp;icon-file-openoffice
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-pdf"></span>
		&nbsp;icon-file-pdf
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-libreoffice"></span>
		&nbsp;icon-libreoffice
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-yelp"></span>
		&nbsp;icon-yelp
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paypal"></span>
		&nbsp;icon-paypal
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paypal-2"></span>
		&nbsp;icon-paypal-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paypal-3"></span>
		&nbsp;icon-paypal-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-foursquare"></span>
		&nbsp;icon-foursquare
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-foursquare-2"></span>
		&nbsp;icon-foursquare-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flattr"></span>
		&nbsp;icon-flattr
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-xing"></span>
		&nbsp;icon-xing
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-xing-2"></span>
		&nbsp;icon-xing-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pinterest"></span>
		&nbsp;icon-pinterest
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pinterest-2"></span>
		&nbsp;icon-pinterest-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-IcoMoon"></span>
		&nbsp;icon-IcoMoon
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stackoverflow"></span>
		&nbsp;icon-stackoverflow
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stumbleupon"></span>
		&nbsp;icon-stumbleupon
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stumbleupon-2"></span>
		&nbsp;icon-stumbleupon-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-delicious"></span>
		&nbsp;icon-delicious
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lastfm"></span>
		&nbsp;icon-lastfm
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lastfm-2"></span>
		&nbsp;icon-lastfm-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-linkedin"></span>
		&nbsp;icon-linkedin
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-reddit"></span>
		&nbsp;icon-reddit
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-skype"></span>
		&nbsp;icon-skype
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-soundcloud"></span>
		&nbsp;icon-soundcloud
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-soundcloud-2"></span>
		&nbsp;icon-soundcloud-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-windows8"></span>
		&nbsp;icon-windows8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-windows"></span>
		&nbsp;icon-windows
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-android"></span>
		&nbsp;icon-android
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-finder"></span>
		&nbsp;icon-finder
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-apple"></span>
		&nbsp;icon-apple
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tux"></span>
		&nbsp;icon-tux
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-yahoo"></span>
		&nbsp;icon-yahoo
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tumblr"></span>
		&nbsp;icon-tumblr
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tumblr-2"></span>
		&nbsp;icon-tumblr-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-blogger"></span>
		&nbsp;icon-blogger
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-blogger-2"></span>
		&nbsp;icon-blogger-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-joomla"></span>
		&nbsp;icon-joomla
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wordpress"></span>
		&nbsp;icon-wordpress
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wordpress-2"></span>
		&nbsp;icon-wordpress-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-github"></span>
		&nbsp;icon-github
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-github-2"></span>
		&nbsp;icon-github-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-github-3"></span>
		&nbsp;icon-github-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-github-4"></span>
		&nbsp;icon-github-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-github-5"></span>
		&nbsp;icon-github-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-steam"></span>
		&nbsp;icon-steam
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-steam-2"></span>
		&nbsp;icon-steam-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-deviantart"></span>
		&nbsp;icon-deviantart
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-deviantart-2"></span>
		&nbsp;icon-deviantart-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-forrst"></span>
		&nbsp;icon-forrst
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-forrst-2"></span>
		&nbsp;icon-forrst-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dribbble"></span>
		&nbsp;icon-dribbble
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dribbble-2"></span>
		&nbsp;icon-dribbble-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dribbble-3"></span>
		&nbsp;icon-dribbble-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-picassa"></span>
		&nbsp;icon-picassa
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-picassa-2"></span>
		&nbsp;icon-picassa-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flickr"></span>
		&nbsp;icon-flickr
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flickr-2"></span>
		&nbsp;icon-flickr-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flickr-3"></span>
		&nbsp;icon-flickr-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flickr-4"></span>
		&nbsp;icon-flickr-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lanyrd"></span>
		&nbsp;icon-lanyrd
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-vimeo"></span>
		&nbsp;icon-vimeo
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-vimeo2"></span>
		&nbsp;icon-vimeo2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-vimeo-2"></span>
		&nbsp;icon-vimeo-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-youtube"></span>
		&nbsp;icon-youtube
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-youtube-2"></span>
		&nbsp;icon-youtube-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-feed"></span>
		&nbsp;icon-feed
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-feed-2"></span>
		&nbsp;icon-feed-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-feed-3"></span>
		&nbsp;icon-feed-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-twitter"></span>
		&nbsp;icon-twitter
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-twitter-2"></span>
		&nbsp;icon-twitter-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-twitter-3"></span>
		&nbsp;icon-twitter-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-instagram"></span>
		&nbsp;icon-instagram
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-facebook"></span>
		&nbsp;icon-facebook
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-facebook-2"></span>
		&nbsp;icon-facebook-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-facebook-3"></span>
		&nbsp;icon-facebook-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-facebook-4"></span>
		&nbsp;icon-facebook-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google-drive"></span>
		&nbsp;icon-google-drive
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google-plus"></span>
		&nbsp;icon-google-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google-plus-2"></span>
		&nbsp;icon-google-plus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google-plus-3"></span>
		&nbsp;icon-google-plus-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google-plus-4"></span>
		&nbsp;icon-google-plus-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-google"></span>
		&nbsp;icon-google
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mail"></span>
		&nbsp;icon-mail
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mail-2"></span>
		&nbsp;icon-mail-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mail-3"></span>
		&nbsp;icon-mail-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mail-4"></span>
		&nbsp;icon-mail-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-share"></span>
		&nbsp;icon-share
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-share-2"></span>
		&nbsp;icon-share-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-9"></span>
		&nbsp;icon-seven-segment-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-8"></span>
		&nbsp;icon-seven-segment-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-7"></span>
		&nbsp;icon-seven-segment-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-6"></span>
		&nbsp;icon-seven-segment-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-5"></span>
		&nbsp;icon-seven-segment-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-4"></span>
		&nbsp;icon-seven-segment-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-3"></span>
		&nbsp;icon-seven-segment-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-2"></span>
		&nbsp;icon-seven-segment-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-1"></span>
		&nbsp;icon-seven-segment-1
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-seven-segment-0"></span>
		&nbsp;icon-seven-segment-0
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-console"></span>
		&nbsp;icon-console
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-code"></span>
		&nbsp;icon-code
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-embed"></span>
		&nbsp;icon-embed
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-popout"></span>
		&nbsp;icon-popout
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-new-tab"></span>
		&nbsp;icon-new-tab
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-new-tab-2"></span>
		&nbsp;icon-new-tab-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-share-3"></span>
		&nbsp;icon-share-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-indent-decrease"></span>
		&nbsp;icon-indent-decrease
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-indent-increase"></span>
		&nbsp;icon-indent-increase
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-justify"></span>
		&nbsp;icon-paragraph-justify
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-right"></span>
		&nbsp;icon-paragraph-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-center"></span>
		&nbsp;icon-paragraph-center
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-left"></span>
		&nbsp;icon-paragraph-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-indent-decrease-2"></span>
		&nbsp;icon-indent-decrease-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-indent-increase-2"></span>
		&nbsp;icon-indent-increase-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-justify-2"></span>
		&nbsp;icon-paragraph-justify-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-right-2"></span>
		&nbsp;icon-paragraph-right-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-center-2"></span>
		&nbsp;icon-paragraph-center-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-left-2"></span>
		&nbsp;icon-paragraph-left-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-justify-3"></span>
		&nbsp;icon-paragraph-justify-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-right-3"></span>
		&nbsp;icon-paragraph-right-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-center-3"></span>
		&nbsp;icon-paragraph-center-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paragraph-left-3"></span>
		&nbsp;icon-paragraph-left-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-right-to-left"></span>
		&nbsp;icon-right-to-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-left-to-right"></span>
		&nbsp;icon-left-to-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pilcrow"></span>
		&nbsp;icon-pilcrow
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-insert-template"></span>
		&nbsp;icon-insert-template
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-table"></span>
		&nbsp;icon-table
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-table-2"></span>
		&nbsp;icon-table-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clear-formatting"></span>
		&nbsp;icon-clear-formatting
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pagebreak"></span>
		&nbsp;icon-pagebreak
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-highlight"></span>
		&nbsp;icon-highlight
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-text-color"></span>
		&nbsp;icon-text-color
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-subscript"></span>
		&nbsp;icon-subscript
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-superscript"></span>
		&nbsp;icon-superscript
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-subscript-2"></span>
		&nbsp;icon-subscript-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-superscript-2"></span>
		&nbsp;icon-superscript-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-page-break"></span>
		&nbsp;icon-page-break
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-page-break-2"></span>
		&nbsp;icon-page-break-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-nbsp"></span>
		&nbsp;icon-nbsp
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sigma"></span>
		&nbsp;icon-sigma
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-omega"></span>
		&nbsp;icon-omega
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-strikethrough"></span>
		&nbsp;icon-strikethrough
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-italic"></span>
		&nbsp;icon-italic
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-underline"></span>
		&nbsp;icon-underline
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bold"></span>
		&nbsp;icon-bold
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-font-size"></span>
		&nbsp;icon-font-size
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-strikethrough-2"></span>
		&nbsp;icon-strikethrough-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-strikethrough-3"></span>
		&nbsp;icon-strikethrough-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-italic-2"></span>
		&nbsp;icon-italic-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-underline-2"></span>
		&nbsp;icon-underline-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bold-2"></span>
		&nbsp;icon-bold-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-width"></span>
		&nbsp;icon-width
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-height"></span>
		&nbsp;icon-height
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-text-width"></span>
		&nbsp;icon-text-width
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-text-height"></span>
		&nbsp;icon-text-height
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-type"></span>
		&nbsp;icon-type
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-font-size-2"></span>
		&nbsp;icon-font-size-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-font"></span>
		&nbsp;icon-font
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-filter"></span>
		&nbsp;icon-filter
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-filter-2"></span>
		&nbsp;icon-filter-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-filter-3"></span>
		&nbsp;icon-filter-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-filter-4"></span>
		&nbsp;icon-filter-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scissors"></span>
		&nbsp;icon-scissors
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scissors-2"></span>
		&nbsp;icon-scissors-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scissors-3"></span>
		&nbsp;icon-scissors-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rulers"></span>
		&nbsp;icon-rulers
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-vector"></span>
		&nbsp;icon-vector
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-crop"></span>
		&nbsp;icon-crop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-crop-2"></span>
		&nbsp;icon-crop-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-circle"></span>
		&nbsp;icon-circle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-circle-2"></span>
		&nbsp;icon-circle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-radio-unchecked"></span>
		&nbsp;icon-radio-unchecked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-radio-checked"></span>
		&nbsp;icon-radio-checked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-partial"></span>
		&nbsp;icon-checkbox-partial
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-unchecked"></span>
		&nbsp;icon-checkbox-unchecked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-checked"></span>
		&nbsp;icon-checkbox-checked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-partial-2"></span>
		&nbsp;icon-checkbox-partial-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-unchecked-2"></span>
		&nbsp;icon-checkbox-unchecked-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox"></span>
		&nbsp;icon-checkbox
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-partial-3"></span>
		&nbsp;icon-checkbox-partial-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-square"></span>
		&nbsp;icon-square
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-unchecked-3"></span>
		&nbsp;icon-checkbox-unchecked-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkbox-checked-2"></span>
		&nbsp;icon-checkbox-checked-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-command"></span>
		&nbsp;icon-command
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-left"></span>
		&nbsp;icon-key-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-down"></span>
		&nbsp;icon-key-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-right"></span>
		&nbsp;icon-key-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-up"></span>
		&nbsp;icon-key-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-A"></span>
		&nbsp;icon-key-A
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-keyboard"></span>
		&nbsp;icon-key-keyboard
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sort"></span>
		&nbsp;icon-sort
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sort-2"></span>
		&nbsp;icon-sort-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-transmission"></span>
		&nbsp;icon-transmission
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-transmission-2"></span>
		&nbsp;icon-transmission-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tab"></span>
		&nbsp;icon-tab
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-backspace"></span>
		&nbsp;icon-backspace
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-backspace-2"></span>
		&nbsp;icon-backspace-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-backspace-3"></span>
		&nbsp;icon-backspace-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-esc"></span>
		&nbsp;icon-esc
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-enter"></span>
		&nbsp;icon-enter
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-enter-2"></span>
		&nbsp;icon-enter-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-close"></span>
		&nbsp;icon-menu-close
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-close-2"></span>
		&nbsp;icon-menu-close-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu"></span>
		&nbsp;icon-menu
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-2"></span>
		&nbsp;icon-menu-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left"></span>
		&nbsp;icon-arrow-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down"></span>
		&nbsp;icon-arrow-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right"></span>
		&nbsp;icon-arrow-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up"></span>
		&nbsp;icon-arrow-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-2"></span>
		&nbsp;icon-arrow-left-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-2"></span>
		&nbsp;icon-arrow-down-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-2"></span>
		&nbsp;icon-arrow-right-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-2"></span>
		&nbsp;icon-arrow-up-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-3"></span>
		&nbsp;icon-arrow-left-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-3"></span>
		&nbsp;icon-arrow-down-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-3"></span>
		&nbsp;icon-arrow-right-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-3"></span>
		&nbsp;icon-arrow-up-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-4"></span>
		&nbsp;icon-arrow-left-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-4"></span>
		&nbsp;icon-arrow-down-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-4"></span>
		&nbsp;icon-arrow-right-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-4"></span>
		&nbsp;icon-arrow-up-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-5"></span>
		&nbsp;icon-arrow-left-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-5"></span>
		&nbsp;icon-arrow-down-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-5"></span>
		&nbsp;icon-arrow-right-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-5"></span>
		&nbsp;icon-arrow-up-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-6"></span>
		&nbsp;icon-arrow-left-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-6"></span>
		&nbsp;icon-arrow-down-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-6"></span>
		&nbsp;icon-arrow-right-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-6"></span>
		&nbsp;icon-arrow-up-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-7"></span>
		&nbsp;icon-arrow-left-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left"></span>
		&nbsp;icon-arrow-down-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-7"></span>
		&nbsp;icon-arrow-down-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right"></span>
		&nbsp;icon-arrow-down-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-7"></span>
		&nbsp;icon-arrow-right-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right"></span>
		&nbsp;icon-arrow-up-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-7"></span>
		&nbsp;icon-arrow-up-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left"></span>
		&nbsp;icon-arrow-up-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-8"></span>
		&nbsp;icon-arrow-left-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-2"></span>
		&nbsp;icon-arrow-down-left-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-8"></span>
		&nbsp;icon-arrow-down-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-2"></span>
		&nbsp;icon-arrow-down-right-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-8"></span>
		&nbsp;icon-arrow-right-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-2"></span>
		&nbsp;icon-arrow-up-right-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-8"></span>
		&nbsp;icon-arrow-up-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-2"></span>
		&nbsp;icon-arrow-up-left-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-9"></span>
		&nbsp;icon-arrow-left-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-bottom"></span>
		&nbsp;icon-arrow-bottom
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-9"></span>
		&nbsp;icon-arrow-right-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-9"></span>
		&nbsp;icon-arrow-up-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-10"></span>
		&nbsp;icon-arrow-left-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-9"></span>
		&nbsp;icon-arrow-down-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-10"></span>
		&nbsp;icon-arrow-right-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-10"></span>
		&nbsp;icon-arrow-up-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-11"></span>
		&nbsp;icon-arrow-left-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-3"></span>
		&nbsp;icon-arrow-down-left-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-10"></span>
		&nbsp;icon-arrow-down-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-3"></span>
		&nbsp;icon-arrow-down-right-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-11"></span>
		&nbsp;icon-arrow-right-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-3"></span>
		&nbsp;icon-arrow-up-right-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-square"></span>
		&nbsp;icon-arrow-square
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-3"></span>
		&nbsp;icon-arrow-up-left-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow"></span>
		&nbsp;icon-arrow
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-2"></span>
		&nbsp;icon-arrow-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-3"></span>
		&nbsp;icon-arrow-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-4"></span>
		&nbsp;icon-arrow-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-5"></span>
		&nbsp;icon-arrow-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-6"></span>
		&nbsp;icon-arrow-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-7"></span>
		&nbsp;icon-arrow-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-8"></span>
		&nbsp;icon-arrow-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-12"></span>
		&nbsp;icon-arrow-left-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-4"></span>
		&nbsp;icon-arrow-down-left-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-11"></span>
		&nbsp;icon-arrow-down-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-4"></span>
		&nbsp;icon-arrow-down-right-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-12"></span>
		&nbsp;icon-arrow-right-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-4"></span>
		&nbsp;icon-arrow-up-right-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-11"></span>
		&nbsp;icon-arrow-up-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-4"></span>
		&nbsp;icon-arrow-up-left-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-13"></span>
		&nbsp;icon-arrow-left-13
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-5"></span>
		&nbsp;icon-arrow-down-left-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-12"></span>
		&nbsp;icon-arrow-down-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-5"></span>
		&nbsp;icon-arrow-down-right-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-13"></span>
		&nbsp;icon-arrow-right-13
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-5"></span>
		&nbsp;icon-arrow-up-right-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-12"></span>
		&nbsp;icon-arrow-up-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-5"></span>
		&nbsp;icon-arrow-up-left-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-14"></span>
		&nbsp;icon-arrow-left-14
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-6"></span>
		&nbsp;icon-arrow-down-left-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-13"></span>
		&nbsp;icon-arrow-down-13
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-6"></span>
		&nbsp;icon-arrow-down-right-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-14"></span>
		&nbsp;icon-arrow-right-14
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-6"></span>
		&nbsp;icon-arrow-up-right-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-13"></span>
		&nbsp;icon-arrow-up-13
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-6"></span>
		&nbsp;icon-arrow-up-left-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-15"></span>
		&nbsp;icon-arrow-left-15
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-left-7"></span>
		&nbsp;icon-arrow-down-left-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-14"></span>
		&nbsp;icon-arrow-down-14
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-right-7"></span>
		&nbsp;icon-arrow-down-right-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-15"></span>
		&nbsp;icon-arrow-right-15
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-right-7"></span>
		&nbsp;icon-arrow-up-right-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-14"></span>
		&nbsp;icon-arrow-up-14
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-left-7"></span>
		&nbsp;icon-arrow-up-left-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-16"></span>
		&nbsp;icon-arrow-left-16
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-15"></span>
		&nbsp;icon-arrow-down-15
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-16"></span>
		&nbsp;icon-arrow-right-16
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-15"></span>
		&nbsp;icon-arrow-up-15
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-left-17"></span>
		&nbsp;icon-arrow-left-17
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-down-16"></span>
		&nbsp;icon-arrow-down-16
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-17"></span>
		&nbsp;icon-arrow-right-17
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-up-16"></span>
		&nbsp;icon-arrow-up-16
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-right-18"></span>
		&nbsp;icon-arrow-right-18
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-arrow-first"></span>
		&nbsp;icon-arrow-first
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wave"></span>
		&nbsp;icon-wave
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wave-2"></span>
		&nbsp;icon-wave-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shuffle"></span>
		&nbsp;icon-shuffle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shuffle-2"></span>
		&nbsp;icon-shuffle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-loop"></span>
		&nbsp;icon-loop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-loop-2"></span>
		&nbsp;icon-loop-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-loop-3"></span>
		&nbsp;icon-loop-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-loop-4"></span>
		&nbsp;icon-loop-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-loop-5"></span>
		&nbsp;icon-loop-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute"></span>
		&nbsp;icon-volume-mute
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute-2"></span>
		&nbsp;icon-volume-mute-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume0"></span>
		&nbsp;icon-volume0
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume1"></span>
		&nbsp;icon-volume1
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume2"></span>
		&nbsp;icon-volume2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume3"></span>
		&nbsp;icon-volume3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume4"></span>
		&nbsp;icon-volume4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume5"></span>
		&nbsp;icon-volume5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-decrease"></span>
		&nbsp;icon-volume-decrease
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-increase"></span>
		&nbsp;icon-volume-increase
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute-3"></span>
		&nbsp;icon-volume-mute-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute-4"></span>
		&nbsp;icon-volume-mute-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-low"></span>
		&nbsp;icon-volume-low
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-medium"></span>
		&nbsp;icon-volume-medium
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-high"></span>
		&nbsp;icon-volume-high
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-decrease-2"></span>
		&nbsp;icon-volume-decrease-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-increase-2"></span>
		&nbsp;icon-volume-increase-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute-5"></span>
		&nbsp;icon-volume-mute-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-mute-6"></span>
		&nbsp;icon-volume-mute-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-low-2"></span>
		&nbsp;icon-volume-low-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-medium-2"></span>
		&nbsp;icon-volume-medium-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-volume-high-2"></span>
		&nbsp;icon-volume-high-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eject"></span>
		&nbsp;icon-eject
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-next"></span>
		&nbsp;icon-next
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-previous"></span>
		&nbsp;icon-previous
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-last"></span>
		&nbsp;icon-last
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-first"></span>
		&nbsp;icon-first
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-forward"></span>
		&nbsp;icon-forward
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-backward"></span>
		&nbsp;icon-backward
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stop"></span>
		&nbsp;icon-stop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pause"></span>
		&nbsp;icon-pause
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-play"></span>
		&nbsp;icon-play
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-forward-2"></span>
		&nbsp;icon-forward-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-backward-2"></span>
		&nbsp;icon-backward-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stop-2"></span>
		&nbsp;icon-stop-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pause-2"></span>
		&nbsp;icon-pause-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-play-2"></span>
		&nbsp;icon-play-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-exit"></span>
		&nbsp;icon-exit
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-exit-2"></span>
		&nbsp;icon-exit-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-enter-3"></span>
		&nbsp;icon-enter-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-exit-3"></span>
		&nbsp;icon-exit-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-enter-4"></span>
		&nbsp;icon-enter-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-exit-4"></span>
		&nbsp;icon-exit-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-enter-5"></span>
		&nbsp;icon-enter-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-plus"></span>
		&nbsp;icon-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-minus"></span>
		&nbsp;icon-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-plus-2"></span>
		&nbsp;icon-plus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-minus-2"></span>
		&nbsp;icon-minus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spell-check"></span>
		&nbsp;icon-spell-check
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark"></span>
		&nbsp;icon-checkmark
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark-2"></span>
		&nbsp;icon-checkmark-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark-3"></span>
		&nbsp;icon-checkmark-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark-4"></span>
		&nbsp;icon-checkmark-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-close"></span>
		&nbsp;icon-close
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-close-2"></span>
		&nbsp;icon-close-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-close-3"></span>
		&nbsp;icon-close-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-close-4"></span>
		&nbsp;icon-close-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-close-5"></span>
		&nbsp;icon-close-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spam"></span>
		&nbsp;icon-spam
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cancel"></span>
		&nbsp;icon-cancel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark-circle"></span>
		&nbsp;icon-checkmark-circle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-checkmark-circle-2"></span>
		&nbsp;icon-checkmark-circle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cancel-circle"></span>
		&nbsp;icon-cancel-circle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cancel-circle-2"></span>
		&nbsp;icon-cancel-circle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-blocked"></span>
		&nbsp;icon-blocked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-info"></span>
		&nbsp;icon-info
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-info-2"></span>
		&nbsp;icon-info-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-minus-circle"></span>
		&nbsp;icon-minus-circle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-minus-circle-2"></span>
		&nbsp;icon-minus-circle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-plus-circle"></span>
		&nbsp;icon-plus-circle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-plus-circle-2"></span>
		&nbsp;icon-plus-circle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-question"></span>
		&nbsp;icon-question
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-question-2"></span>
		&nbsp;icon-question-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-question-3"></span>
		&nbsp;icon-question-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-question-4"></span>
		&nbsp;icon-question-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-question-5"></span>
		&nbsp;icon-question-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-notification"></span>
		&nbsp;icon-notification
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-notification-2"></span>
		&nbsp;icon-notification-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-warning"></span>
		&nbsp;icon-warning
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-warning-2"></span>
		&nbsp;icon-warning-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-resize"></span>
		&nbsp;icon-resize
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-resize-2"></span>
		&nbsp;icon-resize-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-move"></span>
		&nbsp;icon-move
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-play"></span>
		&nbsp;icon-stack-play
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-music"></span>
		&nbsp;icon-stack-music
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack"></span>
		&nbsp;icon-stack
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-user"></span>
		&nbsp;icon-stack-user
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-diamonds"></span>
		&nbsp;icon-stack-diamonds
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-hearts"></span>
		&nbsp;icon-stack-hearts
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-spades"></span>
		&nbsp;icon-stack-spades
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-clubs"></span>
		&nbsp;icon-stack-clubs
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-list"></span>
		&nbsp;icon-stack-list
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-checkmark"></span>
		&nbsp;icon-stack-checkmark
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-cancel"></span>
		&nbsp;icon-stack-cancel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-up"></span>
		&nbsp;icon-stack-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-down"></span>
		&nbsp;icon-stack-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-picture"></span>
		&nbsp;icon-stack-picture
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-star"></span>
		&nbsp;icon-stack-star
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-minus"></span>
		&nbsp;icon-stack-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-plus"></span>
		&nbsp;icon-stack-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-empty"></span>
		&nbsp;icon-stack-empty
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hand"></span>
		&nbsp;icon-hand
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pointer"></span>
		&nbsp;icon-pointer
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-point-left"></span>
		&nbsp;icon-point-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-point-down"></span>
		&nbsp;icon-point-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-point-right"></span>
		&nbsp;icon-point-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-point-up"></span>
		&nbsp;icon-point-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cursor"></span>
		&nbsp;icon-cursor
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cursor-2"></span>
		&nbsp;icon-cursor-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wondering"></span>
		&nbsp;icon-wondering
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wondering-2"></span>
		&nbsp;icon-wondering-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-neutral"></span>
		&nbsp;icon-neutral
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-neutral-2"></span>
		&nbsp;icon-neutral-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-confused"></span>
		&nbsp;icon-confused
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-confused-2"></span>
		&nbsp;icon-confused-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shocked"></span>
		&nbsp;icon-shocked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shocked-2"></span>
		&nbsp;icon-shocked-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-evil"></span>
		&nbsp;icon-evil
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-evil-2"></span>
		&nbsp;icon-evil-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-angry"></span>
		&nbsp;icon-angry
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-angry-2"></span>
		&nbsp;icon-angry-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cool"></span>
		&nbsp;icon-cool
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cool-2"></span>
		&nbsp;icon-cool-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grin"></span>
		&nbsp;icon-grin
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grin-2"></span>
		&nbsp;icon-grin-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wink"></span>
		&nbsp;icon-wink
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wink-2"></span>
		&nbsp;icon-wink-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sad"></span>
		&nbsp;icon-sad
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sad-2"></span>
		&nbsp;icon-sad-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tongue"></span>
		&nbsp;icon-tongue
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tongue-2"></span>
		&nbsp;icon-tongue-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-smiley"></span>
		&nbsp;icon-smiley
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-smiley-2"></span>
		&nbsp;icon-smiley-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-happy"></span>
		&nbsp;icon-happy
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-happy-2"></span>
		&nbsp;icon-happy-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-yin-yang"></span>
		&nbsp;icon-yin-yang
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-peace"></span>
		&nbsp;icon-peace
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-female"></span>
		&nbsp;icon-female
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-woman"></span>
		&nbsp;icon-woman
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-male"></span>
		&nbsp;icon-male
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-man"></span>
		&nbsp;icon-man
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-people"></span>
		&nbsp;icon-people
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up"></span>
		&nbsp;icon-thumbs-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up-2"></span>
		&nbsp;icon-thumbs-up-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up-3"></span>
		&nbsp;icon-thumbs-up-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up-4"></span>
		&nbsp;icon-thumbs-up-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-down"></span>
		&nbsp;icon-thumbs-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-down-2"></span>
		&nbsp;icon-thumbs-down-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up-5"></span>
		&nbsp;icon-thumbs-up-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-thumbs-up-6"></span>
		&nbsp;icon-thumbs-up-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lips"></span>
		&nbsp;icon-lips
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lips-2"></span>
		&nbsp;icon-lips-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-broken"></span>
		&nbsp;icon-heart-broken
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart"></span>
		&nbsp;icon-heart
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-2"></span>
		&nbsp;icon-heart-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-broken-2"></span>
		&nbsp;icon-heart-broken-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-3"></span>
		&nbsp;icon-heart-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-4"></span>
		&nbsp;icon-heart-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-broken-3"></span>
		&nbsp;icon-heart-broken-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-5"></span>
		&nbsp;icon-heart-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-6"></span>
		&nbsp;icon-heart-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-7"></span>
		&nbsp;icon-heart-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-heart-8"></span>
		&nbsp;icon-heart-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star"></span>
		&nbsp;icon-star
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star-2"></span>
		&nbsp;icon-star-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star-3"></span>
		&nbsp;icon-star-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star-4"></span>
		&nbsp;icon-star-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star-5"></span>
		&nbsp;icon-star-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-star-6"></span>
		&nbsp;icon-star-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bed"></span>
		&nbsp;icon-bed
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bed-2"></span>
		&nbsp;icon-bed-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-moon"></span>
		&nbsp;icon-moon
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contrast"></span>
		&nbsp;icon-contrast
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brightness-contrast"></span>
		&nbsp;icon-brightness-contrast
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brightness-low"></span>
		&nbsp;icon-brightness-low
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brightness-medium"></span>
		&nbsp;icon-brightness-medium
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brightness-high"></span>
		&nbsp;icon-brightness-high
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sun"></span>
		&nbsp;icon-sun
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sun-2"></span>
		&nbsp;icon-sun-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-umbrella"></span>
		&nbsp;icon-umbrella
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-fan"></span>
		&nbsp;icon-fan
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-windy"></span>
		&nbsp;icon-windy
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-weather-snow"></span>
		&nbsp;icon-weather-snow
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-weather-rain"></span>
		&nbsp;icon-weather-rain
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-weather-lightning"></span>
		&nbsp;icon-weather-lightning
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-temperature"></span>
		&nbsp;icon-temperature
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-temperature-2"></span>
		&nbsp;icon-temperature-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-snowflake"></span>
		&nbsp;icon-snowflake
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-starburst"></span>
		&nbsp;icon-starburst
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spotlight"></span>
		&nbsp;icon-spotlight
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bookmark"></span>
		&nbsp;icon-bookmark
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bookmarks"></span>
		&nbsp;icon-bookmarks
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bookmark-2"></span>
		&nbsp;icon-bookmark-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bookmark-3"></span>
		&nbsp;icon-bookmark-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye"></span>
		&nbsp;icon-eye
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-2"></span>
		&nbsp;icon-eye-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-3"></span>
		&nbsp;icon-eye-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-4"></span>
		&nbsp;icon-eye-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-5"></span>
		&nbsp;icon-eye-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-blocked"></span>
		&nbsp;icon-eye-blocked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-6"></span>
		&nbsp;icon-eye-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-7"></span>
		&nbsp;icon-eye-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-blocked-2"></span>
		&nbsp;icon-eye-blocked-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eye-8"></span>
		&nbsp;icon-eye-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-attachment"></span>
		&nbsp;icon-attachment
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-attachment-2"></span>
		&nbsp;icon-attachment-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag"></span>
		&nbsp;icon-flag
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag-2"></span>
		&nbsp;icon-flag-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag-3"></span>
		&nbsp;icon-flag-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag-4"></span>
		&nbsp;icon-flag-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag-5"></span>
		&nbsp;icon-flag-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flag-6"></span>
		&nbsp;icon-flag-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-anchor"></span>
		&nbsp;icon-anchor
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link"></span>
		&nbsp;icon-link
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link-2"></span>
		&nbsp;icon-link-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link-3"></span>
		&nbsp;icon-link-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link2"></span>
		&nbsp;icon-link2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link-4"></span>
		&nbsp;icon-link-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link-5"></span>
		&nbsp;icon-link-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-link-6"></span>
		&nbsp;icon-link-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-network"></span>
		&nbsp;icon-network
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-earth"></span>
		&nbsp;icon-earth
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-globe"></span>
		&nbsp;icon-globe
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-globe-2"></span>
		&nbsp;icon-globe-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-globe-3"></span>
		&nbsp;icon-globe-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload"></span>
		&nbsp;icon-upload
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download"></span>
		&nbsp;icon-download
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-2"></span>
		&nbsp;icon-upload-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-2"></span>
		&nbsp;icon-download-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-3"></span>
		&nbsp;icon-upload-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-3"></span>
		&nbsp;icon-download-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-4"></span>
		&nbsp;icon-upload-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-4"></span>
		&nbsp;icon-download-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-5"></span>
		&nbsp;icon-upload-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-5"></span>
		&nbsp;icon-download-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-6"></span>
		&nbsp;icon-upload-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-6"></span>
		&nbsp;icon-download-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cloud-upload"></span>
		&nbsp;icon-cloud-upload
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cloud-download"></span>
		&nbsp;icon-cloud-download
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cloud"></span>
		&nbsp;icon-cloud
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cloud-2"></span>
		&nbsp;icon-cloud-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cloud-3"></span>
		&nbsp;icon-cloud-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-3"></span>
		&nbsp;icon-menu-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-4"></span>
		&nbsp;icon-menu-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-5"></span>
		&nbsp;icon-menu-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-6"></span>
		&nbsp;icon-menu-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-7"></span>
		&nbsp;icon-menu-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-8"></span>
		&nbsp;icon-menu-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-9"></span>
		&nbsp;icon-menu-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-circle-small"></span>
		&nbsp;icon-circle-small
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-10"></span>
		&nbsp;icon-menu-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-menu-11"></span>
		&nbsp;icon-menu-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tree"></span>
		&nbsp;icon-tree
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tree-2"></span>
		&nbsp;icon-tree-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tree-3"></span>
		&nbsp;icon-tree-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid"></span>
		&nbsp;icon-grid
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid-2"></span>
		&nbsp;icon-grid-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid-3"></span>
		&nbsp;icon-grid-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid-4"></span>
		&nbsp;icon-grid-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid-5"></span>
		&nbsp;icon-grid-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-grid-6"></span>
		&nbsp;icon-grid-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-playlist"></span>
		&nbsp;icon-playlist
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-list"></span>
		&nbsp;icon-list
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-list-2"></span>
		&nbsp;icon-list-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-numbered-list"></span>
		&nbsp;icon-numbered-list
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-list-3"></span>
		&nbsp;icon-list-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-list-4"></span>
		&nbsp;icon-list-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-list-5"></span>
		&nbsp;icon-list-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clipboard"></span>
		&nbsp;icon-clipboard
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clipboard-2"></span>
		&nbsp;icon-clipboard-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-signup"></span>
		&nbsp;icon-signup
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clipboard-3"></span>
		&nbsp;icon-clipboard-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clipboard-4"></span>
		&nbsp;icon-clipboard-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-socket"></span>
		&nbsp;icon-socket
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cord"></span>
		&nbsp;icon-cord
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-power-cord"></span>
		&nbsp;icon-power-cord
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-switch"></span>
		&nbsp;icon-switch
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-power"></span>
		&nbsp;icon-power
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-power-2"></span>
		&nbsp;icon-power-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lightning"></span>
		&nbsp;icon-lightning
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bowling"></span>
		&nbsp;icon-bowling
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bowling-2"></span>
		&nbsp;icon-bowling-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bowling-ball"></span>
		&nbsp;icon-bowling-ball
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eight-ball"></span>
		&nbsp;icon-eight-ball
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-racing"></span>
		&nbsp;icon-racing
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hockey"></span>
		&nbsp;icon-hockey
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-golf"></span>
		&nbsp;icon-golf
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-basketball"></span>
		&nbsp;icon-basketball
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-baseball"></span>
		&nbsp;icon-baseball
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-football"></span>
		&nbsp;icon-football
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-soccer"></span>
		&nbsp;icon-soccer
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shield"></span>
		&nbsp;icon-shield
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shield-2"></span>
		&nbsp;icon-shield-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shield-3"></span>
		&nbsp;icon-shield-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-shield-4"></span>
		&nbsp;icon-shield-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gun-ban"></span>
		&nbsp;icon-gun-ban
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gun"></span>
		&nbsp;icon-gun
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-target"></span>
		&nbsp;icon-target
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-target-2"></span>
		&nbsp;icon-target-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-target-3"></span>
		&nbsp;icon-target-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brain"></span>
		&nbsp;icon-brain
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-accessibility"></span>
		&nbsp;icon-accessibility
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-accessibility-2"></span>
		&nbsp;icon-accessibility-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-sun-glasses"></span>
		&nbsp;icon-sun-glasses
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-glasses"></span>
		&nbsp;icon-glasses
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-glasses-2"></span>
		&nbsp;icon-glasses-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-glasses-3"></span>
		&nbsp;icon-glasses-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-puzzle"></span>
		&nbsp;icon-puzzle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-puzzle-2"></span>
		&nbsp;icon-puzzle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-puzzle-3"></span>
		&nbsp;icon-puzzle-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-puzzle-4"></span>
		&nbsp;icon-puzzle-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-package"></span>
		&nbsp;icon-package
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cylinder"></span>
		&nbsp;icon-cylinder
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pyramid"></span>
		&nbsp;icon-pyramid
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pyramid-2"></span>
		&nbsp;icon-pyramid-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cube4"></span>
		&nbsp;icon-cube4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cube"></span>
		&nbsp;icon-cube
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cube-2"></span>
		&nbsp;icon-cube-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cube-3"></span>
		&nbsp;icon-cube-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-boat"></span>
		&nbsp;icon-boat
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-ship"></span>
		&nbsp;icon-ship
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-train"></span>
		&nbsp;icon-train
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-road"></span>
		&nbsp;icon-road
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bike"></span>
		&nbsp;icon-bike
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-truck"></span>
		&nbsp;icon-truck
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bus"></span>
		&nbsp;icon-bus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gas-pump"></span>
		&nbsp;icon-gas-pump
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-car"></span>
		&nbsp;icon-car
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paper-plane"></span>
		&nbsp;icon-paper-plane
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-airplane"></span>
		&nbsp;icon-airplane
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-airplane-2"></span>
		&nbsp;icon-airplane-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-briefcase"></span>
		&nbsp;icon-briefcase
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-briefcase-2"></span>
		&nbsp;icon-briefcase-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-briefcase-3"></span>
		&nbsp;icon-briefcase-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove"></span>
		&nbsp;icon-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-2"></span>
		&nbsp;icon-remove-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-3"></span>
		&nbsp;icon-remove-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-4"></span>
		&nbsp;icon-remove-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-5"></span>
		&nbsp;icon-remove-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-6"></span>
		&nbsp;icon-remove-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-7"></span>
		&nbsp;icon-remove-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-remove-8"></span>
		&nbsp;icon-remove-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lamp"></span>
		&nbsp;icon-lamp
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lamp-2"></span>
		&nbsp;icon-lamp-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lamp-3"></span>
		&nbsp;icon-lamp-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lamp-4"></span>
		&nbsp;icon-lamp-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-skull"></span>
		&nbsp;icon-skull
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-skull-2"></span>
		&nbsp;icon-skull-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-skull-3"></span>
		&nbsp;icon-skull-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dumbbell"></span>
		&nbsp;icon-dumbbell
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-magnet"></span>
		&nbsp;icon-magnet
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-magnet-2"></span>
		&nbsp;icon-magnet-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-magnet-3"></span>
		&nbsp;icon-magnet-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-magnet-4"></span>
		&nbsp;icon-magnet-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-atom"></span>
		&nbsp;icon-atom
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-atom-2"></span>
		&nbsp;icon-atom-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lab"></span>
		&nbsp;icon-lab
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-fire"></span>
		&nbsp;icon-fire
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-fire-2"></span>
		&nbsp;icon-fire-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bomb"></span>
		&nbsp;icon-bomb
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-balance"></span>
		&nbsp;icon-balance
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hammer"></span>
		&nbsp;icon-hammer
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dashboard"></span>
		&nbsp;icon-dashboard
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-meter-fast"></span>
		&nbsp;icon-meter-fast
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-meter-medium"></span>
		&nbsp;icon-meter-medium
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-meter-slow"></span>
		&nbsp;icon-meter-slow
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-meter2"></span>
		&nbsp;icon-meter2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-meter"></span>
		&nbsp;icon-meter
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rocket"></span>
		&nbsp;icon-rocket
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flower"></span>
		&nbsp;icon-flower
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-steps"></span>
		&nbsp;icon-steps
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paw"></span>
		&nbsp;icon-paw
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tree-4"></span>
		&nbsp;icon-tree-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tree-5"></span>
		&nbsp;icon-tree-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-apple-fruit"></span>
		&nbsp;icon-apple-fruit
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-leaf"></span>
		&nbsp;icon-leaf
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-leaf-2"></span>
		&nbsp;icon-leaf-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cup"></span>
		&nbsp;icon-cup
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cup-2"></span>
		&nbsp;icon-cup-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hamburger"></span>
		&nbsp;icon-hamburger
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-food"></span>
		&nbsp;icon-food
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-food-2"></span>
		&nbsp;icon-food-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mug"></span>
		&nbsp;icon-mug
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bottle"></span>
		&nbsp;icon-bottle
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bottle-2"></span>
		&nbsp;icon-bottle-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-glass"></span>
		&nbsp;icon-glass
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-glass-2"></span>
		&nbsp;icon-glass-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-diamond"></span>
		&nbsp;icon-diamond
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-diamond-2"></span>
		&nbsp;icon-diamond-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-trophy-star"></span>
		&nbsp;icon-trophy-star
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-trophy"></span>
		&nbsp;icon-trophy
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-trophy-2"></span>
		&nbsp;icon-trophy-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-crown"></span>
		&nbsp;icon-crown
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-medal"></span>
		&nbsp;icon-medal
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-medal-2"></span>
		&nbsp;icon-medal-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-medal-3"></span>
		&nbsp;icon-medal-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-medal-4"></span>
		&nbsp;icon-medal-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-medal-5"></span>
		&nbsp;icon-medal-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-podium"></span>
		&nbsp;icon-podium
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rating"></span>
		&nbsp;icon-rating
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rating-2"></span>
		&nbsp;icon-rating-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rating-3"></span>
		&nbsp;icon-rating-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-balloon"></span>
		&nbsp;icon-balloon
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gift"></span>
		&nbsp;icon-gift
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gift-2"></span>
		&nbsp;icon-gift-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cake"></span>
		&nbsp;icon-cake
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-ladder"></span>
		&nbsp;icon-ladder
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stairs"></span>
		&nbsp;icon-stairs
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stairs-2"></span>
		&nbsp;icon-stairs-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-chart"></span>
		&nbsp;icon-chart
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stairs-down"></span>
		&nbsp;icon-stairs-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stairs-down-2"></span>
		&nbsp;icon-stairs-down-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stats-down"></span>
		&nbsp;icon-stats-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stats-up"></span>
		&nbsp;icon-stats-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars"></span>
		&nbsp;icon-bars
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars-2"></span>
		&nbsp;icon-bars-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars-3"></span>
		&nbsp;icon-bars-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars-4"></span>
		&nbsp;icon-bars-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars-5"></span>
		&nbsp;icon-bars-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bars-6"></span>
		&nbsp;icon-bars-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stats"></span>
		&nbsp;icon-stats
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stats-2"></span>
		&nbsp;icon-stats-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stats-3"></span>
		&nbsp;icon-stats-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie"></span>
		&nbsp;icon-pie
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-2"></span>
		&nbsp;icon-pie-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-3"></span>
		&nbsp;icon-pie-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-4"></span>
		&nbsp;icon-pie-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-5"></span>
		&nbsp;icon-pie-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-6"></span>
		&nbsp;icon-pie-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pie-7"></span>
		&nbsp;icon-pie-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cone"></span>
		&nbsp;icon-cone
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-construction"></span>
		&nbsp;icon-construction
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-inject"></span>
		&nbsp;icon-inject
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-inject-2"></span>
		&nbsp;icon-inject-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bug"></span>
		&nbsp;icon-bug
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bug-2"></span>
		&nbsp;icon-bug-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-patch"></span>
		&nbsp;icon-patch
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-aid"></span>
		&nbsp;icon-aid
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-health"></span>
		&nbsp;icon-health
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wand"></span>
		&nbsp;icon-wand
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wand-2"></span>
		&nbsp;icon-wand-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screwdriver"></span>
		&nbsp;icon-screwdriver
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screwdriver-2"></span>
		&nbsp;icon-screwdriver-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tools"></span>
		&nbsp;icon-tools
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hammer-2"></span>
		&nbsp;icon-hammer-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-factory"></span>
		&nbsp;icon-factory
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog"></span>
		&nbsp;icon-cog
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-2"></span>
		&nbsp;icon-cog-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-3"></span>
		&nbsp;icon-cog-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-4"></span>
		&nbsp;icon-cog-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-5"></span>
		&nbsp;icon-cog-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-6"></span>
		&nbsp;icon-cog-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cogs"></span>
		&nbsp;icon-cogs
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cog-7"></span>
		&nbsp;icon-cog-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-equalizer"></span>
		&nbsp;icon-equalizer
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-equalizer-2"></span>
		&nbsp;icon-equalizer-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-equalizer-3"></span>
		&nbsp;icon-equalizer-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-settings"></span>
		&nbsp;icon-settings
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wrench"></span>
		&nbsp;icon-wrench
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wrench-2"></span>
		&nbsp;icon-wrench-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wrench-3"></span>
		&nbsp;icon-wrench-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-wrench-4"></span>
		&nbsp;icon-wrench-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-unlocked"></span>
		&nbsp;icon-unlocked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lock"></span>
		&nbsp;icon-lock
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-unlocked-2"></span>
		&nbsp;icon-unlocked-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lock-2"></span>
		&nbsp;icon-lock-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lock-3"></span>
		&nbsp;icon-lock-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lock-4"></span>
		&nbsp;icon-lock-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-lock-5"></span>
		&nbsp;icon-lock-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-keyhole"></span>
		&nbsp;icon-keyhole
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key"></span>
		&nbsp;icon-key
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-2"></span>
		&nbsp;icon-key-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-3"></span>
		&nbsp;icon-key-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-4"></span>
		&nbsp;icon-key-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-key-5"></span>
		&nbsp;icon-key-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contract"></span>
		&nbsp;icon-contract
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-expand"></span>
		&nbsp;icon-expand
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-fullscreen"></span>
		&nbsp;icon-fullscreen
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scale-down"></span>
		&nbsp;icon-scale-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scale-up"></span>
		&nbsp;icon-scale-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contract-2"></span>
		&nbsp;icon-contract-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-expand-2"></span>
		&nbsp;icon-expand-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scale-down-2"></span>
		&nbsp;icon-scale-down-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-scale-up-2"></span>
		&nbsp;icon-scale-up-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contract-3"></span>
		&nbsp;icon-contract-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-expand-3"></span>
		&nbsp;icon-expand-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-search"></span>
		&nbsp;icon-search
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-zoom-out"></span>
		&nbsp;icon-zoom-out
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-zoom-in"></span>
		&nbsp;icon-zoom-in
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-search-2"></span>
		&nbsp;icon-search-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-search-3"></span>
		&nbsp;icon-search-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-zoom-out-2"></span>
		&nbsp;icon-zoom-out-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-zoom-in-2"></span>
		&nbsp;icon-zoom-in-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-search-4"></span>
		&nbsp;icon-search-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-search-5"></span>
		&nbsp;icon-search-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-binoculars"></span>
		&nbsp;icon-binoculars
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-binoculars-2"></span>
		&nbsp;icon-binoculars-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-microscope"></span>
		&nbsp;icon-microscope
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner"></span>
		&nbsp;icon-spinner
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-2"></span>
		&nbsp;icon-spinner-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-3"></span>
		&nbsp;icon-spinner-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-4"></span>
		&nbsp;icon-spinner-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-5"></span>
		&nbsp;icon-spinner-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-6"></span>
		&nbsp;icon-spinner-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-7"></span>
		&nbsp;icon-spinner-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-8"></span>
		&nbsp;icon-spinner-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-9"></span>
		&nbsp;icon-spinner-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-10"></span>
		&nbsp;icon-spinner-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-11"></span>
		&nbsp;icon-spinner-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spinner-12"></span>
		&nbsp;icon-spinner-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-busy"></span>
		&nbsp;icon-busy
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-busy-2"></span>
		&nbsp;icon-busy-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-busy-3"></span>
		&nbsp;icon-busy-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-busy-4"></span>
		&nbsp;icon-busy-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quotes-right"></span>
		&nbsp;icon-quotes-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quotes-right-2"></span>
		&nbsp;icon-quotes-right-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quotes-right-3"></span>
		&nbsp;icon-quotes-right-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-quotes-left"></span>
		&nbsp;icon-quotes-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-hanger"></span>
		&nbsp;icon-hanger
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tshirt"></span>
		&nbsp;icon-tshirt
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-vcard"></span>
		&nbsp;icon-vcard
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-users"></span>
		&nbsp;icon-users
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user"></span>
		&nbsp;icon-user
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-2"></span>
		&nbsp;icon-user-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-users-2"></span>
		&nbsp;icon-users-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-3"></span>
		&nbsp;icon-user-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-4"></span>
		&nbsp;icon-user-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-5"></span>
		&nbsp;icon-user-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-6"></span>
		&nbsp;icon-user-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-block"></span>
		&nbsp;icon-user-block
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-cancel"></span>
		&nbsp;icon-user-cancel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-minus"></span>
		&nbsp;icon-user-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-plus"></span>
		&nbsp;icon-user-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-users-3"></span>
		&nbsp;icon-users-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-7"></span>
		&nbsp;icon-user-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-users-4"></span>
		&nbsp;icon-users-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-block-2"></span>
		&nbsp;icon-user-block-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-cancel-2"></span>
		&nbsp;icon-user-cancel-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-minus-2"></span>
		&nbsp;icon-user-minus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-minus-3"></span>
		&nbsp;icon-user-minus-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-plus-2"></span>
		&nbsp;icon-user-plus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-plus-3"></span>
		&nbsp;icon-user-plus-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-users-5"></span>
		&nbsp;icon-users-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-user-8"></span>
		&nbsp;icon-user-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-forward"></span>
		&nbsp;icon-bubble-forward
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-reply"></span>
		&nbsp;icon-bubble-reply
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-forward-2"></span>
		&nbsp;icon-bubble-forward-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-replu"></span>
		&nbsp;icon-bubble-replu
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-last"></span>
		&nbsp;icon-bubble-last
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-first"></span>
		&nbsp;icon-bubble-first
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-down"></span>
		&nbsp;icon-bubble-down
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-up"></span>
		&nbsp;icon-bubble-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-right"></span>
		&nbsp;icon-bubble-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-left"></span>
		&nbsp;icon-bubble-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-trash"></span>
		&nbsp;icon-bubble-trash
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-notification"></span>
		&nbsp;icon-bubble-notification
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-minus"></span>
		&nbsp;icon-bubble-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-plus"></span>
		&nbsp;icon-bubble-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-cancel"></span>
		&nbsp;icon-bubble-cancel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-paperclip"></span>
		&nbsp;icon-bubble-paperclip
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-heart"></span>
		&nbsp;icon-bubble-heart
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-star"></span>
		&nbsp;icon-bubble-star
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-locked"></span>
		&nbsp;icon-bubble-locked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-link"></span>
		&nbsp;icon-bubble-link
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-video-chat"></span>
		&nbsp;icon-bubble-video-chat
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-check"></span>
		&nbsp;icon-bubble-check
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-user"></span>
		&nbsp;icon-bubble-user
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-quote"></span>
		&nbsp;icon-bubble-quote
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-blocked"></span>
		&nbsp;icon-bubble-blocked
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles"></span>
		&nbsp;icon-bubbles
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-2"></span>
		&nbsp;icon-bubbles-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble"></span>
		&nbsp;icon-bubble
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-dots"></span>
		&nbsp;icon-bubble-dots
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-2"></span>
		&nbsp;icon-bubble-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-3"></span>
		&nbsp;icon-bubble-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-dots-2"></span>
		&nbsp;icon-bubble-dots-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-4"></span>
		&nbsp;icon-bubble-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-3"></span>
		&nbsp;icon-bubbles-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-5"></span>
		&nbsp;icon-bubble-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-4"></span>
		&nbsp;icon-bubbles-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-6"></span>
		&nbsp;icon-bubble-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-7"></span>
		&nbsp;icon-bubble-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-8"></span>
		&nbsp;icon-bubble-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-dots-3"></span>
		&nbsp;icon-bubble-dots-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-9"></span>
		&nbsp;icon-bubble-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-10"></span>
		&nbsp;icon-bubble-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-dots-4"></span>
		&nbsp;icon-bubble-dots-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-11"></span>
		&nbsp;icon-bubble-11
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-5"></span>
		&nbsp;icon-bubbles-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-6"></span>
		&nbsp;icon-bubbles-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-notification-2"></span>
		&nbsp;icon-bubble-notification-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-7"></span>
		&nbsp;icon-bubbles-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-8"></span>
		&nbsp;icon-bubbles-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-12"></span>
		&nbsp;icon-bubble-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-9"></span>
		&nbsp;icon-bubbles-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubbles-10"></span>
		&nbsp;icon-bubbles-10
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bubble-13"></span>
		&nbsp;icon-bubble-13
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-reply"></span>
		&nbsp;icon-reply
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-reply-2"></span>
		&nbsp;icon-reply-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-forward-3"></span>
		&nbsp;icon-forward-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-redo"></span>
		&nbsp;icon-redo
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-undo"></span>
		&nbsp;icon-undo
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-bottom"></span>
		&nbsp;icon-align-bottom
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-center-vertical"></span>
		&nbsp;icon-align-center-vertical
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-top"></span>
		&nbsp;icon-align-top
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-right"></span>
		&nbsp;icon-align-right
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-center-horizontal"></span>
		&nbsp;icon-align-center-horizontal
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-align-left"></span>
		&nbsp;icon-align-left
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-exclude"></span>
		&nbsp;icon-exclude
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-interset"></span>
		&nbsp;icon-interset
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-subtract"></span>
		&nbsp;icon-subtract
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-unite"></span>
		&nbsp;icon-unite
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flip"></span>
		&nbsp;icon-flip
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-flip-2"></span>
		&nbsp;icon-flip-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rotate"></span>
		&nbsp;icon-rotate
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rotate-2"></span>
		&nbsp;icon-rotate-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-redo-2"></span>
		&nbsp;icon-redo-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-undo-2"></span>
		&nbsp;icon-undo-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-database"></span>
		&nbsp;icon-database
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-database-2"></span>
		&nbsp;icon-database-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-database-3"></span>
		&nbsp;icon-database-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-storage"></span>
		&nbsp;icon-storage
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-storage-2"></span>
		&nbsp;icon-storage-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cd"></span>
		&nbsp;icon-cd
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-disk"></span>
		&nbsp;icon-disk
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-upload-7"></span>
		&nbsp;icon-upload-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-download-7"></span>
		&nbsp;icon-download-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-box-remove"></span>
		&nbsp;icon-box-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-box-add"></span>
		&nbsp;icon-box-add
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-box"></span>
		&nbsp;icon-box
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-drawer"></span>
		&nbsp;icon-drawer
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-drawer-2"></span>
		&nbsp;icon-drawer-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-drawer-3"></span>
		&nbsp;icon-drawer-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-archive"></span>
		&nbsp;icon-archive
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cabinet"></span>
		&nbsp;icon-cabinet
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tv"></span>
		&nbsp;icon-tv
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mobile"></span>
		&nbsp;icon-mobile
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tablet"></span>
		&nbsp;icon-tablet
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mobile-2"></span>
		&nbsp;icon-mobile-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mobile-3"></span>
		&nbsp;icon-mobile-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-laptop"></span>
		&nbsp;icon-laptop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screen"></span>
		&nbsp;icon-screen
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screen-2"></span>
		&nbsp;icon-screen-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screen-3"></span>
		&nbsp;icon-screen-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-screen-4"></span>
		&nbsp;icon-screen-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-keyboard"></span>
		&nbsp;icon-keyboard
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-keyboard-2"></span>
		&nbsp;icon-keyboard-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mouse"></span>
		&nbsp;icon-mouse
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mouse-2"></span>
		&nbsp;icon-mouse-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mouse-3"></span>
		&nbsp;icon-mouse-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mouse-4"></span>
		&nbsp;icon-mouse-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-print"></span>
		&nbsp;icon-print
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-print-2"></span>
		&nbsp;icon-print-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-print-3"></span>
		&nbsp;icon-print-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calendar"></span>
		&nbsp;icon-calendar
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calendar-2"></span>
		&nbsp;icon-calendar-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calendar-3"></span>
		&nbsp;icon-calendar-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calendar-4"></span>
		&nbsp;icon-calendar-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calendar-5"></span>
		&nbsp;icon-calendar-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stopwatch"></span>
		&nbsp;icon-stopwatch
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm-cancel"></span>
		&nbsp;icon-alarm-cancel
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm-check"></span>
		&nbsp;icon-alarm-check
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm-minus"></span>
		&nbsp;icon-alarm-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm-plus"></span>
		&nbsp;icon-alarm-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bell"></span>
		&nbsp;icon-bell
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bell-2"></span>
		&nbsp;icon-bell-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm"></span>
		&nbsp;icon-alarm
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-alarm-2"></span>
		&nbsp;icon-alarm-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock"></span>
		&nbsp;icon-clock
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-2"></span>
		&nbsp;icon-clock-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-3"></span>
		&nbsp;icon-clock-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-watch"></span>
		&nbsp;icon-watch
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-4"></span>
		&nbsp;icon-clock-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-5"></span>
		&nbsp;icon-clock-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-6"></span>
		&nbsp;icon-clock-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clock-7"></span>
		&nbsp;icon-clock-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-history"></span>
		&nbsp;icon-history
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-history-2"></span>
		&nbsp;icon-history-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-direction"></span>
		&nbsp;icon-direction
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-map"></span>
		&nbsp;icon-map
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-map-2"></span>
		&nbsp;icon-map-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-map-3"></span>
		&nbsp;icon-map-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-map-4"></span>
		&nbsp;icon-map-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-compass"></span>
		&nbsp;icon-compass
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-compass-2"></span>
		&nbsp;icon-compass-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location"></span>
		&nbsp;icon-location
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-2"></span>
		&nbsp;icon-location-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-3"></span>
		&nbsp;icon-location-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-4"></span>
		&nbsp;icon-location-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-5"></span>
		&nbsp;icon-location-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-6"></span>
		&nbsp;icon-location-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-location-7"></span>
		&nbsp;icon-location-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pushpin"></span>
		&nbsp;icon-pushpin
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-envelop"></span>
		&nbsp;icon-envelop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-envelop-opened"></span>
		&nbsp;icon-envelop-opened
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mail-send"></span>
		&nbsp;icon-mail-send
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-envelop-2"></span>
		&nbsp;icon-envelop-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-envelop-3"></span>
		&nbsp;icon-envelop-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-notebook"></span>
		&nbsp;icon-notebook
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-address-book"></span>
		&nbsp;icon-address-book
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-address-book-2"></span>
		&nbsp;icon-address-book-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-hang-up"></span>
		&nbsp;icon-phone-hang-up
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-hang-up-2"></span>
		&nbsp;icon-phone-hang-up-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone"></span>
		&nbsp;icon-phone
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-2"></span>
		&nbsp;icon-phone-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-call-outgoing"></span>
		&nbsp;icon-call-outgoing
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-call-incoming"></span>
		&nbsp;icon-call-incoming
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contact-remove"></span>
		&nbsp;icon-contact-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contact-add"></span>
		&nbsp;icon-contact-add
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contact-remove-2"></span>
		&nbsp;icon-contact-remove-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-contact-add-2"></span>
		&nbsp;icon-contact-add-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-3"></span>
		&nbsp;icon-phone-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-4"></span>
		&nbsp;icon-phone-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-5"></span>
		&nbsp;icon-phone-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-phone-6"></span>
		&nbsp;icon-phone-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-support"></span>
		&nbsp;icon-support
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calculate"></span>
		&nbsp;icon-calculate
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-calculate-2"></span>
		&nbsp;icon-calculate-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-credit"></span>
		&nbsp;icon-credit
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-credit-2"></span>
		&nbsp;icon-credit-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-coins"></span>
		&nbsp;icon-coins
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-coin"></span>
		&nbsp;icon-coin
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bag"></span>
		&nbsp;icon-bag
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bag-2"></span>
		&nbsp;icon-bag-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bag-3"></span>
		&nbsp;icon-bag-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-basket"></span>
		&nbsp;icon-basket
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-basket-2"></span>
		&nbsp;icon-basket-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-remove"></span>
		&nbsp;icon-cart-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-checkout"></span>
		&nbsp;icon-cart-checkout
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-remove-2"></span>
		&nbsp;icon-cart-remove-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-add"></span>
		&nbsp;icon-cart-add
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-minus"></span>
		&nbsp;icon-cart-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-plus"></span>
		&nbsp;icon-cart-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart"></span>
		&nbsp;icon-cart
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-2"></span>
		&nbsp;icon-cart-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-3"></span>
		&nbsp;icon-cart-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-4"></span>
		&nbsp;icon-cart-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-5"></span>
		&nbsp;icon-cart-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-6"></span>
		&nbsp;icon-cart-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cart-7"></span>
		&nbsp;icon-cart-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-ticket"></span>
		&nbsp;icon-ticket
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-qrcode"></span>
		&nbsp;icon-qrcode
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-barcode"></span>
		&nbsp;icon-barcode
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-barcode-2"></span>
		&nbsp;icon-barcode-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag"></span>
		&nbsp;icon-tag
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tags"></span>
		&nbsp;icon-tags
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tags-2"></span>
		&nbsp;icon-tags-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-2"></span>
		&nbsp;icon-tag-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-3"></span>
		&nbsp;icon-tag-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-4"></span>
		&nbsp;icon-tag-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-5"></span>
		&nbsp;icon-tag-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-6"></span>
		&nbsp;icon-tag-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-7"></span>
		&nbsp;icon-tag-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-tag-8"></span>
		&nbsp;icon-tag-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-cc"></span>
		&nbsp;icon-cc
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-certificate"></span>
		&nbsp;icon-certificate
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-open"></span>
		&nbsp;icon-folder-open
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder"></span>
		&nbsp;icon-folder
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-open-2"></span>
		&nbsp;icon-folder-open-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-2"></span>
		&nbsp;icon-folder-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-upload"></span>
		&nbsp;icon-folder-upload
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-download"></span>
		&nbsp;icon-folder-download
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-upload-2"></span>
		&nbsp;icon-folder-upload-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-download-2"></span>
		&nbsp;icon-folder-download-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-remove"></span>
		&nbsp;icon-folder-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-plus"></span>
		&nbsp;icon-folder-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-minus"></span>
		&nbsp;icon-folder-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-plus-2"></span>
		&nbsp;icon-folder-plus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-3"></span>
		&nbsp;icon-folder-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-4"></span>
		&nbsp;icon-folder-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-open-3"></span>
		&nbsp;icon-folder-open-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-5"></span>
		&nbsp;icon-folder-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-remove-2"></span>
		&nbsp;icon-folder-remove-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder8"></span>
		&nbsp;icon-folder8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-minus-2"></span>
		&nbsp;icon-folder-minus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-minus-3"></span>
		&nbsp;icon-folder-minus-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-plus-3"></span>
		&nbsp;icon-folder-plus-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-plus-4"></span>
		&nbsp;icon-folder-plus-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-upload-3"></span>
		&nbsp;icon-folder-upload-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-download-3"></span>
		&nbsp;icon-folder-download-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-folder-6"></span>
		&nbsp;icon-folder-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-2"></span>
		&nbsp;icon-stack-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-3"></span>
		&nbsp;icon-stack-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-stack-4"></span>
		&nbsp;icon-stack-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paste"></span>
		&nbsp;icon-paste
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paste-2"></span>
		&nbsp;icon-paste-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paste-3"></span>
		&nbsp;icon-paste-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-copy"></span>
		&nbsp;icon-copy
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-copy-2"></span>
		&nbsp;icon-copy-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-copy-3"></span>
		&nbsp;icon-copy-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-copy-4"></span>
		&nbsp;icon-copy-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file"></span>
		&nbsp;icon-file
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-remove"></span>
		&nbsp;icon-file-remove
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-check"></span>
		&nbsp;icon-file-check
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-upload"></span>
		&nbsp;icon-file-upload
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-download"></span>
		&nbsp;icon-file-download
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-minus"></span>
		&nbsp;icon-file-minus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-plus"></span>
		&nbsp;icon-file-plus
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-2"></span>
		&nbsp;icon-file-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-3"></span>
		&nbsp;icon-file-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-remove-2"></span>
		&nbsp;icon-file-remove-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-check-2"></span>
		&nbsp;icon-file-check-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-upload-2"></span>
		&nbsp;icon-file-upload-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-download-2"></span>
		&nbsp;icon-file-download-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-minus-2"></span>
		&nbsp;icon-file-minus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-plus-2"></span>
		&nbsp;icon-file-plus-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-files"></span>
		&nbsp;icon-files
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-4"></span>
		&nbsp;icon-file-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-5"></span>
		&nbsp;icon-file-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-6"></span>
		&nbsp;icon-file-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-7"></span>
		&nbsp;icon-file-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-8"></span>
		&nbsp;icon-file-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-profile"></span>
		&nbsp;icon-profile
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-file-9"></span>
		&nbsp;icon-file-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-graduation"></span>
		&nbsp;icon-graduation
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-library"></span>
		&nbsp;icon-library
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-library-2"></span>
		&nbsp;icon-library-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-reading"></span>
		&nbsp;icon-reading
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-books"></span>
		&nbsp;icon-books
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-book"></span>
		&nbsp;icon-book
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-book-2"></span>
		&nbsp;icon-book-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mic"></span>
		&nbsp;icon-mic
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mic-2"></span>
		&nbsp;icon-mic-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mic-3"></span>
		&nbsp;icon-mic-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mic-4"></span>
		&nbsp;icon-mic-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-mic-5"></span>
		&nbsp;icon-mic-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-podcast"></span>
		&nbsp;icon-podcast
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-podcast-2"></span>
		&nbsp;icon-podcast-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-radio"></span>
		&nbsp;icon-radio
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-connection"></span>
		&nbsp;icon-connection
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-feed-4"></span>
		&nbsp;icon-feed-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-radio-2"></span>
		&nbsp;icon-radio-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-podcast-3"></span>
		&nbsp;icon-podcast-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-connection-2"></span>
		&nbsp;icon-connection-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-connection-3"></span>
		&nbsp;icon-connection-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-new"></span>
		&nbsp;icon-new
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-megaphone"></span>
		&nbsp;icon-megaphone
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bullhorn"></span>
		&nbsp;icon-bullhorn
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-chess"></span>
		&nbsp;icon-chess
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pawn"></span>
		&nbsp;icon-pawn
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-knight"></span>
		&nbsp;icon-knight
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-bishop"></span>
		&nbsp;icon-bishop
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-rock"></span>
		&nbsp;icon-rock
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-queen"></span>
		&nbsp;icon-queen
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-king"></span>
		&nbsp;icon-king
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-diamonds"></span>
		&nbsp;icon-diamonds
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-clubs"></span>
		&nbsp;icon-clubs
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-spades"></span>
		&nbsp;icon-spades
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pacman"></span>
		&nbsp;icon-pacman
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gamepad"></span>
		&nbsp;icon-gamepad
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gamepad-2"></span>
		&nbsp;icon-gamepad-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-gamepad-3"></span>
		&nbsp;icon-gamepad-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-dice"></span>
		&nbsp;icon-dice
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera"></span>
		&nbsp;icon-camera
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-2"></span>
		&nbsp;icon-camera-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-3"></span>
		&nbsp;icon-camera-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-4"></span>
		&nbsp;icon-camera-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-5"></span>
		&nbsp;icon-camera-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-film"></span>
		&nbsp;icon-film
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-film-2"></span>
		&nbsp;icon-film-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-film-3"></span>
		&nbsp;icon-film-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-film-4"></span>
		&nbsp;icon-film-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-movie"></span>
		&nbsp;icon-movie
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-movie-2"></span>
		&nbsp;icon-movie-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-movie-3"></span>
		&nbsp;icon-movie-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-play-3"></span>
		&nbsp;icon-play-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-play-4"></span>
		&nbsp;icon-play-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-headphones"></span>
		&nbsp;icon-headphones
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-headphones-2"></span>
		&nbsp;icon-headphones-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-guitar"></span>
		&nbsp;icon-guitar
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-piano"></span>
		&nbsp;icon-piano
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music"></span>
		&nbsp;icon-music
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music-2"></span>
		&nbsp;icon-music-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music-3"></span>
		&nbsp;icon-music-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music-4"></span>
		&nbsp;icon-music-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music-5"></span>
		&nbsp;icon-music-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-music-6"></span>
		&nbsp;icon-music-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-6"></span>
		&nbsp;icon-camera-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-7"></span>
		&nbsp;icon-camera-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-8"></span>
		&nbsp;icon-camera-8
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-camera-9"></span>
		&nbsp;icon-camera-9
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image"></span>
		&nbsp;icon-image
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-images"></span>
		&nbsp;icon-images
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-2"></span>
		&nbsp;icon-image-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-3"></span>
		&nbsp;icon-image-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-4"></span>
		&nbsp;icon-image-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-images-2"></span>
		&nbsp;icon-images-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-5"></span>
		&nbsp;icon-image-5
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-6"></span>
		&nbsp;icon-image-6
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-image-7"></span>
		&nbsp;icon-image-7
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paint-format"></span>
		&nbsp;icon-paint-format
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-paint-format-2"></span>
		&nbsp;icon-paint-format-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-droplet"></span>
		&nbsp;icon-droplet
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-droplet-2"></span>
		&nbsp;icon-droplet-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-droplet-3"></span>
		&nbsp;icon-droplet-3
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-droplet-4"></span>
		&nbsp;icon-droplet-4
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eyedropper"></span>
		&nbsp;icon-eyedropper
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-eyedropper-2"></span>
		&nbsp;icon-eyedropper-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-palette"></span>
		&nbsp;icon-palette
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-palette-2"></span>
		&nbsp;icon-palette-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-brush"></span>
		&nbsp;icon-brush
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-blog"></span>
		&nbsp;icon-blog
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-blog-2"></span>
		&nbsp;icon-blog-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-marker"></span>
		&nbsp;icon-marker
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-home-12"></span>
		&nbsp;icon-home-12
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-marker-2"></span>
		&nbsp;icon-marker-2
	</span>
	<span class="box1">
		<span aria-hidden="true" class="icon-pen-5"></span>
		&nbsp;icon-pen-5
	</span>
	</section>
	
	</div>
    
    <script>
        document.getElementById("glyphs").addEventListener("click", function (e) {
            var target = e.target;
            if (target.tagName === "INPUT") {
                target.select();
            }
        });

        $(".mtm span").click(function () {
            alert($(this).find('span').attr('class'));
        });
	</script>
</asp:Content>

<asp:Content ID="PageTitle" ContentPlaceHolderID="PlaceHolderPageTitle" runat="server">
Application Page
</asp:Content>

<asp:Content ID="PageTitleInTitleArea" ContentPlaceHolderID="PlaceHolderPageTitleInTitleArea" runat="server" >
My Application Page
</asp:Content>
