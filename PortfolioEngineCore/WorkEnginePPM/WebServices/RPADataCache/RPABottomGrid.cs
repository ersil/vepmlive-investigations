﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CostDataValues;
using EPMLiveCore;
using EPMLiveCore.Infrastructure.Logging;
using Microsoft.SharePoint.Administration;
using PortfolioEngineCore;
using ResourceValues;
using static EPMLiveCore.Infrastructure.Logging.LoggingService;

namespace RPADataCache
{
    internal class RPABottomGrid : RPADataCacheGridBase<clsResFullDAta>
    {
        private static readonly NumberFormatInfo _numberFormat = new NumberFormatInfo
        {
            NumberDecimalSeparator = ".",
            NumberGroupSeparator = ",",
            NumberGroupSizes = new int[] { 3 }
        };

        private readonly bool _useRole;
        private readonly string _roleHeader;
        private readonly bool _useHeatMap;
        private readonly int _heatMapId;
        private readonly int _heatMapColor;
        private readonly int _heatFieldColor;
        private readonly int _mode;
        // (CC-78790, 2018-08-14) Used as out parameters by external method therefore type can not be changed without overhead
        private readonly Dictionary<int, clsViewTargetColours> _targetColors;
        private readonly bool _doZeroRowCleverStuff;
        private readonly bool _displayTotalsDetails;
        private readonly Func<int, string> _resolvePiNameFunc;
        
        protected CStruct DefinitionPI;
        protected CStruct LastDataRowNode;

        public RPABottomGrid(
            bool useRole,
            string roleHeader,
            bool useHeatMap,
            int heatMapId,
            int heatMapColor,
            int mode,
            Dictionary<int, clsViewTargetColours> targetColors,
            bool doZeroRowCleverStuff,
            bool displayTotalsDetails,
            Func<int, string> resolvePiNameFunc,
            IList<clsRXDisp> columns, 
            int pmoAdmin, 
            string xmlString, 
            int displayMode, 
            IList<RPATGRow> displayList, 
            clsResourceValues resourceValues, 
            clsLookupList categoryLookupList) 
        : base(columns, pmoAdmin, xmlString, displayMode, displayList, resourceValues, categoryLookupList)
        {
            _useRole = useRole;
            _roleHeader = roleHeader;
            _useHeatMap = useHeatMap;
            _heatMapId = heatMapId;
            _heatMapColor = heatMapColor;
            _mode = mode;
            _targetColors = targetColors;
            _doZeroRowCleverStuff = doZeroRowCleverStuff;
            _displayTotalsDetails = displayTotalsDetails;
            _resolvePiNameFunc = resolvePiNameFunc;
        }

        protected override void InitializeGridLayout(GridRenderingTypes renderingType)
        {
            if (renderingType == GridRenderingTypes.None)
            {
                throw new ArgumentException("renderingType");
            }

            var xToolbar = Constructor.CreateSubStruct("Toolbar");
            xToolbar.CreateIntAttr("Visible", 0);

            var xMenuc = Constructor.CreateSubStruct("MenuCfg");

            var xPanel = Constructor.CreateSubStruct("Panel");
            xPanel.CreateIntAttr("Visible", 0);
            xPanel.CreateIntAttr("Select", 0);
            xPanel.CreateIntAttr("Delete", 0);
            xPanel.CreateIntAttr("CanHide", 0);
            xPanel.CreateIntAttr("CanSelect", 0);

            var xCfg = InitializeGridLayoutConfig("ResOrRole", 1, 0, 400, 400);
            xCfg.CreateIntAttr("ConstHeight", 1);
            xCfg.CreateIntAttr("LeftCanResize", 0);

            var xLeftCols = Constructor.CreateSubStruct("LeftCols");
            var xCols = Constructor.CreateSubStruct("Cols");
            var xRightCols = Constructor.CreateSubStruct("RightCols");
            PeriodCols = xRightCols;

            var m_xDef = Constructor.CreateSubStruct("Def");
            DefinitionRight = InitializeGridLayoutDefinition("R", m_xDef, true);
            DefinitionLeaf = InitializeGridLayoutDefinition("Leaf", m_xDef, false);
            DefinitionPI = InitializeGridLayoutDefinition("Leaf", m_xDef, false);

            var xHead = Constructor.CreateSubStruct("Head");
            InitializeGridLayoutHeader1(xHead, 1, 2);

            var categoryColumn = InitializeGridLayoutCategoryColumns(xLeftCols).Last();
            DefinitionRight.CreateBooleanAttr("rtSelectCanEdit", true);
            DefinitionLeaf.CreateStringAttr("IconFlag", "/_layouts/ppm/images/Nogif.gif");

            var xSolid = Constructor.CreateSubStruct("Solid");
            var xGroup = xSolid.CreateSubStruct("Group");

            foreach (var column in _columns)
            {
                categoryColumn = InitializeViewColumn(xCols, categoryColumn, column);
            }
        }

