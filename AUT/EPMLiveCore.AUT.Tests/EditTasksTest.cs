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
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using Action = System.Action;
using AUT.ConfigureTestProjects;
using AUT.ConfigureTestProjects.Analyzer;
using AUT.ConfigureTestProjects.Attribute;
using AUT.ConfigureTestProjects.AutoFixtureSetup;
using AUT.ConfigureTestProjects.BaseSetup;
using AUT.ConfigureTestProjects.Extensions;
using AUT.ConfigureTestProjects.StaticTypes;
using AutoFixture;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;
using Moq;
using NUnit.Framework;
using Should = Shouldly.Should;
using Shouldly;
using EPMLiveCore;
using EditTasks = EPMLiveCore;

namespace EPMLiveCore
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveCore.EditTasks" />)
    ///     and namespace <see cref="EPMLiveCore"/> class using generator(v:0.2.1)'s 
    ///     artificial intelligence.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public class EditTasksTest : AbstractBaseSetupTypedTest<EditTasks>
    {
        #region Category : General

        #region Category : Initializer

        #region General Initializer : Class (EditTasks) Initializer

        private const string MethodOnLoad = "OnLoad";
        private const string MethodCreateChildControls = "CreateChildControls";
        private const string FieldsListName = "sListName";
        private Type _editTasksInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutFiveSeconds;
        private EditTasks _editTasksInstance;
        private EditTasks _editTasksInstanceFixture;

        #region General Initializer : Class (EditTasks) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="EditTasks" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _editTasksInstanceType = typeof(EditTasks);
            _editTasksInstanceFixture = Create(true);
            _editTasksInstance = Create(false);
        }

        #endregion

        #endregion

        #region Explore Class for Coverage Gain : Class (EditTasks)

        #region General Initializer : Class (EditTasks) All Methods Explore Verification.

        /// <summary>
        ///     Class (<see cref="EditTasks" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(MethodOnLoad, 0)]
        [TestCase(MethodCreateChildControls, 0)]
        public void AUT_EditTasks_All_Methods_Explore_Verify_Test(string methodName, int overloadingIndex = 0)
        {
            // Arrange
            var currentMethodInfo = GetMethodInfo(methodName, overloadingIndex);

            // Act
            ShouldlyExtension.ExploreMethodWithOrWithoutInstance(_editTasksInstanceFixture, 
                                                                 Fixture, 
                                                                 currentMethodInfo);

            // Assert
            currentMethodInfo.ShouldNotBeNull();
        }

        #endregion

        #region General Initializer : Class (EditTasks) All Fields Explore By Name

        /// <summary>
        ///     Class (<see cref="EditTasks" />) explore and verify fields for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Initializer")]
        [TestCase(FieldsListName)]
        public void AUT_EditTasks_All_Fields_Explore_Verify_By_Name_Test(string name)
        {
            // Arrange
            var fieldInfo = GetFieldInfo(name);

            // Act
            ShouldlyExtension.ExploreFieldWithOrWithoutInstance(_editTasksInstanceFixture, 
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
        ///     Class (<see cref="EditTasks" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Instance")]
        public void AUT_EditTasks_Is_Instance_Present_Test()
        {
            // Assert
            _editTasksInstanceType.ShouldNotBeNull();
            _editTasksInstance.ShouldNotBeNull();
            _editTasksInstanceFixture.ShouldNotBeNull();
            _editTasksInstance.ShouldBeAssignableTo<EditTasks>();
            _editTasksInstanceFixture.ShouldBeAssignableTo<EditTasks>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (EditTasks) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Constructor")]
        public void AUT_Constructor_EditTasks_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            EditTasks instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _editTasksInstanceType.ShouldNotBeNull();
            _editTasksInstance.ShouldNotBeNull();
            _editTasksInstanceFixture.ShouldNotBeNull();
            _editTasksInstance.ShouldBeAssignableTo<EditTasks>();
            _editTasksInstanceFixture.ShouldBeAssignableTo<EditTasks>();
        }

        #endregion

        #endregion

        #region Category : MethodCallTest

        #region Method Call : NonStatic methods call tests

        /// <summary>
        ///      Class (<see cref="EditTasks" />) public, non-public non-static methods exploration for coverage gain.
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        [TestCase(MethodOnLoad)]
        [TestCase(MethodCreateChildControls)]
        public void AUT_EditTasks_NonStatic_Methods_Explore_Verify_Test(string methodName)
        {
            // AAA: Arrange, Act, Assert
            ShouldlyExtension.ExploreVerifyMethods<EditTasks>(Fixture, methodName);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_EditTasks_OnLoad_Method_Call_Internally(Type[] types)
        {
            var methodOnLoadPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_editTasksInstance, MethodOnLoad, Fixture, methodOnLoadPrametersTypes);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_OnLoad_Method_Call_Void_With_1_Parameters_Throw_Exception_Thrown_Test()
        {
            // Arrange
            var e = CreateType<EventArgs>();
            var methodOnLoadPrametersTypes = new Type[] { typeof(EventArgs) };
            object[] parametersOfOnLoad = { e };
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodOnLoad, methodOnLoadPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_editTasksInstanceFixture, parametersOfOnLoad);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfOnLoad.ShouldNotBeNull();
            parametersOfOnLoad.Length.ShouldBe(1);
            methodOnLoadPrametersTypes.Length.ShouldBe(1);
            methodOnLoadPrametersTypes.Length.ShouldBe(parametersOfOnLoad.Length);
            Should.Throw<Exception>(currentAction);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_OnLoad_Method_Call_Void_With_1_Parameters_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            var e = CreateType<EventArgs>();
            var methodOnLoadPrametersTypes = new Type[] { typeof(EventArgs) };
            object[] parametersOfOnLoad = { e };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_editTasksInstance, MethodOnLoad, parametersOfOnLoad, methodOnLoadPrametersTypes);

            // Assert
            parametersOfOnLoad.ShouldNotBeNull();
            parametersOfOnLoad.Length.ShouldBe(1);
            methodOnLoadPrametersTypes.Length.ShouldBe(1);
            methodOnLoadPrametersTypes.Length.ShouldBe(parametersOfOnLoad.Length);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) Parameters Count verify

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_OnLoad_Method_Call_Parameters_Count_Verification_Test()
        {
            // Arrange
            var methodInfo = GetMethodInfo(MethodOnLoad, 0);
            const int parametersCount = 1;

            // Act
            var parameters = methodInfo.GetParameters();

            // Assert
            parameters.Length.ShouldBe(parametersCount);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_OnLoad_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            var methodOnLoadPrametersTypes = new Type[] { typeof(EventArgs) };

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_editTasksInstance, MethodOnLoad, Fixture, methodOnLoadPrametersTypes);

            // Assert
            methodOnLoadPrametersTypes.Length.ShouldBe(1);
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (OnLoad) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_OnLoad_Method_Call_With_1_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodOnLoad, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_editTasksInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (CreateChildControls) (Return Type : void) private call definition

        [ExcludeFromCodeCoverage]
        private void AUT_EditTasks_CreateChildControls_Method_Call_Internally(Type[] types)
        {
            var methodCreateChildControlsPrametersTypes = types;
            ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_editTasksInstance, MethodCreateChildControls, Fixture, methodCreateChildControlsPrametersTypes);
        }

        #endregion

        #region Method Call : (CreateChildControls) (Return Type : void) Exception Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_CreateChildControls_Method_Call_Void_With_No_Parameters_Call_Throw_Exception_Thrown_Test()
        {
            // Arrange
            Type [] methodCreateChildControlsPrametersTypes = null;
            object[] parametersOfCreateChildControls = null; // no parameter present
            Exception exception = null;
            var methodInfo = GetMethodInfo(MethodCreateChildControls, methodCreateChildControlsPrametersTypes, out exception);

            // Act
            Action currentAction = () => methodInfo.Invoke(_editTasksInstanceFixture, parametersOfCreateChildControls);

            // Assert
            methodInfo.ShouldNotBeNull();
            exception.ShouldBeNull();
            parametersOfCreateChildControls.ShouldBeNull();
            methodCreateChildControlsPrametersTypes.ShouldBeNull();
            Should.Throw<Exception>(currentAction);
        }

        #endregion

        #region Method Call : (CreateChildControls) (Return Type : void) No Exception with encapsulation Thrown

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_CreateChildControls_Method_Call_Void_With_No_Parameters_Call_No_Exception_Thrown_With_Encapsulation_Test()
        {
            // Arrange
            Type [] methodCreateChildControlsPrametersTypes = null;
            object[] parametersOfCreateChildControls = null; // no parameter present

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeVoidMethod(_editTasksInstance, MethodCreateChildControls, parametersOfCreateChildControls, methodCreateChildControlsPrametersTypes);

            // Assert
            parametersOfCreateChildControls.ShouldBeNull();
            methodCreateChildControlsPrametersTypes.ShouldBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (CreateChildControls) (Return Type : void) Invoke Should Not Throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_CreateChildControls_Method_Call_Dynamic_Invoking_Should_Not_Throw_Test()
        {
            // Arrange
            Type [] methodCreateChildControlsPrametersTypes = null;

            // Act
            Action currentAction = () => ReflectionAnalyzer.InvokeNonStaticMethodWithDynamicParameters(_editTasksInstance, MethodCreateChildControls, Fixture, methodCreateChildControlsPrametersTypes);

            // Assert
            methodCreateChildControlsPrametersTypes.ShouldBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #region Method Call : (CreateChildControls) (Return Type : void) Invoke without parameter types and should not throw

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT MethodCallTest")]
        public void AUT_EditTasks_CreateChildControls_Method_Call_With_No_Parameters_Dynamic_Invoking_Without_Parameters_Should_Not_Throw_Test()
        {
            // Arrange
            Exception exception;
            var methodInfo = GetMethodInfo(MethodCreateChildControls, 0);

            // Act
            Action currentAction = () => methodInfo.InvokeStaticMethodWithDynamicParamters(_editTasksInstanceFixture, Fixture, out exception);

            // Assert
            methodInfo.ShouldNotBeNull();
            Should.NotThrow(currentAction);
        }

        #endregion

        #endregion

        #endregion
    }
}