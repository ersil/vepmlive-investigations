﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CostDataValues;
using EPMLiveCore;
using EPMLiveCore.Infrastructure.Logging;
using Microsoft.SharePoint.Administration;
using PortfolioEngineCore;
using static EPMLiveCore.Infrastructure.Logging.LoggingService;

namespace CADataCache
{
    internal class CATopGridTemp : CADataCacheGridBase
    {
        public CATopGridTemp(
            bool showFTEs, 
            bool useQuantity,
            bool useCost, 
            bool showCostDetailed,
            int pmoAdmin,
            IList<CATGRow> displayList,
            IList<clsColDisp> columns) 
        : base(showFTEs, useQuantity, useCost, showCostDetailed, pmoAdmin, displayList, columns)
        {
        }

        protected override void InitializeGridLayout(GridRenderingTypes renderingType)
        {
            InitializeGridLayoutConfig();

            var xLeftCols = Constructor.CreateSubStruct("LeftCols");
            var xCols = Constructor.CreateSubStruct("Cols");
            var xRightCols = Constructor.CreateSubStruct("RightCols");
            PeriodCols = xRightCols;
            MiddleCols = xCols;

            Definitions = Constructor.CreateSubStruct("Def");

            DefinitionRight = Definitions.CreateSubStruct("D");
            DefinitionRight.CreateStringAttr("Name", "R");
            DefinitionRight.CreateStringAttr("Calculated", "1");
            DefinitionRight.CreateStringAttr("Calculated", "1");
            DefinitionRight.CreateStringAttr("HoverCell", "Color");
            DefinitionRight.CreateStringAttr("HoverRow", "Color");
            DefinitionRight.CreateStringAttr("FocusCell", string.Empty);
            DefinitionRight.CreateStringAttr("HoverCell", "Color");
            DefinitionRight.CreateIntAttr("NoColorState", 1);
            DefinitionRight.CreateStringAttr("OnFocus", "ClearSelection+Grid.SelectRow(Row,!Row.Selected)");
            DefinitionRight.CreateBooleanAttr("SelectCanEdit", true);
            DefinitionRight.CreateStringAttr("rowid", string.Empty);

            DefinitionLeaf = Definitions.CreateSubStruct("D");
            DefinitionLeaf.CreateStringAttr("Name", "Leaf");
            DefinitionLeaf.CreateStringAttr("Calculated", "0");
            DefinitionLeaf.CreateStringAttr("HoverCell", "Color");
            DefinitionLeaf.CreateStringAttr("HoverRow", "Color");
            DefinitionLeaf.CreateStringAttr("FocusCell", string.Empty);
            DefinitionLeaf.CreateStringAttr("HoverCell", "Color");
            DefinitionLeaf.CreateStringAttr("OnFocus", "ClearSelection+Grid.SelectRow(Row,!Row.Selected)");
            DefinitionLeaf.CreateIntAttr("NoColorState", 1);

            var xHead = Constructor.CreateSubStruct("Head");
            var xFilter = xHead.CreateSubStruct("Filter");
            xFilter.CreateStringAttr("id", "Filter");

            Header1 = Constructor.CreateSubStruct("Header");
            Header1.CreateIntAttr("PortfolioItemVisible", 1);
            Header1.CreateIntAttr("Spanned", 1);
            Header1.CreateIntAttr("SortIcons", 2);
            Header1.CreateStringAttr("HoverCell", "Color");
            Header1.CreateStringAttr("HoverRow", string.Empty);
            Header1.CreateStringAttr("RowSel", GlobalConstants.Whitespace);
            Header1.CreateStringAttr("Select", GlobalConstants.Whitespace);

            Header2 = xHead.CreateSubStruct("Header");
            Header2.CreateIntAttr("PortfolioItemVisible", 1);
            Header2.CreateIntAttr("Spanned", -1);
            Header2.CreateIntAttr("SortIcons", 0);
            Header2.CreateStringAttr("HoverCell", "Color");
            Header2.CreateStringAttr("HoverRow", string.Empty);
            Header2.CreateStringAttr("RowSel", GlobalConstants.Whitespace);
            Header2.CreateStringAttr("Select", GlobalConstants.Whitespace);

            InitializeGridLayoutCategoryColumns(xLeftCols);

            var xSolid = Constructor.CreateSubStruct("Solid");
            var xGroup = xSolid.CreateSubStruct("Group");

            foreach (var column in _columns)
            {
                try
                {
                    InitializeDisplayColumn(column);
                }
                catch (Exception ex)
                {
                    LoggingService.WriteTrace(
                       Area.EPMLiveWorkEnginePPM,
                       Categories.EPMLiveWorkEnginePPM.Others,
                       TraceSeverity.VerboseEx,
                       ex.ToString());
                }
            }
        }

