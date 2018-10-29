﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Resources.Fakes;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.Fakes;
using EPMLiveWorkPlanner.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static EPMLiveWorkPlanner.WorkPlannerAPI;

namespace EPMLiveWorkPlanner.Tests.ISAPI
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class WorkPlannerAPITests
    {
        private WorkPlannerAPI testObject;
        private PrivateObject privateObject;
        private IDisposable shimsContext;
        private BindingFlags publicStatic;
        private BindingFlags publicInstance;
        private BindingFlags nonPublicInstance;
        private ShimSPWeb spWeb;
        private ShimSPSite spSite;
        private ShimSPListCollection spListCollection;
        private ShimSPList spList;
        private ShimSPListItem spListItem;
        private ShimSPFieldCollection spFieldCollection;
        private ShimSPField spField;
        private Guid guid;
        private int validations;
        private const string SampleGuidString = "83e81819-0112-4c22-bb70-d8ba101e9e0c";
        private const string DummyString = "DummyString";
        private const string GetExternalProjectsMethodName = "GetExternalProjects";
        private const string ImportTasksMethodName = "ImportTasks";
        private const string GetPlannersByProjectListMethodName = "GetPlannersByProjectList";
        private const string GetPlannersByTaskListMethodName = "GetPlannersByTaskList";

        [TestInitialize]
        public void Setup()
        {
            SetupShims();
            SetupVariables();

            testObject = new WorkPlannerAPI();
            privateObject = new PrivateObject(testObject);
        }

        [TestCleanup]
        public void TearDown()
        {
            shimsContext?.Dispose();
        }

        private void SetupShims()
        {
            shimsContext = ShimsContext.Create();

            ShimSPSite.ConstructorGuid = (_, __) => new ShimSPSite();
            ShimSPSite.AllInstances.OpenWeb = _ => spWeb;
            ShimSPSite.AllInstances.OpenWebGuid = (_, __) => spWeb;
            ShimSPSite.AllInstances.Dispose = _ => { };
            ShimSPWeb.AllInstances.Dispose = _ => { };
            ShimCoreFunctions.getLockedWebSPWeb = _ => guid;
            ShimCoreFunctions.getConfigSettingSPWebString = (_, __) => DummyString;
            ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = codeToRun => codeToRun();
        }

        private void SetupVariables()
        {
            validations = 0;
            publicStatic = BindingFlags.Static | BindingFlags.Public;
            publicInstance = BindingFlags.Instance | BindingFlags.Public;
            nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            guid = Guid.Parse(SampleGuidString);
            spWeb = new ShimSPWeb()
            {
                IDGet = () => guid,
                SiteGet = () => spSite,
                ListsGet = () => spListCollection
            };
            spSite = new ShimSPSite()
            {
                IDGet = () => guid
            };
            spListCollection = new ShimSPListCollection()
            {
                TryGetListString = _ => spList
            };
            spList = new ShimSPList()
            {
                IDGet = () => guid,
                FieldsGet = () => spFieldCollection
            };
            spListItem = new ShimSPListItem();
            spFieldCollection = new ShimSPFieldCollection()
            {
                GetFieldByInternalNameString = _ => spField
            };
            spField = new ShimSPField()
            {
                TitleGet = () => DummyString
            };
        }

        [TestMethod]
        public void GetExternalProjects_WhenCalled_ReturnsString()
        {
            // Arrange
            const string xmlString = @"
                <Grid>
                    <Header Title=""Title""/>
                    <Body>
                        <B/>
                    </Body>
                </Grid>";
            const string dataXml = @"
                <xmlcfg>
                    <PlannerID/>
                </xmlcfg>";
            var data = new XmlDocument();
            var dataTable = new DataTable();
            var now = DateTime.Now;

            data.LoadXml(dataXml);
            dataTable.Columns.Add("Title");
            dataTable.Columns.Add("Start");
            dataTable.Columns.Add("Finish");
            dataTable.Columns.Add("ID");
            var row = dataTable.NewRow();
            row["Title"] = DummyString;
            row["Start"] = now;
            row["Finish"] = now;
            row["ID"] = DummyString;
            dataTable.Rows.Add(row);

            ShimResourceManager.AllInstances.GetStringStringCultureInfo = (_, _1, _2) => xmlString;
            ShimReportingData.GetReportQuerySPWebSPListStringStringOut = (SPWeb web, SPList list, string spquery, out string orderby) =>
            {
                validations += 1;
                orderby = DummyString;
                return DummyString;
            };
            ShimReportingData.GetReportingDataSPWebStringBooleanStringString = (_1, _2, _3, _4, _5) =>
            {
                validations += 1;
                return dataTable;
            };
            ShimWorkPlannerAPI.getAttributeXmlNodeString = (node, input) =>
            {
                validations += 1;
                return input;
            };

            // Act
            var actual = XDocument.Parse((string)privateObject.Invoke(GetExternalProjectsMethodName, publicStatic, new object[] { data, spWeb.Instance }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual
                    .Element("Grid")
                    .Element("Body")
                    .Element("B")
                    .Element("I")
                    .Attribute("Title")
                    .Value
                    .ShouldBe(DummyString),
                () => actual
                    .Element("Grid")
                    .Element("Body")
                    .Element("B")
                    .Element("I")
                    .Attribute("Start")
                    .Value
                    .ShouldBe(now.ToShortDateString()),
                () => actual
                    .Element("Grid")
                    .Element("Body")
                    .Element("B")
                    .Element("I")
                    .Attribute("Finish")
                    .Value
                    .ShouldBe(now.ToShortDateString()),
                () => actual
                    .Element("Grid")
                    .Element("Body")
                    .Element("B")
                    .Element("I")
                    .Attribute("id")
                    .Value
                    .ShouldBe(DummyString),
                () => validations.ShouldBe(3));
        }

        [TestMethod]
        public void ImportTasks_UidEmptyAllowDuplicatesFalse_ReturnsString()
        {
            // Arrange
            const string expected = "<Result Status=\"1\">UID Column not specified and allow duplicated is on. You must either specify UID column or you must allow duplicates.</Result>";

            var dataXml = string.Format(@"
                <xmlcfg Structure=""Structure"" ResourceField=""ResourceField"" UIDColumn=""{0}"" AllowDuplicateRows=""{1}"" Planner=""Planner"" ID=""ID"">
                </xmlcfg>", string.Empty, false);
            var data = new XmlDocument();

            data.LoadXml(dataXml);

            // Act
            var actual = (string)privateObject.Invoke(ImportTasksMethodName, publicStatic, new object[] { data, spWeb.Instance });

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void ImportTasks_idEmpty_ReturnsString()
        {
            // Arrange
            const string expected = "<Result Status=\"1\">ID Not Specified</Result>";

            var allowDuplicates = true.ToString();
            var dataXml = string.Format(@"
                <xmlcfg Structure=""Structure"" ResourceField=""ResourceField"" UIDColumn=""{0}"" AllowDuplicateRows=""{1}"" Planner=""{2}"" ID=""{3}"">
                </xmlcfg>", DummyString, true, DummyString, string.Empty);
            var data = new XmlDocument();

            data.LoadXml(dataXml);

            // Act
            var actual = (string)privateObject.Invoke(ImportTasksMethodName, publicStatic, new object[] { data, spWeb.Instance });

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void ImportTasks_UidNotEmptyPlannerEmpty_ReturnsString()
        {
            // Arrange
            const string expected = "<Result Status=\"1\">Planner Not Specified</Result>";

            var dataXml = string.Format(@"
                <xmlcfg Structure=""Structure"" ResourceField=""ResourceField"" UIDColumn=""{0}"" AllowDuplicateRows=""{1}"" Planner=""{2}"" ID=""ID"">
                </xmlcfg>", DummyString, true, string.Empty);
            var data = new XmlDocument();

            data.LoadXml(dataXml);

            // Act
            var actual = (string)privateObject.Invoke(ImportTasksMethodName, publicStatic, new object[] { data, spWeb.Instance });

            // Assert
            actual.ShouldBe(expected);
        }

        [TestMethod]
        public void ImportTasks_NoException_ReturnsXml()
        {
            // Arrange
            var dataXml = string.Format(@"
                <xmlcfg Structure=""Structure"" ResourceField=""ResourceField"" UIDColumn=""{0}"" AllowDuplicateRows=""{1}"" Planner=""{2}"" ID=""ID"">
                    <LeftCols>
                        <C Name=""LeftCols1""/>
                    </LeftCols>
                    <Cols>
                        <C Name=""Cols1""/>
                    </Cols>
                    <I id=""1""/>
                    <Item/>
                </xmlcfg>",
                DummyString, true, DummyString);
            var data = new XmlDocument();
            var actualArray = default(ArrayList);
            var props = new PlannerProps()
            {
                sListProjectCenter = DummyString
            };

            data.LoadXml(dataXml);

            ShimWorkPlannerAPI.ImportTasksFixXmlStructureXmlDocumentRefStringString = (ref XmlDocument document, string sUID, string sStructure) =>
            {
                validations += 1;
            };
            ShimWorkPlannerAPI.GetTasksXmlDocumentSPWeb = (_, __) =>
            {
                validations += 1;
                return dataXml;
            };
            ShimWorkPlannerAPI.iGetGeneralLayoutSPWebStringXmlDocumentBoolean = (_1, _2, _3, _4) =>
            {
                validations += 1;
                return dataXml;
            };
            ShimWorkPlannerAPI.getSettingsSPWebString = (_1, _2) =>
            {
                validations += 1;
                return props;
            };
            ShimWorkPlannerAPI.GetResourceTableWorkPlannerAPIPlannerPropsGuidStringSPWeb = (_1, _2, _3, _4) =>
            {
                validations += 1;
                return default(DataSet);
            };
            ShimWorkPlannerAPI.processTasksXmlNodeXmlDocumentRefXmlNodeXmlDocumentStringArrayListBooleanInt32RefStringDataSetString =
                (XmlNode ndImportTask, ref XmlDocument returnDocument, XmlNode ndParent, XmlDocument docPlan,
                string sUID, ArrayList columnArray, bool bAllowDuplicates, ref int curtaskuid, string sResField,
                DataSet dsResources, string sTaskType) =>
                {
                    validations += 1;
                    var newNode = returnDocument.CreateNode(XmlNodeType.Element, "Item", returnDocument.NamespaceURI);
                    var statusAttribute = returnDocument.CreateAttribute("Status");

                    statusAttribute.Value = "1";
                    newNode.Attributes.Append(statusAttribute);
                    returnDocument.FirstChild.AppendChild(newNode);

                    actualArray = columnArray;
                };
            ShimWorkPlannerAPI.SaveWorkPlanXmlDocumentSPWeb = (_1, _2) =>
            {
                validations += 1;
                return DummyString;
            };

            // Act
            var actual = XDocument.Parse((string)privateObject.Invoke(ImportTasksMethodName, publicStatic, new object[] { data, spWeb.Instance }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.Element("Import").Element("SavePlan").Attribute("Status").Value.ShouldBe("0"),
                () => actual.Element("Import").Element("Item").Attribute("Status").Value.ShouldBe("1"),
                () => actual.Element("Import").Attribute("Status").Value.ShouldBe("1"),
                () => actualArray.Count.ShouldBe(2),
                () => actualArray.Contains("LeftCols1").ShouldBeTrue(),
                () => actualArray.Contains("Cols1").ShouldBeTrue(),
                () => validations.ShouldBe(7));
        }

        [TestMethod]
        public void ImportTasks_WithException_ReturnsXml()
        {
            // Arrange
            var dataXml = string.Format(@"
                <xmlcfg Structure=""Structure"" ResourceField=""ResourceField"" UIDColumn=""{0}"" AllowDuplicateRows=""{1}"" Planner=""{2}"" ID=""ID"">
                    <LeftCols>
                        <C Name=""LeftCols1""/>
                    </LeftCols>
                    <Cols>
                        <C Name=""Cols1""/>
                    </Cols>
                    <I id=""1""/>
                    <Item/>
                </xmlcfg>",
                DummyString, true, DummyString);
            var data = new XmlDocument();
            var actualArray = default(ArrayList);
            var props = new PlannerProps()
            {
                sListProjectCenter = DummyString
            };

            data.LoadXml(dataXml);

            ShimWorkPlannerAPI.ImportTasksFixXmlStructureXmlDocumentRefStringString = (ref XmlDocument document, string sUID, string sStructure) =>
            {
                validations += 1;
            };
            ShimWorkPlannerAPI.GetTasksXmlDocumentSPWeb = (_, __) =>
            {
                validations += 1;
                return dataXml;
            };
            ShimWorkPlannerAPI.iGetGeneralLayoutSPWebStringXmlDocumentBoolean = (_1, _2, _3, _4) =>
            {
                validations += 1;
                return dataXml;
            };
            ShimWorkPlannerAPI.getSettingsSPWebString = (_1, _2) =>
            {
                validations += 1;
                return props;
            };
            ShimWorkPlannerAPI.GetResourceTableWorkPlannerAPIPlannerPropsGuidStringSPWeb = (_1, _2, _3, _4) =>
            {
                validations += 1;
                return default(DataSet);
            };
            ShimWorkPlannerAPI.processTasksXmlNodeXmlDocumentRefXmlNodeXmlDocumentStringArrayListBooleanInt32RefStringDataSetString =
                (XmlNode ndImportTask, ref XmlDocument returnDocument, XmlNode ndParent, XmlDocument docPlan,
                string sUID, ArrayList columnArray, bool bAllowDuplicates, ref int curtaskuid, string sResField,
                DataSet dsResources, string sTaskType) =>
                {
                    validations += 1;
                    var newNode = returnDocument.CreateNode(XmlNodeType.Element, "Item", returnDocument.NamespaceURI);
                    var statusAttribute = returnDocument.CreateAttribute("Status");

                    statusAttribute.Value = "1";
                    newNode.Attributes.Append(statusAttribute);
                    returnDocument.FirstChild.AppendChild(newNode);

                    actualArray = columnArray;
                };
            ShimWorkPlannerAPI.SaveWorkPlanXmlDocumentSPWeb = (_1, _2) =>
            {
                validations += 1;
                throw new Exception(DummyString);
            };

            // Act
            var actual = XDocument.Parse((string)privateObject.Invoke(ImportTasksMethodName, publicStatic, new object[] { data, spWeb.Instance }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.Element("Import").Element("SavePlan").Attribute("Status").Value.ShouldBe("1"),
                () => actual.Element("Import").Element("SavePlan").Value.ShouldBe(DummyString),
                () => actual.Element("Import").Element("Item").Attribute("Status").Value.ShouldBe("1"),
                () => actual.Element("Import").Attribute("Status").Value.ShouldBe("1"),
                () => actualArray.Count.ShouldBe(2),
                () => actualArray.Contains("LeftCols1").ShouldBeTrue(),
                () => actualArray.Contains("Cols1").ShouldBeTrue(),
                () => validations.ShouldBe(7));
        }

        [TestMethod]
        public void GetPlannersByProjectList_WhenCalled_ReturnsSortedList()
        {
            // Arrange
            const string planners = "planner11|planner12";
            const string projectList = "Project Center";
            var methodHit = 0;

            spListCollection.TryGetListString = _ => null;

            ShimCoreFunctions.getLockConfigSettingSPWebStringBoolean = (_1, _2, _3) =>
            {
                methodHit += 1;
                var returnValue = DummyString;
                switch (methodHit)
                {
                    case 1:
                        returnValue = planners;
                        break;
                    case 2:
                        returnValue = projectList;
                        break;
                    case 3:
                        returnValue = false.ToString();
                        break;
                }
                return returnValue;
            };
            ShimCoreFunctions.getConfigSettingSPWebString = (_1, _2) => true.ToString();

            // Act
            var actual = (SortedList)privateObject.Invoke(GetPlannersByProjectListMethodName, publicStatic, new object[] { projectList, spWeb.Instance });

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.Count.ShouldBe(2),
                () => actual["planner11"].ShouldBe("planner12"),
                () => actual["MPP"].ShouldBe("Microsoft Office Project"));
        }

        [TestMethod]
        public void GetPlannersByTaskList_WhenCalled_ReturnsSortedList()
        {
            // Arrange
            const string planners = "planner11|planner12,planner21|planner22";
            const string taskList = "Project Center";
            var methodHit = 0;

            ShimCoreFunctions.getLockConfigSettingSPWebStringBoolean = (_1, _2, _3) =>
            {
                methodHit += 1;
                if(methodHit.Equals(1))
                {
                    return planners;
                }
                return taskList;
            };

            // Act
            var actual = (SortedList)privateObject.Invoke(GetPlannersByTaskListMethodName, publicStatic, new object[] { spWeb.Instance, taskList });

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.Count.ShouldBe(2),
                () => actual["planner11"].ShouldBe("planner12"),
                () => actual["planner21"].ShouldBe("planner22"));
        }
    }
}