﻿using System;
using System.Data.SqlClient.Fakes;
using System.Data.Common.Fakes;
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Fakes;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using EPMLiveCore.API.Fakes;
using EPMLiveCore.Fakes;
using EPMLiveCore.ReportHelper.Fakes;
using EPMLive.TestFakes;
using EPMLive.TestFakes.Utility;

namespace EPMLiveCore.API.Tests
{
    [TestClass()]
    public class APITeamTests
    {
        private IDisposable _shimsContext;
        private PrivateType _apiTeamPrivateType;
        private PrivateObject _apiTeamPrivateObject;

        private AdoShims _adoShims;
        private SharepointShims _sharepointShims;

        private DataTable _dataTable;
        private string _currentTeam;
        private string _filterField;
        private string _filterValue;
        private bool _filterIsLookup;
        private bool _hasPerms;
        private ArrayList _columns;
        private XmlNodeList _nodeTeam;
        private Guid _listId;
        private int _itemId;
        private DataTable _resources;
        
        private string _resourcePoolXml;
        private bool _resourcePoolEnsureFilterValueSafe;
        private string _setItemPermissionsUser;
        private string _setItemPermissionsPermissions;
        private XmlDocument _teamDocument;
        private string _getTeamQueryDocumentXml;
        private string _saveTeamQueryDocumentXml;

        private bool _isGetTeamFromCurrentCalled;
        private bool _isGetTeamFromListItemCalled;
        private bool _isGetTeamFromWebCalled;
        private string _filterFieldUsed;
        private string _filterValueUsed;
        private ArrayList _columnsUsed;
        private string _currentTeamUsed;
        private Guid? _listIdUsed;
        private int? _itemIdUsed;
        private bool _saveTeamResourceRateFeatureEnabled;

        [TestInitialize]
        public void SetUp()
        {
            _shimsContext = ShimsContext.Create();
            _adoShims = AdoShims.ShimAdoNetCalls();
            _sharepointShims = SharepointShims.ShimSharepointCalls();

            _apiTeamPrivateType = new PrivateType(typeof(APITeam));
            _apiTeamPrivateObject = new PrivateObject(typeof(APITeam));

            SetUpDefaultValues();

            ShimCoreFunctions.getConfigSettingSPWebStringBooleanBoolean = (web, key, translateUrl, relativeUrl) => string.Empty;

            ShimListCommands.GetGridGanttSettingsSPList = list => new GridGanttSettings(_sharepointShims.ListShim)
            {
                Lookups = "test-11^test-12^test-13^test-14^true|test-21^test-22^test-23^test-24"
            };

            _sharepointShims.ListItemShim.ItemGetString = key => "test";
        }

        private void SetUpDefaultValues()
        {
            _currentTeam = "test-team";
            _filterField = "test-field";
            _filterValue = "test-filter-value";
            _filterIsLookup = false;
            _hasPerms = false;
            _columns = new ArrayList
            {
                "test-column-1",
                "test-column-2"
            };
            _nodeTeam = null;
            _listId = Guid.NewGuid();
            _sharepointShims.ListShim.IDGet = () => _listId;
            _itemId = 10;
            _sharepointShims.ListItemShim.IDGet = () => _itemId;

            _isGetTeamFromCurrentCalled = false;
            _isGetTeamFromListItemCalled = false;
            _isGetTeamFromWebCalled = false;
            _filterFieldUsed = null;
            _filterValueUsed = null;
            _columnsUsed = null;
            _currentTeamUsed = null;
            _listIdUsed = null;
            _itemIdUsed = null;

            _dataTable = new DataTable();
            _resourcePoolXml = @"<XML FilterField='Field1' FilterFieldValue='Field1Value'><Columns>Column1,Column2</Columns></XML>";

            _teamDocument = new XmlDocument();
            _teamDocument.LoadXml("<root />");
            _resources = new DataTable();
            _resources.Columns.Add("SPID");
            _resources.Columns.Add("Groups");
            _resources.Columns.Add("Title");
            _resources.Rows.Add(0, "group-0", "test-title-0");

            _setItemPermissionsUser = "test-user";
            _setItemPermissionsPermissions = string.Join(";", _columns.ToArray());
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
            ShimReportBiz.AllInstances.SiteExists = (instance) => true;
            ShimAPITeam.iGetResourceFromRPTSqlConnectionSPListDataTableSPWebStringStringBooleanBooleanArrayListSPListItemXmlNodeList =
                (a, b, c, d, e, f, g, h, i, j, k) => null;
            ShimAPITeam.iGetResourcesFromlistSPListDataTableSPWebStringStringBooleanArrayListSPListItem =
                (a, b, c, d, e, f, g, h) => null;
            
            // Act
            _apiTeamPrivateType.InvokeStatic("getResources",
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _hasPerms,
                _columns,
                _sharepointShims.ListItemShim.Instance,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.ConnectionsDisposed.Any());
            Assert.AreEqual(1, _adoShims.CommandsDisposed.Count);
            Assert.AreEqual(1, _adoShims.DataReadersDisposed.Count);
        }

