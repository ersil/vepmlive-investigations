﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Data.SqlClient.Fakes;
using Microsoft.SharePoint.Fakes;
using EPMLiveCore.Infrastructure.Logging.Fakes;
using Microsoft.SharePoint.Administration.Fakes;
using EPMLiveCore.Fakes;
using System.Web.Fakes;
using System.Xml;
namespace EPMLiveCore.API.Tests
{
    [TestClass()]
    public class TimerTests
    {
        [TestMethod()]
        public void GetTimerJobStatusTest()
        {
            using (new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {
                Guid jobid = Guid.NewGuid();
                Guid siteid = jobid;
                Guid webid = jobid;
                Guid listid = jobid;
                int openconnection = 0;
                int closeconnetcion = 0;
                ShimSqlConnection con = new ShimSqlConnection();
                ShimSqlConnection.AllInstances.Open = (instance) =>
                {

                    openconnection++;
                };
                ShimSqlConnection.AllInstances.DisposeBoolean = (instance, _bool) =>
                {

                    closeconnetcion++;
                };

                ShimSPWeb spweb = new ShimSPWeb() { SiteGet = () => { return new ShimSPSite() { IDGet = () => { return jobid; } }; }, IDGet = () => { return jobid; } };
                ShimSPSite.ConstructorGuid = (instance, _guid) =>
                {


                };
                ShimSPSite.AllInstances.WebApplicationGet = (instance) =>
                {
                    var webApp = new ShimSPWebApplication();
                    var persistedObject = new ShimSPPersistedObject(webApp);
                    persistedObject.IdGet = () =>
                    {
                        return jobid;
                    };
                    return webApp;
                };
                ShimSPSite.AllInstances.Dispose = (instance) => { };
                ShimLoggingService.WriteTraceStringStringTraceSeverityString = (_str1, _str2, _trace, _str3) =>
                {

                };
                bool read = true;
                ShimCoreFunctions.getConnectionStringGuid = (instance) => { return ""; };
                ShimSqlCommand.AllInstances.ExecuteReader = (instance) =>
                {
                    return new ShimSqlDataReader()
                    {
                        Read = () =>
                        {
                            return read;
                        },
                        GetSqlInt32Int32 = (_int) =>
                        {
                            read = false;
                            return 20;
                        },
                        GetGuidInt32 = (_int) =>
                        {
                            read = false;
                            return jobid;
                        },
                        GetDateTimeInt32 = (_int) =>
                        {
                            read = false;
                            return DateTime.MinValue;
                        },
                        GetStringInt32 = (_int) =>
                        {
                            read = false;
                            return "";
                        },
                        IsDBNullInt32 = (_int) =>
                        {
                            read = false;
                            return false;
                        },
                    };
                };

                ShimHttpUtility.HtmlEncodeString = (a) => { return a; };
                //Act
                XmlNode ndStatus = Timer.GetTimerJobStatus(spweb, jobid);
                //Assert
                string expectedresult = "<TimerJobStatus ID=\"" + jobid + "\" Status=\"0\" PercentComplete=\"0\" Finished=\"1/1/0001 8:00:00 AM\" Result=\"\"><![CDATA[]]></TimerJobStatus>";
                Assert.AreEqual("TimerJobStatus", ndStatus.Name);
                Assert.AreEqual(expectedresult, ndStatus.OuterXml);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                ndStatus = Timer.GetTimerJobStatus(siteid, webid, 0, true);

                //Assert
                expectedresult = "<TimerJobStatus ID=\"" + jobid + "\" Status=\"0\" PercentComplete=\"0\" Finished=\"1/1/0001 8:00:00 AM\">&lt;![CDATA[]]&gt;</TimerJobStatus>";
                Assert.AreEqual("TimerJobStatus", ndStatus.Name);
                Assert.AreEqual(expectedresult, ndStatus.OuterXml);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                ndStatus = Timer.GetTimerJobStatus(siteid, webid, listid, 0, 0);

                //Assert
                Assert.AreEqual("TimerJobStatus", ndStatus.Name);
                Assert.AreEqual(expectedresult, ndStatus.OuterXml);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                Guid result = Timer.AddTimerJob(siteid, webid, listid, 1, "", 1, "", "", 1, 0, "");

                //Assert
                Assert.AreEqual(jobid, result);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                result = Timer.AddTimerJob(siteid, webid, listid, "", 1, "", "", 5, 2, "");


                //Assert
                Assert.AreEqual(jobid, result);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                result = Timer.AddTimerJob(siteid, webid, "", 1, "", "", 5, 3, "");


                //Assert
                Assert.AreEqual(jobid, result);
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                //Void
                Timer.CancelTimerJob(spweb, jobid);

                //Assert
                Assert.AreEqual(openconnection, closeconnetcion);

                //Act
                read = true;
                string _strresult = Timer.IsImportResourceAlreadyRunning(spweb);

                //Assert
                expectedresult = "<ResourceImporter Success=\"True\" JobUid=\"" + jobid + "\" PercentComplete=\"0\" />";
                Assert.AreEqual(expectedresult, _strresult);
                Assert.AreEqual(openconnection, closeconnetcion);
                //Act
                read = false;
                _strresult = Timer.IsSecurityJobAlreadyRunning(spweb, listid, 5);


                //Assert
                expectedresult = "<SecurityJob Success=\"False\" />";
                Assert.AreEqual(expectedresult, _strresult);
                Assert.AreEqual(openconnection, closeconnetcion);
            }
        }
    }
}