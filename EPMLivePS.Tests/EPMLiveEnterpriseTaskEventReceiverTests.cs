﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Fakes;
using System.Data.SqlClient.Fakes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using EPMLiveCore.Fakes;
using EPMLiveEnterprise;
using EPMLiveEnterprise.Fakes;
using EPMLiveEnterprise.WebSvcCustomFields.Fakes;
using EPMLiveEnterprise.WebSvcLookupTables.Fakes;
using EPMLiveEnterprise.WebSvcProject.Fakes;
using EPMLiveEnterprise.WebSvcStatusing.Fakes;
using Microsoft.Office.Project.Server.Library;
using Microsoft.Office.Project.Server.Library.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Fakes;
using Microsoft.SharePoint.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using WebSvcProjectFakes = EPMLiveEnterprise.WebSvcProject.Fakes;
using static EPMLiveEnterprise.WebSvcCustomFields.CustomFieldDataSet;
using static EPMLiveEnterprise.WebSvcCustomFields.Fakes.ShimCustomFieldDataSet;
using static EPMLiveEnterprise.WebSvcLookupTables.LookupTableDataSet;
using static EPMLiveEnterprise.WebSvcLookupTables.Fakes.ShimLookupTableDataSet;

namespace EPMLivePS.Tests
{
    [TestClass]
    public class EPMLiveEnterpriseTaskEventReceiverItemEventReceiverTests
    {
        private EPMLiveEnterpriseTaskEventReceiverItemEventReceiver testObject;
        private PrivateObject privateObject;
        private IDisposable shimsContext;
        private BindingFlags nonPublicInstance;
        private BindingFlags publicInstance;
        private Guid guid;
        private ShimSPWeb spWeb;
        private ShimSPSite spSite;
        private ShimSPList spList;
        private ShimSPListItem spListItem;
        private ShimSPField spField;
        private ShimSqlDataReader dataReader;
        private ShimSPItemEventProperties properties;
        private const int DummyInt32 = 111;
        private const string DummyString = "DummyString";
        private const string GuidString = "83e81819-0112-4c22-bb70-d8ba101e9e0c";
        private const string ItemAddingMethodName = "ItemAdding";
        private const string ItemUpdatingMethodName = "ItemUpdating";
        private const string UpdateHandleMethodName = "UpdateHandle";
        private const string InsertHandleMethodName = "InsertHandle";
        private const string GetProjectUidMethodName = "getProjectUid";
        private const string IsFieldEditableMethodName = "isFieldEditable";
        private const string UpdateTaskMethodName = "UpdateTask";
        private const string ValueToUseMethodName = "ValueToUse";

        [TestInitialize]
        public void Setup()
        {
            SetupShims();
            SetupVariables();
            testObject = new EPMLiveEnterpriseTaskEventReceiverItemEventReceiver();
            privateObject = new PrivateObject(testObject);
        }

