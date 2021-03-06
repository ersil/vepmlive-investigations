﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Fakes;
using System.Data.SqlClient.Fakes;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PortfolioEngineCore;
using PortfolioEngineCore.Fakes;
using PortfolioEngineCore.Infrastructure.Entities;
using Shouldly;
using WorkEnginePPM.Fakes;
using WorkEnginePPM.WebServices.Core;

namespace WorkEnginePPM.Tests.WebServices
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class PFEAdminManagerTests
    {
        private PFEAdminManager testObject;
        private PrivateObject privateObject;
        private IDisposable shimsContext;
        private BindingFlags nonPublicInstance;
        private ShimSPWeb spWeb;
        private ShimSPSite spSite;
        private ShimSqlDataReader dataReader;
        private Guid guid;
        private int validations;
        private const int DummyInt = 1;
        private const int Zero = 0;
        private const int One = 1;
        private const int Two = 2;
        private const int Three = 3;
        private const int Four = 4;
        private const int Five = 5;
        private const string SampleGuidString1 = "83e81819-0112-4c22-bb70-d8ba101e9e0c";
        private const string SampleGuidString2 = "83e81819-0104-4c22-bb70-d8ba101e9e0c";
        private const string DummyString = "DummyString";
        private const string IDStringCaps = "ID";
        private const string SampleUrl = "http://www.sampleurl.com";
        private const string BuildCostCategoryTreeMethodName = "BuildCostCategoryTree";
        private const string InitilizeAdminCoreMethodName = "InitilizeAdminCore";
        private const string DeleteDepartmentsMethodName = "DeleteDepartments";
        private const string DeleteHolidayScheduleMethodName = "DeleteHolidaySchedule";
        private const string DeleteListWorkMethodName = "DeleteListWork";
        private const string DeletePersonalItemsMethodName = "DeletePersonalItems";
        private const string DeletePIListWorkMethodName = "DeletePIListWork";
        private const string DeleteResourceTimeoffMethodName = "DeleteResourceTimeoff";
        private const string DeleteWorkScheduleMethodName = "DeleteWorkSchedule";
        private const string DeleteRolesMethodName = "DeleteRoles";
        private const string UpdateHolidaySchedulesMethodName = "UpdateHolidaySchedules";
        private const string UpdateListWorkMethodName = "UpdateListWork";
        private const string UpdatePersonalItemsMethodName = "UpdatePersonalItems";
        private const string UpdateResourceTimeoffMethodName = "UpdateResourceTimeoff";
        private const string UpdateRolesMethodName = "UpdateRoles";
        private const string UpdateRolesOLDMethodName = "UpdateRoles_OLD";
        private const string UpdateScheduledWorkMethodName = "UpdateScheduledWork";
        private const string UpdateWorkScheduleMethodName = "UpdateWorkSchedule";
        private const string GetCostCategoryRolesMethodName = "GetCostCategoryRoles";
        private const string UpdateDepartmentsMethodName = "UpdateDepartments";
        private const string GetDepartmentsMethodName = "GetDepartments";
        private const string GetHolidaySchedulesMethodName = "GetHolidaySchedules";
        private const string GetPersonalItemsMethodName = "GetPersonalItems";
        private const string GetWorkSchedulesMethodName = "GetWorkSchedules";
        private const string PostCostValuesMethodName = "PostCostValues";

        [TestInitialize]
        public void Setup()
        {
            SetupShims();

            testObject = new PFEAdminManager(spWeb);
            privateObject = new PrivateObject(testObject);
        }

        private void SetupShims()
        {
            shimsContext = ShimsContext.Create();
            SetupVariables();

            ShimSqlConnection.ConstructorString = (_, __) => new ShimSqlConnection();
            ShimSqlConnection.AllInstances.Open = _ => { };
            ShimSqlConnection.AllInstances.Close = _ => { };
            ShimSqlCommand.AllInstances.ExecuteReader = _ => dataReader;
            ShimComponent.AllInstances.Dispose = _ => { };
            ShimSPSite.ConstructorGuid = (_, __) => new ShimSPSite();
            ShimSPSite.AllInstances.OpenWebString = (_, __) => spWeb;
            ShimSPSite.AllInstances.Dispose = _ => { };
            ShimCoreFunctions.getConfigSettingSPWebString = (_, __) => DummyString;
            ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = codeToRun => codeToRun();
            ShimConfigFunctions.GetCleanUsernameSPWeb = _ => DummyString;
        }

        private void SetupVariables()
        {
            validations = 0;
            nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            guid = Guid.Parse(SampleGuidString1);
            spWeb = new ShimSPWeb()
            {
                SiteGet = () => spSite
            };
            spSite = new ShimSPSite()
            {
                IDGet = () => guid
            };
            dataReader = new ShimSqlDataReader()
            {
                Read = () => false
            };
        }

        [TestCleanup]
        public void TearDown()
        {
            shimsContext?.Dispose();
        }

        [TestMethod]
        public void BuildCostCategoryTree_WhenCalled_ReturnsCostCategoryElement()
        {
            // Arrange
            const string xmlString = "<CostCategories/>";
            var childCostCategory = new CostCategory()
            {
                Id = Two,
                Name = "Child",
                Roles = new List<Role>(),
                CostCategories = new List<CostCategory>()
            };
            var parentCostCategory = new CostCategory()
            {
                Id = One,
                Name = "Parent",
                Roles = new List<Role>()
                {
                    new Role()
                    {
                        Id = One,
                        CostCategoryRoleId = One,
                        Name = DummyString
                    }
                },
                CostCategories = new List<CostCategory>()
                {
                    childCostCategory
                }
            };
            var document = XDocument.Parse(xmlString);

            var parameters = new object[]
            {
                parentCostCategory,
                document.Element("CostCategories")
            };

            // Act
            privateObject.Invoke(BuildCostCategoryTreeMethodName, nonPublicInstance, parameters);
            var actual = (XElement)parameters[1];

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual
                    .Element("CostCategory")
                    .Attribute("Id")
                    .Value
                    .ShouldBe(One.ToString()),
                () => actual
                    .Element("CostCategory")
                    .Attribute("Name")
                    .Value
                    .ShouldBe("Parent"),
                () => actual
                    .Element("CostCategory")
                    .Element("Role")
                    .Attribute("CostCategoryRoleId")
                    .Value
                    .ShouldBe(One.ToString()),
                () => actual
                    .Element("CostCategory")
                    .Element("CostCategory")
                    .Attribute("Id")
                    .Value
                    .ShouldBe(Two.ToString()),
                () => actual
                    .Element("CostCategory")
                    .Element("CostCategory")
                    .Attribute("Name")
                    .Value
                    .ShouldBe("Child"));
        }

        [TestMethod]
        public void InitilizeAdminCore_WhenCalled_ReturnsAdminInfos()
        {
            // Arrange and Act
            var actual = (Admininfos)privateObject.Invoke(
                InitilizeAdminCoreMethodName,
                nonPublicInstance,
                new object[]
                {
                    SecurityLevels.AdminCalc,
                    false,
                    DummyString
                });

            // Assert
            actual.ShouldNotBeNull();
        }

        [TestMethod]
        public void DeleteDepartments_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Department Id=""1"" DataId=""1""/>
                        <Department Id=""2"" DataId=""2""/>
                        <Department Id=""3"" DataId=""3""/>
                        <Department Id=""4"" DataId=""4""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteLookupValueInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                methodHit += 1;
                message = DummyString;

                if (methodHit.Equals(Four))
                {
                    throw new Exception(DummyString);
                }

                return !methodHit.Equals(One);
            };
            ShimAdmininfos.AllInstances.DeleteDepartmentsInt32 = (_, input) =>
            {
                return input.Equals(Two);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteDepartmentsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.SelectNodes("//Department").Count.ShouldBe(Four),
                () => actual.FirstChild.SelectSingleNode($@"//Department[@DataId=""{One}""]/Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode($@"//Department[@DataId=""{Two}""]/Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode($@"//Department[@DataId=""{Three}""]/Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode($@"//Department[@DataId=""{Four}""]/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => methodHit.ShouldBe(Four));
        }

        [TestMethod]
        public void DeleteHolidaySchedule_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <HolidaySchedule Id=""1"" DataId=""1""/>
                        <HolidaySchedule Id=""2"" DataId=""2""/>
                        <HolidaySchedule Id=""3"" DataId=""3""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteHolidayScheduleInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                methodHit += 1;
                message = DummyString;

                if (methodHit.Equals(Three))
                {
                    throw new Exception(DummyString);
                }

                return !methodHit.Equals(One);
            };
            ShimAdmininfos.AllInstances.DeleteHolidayScheduleStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""0""/>";
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteHolidayScheduleMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/HolidaySchedule").Count.ShouldBe(Two),
                () => actual.SelectSingleNode($@"//Data/HolidaySchedule[@DataId=""{One}""]/Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => methodHit.ShouldBe(Three));
        }

        [TestMethod]
        public void DeleteListWork_WithoutException_ReturnsResultXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""1""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.DeleteListWorkStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                outXml = resultXml;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeleteListWorkMethodName),
                () => actual.FirstChild.SelectNodes("//Result").Count.ShouldBe(2),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Zero}']").ShouldNotBeNull(),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{One}']").ShouldNotBeNull());
        }

        [TestMethod]
        public void DeleteListWork_WithException_ReturnsResultXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""0""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.DeleteListWorkStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                outXml = resultXml;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString));
        }

        [TestMethod]
        public void DeletePersonalItems_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Item Id=""1"" DataId=""1""/>
                        <Item Id=""2"" DataId=""2""/>
                        <Item Id=""3"" DataId=""3""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.DeletePersonalItemInt32 = (_, __) =>
            {
                methodHit += 1;
                if (methodHit.Equals(Three))
                {
                    throw new Exception(DummyString);
                }
                return !methodHit.Equals(One);
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeletePersonalItemsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/Item").Count.ShouldBe(Three),
                () => actual.SelectSingleNode($@"//Data/Item[@DataId=""{One}""]/Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.SelectSingleNode($@"//Data/Item[@DataId=""{Two}""]/Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.SelectSingleNode($@"//Data/Item[@DataId=""{Three}""]/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => methodHit.ShouldBe(Three));
        }

        [TestMethod]
        public void DeletePIListWork_WithoutException_ReturnsResultXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""1""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.DeletePIListWorkStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                outXml = resultXml;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeletePIListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeletePIListWorkMethodName),
                () => actual.FirstChild.SelectNodes("//Result").Count.ShouldBe(2),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Zero}']").ShouldNotBeNull(),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{One}']").ShouldNotBeNull());
        }

        [TestMethod]
        public void DeletePIListWork_WithException_ReturnsResultXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""0""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.DeletePIListWorkStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                outXml = resultXml;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeletePIListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString));
        }

        [TestMethod]
        public void DeleteResourceTimeoff_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Resource Id=""1"" DataId=""1""/>
                        <Resource Id=""2"" DataId=""2""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.DeleteResourceTimeoffStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                methodHit += 1;
                resultXml = @"<Result Status=""0""/>";
                if (methodHit.Equals(Two))
                {
                    throw new Exception(DummyString);
                }
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteResourceTimeoffMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/Resource").Count.ShouldBe(One),
                () => actual.SelectSingleNode($@"//Data/Resource/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => methodHit.ShouldBe(Two));
        }

        [TestMethod]
        public void DeleteWorkSchedule_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <WorkSchedule Id=""1"" DataId=""1""/>
                        <WorkSchedule Id=""2"" DataId=""2""/>
                        <WorkSchedule Id=""3"" DataId=""3""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteWorkScheduleInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                methodHit += 1;
                message = DummyString;

                if (methodHit.Equals(Three))
                {
                    throw new Exception(DummyString);
                }

                return !methodHit.Equals(One);
            };
            ShimAdmininfos.AllInstances.DeleteWorkScheduleStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""0""/>";
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteWorkScheduleMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/WorkSchedule").Count.ShouldBe(Two),
                () => actual.SelectSingleNode($@"//Data/WorkSchedule[@DataId=""{One}""]/Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => methodHit.ShouldBe(Three));
        }

        [TestMethod]
        public void DeleteRoles_CaseOne_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var errorMessage = $"Cannot delete item, it is used as follows: {DummyString}";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteLookupValueInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                validations += 1;
                message = DummyString;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeleteRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(errorMessage),
                () => validations.ShouldBe(2));
        }

        [TestMethod]
        public void DeleteRoles_CaseTwo_ReturnsDataXml()
        {
            // Arrange
            const string deletemessage = "Cost Categories: Resource Role\n";
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var errorMessage = $"Cannot delete Role, it is used as follows: {DummyString}";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteLookupValueInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                validations += 1;
                message = deletemessage;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimAdmininfos.AllInstances.CountRoleCategoriesInt32 = (_, __) =>
            {
                validations += 1;
                return Zero;
            };
            ShimAdmininfos.AllInstances.CanDeleteCostCategoryRoleInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                validations += 1;
                message = DummyString;
                return false;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeleteRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(errorMessage),
                () => validations.ShouldBe(4));
        }

        [TestMethod]
        public void DeleteRoles_CaseThree_ReturnsDataXml()
        {
            // Arrange
            const string deletemessage = "Cost Categories: Resource Role\n";
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var errorMessage = $"Cannot delete Role, it is used as follows: {DummyString}";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteLookupValueInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                validations += 1;
                message = deletemessage;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimAdmininfos.AllInstances.CountRoleCategoriesInt32 = (_, __) =>
            {
                validations += 1;
                return Five;
            };
            ShimAdmininfos.AllInstances.CanDeleteCostCategoryRolebyCCRIdInt32Int32StringOut = 
                (Admininfos instance, int ccid, int id, out string message) =>
                {
                    validations += 1;
                    message = DummyString;
                    return false;
                };

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeleteRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(errorMessage),
                () => validations.ShouldBe(4));
        }

        [TestMethod]
        public void DeleteRoles_CaseFour_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string errorMessage = "Failed to delete item";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.CanDeleteLookupValueInt32StringOut = (Admininfos instance, int id, out string message) =>
            {
                validations += 1;
                message = DummyString;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimAdmininfos.AllInstances.DeleteRoleInt32 = (_, __) =>
            {
                validations += 1;
                return false;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(DeleteRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(DeleteRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(errorMessage),
                () => validations.ShouldBe(3));
        }

        [TestMethod]
        public void UpdateHolidaySchedules_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <HolidaySchedule Id=""1"" DataId=""1""/>
                        <HolidaySchedule Id=""2"" DataId=""2""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateHolidayScheduleStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""5""/>";
                methodHit += 1;
                if (methodHit.Equals(Two))
                {
                    throw new Exception(DummyString);
                }
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateHolidaySchedulesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/HolidaySchedule").Count.ShouldBe(One),
                () => actual.SelectSingleNode($@"//Data/HolidaySchedule/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Five.ToString()),
                () => methodHit.ShouldBe(Two));
        }

        [TestMethod]
        public void UpdateListWork_WithoutException_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""5""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateListWorkStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                outXml = resultXml;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateListWorkMethodName),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Zero}']").ShouldNotBeNull(),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Five}']").ShouldNotBeNull());
        }

        [TestMethod]
        public void UpdateListWork_WithException_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateListWorkStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""5""/>";
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateListWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"));
        }

        [TestMethod]
        public void UpdatePersonalItems_WithoutException_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            const string resultXml = @"<Result Status=""5""/>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdatePersonalItemsStringStringOutStringOut = (Admininfos instance, string xml, out string outXml, out string error) =>
            {
                outXml = resultXml;
                error = DummyString;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdatePersonalItemsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdatePersonalItemsMethodName),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Zero}']").ShouldNotBeNull(),
                () => actual.FirstChild.SelectSingleNode($"//Result[@Status='{Five}']").ShouldNotBeNull());
        }

        [TestMethod]
        public void UpdatePersonalItems_WithException_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <ListWork Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdatePersonalItemsStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string error) =>
            {
                resultXml = @"<Result Status=""5""/>";
                error = DummyString;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdatePersonalItemsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"));
        }

        [TestMethod]
        public void UpdateResourceTimeoff_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Resource Id=""1"" DataId=""1""/>
                        <Resource Id=""2"" DataId=""2""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateResourceTimeoffStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""5""/>";
                methodHit += 1;
                if (methodHit.Equals(Two))
                {
                    throw new Exception(DummyString);
                }
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateResourceTimeoffMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/Resource").Count.ShouldBe(One),
                () => actual.SelectSingleNode($@"//Data/Resource/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Five.ToString()),
                () => methodHit.ShouldBe(Two));
        }

        [TestMethod]
        public void UpdateRoles_UpdateOkFalse_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateRolesStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimAdmininfos.AllInstances.UpdateCategoriesFromRoles = _ =>
            {
                validations += 1;
                return true;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateRoles_UpdateOkTrue_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateRolesStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.AllInstances.UpdateCategoriesFromRoles = _ =>
            {
                validations += 1;
                return true;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(3));
        }

        [TestMethod]
        public void UpdateRolesold_UpdateOkFalse_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateRoles_OLDStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimAdmininfos.AllInstances.UpdateCategoriesFromRoles = _ =>
            {
                validations += 1;
                return true;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateRolesOLDMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateRolesOld_UpdateOkTrue_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateRoles_OLDStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCCRs = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.AllInstances.UpdateCategoriesFromRoles = _ =>
            {
                validations += 1;
                return true;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateRolesOLDMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateRolesOLDMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateScheduledWork_UpdateOkFalse_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Params Worktype=""1""/>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateScheduledWorkInt32StringStringOut = (Admininfos instance, int workType, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateScheduledWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateScheduledWork_UpdateOkTrue_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Params Worktype=""1""/>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateScheduledWorkInt32StringStringOut = (Admininfos instance, int workType, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = $"<{DummyString}>{DummyString}</{DummyString}>";
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateScheduledWorkMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateScheduledWorkMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateWorkSchedule_WhenCalled_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <WorkSchedule Id=""1"" DataId=""1""/>
                        <WorkSchedule Id=""2"" DataId=""2""/>
                    </Data>
                </xmlcfg>";
            var methodHit = 0;
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateWorkScheduleStringStringOut = (Admininfos instance, string xml, out string resultXml) =>
            {
                resultXml = @"<Result Status=""5""/>";
                methodHit += 1;
                if (methodHit.Equals(Two))
                {
                    throw new Exception(DummyString);
                }
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateWorkScheduleMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.SelectNodes("//Data/WorkSchedule").Count.ShouldBe(One),
                () => actual.SelectSingleNode($@"//Data/WorkSchedule/Result").InnerText.ShouldBe($"Error: {DummyString}"),
                () => actual.SelectSingleNode($@"//Data/Result").Attributes["Status"].Value.ShouldBe(Five.ToString()),
                () => methodHit.ShouldBe(Two));
        }

        [TestMethod]
        public void GetCostCategoryRoles_WhenCalled_ReturnsCostCategoryRoles()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data>
                        <WorkSchedule Id=""1"" DataId=""1""/>
                        <WorkSchedule Id=""2"" DataId=""2""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();
            var childCostCategory = new CostCategory()
            {
                Id = Two,
                Name = "Child",
                Roles = new List<Role>(),
                CostCategories = new List<CostCategory>()
            };
            var parentCostCategory = new List<CostCategory>()
            {
                new CostCategory()
                {
                    Id = One,
                    Name = "Parent",
                    Roles = new List<Role>()
                    {
                        new Role()
                        {
                            Id = One,
                            CostCategoryRoleId = One,
                            Name = DummyString
                        }
                    },
                    CostCategories = new List<CostCategory>()
                    {
                        childCostCategory
                    }
                }
            };

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.GetCostCategoryRoles = _ => parentCostCategory;

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetCostCategoryRolesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(GetCostCategoryRolesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Data/CostCategory").Attributes["Id"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Data/CostCategory").Attributes["Name"].Value.ShouldBe("Parent"),
                () => actual.FirstChild.SelectSingleNode("//Data/CostCategory/Role").Attributes["CostCategoryRoleId"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Data/CostCategory/CostCategory").Attributes["Name"].Value.ShouldBe("Child"));
        }

        [TestMethod]
        public void UpdateDepartments_UpdateOkFalse_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Params Worktype=""1""/>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateDepartmentsStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = DummyString;
                return false;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateDepartmentsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void UpdateDepartments_UpdateOkTrue_ReturnsDataXml()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Params Worktype=""1""/>
                    <Data>
                        <Role Id=""1"" DataId=""1""/>
                    </Data>
                </xmlcfg>";
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.UpdateDepartmentsStringStringOut = (Admininfos instance, string xml, out string outXml) =>
            {
                validations += 1;
                outXml = $"<{DummyString}>{DummyString}</{DummyString}>";
                return true;
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(UpdateDepartmentsMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(UpdateDepartmentsMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetDepartments_WithoutException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetDepts = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetDepartmentsMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(GetDepartmentsMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetDepartments_WithException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetDepts = _ =>
            {
                validations += 1;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetDepartmentsMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(((int)APIError.GetDepartments).ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetHolidaySchedules_WithoutException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetHOLs = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetHolidaySchedulesMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(GetHolidaySchedulesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetHolidaySchedules_WithException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetHOLs = _ =>
            {
                validations += 1;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetHolidaySchedulesMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(((int)APIError.GetHolidaySchedules).ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetPersonalItems_WithoutException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetPersonalItems = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetPersonalItemsMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(GetPersonalItemsMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetPersonalItems_WithException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetPersonalItems = _ =>
            {
                validations += 1;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetPersonalItemsMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(((int)APIError.GetPersonalItems).ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetWorkSchedules_WithoutException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetWHs = _ =>
            {
                validations += 1;
                return $"<{DummyString}>{DummyString}</{DummyString}>";
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetWorkSchedulesMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe(GetWorkSchedulesMethodName),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(Zero.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result").InnerText.ShouldBe(string.Empty),
                () => actual.FirstChild.SelectSingleNode($"//{DummyString}").InnerText.ShouldBe(DummyString),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void GetWorkSchedules_WithException_ReturnsDataXml()
        {
            // Arrange
            var actual = new XmlDocument();

            ShimAdmininfos.AllInstances.GetWHs = _ =>
            {
                validations += 1;
                throw new Exception(DummyString);
            };
            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();

            // Act
            actual.LoadXml((string)privateObject.Invoke(GetWorkSchedulesMethodName, nonPublicInstance, new object[] { }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(((int)APIError.GetWorkSchedules).ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(1));
        }

        [TestMethod]
        public void PostCostValues_CaseOne_ReturnsPostCostValues()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var actual = new XmlDocument();
            var expected = $"PostError0: {(int)StatusEnum.rsDBConnectFailed} - DummyString";

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsDBConnectFailed;
                return StatusEnum.rsDBConnectFailed;
            };
            ShimSqlDb.AllInstances.FormatErrorText = _ =>
            {
                validations += 1;
                return DummyString;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(4));
        }

        [TestMethod]
        public void PostCostValues_CaseTwo_ReturnsPostCostValues()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var actual = new XmlDocument();

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsDBConnectFailed;
                throw new Exception(DummyString);
            };
            ShimSqlDb.AllInstances.FormatErrorText = _ =>
            {
                validations += 1;
                return DummyString;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe($"Error: {DummyString}"),
                () => validations.ShouldBe(3));
        }

        [TestMethod]
        public void PostCostValues_CaseThree_ReturnsPostCostValues()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var actual = new XmlDocument();
            var expected = $"PostError1: {(int)StatusEnum.rsInvalidPeriodID} - DummyString";

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                dbAccess.Status = StatusEnum.rsInvalidPeriodID;
                return StatusEnum.rsInvalidPeriodID;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(5));
        }

        [TestMethod]
        public void PostCostValues_CaseFour_ReturnsPostCostValues()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var actual = new XmlDocument();
            var expected = $"PostError3: {(int)StatusEnum.rsInvalidPeriodID} - DummyString";

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = _ =>
            {
                validations += 1;
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.DBTraceStatusEnumTraceChannelEnumStringStringStringStringBoolean = (instance, _1, _2, _3, _4, _5, _6, _7) =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsInvalidPeriodID;
            };
            ShimIntegration.AllInstances.executeStringString = (_, _1, _2) =>
            {
                validations += 1;
                return null;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(7));
        }

        [TestMethod]
        public void PostCostValues_CaseFive_ReturnsPostCostValues()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Data/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            const string expectedStatusText = "No response from WorkEngine WebService";
            var actual = new XmlDocument();
            var expected = $"PostError4: {99835} - DummyString";

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = instance =>
            {
                if (instance.StatusText.Equals(expectedStatusText))
                {
                    validations += 1;
                }
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.DBTraceStatusEnumTraceChannelEnumStringStringStringStringBoolean = (instance, _1, _2, _3, _4, _5, _6, _7) =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsSuccess;
            };
            ShimIntegration.AllInstances.executeStringString = (_, _1, _2) =>
            {
                validations += 1;
                return null;
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(7));
        }

        [TestMethod]
        public void PostCostValues_CaseSix_ReturnsPostCostValues()
        {
            // Arrange
            var xmlString = $@"
                <xmlcfg>
                    <Data/>
                    <Result Status=""1"">
                        <Error ID=""1"">{DummyString}</Error>
                    </Result>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var expectedStatusText = $"Invalid XML response from WorkEngine WebService. Status=1; Error=1 : {DummyString}";
            var actual = new XmlDocument();
            var expected = $"PostError6: {99833} - DummyString";
            var data = new XmlDocument();

            data.LoadXml(xmlString);

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = instance =>
            {
                if (instance.StatusText.Equals(expectedStatusText))
                {
                    validations += 1;
                }
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.DBTraceStatusEnumTraceChannelEnumStringStringStringStringBoolean = (instance, _1, _2, _3, _4, _5, _6, _7) =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsSuccess;
            };
            ShimIntegration.AllInstances.executeStringString = (_, _1, _2) =>
            {
                validations += 1;
                return data.FirstChild.SelectSingleNode("//Result");
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(8));
        }

        [TestMethod]
        public void PostCostValues_CaseSeven_ReturnsPostCostValues()
        {
            // Arrange
            var xmlString = $@"
                <xmlcfg>
                    <Data/>
                    <Result Status=""1"">
                        <Item Error=""1"">{DummyString}</Item>
                    </Result>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            var expectedStatusText = $"Invalid XML response from WorkEngine WebService. Status=1; Error=1 : {DummyString}";
            var actual = new XmlDocument();
            var expected = $"PostError7: {99999} - DummyString";
            var data = new XmlDocument();

            data.LoadXml(xmlString);

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = instance =>
            {
                if (instance.StatusText.Equals(expectedStatusText))
                {
                    validations += 1;
                }
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.DBTraceStatusEnumTraceChannelEnumStringStringStringStringBoolean = (instance, _1, _2, _3, _4, _5, _6, _7) =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsSuccess;
            };
            ShimIntegration.AllInstances.executeStringString = (_, _1, _2) =>
            {
                validations += 1;
                return data.FirstChild.SelectSingleNode("//Result");
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(8));
        }

        [TestMethod]
        public void PostCostValues_CaseEight_ReturnsPostCostValues()
        {
            // Arrange
            var xmlString = $@"
                <xmlcfg>
                    <Data/>
                    <Result Status=""1""/>
                </xmlcfg>";
            const string postInstructionString = @"
                <PostInstruction>
                    <IDs Id=""2""/>
                    <PIs Id=""4""/>
                </PostInstruction>";
            const string expectedStatusText = "XML response from WorkEngine WebService not recognized";
            var actual = new XmlDocument();
            var expected = $"PostError8: {99999} - DummyString";
            var data = new XmlDocument();

            data.LoadXml(xmlString);

            ShimAdmininfos.ConstructorStringStringStringStringStringSecurityLevelsBoolean = (_, _1, _2, _3, _4, _5, _6, _7) => new ShimAdmininfos();
            ShimAdmininfos.AllInstances.PostCostValuesStringStringOutStringOut = (Admininfos instance, string xml, out string resultXml, out string postInstruction) =>
            {
                validations += 1;
                resultXml = DummyString;
                postInstruction = postInstructionString;
                return true;
            };
            ShimWebAdmin.GetConnectionString = () =>
            {
                validations += 1;
                return DummyString;
            };
            ShimSqlDb.AllInstances.Open = instance =>
            {
                validations += 1;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.FormatErrorText = instance =>
            {
                if (instance.StatusText.Equals(expectedStatusText))
                {
                    validations += 1;
                }
                return DummyString;
            };
            ShimdbaUsers.ExportPIInfoDBAccessStringStringOut = (DBAccess dbAccess, string pi, out string xmlRequest) =>
            {
                validations += 1;
                xmlRequest = DummyString;
                return StatusEnum.rsSuccess;
            };
            ShimSqlDb.AllInstances.DBTraceStatusEnumTraceChannelEnumStringStringStringStringBoolean = (instance, _1, _2, _3, _4, _5, _6, _7) =>
            {
                validations += 1;
                instance.Status = StatusEnum.rsSuccess;
            };
            ShimIntegration.AllInstances.executeStringString = (_, _1, _2) =>
            {
                validations += 1;
                return data.FirstChild.SelectSingleNode("//Result");
            };

            // Act
            actual.LoadXml((string)privateObject.Invoke(PostCostValuesMethodName, nonPublicInstance, new object[] { xmlString }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.FirstChild.Name.ShouldBe("Result"),
                () => actual.FirstChild.SelectSingleNode("//Result").Attributes["Status"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["ID"].Value.ShouldBe(One.ToString()),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").Attributes["PfEFailure"].Value.ShouldBe(bool.FalseString),
                () => actual.FirstChild.SelectSingleNode("//Result/Error").InnerText.ShouldBe(expected),
                () => validations.ShouldBe(8));
        }
    }
}