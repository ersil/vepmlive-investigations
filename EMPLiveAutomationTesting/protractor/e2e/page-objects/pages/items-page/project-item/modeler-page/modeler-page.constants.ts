export class ModelerPageConstants {
    static readonly selectModel = 'Modeler popup';
    static readonly modeler = 'Modeler page';
    static readonly findNext = 'Find Next Button';
    static readonly ok = 'OK Button';
    static readonly cancel = 'Cancel Button';
    static readonly selectModelDropdown = 'Select Model Dropdown';
    static readonly selectVersions = 'Select versions selection box';
    static readonly activeTab = 'Active Tab';
    static readonly viewTab = 'View Tab';
    static readonly settingsSearch = 'Search for label';
    static readonly settingsWhere = 'Where dropdown';
    static readonly settingsIn = 'In dropdown';
    static readonly settingsDetails = 'Details radio button';
    static readonly settingsTotals = 'Totals radio button';
    static readonly disabled = 'disabled';
    static readonly defaultSelectedOption = 'Current';
    static readonly selectVersionsOptions: string[] = ['Current', 'Version 1', 'Version 2', 'Version 3'];
    static readonly selectVersionDefault = 'Select versions - default';
    static readonly selectVersionsOptionsText = 'Select versions options';
    static readonly close = 'Close button';
    static readonly totalDetails = 'Total Details';
    static readonly searchSettings = 'Search Settings';
    static readonly saveVersion = 'Save Version';
    static readonly copyVersion = 'Copy Version';
    static readonly exportToExcel = 'Export to Excel Top';
    static readonly print = 'Print top';
    static readonly detailsTopGrid = 'Details Top Grid';
    static readonly applyTarget = 'Apply Target';
    static readonly totalsDetails = 'Totals Details';
    static readonly createTarget = 'Create Target';
    static readonly editTarget = 'Edit Target';
    static readonly deleteTarget = 'Delete Target';
    static readonly copyTarget = 'Copy Target';
    static readonly exportToExcelBottom = 'Export to Excel Bottom';
    static readonly printBottom = 'Print Bottom';
    static readonly totalsBottomGrid = 'Totals Bottom Grid';
    static readonly noVersionsPopupText = 'There are either no versions defined or versions that have been loaded or copied to save!';
    static readonly from = 'From dropdown';
    static readonly to = 'To dropdown';
    static readonly notInToVersion = 'Not in To Version selection box';
    static readonly bothVersions = 'Both Versions';
    static readonly collapsedTopRibbon = 'Collapsed Top Ribbon';
    static readonly collapsedBottomRibbon = 'Collapsed Bottom Ribbon';
    static readonly noTargetsAlertText = 'No Targets have been defined  or are available to apply';
    static readonly saveView = 'Save View';
    static readonly renameView = 'Rename View';
    static readonly deleteView = 'Delete View';
    static readonly sortAndGroup = 'Sort And Group';
    static readonly columnOrder = 'Colum Order';
    static readonly periodsAndValues = 'Periods And Values';
    static readonly showGantt = 'Show Gantt';
    static readonly ganttZoom = 'Gantt Zoom Dropdown';
    static readonly currentView = 'Current View Dropdown';

    static get selectModelAndVersionsPopup() {
        return {
            title: 'Select Model and Versions',
            titleClass: 'dhtmlx_wins_title',
            selectModelDropdown: 'idModelList',
            versionsSelectionBox: 'idVersionList',
            ok: 'OK',
            cancel: 'Cancel',
        };
    }

    static get tabNames() {
        return {
            display: 'Display',
            view: 'View'
        };
    }

    static get displayTabOptions() {
        return {
            close: 'close',
            totalDetails: 'DetailsBtn',
            searchSettings: 'tSearchBtn',
            findNext: 'FindBtn',
            saveVersion: 'SaveVers',
            copyVersion: 'CopyVers',
            exportToExcel: 'idExportExcelTop',
            print: 'idPrintTop',
            detailsTopGrid: 'gridDiv_1',
            applyTarget: 'LoadTargetBtn',
            totalsDetails: 'TotalsBtn',
            createTarget: 'CreateTargetBtn',
            editTarget: 'EditTargetBtn',
            deleteTarget: 'DeleteTargetBtn',
            copyTarget: 'CopyTargetBtn',
            exportToExcelBottom: 'ExportBotBtn',
            printBottom: 'PrintBotBtn',
            comparisonData: 'Comparison Data',
            totalsBottomGrid: 'bottomgridDiv_1'
        };
    }

    static get searchSettingsPopup() {
        return {
            searchFor: 'idtxtsearch',
            where: 'idSearchHow',
            in: 'idSearchWhere',
            details: 'rbSearchDet',
            totals: 'rbSearchTot',
            ok: 'OK',
            cancel: 'Cancel'
        };
    }

    static get copyVersionPopup() {
        return {
            from: 'SelFromVersion',
            to: 'SelToVersion',
            notInToVersion: 'idSelVersTo',
            bothVersions: 'idSelVersBoth',
            ok: 'OK',
            cancel: 'Cancel'
        };
    }

    static get collapseAndExpandRibbons() {
        return {
            collapseViewTopRibbon: 'idRibbonDiv_ulCollapsed',
            expandViewTopRibbon: 'idRibbonDiv_ul',
            collapseViewBottomRibbon: 'idBottomRibbonDiv_ulCollapsed',
            expandViewBottomRibbon: 'idBottomRibbonDiv_ul',
        };
    }

    static get viewTabOptions() {
        return {
            close: 'close',
            saveView: 'SaveViewBtn',
            renameView: 'RenameViewBtn',
            deleteView: 'DeleteViewBtn',
            sortAndGroup: 'SortBtn',
            columnOrder: 'ColumnBtn',
            periodsAndValues: 'PerBtn',
            showGantt: 'GanttChk',
            ganttZoom: 'idGanttZoom_textbox',
            currentView: 'idUserViews_button'
        };
    }
}