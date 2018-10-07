using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Action = System.Action;
using AUT.ConfigureTestProjects;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.Attribute;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.BaseSetup.Version.V2;
using AUT.ConfigureTestProjects.BaseSetup.Version.V3;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.Model;
using AUT.ConfigureTestProjects.StaticTypes;
using AutoFixture;
using Moq;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;
using UplandIntegrations.TenroxAssignmentService;
using PortfolioBudget = UplandIntegrations.TenroxAssignmentService;

namespace UplandIntegrations.TenroxAssignmentService
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="UplandIntegrations.TenroxAssignmentService.PortfolioBudget" />)
    ///     and namespace <see cref="UplandIntegrations.TenroxAssignmentService"/> class using generator(v:0.2.2)'s 
    ///     artificial intelligence. Compatible with <see cref="AUT.ConfigureTestProjects" /> v4.2.6.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class PortfolioBudgetTest : AbstractBaseSetupV3Test
    {
        public PortfolioBudgetTest() : base(typeof(PortfolioBudget))
        {}

        #region Category : General

        #region Category : Initializer

        #region General Initializer : Overrides of IAbstractBaseSetupV2Test

        /// <summary>
        ///    Configure and ignore problematic tests.
        ///    Added tests will be ignored.
        /// </summary>
        public override void ConfigureIgnoringTests()
        {
            base.ConfigureIgnoringTests();
        }

        #endregion

        #region General Initializer : Class (PortfolioBudget) Initializer

        #region Properties

        private const string PropertyExtensionData = "ExtensionData";
        private const string PropertyAllocatedBudgets = "AllocatedBudgets";
        private const string PropertyBudgetAmount = "BudgetAmount";
        private const string PropertyDescription = "Description";
        private const string PropertyEndDate = "EndDate";
        private const string PropertyFundingSource = "FundingSource";
        private const string PropertyName = "Name";
        private const string PropertyPortfolio = "Portfolio";
        private const string PropertyPortfolioId = "PortfolioId";
        private const string PropertyStartDate = "StartDate";
        private const string PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey = "SystemDataObjectsDataClassesIEntityWithKeyEntityKey";
        private const string PropertyUniqueId = "UniqueId";

        #endregion

        #region Methods

        private const string MethodRaisePropertyChanged = "RaisePropertyChanged";

        #endregion

        #region Fields

        private const string FieldextensionDataField = "extensionDataField";
        private const string FieldAllocatedBudgetsField = "AllocatedBudgetsField";
        private const string FieldBudgetAmountField = "BudgetAmountField";
        private const string FieldDescriptionField = "DescriptionField";
        private const string FieldEndDateField = "EndDateField";
        private const string FieldFundingSourceField = "FundingSourceField";
        private const string FieldNameField = "NameField";
        private const string FieldPortfolioField = "PortfolioField";
        private const string FieldPortfolioIdField = "PortfolioIdField";
        private const string FieldStartDateField = "StartDateField";
        private const string FieldSystemDataObjectsDataClassesIEntityWithKeyEntityKeyField = "SystemDataObjectsDataClassesIEntityWithKeyEntityKeyField";
        private const string FieldUniqueIdField = "UniqueIdField";

        #endregion

        private Type _portfolioBudgetInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutEightSeconds;
        private PortfolioBudget _portfolioBudgetInstance;
        private PortfolioBudget _portfolioBudgetInstanceFixture;

        #region General Initializer : Class (PortfolioBudget) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="PortfolioBudget" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _portfolioBudgetInstanceType = typeof(PortfolioBudget);
            _portfolioBudgetInstanceFixture = this.Create<PortfolioBudget>(true);
            _portfolioBudgetInstance = _portfolioBudgetInstanceFixture ?? this.Create<PortfolioBudget>(false);
            CurrentInstance = _portfolioBudgetInstanceFixture;
            ConfigureIgnoringTests();
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (PortfolioBudget)

        #region General Initializer : Class (PortfolioBudget) All Methods Explore Verification.

        /// <summary>
        ///     Class (<see cref="PortfolioBudget" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(MethodRaisePropertyChanged, 0)]
        public void AUT_PortfolioBudget_All_Methods_Explore_Verify_Test(string name, int overloadingIndex = 0)
        {
            // Arrange
            this.ValidateExecuteCondition(name);
            var currentMethodInfo = this.GetMethodInfo(name, overloadingIndex);

            // Act
            ShouldlyExtension.ExploreMethodWithOrWithoutInstance(_portfolioBudgetInstanceFixture, 
                                                                 Fixture, 
                                                                 currentMethodInfo);

            // Assert
            currentMethodInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (PortfolioBudget) All Properties Explore By Name

        /// <summary>
        ///     Class (<see cref="PortfolioBudget" />) explore and verify properties for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(PropertyExtensionData)]
        [TestCase(PropertyAllocatedBudgets)]
        [TestCase(PropertyBudgetAmount)]
        [TestCase(PropertyDescription)]
        [TestCase(PropertyEndDate)]
        [TestCase(PropertyFundingSource)]
        [TestCase(PropertyName)]
        [TestCase(PropertyPortfolio)]
        [TestCase(PropertyPortfolioId)]
        [TestCase(PropertyStartDate)]
        [TestCase(PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey)]
        [TestCase(PropertyUniqueId)]
        public void AUT_PortfolioBudget_All_Properties_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            this.ValidateExecuteCondition(name);
            var propertyInfo = this.GetPropertyInfo(name);

            // Act
            ShouldlyExtension.ExploreProperty(_portfolioBudgetInstanceFixture,
                                              Fixture,
                                              propertyInfo);

            // Assert
            propertyInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (PortfolioBudget) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="PortfolioBudget" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(FieldextensionDataField)]
        [TestCase(FieldAllocatedBudgetsField)]
        [TestCase(FieldBudgetAmountField)]
        [TestCase(FieldDescriptionField)]
        [TestCase(FieldEndDateField)]
        [TestCase(FieldFundingSourceField)]
        [TestCase(FieldNameField)]
        [TestCase(FieldPortfolioField)]
        [TestCase(FieldPortfolioIdField)]
        [TestCase(FieldStartDateField)]
        [TestCase(FieldSystemDataObjectsDataClassesIEntityWithKeyEntityKeyField)]
        [TestCase(FieldUniqueIdField)]
        public void AUT_PortfolioBudget_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            this.ValidateExecuteCondition(name);
            var fieldInfo = this.GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_portfolioBudgetInstanceFixture, 
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
        ///     Class (<see cref="PortfolioBudget" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Instance")]
        public void AUT_PortfolioBudget_Is_Instance_Present_Test()
        {
            // Assert
            _portfolioBudgetInstanceType.ShouldNotBeNull();
            _portfolioBudgetInstance.ShouldNotBeNull();
            _portfolioBudgetInstanceFixture.ShouldNotBeNull();
            _portfolioBudgetInstance.ShouldBeAssignableTo<PortfolioBudget>();
            _portfolioBudgetInstanceFixture.ShouldBeAssignableTo<PortfolioBudget>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (PortfolioBudget) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Constructor")]
        public void AUT_Constructor_PortfolioBudget_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            PortfolioBudget instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _portfolioBudgetInstanceType.ShouldNotBeNull();
            _portfolioBudgetInstance.ShouldNotBeNull();
            _portfolioBudgetInstanceFixture.ShouldNotBeNull();
            _portfolioBudgetInstance.ShouldBeAssignableTo<PortfolioBudget>();
            _portfolioBudgetInstanceFixture.ShouldBeAssignableTo<PortfolioBudget>();
        }

        #endregion

        #endregion

        #region Category : GetterSetter

        #region General Getters/Setters : Class (PortfolioBudget) => all properties (non-static) explore and verify type tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        [TestCaseGeneric(typeof(System.Runtime.Serialization.ExtensionDataObject) , PropertyExtensionData)]
        [TestCaseGeneric(typeof(UplandIntegrations.TenroxAssignmentService.PortfolioAllocatedBudget[]) , PropertyAllocatedBudgets)]
        [TestCaseGeneric(typeof(System.Nullable<decimal>) , PropertyBudgetAmount)]
        [TestCaseGeneric(typeof(string) , PropertyDescription)]
        [TestCaseGeneric(typeof(System.DateTime) , PropertyEndDate)]
        [TestCaseGeneric(typeof(System.Nullable<int>) , PropertyFundingSource)]
        [TestCaseGeneric(typeof(string) , PropertyName)]
        [TestCaseGeneric(typeof(UplandIntegrations.TenroxAssignmentService.Portfolio) , PropertyPortfolio)]
        [TestCaseGeneric(typeof(int) , PropertyPortfolioId)]
        [TestCaseGeneric(typeof(System.DateTime) , PropertyStartDate)]
        [TestCaseGeneric(typeof(UplandIntegrations.TenroxAssignmentService.EntityKey) , PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey)]
        [TestCaseGeneric(typeof(int) , PropertyUniqueId)]
        public void AUT_PortfolioBudget_Property_Type_Verify_Explore_By_Name_Test<T>(string name)
        {
            // Arrange
            this.ValidateExecuteCondition(name);

            // Assert
            ShouldlyExtension.PropertyTypeVerify<PortfolioBudget, T>(_portfolioBudgetInstance, name, Fixture);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (AllocatedBudgets) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_AllocatedBudgets_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyAllocatedBudgets);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertyAllocatedBudgets);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (AllocatedBudgets) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_AllocatedBudgets_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyAllocatedBudgets);
            var propertyInfo  = this.GetPropertyInfo(PropertyAllocatedBudgets);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (BudgetAmount) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_BudgetAmount_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyBudgetAmount);
            var propertyInfo  = this.GetPropertyInfo(PropertyBudgetAmount);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (Description) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_Description_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyDescription);
            var propertyInfo  = this.GetPropertyInfo(PropertyDescription);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (EndDate) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_EndDate_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyEndDate);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertyEndDate);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (EndDate) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_EndDate_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyEndDate);
            var propertyInfo  = this.GetPropertyInfo(PropertyEndDate);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (ExtensionData) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_ExtensionData_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyExtensionData);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertyExtensionData);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (ExtensionData) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_ExtensionData_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyExtensionData);
            var propertyInfo  = this.GetPropertyInfo(PropertyExtensionData);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (FundingSource) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_FundingSource_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyFundingSource);
            var propertyInfo  = this.GetPropertyInfo(PropertyFundingSource);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (Name) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_Name_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyName);
            var propertyInfo  = this.GetPropertyInfo(PropertyName);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (Portfolio) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Portfolio_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyPortfolio);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertyPortfolio);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (Portfolio) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_Portfolio_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyPortfolio);
            var propertyInfo  = this.GetPropertyInfo(PropertyPortfolio);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (PortfolioId) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_PortfolioId_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyPortfolioId);
            var propertyInfo  = this.GetPropertyInfo(PropertyPortfolioId);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (StartDate) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_StartDate_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyStartDate);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertyStartDate);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (StartDate) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_StartDate_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyStartDate);
            var propertyInfo  = this.GetPropertyInfo(PropertyStartDate);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (SystemDataObjectsDataClassesIEntityWithKeyEntityKey) Property Type Test Except String

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_SystemDataObjectsDataClassesIEntityWithKeyEntityKey_Property_Setting_String_Throw_Argument_Exception_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey);
            var randomString = this.CreateType<string>();

            // Act
            var propertyInfo = this.GetPropertyInfo(PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey);
            Action currentAction = () => propertyInfo.SetValue(_portfolioBudgetInstance, randomString, null);

            // Assert
            propertyInfo.ShouldNotBeNull();
            Should.Throw<ArgumentException>(currentAction);
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (SystemDataObjectsDataClassesIEntityWithKeyEntityKey) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_SystemDataObjectsDataClassesIEntityWithKeyEntityKey_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey);
            var propertyInfo  = this.GetPropertyInfo(PropertySystemDataObjectsDataClassesIEntityWithKeyEntityKey);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #region General Getters/Setters : Class (PortfolioBudget) => Property (UniqueId) (Can Read) tests

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT GetterSetter")]
        public void AUT_PortfolioBudget_Public_Class_UniqueId_Coverage_For_Property_Is_Present_And_Can_Read_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(PropertyUniqueId);
            var propertyInfo  = this.GetPropertyInfo(PropertyUniqueId);

            // Act
            var canRead = propertyInfo?.CanRead;

            // Assert
            propertyInfo.ShouldNotBeNull();
            canRead.ShouldNotBeNull();
            canRead?.ShouldBeTrue();
        }

        #endregion

        #endregion

        #region Category : MethodCallTest

        #region Method Call : (RaisePropertyChanged) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_Internally(Type[] types)
        {
            var methodRaisePropertyChangedPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_portfolioBudgetInstance, MethodRaisePropertyChanged, Fixture, methodRaisePropertyChangedPrametersTypes);
        }

        #endregion

        #region Method Call : (RaisePropertyChanged) (Return Type : void) No Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_Void_With_No_Parameters_No_Exception_Thrown_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(MethodRaisePropertyChanged);
            var propertyName = this.CreateType<string>();
            var methodRaisePropertyChangedPrametersTypes = new Type[] { typeof(string) };
            object[] parametersOfRaisePropertyChanged = { propertyName };
            Exception exception = null;
            var methodInfo = this.GetMethodInfo(MethodRaisePropertyChanged, methodRaisePropertyChangedPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_portfolioBudgetInstanceFixture, parametersOfRaisePropertyChanged);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfRaisePropertyChanged.ShouldNotBeNull();
            parametersOfRaisePropertyChanged.Length.ShouldBe(1);
            methodRaisePropertyChangedPrametersTypes.Length.ShouldBe(1);
            methodRaisePropertyChangedPrametersTypes.Length.ShouldBe(parametersOfRaisePropertyChanged.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (RaisePropertyChanged) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_Void_With_1_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(MethodRaisePropertyChanged);
            var propertyName = this.CreateType<string>();
            var methodRaisePropertyChangedPrametersTypes = new Type[] { typeof(string) };
            object[] parametersOfRaisePropertyChanged = { propertyName };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_portfolioBudgetInstance, MethodRaisePropertyChanged, parametersOfRaisePropertyChanged, methodRaisePropertyChangedPrametersTypes);

            // Assert
            parametersOfRaisePropertyChanged.ShouldNotBeNull();
            parametersOfRaisePropertyChanged.Length.ShouldBe(1);
            methodRaisePropertyChangedPrametersTypes.Length.ShouldBe(1);
            methodRaisePropertyChangedPrametersTypes.Length.ShouldBe(parametersOfRaisePropertyChanged.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (RaisePropertyChanged) (Return Type : void) Parameters Count verify

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_Parameters_Count_Verification_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(MethodRaisePropertyChanged);
            var methodInfo = this.GetMethodInfo(MethodRaisePropertyChanged, 0);
            const int parametersCount = 1;

            // Act
            var parameters = methodInfo.GetParameters();

            // Assert
            parameters.Length.ShouldBe(parametersCount);
        }

        #endregion

        #region Method Call : (RaisePropertyChanged) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(MethodRaisePropertyChanged);
            var methodRaisePropertyChangedPrametersTypes = new Type[] { typeof(string) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_portfolioBudgetInstance, MethodRaisePropertyChanged, Fixture, methodRaisePropertyChangedPrametersTypes);

            // Assert
            methodRaisePropertyChangedPrametersTypes.Length.ShouldBe(1);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (RaisePropertyChanged) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PortfolioBudget_RaisePropertyChanged_Method_Call_With_1_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            this.ValidateExecuteCondition(MethodRaisePropertyChanged);
            Exception exception;
            var methodInfo = this.GetMethodInfo(MethodRaisePropertyChanged, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_portfolioBudgetInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #endregion

        #endregion
    }
}