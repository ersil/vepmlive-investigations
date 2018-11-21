using System;
using System.Diagnostics.CodeAnalysis;
using Action = System.Action;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.Attribute;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;

namespace EPMLiveCore.SalesforceMetadataService
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.SalesforceMetadataService.DataCategory" />)
    ///     and namespace <see cref="EPMLiveCore.SalesforceMetadataService"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class DataCategoryTest : AbstractBaseSetupTypedTest<DataCategory>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (DataCategory) Initializer

        private const string PropertydataCategory = "dataCategory";
        private const string Propertylabel = "label";
        private const string Propertyname = "name";
        private const string FielddataCategoryField = "dataCategoryField";
        private const string FieldlabelField = "labelField";
        private const string FieldnameField = "nameField";
        private Type _dataCategoryInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private DataCategory _dataCategoryInstance;
        private DataCategory _dataCategoryInstanceFixture;

        #region General Initializer : Class (DataCategory) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="DataCategory" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _dataCategoryInstanceType = typeof(DataCategory);
            _dataCategoryInstanceFixture = Create(true);
            _dataCategoryInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (DataCategory)

        #region General Initializer : Class (DataCategory) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="DataCategory" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(PropertydataCategory)]
        [TestCase(Propertylabel)]
        [TestCase(Propertyname)]
        public void AUT_DataCategory_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var propertyInfo = GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_dataCategoryInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (DataCategory) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="DataCategory" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(FielddataCategoryField)]
        [TestCase(FieldlabelField)]
        [TestCase(FieldnameField)]
        public void AUT_DataCategory_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_dataCategoryInstanceFixture, 
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
        ///     Class (<see cref="DataCategory" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Instance")]
        public void AUT_DataCategory_Is_Instance_Present_Test()
        {
            // Assert
            _dataCategoryInstanceType.ShouldNotBeNull();
            _dataCategoryInstance.ShouldNotBeNull();
            _dataCategoryInstanceFixture.ShouldNotBeNull();
            _dataCategoryInstance.ShouldBeAssignableTo<DataCategory>();
            _dataCategoryInstanceFixture.ShouldBeAssignableTo<DataCategory>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (DataCategory) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Constructor")]
        public void AUT_Constructor_DataCategory_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            DataCategory instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _dataCategoryInstanceType.ShouldNotBeNull();
            _dataCategoryInstance.ShouldNotBeNull();
            _dataCategoryInstanceFixture.ShouldNotBeNull();
            _dataCategoryInstance.ShouldBeAssignableTo<DataCategory>();
            _dataCategoryInstanceFixture.ShouldBeAssignableTo<DataCategory>();
        }

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (DataCategory) => all properties (non-static) explore and verify type tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        [TestCaseGeneric(typeof(DataCategory[]) , PropertydataCategory)]
        [TestCaseGeneric(typeof(string) , Propertylabel)]
        [TestCaseGeneric(typeof(string) , Propertyname)]
        public void AUT_DataCategory_Property_Type_Verify_Explore_By_Name_Test<T>(string propertyName)
        {
            // AAA : Arrange, Act, Assert
            ShouldlyExtension.PropertyTypeVerify<DataCategory, T>(_dataCategoryInstance, propertyName, Fixture);
        }

        #endregion

        #region General Getters/Setters : Class (DataCategory) => Property (dataCategory) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_DataCategory_dataCategory_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            var randomString = CreateType<string>();

            // Act
            var propertyInfo = GetPropertyInfo(PropertydataCategory);
            Action currentAction = () => propertyInfo.SetValue(_dataCategoryInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (DataCategory) => Property (dataCategory) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_DataCategory_Public_Class_dataCategory_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertydataCategory);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (DataCategory) => Property (label) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_DataCategory_Public_Class_label_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(Propertylabel);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (DataCategory) => Property (name) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_DataCategory_Public_Class_name_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(Propertyname);

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