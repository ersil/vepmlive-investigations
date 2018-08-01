﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.SharePoint.Fakes;
using EPMLiveCore.API.Fakes;
using System.Xml;
using Microsoft.SharePoint;
using System.Reflection;
using System.Collections;
using EPMLiveCore.Fakes;
using System.Collections.Generic;
using Microsoft.QualityTools.Testing.Fakes;
using EPMLiveCore.ReportHelper.Fakes;
using System.Data.SqlClient.Fakes;
using System.Data.Common.Fakes;
using System.Data.SqlClient;
using System.ComponentModel.Fakes;
using System.Linq;

namespace EPMLiveCore.API.Tests
{
    [TestClass()]
    public class APITeamTests
    {
        private IDisposable _shimsContext;
        private ShimSPWeb _webShim;
        private PrivateType _apiTeamPrivateType;
        private PrivateObject _apiTeamPrivateObject;

        private IList<string> _sqlConnectionsDisposed;
        private IList<string> _sqlCommandsDisposed;
        private int _sqlDataReadersDisposed;

        [TestInitialize]
        public void SetUp()
        {
            _shimsContext = ShimsContext.Create();

            _webShim = new ShimSPWeb
            {
                SiteGet = () => new ShimSPSite(),
                ListsGet = () => new ShimSPListCollection
                {
                    ItemGetString = key => new ShimSPList()
                }
            };
            _apiTeamPrivateType = new PrivateType(typeof(APITeam));
            _apiTeamPrivateObject = new PrivateObject(typeof(APITeam));

            ShimEPMData.ConstructorGuid = (instance, siteId) => { };

            ShimSqlCommand.AllInstances.ExecuteReader = instance => new ShimSqlDataReader();
            ShimCoreFunctions.getConfigSettingSPWebString = (web, name) => string.Empty;

            ShimComponent.AllInstances.Dispose = instance =>
            {
                if (instance is SqlConnection)
                {
                    _sqlConnectionsDisposed.Add((instance as SqlConnection).ConnectionString);
                }
                if (instance is SqlCommand)
                {
                    _sqlCommandsDisposed.Add((instance as SqlCommand).CommandText);
                }
            };
            ShimDbDataReader.AllInstances.Dispose = instance =>
            {
                if (instance is SqlDataReader)
                {
                    _sqlDataReadersDisposed++;
                }
            };

            _sqlConnectionsDisposed = new List<string>();
            _sqlCommandsDisposed = new List<string>();
            _sqlDataReadersDisposed = 0;
        }

        [TestCleanup]
        public void TearDown()
        {
            _shimsContext.Dispose();
        }