        private void InitializeGridLayoutConfig()
        {
            var xToolbar = Constructor.CreateSubStruct("Toolbar");
            xToolbar.CreateIntAttr("Visible", 0);

            var xPanel = Constructor.CreateSubStruct("Panel");
            xPanel.CreateIntAttr("Visible", 0);
            xPanel.CreateIntAttr("Select", 0);
            xPanel.CreateIntAttr("Delete", 0);
            xPanel.CreateIntAttr("CanHide", 0);
            xPanel.CreateIntAttr("CanSelect", 0);

            var xCfg = Constructor.CreateSubStruct("Cfg");
            xCfg.CreateStringAttr("MainCol", "zXPortfolioItem");
            xCfg.CreateStringAttr("Code", "GTACCNPSQEBSLC");
            xCfg.CreateIntAttr("SuppressCfg", 3);
            xCfg.CreateIntAttr("SuppressMessage", 3);
            xCfg.CreateIntAttr("Dragging", _pmoAdmin);
            xCfg.CreateIntAttr("Sorting", 1);
            xCfg.CreateIntAttr("ColsMoving", 1);
            xCfg.CreateIntAttr("ColsPosLap", 1);
            xCfg.CreateIntAttr("ColsLap", 1);
            xCfg.CreateIntAttr("VisibleLap", 1);
            xCfg.CreateIntAttr("SectionWidthLap", 1);
            xCfg.CreateIntAttr("GroupLap", 1);
            xCfg.CreateIntAttr("WideHScroll", 0);
            xCfg.CreateIntAttr("LeftWidth", 150);
            xCfg.CreateIntAttr("Width", 400);
            xCfg.CreateIntAttr("RightWidth", 800);
            xCfg.CreateIntAttr("MinMidWidth", 50);
            xCfg.CreateIntAttr("MinRightWidth", 400);
            xCfg.CreateIntAttr("LeftCanResize", 0);
            xCfg.CreateIntAttr("RightCanResize", 1);
            xCfg.CreateIntAttr("FocusWholeRow", 1);
            xCfg.CreateIntAttr("MaxHeight", 0);
            xCfg.CreateIntAttr("ShowDeleted", 0);
            xCfg.CreateBooleanAttr("DateStrings", true);
            xCfg.CreateIntAttr("MaxWidth", 1);
            xCfg.CreateIntAttr("MaxSort", 2);
            xCfg.CreateIntAttr("AppendId", 0);
            xCfg.CreateIntAttr("FullId", 0);
            xCfg.CreateStringAttr("IdChars", "0123456789");
            xCfg.CreateIntAttr("NumberId", 1);
            xCfg.CreateIntAttr("LastId", 1);
            xCfg.CreateIntAttr("CaseSensitiveId", 0);
            xCfg.CreateStringAttr("Style", "GM");
            xCfg.CreateStringAttr("CSS", "ResPlanAnalyzer");
            xCfg.CreateIntAttr("FastColumns", 1);
            xCfg.CreateIntAttr("ExpandAllLevels", 3);
            xCfg.CreateIntAttr("GroupSortMain", 1);
            xCfg.CreateIntAttr("GroupRestoreSort", 1);
            xCfg.CreateIntAttr("NoTreeLines", 1);
            xCfg.CreateIntAttr("ShowVScroll", 1);
        }

