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
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.SalesforceMetadataService.PermissionSetApexPageAccess" />)
    ///     and namespace <see cref="EPMLiveCore.SalesforceMetadataService"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class PermissionSetApexPageAccessTest : AbstractBaseSetupTypedTest<PermissionSetApexPageAccess>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (PermissionSetApexPageAccess) Initializer

        private const string PropertyapexPage = "apexPage";
        private const string Propertyenabled = "enabled";
        private const string FieldapexPageField = "apexPageField";
        private const string FieldenabledField = "enabledField";
        private Type _permissionSetApexPageAccessInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private PermissionSetApexPageAccess _permissionSetApexPageAccessInstance;
        private PermissionSetApexPageAccess _permissionSetApexPageAccessInstanceFixture;

        #region General Initializer : Class (PermissionSetApexPageAccess) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="PermissionSetApexPageAccess" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _permissionSetApexPageAccessInstanceType = typeof(PermissionSetApexPageAccess);
            _permissionSetApexPageAccessInstanceFixture = Create(true);
            _permissionSetApexPageAccessInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (PermissionSetApexPageAccess)

        #region General Initializer : Class (PermissionSetApexPageAccess) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="PermissionSetApexPageAccess" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(PropertyapexPage)]
        [TestCase(Propertyenabled)]
        public void AUT_PermissionSetApexPageAccess_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var propertyInfo = GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_permissionSetApexPageAccessInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (PermissionSetApexPageAccess) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="PermissionSetApexPageAccess" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(FieldapexPageField)]
        [TestCase(FieldenabledField)]
        public void AUT_PermissionSetApexPageAccess_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_permissionSetApexPageAccessInstanceFixture, 
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
        ///     Class (<see cref="PermissionSetApexPageAccess" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Instance")]
        public void AUT_PermissionSetApexPageAccess_Is_Instance_Present_Test()
        {
            // Assert
            _permissionSetApexPageAccessInstanceType.ShouldNotBeNull();
            _permissionSetApexPageAccessInstance.ShouldNotBeNull();
            _permissionSetApexPageAccessInstanceFixture.ShouldNotBeNull();
            _permissionSetApexPageAccessInstance.ShouldBeAssignableTo<PermissionSetApexPageAccess>();
            _permissionSetApexPageAccessInstanceFixture.ShouldBeAssignableTo<PermissionSetApexPageAccess>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (PermissionSetApexPageAccess) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Constructor")]
        public void AUT_Constructor_PermissionSetApexPageAccess_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            PermissionSetApexPageAccess instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _permissionSetApexPageAccessInstanceType.ShouldNotBeNull();
            _permissionSetApexPageAccessInstance.ShouldNotBeNull();
            _permissionSetApexPageAccessInstanceFixture.ShouldNotBeNull();
            _permissionSetApexPageAccessInstance.ShouldBeAssignableTo<PermissionSetApexPageAccess>();
            _permissionSetApexPageAccessInstanceFixture.ShouldBeAssignableTo<PermissionSetApexPageAccess>();
        }

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (PermissionSetApexPageAccess) => all properties (non-static) explore and verify type tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        [TestCaseGeneric(typeof(string) , PropertyapexPage)]
        [TestCaseGeneric(typeof(bool) , Propertyenabled)]
        public void AUT_PermissionSetApexPageAccess_Property_Type_Verify_Explore_By_Name_Test<T>(string propertyName)
        {
            // AAA : Arrange, Act, Assert
            ShouldlyExtension.PropertyTypeVerify<PermissionSetApexPageAccess, T>(_permissionSetApexPageAccessInstance, propertyName, Fixture);
        }

        #endregion

        #region General Getters/Setters : Class (PermissionSetApexPageAccess) => Property (apexPage) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_PermissionSetApexPageAccess_Public_Class_apexPage_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyapexPage);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PermissionSetApexPageAccess) => Property (enabled) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_PermissionSetApexPageAccess_Public_Class_enabled_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(Propertyenabled);

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