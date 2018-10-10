using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common.Fakes;
using System.Data.Fakes;
using System.Data.SqlClient;
using System.Data.SqlClient.Fakes;
using System.Reflection;
using System.Web;
using System.Web.Fakes;
using System.Web.UI.Fakes;
using System.Xml;
using EPMLive.TestFakes.Utility;
using EPMLiveCore.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using TimeSheets;

namespace EPMLiveTimesheets.Tests.WebPageCode
{
    [TestClass]
    public partial class GetTsApprovalsTests
    {
        private IDisposable _shimContext;
        private bool _readFirstCall;
        private int _readCallCnt;
        private bool _readTimeEditorCall;
        private AdoShims _shimAdoNetCalls;
        private SharepointShims _shimSharepointCalls;

        [TestInitialize]
        public void TestInitialize()
        {
            _shimContext = ShimsContext.Create();
            _readFirstCall = true;
            _shimAdoNetCalls = AdoShims.ShimAdoNetCalls();
            _shimSharepointCalls = SharepointShims.ShimSharepointCalls();
            ArrangeShims();
            _readCallCnt = 0;
            _readTimeEditorCall = true;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _shimContext?.Dispose();
        }

        [TestMethod]
        public void outputXml_When_Table_DataRow_Not_Empty()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>
        <userdata name='listid'></userdata>
        <userdata name='itemid'></userdata>
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");
            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"listid\"></userdata><userdata name=\"itemid\"></userdata><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Pending\">]]></cell><cell type=\"timeeditor\">0|6|N|</cell><cell style=\"background: #EFEFEF; font-weight: bold\">6</cell><cell style=\"background: #EFEFEF; font-weight: bold\">|1</cell></row></rows></root>";
            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimDataTable.AllInstances.SelectString = (table, s) => new[] { new ShimDataRow().Instance };
            ShimDataRowCollection.AllInstances.CountGet = _ => 1;
            ShimDataRowCollection.AllInstances.ItemGetInt32 = (a, b) => new ShimDataRow();

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "TS_ITEM_HOURS" ? "6" : string.Empty;
            ShimDataRow.AllInstances.ItemGetInt32 = (a, b) => 1;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            Assert.AreEqual(expected, actual);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("spTSgetTSHours", list[2].CommandText);
            Assert.AreEqual("spTSGetTotalHoursForItem", list[3].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[4].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[2].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void outputXml_When_Project_And_Table_DataRow_Not_Empty()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <column width='0' type='' id='Project'></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>
        <userdata name='listid'></userdata>
        <userdata name='itemid'></userdata>
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");

            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><column width=\"50\" type=\"ro\" id=\"Project\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"listid\"></userdata><userdata name=\"itemid\"></userdata><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Pending\">]]></cell><cell type=\"timeeditor\">0|6|N|</cell><cell style=\"background: #EFEFEF; font-weight: bold\">6</cell><cell style=\"background: #EFEFEF; font-weight: bold\">|1</cell></row></rows></root>";

            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimDataTable.AllInstances.SelectString = (table, s) => new[] { new ShimDataRow().Instance };
            ShimDataRowCollection.AllInstances.CountGet = _ => 1;
            ShimDataRowCollection.AllInstances.ItemGetInt32 = (a, b) => new ShimDataRow();
            ShimDataRow.AllInstances.ItemGetString = (a, b) =>
            {
                switch (b)
                {
                    case "TS_ITEM_HOURS":
                        return "6";
                    default:
                        return string.Empty;
                }
            };

            ShimDataRow.AllInstances.ItemGetInt32 = (a, b) => 1;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            actual.ShouldBe(expected);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("spTSgetTSHours", list[2].CommandText);
            Assert.AreEqual("spTSGetTotalHoursForItem", list[3].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[4].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[2].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void outputXml_When_Table_DataRow_Not_Empty_And_TimeEditor_False()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>
        <userdata name='listid'></userdata>
        <userdata name='itemid'></userdata>
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");
            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"listid\"></userdata><userdata name=\"itemid\"></userdata><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Pending\">]]></cell><cell type=\"ro\">6</cell><cell style=\"background: #EFEFEF; font-weight: bold\">6</cell><cell style=\"background: #EFEFEF; font-weight: bold\">|1</cell></row></rows></root>";
            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimDataTable.AllInstances.SelectString = (table, s) => new[] { new ShimDataRow().Instance };
            ShimDataRowCollection.AllInstances.CountGet = _ => 1;
            ShimDataRowCollection.AllInstances.ItemGetInt32 = (a, b) => new ShimDataRow();
            ShimDataRow.AllInstances.ItemGetString = (a, b) =>
            {
                switch (b)
                {
                    case "TS_ITEM_HOURS":
                        return "6";
                    default:
                        return string.Empty;
                }
            };

            ShimCoreFunctions.getConfigSettingSPWebString = (a, b) =>
            {
                switch (b)
                {
                    case "EPMLiveDaySettings":
                        return "True|True|True|True";

                    default:
                        return false.ToString();
                }
            };

            ShimDataRow.AllInstances.ItemGetInt32 = (a, b) => 1;

            _readTimeEditorCall = false;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            actual.ShouldBe(expected);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("spTSgetTSHours", list[2].CommandText);
            Assert.AreEqual("spTSGetTotalHoursForItem", list[3].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[4].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[2].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void outputXml_When_Table_DataRow_Empty()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>
        <userdata name='listid'></userdata>
        <userdata name='itemid'></userdata>
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");
            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"listid\"></userdata><userdata name=\"itemid\"></userdata><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Pending\">]]></cell><cell type=\"timeeditor\">0|0|N|</cell><cell style=\"background: #EFEFEF; font-weight: bold\">0</cell><cell style=\"background: #EFEFEF; font-weight: bold\">|0</cell></row></rows></root>";

            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimDataTable.AllInstances.SelectString = (table, s) => new DataRow[0];
            ShimInternalDataCollectionBase.AllInstances.CountGet = _ => 0;
            ShimDataRowCollection.AllInstances.CountGet = _ => 0;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            actual.ShouldBe(expected);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("spTSgetTSHours", list[2].CommandText);
            Assert.AreEqual("spTSGetTotalHoursForItem", list[3].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[4].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[2].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void outputXml_When_Table_DataRow_Empty_And_TimeEditor_False()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>
        <userdata name='listid'></userdata>
        <userdata name='itemid'></userdata>
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");
            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"listid\"></userdata><userdata name=\"itemid\"></userdata><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell type=\"ro\" style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Pending\">]]></cell><cell type=\"ro\">0</cell><cell style=\"background: #EFEFEF; font-weight: bold\">0</cell><cell style=\"background: #EFEFEF; font-weight: bold\">|0</cell></row></rows></root>";
            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimCoreFunctions.getConfigSettingSPWebString = (a, b) =>
            {
                switch (b)
                {
                    case "EPMLiveDaySettings":
                        return "True|True|True|True";

                    default:
                        return false.ToString();
                }
            };

            ShimDataTable.AllInstances.SelectString = (table, s) => new DataRow[0];
            ShimDataRowCollection.AllInstances.CountGet = _ => 0;
            _readTimeEditorCall = false;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            actual.ShouldBe(expected);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(5, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("spTSgetTSHours", list[2].CommandText);
            Assert.AreEqual("spTSGetTotalHoursForItem", list[3].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[4].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);
            Assert.AreEqual("select ts_item_uid,submitted,approval_status,project from vwtstasks where list_uid=@listuid and item_id=@itemid and username=@username and period_id=@period_id", list[2].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void outputXml_When_No_ListId_Item_And_Table_DataRow_Not_Empty()
        {
            // Arrange            
            var docXml = new XmlDocument();
            docXml.LoadXml(@"<root>
<head>
    <settings></settings>
    <column width='0' type=''></column>
    <beforeInit>
        <call command='attachHeader'>
            <dummy><![CDATA["" gridfilterSomeText('1 ""]]></dummy>
        </call>
    </beforeInit>
</head>
<rows>
    <row id='                                                                           . .'>        
        <userdata name='Work'></userdata>
    </row>
</rows>
</root>");
            var expected = "<root><head><column type=\"ch\" width=\"25\" align=\"center\" color=\"#F0F0F0\"><![CDATA[#master_checkbox]]></column><column type=\"tsnotes\" width=\"50\" align=\"center\" color=\"#F0F0F0\"><![CDATA[Notes]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[TM]]></column><column type=\"ro\" width=\"35\" align=\"center\" color=\"#F0F0F0\"><![CDATA[PM]]></column><column width=\"50\" type=\"ro\"></column><beforeInit><call command=\"attachHeader\"><dummy><![CDATA[&nbsp;,&nbsp;,&nbsp;,&nbsp;,\" gridfilterSomeText('2 \",&nbsp;,&nbsp;]]></dummy></call></beforeInit><column type=\"ro[=sum]\" width=\"40\" align=\"right\" id=\"_TsDate_1_1_0001\"><![CDATA[Mon<br>1]]></column><column type=\"ro[=sum]\" width=\"50\" align=\"right\" id=\"_TsTotal_\">Total</column><column type=\"percentwork\" width=\"125\" align=\"left\" color=\"#F0F0F0\" id=\"_PercentWork_\"><![CDATA[% Work Spent]]></column></head><rows><row id=\"1\"><userdata name=\"Work\"></userdata><cell type=\"ro\" style=\"background: #EFEFEF; font-weight: bold;\"></cell><cell type=\"ro\" style=\"background: #EFEFEF; font-weight: bold;\"></cell><cell type=\"ro\" style=\"background: #EFEFEF; font-weight: bold;\"></cell><cell type=\"ro\" style=\"background: #EFEFEF; font-weight: bold;\"></cell><cell style=\"background: #EFEFEF\"></cell><cell style=\"background: #EFEFEF;font-weight: bold;\"></cell><cell style=\"background: #EFEFEF;font-weight: bold;\" type=\"ro\"></cell></row></rows></root>";
            var approval = new GetTsApprovals();
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "cn", new ShimSqlConnection().Instance);
            SetFieldValue(approval, "gridname", "SomeText");

            ShimDataTable.AllInstances.SelectString = (table, s) => new[] { new ShimDataRow().Instance };
            ShimDataRowCollection.AllInstances.CountGet = _ => 1;
            ShimDataRowCollection.AllInstances.ItemGetInt32 = (a, b) => new ShimDataRow();
            ShimDataRow.AllInstances.ItemGetString = (a, b) =>
            {
                switch (b)
                {
                    case "TS_ITEM_HOURS":
                        return "6";
                    default:
                        return string.Empty;
                }
            };

            ShimDataRow.AllInstances.ItemGetInt32 = (a, b) => 1;

            // Act
            InvokeMethod(approval, "outputXml", new object[] { });
            var actual = GetFieldValue<string>(approval, "data");

            // Assert            
            actual.ShouldBe(expected);

            var list = _shimAdoNetCalls.CommandsCreated;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);

            list = _shimAdoNetCalls.CommandsExecuted;
            Assert.AreEqual(2, list.Count);
            Assert.AreEqual("select TSTYPE_ID from TSTYPE where site_uid=@siteid", list[0].CommandText);
            Assert.AreEqual("select period_start,period_end,locked from TSPERIOD where period_id=@period_id and site_id=@siteid", list[1].CommandText);

            Assert.AreEqual(_shimAdoNetCalls.CommandsCreated.Count, _shimAdoNetCalls.CommandsDisposed.Count);
        }

        [TestMethod]
        public void addTSItem()
        {
            // Arrange
            ArrangeShims();
            var approval = new GetTsApprovals();
            SetFieldValue(approval, "arrGroupFields", new[] { "field" });
            SetFieldValue(approval, "list", new ShimSPList().Instance);
            SetFieldValue(approval, "view", new ShimSPView().Instance);

            // Act
            InvokeMethod(approval, "addTSItem", new object[] { new ShimSPListItem().Instance, new SortedList(), "userName", "resource" });
            var actual = GetFieldValue<Queue>(approval, "queueAllItems");

            // Assert
            Assert.IsTrue(actual.Count > 0);
        }
        [TestMethod]
        public void populateGroups_When_Approval_Processing()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">1</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/epmlive/images/tsqueueprocessing.gif\" alt=\"Approval Processing\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) =>
            {
                switch (key)
                {
                    case "jobstatus":
                        return "1";
                    case "jobtype_id":
                        return "30";
                    default: return string.Empty;
                }
            };

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Item_Processing()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">1</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/epmlive/images/tsqueueprocessing.gif\" alt=\"Item Processing\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "jobstatus" ? "1" : string.Empty;

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Approval_Queued()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">1</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/epmlive/images/tsqueueprocessing.gif\" alt=\"Approval Queued\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) =>
            {
                switch (key)
                {
                    case "jobstatus":
                        return "0";
                    case "jobtype_id":
                        return "30";
                    default: return string.Empty;
                }
            };

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Item_Queued()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">1</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/epmlive/images/tsqueueprocessing.gif\" alt=\"Item Queued\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "jobstatus" ? "0" : string.Empty;

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Approved()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">0</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/images/green.gif\" alt=\"Approved\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "approval_status" ? "1" : string.Empty;

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Rejected()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">0</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/images/red.gif\" alt=\"Rejected\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "approval_status" ? "2" : string.Empty;

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_Submitted()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">0</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"><![CDATA[<img src=\"_layouts/images/yellow.gif\" alt=\"Submitted\">]]></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataRow.AllInstances.ItemGetString = (a, key) => key == "submitted" ? "true" : string.Empty;

            ShimDataRow.AllInstances.ItemGetInt32 = (a, index) =>
            {
                switch (index)
                {
                    case 0:
                    case 1:
                        return Guid.NewGuid();
                    case 2:
                        return 1;
                    default:
                        return string.Empty;
                }
            };

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void populateGroups_When_DataRow_Empty()
        {
            // Arrange
            var expected = "<rows><row id=\"title\" style=\"background: #DFE7F7; font-weight:bold\"><userdata name=\"tsuid\"></userdata><userdata name=\"nosub\">1</userdata><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell style=\"background: #DFE7F7; font-weight:bold\" /><cell style=\"background: #DFE7F7; font-weight:bold\"></cell><cell>title</cell></row></rows>";
            var approval = new GetTsApprovals();
            var arrGTemp = new SortedList();
            var docXml = new XmlDocument();
            docXml.LoadXml("<rows></rows>");
            var ndMainParent = docXml.ChildNodes[0];

            ShimDataTable.AllInstances.SelectString = (a, b) => new DataRow[0];

            SetFieldValue(approval, "resWeb", new ShimSPWeb().Instance);
            SetFieldValue(approval, "dsTimesheetTasks", new ShimDataSet().Instance);
            SetFieldValue(approval, "dsTimesheets", new ShimDataSet().Instance);
            SetFieldValue(approval, "docXml", docXml);
            SetFieldValue(approval, "ndMainParent", ndMainParent);

            // Act
            approval.populateGroups(string.Empty, arrGTemp, new ShimSPWeb().Instance);
            var actual = docXml.OuterXml;

            // Assert
            actual.ShouldBe(expected);
        }

        private void ArrangeShims()
        {
            ShimSPContext.CurrentGet = () => new ShimSPContext();
            ShimSPContext.AllInstances.SiteGet = context => new ShimSPSite();
            ShimSPContext.AllInstances.WebGet = context => new ShimSPWeb();

            ShimSPWeb.AllInstances.CurrentUserGet = web => new ShimSPUser();

            ShimSPSite.AllInstances.RootWebGet = site => new ShimSPWeb();

            ShimCoreFunctions.getConfigSettingSPWebString = (a, b) =>
            {
                switch (b)
                {
                    case "EPMLiveTSAllowNotes":
                        return true.ToString();

                    case "EPMLiveDaySettings":
                        return "True|True|True|True";

                    default:
                        return false.ToString();
                }
            };


            ShimSqlDataReader.AllInstances.Read = reader =>
            {
                if (_readFirstCall)
                {
                    _readFirstCall = false;
                    return _readCallCnt++ > 0 ? true : _readTimeEditorCall;
                }
                return false;
            };
            ShimSqlDataReader.AllInstances.Close = reader => _readFirstCall = true;
            ShimDbDataReader.AllInstances.Dispose = reader => _readFirstCall = true;

            ShimSqlDataReader.AllInstances.GetBooleanInt32 = (reader, i) => true;

            ShimSqlParameterCollection.AllInstances.AddWithValueStringObject = (a, b, c) => new SqlParameter();

            var valueCollection = new NameValueCollection
            {
                ["width"] = "1"
            };
            ShimHttpRequest.AllInstances.QueryStringGet = _ => valueCollection;

            ShimPage.AllInstances.RequestGet = page => new HttpRequest(string.Empty, "http://site.com", "width=1");

            ShimSPList.AllInstances.FieldsGet = list => new ShimSPFieldCollection();

            ShimSPFieldCollection.AllInstances.GetFieldByInternalNameString = (collection, s) => new ShimSPField();
            ShimSPField.AllInstances.TypeGet = field => SPFieldType.User;

            ShimSPListItem.AllInstances.IDGet = item => 0;
            ShimSPListItem.AllInstances.ParentListGet = item => new ShimSPList();

            ShimSPList.AllInstances.IDGet = list => Guid.Empty;
            ShimSPList.AllInstances.ParentWebGet = list => new ShimSPWeb();
            ShimSPList.AllInstances.GetItemByIdInt32 = (a, b) => new ShimSPListItem();

            ShimSPWeb.AllInstances.IDGet = web => Guid.Empty;

            ShimDbDataAdapter.AllInstances.FillDataSet = (adapter, set) => 0;

            ShimDataSet.AllInstances.TablesGet = _ => new ShimDataTableCollection();
            ShimInternalDataCollectionBase.AllInstances.CountGet = _ => 1;

            ShimDataTableCollection.AllInstances.ItemGetInt32 = (a, b) => new ShimDataTable();

            ShimDataTable.AllInstances.RowsGet = _ => new ShimDataRowCollection();

            ShimSPView.AllInstances.ViewFieldsGet = view => new ShimSPViewFieldCollection();
            ShimSPViewFieldCollection.AllInstances.CountGet = collection => 0;

            ShimSPList.AllInstances.GetItemsSPQuery = (a, b) => new ShimSPListItemCollection();
            ShimSPListItemCollection.AllInstances.GetEnumerator = _ =>
            {
                var list = new List<SPListItem>
                {
                    new ShimSPListItem().Instance
                };

                return list.GetEnumerator();
            };

            ShimSPWeb.AllInstances.ListsGet = _ => new ShimSPListCollection();
            ShimSPListCollection.AllInstances.ItemGetString = (a, b) => new ShimSPList();

            ShimSPListItem.AllInstances.ItemGetString = (a, b) => string.Empty;
            ShimSPListItem.AllInstances.TitleGet = _ => "title";

            ShimSPFieldUserValue.ConstructorSPWebString = (a, b, c) => { };
            ShimSPFieldUserValue.AllInstances.UserGet = _ => new ShimSPUser();

            ShimSPUser.AllInstances.LoginNameGet = _ => "loginNam";

            ShimDataTable.AllInstances.SelectString = (a, b) => new[] { new ShimDataRow().Instance };
            ShimDataTable.AllInstances.SelectStringString = (a, b, c) => new[] { new ShimDataRow().Instance };
        }

        public static void SetFieldValue(object obj, string fieldName, object fieldValue)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            fieldInfo?.SetValue(obj, fieldValue);
        }

        public static T GetFieldValue<T>(object obj, string fieldName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var fieldInfo = obj.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            return (T)fieldInfo?.GetValue(obj);
        }

        public static void InvokeMethod(object obj, string methodName, object[] parameters)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var dynMethod = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod?.Invoke(obj, parameters);
        }
    }
}