        private IEnumerable<CStruct> InitializeGridLayoutCategoryColumns(CStruct xLeftCols)
        {
            var categoryColumn = CreateColumn(xLeftCols, "RowSel", "Icon",
                width: 20,
                canMove: false,
                canExport: false,
                canEdit: false,
                canHide: null,
                canSelect: null);
            categoryColumn.CreateStringAttr("Color", "rgb(223, 227, 232)");
            Header1.CreateStringAttr("RowSel", GlobalConstants.Whitespace);
            Header2.CreateStringAttr("RowSel", GlobalConstants.Whitespace);
            yield return categoryColumn;

            categoryColumn = CreateColumn(xLeftCols, "rowid", "Int",
                visible: false,
                canExport: false);
            yield return categoryColumn;

            categoryColumn = CreateColumn(xLeftCols, "IconFlag", "Icon",
                width: 20,
                visible: true,
                canMove: false);
            Header1.CreateStringAttr("IconFlag", GlobalConstants.Whitespace);
            yield return categoryColumn;

            categoryColumn = CreateColumn(xLeftCols, "rtSelect", "Bool",
                width: 20,
                visible: true,
                canEdit: true,
                canMove: false);
            categoryColumn.CreateStringAttr("Class", string.Empty);
            Header1.CreateString("rtSelect", GlobalConstants.Whitespace);
            Header2.CreateString("rtSelect", GlobalConstants.Whitespace);
            yield return categoryColumn;
        }

        private CStruct InitializeViewColumn(CStruct xCols, CStruct categoryColumn, clsRXDisp col)
        {
            try
            {
                if (col.m_id == RPConstants.TGRID_TOTITEM_ID
                    || col.m_id == RPConstants.TGRID_TOTRESRES_ID
                    || _useRole == false)
                {
                    var sn = RemoveCharacters(col.m_realname, " \r\n")
                        .Replace("/n", string.Empty);

                    var snv = col.m_dispname.Replace("/n", "\n");

                    if (col.m_id == RPConstants.TGRID_TOTRESRES_ID)
                    {
                        snv = _roleHeader;
                        sn = "ResOrRole";
                    }

                    categoryColumn = xCols.CreateSubStruct("C");
                    if (col.m_id > 100)
                    {
                        sn = "x" + sn;
                    }
                    categoryColumn.CreateStringAttr("Name", sn);
                    categoryColumn.CreateIntAttr("ShowHint", 0);
                    categoryColumn.CreateStringAttr("Class", "GMCellMain");

                    DefinitionRight.CreateStringAttr(sn + "HtmlPrefix", "<B>");
                    DefinitionRight.CreateStringAttr(sn + "HtmlPostfix", "</B>");
                    DefinitionLeaf.CreateStringAttr(sn + "HtmlPrefix", string.Empty);
                    DefinitionLeaf.CreateStringAttr(sn + "HtmlPostfix", string.Empty);
                    DefinitionPI.CreateStringAttr(sn + "HtmlPrefix", string.Empty);
                    DefinitionPI.CreateStringAttr(sn + "HtmlPostfix", string.Empty);

                    switch (col.m_type)
                    {
                        case 2:
                            break;
                        case 3:
                            categoryColumn.CreateStringAttr("Type", "Float");
                            categoryColumn.CreateStringAttr("Format", ",0.##");
                            break;
                        default:
                            categoryColumn.CreateStringAttr("Type", "Text");
                            break;
                    }

                    if (sn.Equals(RPConstants.CONST_PRIORITY))
                    {
                        categoryColumn.CreateStringAttr("NumberSort", "1");
                    }

                    categoryColumn.CreateIntAttr("CanEdit", 0);
                    categoryColumn.CreateIntAttr("CanMove", 1);
                    categoryColumn.CreateIntAttr("CanSort", 1);
                    categoryColumn.CreateIntAttr("CaseSensitive", 0);

                    if (col.m_col_hidden == true)
                    {
                        categoryColumn.CreateIntAttr("Width", 0);
                    }

                    Header1.CreateStringAttr(sn, snv);
                    Header1.CreateIntAttr(sn + "SortIcons", 1);
                    Header2.CreateStringAttr(sn, GlobalConstants.Whitespace);
                }
            }
            catch (Exception ex)
            {
                WriteTrace(
                    Area.EPMLiveWorkEnginePPM,
                    Categories.EPMLiveWorkEnginePPM.Others,
                    TraceSeverity.VerboseEx,
                    ex.ToString());
            }

            return categoryColumn;
        }

