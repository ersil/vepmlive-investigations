﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Fakes;
using System.Data;
using System.Data.Common.Fakes;
using System.Data.SqlClient;
using System.Data.SqlClient.Fakes;
using System.Diagnostics.CodeAnalysis;
using System.IO.Fakes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Fakes;
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.API.Fakes;
using EPMLiveCore.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Fakes;
using Microsoft.SharePoint.Fakes;
using Microsoft.SharePoint.Utilities.Fakes;
using Microsoft.SharePoint.WebControls.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ResourceValues;
using RPADataCache;
using RPADataCache.Fakes;
using Shouldly;

namespace WorkEnginePPM.Tests.WebServices
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class RPADataTests
    {
        private RPAData testObject;
        private PrivateObject privateObject;
        private IDisposable shimsContext;
        private BindingFlags publicStatic;
        private BindingFlags publicInstance;
        private BindingFlags nonPublicInstance;
        private ShimSPWeb spWeb;
        private ShimSPSite spSite;
        private ShimSPListCollection spListCollection;
        private ShimSPList spList;
        private ShimSPListItemCollection spListItemCollection;
        private ShimSPListItem spListItem;
        private ShimSPFieldCollection spFieldCollection;
        private ShimSPField spField;
        private ShimSPUser spUser;
        private ShimSPFolderCollection spFolderCollection;
        private ShimSPFolder spFolder;
        private ShimSPFileCollection spFileCollection;
        private ShimSPFile spFile;
        private ShimSPViewCollection spViewCollection;
        private ShimSPView spView;
        private ShimSPViewFieldCollection spViewFieldCollection;
        private ShimSPFieldLinkCollection spFieldLinkCollection;
        private ShimSPContentTypeCollection spContentTypeCollection;
        private ShimSPContentType spContentType;
        private ShimSqlTransaction transaction;
        private Guid guid;
        private int validations;
        private const int DummyInt = 1;
        private const int One = 1;
        private const int Two = 2;
        private const int Three = 3;
        private const int Four = 4;
        private const int Five = 5;
        private const string SampleGuidString1 = "83e81819-0112-4c22-bb70-d8ba101e9e0c";
        private const string SampleGuidString2 = "83e81819-0104-4c22-bb70-d8ba101e9e0c";
        private const string DummyString = "DummyString";
        private const string IDStringCaps = "ID";
        private const string SampleUrl = "http://www.sampleurl.com";
        private const string ResourceValuesFieldName = "m_cResVals";
        private const string RolesAllowedFieldName = "m_CSRoleAllowed";
        private const string CatLookupFieldName = "m_maj_Cat_lookup";
        private const string UserDeptsFieldName = "UserDepts";
        private const string CostCategoryRoleMappingFieldName = "m_CCR_Role_Mapping";
        private const string TotalMasterColumnFieldName = "m_totmastercln";
        private const string DetMasterColumnFieldName = "m_detmastercln";
        private const string TotalDisplayColumnFieldName = "m_totdispcln";
        private const string DetDisplayColumnFieldName = "m_detdispcln";
        private const string StashCSRoleModeMethodName = "StashCSRoleMode";
        private const string IsCSRoleAllowedMethodName = "IsCSRoleAllowed";
        private const string SetMajorCatListlookupMethodName = "SetMajorCatListlookup";
        private const string ItemListAddItemMethodName = "ItemListAddItem";
        private const string GrabRADataMethodName = "GrabRAData";
        private const string DoUserDeptsMethodName = "DoUserDepts";
        private const string BuileCCR2RoleMapMethodName = "BuileCCR2RoleMap";
        private const string MapCCR2RoleMethodName = "MapCCR2Role";
        private const string setupdispcolnsMethodName = "setupdispcolns";

        [TestInitialize]
        public void Setup()
        {
            testObject = new RPAData();
            privateObject = new PrivateObject(testObject);

            SetupShims();
        }

        private void SetupShims()
        {
            shimsContext = ShimsContext.Create();
            SetupVariables();

            ShimSqlConnection.ConstructorString = (_, __) => new ShimSqlConnection();
            ShimSqlConnection.AllInstances.Open = _ => { };
            ShimSqlConnection.AllInstances.Close = _ => { };
            ShimSqlConnection.AllInstances.BeginTransaction = _ => transaction;
            ShimDbTransaction.AllInstances.Dispose = _ => { };
            ShimSqlConnection.AllInstances.CreateCommand = _ => new SqlCommand();
            ShimSqlCommand.AllInstances.ExecuteNonQuery = _ => DummyInt;
            ShimComponent.AllInstances.Dispose = _ => { };
            ShimSqlCommand.AllInstances.TransactionSetSqlTransaction = (_, __) => { };
            ShimSPDatabase.AllInstances.DatabaseConnectionStringGet = _ => DummyString;
            ShimGridGanttSettings.ConstructorSPList = (_, __) => new ShimGridGanttSettings();
            ShimHttpUtility.HtmlEncodeString = input => input;
            ShimSPSite.ConstructorString = (_, __) => new ShimSPSite();
            ShimSPSite.ConstructorGuid = (_, __) => new ShimSPSite();
            ShimSPSite.ConstructorGuidSPUserToken = (_, _1, _2) => new ShimSPSite();
            ShimSPSite.AllInstances.OpenWeb = _ => spWeb;
            ShimSPSite.AllInstances.OpenWebString = (_, __) => spWeb;
            ShimSPSite.AllInstances.OpenWebGuid = (_, __) => spWeb;
            ShimSPSite.AllInstances.Dispose = _ => { };
            ShimSPWeb.AllInstances.Dispose = _ => { };
            ShimCoreFunctions.getLockedWebSPWeb = _ => guid;
            ShimCoreFunctions.getConfigSettingSPWebString = (_, __) => DummyString;
            ShimCoreFunctions.getListSettingStringSPList = (_, __) => DummyString;
            ShimCoreFunctions.getConnectionStringGuid = _ => DummyString;
            ShimCoreFunctions.getLockConfigSettingSPWebStringBoolean = (_1, _2, _3) => DummyString;
            ShimSPList.AllInstances.GetItemsSPQuery = (_, __) => spListItemCollection;
            ShimSPPersistedObject.AllInstances.IdGet = _ => guid;
            ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = codeToRun => codeToRun();
            ShimUnsecuredLayoutsPageBase.AllInstances.SiteGet = _ => spSite;
            ShimUnsecuredLayoutsPageBase.AllInstances.WebGet = _ => spWeb;
            ShimAct.ConstructorSPWeb = (_, __) => new ShimAct();
            ShimSPContext.CurrentGet = () => new ShimSPContext();
            ShimSPContext.AllInstances.WebGet = _ => spWeb;
            ShimSPContext.AllInstances.SiteGet = _ => spSite;
            ShimSPFieldLookupValueCollection.ConstructorString = (_, __) => new ShimSPFieldLookupValueCollection();
            ShimSPFieldLookupValue.ConstructorString = (_, __) => new ShimSPFieldLookupValue();
            ShimSPFieldLookupValue.AllInstances.LookupIdGet = _ => DummyInt;
            ShimDisabledItemEventScope.Constructor = _ => new ShimDisabledItemEventScope();
            ShimDisabledItemEventScope.AllInstances.Dispose = _ => { };
            ShimSPUserCollection.AllInstances.GetByIDInt32 = (_, __) => spUser;
            ShimSPSiteDataQuery.Constructor = _ => new ShimSPSiteDataQuery();
        }

        private void SetupVariables()
        {
            validations = 0;
            publicStatic = BindingFlags.Static | BindingFlags.Public;
            publicInstance = BindingFlags.Instance | BindingFlags.Public;
            nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            guid = Guid.Parse(SampleGuidString1);
            spWeb = new ShimSPWeb()
            {
                IDGet = () => guid,
                SiteGet = () => spSite,
                ListsGet = () => spListCollection,
                GetFolderString = _ => spFolder,
                GetFileString = _ => spFile,
                FoldersGet = () => spFolderCollection,
                CurrentUserGet = () => spUser,
                ServerRelativeUrlGet = () => SampleUrl,
                AllUsersGet = () => new ShimSPUserCollection(),
                SiteUsersGet = () => new ShimSPUserCollection(),
            };
            spSite = new ShimSPSite()
            {
                IDGet = () => guid,
                WebApplicationGet = () => new ShimSPWebApplication(),
                RootWebGet = () => spWeb,
                FeaturesGet = () => new ShimSPFeatureCollection()
                {
                    ItemGetGuid = _ => new ShimSPFeature()
                },
                ContentDatabaseGet = () => new ShimSPContentDatabase()
            };
            spListCollection = new ShimSPListCollection()
            {
                TryGetListString = _ => spList,
                ItemGetString = _ => spList,
                ItemGetGuid = _ => spList
            };
            spList = new ShimSPList()
            {
                IDGet = () => guid,
                FieldsGet = () => spFieldCollection,
                GetItemByIdInt32 = _ => spListItem,
                ItemsGet = () => spListItemCollection,
                GetItemsSPQuery = _ => spListItemCollection,
                RootFolderGet = () => spFolder,
                ParentWebGet = () => spWeb,
                DefaultViewGet = () => spView,
                ViewsGet = () => spViewCollection,
                ContentTypesGet = () => spContentTypeCollection,
                TitleGet = () => DummyString,
                EventReceiversGet = () => new ShimSPEventReceiverDefinitionCollection(),
                DefaultViewUrlGet = () => SampleUrl
            };
            spListItemCollection = new ShimSPListItemCollection()
            {
                CountGet = () => DummyInt,
                ItemGetInt32 = _ => spListItem
            };
            spListItem = new ShimSPListItem()
            {
                IDGet = () => DummyInt,
                TitleGet = () => DummyString,
                ItemGetString = _ => DummyString,
                ItemGetGuid = _ => DummyString,
                ItemSetGuidObject = (_, __) => { },
                Update = () => { },
                FileGet = () => spFile,
                ParentListGet = () => spList,
                NameGet = () => DummyString
            };
            spFieldCollection = new ShimSPFieldCollection()
            {
                GetFieldByInternalNameString = _ => spField,
                ContainsFieldString = _ => false,
                GetFieldString = _ => spField,
                ItemGetString = _ => spField
            };
            spField = new ShimSPField()
            {
                IdGet = () => guid,
                TitleGet = () => DummyString,
                InternalNameGet = () => DummyString,
                ReadOnlyFieldGet = () => false,
                HiddenGet = () => false,
                ReorderableGet = () => true,
                TypeAsStringGet = () => DummyString
            };
            spUser = new ShimSPUser()
            {
                IDGet = () => DummyInt,
                IsSiteAdminGet = () => true,
                UserTokenGet = () => new ShimSPUserToken()
            };
            spFolderCollection = new ShimSPFolderCollection()
            {
                ItemGetString = _ => spFolder,
                AddString = _ => spFolder
            };
            spFolder = new ShimSPFolder()
            {
                ExistsGet = () => false,
                SubFoldersGet = () => spFolderCollection,
                FilesGet = () => spFileCollection,
                UrlGet = () => SampleUrl,
                UniqueIdGet = () => guid,
                ParentWebGet = () => spWeb
            };
            spFileCollection = new ShimSPFileCollection()
            {
                CountGet = () => DummyInt,
                AddStringByteArrayBoolean = (_1, _2, _3) => spFile,
                AddStringStream = (_1, _2) => spFile,
                ItemGetString = _ => spFile
            };
            spFile = new ShimSPFile()
            {
                Delete = () => { },
                OpenBinaryStream = () => null,
                NameGet = () => DummyString,
                GetListItemStringArray = _ => spListItem
            };
            spViewCollection = new ShimSPViewCollection()
            {
                ItemGetString = _ => spView
            };
            spView = new ShimSPView()
            {
                ViewFieldsGet = () => spViewFieldCollection,
                ServerRelativeUrlGet = () => SampleUrl
            };
            spViewFieldCollection = new ShimSPViewFieldCollection();
            spContentTypeCollection = new ShimSPContentTypeCollection()
            {
                ItemGetString = _ => spContentType
            };
            spContentType = new ShimSPContentType()
            {
                IdGet = () => default(SPContentTypeId),
                FieldLinksGet = () => spFieldLinkCollection
            };
            spFieldLinkCollection = new ShimSPFieldLinkCollection()
            {
                ItemGetGuid = _ => new ShimSPFieldLink()
            };
            transaction = new ShimSqlTransaction()
            {
                Commit = () => { },
                Rollback = () => { }
            };
        }

        [TestCleanup]
        public void TearDown()
        {
            shimsContext?.Dispose();
        }

        [TestMethod]
        public void StashCSRoleMode_WhenCalled_SetsRoleAllowedField()
        {
            // Arrange
            const bool expected = true;

            privateObject.SetFieldOrProperty(RolesAllowedFieldName, nonPublicInstance, false);

            // Act
            privateObject.Invoke(StashCSRoleModeMethodName, publicInstance, new object[] { expected });
            var roleAllowed = (bool)privateObject.GetFieldOrProperty(RolesAllowedFieldName, nonPublicInstance);

            // Assert
            roleAllowed.ShouldBe(expected);
        }

        [TestMethod]
        public void IsCSRoleAllowed_WhenCalled_SetsRoleAllowedField()
        {
            // Arrange
            const bool expected = true;

            privateObject.SetFieldOrProperty(RolesAllowedFieldName, nonPublicInstance, expected);

            // Act
            privateObject.Invoke(IsCSRoleAllowedMethodName, publicInstance, new object[] { });
            var roleAllowed = (bool)privateObject.GetFieldOrProperty(RolesAllowedFieldName, nonPublicInstance);

            // Assert
            roleAllowed.ShouldBe(expected);
        }

        [TestMethod]
        public void SetMajorCatListlookup_WhenCalled_SetsCatLookupField()
        {
            // Arrange
            var resourceValues = new clsResourceValues()
            {
                MajorCategoryFieldID = Two,
                Lookups = new Dictionary<int, clsLookupList>()
                {
                    [One] = new clsLookupList()
                    {
                        FieldID = One
                    },
                    [Two] = new clsLookupList()
                    {
                        FieldID = Two
                    }
                }
            };

            privateObject.SetFieldOrProperty(ResourceValuesFieldName, nonPublicInstance, resourceValues);

            // Act
            privateObject.Invoke(SetMajorCatListlookupMethodName, nonPublicInstance, new object[] { });
            var actual = (clsLookupList)privateObject.GetFieldOrProperty(CatLookupFieldName, nonPublicInstance);

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.ShouldNotBeNull(),
                () => actual.FieldID.ShouldBe(Two));
        }

        [TestMethod]
        public void ItemListAddItem_WhenCalled_AddsNewItem()
        {
            // Arrange
            var itemList = new List<clsEPKItem>();

            // Act
            privateObject.Invoke(
                ItemListAddItemMethodName,
                nonPublicInstance,
                new object[]
                {
                    itemList,
                    DummyInt,
                    DummyString,
                    DummyString
                });

            // Assert
            itemList.ShouldSatisfyAllConditions(
                () => itemList.Count.ShouldBe(1),
                () => itemList[0].ID.ShouldBe(DummyInt),
                () => itemList[0].Name.ShouldBe(DummyString));
        }

        [TestMethod]
        public void GrabRAData_WhenCalled_SetsResourceValuesAndOtherFields()
        {
            // Arrange
            const string xmlString = @"<xmlcfg/>";
            var resourceValues = new clsResourceValues()
            {
                MajorCategoryFieldID = Two,
                gpPMOAdmin = DummyInt,
                CommitmentsOpMode = 0,
                CommitmentHours = new List<clsCommitmentHours>(),
                Lookups = new Dictionary<int, clsLookupList>()
                {
                    [One] = new clsLookupList()
                    {
                        FieldID = One
                    },
                    [Two] = new clsLookupList()
                    {
                        FieldID = Two
                    }
                },
                Commitments = new Dictionary<int, clsCommitment>()
                {
                    [One] = new clsCommitment()
                    {
                        UID = One
                    }
                },
                OpenReqs = new Dictionary<int, clsCommitment>()
                {
                    [Two] = new clsCommitment()
                    {
                        UID = Two
                    }
                },
                OpenReqHours = new List<clsCommitmentHours>()
                {
                    new clsCommitmentHours()
                    {
                        UID = Three
                    }
                }
            };

            ShimRPAData.AllInstances.SetMajorCatListlookup = _ =>
            {
                validations += 1;
            };
            ShimRPAData.AllInstances.PopulateInternalsStringOut = (RPAData instance, out string serrlog) =>
            {
                validations += 1;
                serrlog = DummyString;
            };
            ShimRPAData.AllInstances.setupdispcolnsStringOut = (RPAData instance, out string errlog) =>
            {
                validations += 1;
                errlog = DummyString;
            };
            ShimRPAData.AllInstances.DoUserDepts = _ =>
            {
                validations += 1;
            };
            ShimRPAData.AllInstances.ReDrawGrid = _ =>
            {
                validations += 1;
            };

            // Act
            privateObject.Invoke(
                GrabRADataMethodName,
                nonPublicInstance,
                new object[]
                {
                    resourceValues,
                    DummyString,
                    DummyString,
                    DummyInt,
                    DummyInt,
                    xmlString,
                    DummyString
                });
            resourceValues = (clsResourceValues)privateObject.GetFieldOrProperty(ResourceValuesFieldName, nonPublicInstance);

            // Assert
            resourceValues.ShouldSatisfyAllConditions(
                () => resourceValues.Commitments.Count.ShouldBe(2),
                () => resourceValues.Commitments[100003].Status.ShouldBe(RPConstants.CONST_OPENREQUEST),
                () => resourceValues.Commitments[100003].UID.ShouldBe(100003),
                () => resourceValues.CommitmentHours.Count.ShouldBe(1),
                () => resourceValues.CommitmentHours[0].UID.ShouldBe(100004));
        }

        [TestMethod]
        public void DoUserDepts_WhenCalled_AddsUserDepts()
        {
            // Arrange
            const string expected = "All Departments";
            var resourceValues = new clsResourceValues()
            {
                UserDepartments = new List<int>()
                {
                    One,
                    Two,
                    Three
                },
                Departments = new Dictionary<int, clsEPKItem>()
                {
                    [Three] = new clsEPKItem()
                    {
                        Name = Three.ToString()
                    },
                    [One] = new clsEPKItem()
                    {
                        Name = One.ToString()
                    }
                }
            };

            privateObject.SetFieldOrProperty(ResourceValuesFieldName, nonPublicInstance, resourceValues);

            // Act
            privateObject.Invoke(DoUserDeptsMethodName, nonPublicInstance, new object[] { });
            var userDepts = (List<clsEPKItem>)privateObject.GetFieldOrProperty(UserDeptsFieldName, nonPublicInstance);

            // Assert
            userDepts.ShouldSatisfyAllConditions(
                () => userDepts.Count.ShouldBe(3),
                () => userDepts.Any(x => x.ID.Equals(One) && x.Name.Equals(One.ToString())),
                () => userDepts.Any(x => x.ID.Equals(Three) && x.Name.Equals(Three.ToString())),
                () => userDepts.Any(x => x.ID.Equals(0) && x.Name.Equals(expected)));
        }

        [TestMethod]
        public void BuileCCR2RoleMap_WhenCalled_AddsCostCategoryRoleMapping()
        {
            // Arrange
            var resourceValues = new clsResourceValues()
            {
                CostCategories = new Dictionary<int, clsCatItem>()
                {
                    [One] = new clsCatItem()
                    {
                        UID = One,
                        Name = One.ToString()
                    },
                    [Two] = new clsCatItem()
                    {
                        UID = Two,
                        Name = Two.ToString()
                    },
                    [Three] = new clsCatItem()
                    {
                        UID = Three,
                        Name = Three.ToString()
                    }
                },
                Roles = new Dictionary<int, clsListItem>()
                {
                    [One] = new clsListItem()
                    {
                        ID = One,
                        Name = One.ToString()
                    },
                    [Three] = new clsListItem()
                    {
                        ID = Three,
                        Name = Three.ToString()
                    }
                }
            };

            privateObject.SetFieldOrProperty(ResourceValuesFieldName, nonPublicInstance, resourceValues);

            // Act
            privateObject.Invoke(BuileCCR2RoleMapMethodName, nonPublicInstance, new object[] { });
            var mapping = (Dictionary<int, int>)privateObject.GetFieldOrProperty(CostCategoryRoleMappingFieldName, nonPublicInstance);

            // Assert
            mapping.ShouldSatisfyAllConditions(
                () => mapping.Count.ShouldBe(3),
                () => mapping[One].ShouldBe(One),
                () => mapping[Two].ShouldBe(0),
                () => mapping[Three].ShouldBe(Three));
        }

        [TestMethod]
        public void MapCCR2Role_KeyPresent_ReturnsProperInteger()
        {
            // Arrange
            const int category = One;
            var mapping = new Dictionary<int, int>()
            {
                [One] = DummyInt
            };

            privateObject.SetFieldOrProperty(CostCategoryRoleMappingFieldName, nonPublicInstance, mapping);

            // Act
            var actual = (int)privateObject.Invoke(MapCCR2RoleMethodName, nonPublicInstance, new object[] { category });

            // Assert
            actual.ShouldBe(DummyInt);
        }

        [TestMethod]
        public void MapCCR2Role_KeyNotPresent_ReturnsZero()
        {
            // Arrange
            const int category = Two;
            var mapping = new Dictionary<int, int>()
            {
                [One] = DummyInt
            };

            privateObject.SetFieldOrProperty(CostCategoryRoleMappingFieldName, nonPublicInstance, mapping);

            // Act
            var actual = (int)privateObject.Invoke(MapCCR2RoleMethodName, nonPublicInstance, new object[] { category });

            // Assert
            actual.ShouldBe(0);
        }

        [TestMethod]
        public void MapCCR2Role_MappingNull_ReturnsZero()
        {
            // Arrange
            const int category = Two;
            var mapping = default(Dictionary<int, int>);

            privateObject.SetFieldOrProperty(CostCategoryRoleMappingFieldName, nonPublicInstance, mapping);

            // Act
            var actual = (int)privateObject.Invoke(MapCCR2RoleMethodName, nonPublicInstance, new object[] { category });

            // Assert
            actual.ShouldBe(0);
        }

        [TestMethod]
        public void setupdispcolns_WhenCalled_SetsColumns()
        {
            // Arrange
            var resourceValues = new clsResourceValues()
            {
                ResFields = new List<clsPortField>()
                {
                    new clsPortField()
                    {
                        ID = 1111,
                        GivenName = One.ToString(),
                        Name = One.ToString()
                    },
                    new clsPortField()
                    {
                        ID = 1112,
                        GivenName = string.Empty,
                        Name = Two.ToString()
                    }
                },
                PlanFields = new List<clsPortField>()
                {
                    new clsPortField()
                    {
                        ID = 1113,
                        GivenName = One.ToString(),
                        Name = One.ToString()
                    },
                    new clsPortField()
                    {
                        ID = 1114,
                        GivenName = string.Empty,
                        Name = Two.ToString()
                    }
                },
                PIFields = new List<clsPortField>()
                {
                    new clsPortField()
                    {
                        ID = 1115,
                        GivenName = One.ToString(),
                        Name = One.ToString()
                    },
                    new clsPortField()
                    {
                        ID = 1116,
                        GivenName = string.Empty,
                        Name = Two.ToString()
                    }
                }
            };

            privateObject.SetFieldOrProperty(ResourceValuesFieldName, nonPublicInstance, resourceValues);

            // Act
            privateObject.Invoke(setupdispcolnsMethodName, nonPublicInstance, new object[] { DummyString });
            var totalMasterColumns = (List<clsRXDisp>)privateObject.GetFieldOrProperty(TotalMasterColumnFieldName, nonPublicInstance);
            var totalDispColumns = (List<clsRXDisp>)privateObject.GetFieldOrProperty(TotalDisplayColumnFieldName, nonPublicInstance);
            var detMasterColumns = (Dictionary<int, clsRXDisp>)privateObject.GetFieldOrProperty(DetMasterColumnFieldName, nonPublicInstance);
            var detDispColumns = (List<clsRXDisp>)privateObject.GetFieldOrProperty(DetDisplayColumnFieldName, nonPublicInstance);

            // Assert
            totalMasterColumns.ShouldSatisfyAllConditions(
                () => totalMasterColumns.Count.ShouldBe(8),
                () => totalMasterColumns.Count(x => x.m_dispname.Equals(One.ToString()) || x.m_dispname.Equals(Two.ToString())).ShouldBe(2),
                () => totalDispColumns.Count.ShouldBe(8),
                () => totalDispColumns.Count(x => x.m_dispname.Equals(One.ToString()) || x.m_dispname.Equals(Two.ToString())).ShouldBe(2),
                () => detMasterColumns.Count.ShouldBe(32),
                () => detMasterColumns.Count(x => x.Value.m_dispname.Equals(One.ToString()) || x.Value.m_dispname.Equals(Two.ToString())).ShouldBe(6),
                () => detDispColumns.Count.ShouldBe(32),
                () => detDispColumns.Count(x => x.m_dispname.Equals(One.ToString()) || x.m_dispname.Equals(Two.ToString())).ShouldBe(6));
        }
    }
}