        private void InitializeGridLayoutCategoryColumns(CStruct columnsContainer)
        {
            CStruct column;

            column = CreateColumn(columnsContainer, "RowSel", "Icon",
                width: 20,
                canEdit: false,
                canMove: false,
                canExport: false);
            column.CreateStringAttr("Color", "rgb(223, 227, 232)");

            column = CreateColumn(columnsContainer, "rowid", "Text",
                visible: false,
                canExport: false);
            column.CreateStringAttr("Color", "rgb(223, 227, 232)");

            column = CreateColumn(columnsContainer, "Select", "Bool",
                width: 20,
                canEdit: true,
                canMove: false,
                canExport: false);
            column.CreateStringAttr("Class", string.Empty);
        }

        private void InitializeDisplayColumn(clsColDisp column)
        {
            CStruct categoryColumn;

            var realName = "zX" + CleanupString(column.m_realname);
            var displayName = column.m_dispname.Replace("/n", "\n");

            Header1.CreateStringAttr(realName, displayName);
            Header2.CreateStringAttr(realName, GlobalConstants.Whitespace);

            categoryColumn = CreateColumn(MiddleCols, realName,
                visible: column.m_def_fld,
                canEdit: false,
                canMove: true,
                canResize: null,
                canFilter: null,
                canHide: column.m_unselectable,
                canSelect: null);
            categoryColumn.CreateStringAttr("Class", "GMCellMain");
            categoryColumn.CreateIntAttr("CanDrag", 0);
            categoryColumn.CreateIntAttr("CaseSensitive", 0);
            categoryColumn.CreateStringAttr("OnDragCell", "Focus,DragCell");
            if (column.m_col_hidden)
            {
                categoryColumn.CreateIntAttr("Width", 0);
            }
            switch (column.m_type)
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

            DefinitionRight.CreateIntAttr(realName + "CanDrag", 0);
            DefinitionRight.CreateStringAttr(realName + "HtmlPrefix", "<B>");
            DefinitionRight.CreateStringAttr(realName + "HtmlPostfix", "</B>");
            DefinitionLeaf.CreateIntAttr(realName + "CanDrag", 0);
            DefinitionLeaf.CreateStringAttr(realName + "HtmlPrefix", string.Empty);
            DefinitionLeaf.CreateStringAttr(realName + "HtmlPostfix", string.Empty);

            const string sMaxFunc = "(Row.id == 'Filter' ? '' : max())";
            const string sMinFunc = "(Row.id == 'Filter' ? '' : min())";

            categoryColumn = CreateColumn(MiddleCols, "xinterenalPeriodMin", "Int",
                visible: false,
                canMove: false,
                canResize: null,
                canFilter: null
            );
            categoryColumn.CreateIntAttr("CanDrag", 0);
            DefinitionRight.CreateStringAttr("xinterenalPeriodMin" + "Formula", sMinFunc);
            DefinitionRight.CreateIntAttr("xinterenalPeriodMin" + "CanDrag", 0);
            DefinitionLeaf.CreateStringAttr("xinterenalPeriodMin" + "Formula", string.Empty);
            DefinitionLeaf.CreateIntAttr("xinterenalPeriodMin" + "CanDrag", 0);

            categoryColumn = CreateColumn(MiddleCols, "xinterenalPeriodMax", "Int",
                visible: false,
                canMove: false,
                canResize: null,
                canFilter: null
            );
            categoryColumn.CreateStringAttr("Align", "Right");
            categoryColumn.CreateIntAttr("CanDrag", 0);
            DefinitionRight.CreateStringAttr("xinterenalPeriodMax" + "Formula", sMaxFunc);
            DefinitionRight.CreateIntAttr("xinterenalPeriodMax" + "CanDrag", 0);
            DefinitionLeaf.CreateStringAttr("xinterenalPeriodMax" + "Formula", string.Empty);
            DefinitionLeaf.CreateIntAttr("xinterenalPeriodMax" + "CanDrag", 0);

            categoryColumn = CreateColumn(MiddleCols, "xinterenalPeriodTotal", "Int",
                visible: false,
                canMove: false,
                canResize: null,
                canFilter: null
            );
            categoryColumn.CreateStringAttr("Align", "Right");
            categoryColumn.CreateIntAttr("CanDrag", 0);
            DefinitionLeaf.CreateIntAttr("xinterenalPeriodMax" + "CanDrag", 0);
            DefinitionRight.CreateIntAttr("xinterenalPeriodMax" + "CanDrag", 0);
        }

