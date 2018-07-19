﻿using System;
using System.Data.SqlClient.Fakes;
using System.Data;
using EPMLiveCore.ReportHelper;
using EPMLiveCore.ReportHelper.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EPMLiveCore.Tests.ReportHelper
{
    [TestClass]
    public class EPMDataTest
    {
        private readonly EPMData EpmData = new EPMData(Guid.NewGuid());
        private const string CreateEventMessageMethod = "CreateEventMessage";
        private const string LogWindowsEventsMethod = "LogWindowsEvents";
        private Exception _exception;
        private PrivateObject _privateObject;

        [TestInitialize]
        public void Setup()
        {
            _exception = new Exception();
            _privateObject = new PrivateObject(EpmData);
        }

        [TestMethod]
        public void BulkInsertTest()
        {
            using (SPEmulators.SPEmulationContext ctx = new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {
               
               
                ShimEPMData.ConstructorGuid = (instance, _guid) =>
                {
                };
                ShimEPMData.AllInstances.LogStatusStringStringStringStringInt32Int32String = (String1, String2, String3, String4, Int321, Int322, String5, _bool) => { return true; };
                ShimSqlConnection.AllInstances.BeginTransaction = (instance) => { return new ShimSqlTransaction() { Commit = () => { }, DisposeBoolean = (_bool) => { }, Rollback = () => { } }; };
                ShimSqlBulkCopy.ConstructorSqlConnectionSqlBulkCopyOptionsSqlTransaction = (_a, _b, _c, _d) =>
                {

                };
                ShimSqlBulkCopy.AllInstances.Close = (instance) => { };
                ShimSqlBulkCopy.AllInstances.NotifyAfterGet = (instance) => { return 0; };
                ShimSqlBulkCopy.AllInstances.WriteToServerDataTable = (instance, _dt) => { };
                ShimSqlBulkCopy.AllInstances.ColumnMappingsGet = (instance) => { return new ShimSqlBulkCopyColumnMappingCollection() { AddStringString = (_string, str) => { return new ShimSqlBulkCopyColumnMapping(); } }; };
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("TestColumn");
                ds.Tables.Add(dt);
                EPMData epmdata = new EPMData(Guid.NewGuid());
                string message = string.Empty;

                using (TestCheck.OpenCloseConnections)
                {
                    //Act
                    var result = epmdata.BulkInsert(ds, true, out message);
                    //Assert
                    Assert.AreEqual(true, result);
                }

                using (TestCheck.OpenCloseConnections)
                {
                    //Act
                    var result = epmdata.BulkInsert(ds, Guid.NewGuid());

                    //Assert
                    Assert.AreEqual(true, result);
                }
            }

        }

        [TestMethod]
        public void BulkInsertTest_ExecuteCatch()
        {
            using (SPEmulators.SPEmulationContext ctx = new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {
                ShimEPMData.ConstructorGuid = (instance, _guid) =>
                {
                };
                ShimEPMData.AllInstances.LogStatusStringStringStringStringInt32Int32String = (String1, String2, String3, String4, Int321, Int322, String5, _bool) => { return true; };
                ShimSqlConnection.AllInstances.BeginTransaction = (instance) => { return new ShimSqlTransaction() { Commit = () => { }, DisposeBoolean = (_bool) => { }, Rollback = () => { } }; };
                ShimSqlBulkCopy.ConstructorSqlConnectionSqlBulkCopyOptionsSqlTransaction = (_a, _b, _c, _d) =>
                {

                };
                ShimSqlBulkCopy.AllInstances.Close = (instance) => { };
                
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("TestColumn");
                ds.Tables.Add(dt);
                EPMData epmdata = new EPMData(Guid.NewGuid());
                string message = string.Empty;

                using (TestCheck.OpenCloseConnections)
                {
                    //Act
                    var result = epmdata.BulkInsert(ds, true, out message);
                    //Assert
                    Assert.AreEqual(false, result);
                }

                using (TestCheck.OpenCloseConnections)
                {
                    //Act
                    var result = epmdata.BulkInsert(ds, Guid.NewGuid());
                    //Assert
                    Assert.AreEqual(false, result);
                }
            }
        }
        [TestMethod]
        public void BulkInsertTest_ExecuteCatch_2()
        {
            using (SPEmulators.SPEmulationContext ctx = new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {
                int Openconnection = 0;
                int Closeconnection = 0;
                
                ShimEPMData.ConstructorGuid = (instance, _guid) =>
                {
                };
                ShimSqlConnection.ConstructorString = (instance, _string) => { };
                
                ShimEPMData.AllInstances.LogStatusStringStringStringStringInt32Int32String = (String1, String2, String3, String4, Int321, Int322, String5, _bool) => { return true; };
                ShimSqlConnection.AllInstances.DisposeBoolean = (instance, _bool) => { Closeconnection++; };
              
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Columns.Add("TestColumn");
                ds.Tables.Add(dt);
                EPMData epmdata = new EPMData(Guid.NewGuid());
                string message = string.Empty;
                //Act
                var result = epmdata.BulkInsert(ds, true, out message);
                
                //Assert
                Assert.AreEqual(false, result);
                Assert.AreNotEqual(Openconnection, Closeconnection);

                //Act
                result = epmdata.BulkInsert(ds, Guid.NewGuid());

                //Assert
                Assert.AreEqual(false, result);
                Assert.AreNotEqual(Openconnection, Closeconnection);
            }
        }

        //[TestMethod]
        //public void CreateEventMessage_WhenExceptionIsNull_ThrowsException()
        //{
        //    using (SPEmulators.SPEmulationContext ctx = new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
        //    {
        //        // Arrange
        //        _exception = null;

        //        try
        //        {
        //            // Act
        //            _privateObject.Invoke(CreateEventMessageMethod, _exception);
        //            Assert.Fail("CreateEventMessage: not throw ArgumentNullException");
        //        }
        //        catch (Exception ex)
        //        {
        //            // Assert
        //            Assert.IsTrue(ex is ArgumentNullException);
        //        }
        //    }
        //}




    }
}