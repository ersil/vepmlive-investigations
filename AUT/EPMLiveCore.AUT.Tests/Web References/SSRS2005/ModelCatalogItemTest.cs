using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Action = System.Action;
using AUT.ConfigureTestProjects;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.Attribute;
using AUT.ConfigureTestProjects.AutoFixtureSetup;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using AutoFixture;
using Moq;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;
using EPMLiveCore.SSRS2005;
using ModelCatalogItem = EPMLiveCore.SSRS2005;

namespace EPMLiveCore.SSRS2005
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.SSRS2005.ModelCatalogItem" />)
    ///     and namespace <see cref="EPMLiveCore.SSRS2005"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class ModelCatalogItemTest : AbstractBaseSetupTypedTest<ModelCatalogItem>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (ModelCatalogItem) Initializer

        private const string PropertyModel = "Model";
        private const string PropertyDescription = "Description";
        private const string PropertyPerspectives = "Perspectives";
        private const string FieldmodelField = "modelField";
        private const string FielddescriptionField = "descriptionField";
        private const string FieldperspectivesField = "perspectivesField";
        private Type _modelCatalogItemInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private ModelCatalogItem _modelCatalogItemInstance;
        private ModelCatalogItem _modelCatalogItemInstanceFixture;

        #region General Initializer : Class (ModelCatalogItem) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="ModelCatalogItem" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _modelCatalogItemInstanceType = typeof(ModelCatalogItem);
            _modelCatalogItemInstanceFixture = Create(true);
            _modelCatalogItemInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (ModelCatalogItem)

        #region General Initializer : Class (ModelCatalogItem) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="ModelCatalogItem" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(PropertyModel)]
        [TestCase(PropertyDescription)]
        [TestCase(PropertyPerspectives)]
        public void AUT_ModelCatalogItem_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var propertyInfo = GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_modelCatalogItemInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (ModelCatalogItem) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="ModelCatalogItem" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(FieldmodelField)]
        [TestCase(FielddescriptionField)]
        [TestCase(FieldperspectivesField)]
        public void AUT_ModelCatalogItem_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_modelCatalogItemInstanceFixture, 
                                                                Fixture, 
                                                                fieldInfo);

            // Assert
            fieldInfo.ShouldNotBeNull();
        }

        #endregion

        #endregion

        #endregion

        #region Category : Instance

        /// <summary>
        ///     Class (<see cref="ModelCatalogItem" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Instance")]
        public void AUT_ModelCatalogItem_Is_Instance_Present_Test()
        {
            // Assert
            _modelCatalogItemInstanceType.ShouldNotBeNull();
            _modelCatalogItemInstance.ShouldNotBeNull();
            _modelCatalogItemInstanceFixture.ShouldNotBeNull();
            _modelCatalogItemInstance.ShouldBeAssignableTo<ModelCatalogItem>();
            _modelCatalogItemInstanceFixture.ShouldBeAssignableTo<ModelCatalogItem>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (ModelCatalogItem) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Constructor")]
        public void AUT_Constructor_ModelCatalogItem_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            ModelCatalogItem instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _modelCatalogItemInstanceType.ShouldNotBeNull();
            _modelCatalogItemInstance.ShouldNotBeNull();
            _modelCatalogItemInstanceFixture.ShouldNotBeNull();
            _modelCatalogItemInstance.ShouldBeAssignableTo<ModelCatalogItem>();
            _modelCatalogItemInstanceFixture.ShouldBeAssignableTo<ModelCatalogItem>();
        }

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (ModelCatalogItem) => all properties (non-static) explore and verify type tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        [TestCaseGeneric(typeof(string) , PropertyModel)]
        [TestCaseGeneric(typeof(string) , PropertyDescription)]
        [TestCaseGeneric(typeof(ModelPerspective[]) , PropertyPerspectives)]
        public void AUT_ModelCatalogItem_Property_Type_Verify_Explore_By_Name_Test<T>(string propertyName)
        {
            // AAA : Arrange, Act, Assert
            ShouldlyExtension.PropertyTypeVerify<ModelCatalogItem, T>(_modelCatalogItemInstance, propertyName, Fixture);
        }

        #endregion

        #region General Getters/Setters : Class (ModelCatalogItem) => Property (Description) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_ModelCatalogItem_Public_Class_Description_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyDescription);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (ModelCatalogItem) => Property (Model) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_ModelCatalogItem_Public_Class_Model_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyModel);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (ModelCatalogItem) => Property (Perspectives) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_ModelCatalogItem_Perspectives_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            var randomString = CreateType<string>();

            // Act
            var propertyInfo = GetPropertyInfo(PropertyPerspectives);
            Action currentAction = () => propertyInfo.SetValue(_modelCatalogItemInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (ModelCatalogItem) => Property (Perspectives) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_ModelCatalogItem_Public_Class_Perspectives_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyPerspectives);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #endregion

        #endregion
    }
}