        [TestMethod]
        public void iGetResourceFromRPT_Always_SelectSchemaForspGetReportListDataCommandCreatedAndDisposed()
        {
            // Arrange
            SPListItem listItem = null;
            const string sql = "select * from information_schema.parameters where specific_name='spGetReportListData' and parameter_name='@orderby'";

            // Act
            _apiTeamPrivateType.InvokeStatic("iGetResourceFromRPT",
                _adoShims.ConnectionShim.Instance,
                _sharepointShims.ListShim.Instance,
                _dataTable,
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _filterIsLookup,
                _hasPerms,
                _columns,
                listItem,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.CommandsCreated.Any(pred => pred.CommandText == sql));
            Assert.IsTrue(_adoShims.CommandsDisposed.Any(pred => pred.CommandText == sql));
        }

        [TestMethod]
        public void iGetResourceFromRPT_Always_spGetReportListDataAdapterCreatedAndDisposed()
        {
            // Arrange
            SPListItem listItem = null;
            const string sql = "spGetReportListData";

            // Act
            _apiTeamPrivateType.InvokeStatic("iGetResourceFromRPT",
                _adoShims.ConnectionShim.Instance,
                _sharepointShims.ListShim.Instance,
                _dataTable,
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _filterIsLookup,
                _hasPerms,
                _columns,
                listItem,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.DataAdaptersCreated.Any(pred => pred.Key.CommandText == sql));
            Assert.IsTrue(_adoShims.DataAdaptersDisposed.Any(pred => pred.Key.CommandText == sql));
        }

        [TestMethod]
        public void iGetResourceFromRPT_Always_spGetReportListCommandCreatedAndDisposed()
        {
            // Arrange
            SPListItem listItem = null;
            const string sql = "spGetReportListData";

            // Act
            _apiTeamPrivateType.InvokeStatic("iGetResourceFromRPT",
                _adoShims.ConnectionShim.Instance,
                _sharepointShims.ListShim.Instance,
                _dataTable,
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _filterIsLookup,
                _hasPerms,
                _columns,
                listItem,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.CommandsCreated.Any(pred => pred.CommandText == sql));
            Assert.IsTrue(_adoShims.CommandsDisposed.Any(pred => pred.CommandText == sql));
        }

        [TestMethod]
        public void iGetResourceFromRPT_Always_LSTUserInformationListDataAdapterCreatedAndDisposed()
        {
            // Arrange
            SPListItem listItem = null;
            const string sql = "SELECT ID,Picture FROM LSTUserInformationList where siteid = @siteid and webid = @webid";

            // Act
            _apiTeamPrivateType.InvokeStatic("iGetResourceFromRPT",
                _adoShims.ConnectionShim.Instance,
                _sharepointShims.ListShim.Instance,
                _dataTable,
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _filterIsLookup,
                _hasPerms,
                _columns,
                listItem,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.DataAdaptersCreated.Any(pred => pred.Key.CommandText == sql));
            Assert.IsTrue(_adoShims.DataAdaptersDisposed.Any(pred => pred.Key.CommandText == sql));
        }

        [TestMethod]
        public void iGetResourceFromRPT_Always_LSTUserInformationListCommandCreatedAndDisposed()
        {
            // Arrange
            SPListItem listItem = null;
            const string sql = "SELECT ID,Picture FROM LSTUserInformationList where siteid = @siteid and webid = @webid";

            // Act
            _apiTeamPrivateType.InvokeStatic("iGetResourceFromRPT",
                _adoShims.ConnectionShim.Instance,
                _sharepointShims.ListShim.Instance,
                _dataTable,
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _filterIsLookup,
                _hasPerms,
                _columns,
                listItem,
                _nodeTeam);

            // Assert
            Assert.IsTrue(_adoShims.CommandsCreated.Any(pred => pred.CommandText == sql));
            Assert.IsTrue(_adoShims.CommandsDisposed.Any(pred => pred.CommandText == sql));
        }

        [TestMethod]
        public void VerifyProjectTeamWorkspace_EPMDataAndConnectionInitialized_CommandCreatedAndDisposed()
        {
            // Arrange
            var sql = $"SELECT ItemId, ItemListId, WebId FROM RPTWeb WHERE WebId = '{_sharepointShims.WebShim.Instance.ID}'";
            int itemId;
            Guid listId;

            // Act
            APITeam.VerifyProjectTeamWorkspace(_sharepointShims.WebShim, out itemId, out listId);

            // Assert
            Assert.IsTrue(_adoShims.CommandsCreated.Any(pred => pred.CommandText == sql));
            Assert.IsTrue(_adoShims.CommandsDisposed.Any(pred => pred.CommandText == sql));
        }

