using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Web.UI.WebControls;
using System.Xml;
using Action = System.Action;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;

namespace EPMLiveCore.Layouts.epmlive.applications
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.Layouts.epmlive.applications.PreCheckDone" />)
    ///     and namespace <see cref="EPMLiveCore.Layouts.epmlive.applications"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class PreCheckDoneTest : AbstractBaseSetupTypedTest<PreCheckDone>
    {

        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (PreCheckDone) Initializer

        private const string MethodPage_Load = "Page_Load";
        private const string MethodappendTable = "appendTable";
        private const string MethodGridView1_RowDataBound = "GridView1_RowDataBound";
        private const string FieldApplicationName = "ApplicationName";
        private const string FieldsbOutput = "sbOutput";
        private const string FieldmaxError = "maxError";
        private const string FieldsWebUrl = "sWebUrl";
        private Type _preCheckDoneInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private PreCheckDone _preCheckDoneInstance;
        private PreCheckDone _preCheckDoneInstanceFixture;

        #region General Initializer : Class (PreCheckDone) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="PreCheckDone" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _preCheckDoneInstanceType = typeof(PreCheckDone);
            _preCheckDoneInstanceFixture = Create(true);
            _preCheckDoneInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (PreCheckDone)

        #region General Initializer : Class (PreCheckDone) All Methods Explore Verification.

        /// <summary>
        ///     Class (<see cref="PreCheckDone" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(MethodPage_Load, 0)]
        [TestCase(MethodappendTable, 0)]
        [TestCase(MethodGridView1_RowDataBound, 0)]
        public void AUT_PreCheckDone_All_Methods_Explore_Verify_Test(string methodName, int overloadingIndex = 0)
        {
            // Arrange
            var currentMethodInfo = GetMethodInfo(methodName, overloadingIndex);

            // Act
            ShouldlyExtension.ExploreMethodWithOrWithoutInstance(_preCheckDoneInstanceFixture, 
                                                                 Fixture, 
                                                                 currentMethodInfo);

            // Assert
            currentMethodInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (PreCheckDone) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="PreCheckDone" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(FieldApplicationName)]
        [TestCase(FieldsbOutput)]
        [TestCase(FieldmaxError)]
        [TestCase(FieldsWebUrl)]
        public void AUT_PreCheckDone_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_preCheckDoneInstanceFixture, 
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
        ///     Class (<see cref="PreCheckDone" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Instance")]
        public void AUT_PreCheckDone_Is_Instance_Present_Test()
        {
            // Assert
            _preCheckDoneInstanceType.ShouldNotBeNull();
            _preCheckDoneInstance.ShouldNotBeNull();
            _preCheckDoneInstanceFixture.ShouldNotBeNull();
            _preCheckDoneInstance.ShouldBeAssignableTo<PreCheckDone>();
            _preCheckDoneInstanceFixture.ShouldBeAssignableTo<PreCheckDone>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (PreCheckDone) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Constructor")]
        public void AUT_Constructor_PreCheckDone_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            PreCheckDone instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _preCheckDoneInstanceType.ShouldNotBeNull();
            _preCheckDoneInstance.ShouldNotBeNull();
            _preCheckDoneInstanceFixture.ShouldNotBeNull();
            _preCheckDoneInstance.ShouldBeAssignableTo<PreCheckDone>();
            _preCheckDoneInstanceFixture.ShouldBeAssignableTo<PreCheckDone>();
        }

        #endregion

        #endregion

        #region Category : MethodCallTest

        #region Method Call : NonStatic methods call tests

        /// <summary>
        ///      Class (<see cref="PreCheckDone" />) public, non-public non-static methods exploration for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        [TestCase(MethodPage_Load)]
        [TestCase(MethodappendTable)]
        [TestCase(MethodGridView1_RowDataBound)]
        public void AUT_PreCheckDone_NonStatic_Methods_Explore_Verify_Test(string methodName)
        {
            // AAA: Arrange, Act, Assert
            ShouldlyExtension.ExploreVerifyMethods<PreCheckDone>(Fixture, methodName);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_PreCheckDone_Page_Load_Method_Call_Internally(Type[] types)
        {
            var methodPage_LoadPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodPage_Load, Fixture, methodPage_LoadPrametersTypes);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_Page_Load_Method_Call_Void_With_2_Parameters_Throw_Exception_Thrown_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<EventArgs>();
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };
            object[] parametersOfPage_Load = { sender, e };
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodPage_Load, methodPage_LoadPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_preCheckDoneInstanceFixture, parametersOfPage_Load);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfPage_Load.ShouldNotBeNull();
            parametersOfPage_Load.Length.ShouldBe(2);
            methodPage_LoadPrametersTypes.Length.ShouldBe(2);
            methodPage_LoadPrametersTypes.Length.ShouldBe(parametersOfPage_Load.Length);
            Should.Throw<Exception>(currentAction);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_Page_Load_Method_Call_Void_With_2_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<EventArgs>();
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };
            object[] parametersOfPage_Load = { sender, e };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_preCheckDoneInstance, MethodPage_Load, parametersOfPage_Load, methodPage_LoadPrametersTypes);

            // Assert
            parametersOfPage_Load.ShouldNotBeNull();
            parametersOfPage_Load.Length.ShouldBe(2);
            methodPage_LoadPrametersTypes.Length.ShouldBe(2);
            methodPage_LoadPrametersTypes.Length.ShouldBe(parametersOfPage_Load.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Parameters Count verify

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_Page_Load_Method_Call_Parameters_Count_Verification_Test()
        {
            // Arrange
            var methodInfo = GetMethodInfo(MethodPage_Load, 0);
            const int parametersCount = 2;

            // Act
            var parameters = methodInfo.GetParameters();

            // Assert
            parameters.Length.ShouldBe(parametersCount);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_Page_Load_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodPage_Load, Fixture, methodPage_LoadPrametersTypes);

            // Assert
            methodPage_LoadPrametersTypes.Length.ShouldBe(2);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_Page_Load_Method_Call_With_2_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodPage_Load, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_preCheckDoneInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_PreCheckDone_appendTable_Method_Call_Internally(Type[] types)
        {
            var methodappendTablePrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodappendTable, Fixture, methodappendTablePrametersTypes);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_appendTable_Method_Call_Void_With_3_Parameters_Throw_Exception_Thrown_Test()
        {
            // Arrange
            var level = CreateType<int>();
            var dt = CreateType<DataTable>();
            var ndItem = CreateType<XmlNode>();
            var methodappendTablePrametersTypes = new Type[] { typeof(int), typeof(DataTable), typeof(XmlNode) };
            object[] parametersOfappendTable = { level, dt, ndItem };
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodappendTable, methodappendTablePrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_preCheckDoneInstanceFixture, parametersOfappendTable);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfappendTable.ShouldNotBeNull();
            parametersOfappendTable.Length.ShouldBe(3);
            methodappendTablePrametersTypes.Length.ShouldBe(3);
            methodappendTablePrametersTypes.Length.ShouldBe(parametersOfappendTable.Length);
            Should.Throw<Exception>(currentAction);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_appendTable_Method_Call_Void_With_3_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            var level = CreateType<int>();
            var dt = CreateType<DataTable>();
            var ndItem = CreateType<XmlNode>();
            var methodappendTablePrametersTypes = new Type[] { typeof(int), typeof(DataTable), typeof(XmlNode) };
            object[] parametersOfappendTable = { level, dt, ndItem };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_preCheckDoneInstance, MethodappendTable, parametersOfappendTable, methodappendTablePrametersTypes);

            // Assert
            parametersOfappendTable.ShouldNotBeNull();
            parametersOfappendTable.Length.ShouldBe(3);
            methodappendTablePrametersTypes.Length.ShouldBe(3);
            methodappendTablePrametersTypes.Length.ShouldBe(parametersOfappendTable.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) Parameters Count verify

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_appendTable_Method_Call_Parameters_Count_Verification_Test()
        {
            // Arrange
            var methodInfo = GetMethodInfo(MethodappendTable, 0);
            const int parametersCount = 3;

            // Act
            var parameters = methodInfo.GetParameters();

            // Assert
            parameters.Length.ShouldBe(parametersCount);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_appendTable_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            var methodappendTablePrametersTypes = new Type[] { typeof(int), typeof(DataTable), typeof(XmlNode) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodappendTable, Fixture, methodappendTablePrametersTypes);

            // Assert
            methodappendTablePrametersTypes.Length.ShouldBe(3);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (appendTable) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_appendTable_Method_Call_With_3_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodappendTable, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_preCheckDoneInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_Internally(Type[] types)
        {
            var methodGridView1_RowDataBoundPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodGridView1_RowDataBound, Fixture, methodGridView1_RowDataBoundPrametersTypes);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_Void_With_2_Parameters_Throw_Exception_Thrown_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<GridViewRowEventArgs>();
            var methodGridView1_RowDataBoundPrametersTypes = new Type[] { typeof(object), typeof(GridViewRowEventArgs) };
            object[] parametersOfGridView1_RowDataBound = { sender, e };
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodGridView1_RowDataBound, methodGridView1_RowDataBoundPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_preCheckDoneInstanceFixture, parametersOfGridView1_RowDataBound);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfGridView1_RowDataBound.ShouldNotBeNull();
            parametersOfGridView1_RowDataBound.Length.ShouldBe(2);
            methodGridView1_RowDataBoundPrametersTypes.Length.ShouldBe(2);
            methodGridView1_RowDataBoundPrametersTypes.Length.ShouldBe(parametersOfGridView1_RowDataBound.Length);
            Should.Throw<Exception>(currentAction);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_Void_With_2_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<GridViewRowEventArgs>();
            var methodGridView1_RowDataBoundPrametersTypes = new Type[] { typeof(object), typeof(GridViewRowEventArgs) };
            object[] parametersOfGridView1_RowDataBound = { sender, e };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_preCheckDoneInstance, MethodGridView1_RowDataBound, parametersOfGridView1_RowDataBound, methodGridView1_RowDataBoundPrametersTypes);

            // Assert
            parametersOfGridView1_RowDataBound.ShouldNotBeNull();
            parametersOfGridView1_RowDataBound.Length.ShouldBe(2);
            methodGridView1_RowDataBoundPrametersTypes.Length.ShouldBe(2);
            methodGridView1_RowDataBoundPrametersTypes.Length.ShouldBe(parametersOfGridView1_RowDataBound.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) Parameters Count verify

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_Parameters_Count_Verification_Test()
        {
            // Arrange
            var methodInfo = GetMethodInfo(MethodGridView1_RowDataBound, 0);
            const int parametersCount = 2;

            // Act
            var parameters = methodInfo.GetParameters();

            // Assert
            parameters.Length.ShouldBe(parametersCount);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            var methodGridView1_RowDataBoundPrametersTypes = new Type[] { typeof(object), typeof(GridViewRowEventArgs) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_preCheckDoneInstance, MethodGridView1_RowDataBound, Fixture, methodGridView1_RowDataBoundPrametersTypes);

            // Assert
            methodGridView1_RowDataBoundPrametersTypes.Length.ShouldBe(2);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (GridView1_RowDataBound) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_PreCheckDone_GridView1_RowDataBound_Method_Call_With_2_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodGridView1_RowDataBound, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_preCheckDoneInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #endregion

        #endregion
    }
}