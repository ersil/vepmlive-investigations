﻿<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.Web.CommandUI, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Register Tagprefix="SharePoint" Namespace="Microsoft.SharePoint.WebControls" Assembly="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="SocialStream.ascx.cs" Inherits="EPMLiveWebParts.CONTROLTEMPLATES.SocialStream" %>

<SharePoint:ScriptBlock runat="server">
    (function() {
        'use strict';
        
        window.epmLive = window.epmLive || {};
        window.epmLive.currentUserTimeZone = <%= CurrentUserTimeZone %>;
        window.epmLive.currentUserDisplayName = '<%= CurrentUserDisplayName %>';
        window.epmLive.currentUserAvatar = '<%= CurrentUserAvatar %>';
    })();
</SharePoint:ScriptBlock>

<div id="epm-social-stream">
    <div id="epm-se-status-update-box" class="epm-se-comment-box">
        <div class="epm-se-comment-input" contenteditable="true"></div>
        <button id="epm-se-comment-post-<%= SEID %>">Post</button>
    </div>
    <ul id="epm-se-threads"></ul>
    <div id="epm-se-no-activity">Get to work! Once you start working in the system, items will appear here in the stream.</div>
    <div id="epm-se-pagination"><span>Loading more...</span></div>
</div>

<script id="epm-se-thread-template" type="text/x-handlebars-template">
    <li id="epm-se-thread-{{id}}" class="epm-se-thread clearfix">
        <div class="epm-se-thread-header clearfix">
            {{> user-avatar}}{{> thread-info}}{{> object-info}}
        </div>
        <ul class="epm-se-activities"></ul>
        <ul class="epm-se-older-activities"></ul>
        {{#if hasMoreActivities}}
            <div class="epm-show-older" data-kind="activities" data-threadId="{{id}}" data-offset="{{earliestActivityTime}}">show older activities</div>
        {{/if}}
        <div class="epm-se-comments {{commentsHidden}}">
            {{#if hasMoreComments}}
                <div class="epm-show-older" data-kind="comments" data-threadId="{{id}}" data-offset="{{earliestCommentTime}}">show older comments</div>
            {{/if}}
            <ul class="epm-se-older-comments"></ul>
            <ul class="epm-se-comments"></ul>
            <ul class="epm-se-latest-comments"></ul>
        </div>
        <div class="epm-se-comment-box" data-threadId="{{id}}">{{> comment-box}}</div>
    </li>
</script>

<script id="epm-se-activity-template" type="text/x-handlebars-template">
    <li id="epm-se-activity-{{id}}" class="epm-se-activity clearfix">
        {{> activity-icon}}{{> user-info}}{{> activity-info}}{{> activity-time}}
    </li>

</script>

<script id="epm-se-comment-template" type="text/x-handlebars-template">
    <li id="epm-se-comment-{{id}}" class="epm-se-comment">
        <div class="epm-se-user">{{> user-avatar}}</div>
        <div class="epm-se-comment-details">
            <div class="epm-se-comment-header">{{> user-info}}{{> activity-time}}</div>
            <div class="epm-se-comment-text">{{{text}}}</div>
        </div>
    </li>
</script>

<script id="_epm-se-user-avatar-template" type="text/x-handlebars-template">
    <div class="epm-se-user-avatar">
        {{#if user.picture}}
            <img src="{{user.picture}}" />
        {{/if}}
    </div>
</script>

<script id="_epm-se-thread-info-template" type="text/x-handlebars-template">
    <div class="epm-se-thread-info clearfix">{{> thread-icon}}{{> thread-title}}</div>
</script>

<script id="_epm-se-thread-icon-template" type="text/x-handlebars-template">
    <span class="epm-se-thread-icon {{icon}}"></span>
</script>

<script id="_epm-se-thread-title-template" type="text/x-handlebars-template">
    <h2>
        {{#if url}}
            <a href="{{url}}" class="epm-se-link" target="_blank" data-kind="{{kind}}">{{title}}</a>
        {{else}}
            {{title}}
        {{/if}}
    </h2>
</script>

<script id="_epm-se-activity-icon-template" type="text/x-handlebars-template">
    <span class="epm-se-activity-icon {{icon}}"></span>
</script>

<script id="_epm-se-user-info-template" type="text/x-handlebars-template">
    <span class="epm-se-user-info">
        <a href="{{user.profileUrl}}" target="_blank" class="epm-se-link">{{user.friendlyName}}</a>
    </span>
</script>

<script id="_epm-se-activity-info-template" type="text/x-handlebars-template">
    <span class="epm-se-activity-info">{{action}}</span>
</script>

<script id="_epm-se-activity-time-template" type="text/x-handlebars-template">
    <span class="epm-se-activity-time epm-se-has-tooltip" title="{{longTime}}" data-toggle="tooltip" data-placement="top" data-delay='{"show":500, "hide":100}'>{{friendlyTime}}</span>
</script>

<script id="_epm-se-object-info-template" type="text/x-handlebars-template">
    <span class="epm-se-object-info">
        {{#if web}}
            <span class="epm-se-workspace"><a href="{{web.url}}" target="_blank">{{web.title}}</a></span>
        {{/if}}
        {{#if list}}
            <a href="{{list.url}}" target="_blank" class="epm-se-link">{{list.name}}</a>
        {{/if}}
    </span>
</script>

<script id="_epm-se-comment-box-template" type="text/x-handlebars-template">
    <div class="epm-se-comment-input" contenteditable="true"></div>
    <button id="epm-se-comment-post-{{id}}">Post</button>
</script>