        [TestMethod]
        public void VerifyProjectTeamWorkspace_EPMDataAndConnectionInitialized_DataAdapterCreatedAndDisposed()
        {
            // Arrange
            var sql = $"SELECT ItemId, ItemListId, WebId FROM RPTWeb WHERE WebId = '{_sharepointShims.WebShim.Instance.ID}'";
            int itemId;
            Guid listId;

            // Act
            APITeam.VerifyProjectTeamWorkspace(_sharepointShims.WebShim, out itemId, out listId);

            // Assert
            Assert.IsTrue(_adoShims.DataAdaptersCreated.Any(pred => pred.Key.CommandText == sql));
            Assert.IsTrue(_adoShims.DataAdaptersDisposed.Any(pred => pred.Key.CommandText == sql));
        }

        [TestMethod]
        public void GetResourcePool_EmptyXml_UsesDefaultValuesForFiltersAndColumns()
        {
            // Arrange
            _resourcePoolXml = null;

            string filterFieldUsed = null;
            string filterValueUsed = null;
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList =
                (a, filterField, filterValue, d, e, f, g) =>
                {
                    filterFieldUsed = filterField;
                    filterValueUsed = filterValue;
                    return new DataTable();
                };

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetResourcePool",
                _resourcePoolXml,
                _sharepointShims.WebShim.Instance,
                _nodeTeam,
                _resourcePoolEnsureFilterValueSafe);

            // Assert
            Assert.AreEqual(string.Empty, filterFieldUsed);
            Assert.AreEqual(string.Empty, filterValueUsed);
        }

        [TestMethod]
        public void GetResourcePool_ValidXml_UsesXmlValuesForFiltersAndColumns()
        {
            // Arrange
            string filterFieldUsed = null;
            string filterValueUsed = null;
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList =
                (a, filterField, filterValue, d, e, f, g) =>
                {
                    filterFieldUsed = filterField;
                    filterValueUsed = filterValue;
                    return new DataTable();
                };

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetResourcePool",
                _resourcePoolXml,
                _sharepointShims.WebShim.Instance,
                _nodeTeam,
                _resourcePoolEnsureFilterValueSafe);

