using System;
using System.Diagnostics.CodeAnalysis;
using Action = System.Action;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;

namespace EPMLiveWebParts.SSRS2010
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveWebParts.SSRS2010.GetRolePropertiesCompletedEventArgs" />)
    ///     and namespace <see cref="EPMLiveWebParts.SSRS2010"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class GetRolePropertiesCompletedEventArgsTest : AbstractBaseSetupTypedTest<GetRolePropertiesCompletedEventArgs>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (GetRolePropertiesCompletedEventArgs) Initializer

        private const string PropertyResult = "Result";
        private const string PropertyTaskIDs = "TaskIDs";
        private const string Fieldresults = "results";
        private Type _getRolePropertiesCompletedEventArgsInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private GetRolePropertiesCompletedEventArgs _getRolePropertiesCompletedEventArgsInstance;
        private GetRolePropertiesCompletedEventArgs _getRolePropertiesCompletedEventArgsInstanceFixture;

        #region General Initializer : Class (GetRolePropertiesCompletedEventArgs) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="GetRolePropertiesCompletedEventArgs" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _getRolePropertiesCompletedEventArgsInstanceType = typeof(GetRolePropertiesCompletedEventArgs);
            _getRolePropertiesCompletedEventArgsInstanceFixture = Create(true);
            _getRolePropertiesCompletedEventArgsInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (GetRolePropertiesCompletedEventArgs)

        #region General Initializer : Class (GetRolePropertiesCompletedEventArgs) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="GetRolePropertiesCompletedEventArgs" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(PropertyResult)]
        [TestCase(PropertyTaskIDs)]
        public void AUT_GetRolePropertiesCompletedEventArgs_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var propertyInfo = GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_getRolePropertiesCompletedEventArgsInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (GetRolePropertiesCompletedEventArgs) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="GetRolePropertiesCompletedEventArgs" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT Initializer")]
        [TestCase(Fieldresults)]
        public void AUT_GetRolePropertiesCompletedEventArgs_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_getRolePropertiesCompletedEventArgsInstanceFixture, 
                                                                Fixture, 
                                                                fieldInfo);

            // Assert
            fieldInfo.ShouldNotBeNull();
        }

        #endregion

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (GetRolePropertiesCompletedEventArgs) => Property (Result) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_GetRolePropertiesCompletedEventArgs_Public_Class_Result_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyResult);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (GetRolePropertiesCompletedEventArgs) => Property (TaskIDs) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_GetRolePropertiesCompletedEventArgs_TaskIDs_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            var randomString = CreateType<string>();

            // Act
            var propertyInfo = GetPropertyInfo(PropertyTaskIDs);
            Action currentAction = () => propertyInfo.SetValue(_getRolePropertiesCompletedEventArgsInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (GetRolePropertiesCompletedEventArgs) => Property (TaskIDs) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [NUnit.Framework.Category("AUT GetterSetter")]
        public void AUT_GetRolePropertiesCompletedEventArgs_Public_Class_TaskIDs_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            var propertyInfo  = GetPropertyInfo(PropertyTaskIDs);

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