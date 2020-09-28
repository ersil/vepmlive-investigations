﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EPMLiveCore.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("EPMLiveCore.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to DECLARE @creatoralter varchar(10)
        ///IF NOT EXISTS (SELECT routine_name FROM INFORMATION_SCHEMA.routines WHERE routine_name = &apos;spGetWebs&apos;)
        ///BEGIN
        ///    PRINT &apos;Creating Stored Procedure spGetWebs&apos;
        ///    SET @creatoralter = &apos;CREATE&apos;
        ///END
        ///ELSE
        ///BEGIN
        ///    PRINT &apos;Updating Stored Procedure spGetWebs&apos;
        ///    SET @creatoralter = &apos;ALTER&apos;
        ///END
        ///EXEC(@creatoralter + &apos; PROCEDURE [dbo].[spGetWebs]
        ///      -- Add the parameters for the stored procedure here
        ///      @UserId AS INT,
        ///      @SiteId AS UNIQUEIDENTIFIER
        ///AS
        ///BEGIN
        ///      -- SET NOC [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CheckReqSP
        {
            get
            {
                return ResourceManager.GetString("CheckReqSP", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to ---------------TABLE: ReportListIds----------------------
        ///IF NOT EXISTS (SELECT TABLE_NAME FROM INFORMATION_SCHEMA.tables WHERE TABLE_NAME = &apos;ReportListIds&apos;)
        ///BEGIN
        ///	PRINT &apos;Creating Table ReportListIds&apos;
        ///	CREATE TABLE [dbo].[ReportListIds] ( Id uniqueidentifier )
        ///END
        /// 
        ///IF NOT EXISTS (SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = &apos;ReportListIds&apos; AND COLUMN_NAME = &apos;ListIcon&apos;)
        ///BEGIN
        ///	PRINT &apos;Add Column ListIcon&apos;
        ///	ALTER TABLE [dbo].[ReportListIds]
        ///	ADD [ListIcon] [NVARCHAR](100) NULL
        ///END
        ///
        ///- [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string CheckSchema
        {
            get
            {
                return ResourceManager.GetString("CheckSchema", resourceCulture);
            }
        }
        /// <summary>
        ///   Looks up a localized string similar to 
        ///---------------TABLE: EPMLive_Log----------------------
        ///if not exists (select column_name FROM INFORMATION_SCHEMA.COLUMNS where table_name = &apos;EPMLive_Log&apos; and column_name = &apos;timerjobuid&apos;)
        ///begin
        ///
        ///	if exists (select table_name from INFORMATION_SCHEMA.tables where table_name = &apos;EPMLive_Log&apos;)
        ///	begin
        ///		DROP TABLE EPMLive_log
        ///		Print &apos;     Dropping EPM Live Log&apos;
        ///	end
        ///	
        ///
        ///end
        ///
        ///if not exists (select table_name from INFORMATION_SCHEMA.tables where table_name = &apos;EPMLive_Log&apos;)
        ///	begin
        ///		print &apos;Creating  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _0Tables01 {
            get {
                return ResourceManager.GetString("_0Tables01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to ---------------TABLE: TSUSER----------------------
        ///
        ///if not exists (select table_name from INFORMATION_SCHEMA.tables where table_name = &apos;TSUSER&apos;)
        ///	begin
        ///		print &apos;Creating Table TSUSER&apos;
        ///		
        ///		CREATE TABLE [dbo].[TSUSER](
        ///			[TSUSERUID] [uniqueidentifier] NULL DEFAULT (newid()),
        ///			[SITE_UID] [uniqueidentifier] NULL,
        ///			[USER_ID] [int] NULL,
        ///			[USERNAME] [varchar](255) NULL,
        ///			[NAME] [varchar](255) NULL
        ///		) ON [PRIMARY]
        ///				
        ///	end
        ///else
        ///	begin
        ///		Print &apos;Updating Table TSUSER&apos;
        ///	end
        ///------------ [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _0Tables02 {
            get {
                return ResourceManager.GetString("_0Tables02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to declare @createoralter varchar(10)
        ///------------------------------View: vwMeta---------------------------
        ///if not exists (select table_name from INFORMATION_SCHEMA.tables where table_name = &apos;vwMeta&apos;)
        ///begin
        ///    Print &apos;Creating View vwMeta&apos;
        ///    SET @createoralter = &apos;CREATE&apos;
        ///end
        ///else
        ///begin
        ///    Print &apos;Updating View vwMeta&apos;
        ///    SET @createoralter = &apos;ALTER&apos;
        ///end
        ///exec(@createoralter + &apos; VIEW dbo.vwMeta
        ///AS
        ///SELECT     dbo.TSTIMESHEET.USERNAME AS Username, dbo.TSTIMESHEET.RESOURCENAME AS [Resource Name], d [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _1Views01 {
            get {
                return ResourceManager.GetString("_1Views01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to declare @createoralter varchar(10)
        ///if not exists (select routine_name from INFORMATION_SCHEMA.routines where routine_name = &apos;Split&apos;)
        ///begin
        ///    Print &apos;Creating Function Split&apos;
        ///    SET @createoralter = &apos;CREATE&apos;
        ///end
        ///else
        ///begin
        ///    Print &apos;Updating Function Split&apos;
        ///    SET @createoralter = &apos;ALTER&apos;
        ///end
        ///exec(@createoralter + &apos; FUNCTION [dbo].[Split]
        ///(
        ///@ItemList NVARCHAR(MAX),
        ///@delimiter CHAR(2)
        ///)
        ///RETURNS @IDTable TABLE (Item NVARCHAR(255) collate database_default )
        ///AS
        ///BEGIN
        ///DECLARE @tempItemList  [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _2SPs01 {
            get {
                return ResourceManager.GetString("_2SPs01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if not exists (select emailid from EMAILTEMPLATES where emailid = 2)
        ///begin
        ///    INSERT INTO EMAILTEMPLATES (emailid,title,subject,body) VALUES (2,&apos;Build Team Grant&apos;,&apos;You have been granted access to an EPM Live site&apos;,&apos;&lt;html&gt;
        ///&lt;body&gt;
        ///&lt;table width=&quot;100%&quot; cellpadding=&quot;0&quot; cellspacing=&quot;0&quot;&gt;
        ///&lt;tr&gt;
        ///&lt;td style=&quot;font-size:20px;color:#666666;font-family:Lucida Grande,Arial Unicode MS,sans-serif&quot;&gt;{CurUser_Name} has granted you access to the &lt;u&gt;&lt;a href=&quot;{SiteUrl}&quot; style=&quot;font-size:20px;color:#3366CC;&quot;&gt;{SiteName}&lt;/a&gt;&lt;/u [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _9Data01 {
            get {
                return ResourceManager.GetString("_9Data01", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to if not exists (select jobtype_id from TIMERJOBTYPES where jobtype_id = 52)
        ///begin
        ///    INSERT INTO TIMERJOBTYPES (jobtype_id,NetAssembly,NetClass,[Title],priority) VALUES (52,&apos;EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5&apos;,&apos;EPMLiveCore.Jobs.Applications.Uninstall&apos;,&apos;Application Uninstall&apos;,3)
        ///end
        ///else
        ///begin
        ///    UPDATE TIMERJOBTYPES SET NetAssembly=&apos;EPM Live Core, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5&apos;, NetClass=&apos;EPMLiveCore.Jobs.Applications.U [rest of string was truncated]&quot;;.
        /// </summary>
        public static string _9Data02 {
            get {
                return ResourceManager.GetString("_9Data02", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg Code=&quot;GTACCNPSQEBSLC&quot; Version=&quot;4.3.1&quot; /&gt;
        ///	&lt;Cfg SuppressCfg=&quot;1&quot;/&gt; &lt;!-- Base settings, suppresses saving configuration to cookies --&gt;
        ///    &lt;Cfg NumberId=&quot;1&quot; IdChars=&quot;0123456789&quot;/&gt; &lt;!-- Controls generation of new row ids --&gt;
        ///    &lt;Cfg NoFormatEscape=&quot;1&quot;/&gt; &lt;!-- You can use HTML code in formatting, set here because ValueSeparator and RangeSeparator contain HTML code --&gt;
        ///    &lt;Cfg Sort=&quot;StartDate&quot;/&gt; &lt;!-- Default sort is by StartDate --&gt;
        ///    &lt;Cfg Group=&quot;AssignedToText&quot;/&gt; &lt;!-- The grid is grouped by [rest of string was truncated]&quot;;.
        /// </summary>
        public static string AssignmentPlannerGridLayout {
            get {
                return ResourceManager.GetString("AssignmentPlannerGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg Code=&apos;GTACCNPSQEBSLC&apos; Version=&apos;4.3.2.120412&apos; /&gt;
        ///	&lt;Cfg SuppressCfg=&apos;1&apos; /&gt;
        ///	&lt;Cfg MainCol=&apos;Title&apos; NameCol=&apos;Title&apos; /&gt;
        ///	&lt;Cfg ConstWidth=&apos;0&apos; /&gt;
        ///	&lt;Cfg Undo=&apos;0&apos; /&gt;
        ///	&lt;Cfg Searching=&apos;0&apos; Selecting=&quot;1&quot; Deleting=&apos;0&apos; /&gt;
        ///	&lt;Cfg StaticCursor=&apos;1&apos; Dragging=&apos;0&apos; /&gt;
        ///	&lt;Cfg NoTreeLines=&apos;1&apos; /&gt;
        ///	&lt;Cfg Editing=&apos;0&apos; /&gt;
        ///	&lt;Cfg NoVScroll=&apos;1&apos; /&gt;
        ///	&lt;Def&gt;
        ///		&lt;D Name=&apos;C&apos; Width=&apos;320&apos; /&gt;
        ///		&lt;D Name=&quot;R&quot; Height=&quot;30&quot; /&gt;
        ///		&lt;D Name=&quot;R&quot; HoverCell=&quot;Color&quot; HoverRow=&quot;Color&quot; FocusCell=&quot;Background&quot; FocusRow=&quot;Background&quot; /&gt;
        ///	&lt;/Def&gt;        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ChartWizardDataSourceGridLayout {
            get {
                return ResourceManager.GetString("ChartWizardDataSourceGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Navs&gt;
        ///&lt;QuickLaunch&gt;
        ///&lt;Item Name=&quot;My Work&quot; Url=&quot;{SiteUrl}/MyWork.aspx&quot;&gt;
        ///    &lt;Item Name=&quot;My Work&quot; Url=&quot;{SiteUrl}/MyWork.aspx&quot;/&gt;
        ///    &lt;Item Name=&quot;To Do&quot; Url=&quot;{SiteUrl}/lists/To Do&quot; List=&quot;To Do&quot;/&gt;
        ///    &lt;Item Name=&quot;My Time Off&quot; Url=&quot;{SiteUrl}/Lists/Time Off&quot; List=&quot;Time Off&quot;/&gt;
        ///&lt;/Item&gt;
        ///&lt;/QuickLaunch&gt;
        ///&lt;TopNav&gt;
        ///&lt;/TopNav&gt;
        ///&lt;/Navs&gt;.
        /// </summary>
        public static string DefaultCommunityNavs {
            get {
                return ResourceManager.GetString("DefaultCommunityNavs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;script language=&apos;javascript&apos; type=&apos;text/javascript&apos;&gt;
        ///
        ///function gridfilter#Grid#(value)
        ///{
        ///var vals = value.split(&apos;|&apos;);
        ///myGrid#Grid#.filterBy(vals[0],vals[1]);
        ///}
        ///
        ///function postResources#Grid#()
        ///{
        ///var arrRows = myGrid#Grid#.getCheckedRows(0).split(&apos;,&apos;);
        ///var ress = &apos;&apos;;
        ///var resCount = 0;
        ///if(arrRows != &apos;&apos;)
        ///{
        ///for(var i = 0;i &lt; arrRows.length;i++)
        ///{
        ///var res = myGrid#Grid#.getUserData(arrRows[i],&apos;itemid&apos;);
        /// if(res !=&apos;&apos;)
        /// {
        /// ress += res + &apos;,&apos;;
        /// resCount++;
        /// }
        /// }
        /// if(resCount &gt; 20)
        /// {
        /// alert [rest of string was truncated]&quot;;.
        /// </summary>
        public static string Javascript {
            get {
                return ResourceManager.GetString("Javascript", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg Code=&quot;GTACCNPSQEBSLC&quot; Version=&quot;4.4.0.32713&quot; /&gt;
        ///	&lt;Cfg SuppressCfg=&quot;1&quot; Style=&quot;GM&quot; /&gt;
        ///	&lt;Cfg MainCol=&quot;Title&quot; NameCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg ConstWidth=&quot;0&quot; /&gt;
        ///	&lt;Cfg NoVScroll=&quot;0&quot; /&gt;
        ///	&lt;Cfg Undo=&quot;0&quot; /&gt;
        ///	&lt;Cfg NumberId=&quot;1&quot; FullId=&quot;0&quot; IdChars=&quot;1234567890&quot; AddFocusCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg Searching=&quot;1&quot; /&gt;
        ///	&lt;Cfg StaticCursor=&quot;1&quot; Dragging=&quot;0&quot; SelectingCells=&quot;1&quot; SelectClass=&quot;0&quot; /&gt;
        ///	&lt;Cfg NoTreeLines=&quot;1&quot; DetailOn=&quot;0&quot; MinRowHeight=&quot;20&quot; MaxRowHeight=&quot;20&quot; MidWidth=&quot;300&quot; MenuColumnsSort=&quot;1&quot; StandardFilter=&quot;2&quot; /&gt;
        /// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string MyWorkGridLayout {
            get {
                return ResourceManager.GetString("MyWorkGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EPMLiveReportsAdmin, Version=1.0.0.0, Culture=neutral, PublicKeyToken=b90e532f481cf050.
        /// </summary>
        public static string ReportingAssembly {
            get {
                return ResourceManager.GetString("ReportingAssembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EPMLiveReportsAdmin.ListEvents.
        /// </summary>
        public static string ReportingClassName {
            get {
                return ResourceManager.GetString("ReportingClassName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTColumn.
        /// </summary>
        public static string ReportingColumnTable {
            get {
                return ResourceManager.GetString("ReportingColumnTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTDatabases.
        /// </summary>
        public static string ReportingDatabaseTable {
            get {
                return ResourceManager.GetString("ReportingDatabaseTable", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InitDatabaseCreateProcedures.sql.
        /// </summary>
        public static string ReportingInitDatabaseCreateProcedures {
            get {
                return ResourceManager.GetString("ReportingInitDatabaseCreateProcedures", resourceCulture);
            }
        }

        /// <summary>
        ///   Looks up a localized string similar to InitDatabaseCreateTables.sql.
        /// </summary>
        public static string ReportingInitDatabaseCreateTables {
            get {
                return ResourceManager.GetString("ReportingInitDatabaseCreateTables", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to LST.
        /// </summary>
        public static string ReportingListPrefix {
            get {
                return ResourceManager.GetString("ReportingListPrefix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to VWRPTListSummary.
        /// </summary>
        public static string ReportingListSummaryView {
            get {
                return ResourceManager.GetString("ReportingListSummaryView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTList.
        /// </summary>
        public static string ReportingListTable {
            get {
                return ResourceManager.GetString("ReportingListTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTLog.
        /// </summary>
        public static string ReportingLogTable {
            get {
                return ResourceManager.GetString("ReportingLogTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTSettings.
        /// </summary>
        public static string ReportingSettingsTable {
            get {
                return ResourceManager.GetString("ReportingSettingsTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Snapshot.
        /// </summary>
        public static string ReportingSnapshotTableSuffix {
            get {
                return ResourceManager.GetString("ReportingSnapshotTableSuffix", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTTSData.
        /// </summary>
        public static string ReportingTimesheetTable {
            get {
                return ResourceManager.GetString("ReportingTimesheetTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to RPTWork.
        /// </summary>
        public static string ReportingWorkTable {
            get {
                return ResourceManager.GetString("ReportingWorkTable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;/td&gt;
        ///	    &lt;/tr&gt;
        ///	&lt;/table&gt;
        ///&lt;/td&gt;
        ///&lt;td class=&apos;ms-toolbar&apos; nowrap=&apos;true&apos; align=&apos;right&apos;&gt;
        ///	&lt;table border=0 cellpadding=0 cellspacing=0 style=&apos;margin-right: 4px&apos;&gt;
        ///		    &lt;tr&gt;
        ///		    
        ///		    &lt;td nowrap class=&apos;ms-listheaderlabel&apos;&gt;Resource Pool View:&amp;nbsp;&lt;/td&gt;
        ///            &lt;td id=&apos;&apos; nowrap=&apos;nowrap&apos; class=&apos;ms-viewselector&apos; onmouseover=&quot;&quot;this.className=&apos;ms-viewselectorhover&apos;&quot;&quot; onmouseout=&quot;&quot;this.className=&apos;ms-viewselector&apos;&quot;&quot;&gt;
        ///			&lt;span style=&apos;display:none&apos;&gt;
        ///
        ///
        ///                &lt;menu type=&apos;ServerMenu&apos; id=&apos;EPMLive [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ResourceDDLBottom {
            get {
                return ResourceManager.GetString("ResourceDDLBottom", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;table class=&apos;ms-menutoolbar&apos; cellpadding=&apos;2&apos; cellspacing=&apos;0&apos; border=&apos;0&apos; width=&apos;100%&apos;&gt;
        ///&lt;tr height=&apos;23&apos;&gt;
        ///&lt;td class=&apos;ms-toolbar&apos; &gt;
        ///	&lt;table&gt;
        ///        &lt;tr&gt;
        ///        &lt;td valign=&apos;center&apos;&gt;.
        /// </summary>
        public static string ResourceDDLTop {
            get {
                return ResourceManager.GetString("ResourceDDLTop", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0M8R4KGxGuEAAAAAAAAAAAAAAAAAAAAAPgADAP7/CQAGAAAAAAAAAAAAAAACAAAA2gAAAAAAAAAAEAAAAgAAAAQAAAD+////AAAAANcAAADWAAAA//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ResourceExporterVBA {
            get {
                return ResourceManager.GetString("ResourceExporterVBA", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 3.
        /// </summary>
        public static string ResourceGridDefaultGlobalViewVersion {
            get {
                return ResourceManager.GetString("ResourceGridDefaultGlobalViewVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg Code=&quot;GTACCNPSQEBSLC&quot; Version=&quot;4.3.2.120412&quot; /&gt;
        ///	&lt;Cfg SuppressCfg=&quot;1&quot; /&gt;
        ///	&lt;Cfg MainCol=&quot;Title&quot; NameCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg ConstWidth=&quot;0&quot; /&gt;
        ///	&lt;Cfg Undo=&quot;0&quot; /&gt;
        ///	&lt;Cfg NumberId=&quot;1&quot; FullId=&quot;0&quot; IdChars=&quot;1234567890&quot; AddFocusCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg Searching=&quot;1&quot; Selecting=&quot;1&quot; Deleting=&quot;0&quot; /&gt;
        ///	&lt;Cfg StaticCursor=&quot;1&quot; Dragging=&quot;0&quot; SelectingCells=&quot;1&quot; SelectClass=&quot;0&quot; /&gt;
        ///	&lt;Cfg NoTreeLines=&quot;1&quot; DetailOn=&quot;0&quot; MinRowHeight=&quot;20&quot; MaxRowHeight=&quot;25&quot; MidWidth=&quot;300&quot; MenuColumnsSort=&quot;1&quot; StandardFilter=&quot;2&quot; /&gt;
        ///	&lt;Cfg P [rest of string was truncated]&quot;;.
        /// </summary>
        public static string ResourceGridLayout {
            get {
                return ResourceManager.GetString("ResourceGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to /* This trigger has been created by the EPM Live Integration adapter on {TIMESTAMP} */
        ///
        ///trigger {TRIGGER_NAME} on {TRIGGER_OBJECT} bulk (after insert, after update, after delete) {
        ///	{APP_NAMESPACE}.GlobalTriggerDispatcher.DispatchTrigger(&apos;{TRIGGER_OBJECT}&apos;, Trigger.isInsert, Trigger.isUpdate, Trigger.isDelete, Trigger.isBefore, Trigger.isAfter, Trigger.isExecuting, Trigger.new, Trigger.newMap, Trigger.old, Trigger.oldMap);
        ///}.
        /// </summary>
        public static string SFIntTrigger {
            get {
                return ResourceManager.GetString("SFIntTrigger", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;td width=&quot;50&quot; valign=&quot;top&quot; style=&quot;padding:10px;&quot;&gt;
        ///&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;br&gt;&lt;br&gt;
        ///	&lt;input type=&quot;button&quot; class=&quot;ms-ButtonHeightWidth&quot; value=&quot;&amp;lt; Add&quot; onClick=&quot;AddResource()&quot; style=&quot;width:100px&quot;&gt;&lt;br&gt;&lt;br&gt;
        ///	&lt;input type=&quot;button&quot; class=&quot;ms-ButtonHeightWidth&quot; value=&quot;Remove &amp;gt;&quot; onClick=&quot;RemoveResource()&quot; style=&quot;width:100px&quot;&gt;
        ///&lt;/td&gt;
        ///&lt;td id=&quot;tdRes&quot; valign=&quot;top&quot;&gt;
        ///	&lt;div class=&quot;gridHeader&quot;&gt;Resource Pool&lt;/div&gt;
        ///	&lt;div class=&quot;toolbar&quot;&gt;
        ///		&lt;ul&gt;
        /// 		    &lt;li&gt;
        ///			    &lt;a href=&quot;javascript:void(0);&quot; onclick=&quot;AddResourcePoo [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtBuildTeamResPool {
            get {
                return ResourceManager.GetString("txtBuildTeamResPool", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;GroupTemplate Id=&quot;Ribbon.Templates.StandardGroup&quot;&gt;
        ///	&lt;Layout Title=&quot;OneLargeTwoMedium&quot; LayoutTitle=&quot;OneLargeTwoMedium&quot;&gt;
        ///		&lt;Section Alignment=&quot;Top&quot; Type=&quot;OneRow&quot;&gt;
        ///		&lt;Row&gt;
        ///			&lt;ControlRef DisplayMode=&quot;Large&quot; TemplateAlias=&quot;cust1&quot; /&gt;
        ///			&lt;ControlRef DisplayMode=&quot;Large&quot; TemplateAlias=&quot;cust2&quot; /&gt;
        ///		&lt;/Row&gt;
        ///		&lt;/Section&gt;
        ///		&lt;OverflowSection Type=&quot;OneRow&quot; TemplateAlias=&quot;o3&quot; DisplayMode=&quot;Large&quot;/&gt;
        ///		&lt;OverflowSection Type=&quot;ThreeRow&quot; TemplateAlias=&quot;oM&quot; DisplayMode=&quot;Medium&quot;/&gt;
        ///		&lt;OverflowSection Type=&quot;ThreeRow&quot; Temp [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtBuildTeamRibbonTemplate {
            get {
                return ResourceManager.GetString("txtBuildTeamRibbonTemplate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Tab Id=&quot;Ribbon.BuildTeam&quot; Title=&quot;Build Team&quot; Description=&quot;Build Team&quot; Sequence=&quot;1105&quot;&gt;
        ///    &lt;Scaling Id=&quot;Ribbon.BuildTeam.Scaling&quot;&gt;
        ///
        ///        &lt;MaxSize Id=&quot;Ribbon.BuildTeam.StandardGroup.MaxSize&quot; GroupId=&quot;Ribbon.BuildTeam.StandardGroup&quot; Size=&quot;OneLargeTwoMedium&quot;/&gt;
        ///        &lt;Scale Id=&quot;Ribbon.BuildTeam.StandardGroup.Scaling.CustomTabScaling&quot; GroupId=&quot;Ribbon.BuildTeam.StandardGroup&quot; Size=&quot;OneLargeTwoMedium&quot; /&gt;
        ///
        ///		&lt;MaxSize Id=&quot;Ribbon.BuildTeam.TeamGroup.MaxSize&quot; GroupId=&quot;Ribbon.BuildTeam.TeamGroup&quot; Size=&quot;OneL [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtBuildTeamTab {
            get {
                return ResourceManager.GetString("txtBuildTeamTab", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function findacontrol(FieldName) {   
        ///	var arr = document.getElementsByTagName(&quot;!&quot;);
        ///	for (var i=0;i &lt; arr.length; i++ )
        ///	  if (arr[i].innerHTML.indexOf(FieldName) &gt; 0)
        ///		return arr[i];
        ///}   
        ///
        ///function getFormInput(tablefield)
        ///{
        ///	return tablefield.nextSibling.firstChild;
        ///}
        ///
        ///function getUsers(fieldname)
        ///{
        ///	var users = &quot;&quot;;
        ///	try
        ///	{
        ///		var resTable = findacontrol(fieldname);
        ///		var userField = &quot;&quot;;
        ///
        ///		if(resTable != null)
        ///		{
        ///			userField = resTable.nextSibling.firstChild.id.replace(&quot;HiddenUse [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtCheckResourceJS {
            get {
                return ResourceManager.GetString("txtCheckResourceJS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;table width=&quot;100%&quot; border=&quot;0&quot; cellpadding=&quot;0&quot; cellspacing=&quot;0&quot;&gt;
        ///    &lt;tr id=&quot;idWorkspaceArea&quot;&gt;
        ///        &lt;td&gt;
        ///            &lt;div id=&quot;idEPK3PDiv&quot;&gt;
        ///                &lt;object classid=&quot;CLSID:376F87A8-2670-4595-8ECD-435834A81C7E&quot; codebase=&quot;***EPKURL***/CAB/EPK_3P.CAB#version=62,0,0,1531&quot; type=&quot;application/x-oleobject&quot; style=&quot;display: none&quot;&gt;&lt;/object&gt;
        ///            &lt;/div&gt;
        ///
        ///            &lt;div id=&quot;EPKDisplayDiv&quot;&gt;
        ///                &lt;object classid=&quot;CLSID:7393552F-C4E6-49F0-8B01-52819BB9A0BC&quot; type=&quot;application/x-oleobject&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtEPKWebpart {
            get {
                return ResourceManager.GetString("txtEPKWebpart", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Overdue Tasks`Task Center`Title|Name
        ///StartDate|Start
        ///DueDate|Finish
        ///PercentComplete|% Complete
        ///Project`&lt;And&gt;&lt;Eq&gt;&lt;FieldRef Name=&quot;AssignedTo&quot;/&gt;&lt;Value Type=&quot;UserMulti&quot;&gt;&lt;UserID/&gt;&lt;/Value&gt;&lt;/Eq&gt;&lt;And&gt;&lt;Neq&gt;&lt;FieldRef Name=&quot;PercentComplete&quot;/&gt;&lt;Value Type=&quot;Number&quot;&gt;1&lt;/Value&gt;&lt;/Neq&gt;&lt;Lt&gt;&lt;FieldRef Name=&quot;DueDate&quot;/&gt;&lt;Value Type=&quot;Text&quot;&gt;&lt;Today/&gt;&lt;/Value&gt;&lt;/Lt&gt;&lt;/And&gt;&lt;/And&gt;	Task Scheduled to Start in Next 14 Days`Task Center`Title|Name
        ///StartDate|Start
        ///DueDate|Finish
        ///Project`&lt;And&gt;&lt;And&gt;&lt;Eq&gt;&lt;FieldRef Name=&quot;AssignedTo&quot;/&gt;&lt;Value Typ [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtFileDefaultSections {
            get {
                return ResourceManager.GetString("txtFileDefaultSections", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg id=&apos;TeamGrid&apos; SuppressCfg=&apos;0&apos;/&gt; &lt;!-- Configuration is not saved to cookies --&gt;
        ///	&lt;Cfg MainCol=&apos;Title&apos; NameCol=&apos;Title&apos; Style=&apos;GM&apos; CSS=&apos;TreeGrid/WorkEngine/Grid.css&apos;  Undo=&apos;0&apos; ChildParts=&apos;0&apos;/&gt;
        ///	&lt;Cfg ExportType=&apos;Expanded,Outline&apos;/&gt;
        ///	&lt;Cfg PrintCols=&apos;1&apos;/&gt;
        ///	&lt;Cfg ExportCols=&apos;1&apos;/&gt;
        ///	&lt;Cfg NumberId=&apos;1&apos; FullId=&apos;0&apos; IdChars=&apos;1234567890&apos; AddFocusCol=&apos;Title&apos; /&gt;
        ///	&lt;Cfg StaticCursor=&apos;1&apos; Dragging=&apos;0&apos; SelectingCells=&apos;1&apos; ShowDeleted=&apos;0&apos; SelectClass=&apos;0&apos; Hover=&apos;0&apos;/&gt;
        ///	&lt;Cfg Paging=&quot;2&quot; AllPages=&quot;1&quot; PageLength=&quot;25&quot; [rest of string was truncated]&quot;;.
        /// </summary>
        public static string txtTeamGridLayout {
            get {
                return ResourceManager.GetString("txtTeamGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to EPMLiveWebParts, Version=1.0.0.0, Culture=neutral, PublicKeyToken=9f4da00116c38ec5.
        /// </summary>
        public static string WebPartsAssembly {
            get {
                return ResourceManager.GetString("WebPartsAssembly", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to FancyDisplayForm.
        /// </summary>
        public static string WebPartsFancyDisplayForm {
            get {
                return ResourceManager.GetString("WebPartsFancyDisplayForm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to GridListView.
        /// </summary>
        public static string WebPartsGridListView {
            get {
                return ResourceManager.GetString("WebPartsGridListView", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to MyWorkWebPart.
        /// </summary>
        public static string WebPartsMyWork {
            get {
                return ResourceManager.GetString("WebPartsMyWork", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///	&lt;Cfg Code=&quot;GTACCNPSQEBSLC&quot; Version=&quot;4.0&quot; /&gt;
        ///	&lt;Cfg SuppressCfg=&quot;1&quot; Style=&quot;GS&quot; /&gt;
        ///	&lt;Cfg MainCol=&quot;Title&quot; NameCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg ConstHeight=&quot;1&quot; ConstWidth=&quot;0&quot; /&gt;
        ///	&lt;Cfg Undo=&quot;0&quot; /&gt;
        ///	&lt;Cfg NumberId=&quot;1&quot; FullId=&quot;0&quot; IdChars=&quot;1234567890&quot; AddFocusCol=&quot;Title&quot; /&gt;
        ///	&lt;Cfg StaticCursor=&quot;1&quot; Dragging=&quot;0&quot; SelectingCells=&quot;1&quot; SelectClass=&quot;0&quot; /&gt;
        ///	&lt;Cfg NoTreeLines=&quot;1&quot; DetailOn=&quot;0&quot; MidWidth=&quot;300&quot; MenuColumnsSort=&quot;1&quot; StandardFilter=&quot;2&quot; /&gt;
        ///	&lt;Def&gt;
        ///		&lt;D Name=&quot;R&quot; HoverCell=&quot;Color&quot; HoverRow=&quot;Color&quot; FocusCell=&quot;&quot; FocusR [rest of string was truncated]&quot;;.
        /// </summary>
        public static string WorkingOnGridLayout {
            get {
                return ResourceManager.GetString("WorkingOnGridLayout", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;Grid&gt;
        ///&lt;Cfg id=&quot;gridWorkSpaceCenter&quot;/&gt;
        ///  &lt;Cfg Code=&quot;GTACCNPSQEBSLC&quot; Version=&quot;4.4.0.32713&quot;/&gt;
        ///  &lt;Cfg SuppressCfg=&quot;1&quot; Style=&apos;GM&apos; CSS=&quot;/_layouts/epmlive/treegrid/grid/grid.css&quot; /&gt;
        ///  &lt;Cfg MainCol=&quot;WebTitle&quot; NameCol=&quot;WebTitle&quot; /&gt;
        ///  &lt;Cfg ConstWidth=&quot;0&quot; /&gt;
        ///  &lt;Cfg NoVScroll=&quot;0&quot; /&gt;
        ///  &lt;Cfg Undo=&quot;0&quot; /&gt;
        ///  &lt;Cfg NumberId=&quot;1&quot; FullId=&quot;0&quot; IdChars=&quot;1234567890&quot; AddFocusCol=&quot;WebTitle&quot; /&gt;
        ///  &lt;Cfg Searching=&quot;1&quot; /&gt;
        ///  &lt;Cfg StaticCursor=&quot;1&quot; Dragging=&quot;0&quot; SelectingCells=&quot;1&quot; SelectClass=&quot;0&quot; /&gt;
        ///  &lt;Cfg NoTreeLines=&quot;1&quot; DetailOn= [rest of string was truncated]&quot;;.
        /// </summary>
        public static string WorkSpaceCenterLayout {
            get {
                return ResourceManager.GetString("WorkSpaceCenterLayout", resourceCulture);
            }
        }
    }
}