            // Assert
            Assert.AreEqual("Field1", filterFieldUsed);
            Assert.AreEqual("Field1Value", filterValueUsed);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception), "access is denied")]
        public void GetResourcePool_GetResourceDataThrowsAccessDenied_RetriesWithElevatedPrivelege()
        {
            // Arrange
            var retries = 0;
            var isPrivelegesElevated = false;
            ShimAPITeam.GetResourceDataXmlNodeListArrayListStringStringString = (a, b, c, d, e) =>
            {
                if (retries++ == 0)
                {
                    ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = action =>
                    {
                        isPrivelegesElevated = true;
                        action();
                    };

                    throw new Exception("access is denied");
                }

                return new DataTable();
            };

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetResourcePool",
                _resourcePoolXml,
                _sharepointShims.WebShim.Instance,
                _nodeTeam,
                _resourcePoolEnsureFilterValueSafe);

            // Assert
            Assert.AreEqual(2, retries);
            Assert.IsTrue(isPrivelegesElevated);
        }

        [TestMethod]
        public void setItemPermissions_UserNotNullGroupInList_AddUserToGroup()
        {
            // Arrange
            var isUserAddedToGroup = false;
            _setItemPermissionsPermissions = "0";
            _sharepointShims.UsersShim.GetByIDInt32 = id => null;
            _sharepointShims.GroupShim.AddUserSPUser = user =>
            {
                isUserAddedToGroup = true;
            };

            // Act
            _apiTeamPrivateType.InvokeStatic("setItemPermissions",
                _sharepointShims.WebShim.Instance,
                _setItemPermissionsUser,
                _setItemPermissionsPermissions,
                _sharepointShims.ListItemShim.Instance);

            // Assert
            Assert.IsTrue(isUserAddedToGroup);
        }

        [TestMethod]
        public void setItemPermissions_UserNotNullGroupInListHasSecurityFieldValueAndRoleAssignments_DoesNotAddUserToGroup()
        {
            // Arrange
            var isUserAddedToGroup = false;
            var field = new ShimSPFieldLookup
            {
                LookupListGet = () => Guid.NewGuid().ToString()
            };

            _setItemPermissionsPermissions = "0";
            _sharepointShims.UsersShim.GetByIDInt32 = id => null;
            _sharepointShims.GroupShim.AddUserSPUser = user => isUserAddedToGroup = true;
            _sharepointShims.FieldsShim.GetFieldByInternalNameString = fieldName => field;
            _sharepointShims.ListItemShim.ItemGetString = (key) => "security-value";
            _sharepointShims.ListItemShim.HasUniqueRoleAssignmentsGet = () => true;

            // Act
            _apiTeamPrivateType.InvokeStatic("setItemPermissions",
                _sharepointShims.WebShim.Instance,
                _setItemPermissionsUser,
                _setItemPermissionsPermissions,
                _sharepointShims.ListItemShim.Instance);

            // Assert
            Assert.IsFalse(isUserAddedToGroup);
        }

        [TestMethod]
        public void setItemPermissions_UserNotNullGroupInListHasSecurityFieldValueAndNoRoleAssignments_AddUserToGroup()
        {
            // Arrange
            var isUserAddedToGroup = false;
            var field = new ShimSPFieldLookup
            {
                LookupListGet = () => Guid.NewGuid().ToString()
            };

            _setItemPermissionsPermissions = "0";
            _sharepointShims.UsersShim.GetByIDInt32 = id => null;
            _sharepointShims.GroupShim.AddUserSPUser = user => isUserAddedToGroup = true;
            _sharepointShims.FieldsShim.GetFieldByInternalNameString = fieldName => field;
            _sharepointShims.ListItemShim.ItemGetString = (key) => "security-value";
            _sharepointShims.ListItemShim.HasUniqueRoleAssignmentsGet = () => false;

            // Act
            _apiTeamPrivateType.InvokeStatic("setItemPermissions",
                _sharepointShims.WebShim.Instance,
                _setItemPermissionsUser,
                _setItemPermissionsPermissions,
                _sharepointShims.ListItemShim.Instance);

            // Assert
            Assert.IsTrue(isUserAddedToGroup);
        }

        [TestMethod]
        public void setItemPermissions_UserNotNullGroupInListHasNoSecurityFieldValue_AddUserToGroup()
        {
            // Arrange
            var isUserAddedToGroup = false;
            var field = new ShimSPFieldLookup
            {
                LookupListGet = () => Guid.NewGuid().ToString()
            };

            _setItemPermissionsPermissions = "0";
            _sharepointShims.UsersShim.GetByIDInt32 = id => null;
            _sharepointShims.GroupShim.AddUserSPUser = user => isUserAddedToGroup = true;
            _sharepointShims.FieldsShim.GetFieldByInternalNameString = fieldName => field;
            _sharepointShims.ListItemShim.ItemGetString = (key) => string.Empty;
            _sharepointShims.ListItemShim.HasUniqueRoleAssignmentsGet = () => true;

            // Act
            _apiTeamPrivateType.InvokeStatic("setItemPermissions",
                _sharepointShims.WebShim.Instance,
                _setItemPermissionsUser,
                _setItemPermissionsPermissions,
                _sharepointShims.ListItemShim.Instance);

            // Assert
            Assert.IsTrue(isUserAddedToGroup);
        }

        [TestMethod]
        public void setItemPermissions_UserNotNullHasUnqueRoleAssignmentsGroupNotList_RemoveUserFromGroup()
        {
            // Arrange
            var isUserRemovedFromGroup = false;

            _setItemPermissionsPermissions = "1";
            _sharepointShims.GroupShim.RemoveUserSPUser = user =>
            {
                isUserRemovedFromGroup = true;
            };
            _sharepointShims.ListItemShim.HasUniqueRoleAssignmentsGet = () => true;

            // Act
            _apiTeamPrivateType.InvokeStatic("setItemPermissions",
                _sharepointShims.WebShim.Instance,
                _setItemPermissionsUser,
                _setItemPermissionsPermissions,
                _sharepointShims.ListItemShim.Instance);

            // Assert
            Assert.IsTrue(isUserRemovedFromGroup);
        }

        [TestMethod]
        [ExpectedException(typeof(APIException))]
        public void GetTeamFromListItem_UnexpectedException_ApiExceptionThrown()
        {
            // Arrange
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList = (a, b, c, d, e, f, g) => 
            {
                throw new Exception("Unexpected exception");
            };
            
            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetTeamFromListItem",
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _columns,
                _listId,
                _itemId) as XmlDocument;

            // Assert
            // ExpectedException - APIException
        }

        [TestMethod]
        public void GetTeamFromListItem_NoFieldUserValues_EmptyDocumentReturned()
        {
            // Arrange
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList = (a, b, c, d, e, f, g) => new DataTable();
            _sharepointShims.FieldUserValuesShim.Instance.Clear();

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetTeamFromListItem",
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _columns,
                _listId,
                _itemId) as XmlDocument;

            // Assert
            Assert.AreEqual(0, result.FirstChild.ChildNodes.Count);
        }

        [TestMethod]
        public void GetTeamFromListItem_HasUserValues_AddedToDocument()
        {
            // Arrange
            var isAppendUserValuesToTeamDocumentCalled = false;
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList = (a, b, c, d, e, f, g) => new DataTable();
            ShimAPITeam.AppendUserValuesToTeamDocumentSPFieldUserValueCollectionXmlDocumentDataTableIListOfString = (a, b, c, d) =>
            {
                isAppendUserValuesToTeamDocumentCalled = true;
            };

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("GetTeamFromListItem",
                _sharepointShims.WebShim.Instance,
                _filterField,
                _filterValue,
                _columns,
                _listId,
                _itemId) as XmlDocument;

            // Assert
            Assert.IsTrue(isAppendUserValuesToTeamDocumentCalled);
        }

        [TestMethod]
        public void AppendUserValuesToTeamDocument_NoSPIDResource_UserValueNotAppended()
        {
            // Arrange
            var additionalPermissions = new string[] { };
            _resources.Clear();

            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList = (a, b, c, d, e, f, g) => new DataTable();

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("AppendUserValuesToTeamDocument",
                _sharepointShims.FieldUserValuesShim.Instance,
                _teamDocument,
                _resources,
                additionalPermissions);

            // Assert
            Assert.AreEqual(0, _teamDocument.FirstChild.ChildNodes.Count);
        }

        [TestMethod]
        public void AppendUserValuesToTeamDocument_HasSPIDResource_UserValueAppended()
        {
            // Arrange
            var additionalPermissions = new string[] { };
            ShimAPITeam.getResourcesSPWebStringStringBooleanArrayListSPListItemXmlNodeList = (a, b, c, d, e, f, g) => new DataTable();

            // Act
            var result = _apiTeamPrivateType.InvokeStatic("AppendUserValuesToTeamDocument",
                _sharepointShims.FieldUserValuesShim.Instance,
                _teamDocument,
                _resources,
                additionalPermissions);

            // Assert
            Assert.AreEqual(1, _teamDocument.FirstChild.ChildNodes.Count);
            Assert.AreEqual(_resources.Rows[0]["SPID"].ToString(), _teamDocument.FirstChild.ChildNodes[0].Attributes["SPID"].Value);
            Assert.AreEqual(_resources.Rows[0]["Groups"].ToString(), _teamDocument.FirstChild.ChildNodes[0].Attributes["Groups"].Value);
            Assert.AreEqual(_resources.Rows[0]["Title"].ToString(), _teamDocument.FirstChild.ChildNodes[0].Attributes["Title"].Value);
            Assert.AreEqual(_resources.Rows[0]["Groups"].ToString(), _teamDocument.FirstChild.ChildNodes[0].Attributes["Permissions"].Value);
        }

        [TestMethod]
        public void GetTeam_HasQueryDoc_UsesQueryDocValuesAsParameters()
        {
            // Arrange
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.AreEqual(_filterField, _filterFieldUsed);
            Assert.AreEqual(_filterValue, _filterValueUsed);
            Assert.IsNotNull(_columnsUsed);
            Assert.AreEqual(_columns.Count, _columnsUsed.Count);
        }

        [TestMethod]
        public void GetTeam_WebIdEmptyCurrentTeamNotNull_GetsTeamFromCurrent()
        {
            // Arrange
            _currentTeam = "test-team";
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(_isGetTeamFromCurrentCalled);
            Assert.IsFalse(_isGetTeamFromListItemCalled);
            Assert.IsFalse(_isGetTeamFromWebCalled);
            Assert.AreEqual(_currentTeam, _currentTeamUsed);
        }

        [TestMethod]
        public void GetTeam_WebIdEmptyListIdNotEmptyAndUseTeam_GetsTeamFromListItem()
        {
            // Arrange
            _currentTeam = null;
            ShimGridGanttSettings.ConstructorSPList = (instance, list) =>
            {
                instance.BuildTeam = true;
            };
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsFalse(_isGetTeamFromCurrentCalled);
            Assert.IsTrue(_isGetTeamFromListItemCalled);
            Assert.IsFalse(_isGetTeamFromWebCalled);
            Assert.AreEqual(_listId, _listIdUsed);
            Assert.AreEqual(_itemId, _itemIdUsed);
        }

        [TestMethod]
        public void GetTeam_WebIdEmptyOther_GetsTeamFromWeb()
        {
            // Arrange
            _currentTeam = null;
            ShimGridGanttSettings.ConstructorSPList = (instance, list) =>
            {
                instance.BuildTeam = false;
            };
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsFalse(_isGetTeamFromCurrentCalled);
            Assert.IsFalse(_isGetTeamFromListItemCalled);
            Assert.IsTrue(_isGetTeamFromWebCalled);
        }

        [TestMethod]
        public void GetTeam_WebIdNotEmptyCurrentTeamEmptyListIdNotEmpty_GetsUseTeamSettingFromGanttGridSettings()
        {
            // Arrange
            _currentTeam = null;
            _sharepointShims.WebShim.IDGet = () => Guid.NewGuid();
            ShimListCommands.GetGridGanttSettingsSPList = list =>
            {
                var result = new ShimGridGanttSettings();
                result.Instance.BuildTeam = true;
                return result;
            };
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(_isGetTeamFromListItemCalled);
        }

        [TestMethod]
        public void GetTeam_WebIdNotEmptyCurrentTeamEmptyListEmpty_UseRootWebForReference()
        {
            // Arrange
            _currentTeam = null;
            _listId = Guid.Empty;
            _itemId = 0;
            _sharepointShims.WebShim.IDGet = () => Guid.NewGuid();

            var isParentListsUsed = false;
            var parentWeb = new ShimSPWeb(_sharepointShims.WebShim)
            {
                IsRootWebGet = () => true,
                ListsGet = () =>
                {
                    isParentListsUsed = true;
                    return _sharepointShims.ListsShim;
                }
            };
            _sharepointShims.WebShim.ParentWebGet = () => parentWeb;

            ShimAPITeam.VerifyProjectTeamWorkspaceSPWebInt32OutGuidOut = (SPWeb web, out int itemId, out Guid listId) =>
            {
                itemId = 10;
                listId = Guid.NewGuid();
            };
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isParentListsUsed);
        }

        [TestMethod]
        public void GetTeam_WebIdNotEmptyCurrentTeamEmptyListEmpty_UseParentWebList()
        {
            // Arrange
            _currentTeam = null;
            _listId = Guid.Empty;
            _itemId = 0;
            _sharepointShims.WebShim.IDGet = () => Guid.NewGuid();
            _sharepointShims.WebShim.IsRootWebGet = () => true;
            var isWebListUsed = false;

            ShimAPITeam.VerifyProjectTeamWorkspaceSPWebInt32OutGuidOut = (SPWeb web, out int itemId, out Guid listId) =>
            {
                itemId = 10;
                listId = Guid.NewGuid();
            };

            ShimListCommands.GetGridGanttSettingsSPList = list =>
            {
                isWebListUsed = ReferenceEquals(list, _sharepointShims.ListShim.Instance);
                var result = new ShimGridGanttSettings();
                return result;
            };
            SetUpForGetTeam();

            // Act
            APITeam.GetTeam(_getTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isWebListUsed);
        }

        private void SetUpForGetTeam()
        {
            ShimGetTeamFromCurrent();
            ShimGetTeamFromListItem();
            ShimGetTeamFromWeb();

            _getTeamQueryDocumentXml = $@"<Query {(_currentTeam != null ? $"CurrentTeam='{_currentTeam}'" : null)}
                                            WebId ='{_sharepointShims.WebShim.Instance.ID}' 
                                            ListId='{_sharepointShims.ListShim.Instance.ID}'
                                            ItemId='{_sharepointShims.ListItemShim.Instance.ID}'
                                            Column='{_filterField}'
                                            Value='{_filterValue}'>
                                        <Columns>{string.Join(",", _columns.ToArray())}</Columns>
                                        </Query>";
        }

        [TestMethod]
        public void SaveTeam_ListIdAndItemIdEmpty_UseWebRootForReference()
        {
            // Arrange
            _currentTeam = null;
            _listId = Guid.Empty;
            _itemId = 0;
            _sharepointShims.WebShim.IDGet = () => Guid.NewGuid();

            var isParentListsUsed = false;
            var parentWeb = new ShimSPWeb(_sharepointShims.WebShim)
            {
                IsRootWebGet = () => true,
                ListsGet = () =>
                {
                    isParentListsUsed = true;
                    return _sharepointShims.ListsShim;
                }
            };
            _sharepointShims.WebShim.ParentWebGet = () => parentWeb;

            ShimAPITeam.VerifyProjectTeamWorkspaceSPWebInt32OutGuidOut = (SPWeb web, out int itemId, out Guid listId) =>
            {
                itemId = 10;
                listId = Guid.NewGuid();
            };
            SetUpForSaveTeam();

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isParentListsUsed);
        }

        [TestMethod]
        public void SaveTeam_ListIdAndItemIdEmpty_UseParentForWeb()
        {
            // Arrange
            _currentTeam = null;
            _listId = Guid.Empty;
            _itemId = 0;
            _sharepointShims.WebShim.IDGet = () => Guid.NewGuid();
            _sharepointShims.WebShim.IsRootWebGet = () => true;
            var isWebListUsed = false;

            SetUpForSaveTeam();
            ShimAPITeam.VerifyProjectTeamWorkspaceSPWebInt32OutGuidOut = (SPWeb web, out int itemId, out Guid listId) =>
            {
                itemId = 10;
                listId = Guid.NewGuid();
            };

            ShimListCommands.GetGridGanttSettingsSPList = list =>
            {
                isWebListUsed = ReferenceEquals(list, _sharepointShims.ListShim.Instance);
                var result = new ShimGridGanttSettings();
                return result;
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isWebListUsed);
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndNotUseTeam_SetPermissionsForEachMember()
        {
            // Arrange
            var setPermissionsTimesCalled = 0;
            string accountInfoUsed = null;
            string permissionsUsed = null;

            SetUpForSaveTeam();
            ShimAPITeam.setPermissionsSPWebStringString = (web, accountInfo, permissions) =>
            {
                setPermissionsTimesCalled++;
                accountInfoUsed = accountInfo;
                permissionsUsed = permissions;
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.AreEqual(2, setPermissionsTimesCalled);
            Assert.AreEqual("test-account-info-2", accountInfoUsed);
            Assert.AreEqual("test-permissions-2", permissionsUsed);
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndNotUseTeam_RPTWebRecordUpdated()
        {
            // Arrange
            SetUpForSaveTeam();

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(_adoShims.CommandsCreated.Any(pred =>
                pred.CommandText == $@"Update [dbo].[RPTWeb] Set [Members] = 2 WHERE [SiteId] = '{_sharepointShims.SiteShim.Instance.ID}' AND [WebId] = '{_sharepointShims.WebShim.Instance.ID}'"
            ));
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndNotUseTeamResourceNotExistsInTeamResources_RemovesUserFromGroups()
        {
            // Arrange
            var removeUserByIdTimesCalled = 0;
            var itemIdsRemoved = new List<int>();

            SetUpForSaveTeam();
            _sharepointShims.ListItemsShim.GetDataTable = () =>
            {
                var result = new DataTable();
                result.Columns.Add("ID");
                result.Columns.Add("ResID");

                result.Rows.Add(1, 11);
                return result;
            };
            _sharepointShims.UsersShim.RemoveByIDInt32 = id =>
            {
                removeUserByIdTimesCalled++;
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.AreEqual(_sharepointShims.GroupsShim.Instance.Count, removeUserByIdTimesCalled);
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndNotUseTeamAndResourceNotExistsInTeamResources_ItemsDeleted()
        {
            // Arrange
            var itemIdsRemoved = new List<int>();

            SetUpForSaveTeam();
            _sharepointShims.ListItemsShim.GetDataTable = () =>
            {
                var result = new DataTable();
                result.Columns.Add("ID");
                result.Columns.Add("ResID");

                result.Rows.Add(1, 11);
                return result;
            };
            _sharepointShims.ListItemsShim.DeleteItemByIdInt32 = id =>
            {
                itemIdsRemoved.Add(id);
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(itemIdsRemoved.SequenceEqual(new[] { 1 }));
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndNotUseTeamAndListDataTableNotNullItemsWithResIdSpecifiedExist_ItemsNotDeleted()
        {
            // Arrange
            var itemIdsRemoved = new List<int>();

            SetUpForSaveTeam();
            _sharepointShims.ListItemsShim.GetDataTable = () =>
            {
                var result = new DataTable();
                result.Columns.Add("ID");
                result.Columns.Add("ResID");

                result.Rows.Add(1, 1);
                return result;
            };
            _sharepointShims.ListItemsShim.DeleteItemByIdInt32 = id =>
            {
                itemIdsRemoved.Add(id);
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.AreEqual(0, itemIdsRemoved.Count);
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndResourceRatesFeatureIsEnabled_UpdateProjectResourceRateUpdated()
        {
            // Arrange
            var isUpdateProjectResourceRateCalled = false;
            int projectIdUsed;
            int resourceIdUsed;
            decimal? rateUsed = null;

            ShimGridGanttSettings.ConstructorSPList = (instance, list) =>
            {
                instance.BuildTeam = true;
            };
            ShimAPITeam.UpdateProjectResourceRateSPWebInt32Int32NullableOfDecimal = (web, projectId, resourceId, rate) =>
            {
                isUpdateProjectResourceRateCalled = true;
                projectIdUsed = projectId;
                resourceIdUsed = resourceId;
                rateUsed = rate;
            };

            SetUpForSaveTeam();
            _saveTeamResourceRateFeatureEnabled = true;

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isUpdateProjectResourceRateCalled);
            Assert.AreEqual(1, rateUsed);
        }

        [TestMethod]
        public void SaveTeam_ListIdNotEmptyAndResourceRatesFeatureIsEnabled_DeletesProjectResourceRates()
        {
            // Arrange
            var isDeleteProjectResourceRatesCalled = false;
            int projectIdExpected = 100, projectIdUsed = 0;
            IList<int> userIdsExpected = new[] { 200, 200 }, userIdsUsed = null;

            ShimGridGanttSettings.ConstructorSPList = (instance, list) =>
            {
                instance.BuildTeam = true;
            };
            ShimAPITeam.UpdateProjectResourceRateSPWebInt32Int32NullableOfDecimal = (web, projectId, resourceId, rate) => { };

            SetUpForSaveTeam();
            _saveTeamResourceRateFeatureEnabled = true;
            ShimAPITeam.DeleteProjectResourceRatesSPWebInt32Int32Array = (web, projectId, userIds) =>
            {
                isDeleteProjectResourceRatesCalled = true;
                projectIdUsed = projectId;
                userIdsUsed = userIds;
            };

            // Act
            APITeam.SaveTeam(_saveTeamQueryDocumentXml, _sharepointShims.WebShim);

            // Assert
            Assert.IsTrue(isDeleteProjectResourceRatesCalled);
            Assert.AreEqual(projectIdExpected, projectIdUsed);
            Assert.IsTrue(userIdsExpected.SequenceEqual(userIdsUsed));
        }

        private void SetUpForSaveTeam()
        {
            _saveTeamQueryDocumentXml = $@"<Query ListId='{_sharepointShims.ListShim.Instance.ID}'
                                                  ItemId='{_sharepointShims.ListItemShim.Instance.ID}'>
                                               <Team>
                                                   <Member ID='1' Permissions='test-permissions-1' ProjectRate='1' />
                                                   <Member ID='2' Permissions='test-permissions-2' ProjectRate='1' />
                                               </Team>
                                           </Query>";
            _resources.Clear();
            _resources.Columns.Clear();
            _resources.Columns.Add("ID");
            _resources.Columns.Add("SPID");
            _resources.Columns.Add("SPAccountInfo");
            _resources.Columns.Add("Title");
            _resources.Columns.Add("StandardRate");

            _resources.Rows.Add("1", 1, "test-account-info-1", "test-title-1", 1);
            _resources.Rows.Add("11", 11, "test-account-info-11", "test-title-11", 11);
            _resources.Rows.Add("2", 11, "test-account-info-2", "test-title-2", 2);

            _saveTeamResourceRateFeatureEnabled = false;

            ShimAPITeam.GetResourcePoolStringSPWeb = (documentXml, web) => _resources;
            ShimAPITeam.IsProjectCenterSPWebGuid = (web, list) => _saveTeamResourceRateFeatureEnabled;
            ShimAPITeam.IsPfeSiteSPWeb = (web) => _saveTeamResourceRateFeatureEnabled;
            ShimAPITeam.setPermissionsSPWebStringString = (web, accountInfo, permissions) => { };
            ShimAPITeam.GetResourcePoolForSaveStringSPWebXmlNodeList = (a, b, c) => _resources;
            ShimAPITeam.setItemPermissionsSPWebStringStringSPListItem = (a, b, c, d) => { };
            ShimAPITeam.GetProjectIdSPWebGuidInt32 = (a, b, c) => 100;
            ShimAPITeam.GetResourceIdSPWebDataRow = (a, b) => 200;
            ShimAPITeam.DeleteProjectResourceRatesSPWebInt32Int32Array = (a, b, c) => { };
            ShimCoreFunctions.getReportingConnectionStringGuidGuid = (webId, siteId) => string.Empty;
        }

        private void ShimGetTeamFromCurrent()
        {
            ShimAPITeam.GetTeamFromCurrentSPWebStringStringArrayListString = (web, filterField, filterValue, columns, currentTeam) =>
            {
                _isGetTeamFromCurrentCalled = true;

                _filterFieldUsed = filterField;
                _filterValueUsed = filterValue;
                _columnsUsed = columns;
                _currentTeamUsed = currentTeam;

                return _teamDocument;
            };
        }

        private void ShimGetTeamFromListItem()
        {
            ShimAPITeam.GetTeamFromListItemSPWebStringStringArrayListGuidInt32 = (web, filterField, filterValue, columns, listId, itemId) =>
            {
                _isGetTeamFromListItemCalled = true;

                _filterFieldUsed = filterField;
                _filterValueUsed = filterValue;
                _columnsUsed = columns;
                _listIdUsed = listId;
                _itemIdUsed = itemId;

                return _teamDocument;
            };
        }

        private void ShimGetTeamFromWeb()
        {
            ShimAPITeam.GetTeamFromWebSPWebStringStringArrayList = (web, filterField, filterValue, columns) =>
            {
                _isGetTeamFromWebCalled = true;

                _filterFieldUsed = filterField;
                _filterValueUsed = filterValue;
                _columnsUsed = columns;

                return _teamDocument;
            };
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