        private void SetupShims()
        {
            shimsContext = ShimsContext.Create();

            ShimSqlConnection.Constructor = _ => new ShimSqlConnection();
            ShimSqlConnection.ConstructorString = (_, _1) => new ShimSqlConnection();
            ShimStatusing.Constructor = _ => new ShimStatusing();
            ShimCustomFields.Constructor = _ => new ShimCustomFields();
            ShimCustomFieldDataSet.Constructor = _ => new ShimCustomFieldDataSet();
            ShimLookupTableDataSet.Constructor = _ => new ShimLookupTableDataSet();
            WebSvcProjectFakes.ShimProject.Constructor = _ => new WebSvcProjectFakes.ShimProject();
            ShimLookupTable.Constructor = _ => new ShimLookupTable();
            ShimSPFieldLookupValue.ConstructorString = (_, __) => new ShimSPFieldLookupValue();
            ShimSPSite.ConstructorString = (_, __) => new ShimSPSite();
            ShimSPSite.ConstructorGuid = (_, __) => new ShimSPSite();
            ShimSPSite.AllInstances.IDGet = _ => guid;
            ShimSPSite.AllInstances.UrlGet = _ => DummyString;
            ShimSPSite.AllInstances.WebApplicationGet = _ => new ShimSPWebApplication();
            ShimSPSite.AllInstances.Close = _ => { };
            ShimSPPersistedObject.AllInstances.IdGet = _ => guid;
            ShimSPEventPropertiesBase.AllInstances.SiteIdGet = _ => guid;
            ShimSPEventPropertiesBase.AllInstances.ErrorMessageSetString = (_, _1) => { };
            ShimSPEventPropertiesBase.AllInstances.CancelSetBoolean = (_, _1) => { };
            ShimSPFieldLookupValue.AllInstances.LookupIdGet = _ => DummyInt32;
            ShimSPSecurity.RunWithElevatedPrivilegesSPSecurityCodeToRunElevated = codeToRunElevated => codeToRunElevated();
            ShimCoreFunctions.getConnectionStringGuid = _ => DummyString;
            ShimCoreFunctions.getListSettingStringSPList = (_, __) => DummyString;
            ShimSqlConnection.AllInstances.Open = _ => { };
            ShimSqlConnection.AllInstances.Close = _ => { };
            ShimSqlCommand.AllInstances.ExecuteReader = _ => dataReader;
            ShimEntityCollection.EntitiesGet = () => new ShimEntityCollection()
            {
                TaskEntityGet = () => new ShimEntity()
                {
                    UniqueIdGet = () => GuidString
                }
            };
        }

        private void SetupVariables()
        {
            nonPublicInstance = BindingFlags.Instance | BindingFlags.NonPublic;
            publicInstance = BindingFlags.Instance | BindingFlags.Public;
            guid = Guid.Parse(GuidString);
            dataReader = new ShimSqlDataReader()
            {
                Read = () => true,
                GetStringInt32 = _ => DummyString,
                Close = () => { }
            };
            properties = new ShimSPItemEventProperties()
            {
                ListItemGet = () => spListItem,
                ListIdGet = () => guid,
                OpenWeb = () => spWeb,
                WebUrlGet = () => DummyString,
                AfterPropertiesGet = () => new ShimSPItemEventDataCollection(),
                UserLoginNameGet = () => DummyString
            };
            spField = new ShimSPField()
            {
                InternalNameGet = () => DummyString
            };
            spListItem = new ShimSPListItem()
            {
                ItemGetString = _ => DummyString,
                TitleGet = () => DummyString
            };
            spList = new ShimSPList()
            {
                GetItemByIdInt32 = _ => spListItem,
                FieldsGet = () => new ShimSPFieldCollection()
                {
                    GetFieldByInternalNameString = _ => spField
                }
            };
            spSite = new ShimSPSite()
            {
                UrlGet = () => DummyString,
                IDGet = () => guid
            };
            spWeb = new ShimSPWeb()
            {
                ListsGet = () => new ShimSPListCollection()
                {
                    ItemGetString = _ => spList,
                    ItemGetGuid = _ => spList
                },
                SiteGet = () => spSite,
                Close = () => { },
                IDGet = () => guid
            };
        }

        [TestCleanup]
        public void TearDown()
        {
            shimsContext?.Dispose();
        }