        [TestMethod()]
        public void GetTeamGridLayoutTest()
        {
            Guid jobid = Guid.NewGuid();
            string XML = "<Grid WebId=\"0f681e6a-0113-4abc-a5b3-4d1f7e42ac41\" ListId=\"5bb9f6de-8444-4f17-b142-68b6f05a1221\" ItemId=\"4\" />";

            using (new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {
                ShimSPWeb spweb = new ShimSPWeb()
                {
                    SiteGet = () =>
                    {
                        return
                        new ShimSPSite()
                        {
                            IDGet = () => { return jobid; },
                        };
                    },
                    IDGet = () => { return jobid; },
                    Dispose = () => { },
                    ListsGet = () =>
                    {
                        ShimSPListCollection lists = new ShimSPListCollection();
                        lists.ItemGetGuid = (guid2) =>
                        {
                            ShimSPList list = new ShimSPList();
                            //list.TitleGet = () => { return listTitle; };

                            list.GetItemByIdInt32 = (_int) => { return new ShimSPListItem() { HasUniqueRoleAssignmentsGet = () => { return true; } }; };
                            return list;
                        };


                        return lists;
                    }
                };
                ShimAPITeam.GetGenericResourceGridStringSPWeb = (str, web) =>
                {
                    XmlDocument docOut = new XmlDocument();
                    docOut.LoadXml("<Grid><Cfg id=\"TeamGrid\" SuppressCfg=\"0\" /><!-- Configuration is not saved to cookies --><Cfg MainCol=\"Title\" NameCol=\"Title\" Style=\"GM\" CSS=\"TreeGrid / WorkEngine / Grid.css\" Undo=\"0\" ChildParts=\"0\" /><Cfg ExportType=\"Expanded,Outline\" /><Cfg PrintCols=\"1\" /><Cfg ExportCols=\"1\" /><Cfg NumberId=\"1\" FullId=\"0\" IdChars=\"1234567890\" AddFocusCol=\"Title\" /><Cfg StaticCursor=\"1\" Dragging=\"0\" SelectingCells=\"1\" ShowDeleted=\"0\" SelectClass=\"0\" Hover=\"0\" /><Cfg Paging=\"2\" AllPages=\"1\" PageLength=\"25\" MaxPages=\"20\" NoPager=\"1\" ChildParts=\"2\" /><Toolbar Visible=\"0\"></Toolbar><Panel Visible=\"1\" Delete=\"0\" Move=\"0\" NoFormatEscape=\"1\" CanHide=\"0\" SelectWidth=\"30\" /><Cfg NoTreeLines=\"1\" DetailOn=\"0\" MinRowHeight=\"20\" MidWidth=\"300\" MenuColumnsSort=\"1\" StandardFilter=\"2\" /><Header SortIcons=\"2\" CanLogin=\"User\" Generic=\"Generic\" FirstName=\"First Name\" LastName=\"Last Name\" SharePointAccount=\"SharePoint Account\" Email=\"Email\" Approved=\"Approved\" StandardRate=\"Standard Rate\" Disabled=\"Disabled\" AvailableFrom=\"Available From\" AvailableTo=\"Available To\" HolidaySchedule=\"Holiday Schedule\" WorkHours=\"Work Hours\" Department=\"Department\" Role=\"Role\" ResourceLevel=\"License Type\" TimesheetAdministrator=\"Timesheet Administrator\" TimesheetDelegates=\"Timesheet Delegates\" TimesheetManager=\"Timesheet Manager\" /><Cfg Code=\"GTACCNPSQEBSLC\" /><LeftCols><C Name=\"Photo\" Visible=\"1\" Width=\"65\" Type=\"Html\" CanHide=\"1\" /><C Name=\"Title\" Visible=\"1\" RelWidth=\"1\" CanHide=\"0\" Type=\"Html\" CaseSensitive=\"0\" /></LeftCols><Cols><C Name=\"CanLogin\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Generic\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"FirstName\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"LastName\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"SharePointAccount\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Email\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Approved\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"StandardRate\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Disabled\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"AvailableFrom\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"AvailableTo\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"HolidaySchedule\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"WorkHours\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Department\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"Role\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"ResourceLevel\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"TimesheetAdministrator\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"TimesheetDelegates\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"TimesheetManager\" Type=\"Html\" Visible=\"0\" Width=\"100\" /><C Name=\"SPAccountInfo\" Type=\"Html\" Visible=\"0\" /><C Name=\"Groups\" Type=\"Html\" Visible=\"0\" /></Cols><RightCols></RightCols><Def><D Name=\"R\" Calculated=\"0\" /><D Name=\"Group\" CanSelect=\"0\" /></Def><Solid></Solid><Header Visible=\"1\" /><Head><I Kind=\"Filter\" Visible=\"0\" id=\"Filter\" /><I Kind=\"Group\" Visible=\"0\" id=\"GroupRow\" /></Head></Grid>");
                    return docOut;
                };
                ShimSPSite.ConstructorGuid = (spsite, gui) =>
                {

                };
                ShimSPSite.AllInstances.OpenWebGuid = (instance, id) =>
                {
                    return spweb;
                };


                ShimSPSite.AllInstances.Dispose = (instance) => { };
                string result = APITeam.GetTeamGridLayout(XML, spweb);

                Assert.IsNotNull(result);
                Assert.IsTrue(result.Contains("TeamGrid"));
                Assert.IsTrue(result.Contains("<!-- Configuration is not saved to cookies -->"));
            }

        }

        [TestMethod()]
        [ExpectedException(typeof(System.Reflection.TargetInvocationException))]
        public void setPermissionsTest()
        {
            Guid jobid = Guid.NewGuid();
            string XML = "<Grid WebId=\"0f681e6a-0113-4abc-a5b3-4d1f7e42ac41\" ListId=\"5bb9f6de-8444-4f17-b142-68b6f05a1221\" ItemId=\"4\" />";
            var method = typeof(APITeam).GetMethod("setPermissions", BindingFlags.Static | BindingFlags.NonPublic);
            using (new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {


                SPWeb spweb = new ShimSPWeb()
                {
                    

                    GroupsGet = () =>
                    {
                        var groups = new ShimSPGroupCollection();
                        ShimSPBaseCollection coll = new ShimSPBaseCollection(groups);
                        coll.GetEnumerator = () =>
                        {
                            return new TestGroupEnumerator();
                        };
                        return groups;
                    },
                   
                    
                };
               
               

               
                method.Invoke("setPermissions", new object[] { spweb, "", "" });
            }
        }
        [TestMethod()]
        
        public void setItemPermissions()
        {
            Guid jobid = Guid.NewGuid();
            int fieldcount=0;
            string XML = "<Grid WebId=\"0f681e6a-0113-4abc-a5b3-4d1f7e42ac41\" ListId=\"5bb9f6de-8444-4f17-b142-68b6f05a1221\" ItemId=\"4\" />";
            var method = typeof(APITeam).GetMethod("setItemPermissions", BindingFlags.Static | BindingFlags.NonPublic);
            using (new SPEmulators.SPEmulationContext(SPEmulators.IsolationLevel.Fake))
            {

                SPListItem item = new ShimSPListItem()
                {
                    ParentListGet = () =>
                    {
                        return new ShimSPList()
                        {
                            FieldsGet = () =>
                            {
                                return new ShimSPFieldCollection()
                                {
                                    GetFieldByInternalNameString = (s) =>
                                    {
                                        return new ShimSPFieldLookup()
                                        {
                                            LookupListGet = () => { return Guid.NewGuid().ToString(); }
                                        };
                                    },
                                    
                                };
                            },
                        };
                    },
                    HasUniqueRoleAssignmentsGet = () => { return true; },
                    ItemGetString = (guid2) =>
                    {
                        fieldcount++;
                        return "0";
                    }

                };
                SPWeb spweb = new ShimSPWeb()
                {
                    SiteGet = () =>
                    {
                        return
                        new ShimSPSite()
                        {
                            IDGet = () => { return jobid; },
                        };
                    },

                 
                    IDGet = () => { return jobid; },
                    Dispose = () => { },
                    ListsGet = () =>
                    {
                        ShimSPListCollection lists = new ShimSPListCollection();
                        lists.ItemGetGuid = (guid2) =>
                        {
                            ShimSPList list = new ShimSPList()
                            {
                                GetItemByIdInt32 = (_int) =>
                                {

                                    return new ShimSPListItem()
                                    {
                                        HasUniqueRoleAssignmentsGet = () => { return false; },

                                    };
                                }
                            };
                            //list.TitleGet = () => { return listTitle; };


                            return list;
                        };
                        return lists;
                    },
                    AllowUnsafeUpdatesSetBoolean = (bl) => { },
                    
                    EnsureUserString = (_str) => {
                        return null;
                    }
                    
                };

               
                ShimSPSite.ConstructorGuid = (spsite, gui) =>
                {

                };
                ShimSPSite.AllInstances.OpenWebGuid = (instance, id) =>
                {
                    return spweb;
                };
                ShimListCommands.GetGridGanttSettingsSPList = (lst) =>
                {
                    GridGanttSettings stng = new ShimGridGanttSettings();
                    stng.BuildTeamPermissions = "6|~|1758263";
                    return stng;
                };
                List<string> lststr = new List<string>();
                lststr.Add("Field1");
                lststr.Add("Field2");
                lststr.Add("Field3");
                lststr.Add("Field4");
                lststr.Add("Field5");
                ShimSPSite.AllInstances.Dispose = (instance) => { };
                ShimEnhancedLookupConfigValuesHelper.ConstructorString = (instance, _str) => { };
                ShimEnhancedLookupConfigValuesHelper.AllInstances.GetSecuredFields = (instance) =>
                {
                   
                    return lststr;
                };
                ShimSPFieldUserValue.AllInstances.UserGet = (instance) =>
                {
                    return new ShimSPUser()
                    {
                        LoginNameGet = () => { return "test/test"; }
                    };
                };
                ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = (w) =>
                {
                    w();
                };
               
                ShimGridGanttSettings.ConstructorSPList = (instance, slist) => { };
                method.Invoke("setItemPermissions", new object[] { spweb, "", "", item });
                Assert.AreEqual(lststr.Count, fieldcount);
            }
        }

        [TestMethod]
        public void getResources_SiteExists_AdoObjectsDisposed()
        {
            // Arrange
            var filterField = "test-field";
            var filterValue = "test-filter-value";
            var hasPerms = false;
            var arrColumns = new ArrayList();
            var shimListItem = new ShimSPListItem();
            XmlNodeList nodeTeam = null;

            ShimReportBiz.AllInstances.SiteExists = (instance) => true;
            ShimAPITeam.iGetResourceFromRPTSqlConnectionSPListDataTableSPWebStringStringBooleanBooleanArrayListSPListItemXmlNodeList =
                (a, b, c, d, e, f, g, h, i, j, k) => null;
            ShimAPITeam.iGetResourcesFromlistSPListDataTableSPWebStringStringBooleanArrayListSPListItem =
                (a, b, c, d, e, f, g, h) => null;
            
            // Act
            _apiTeamPrivateType.InvokeStatic("getResources",
                _webShim.Instance,
                filterField,
                filterValue,
                hasPerms,
                arrColumns,
                shimListItem.Instance,
                nodeTeam);

            // Assert
            Assert.IsTrue(_sqlConnectionsDisposed.Any());
            Assert.AreEqual(1, _sqlCommandsDisposed.Count);
            Assert.AreEqual(1, _sqlDataReadersDisposed);
        }
    }
    public class TestGroupEnumerator : IEnumerator
    {
        public SPGroup[] _spGroup = new SPGroup[1];
        int position = -1;
        public TestGroupEnumerator()
        {
            _spGroup[0] = new ShimSPGroup()
            {

            };

        }

        public object Current
        {
            get
            {
                return _spGroup[position];
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < _spGroup.Length);
        }

        public void Reset()
        { }
    }
}