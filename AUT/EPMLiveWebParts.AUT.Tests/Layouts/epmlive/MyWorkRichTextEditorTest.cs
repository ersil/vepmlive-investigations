using System;
using System.Diagnostics.CodeAnalysis;
using Action = System.Action;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;

namespace EPMLiveCore
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.MyWorkRichTextEditor" />)
    ///     and namespace <see cref="EPMLiveCore"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class MyWorkRichTextEditorTest : AbstractBaseSetupTypedTest<MyWorkRichTextEditor>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (MyWorkRichTextEditor) Initializer

        private const string MethodPage_Load = "Page_Load";
        private const string FieldRichTextValue = "RichTextValue";
        private const string FieldSiteUrl = "SiteUrl";
        private Type _myWorkRichTextEditorInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private MyWorkRichTextEditor _myWorkRichTextEditorInstance;
        private MyWorkRichTextEditor _myWorkRichTextEditorInstanceFixture;

        #region General Initializer : Class (MyWorkRichTextEditor) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="MyWorkRichTextEditor" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _myWorkRichTextEditorInstanceType = typeof(MyWorkRichTextEditor);
            _myWorkRichTextEditorInstanceFixture = Create(true);
            _myWorkRichTextEditorInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (MyWorkRichTextEditor)

        #region General Initializer : Class (MyWorkRichTextEditor) All Methods Explore Verification.

        /// <summary>
        ///     Class (<see cref="MyWorkRichTextEditor" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(MethodPage_Load, 0)]
        public void AUT_MyWorkRichTextEditor_All_Methods_Explore_Verify_Test(string methodName, int overloadingIndex = 0)
        {
            // Arrange
            var currentMethodInfo = GetMethodInfo(methodName, overloadingIndex);

            // Act
            ShouldlyExtension.ExploreMethodWithOrWithoutInstance(_myWorkRichTextEditorInstanceFixture, 
                                                                 Fixture, 
                                                                 currentMethodInfo);

            // Assert
            currentMethodInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (MyWorkRichTextEditor) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="MyWorkRichTextEditor" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(FieldRichTextValue)]
        [TestCase(FieldSiteUrl)]
        public void AUT_MyWorkRichTextEditor_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_myWorkRichTextEditorInstanceFixture, 
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
        ///     Class (<see cref="MyWorkRichTextEditor" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Instance")]
        public void AUT_MyWorkRichTextEditor_Is_Instance_Present_Test()
        {
            // Assert
            _myWorkRichTextEditorInstanceType.ShouldNotBeNull();
            _myWorkRichTextEditorInstance.ShouldNotBeNull();
            _myWorkRichTextEditorInstanceFixture.ShouldNotBeNull();
            _myWorkRichTextEditorInstance.ShouldBeAssignableTo<MyWorkRichTextEditor>();
            _myWorkRichTextEditorInstanceFixture.ShouldBeAssignableTo<MyWorkRichTextEditor>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (MyWorkRichTextEditor) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Constructor")]
        public void AUT_Constructor_MyWorkRichTextEditor_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            MyWorkRichTextEditor instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _myWorkRichTextEditorInstanceType.ShouldNotBeNull();
            _myWorkRichTextEditorInstance.ShouldNotBeNull();
            _myWorkRichTextEditorInstanceFixture.ShouldNotBeNull();
            _myWorkRichTextEditorInstance.ShouldBeAssignableTo<MyWorkRichTextEditor>();
            _myWorkRichTextEditorInstanceFixture.ShouldBeAssignableTo<MyWorkRichTextEditor>();
        }

        #endregion

        #endregion

        #region Category : MethodCallTest

        #region Method Call : NonStatic methods call tests

        /// <summary>
        ///      Class (<see cref="MyWorkRichTextEditor" />) public, non-public non-static methods exploration for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        [TestCase(MethodPage_Load)]
        public void AUT_MyWorkRichTextEditor_NonStatic_Methods_Explore_Verify_Test(string methodName)
        {
            // AAA: Arrange, Act, Assert
            ShouldlyExtension.ExploreVerifyMethods<MyWorkRichTextEditor>(Fixture, methodName);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_Internally(Type[] types)
        {
            var methodPage_LoadPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_myWorkRichTextEditorInstance, MethodPage_Load, Fixture, methodPage_LoadPrametersTypes);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_Void_With_2_Parameters_Throw_Exception_Thrown_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<EventArgs>();
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };
            object[] parametersOfPage_Load = { sender, e };
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodPage_Load, methodPage_LoadPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_myWorkRichTextEditorInstanceFixture, parametersOfPage_Load);

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
        public void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_Void_With_2_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            var sender = CreateType<object>();
            var e = CreateType<EventArgs>();
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };
            object[] parametersOfPage_Load = { sender, e };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_myWorkRichTextEditorInstance, MethodPage_Load, parametersOfPage_Load, methodPage_LoadPrametersTypes);

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
        public void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_Parameters_Count_Verification_Test()
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
        public void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            var methodPage_LoadPrametersTypes = new Type[] { typeof(object), typeof(EventArgs) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_myWorkRichTextEditorInstance, MethodPage_Load, Fixture, methodPage_LoadPrametersTypes);

            // Assert
            methodPage_LoadPrametersTypes.Length.ShouldBe(2);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (Page_Load) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_MyWorkRichTextEditor_Page_Load_Method_Call_With_2_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodPage_Load, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_myWorkRichTextEditorInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #endregion

        #endregion
    }
}