        [TestMethod]
        public void ItemAdding_WhenCalled_Inserts()
        {
            // Arrange
            var validation = 0;

            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.InsertHandleSPItemEventProperties = (_, __) =>
            {
                validation += 1;
                return false;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ErrorTrapInt32String = (_, _1, _2) =>
            {
                validation += 1;
            };

            // Act
            privateObject.Invoke(ItemAddingMethodName, publicInstance, new object[] { properties.Instance });

            // Assert
            validation.ShouldBe(2);
        }

        [TestMethod]
        public void ItemUpdating_WhenCalled_Updates()
        {
            // Arrange
            var validation = 0;

            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateHandleSPItemEventProperties = (_, __) =>
            {
                validation += 1;
                return false;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ErrorTrapInt32String = (_, _1, _2) =>
            {
                validation += 1;
            };

            // Act
            privateObject.Invoke(ItemUpdatingMethodName, publicInstance, new object[] { properties.Instance });

            // Assert
            validation.ShouldBe(2);
        }

        [TestMethod]
        public void UpdateHandle_WhenCalled_Updates()
        {
            // Arrange
            var validation = 0;

            spListItem.ItemGetString = _ => $"{DummyString}.{DummyString}";
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                throw new Exception();
            };
            ShimSPItemEventDataCollection.AllInstances.ItemSetStringObject = (_, _1, _2) =>
            {
                validation += 1;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getProjectUidStringSPWeb = (_, _1, _2) =>
            {
                validation += 1;
                return GuidString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getPubTypeString = (_, _1) =>
            {
                validation += 1;
                return 1;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.RightStringInt32 = (_, _1) =>
            {
                validation += 1;
                return GuidString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateTaskGuidGuidSPItemEventProperties = (_, _1, _2, _3) =>
            {
                validation += 1;
            };
            spWeb.Close = () =>
            {
                validation += 1;
                throw new Exception();
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ErrorTrapInt32String = (_, _1, _2) =>
            {
                validation += 1;
            };

            // Act
            var actual = (bool)privateObject.Invoke(UpdateHandleMethodName, publicInstance, new object[] { properties.Instance });

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.ShouldBeFalse(),
                () => validation.ShouldBe(7));
        }

        [TestMethod]
        public void InsertHandle_WhenCalled_Inserts()
        {
            // Arrange
            var validation = 0;
            var methodHit = 0;

            spListItem.ItemGetString = _ => $"{DummyString}.{DummyString}";
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                methodHit += 1;
                if (methodHit.Equals(1))
                {
                    throw new Exception();
                }
                return DummyString;
            };
            ShimSPItemEventDataCollection.AllInstances.ItemSetStringObject = (_, _1, _2) =>
            {
                validation += 1;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getProjectUidStringSPWeb = (_, _1, _2) =>
            {
                validation += 1;
                return GuidString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getPubTypeString = (_, _1) =>
            {
                validation += 1;
                return 1;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.InsertTaskGuidStringStringStringSPItemEventProperties = (_, _1, _2, _3, _4, _5) =>
            {
                validation += 1;
                return DummyString;
            };
            spWeb.Close = () =>
            {
                validation += 1;
                throw new Exception();
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ErrorTrapInt32String = (_, _1, _2) =>
            {
                validation += 1;
            };

            // Act
            var actual = (bool)privateObject.Invoke(InsertHandleMethodName, publicInstance, new object[] { properties.Instance });

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual.ShouldBeFalse(),
                () => validation.ShouldBe(6));
        }

        [TestMethod]
        public void GetProjectUid_WhenCalled_ReturnsString()
        {
            // Arrange
            var projectName = "111";

            // Act
            var actual = (string)privateObject.Invoke(GetProjectUidMethodName, publicInstance, new object[] { projectName, spWeb.Instance });

            // Assert
            actual.ShouldBe(DummyString);
        }

        [TestMethod]
        public void IsFieldEditable_WhereMe_ReturnsBoolean()
        {
            // Arrange
            var displaySettings = "where;[Me];condition;group;valueCondition";
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>()
            {
                [DummyString] = new Dictionary<string, string>()
                {
                    ["Edit"] = displaySettings
                }
            };

            ShimEditableFieldDisplay.RenderFieldSPFieldStringStringStringStringStringSPListItem = (_, _1, _2, _3, _4, _5, _6) => false;

            privateObject.SetFieldOrProperty("fieldProperties", nonPublicInstance, fieldProperties);

            // Act
            var actual = (bool)privateObject.Invoke(IsFieldEditableMethodName, nonPublicInstance, new object[] { spField.Instance, spListItem.Instance });

            // Assert
            actual.ShouldBeTrue();
        }

        [TestMethod]
        public void IsFieldEditable_WhereNotMe_ReturnsBoolean()
        {
            // Arrange
            var displaySettings = "where;[NotMe];condition;group;valueCondition";
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>()
            {
                [DummyString] = new Dictionary<string, string>()
                {
                    ["Edit"] = displaySettings
                }
            };

            ShimEditableFieldDisplay.RenderFieldSPFieldStringStringStringStringStringSPListItem = (_, _1, _2, _3, _4, _5, _6) => true;

            privateObject.SetFieldOrProperty("fieldProperties", nonPublicInstance, fieldProperties);

            // Act
            var actual = (bool)privateObject.Invoke(IsFieldEditableMethodName, nonPublicInstance, new object[] { spField.Instance, spListItem.Instance });

            // Assert
            actual.ShouldBeFalse();
        }

        [TestMethod]
        public void UpdateTask_FieldCategory1_UpdatesTask()
        {
            // Arrange
            var validation = 0;
            var customFields = new ShimCustomFields()
            {
                UrlSetString = _ => { },
                UseDefaultCredentialsSetBoolean = _ => { },
                ReadCustomFieldsByEntityGuid = _ => new ShimCustomFieldDataSet()
                {
                    CustomFieldsGet = () => new ShimCustomFieldsDataTable()
                }
            };
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>();
            var fieldDataSet = new DataSet();
            var dataTable = new DataTable();
            dataTable.Columns.Add("wssFieldName");
            dataTable.Columns.Add("fieldtype");
            dataTable.Columns.Add("fieldCategory", typeof(int));
            dataTable.Columns.Add("assnUpdateColumnId");
            dataTable.Columns.Add("multiplier", typeof(int));
            var row = dataTable.NewRow();
            row["wssFieldName"] = DummyString;
            row["fieldtype"] = DummyString;
            row["fieldCategory"] = 1;
            row["assnUpdateColumnId"] = "assnUpdateColumn";
            row["multiplier"] = 1;
            dataTable.Rows.Add(row);
            fieldDataSet.Tables.Add(dataTable);

            spField.TypeGet = () => SPFieldType.DateTime;
            spField.SchemaXmlGet = () => @"<SchemaXml Format=""DateOnly""/>";
            ShimListDisplayUtils.ConvertFromStringString = _ => fieldProperties;
            ShimDataTable.AllInstances.SelectString = (_, _1) =>
            {
                return new CustomFieldsRow[]
                {
                    new ShimCustomFieldsRow()
                    {
                        MD_PROP_UID_SECONDARYGet = () => guid,
                        MD_PROP_NAMEGet = () => DummyString
                    }
                };
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.RetrieveEditableFields = _ =>
            {
                validation += 1;
                return fieldDataSet;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.GetResourceGuidByWindowsAccountStringString = (_, _1, _2) =>
            {
                validation += 1;
                return guid;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.isFieldEditableSPFieldSPListItem = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ReturnCalculatedFieldStringInt32StringInt32 = (_, _1, _2, _3, _4) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ValueToUseSPItemEventPropertiesSPField = (_, _1, _2) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.MidStringInt32Int32 = (input, _1, _2) =>
            {
                validation += 1;
                return input;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateStatusStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.SubmitAllUpdatesStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                validation += 1;
                return DummyInt32;
            };

            privateObject.SetFieldOrProperty("pwaSiteUrl", nonPublicInstance, DummyString);
            privateObject.SetFieldOrProperty("pwaSiteGuid", nonPublicInstance, guid);
            privateObject.SetFieldOrProperty("CustomFields", nonPublicInstance, customFields.Instance);

            // Act
            privateObject.Invoke(UpdateTaskMethodName, publicInstance, new object[] { guid, guid, properties.Instance });

            // Assert
            validation.ShouldBe(12);
        }

        [TestMethod]
        public void UpdateTask_FieldCategory2_UpdatesTask()
        {
            // Arrange
            var validation = 0;
            var customFields = new ShimCustomFields()
            {
                UrlSetString = _ => { },
                UseDefaultCredentialsSetBoolean = _ => { },
                ReadCustomFieldsByEntityGuid = _ => new ShimCustomFieldDataSet()
                {
                    CustomFieldsGet = () => new ShimCustomFieldsDataTable()
                }
            };
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>();
            var fieldDataSet = new DataSet();
            var dataTable = new DataTable();
            dataTable.Columns.Add("wssFieldName");
            dataTable.Columns.Add("fieldtype");
            dataTable.Columns.Add("fieldCategory", typeof(int));
            dataTable.Columns.Add("assnUpdateColumnId");
            dataTable.Columns.Add("multiplier", typeof(int));
            var row = dataTable.NewRow();
            row["wssFieldName"] = "Dur";
            row["fieldtype"] = "DATETIME";
            row["fieldCategory"] = 2;
            row["assnUpdateColumnId"] = "assnUpdateColumn";
            row["multiplier"] = 1;
            dataTable.Rows.Add(row);
            fieldDataSet.Tables.Add(dataTable);

            dataReader.GetStringInt32 = _ => "251658250";
            spField.TypeGet = () => SPFieldType.DateTime;
            spField.SchemaXmlGet = () => @"<SchemaXml Format=""DateOnly""/>";
            ShimListDisplayUtils.ConvertFromStringString = _ => fieldProperties;
            ShimDataTable.AllInstances.SelectString = (_, _1) =>
            {
                return new CustomFieldsRow[]
                {
                    new ShimCustomFieldsRow()
                    {
                        MD_PROP_UID_SECONDARYGet = () => guid,
                        MD_PROP_NAMEGet = () => DummyString
                    }
                };
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.RetrieveEditableFields = _ =>
            {
                validation += 1;
                return fieldDataSet;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.GetResourceGuidByWindowsAccountStringString = (_, _1, _2) =>
            {
                validation += 1;
                return guid;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.isFieldEditableSPFieldSPListItem = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ReturnCalculatedFieldStringInt32StringInt32 = (_, _1, _2, _3, _4) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ValueToUseSPItemEventPropertiesSPField = (_, _1, _2) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.MidStringInt32Int32 = (input, _1, _2) =>
            {
                validation += 1;
                return input;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateStatusStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.SubmitAllUpdatesStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getCustomFieldChangeTypeString = (_, _1) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                validation += 1;
                return DummyInt32;
            };

            privateObject.SetFieldOrProperty("pwaSiteUrl", nonPublicInstance, DummyString);
            privateObject.SetFieldOrProperty("pwaSiteGuid", nonPublicInstance, guid);
            privateObject.SetFieldOrProperty("CustomFields", nonPublicInstance, customFields.Instance);

            // Act
            privateObject.Invoke(UpdateTaskMethodName, publicInstance, new object[] { guid, guid, properties.Instance });

            // Assert
            validation.ShouldBe(14);
        }

        [TestMethod]
        public void UpdateTask_FieldCategory3UIDNull_UpdatesTask()
        {
            // Arrange
            var validation = 0;
            var customFields = new ShimCustomFields()
            {
                UrlSetString = _ => { },
                UseDefaultCredentialsSetBoolean = _ => { },
                ReadCustomFieldsByEntityGuid = _ => new ShimCustomFieldDataSet()
                {
                    CustomFieldsGet = () => new ShimCustomFieldsDataTable()
                }
            };
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>();
            var fieldDataSet = new DataSet();
            var dataTable = new DataTable();
            dataTable.Columns.Add("wssFieldName");
            dataTable.Columns.Add("assnfieldname");
            dataTable.Columns.Add("fieldtype");
            dataTable.Columns.Add("fieldCategory", typeof(int));
            dataTable.Columns.Add("assnUpdateColumnId");
            dataTable.Columns.Add("multiplier", typeof(int));
            var row = dataTable.NewRow();
            row["wssFieldName"] = "Dur";
            row["assnfieldname"] = "Dur";
            row["fieldtype"] = "DATETIME";
            row["fieldCategory"] = 3;
            row["assnUpdateColumnId"] = "assnUpdateColumn";
            row["multiplier"] = 1;
            dataTable.Rows.Add(row);
            fieldDataSet.Tables.Add(dataTable);

            dataReader.GetStringInt32 = _ => "251658250";
            spField.TypeGet = () => SPFieldType.DateTime;
            spField.SchemaXmlGet = () => @"<SchemaXml Format=""DateOnly""/>";
            ShimListDisplayUtils.ConvertFromStringString = _ => fieldProperties;
            ShimDataTable.AllInstances.SelectString = (_, _1) =>
            {
                return new CustomFieldsRow[]
                {
                    new ShimCustomFieldsRow()
                    {
                        MD_PROP_UID_SECONDARYGet = () => guid,
                        MD_PROP_NAMEGet = () => DummyString,
                        MD_PROP_TYPE_ENUMGet = () => (byte)PSDataType.NUMBER,
                        IsMD_LOOKUP_TABLE_UIDNull = () => true
                    }
                };
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.RetrieveEditableFields = _ =>
            {
                validation += 1;
                return fieldDataSet;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.GetResourceGuidByWindowsAccountStringString = (_, _1, _2) =>
            {
                validation += 1;
                return guid;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.isFieldEditableSPFieldSPListItem = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ReturnCalculatedFieldStringInt32StringInt32 = (_, _1, _2, _3, _4) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ValueToUseSPItemEventPropertiesSPField = (_, _1, _2) =>
            {
                validation += 1;
                return string.Empty;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.MidStringInt32Int32 = (input, _1, _2) =>
            {
                validation += 1;
                return input;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateStatusStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.SubmitAllUpdatesStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getCustomFieldChangeTypePSDataType = (_, _1) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                validation += 1;
                return DummyInt32;
            };

            privateObject.SetFieldOrProperty("pwaSiteUrl", nonPublicInstance, DummyString);
            privateObject.SetFieldOrProperty("pwaSiteGuid", nonPublicInstance, guid);
            privateObject.SetFieldOrProperty("CustomFields", nonPublicInstance, customFields.Instance);

            // Act
            privateObject.Invoke(UpdateTaskMethodName, publicInstance, new object[] { guid, guid, properties.Instance });

            // Assert
            validation.ShouldBe(11);
        }

        [TestMethod]
        public void UpdateTask_FieldCategory3UIDNotNull_UpdatesTask()
        {
            // Arrange
            var validation = 0;
            var customFields = new ShimCustomFields()
            {
                UrlSetString = _ => { },
                UseDefaultCredentialsSetBoolean = _ => { },
                ReadCustomFieldsByEntityGuid = _ => new ShimCustomFieldDataSet()
                {
                    CustomFieldsGet = () => new ShimCustomFieldsDataTable()
                }
            };
            var lookupTable = new ShimLookupTable()
            {
                ReadLookupTablesByUidsGuidArrayBooleanInt32 = (_, _1, _2) => new ShimLookupTableDataSet()
                {
                    LookupTableTreesGet = () => new ShimLookupTableTreesDataTable()
                }
            };
            var fieldProperties = new Dictionary<string, Dictionary<string, string>>();
            var fieldDataSet = new DataSet();
            var dataTable = new DataTable();
            dataTable.Columns.Add("wssFieldName");
            dataTable.Columns.Add("assnfieldname");
            dataTable.Columns.Add("fieldtype");
            dataTable.Columns.Add("fieldCategory", typeof(int));
            dataTable.Columns.Add("assnUpdateColumnId");
            dataTable.Columns.Add("multiplier", typeof(int));
            var row = dataTable.NewRow();
            row["wssFieldName"] = "Dur";
            row["assnfieldname"] = "Dur";
            row["fieldtype"] = "DATETIME";
            row["fieldCategory"] = 3;
            row["assnUpdateColumnId"] = "assnUpdateColumn";
            row["multiplier"] = 1;
            dataTable.Rows.Add(row);
            fieldDataSet.Tables.Add(dataTable);

            dataReader.GetStringInt32 = _ => "251658250";
            spField.TypeGet = () => SPFieldType.DateTime;
            spField.SchemaXmlGet = () => @"<SchemaXml Format=""DateOnly""/>";
            ShimListDisplayUtils.ConvertFromStringString = _ => fieldProperties;
            ShimDataTable.AllInstances.SelectString = (_, condition) =>
            {
                if (condition.StartsWith("MD_PROP_UID_SECONDARY"))
                {
                    return new CustomFieldsRow[]
                    {
                        new ShimCustomFieldsRow()
                        {
                            MD_PROP_UID_SECONDARYGet = () => guid,
                            MD_PROP_NAMEGet = () => DummyString,
                            MD_PROP_TYPE_ENUMGet = () => (byte)PSDataType.NUMBER,
                            IsMD_LOOKUP_TABLE_UIDNull = () => false
                        }
                    };
                }

                return new LookupTableTreesRow[]
                {
                    new ShimLookupTableTreesRow()
                    {
                        LT_UIDGet = () => guid
                    }
                };
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.RetrieveEditableFields = _ =>
            {
                validation += 1;
                return fieldDataSet;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.GetResourceGuidByWindowsAccountStringString = (_, _1, _2) =>
            {
                validation += 1;
                return guid;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.isFieldEditableSPFieldSPListItem = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ReturnCalculatedFieldStringInt32StringInt32 = (_, _1, _2, _3, _4) =>
            {
                validation += 1;
                return DummyString;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.ValueToUseSPItemEventPropertiesSPField = (_, _1, _2) =>
            {
                validation += 1;
                return string.Empty;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.MidStringInt32Int32 = (input, _1, _2) =>
            {
                validation += 1;
                return input;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.UpdateStatusStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.SubmitAllUpdatesStringSPItemEventProperties = (_, _1, _2) =>
            {
                validation += 1;
                return true;
            };
            ShimEPMLiveEnterpriseTaskEventReceiverItemEventReceiver.AllInstances.getCustomFieldChangeTypePSDataType = (_, _1) =>
            {
                validation += 1;
                return "Number";
            };
            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) =>
            {
                validation += 1;
                return DummyInt32;
            };

            privateObject.SetFieldOrProperty("pwaSiteUrl", nonPublicInstance, DummyString);
            privateObject.SetFieldOrProperty("pwaSiteGuid", nonPublicInstance, guid);
            privateObject.SetFieldOrProperty("CustomFields", nonPublicInstance, customFields.Instance);
            privateObject.SetFieldOrProperty("LookupTable", nonPublicInstance, lookupTable.Instance);

            // Act
            privateObject.Invoke(UpdateTaskMethodName, publicInstance, new object[] { guid, guid, properties.Instance });

            // Assert
            validation.ShouldBe(12);
        }

        [TestMethod]
        public void ValueToUse_WhenCalled_ReturnsString()
        {
            // Arrange
            const string expected = "somerandomtexttobetested";

            ShimSPItemEventDataCollection.AllInstances.ItemGetString = (_, __) => expected;

            // Act
            var actual = (string)privateObject.Invoke(ValueToUseMethodName, nonPublicInstance, new object[] { properties.Instance, spField.Instance });

            // Assert
            actual.ShouldBe(expected);
        }
    }
}