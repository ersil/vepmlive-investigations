using System;
using System.Diagnostics.CodeAnalysis;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.Attribute;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using NUnit.Framework;
using Shouldly;

namespace EPMLiveCore.SalesforceMetadataService
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.SalesforceMetadataService.FieldSetItem" />)
    ///     and namespace <see cref="EPMLiveCore.SalesforceMetadataService"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class FieldSetItemTest : AbstractBaseSetupTypedTest<FieldSetItem>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (FieldSetItem) Initializer

        private const string Propertyfield = "field";
        private const string PropertyisRequired = "isRequired";
        private const string PropertyisRequiredSpecified = "isRequiredSpecified";
        private const string FieldfieldField = "fieldField";
        private const string FieldisRequiredField = "isRequiredField";
        private const string FieldisRequiredFieldSpecified = "isRequiredFieldSpecified";
        private Type _fieldSetItemInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private FieldSetItem _fieldSetItemInstance;
        private FieldSetItem _fieldSetItemInstanceFixture;

        #region General Initializer : Class (FieldSetItem) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="FieldSetItem" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _fieldSetItemInstanceType = typeof(FieldSetItem);
            _fieldSetItemInstanceFixture = Create(true);
            _fieldSetItemInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (FieldSetItem)

        #region General Initializer : Class (FieldSetItem) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="FieldSetItem" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(Propertyfield)]
        [TestCase(PropertyisRequired)]
        [TestCase(PropertyisRequiredSpecified)]
        public void AUT_FieldSetItem_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var propertyInfo = GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_fieldSetItemInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (FieldSetItem) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="FieldSetItem" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(FieldfieldField)]
        [TestCase(FieldisRequiredField)]
        [TestCase(FieldisRequiredFieldSpecified)]
        public void AUT_FieldSetItem_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_fieldSetItemInstanceFixture, 
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
        ///     Class (<see cref="FieldSetItem" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Instance")]
        public void AUT_FieldSetItem_Is_Instance_Present_Test()
        {
            // Assert
            _fieldSetItemInstanceType.ShouldNotBeNull();
            _fieldSetItemInstance.ShouldNotBeNull();
            _fieldSetItemInstanceFixture.ShouldNotBeNull();
            _fieldSetItemInstance.ShouldBeAssignableTo<FieldSetItem>();
            _fieldSetItemInstanceFixture.ShouldBeAssignableTo<FieldSetItem>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (FieldSetItem) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Constructor")]
        public void AUT_Constructor_FieldSetItem_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            FieldSetItem instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _fieldSetItemInstanceType.ShouldNotBeNull();
            _fieldSetItemInstance.ShouldNotBeNull();
            _fieldSetItemInstanceFixture.ShouldNotBeNull();
            _fieldSetItemInstance.ShouldBeAssignableTo<FieldSetItem>();
            _fieldSetItemInstanceFixture.ShouldBeAssignableTo<FieldSetItem>();
        }

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (FieldSetItem) => all properties (non-static) explore and verify type tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        [TestCaseGeneric(typeof(string) , Propertyfield)]
        [TestCaseGeneric(typeof(bool) , PropertyisRequired)]
        [TestCaseGeneric(typeof(bool) , PropertyisRequiredSpecified)]
        public void AUT_FieldSetItem_Property_Type_Verify_Explore_By_Name_Test<T>(string propertyName)
        {
            // AAA : Arrange, Act, Assert
            ShouldlyExtension.PropertyTypeVerify<FieldSetItem, T>(_fieldSetItemInstance, propertyName, Fixture);
        }

        #endregion

        #region General Getters/Setters : Class (FieldSetItem) => Property (field) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_FieldSetItem_Public_Class_field_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(Propertyfield);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (FieldSetItem) => Property (isRequired) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_FieldSetItem_Public_Class_isRequired_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyisRequired);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (FieldSetItem) => Property (isRequiredSpecified) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_FieldSetItem_Public_Class_isRequiredSpecified_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyisRequiredSpecified);

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