        protected override string ResolvePeriodId(CPeriod periodData, int index)
        {
            return periodData.PeriodID.ToString();
        }

        protected override void AddPeriodColumns(IEnumerable<CPeriod> periods)
        {
            var index = 0;
            foreach (var period in periods)
            {
                var periodId = ResolvePeriodId(period, index++);
                var periodName = period.PeriodName;

                const string sumFunc = "(Row.id == 'Filter' ? '' : sum())";
                const string maxFunc = "(Row.id == 'Filter' ? '' : max())";
                const string minFunc = "(Row.id == 'Filter' ? '' : min())";

                var span = _displayList.Count;
                if (_useHeatMap)
                {
                    InitializePeriodHeatMapColumn(periodId, periodName, _mode != 3, sumFunc);
                    span *= 2;
                }

                var counter = 0;
                foreach (var displayRow in _displayList)
                {
                    try
                    {
                        counter++;
                        var prefix = $"P{periodId}C{counter}";
                        if (counter == 1)
                        {
                            if (_displayList.Count > 1)
                            {
                                Header1.CreateIntAttr($"P{periodId}C1Span", span);
                            }

                            Header1.CreateStringAttr(prefix, periodName);

                            var xC = CreateColumn(
                                PeriodCols,
                                prefix,
                                "Float",
                                canMove: false,
                                canResize: null,
                                canFilter: null);
                            xC.CreateStringAttr("Format", ",0.##");
                            xC.CreateStringAttr("Align", "Right");

                            if (_useHeatMap)
                            {
                                Header1.CreateStringAttr($"X{periodId}C{counter}", GlobalConstants.Whitespace);
                                Header1.CreateStringAttr($"Y{periodId}C{counter}", GlobalConstants.Whitespace);
                                Header2.CreateStringAttr($"X{periodId}C{counter}", periodName + displayRow.Name + "HeatMap");
                                Header2.CreateStringAttr($"Y{periodId}C{counter}", periodName + displayRow.Name + "HeatMap");

                                xC = CreateColumn(
                                    PeriodCols,
                                    $"X{periodId}C{counter}",
                                    "Float",
                                    visible: false,
                                    canMove: false,
                                    canResize: null,
                                    canFilter: null);
                                xC.CreateStringAttr("Format", ",0.##");
                                xC.CreateStringAttr("Align", "Right");
                                
                                xC = CreateColumn(
                                    PeriodCols,
                                    $"Y{periodId}C{counter}",
                                    "Float",
                                    visible: false,
                                    canMove: false,
                                    canResize: null,
                                    canFilter: null);
                                xC.CreateStringAttr("Format", ",0.##");
                                xC.CreateStringAttr("Align", "Right");
                            }

                            if (_mode != 3)
                            {
                                if (_useHeatMap)
                                {
                                    DefinitionRight.CreateStringAttr($"X{periodId}C{counter}Format", "##");
                                    DefinitionRight.CreateStringAttr($"X{periodId}C{counter}Formula", maxFunc);
                                    DefinitionRight.CreateStringAttr($"Y{periodId}C{counter}Format", "##");
                                    DefinitionRight.CreateStringAttr($"Y{periodId}C{counter}Formula", minFunc);
                                }

                                DefinitionRight.CreateStringAttr($"P{periodId}C{counter}Format", "##");
                                DefinitionRight.CreateStringAttr($"P{periodId}C{counter}Formula", sumFunc);
                            }

                            if (_useHeatMap)
                            {
                                DefinitionLeaf.CreateStringAttr($"X{periodId}C{counter}Formula", string.Empty);
                                DefinitionLeaf.CreateStringAttr($"Y{periodId}C{counter}Formula", string.Empty);
                                DefinitionPI.CreateStringAttr($"X{periodId}C{counter}Formula", string.Empty);
                                DefinitionPI.CreateStringAttr($"Y{periodId}C{counter}Formula", string.Empty);
                            }

                            DefinitionLeaf.CreateStringAttr($"P{periodId}C{counter}Formula", string.Empty);
                            DefinitionPI.CreateStringAttr($"P{periodId}C{counter}Formula", string.Empty);

                            xC.CreateIntAttr("MinWidth", 45);
                            xC.CreateIntAttr("Width", 65);
                        }
                        else
                        {
                            Header1.CreateStringAttr(prefix, GlobalConstants.Whitespace);
                        }

                        Header2.CreateStringAttr(prefix, displayRow.Name);
                        Header2.CreateStringAttr(prefix + "SortIcons", "0");
                    }
                    catch (Exception ex)
                    {
                        WriteTrace(
                          Area.EPMLiveWorkEnginePPM,
                          Categories.EPMLiveWorkEnginePPM.Others,
                          TraceSeverity.VerboseEx,
                          ex.ToString());
                    }
                }
            }
        }