        protected override string ResolvePeriodId(clsPeriodData periodData, int index)
        {
            return periodData.PeriodID.ToString();
        }

        protected override void AddPeriodColumns(IEnumerable<clsPeriodData> periods)
        {
            var index = 0;
            foreach (var period in periods)
            {
                var periodId = ResolvePeriodId(period, index++);
                var periodName = period.PeriodName;

                var counter = _displayList.Where(pred => pred.bUse).Count();
                if (counter == 0)
                {
                    return;
                }

                var span = (_useQuantity ? 1 : 0)
                        + (_showFTEs ? 1 : 0)
                        + (_useCost ? 1 : 0);

                span *= counter;
                counter = 0;

                var prefix = string.Empty;
                foreach (var displayRow in _displayList)
                {
                    try
                    {
                        if (displayRow.bUse)
                        {
                            ++counter;
                            prefix = "C";
                            var attributePrefix = "P" + periodId + prefix + counter;

                            if (counter == 1)
                            {
                                if (span > 1)
                                {
                                    Header1.CreateIntAttr(attributePrefix + "Span", span);
                                }
                                Header1.CreateStringAttr(attributePrefix, periodName);
                            }
                            else
                            {
                                Header1.CreateStringAttr(attributePrefix, GlobalConstants.Whitespace);
                            }
                            
                            if (_useQuantity)
                            {
                                Header2.CreateStringAttr(attributePrefix, "Qty");
                                DefinePeriodColumn(attributePrefix, ",0.##", ",0.##");
                                ++counter;
                            }

                            if (_showFTEs)
                            {
                                Header2.CreateStringAttr(attributePrefix, "FTE");
                                DefinePeriodColumn(attributePrefix, ",0.###", ",0.##");
                                ++counter;
                            }

                            if (_useCost)
                            {
                                Header2.CreateStringAttr(attributePrefix, "Cost");
                                DefinePeriodColumn(attributePrefix, null, ",0.###");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LoggingService.WriteTrace(
                          Area.EPMLiveWorkEnginePPM,
                          Categories.EPMLiveWorkEnginePPM.Others,
                          TraceSeverity.VerboseEx,
                          ex.ToString());
                    }
                }
            }
        }

        private CStruct DefinePeriodColumn(string attributePrefix, string columnFormat, string definitionFormat)
        {
            var column = CreateColumn(PeriodCols, attributePrefix, "Float",
                    canMove: false,
                    canResize: null,
                    canFilter: null);

            if (columnFormat != null)
            {
                column.CreateStringAttr("Format", columnFormat);
            }

            column.CreateIntAttr("CanDrag", _pmoAdmin);
            column.CreateStringAttr("Align", "Right");

            if (_pmoAdmin != 0)
            {
                column.CreateStringAttr("OnDragCell", "Focus,DragCell");
            }

            column.CreateIntAttr("MinWidth", 45);
            column.CreateIntAttr("Width", 65);

            const string sFunc = "(Row.id == 'Filter' ? '' : sum())";
            DefinitionRight.CreateStringAttr(attributePrefix + "Formula", sFunc);
            DefinitionRight.CreateStringAttr(attributePrefix + "Format", definitionFormat);
            DefinitionRight.CreateIntAttr(attributePrefix + "CanDrag", _pmoAdmin);
            DefinitionRight.CreateStringAttr(attributePrefix + "ClassInner", string.Empty);

            DefinitionLeaf.CreateStringAttr(attributePrefix + "Formula", string.Empty);
            DefinitionLeaf.CreateIntAttr(attributePrefix + "CanDrag", _pmoAdmin);
            DefinitionLeaf.CreateStringAttr(attributePrefix + "ClassInner", string.Empty);
            return column;
        }

        protected override bool CheckIfDetailRowShouldBeAdded(clsDetailRowData detailRow)
        {
            throw new NotImplementedException();
        }
        protected override void AddDetailRow(clsDetailRowData detailRowData, int rowId)
        {
            throw new NotImplementedException();
        }

        protected override void InitializeGridData(GridRenderingTypes renderingType)
        {
            throw new NotImplementedException();
        }
    }
}
