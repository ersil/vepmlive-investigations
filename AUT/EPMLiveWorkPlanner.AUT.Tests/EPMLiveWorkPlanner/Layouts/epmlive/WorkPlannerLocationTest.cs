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
using EPMLiveWorkPlanner.Layouts.epmlive;
using WorkPlannerLocation = EPMLiveWorkPlanner.Layouts.epmlive;

namespace EPMLiveWorkPlanner.Layouts.epmlive
{
    /// <summary>
    ///     Automatic Unit Tests or bulk unit tests for (<see cref="EPMLiveWorkPlanner.Layouts.epmlive.WorkPlannerLocation" />)
    ///     and namespace <see cref="EPMLiveWorkPlanner.Layouts.epmlive"/> class using generator(v:0.2.2)'s 
    ///     artificial intelligence. Compatible with <see cref="AUT.ConfigureTestProjects" /> v4.2.6.
    /// </summary>
    [TestFixture]
    [ExcludeFromCodeCoverage]
    public partial class WorkPlannerLocationTest : AbstractBaseSetupV3Test
    {
        public WorkPlannerLocationTest() : base(typeof(WorkPlannerLocation))
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

        #region General Initializer : Class (WorkPlannerLocation) Initializer

        private Type _workPlannerLocationInstanceType;
        private const int TestsTimeOut = TestContants.TimeOutEightSeconds;
        private WorkPlannerLocation _workPlannerLocationInstance;
        private WorkPlannerLocation _workPlannerLocationInstanceFixture;

        #region General Initializer : Class (WorkPlannerLocation) One time setup

        /// <summary>
        ///    Setting up everything for <see cref="WorkPlannerLocation" /> one time.
        /// </summary>
        [OneTimeSetUp]
        [ExcludeFromCodeCoverage]
        public void OneTimeSetup()
        {
            _workPlannerLocationInstanceType = typeof(WorkPlannerLocation);
            _workPlannerLocationInstanceFixture = this.Create<WorkPlannerLocation>(true);
            _workPlannerLocationInstance = _workPlannerLocationInstanceFixture ?? this.Create<WorkPlannerLocation>(false);
            CurrentInstance = _workPlannerLocationInstanceFixture;
            ConfigureIgnoringTests();
        }

        #endregion

        #endregion

        #endregion

        #region Category : Instance

        /// <summary>
        ///     Class (<see cref="WorkPlannerLocation" />) can be created test
        /// </summary>
        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Instance")]
        public void AUT_WorkPlannerLocation_Is_Instance_Present_Test()
        {
            // Assert
            _workPlannerLocationInstanceType.ShouldNotBeNull();
            _workPlannerLocationInstance.ShouldNotBeNull();
            _workPlannerLocationInstanceFixture.ShouldNotBeNull();
            _workPlannerLocationInstance.ShouldBeAssignableTo<WorkPlannerLocation>();
            _workPlannerLocationInstanceFixture.ShouldBeAssignableTo<WorkPlannerLocation>();
        }

        #endregion

        #region Category : Constructor

        #region General Constructor : Class (WorkPlannerLocation) without Parameter Test

        [Test]
        [Timeout(TestsTimeOut)]
        [Category("AUT Constructor")]
        public void AUT_Constructor_WorkPlannerLocation_Instantiated_Without_Parameter_No_Throw_Exception_Test()
        {
            // Arrange
            WorkPlannerLocation instance = null;

            // Act
            var exception = CreateAnalyzer.GetThrownExceptionWhenCreate(out instance);

            // Assert
            instance.ShouldNotBeNull();
            exception.ShouldBeNull();
            _workPlannerLocationInstanceType.ShouldNotBeNull();
            _workPlannerLocationInstance.ShouldNotBeNull();
            _workPlannerLocationInstanceFixture.ShouldNotBeNull();
            _workPlannerLocationInstance.ShouldBeAssignableTo<WorkPlannerLocation>();
            _workPlannerLocationInstanceFixture.ShouldBeAssignableTo<WorkPlannerLocation>();
        }

        #endregion

        #endregion

        #endregion
    }
}