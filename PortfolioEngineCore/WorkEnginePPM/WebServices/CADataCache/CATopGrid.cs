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
    internal class CATopGrid : CADataCacheGridBase
    {
        private readonly bool _hideRowsWithAllZeros;

        public CATopGrid(
            bool hideRowsWithAllZeros,
            bool showFTEs, 
            bool useQuantity,
            bool useCost, 
            bool showCostDetailed,
            int pmoAdmin,
            IList<CATGRow> displayList,
            IList<clsColDisp> columns) 
        : base(showFTEs, useQuantity, useCost, showCostDetailed, pmoAdmin, displayList, columns, true)
        {
            _hideRowsWithAllZeros = hideRowsWithAllZeros;
        }

        protected override void InitializeGridLayoutCategoryColumns(CStruct columnsContainer)
        {
            var column = columnsContainer.CreateSubStruct("C");

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

        protected override int CalculatePeriodColumnsSpan(string periodId, string periodName, int counter)
        {
            var span = (_useQuantity ? 1 : 0)
                        + (_showFTEs ? 1 : 0)
                        + (_useCost ? 1 : 0);

            span *= counter;
            return span;
        }

        protected override void InitializePeriodDisplayRow(string periodId, string periodName, int counter, CATGRow displayRow)
        {
            if (_useQuantity)
            {
                Header2.CreateStringAttr(GeneratePeriodAttributeName("P", periodId, counter), "Qty");
                DefinePeriodColumn(GeneratePeriodAttributeName("P", periodId, counter), ",0.##", ",0.##");
                ++counter;
            }

            if (_showFTEs)
            {
                Header2.CreateStringAttr(GeneratePeriodAttributeName("P", periodId, counter), "FTE");
                DefinePeriodColumn(GeneratePeriodAttributeName("P", periodId, counter), ",0.###", ",0.##");
                ++counter;
            }

            if (_useCost)
            {
                Header2.CreateStringAttr(GeneratePeriodAttributeName("P", periodId, counter), "Cost");
                DefinePeriodColumn(GeneratePeriodAttributeName("P", periodId, counter), null, ",0.###");
            }
        }

        protected override bool CheckIfDetailRowShouldBeAdded(clsDetailRowData detailRow)
        {
            if (!_hideRowsWithAllZeros)
            {
                return true;
            }

            var quantityValue = 0d;
            var costValue = 0d;
            var fteValue = 0d;

            var totalPeriods = detailRow.zFTE.Length - 1;
            for (var i = 1; i <= totalPeriods; i++)
            {
                costValue = 0;
                quantityValue = 0;
                fteValue = 0;

                // (CC-76588, 2018-07-26) Could be simplified to 2-3 lines if we knew that values can not be negative.
                // Without knowing the idea behind the code, it's better to avoid refactoring logic

                if (_useQuantity && detailRow.zValue[i] != double.MinValue)
                {
                    quantityValue = detailRow.zValue[i];
                }

                if (_showFTEs && detailRow.zFTE[i] != double.MinValue)
                {
                    fteValue = detailRow.zFTE[i];
                }

                if (_useCost)
                {
                    costValue = _showCostDetailed ? detailRow.zCost[i] : Math.Round(detailRow.zCost[i]);
                }

                if (costValue != 0 || quantityValue != 0 || fteValue != 0)
                {
                    break;
                }
            }

            return costValue + quantityValue + fteValue > 0;
        }

        protected override void AddDetailRow(clsDetailRowData detailRowData, int rowId)
        {
            CStruct xIParent = Levels[0];
            CStruct xI = xIParent.CreateSubStruct("I");

            Levels[1] = xI;
            xI.CreateStringAttr("id", rowId.ToString());
            xI.CreateStringAttr("rowid", "r" + rowId.ToString());
            xI.CreateStringAttr("Color", "white");
            xI.CreateStringAttr("Def", "Leaf");
            xI.CreateIntAttr("NoColorState", 1);
            xI.CreateBooleanAttr("CanEdit", false);
            xI.CreateStringAttr("Select", (detailRowData.bSelected ? "1" : "0"));
            xI.CreateBooleanAttr("SelectCanEdit", true);

            foreach (var column in _columns)
            {
                var attributeName = "zX" + CleanUpString(column.m_realname);

                string value;
                if (TryGetDataFromDetailRowDataField(detailRowData, column.m_id, out value))
                {
                    // (CC-76681, 2018-07-13) Additional condition, specific to TopGrid
                    if (value == GlobalConstants.Whitespace)
                    {
                        if (column.m_id >= (int)FieldIDs.PI_USE_EXTRA + 1 && column.m_id <= (int)FieldIDs.PI_USE_EXTRA + (int)FieldIDs.MAX_PI_EXTRA)
                        {
                            if (detailRowData.m_PI_Format_Extra_data != null)
                            {
                                value = detailRowData.m_PI_Format_Extra_data[column.m_id - (int)FieldIDs.PI_USE_EXTRA];
                            }
                            else
                            {
                                continue;
                            }
                        }
                    }

                    xI.CreateStringAttr(attributeName, value);
                }
            }

            var periodTotal = detailRowData.zFTE.Length - 1;

            var periodMin = CalculateInternalPeriodMin(detailRowData);
            var periodMax = 0;
            if (periodMin != 0)
            {
                periodMax = CalculateInternalPeriodMax(detailRowData);
            }
            else
            {
                periodMin = periodTotal + 1;
            }
            
            xI.CreateIntAttr("xinterenalPeriodMin", periodMin);
            xI.CreateIntAttr("xinterenalPeriodMax", periodMax);
            xI.CreateIntAttr("xinterenalPeriodTotal", periodTotal);
            
            for (int i = 1; i <= periodTotal; i++)
            {
                UpdateDisplayRowsWithPeriodData(detailRowData, xI, i);
            }
        }

        private void UpdateDisplayRowsWithPeriodData(clsDetailRowData detailRowData, CStruct xI, int i)
        {
            var count = 0;
            foreach (var displayRow in _displayList)
            {
                if (displayRow.bUse)
                {
                    ++count;
                    var attributeName = "P" + i.ToString() + "C" + count;

                    if (_useQuantity)
                    {
                        if (detailRowData.zValue[i] != double.MinValue)
                        {
                            xI.CreateDoubleAttr(attributeName, detailRowData.zValue[i]);
                        }

                        ++count;
                    }

                    if (_showFTEs)
                    {
                        if (detailRowData.zFTE[i] != double.MinValue)
                        {
                            xI.CreateDoubleAttr(attributeName, detailRowData.zFTE[i]);
                        }

                        ++count;
                    }

                    if (_useCost)
                    {
                        var cost = detailRowData.zCost[i];

                        if (!_showCostDetailed == false)
                        {
                            cost = Math.Floor(cost);
                        }

                        if (detailRowData.zCost[i] != double.MinValue)
                        {
                            xI.CreateDoubleAttr(attributeName, cost);
                        }
                    }
                }
            }
        }

        protected override void InitializeGridData(GridRenderingTypes renderingType)
        {
            if (renderingType == GridRenderingTypes.None)
            {
                throw new ArgumentException("renderingType");
            }

            var xBody = Constructor.CreateSubStruct("Body");
            var xB = xBody.CreateSubStruct("B");
            var xI = xBody.CreateSubStruct("I");
            xI.CreateStringAttr("Grouping", "Totals");
            xI.CreateBooleanAttr("CanEdit", false);

            Level = 0;
            Levels[Level] = xI;
        }
    }
}