        protected override bool CheckIfDetailRowShouldBeAdded(clsResFullDAta detailRow)
        {
            if (_doZeroRowCleverStuff)
            {
                if (_displayList.Count != 0)
                {
                    for (var i = 0; i < Periods.Count; i++)
                    {
                        try
                        {
                            foreach (var displayRow in _displayList)
                            {
                                if (GetDataValue(detailRow, displayRow.fid, _mode,  i, false, _useHeatMap ? _heatMapId : 0) != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteTrace(
                                Area.EPMLiveWorkEnginePPM,
                                Categories.EPMLiveWorkEnginePPM.Others,
                                TraceSeverity.VerboseEx,
                                ex.ToString());
                        }
                    }
                }

                return false;
            }

            return true;
        }

        protected override void AddDetailRow(clsResFullDAta detailRowData, int rowId)
        {
            var xIParent = Levels[0];
            var xI = xIParent.CreateSubStruct("I");
            Levels[1] = xI;
            LastDataRowNode = xI;

            xI.CreateStringAttr("id", rowId.ToString());
            xI.CreateStringAttr("Color", "white");
            xI.CreateStringAttr("Def", "Leaf");
            xI.CreateBooleanAttr("CanEdit", false);
            xI.CreateIntAttr("NoColorState", 1);
            xI.CreateIntAttr("rowid", rowId);
            xI.CreateBooleanAttr("rowidCanEdit", false);
            xI.CreateStringAttr("rtSelect", Convert.ToInt32(detailRowData.bSelected).ToString());
            xI.CreateBooleanAttr("rtSelectCanEdit", true);

            InitializeDataColumns((column, sn) =>
            {
                InitializeDetailRowDataColumn(detailRowData, xI, column, sn);
            });

            InitializeDetailRowDataStyling(detailRowData, xI);

            if (_displayTotalsDetails)
            {
                detailRowData.ProcessPITotals(_resourceValues.Periods.Count);

                var i = 0;
                foreach (var detailPiData in detailRowData.PerPItotals.Values)
                {
                    i++;
                    if (string.IsNullOrEmpty(detailPiData.ProjectName))
                    {
                        detailPiData.ProjectName = _resolvePiNameFunc(detailPiData.ProjectID);
                    }

                    if (CheckIfPiRowShouldBeAdded(detailPiData))
                    {
                        AddPIRow(detailRowData, detailPiData, i * 10000000 + i, i - 1, detailPiData.ProjectID);
                    }
                }
            }
        }

        private void InitializeDataColumns(Action<clsRXDisp, string> columnInitializationFunc)
        {
            foreach (var column in _columns.Where(column =>
                column.m_id == RPConstants.TGRID_TOTITEM_ID
                || !(_useRole && (column.m_id > 100 || column.m_id == RPConstants.TGRID_TOTDEPT_ID))
            ))
            {
                try
                {
                    var sn = column.m_id == RPConstants.TGRID_TOTRESRES_ID
                        ? "ResOrRole"
                        : RemoveCharacters(column.m_realname, " \r\n").Replace("/n", string.Empty);

                    columnInitializationFunc(column, sn);
                }
                catch (Exception ex)
                {
                    WriteTrace(
                        Area.EPMLiveWorkEnginePPM,
                        Categories.EPMLiveWorkEnginePPM.Others,
                        TraceSeverity.VerboseEx,
                        ex.ToString());
                }
            }
        }

        private void InitializeDetailRowDataColumn(clsResFullDAta detailRowData, CStruct xI, clsRXDisp column, string sn)
        {
            clsEPKItem epkItem = null;
            clsListItem listItem = null;
            clsCatItem categoryItem = null;
            switch (column.m_id)
            {
                case RPConstants.TGRID_TOTDEPT_ID:
                    if (!_useRole)
                    {
                        if (_resourceValues.Departments?.TryGetValue(
                            detailRowData.resavail.DeptID,
                            out epkItem) == true)
                        {
                            xI.CreateStringAttr(sn, epkItem.Name);
                        }
                    }
                    break;
                case RPConstants.TGRID_TOTRESRES_ID:
                    xI.CreateStringAttr(sn, detailRowData.ResOrRole);
                    break;
                case RPConstants.TGRID_TOTROLE_ID:
                    if (!_useRole)
                    {
                        if (_resourceValues.Roles?.TryGetValue(detailRowData.resavail.RoleID, out listItem) == true)
                        {
                            xI.CreateStringAttr(sn, listItem.Name);
                        }
                    }
                    break;
                case RPConstants.TGRID_TOTITEM_ID:
                    var piValue = string.Join(",", detailRowData.PIList.Values);
                    xI.CreateStringAttr(sn, piValue);
                    break;
                case RPConstants.TGRID_TOTCC_ID:
                    if (!_useRole)
                    {
                        if (_resourceValues.CostCategories?.TryGetValue(detailRowData.resavail.CostCat, out categoryItem) == true)
                        {
                            xI.CreateStringAttr(sn, categoryItem.Name);
                        }
                    }
                    break;
                case RPConstants.TGRID_TOTCCFULL_ID:
                    if (!_useRole)
                    {
                        if (_resourceValues.CostCategories?.TryGetValue(detailRowData.resavail.CostCatRole, out categoryItem) == true)
                        {
                            xI.CreateStringAttr(sn, categoryItem.FullName);
                        }
                    }
                    break;
                default:
                    if (_resourceValues.ResFields != null)
                    {
                        foreach (var field in _resourceValues.ResFields)
                        {
                            if (column.m_id == field.ID)
                            {
                                int j = 0;
                                string full;
                                RPConstants.GetCustValue(
                                    field.ID,
                                    detailRowData.resavail.CustomFields,
                                    out j,
                                    out full,
                                    _resourceValues);

                                xI.CreateStringAttr("x" + sn, full);
                                break;
                            }
                        }
                    }
                    break;
            }
        }

        private void InitializeDetailRowDataStyling(clsResFullDAta detailRowData, CStruct xI)
        {
            if (_displayList.Count == 0)
            {
                return;
            }

            var i = 0;
            var counter = _displayList.Count;
            foreach (var period in _resourceValues.Periods.Values)
            {
                i++;

                try
                {
                    string prefix;
                    var heatMapValue = 0d;
                    var rowValue = 0d;

                    if (_useHeatMap)
                    {
                        heatMapValue = GetDataValue(detailRowData, _heatMapId, _mode, i, true, 0);
                        var cellValue = heatMapValue.ToString(_numberFormat);
                        xI.CreateStringAttr($"P{period.PeriodID}H", cellValue);
                    }

                    var targetLevel = 0;
                    counter = 0;
                    foreach (var displayRow in _displayList)
                    {
                        ++counter;
                        prefix = $"P{period.PeriodID}C{counter}";

                        rowValue = GetDataValue(detailRowData, displayRow.fid, _mode, i, false, _useHeatMap ? _heatMapId : 0);
                        var cellValue = rowValue.ToString(_numberFormat);
                        xI.CreateStringAttr(prefix, cellValue);

                        if (_useHeatMap && displayRow.fid == 0)
                        {
                            rowValue = GetDataValue(detailRowData, displayRow.fid, _mode, i, true, _heatMapId);
                            heatMapValue = GetDataValue(detailRowData, _heatMapId, _mode, i, true, _heatMapId);

                            var rgb = RPAGridHelper.TargetBackground(rowValue, heatMapValue, _targetColors, out targetLevel, _heatFieldColor);

                            xI.CreateIntAttr($"C{period.PeriodID}C{counter}", targetLevel);
                            xI.CreateIntAttr($"Y{period.PeriodID}C{counter}", targetLevel > 0 ? targetLevel : 0);

                            if (!string.IsNullOrEmpty(rgb) && displayRow.fid != _heatMapId)
                            {
                                xI.CreateStringAttr(prefix + "Color", rgb);
                                xI.CreateStringAttr(prefix + "ExportStyle", "background-color: " + rgb);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace(
                        Area.EPMLiveWorkEnginePPM,
                        Categories.EPMLiveWorkEnginePPM.Others,
                        TraceSeverity.VerboseEx,
                        ex.ToString());
                }
            }
        }
        
        private double GetDataValue(clsResFullDAta oDet, int fid, int iMode, int i, bool bForHeatmap, int iHeatmapID)
        {
            double retval = 0;
            double vval = 0;
            double fval = 0;

            if (fid == 0)
            {
                vval = oDet.tot_Totals.getvarr(i);
                fval = oDet.tot_Totals.getftarr(i);
            }
            else if (fid == -1)
            {
                vval = oDet.tot_actual.getvarr(i);
                fval = oDet.tot_actual.getftarr(i);
            }
            else if (fid == -2)
            {
                vval = oDet.tot_proposal.getvarr(i);
                fval = oDet.tot_proposal.getftarr(i);
            }
            else if (fid == -3)
            {
                vval = oDet.tot_scheduled.getvarr(i);
                fval = oDet.tot_scheduled.getftarr(i);
            }
            else if (fid == -4)
            {
                vval = oDet.tot_committed.getvarr(i);
                fval = oDet.tot_committed.getftarr(i);
            }
            else if (fid == -5)
            {
                vval = oDet.tot_personel.getvarr(i);
                fval = oDet.tot_personel.getftarr(i);
            }
            else if (fid == -6)
            {
                vval = oDet.tot_avail.getvarr(i);
                fval = oDet.tot_avail.getftarr(i);
            }
            else if (fid == -7)
            {
                if (bForHeatmap)
                {
                    vval = oDet.tot_Totals.getvarr(i);
                    fval = oDet.tot_Totals.getftarr(i);
                }
                else
                {
                    vval = oDet.tot_avail.getvarr(i) - oDet.tot_Totals.getvarr(i);
                    fval = oDet.tot_avail.getftarr(i) - oDet.tot_Totals.getftarr(i);
                }
            }
            else if (fid == -8)
            {
                if (iHeatmapID == 0 || iHeatmapID > oDet.CapScen.Count)
                {
                    vval = -oDet.tot_Totals.getvarr(i);
                    fval = -oDet.tot_Totals.getftarr(i);
                }
                else if (iHeatmapID <= -1 && iHeatmapID >= -6)
                {
                    if (iHeatmapID == -1)
                    {
                        vval = oDet.tot_actual.getvarr(i);
                        fval = oDet.tot_actual.getftarr(i);
                    }
                    else if (iHeatmapID == -2)
                    {
                        vval = oDet.tot_proposal.getvarr(i);
                        fval = oDet.tot_proposal.getftarr(i);
                    }
                    else if (iHeatmapID == -3)
                    {
                        vval = oDet.tot_scheduled.getvarr(i);
                        fval = oDet.tot_scheduled.getftarr(i);
                    }
                    else if (iHeatmapID == -4)
                    {
                        vval = oDet.tot_committed.getvarr(i);
                        fval = oDet.tot_committed.getftarr(i);
                    }
                    else if (iHeatmapID == -5)
                    {
                        vval = oDet.tot_personel.getvarr(i);
                        fval = oDet.tot_personel.getftarr(i);
                    }
                    else if (iHeatmapID == -6)
                    {
                        vval = oDet.tot_avail.getvarr(i);
                        fval = oDet.tot_avail.getftarr(i);
                    }

                    vval -= oDet.tot_Totals.getvarr(i);
                    fval -= oDet.tot_Totals.getftarr(i);
                }

                else if (bForHeatmap)
                {
                    vval = oDet.tot_Totals.getvarr(i);
                    fval = oDet.tot_Totals.getftarr(i);
                }
                else
                {
                    vval = oDet.CapScen[iHeatmapID - 1].getvarr(i) - oDet.tot_Totals.getvarr(i);
                    fval = oDet.CapScen[iHeatmapID - 1].getftarr(i) - oDet.tot_Totals.getftarr(i);
                }
            }
            else if (fid > 0 && fid <= oDet.CapScen.Count)
            {
                vval = oDet.CapScen[fid - 1].getvarr(i);
                fval = oDet.CapScen[fid - 1].getftarr(i);
            }

            if (iMode == 3)
            {
                if (fval == 0)
                    retval = 0;
                else
                    retval = (vval * 100) / fval;
            }
            else if (iMode == 0)
                retval = vval;
            else
                retval = fval;

            if (iMode == 1)
                retval /= 100;

            return retval;
        }

        private bool CheckIfPiRowShouldBeAdded(clsResXData detailRow)
        {
            if (_doZeroRowCleverStuff)
            {
                if (_displayList.Count != 0)
                {
                    for (var i = 0; i < Periods.Count; i++)
                    {
                        try
                        {
                            foreach (var displayRow in _displayList)
                            {
                                if (GetPIDataValue(detailRow, i) != 0)
                                {
                                    return true;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            WriteTrace(
                                Area.EPMLiveWorkEnginePPM,
                                Categories.EPMLiveWorkEnginePPM.Others,
                                TraceSeverity.VerboseEx,
                                ex.ToString());
                        }
                    }
                }

                return false;
            }

            return true;
        }

        public void AddPIRow(clsResFullDAta detailRowData, clsResXData piData, int rID, int xLi, int projectId)
        {
            var xIParent = Levels[0];
            var xI = LastDataRowNode.CreateSubStruct("I");
            Levels[2] = xI;

            xI.CreateStringAttr("id", rID.ToString());
            xI.CreateStringAttr("Color", "white");
            xI.CreateStringAttr("Def", "PI");
            xI.CreateBooleanAttr("CanEdit", false);
            xI.CreateIntAttr("NoColorState", 1);
            xI.CreateIntAttr("rowid", rID);
            xI.CreateBooleanAttr("rowidCanEdit", false);
            xI.CreateStringAttr("rtSelectType", "Text");
            xI.CreateStringAttr("rtSelect", " ");
            xI.CreateBooleanAttr("rtSelectCanEdit", false);
            
            InitializeDataColumns((column, sn) =>
            {
                InitializePiDataColumn(piData, xI, column, sn);
            });

            if (_displayList.Count == 0)
            {
                return;
            }

            var i = 0;
            foreach (var period in _resourceValues.Periods.Values)
            {
                try
                {
                    i++;
                    string prefix;
                    var counter = 0;

                    foreach (var displayRow in _displayList)
                    {
                        counter++;
                        prefix = $"P{period.PeriodID}C{counter}";

                        if (displayRow.fid <= 0)
                        {
                            var piDataValue = GetPIDataValue(detailRowData, piData, i, displayRow.fid, xLi, projectId);
                            var cellValue = _mode == 0
                                ? piDataValue.ToString("0.##")
                                : piDataValue.ToString("0.###");

                            xI.CreateStringAttr(prefix, cellValue);

                            if (_useHeatMap && displayRow.fid <= 0)
                            {
                                int targetLevel;
                                var rgb = RPAGridHelper.TargetBackground(
                                    piDataValue, 
                                    piDataValue, 
                                    _targetColors, 
                                    out targetLevel, 
                                    _heatFieldColor);

                                xI.CreateIntAttr($"X{period.PeriodID}C{counter}", targetLevel);
                                xI.CreateIntAttr($"Y{period.PeriodID}C{counter}", targetLevel > 0 ? targetLevel : 0);

                                if (!string.IsNullOrEmpty(rgb) && displayRow.fid != _heatMapId)
                                {
                                    xI.CreateStringAttr(prefix + "Color", rgb);
                                    xI.CreateStringAttr(prefix + "ExportStyle", "background-color: " + rgb);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    WriteTrace(
                        Area.EPMLiveWorkEnginePPM,
                        Categories.EPMLiveWorkEnginePPM.Others,
                        TraceSeverity.VerboseEx,
                        ex.ToString());
                }
            }
        }

        private void InitializePiDataColumn(clsResXData piData, CStruct xI, clsRXDisp column, string sn)
        {
            switch (column.m_id)
            {
                case RPConstants.TGRID_TOTDEPT_ID:
                case RPConstants.TGRID_TOTRESRES_ID:
                case RPConstants.TGRID_TOTROLE_ID:
                case RPConstants.TGRID_TOTCC_ID:
                case RPConstants.TGRID_TOTCCFULL_ID:
                    break;
                case RPConstants.TGRID_TOTITEM_ID:
                    xI.CreateStringAttr(sn, piData.ProjectName);
                    break;
                default:
                    break;
            }
        }

        private double GetPIDataValue(clsResXData piData, int i)
        {
            var workHours = piData.getvarr(i);
            var fteValue = piData.getftarr(i);
            return ComputePIValue(workHours, fteValue);
        }

        private double GetPIDataValue(clsResFullDAta detailRowData, clsResXData piData, int i, int fieldId, int idx, int projectId)
        {
            //All the calculation performd for Show Totals section is based on following link:
            //https://upland.screenstepslive.com/s/EPMLive2013/m/UserGuide/l/147531-how-do-i-use-the-total-column-feature-within-the-resource-analyzer
            double workHours = 0;
            double fteValue = 0;

            try
            {
                switch (fieldId)
                {
                    case 0:
                        workHours = piData.getvarr(i);
                        fteValue = piData.getftarr(i);
                        break;
                    case -1:
                        //Actual Work equals Timesheet Actuals entered.
                        ComputePIValues(detailRowData.actual, projectId, i, -1, -1, ref workHours, ref fteValue);
                        break;
                    case -2:
                        //Proposed Work equals resource planned work that has not yet been committed.
                        ComputePIValues(detailRowData.proposal, projectId, i, -1, -1, ref workHours, ref fteValue);
                        break;
                    case -3:
                        //Scheduled Work equals the work allocation pulled in from the designated work lists, such as Task Center, Issues, Risks, etc..
                        ComputePIValues(detailRowData.scheduled, projectId, i, idx, -1, ref workHours, ref fteValue);
                        break;
                    case -4:
                        //Committed Work equals the hours in resource plans that have been committed.
                        ComputePIValues(detailRowData.committed, projectId, i, idx, -1, ref workHours, ref fteValue);
                        break;
                    case -5:
                        //Personal Time Off pulls in the hours entered into the Time Off Requests.
                        ComputePIValues(detailRowData.personel, projectId, i, idx, -1, ref workHours, ref fteValue);
                        break;
                    case -6:
                        //Availability equals the number of work hours each resource is available for each calendar period (monthly/weekly/quarterly/etc.). 
                        //It is important to note that this is resource specific based on each resource’s work hours schedule.
                        workHours = detailRowData.tot_avail.getvarr(i);
                        fteValue = detailRowData.tot_avail.getftarr(i);
                        break;
                    case -7:
                        //Remaining Availability equals Availability minus any committed and scheduled work.
                        double totWrkHrs = 0, totFTEHrs = 0;
                        double avlWrkHrs = 0, avlFTEHrs = 0;

                        if (piData.ProjectID == projectId)
                        {
                            if (_useRole)
                            {
                                avlWrkHrs = detailRowData.tot_avail.getvarr(i);
                                avlFTEHrs = detailRowData.tot_avail.getftarr(i);
                            }
                            else
                            {
                                if (detailRowData.avail.Count > 0)
                                {
                                    idx = 0;
                                    for (int counter = 0; counter < detailRowData.avail.Count; counter++)
                                    {
                                        if (detailRowData.avail[counter].ProjectID == projectId)
                                        {
                                            idx = counter;
                                            break;
                                        }
                                    }
                                    if (idx == -1)
                                    {
                                        avlWrkHrs = avlFTEHrs = 0;
                                    }
                                    else
                                    {
                                        avlWrkHrs = Convert.ToDouble(detailRowData.avail[idx].WrkHours[i]);
                                        avlFTEHrs = Convert.ToDouble(detailRowData.avail[idx].FTEVals[i]);
                                    }
                                }
                            }

                            totWrkHrs = piData.getvarr(i);
                            totFTEHrs = piData.getftarr(i);

                            workHours = avlWrkHrs - totWrkHrs;
                            fteValue = avlFTEHrs - totFTEHrs;
                        }
                        break;
                    default:
                        if (fieldId > 0 && fieldId <= detailRowData.CapScen.Count)
                        {
                            workHours = detailRowData.CapScen[fieldId - 1].getvarr(i);
                            fteValue = detailRowData.CapScen[fieldId - 1].getftarr(i);
                        }
                        break;
                }

                return ComputePIValue(workHours, fteValue);
            }
            catch (Exception ex)
            {
                WriteTrace(
                    Area.EPMLiveWorkEnginePPM,
                    Categories.EPMLiveWorkEnginePPM.Others,
                    TraceSeverity.VerboseEx,
                    ex.ToString());

                return 0;
            }
        }

        private double ComputePIValue(double vval, double fval)
        {
            double result;
            switch (_mode)
            {
                case 3:
                    result = fval != 0
                        ? (vval * 100) / fval
                        : 0;
                    break;
                case 1:
                    result = fval / 100;
                    break;
                case 0:
                    result = vval;
                    break;
                default:
                    result = fval;
                    break;
            }

            return result;
        }

        private void ComputePIValues(
            IList<clsResXData> dataRecords, 
            int projectId, 
            int index, 
            int idxWithRole, 
            int idxWithoutRole,
            ref double workHours, 
            ref double fteValue)
        {
            if (_useRole)
            {
                ComputePIValuesWithRole(dataRecords, projectId, index, idxWithRole, ref workHours, ref fteValue);
            }
            else
            {
                ComputePIValuesWithoutRole(dataRecords, projectId, index, idxWithoutRole, ref workHours, ref fteValue);
            }
        }

        private static void ComputePIValuesWithoutRole(IList<clsResXData> dataRecords, int projectId, int index, int idx, ref double workHours, ref double fteValues)
        {
            for (int counter = 0; counter < dataRecords.Count; counter++)
            {
                if (dataRecords[counter].ProjectID == projectId)
                {
                    idx = counter;
                    break;
                }
            }
            if (idx == -1)
            {
                workHours = fteValues = 0;
            }
            else
            {
                workHours = Convert.ToDouble(dataRecords[idx].WrkHours[index]);
                fteValues = Convert.ToDouble(dataRecords[idx].FTEVals[index]);
            }
        }

        private static void ComputePIValuesWithRole(IList<clsResXData> dataRecords, int projectId, int index, int idx, ref double workHours, ref double fteValues)
        {
            for (int counter = 0; counter < dataRecords.Count; counter++)
            {
                if (dataRecords[counter].ProjectID == projectId)
                {
                    idx = counter;
                    workHours += Convert.ToDouble(dataRecords[idx].WrkHours[index]);
                    fteValues += Convert.ToDouble(dataRecords[idx].FTEVals[index]);
                }
            }
            if (idx == -1)
            {
                workHours = fteValues = 0;
            }
        }
    }
}
