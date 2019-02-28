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
using System.Xml;
using System.Xml.Linq;
using EPMLiveCore.API.Fakes;
using EPMLiveCore.Fakes;
using EPMLiveWorkPlanner.Fakes;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration.Fakes;
using Microsoft.SharePoint.Fakes;
using Microsoft.SharePoint.Utilities.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shouldly;
using static EPMLiveWorkPlanner.WorkPlannerAPI;

namespace EPMLiveWorkPlanner.Tests.Jobs
{
    [TestClass, ExcludeFromCodeCoverage]
    public class PublishJobTests
    {
        private PublishJob testObject;
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
        private const string UserIdFieldName = "userid";
        private const string KeyFieldName = "key";
        private const string ErrorBooleanFieldName = "bErrors";
        private const string ErrorStringFieldName = "sErrors";
        private const string TotalCountFieldName = "totalCount";
        private const string HashLinksFieldName = "hshLinks";
        private const string TaskUidColumnName = "taskuid";
        private const string IsPublishedColumnName = "IsPublished";
        private const string UpdateScheduledWorkNodeName = "UpdateScheduledWork";
        private const string MoveListItemToFolderMethodName = "MoveListItemToFolder";
        private const string SetupTaskCenterMethodName = "setupTaskCenter";
        private const string EnsureFolderMethodName = "ensureFolder";
        private const string StartPublishMethodName = "StartPublish";
        private const string ExecuteMethodName = "execute";
        private const string FormatPFEWorkJobXmlMethodName = "FormatPFEWorkJobXml";
        private const string ProcessTaskMethodName = "processTask";
        private const string PublishTasksMethodName = "publishTasks";

        [TestInitialize]
        public void Setup()
        {
            testObject = new PublishJob();
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
            ShimComponent.AllInstances.Dispose = _ => { };
            ShimSqlCommand.AllInstances.TransactionSetSqlTransaction = (_, __) => { };
            ShimSPDatabase.AllInstances.DatabaseConnectionStringGet = _ => DummyString;
            ShimSPSite.ConstructorGuidSPUserToken = (_, _1, _2) => new ShimSPSite();
            ShimSPSite.AllInstances.OpenWebGuid = (_, __) => spWeb;
            ShimSPSite.AllInstances.Dispose = _ => { };
            ShimSPWeb.AllInstances.Dispose = _ => { };
            ShimCoreFunctions.getConnectionStringGuid = _ => DummyString;
            ShimSPPersistedObject.AllInstances.IdGet = _ => guid;
            ShimDisabledItemEventScope.Constructor = _ => new ShimDisabledItemEventScope();
            ShimDisabledItemEventScope.AllInstances.Dispose = _ => { };
            ShimSPUserCollection.AllInstances.GetByIDInt32 = (_, __) => spUser;
            ShimSPSiteDataQuery.Constructor = _ => new ShimSPSiteDataQuery();
            ShimSPList.AllInstances.GetItemsSPQuery = (sender, spQuery) => new ShimSPListItemCollection();
            ShimSPListItemCollection.AllInstances.GetEnumerator = sender => new List<SPListItem>().GetEnumerator();
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
                ServerRelativeUrlGet = () => SampleUrl,
                AllUsersGet = () => new ShimSPUserCollection(),
                SiteUsersGet = () => new ShimSPUserCollection(),
            };
            spSite = new ShimSPSite()
            {
                IDGet = () => guid,
                WebApplicationGet = () => new ShimSPWebApplication(),
                FeaturesGet = () => new ShimSPFeatureCollection()
                {
                    ItemGetGuid = _ => new ShimSPFeature()
                },
                ContentDatabaseGet = () => new ShimSPContentDatabase()
            };
            spListCollection = new ShimSPListCollection()
            {
                ItemGetString = _ => spList,
                ItemGetGuid = _ => spList
            };
            spList = new ShimSPList()
            {
                IDGet = () => guid,
                FieldsGet = () => spFieldCollection,
                GetItemByIdInt32 = _ => spListItem,
                ParentWebGet = () => spWeb,
                ContentTypesGet = () => spContentTypeCollection,
                EventReceiversGet = () => new ShimSPEventReceiverDefinitionCollection(),
                DefaultViewUrlGet = () => SampleUrl
            };
            spListItemCollection = new ShimSPListItemCollection();
            spListItem = new ShimSPListItem()
            {
                IDGet = () => DummyInt,
                ItemGetGuid = _ => DummyString,
                ParentListGet = () => spList,
                NameGet = () => DummyString
            };
            spFieldCollection = new ShimSPFieldCollection()
            {
                GetFieldByInternalNameString = _ => spField,
                ItemGetString = _ => spField
            };
            spField = new ShimSPField()
            {
                IdGet = () => guid,
                ReadOnlyFieldGet = () => false,
            };
            spUser = new ShimSPUser()
            {
                IDGet = () => DummyInt,
                UserTokenGet = () => new ShimSPUserToken()
            };
            spFolderCollection = new ShimSPFolderCollection();
            spFolder = new ShimSPFolder()
            {
                ExistsGet = () => false,
                UrlGet = () => SampleUrl,
                UniqueIdGet = () => guid,
                ParentWebGet = () => spWeb
            };
            spFileCollection = new ShimSPFileCollection();
            spFile = new ShimSPFile();
            spViewCollection = new ShimSPViewCollection();
            spView = new ShimSPView();
            spViewFieldCollection = new ShimSPViewFieldCollection();
            spContentTypeCollection = new ShimSPContentTypeCollection()
            {
                ItemGetString = _ => spContentType
            };
            spContentType = new ShimSPContentType()
            {
                IdGet = () => default(SPContentTypeId)
            };
            spFieldLinkCollection = new ShimSPFieldLinkCollection();
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
        public void MoveListItemToFolder_WhenCalled_UpdatesDatabase()
        {
            // Arrange
            var expectedCommands = new List<string>()
            {
                $"update AllUserData set tp_ParentId = @tp_ParentId where tp_ListId = @ListId and tp_ID = @ItemId;",
                $"update AllDocs set DirName = @NewDir, ParentId = @ParentId where ListId = @ListId and doclibrowid = @ItemId"
            };

            ShimSPUrlUtility.CombineUrlStringString = (_, __) =>
            {
                validations += 1;
                return SampleUrl;
            };
            ShimSqlCommand.AllInstances.ExecuteNonQuery = command =>
            {
                if (expectedCommands.Contains(command.CommandText))
                {
                    validations += 1;
                }
                return DummyInt;
            };

            // Act
            var actual = privateObject.Invoke(
                MoveListItemToFolderMethodName,
                publicStatic,
                new object[]
                {
                    spListItem.Instance,
                    spFolder.Instance
                });

            // Assert
            validations.ShouldBe(3);
        }

        [TestMethod]
        public void SetupTaskCenter_WhenCalled_AddsEventReceiver()
        {
            // Arrange
            var textField = new ShimSPFieldText();

            spFieldCollection.ContainsFieldString = _ =>
            {
                validations += 1;
                return false;
            };
            spFieldCollection.AddStringSPFieldTypeBoolean = (_1, _2, _3) =>
            {
                validations += 1;
                return DummyString;
            };
            spFieldCollection.AddSPField = _ =>
            {
                validations += 1;
                return DummyString;
            };
            spFieldCollection.CreateNewFieldStringString = (_, __) =>
            {
                validations += 1;
                return textField;
            };
            spField.RequiredSetBoolean = input =>
            {
                if (!input)
                {
                    validations += 1;
                }
            };
            spField.HiddenSetBoolean = input =>
            {
                if (input)
                {
                    validations += 1;
                }
            };
            spField.TitleSetString = input =>
            {
                validations += 1;
            };
            spField.Update = () =>
            {
                validations += 1;
            };
            spList.Update = () =>
            {
                validations += 1;
            };

            ShimSPField.AllInstances.HiddenSetBoolean = (_, input) =>
            {
                if (input)
                {
                    validations += 1;
                }
            };
            ShimSPField.AllInstances.RequiredSetBoolean = (_, input) =>
            {
                if (!input)
                {
                    validations += 1;
                }
            };
            ShimSPField.AllInstances.Update = _ =>
            {
                validations += 1;
            };
            ShimSPBaseCollection.AllInstances.GetEnumerator = _ => new List<SPEventReceiverDefinition>()
            {
                new ShimSPEventReceiverDefinition()
                {
                    TypeGet = () => SPEventReceiverType.ItemAdding,
                    ClassGet = () => DummyString
                }
            }.GetEnumerator();
            ShimSPEventReceiverDefinitionCollection.AllInstances.AddSPEventReceiverTypeStringString = (_, _1, _2, _3) =>
            {
                validations += 1;
            };

            // Act
            privateObject.Invoke(SetupTaskCenterMethodName, nonPublicInstance, new object[] { spList.Instance });

            // Assert
            validations.ShouldBe(19);
        }

        [TestMethod]
        public void EnsureFolder_WhenCalled_UpdatesListItem()
        {
            // Arrange
            spList.AddItemStringSPFileSystemObjectTypeString = (fullFolder, objectType, folder) =>
            {
                if (fullFolder.Equals(SampleUrl) && folder.Equals(DummyString))
                {
                    validations += 1;
                }
                return spListItem;
            };
            spListItem.ItemSetStringObject = (_, __) =>
            {
                validations += 1;
            };
            spListItem.Update = () =>
            {
                validations += 1;
            };

            ShimPath.GetDirectoryNameString = input => input;

            // Act
            var actual = privateObject.Invoke(EnsureFolderMethodName, nonPublicInstance, new object[] { spList.Instance, DummyString });

            // Assert
            validations.ShouldBe(4);
        }

        [TestMethod]
        public void StartPublish_WhenCalled_PublishesTasks()
        {
            // Arrange
            const string xmlString = @"
                <xmlcfg>
                    <Task/>
                    <Task/>
                    <Task/>
                    <Task/>
                    <Task/>
                </xmlcfg>";
            var document = new XmlDocument();
            var plannerProps = new PlannerProps();
            var methodHit = 0;

            document.LoadXml(xmlString);

            spListItem.Update = () =>
            {
                methodHit += 1;
                validations += 1;
                if (methodHit.Equals(1))
                {
                    throw new Exception();
                }
            };

            ShimPublishJob.AllInstances.getPrefixSPSite = (_, __) =>
            {
                validations += 1;
                return DummyString;
            };
            ShimPublishJob.AllInstances.publishTasksXmlDocumentSPListStringHashtableStringStringWorkPlannerAPIPlannerPropsString =
                (_, _1, _2, _3, _4, _5, _6, _7, _8) =>
                {
                    validations += 1;
                };

            // Act
            var actual = privateObject.Invoke(
                StartPublishMethodName,
                nonPublicInstance,
                new object[]
                {
                    document,
                    spSite.Instance,
                    spWeb.Instance,
                    spList.Instance,
                    spList.Instance,
                    DummyInt.ToString(),
                    plannerProps,
                    DummyString
                });
            var totalCount = (float)privateObject.GetFieldOrProperty(TotalCountFieldName, nonPublicInstance);

            // Assert
            validations.ShouldSatisfyAllConditions(
                () => totalCount.ShouldBe(5),
                () => validations.ShouldBe(4));
        }

        [TestMethod]
        public void Execute_UserIdzero_Enqueues()
        {
            // Arrange
            const string key = "msproject";
            var id = $"1.{guid}.{DummyInt}";
            var data = $@"<xmlcfg Key=""{key}"" ID=""{id}""/>";
            var fieldMappings = $"{DummyString},{DummyString}";
            var expectedErrorMessage = $"Error Publishing: {DummyString}";
            var plannerProps = new PlannerProps()
            {
                sListTaskCenter = string.Empty,
                sProjectField = string.Empty,
                sFieldMappings = fieldMappings
            };
           
            ShimWorkPlannerAPI.getSettingsSPWebString = (_, __) =>
            {
                validations += 1;
                return plannerProps;
            };
            ShimPublishJob.AllInstances.setupProjectCenterSPList = (_, __) =>
            {
                validations += 1;
            };
            ShimPublishJob.AllInstances.setupTaskCenterSPList = (_, __) =>
            {
                validations += 1;
            };
            ShimPublishJob.AllInstances.StartPublishXmlDocumentSPSiteSPWebSPListSPListStringWorkPlannerAPIPlannerPropsString =
                (_, _1, _2, _3, _4, _5, _6, _7, _8) =>
                {
                    validations += 1;
                };
            ShimTimer.AddTimerJobGuidGuidGuidInt32StringInt32StringStringInt32Int32String =
                (_1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11) =>
                {
                    validations += 1;
                    return guid;
                };
            ShimTimer.EnqueueGuidInt32SPSite =
                (_1, _2, _3) =>
                {
                    validations += 1;
                    throw new Exception(DummyString);
                };
            ShimAPIEmail.QueueItemMessageInt32BooleanHashtableStringArrayStringArrayBooleanBooleanSPWebSPUserBoolean =
                (_1, _2, _3, _4, _5, _6, _7, _8, _9, _10) =>
                {
                    validations += 1;
                };
            ShimPublishJob.AllInstances.FormatPFEWorkJobXmlXmlDocument = (_, __) =>
            {
                validations += 1;
                return DummyString;
            };

            privateObject.SetFieldOrProperty(UserIdFieldName, publicInstance, 0);
            privateObject.SetFieldOrProperty(KeyFieldName, publicInstance, key);

            // Act
            privateObject.Invoke(
                ExecuteMethodName,
                publicInstance,
                new object[]
                {
                    spSite.Instance,
                    spWeb.Instance,
                    data
                });
            var isError = (bool)privateObject.GetFieldOrProperty(ErrorBooleanFieldName, publicInstance);
            var errorMessage = (string)privateObject.GetFieldOrProperty(ErrorStringFieldName, publicInstance);

            // Assert
            validations.ShouldSatisfyAllConditions(
                () => isError.ShouldBeTrue(),
                () => errorMessage.ShouldBe(expectedErrorMessage),
                () => validations.ShouldBe(8));
        }

        [TestMethod]
        public void Execute_UserIdNotzero_Enqueues()
        {
            // Arrange
            const string key = "msproject";
            var id = $"1.{guid}.{DummyInt}";
            var data = $@"<xmlcfg Key=""{key}"" ID=""{id}""/>";
            var fieldMappings = $"{DummyString},{DummyString}";
            var expectedErrorMessage = $"Error Publishing: {DummyString}";
            var plannerProps = new PlannerProps()
            {
                sListTaskCenter = string.Empty,
                sProjectField = string.Empty,
                sFieldMappings = fieldMappings
            };

            ShimWorkPlannerAPI.getSettingsSPWebString = (_, __) =>
            {
                validations += 1;
                return plannerProps;
            };
            ShimPublishJob.AllInstances.setupProjectCenterSPList = (_, __) =>
            {
                validations += 1;
            };
            ShimPublishJob.AllInstances.setupTaskCenterSPList = (_, __) =>
            {
                validations += 1;
            };
            ShimPublishJob.AllInstances.StartPublishXmlDocumentSPSiteSPWebSPListSPListStringWorkPlannerAPIPlannerPropsString =
                (_, _1, _2, _3, _4, _5, _6, _7, _8) =>
                {
                    validations += 1;
                };
            ShimTimer.AddTimerJobGuidGuidGuidInt32StringInt32StringStringInt32Int32String =
                (_1, _2, _3, _4, _5, _6, _7, _8, _9, _10, _11) =>
                {
                    validations += 1;
                    return guid;
                };
            ShimTimer.EnqueueGuidInt32SPSite =
                (_1, _2, _3) =>
                {
                    validations += 1;
                    throw new Exception(DummyString);
                };
            ShimAPIEmail.QueueItemMessageInt32BooleanHashtableStringArrayStringArrayBooleanBooleanSPWebSPUserBoolean =
                (_1, _2, _3, _4, _5, _6, _7, _8, _9, _10) =>
                {
                    validations += 1;
                };
            ShimPublishJob.AllInstances.FormatPFEWorkJobXmlXmlDocument = (_, __) =>
            {
                validations += 1;
                return DummyString;
            };

            privateObject.SetFieldOrProperty(UserIdFieldName, publicInstance, One);
            privateObject.SetFieldOrProperty(KeyFieldName, publicInstance, key);

            // Act
            privateObject.Invoke(
                ExecuteMethodName,
                publicInstance,
                new object[]
                {
                    spSite.Instance,
                    spWeb.Instance,
                    data
                });
            var isError = (bool)privateObject.GetFieldOrProperty(ErrorBooleanFieldName, publicInstance);
            var errorMessage = (string)privateObject.GetFieldOrProperty(ErrorStringFieldName, publicInstance);

            // Assert
            validations.ShouldSatisfyAllConditions(
                () => isError.ShouldBeTrue(),
                () => errorMessage.ShouldBe(expectedErrorMessage),
                () => validations.ShouldBe(8));
        }

        [TestMethod]
        public void FormatPFEWorkJobXml_WhenCalled_ReturnsXmlString()
        {
            // Arrange
            const string xmlString = @"
                <Project>
                    <UpdateScheduledWork>
                        <node/>
                        <node/>
                        <node/>
                        <node/>
                        <node/>
                    </UpdateScheduledWork>
                </Project>";
            var data = new XmlDocument();

            data.LoadXml(xmlString);

            // Act
            var actual = XDocument.Parse((string)privateObject.Invoke(
                FormatPFEWorkJobXmlMethodName,
                nonPublicInstance,
                new object[]
                {
                    data
                }));

            // Assert
            actual.ShouldSatisfyAllConditions(
                () => actual
                    .Element(UpdateScheduledWorkNodeName)
                    .Elements("node")
                    .Count()
                    .ShouldBe(5));
        }

        [TestMethod]
        public void ProcessTask_WhenCalled_UpdatesListItem()
        {
            // Arrange
            const string linkedTask = "LinkedTask";
            const string descendants = "Descendants";
            const string predecessors = "Predecessors";
            const string externalLink = "ExternalLink";
            const string isExternal = "IsExternal";
            const string dueDate = "DueDate";
            var xmlString = $@"
                <xmlcfg>
                    <Task UID=""{DummyInt}"" ID=""{DummyInt}"">
                        <Field Name=""{isExternal}"">{One}</Field>
                        <Field Name=""{externalLink}"">{SampleUrl}</Field>
                        <Field Name=""{predecessors}"">{DummyInt}</Field>
                        <Field Name=""{descendants}"">{DummyInt}</Field>
                        <Field Name=""{linkedTask}""></Field>
                        <Field Name=""{DummyString}""></Field>
                        <Field Name=""{DummyString}"">{DummyString}</Field>
                    </Task>
                </xmlcfg>";
            var data = new XmlDocument();
            var taskNode = default(XmlNode);
            var taskFields = new Hashtable()
            {
                [linkedTask] = linkedTask,
                [descendants] = descendants,
                [predecessors] = predecessors,
            };

            data.LoadXml(xmlString);
            taskNode = data.FirstChild.SelectSingleNode("//Task");
            spFieldCollection.GetFieldByInternalNameString = input =>
            {
                spField.TypeGet = () => SPFieldType.Computed;
                spField.InternalNameGet = () => DummyString;

                if (input.Equals(descendants) || input.Equals(isExternal))
                {
                    spField.TypeGet = () => SPFieldType.User;
                }
                if (input.Equals(externalLink))
                {
                    spField.InternalNameGet = () => dueDate;
                }

                return spField;
            };
            spListItem.ItemGetString = _ => 0;
            spListItem.ItemSetGuidObject = (_, __) =>
            {
                validations += 1;
            };
            spListItem.Update = () =>
            {
                validations += 1;
            };

            ShimCoreFunctions.getUserStringStringSPWebString = (_1, _2, _3) =>
            {
                validations += 1;
                return DummyString;
            };

            // Act
            var actual = privateObject.Invoke(
                ProcessTaskMethodName,
                nonPublicInstance,
                new object[]
                {
                    taskNode,
                    spListItem.Instance,
                    taskFields,
                    spWeb.Instance,
                    DummyString,
                    DummyString
                });

            // Assert
            validations.ShouldBe(10);
        }

        [TestMethod]
        public void PublishTasks_WhenCalled_PublishesTasks()
        {
            // Arrange
            const string randomDestination = "1.2.3.4.5";
            const string randomSource = "1.2.3";
            var xmlString = $@"
                <xmlcfg>
                    <Task UID=""0"" SPID=""0"" SPUID=""0"" Folder=""{DummyString}"" ID=""0"" Title=""{DummyString}"" Iteration=""{One}""/>
                    <Task UID=""{One}"" SPID=""{One}"" SPUID=""{One}"" Folder=""{DummyString}"" ID=""{One}"" Title=""{DummyString}"" Iteration=""{One}"">
                        <Field Name=""CT{DummyString}"">0</Field>
                        <Title>{One}</Title>
                    </Task>
                    <Task UID=""{Two}"" SPID=""{Two}"" Folder=""{DummyString}"" ID=""{Two}"" Title=""{DummyString}"" Iteration=""{One}"">
                        <Field Name=""CT{DummyString}"">0</Field>
                        <Title>{Two}</Title>
                    </Task>
                    <Task UID=""{Three}"" SPID=""{Three}"" Folder="""" ID=""{Three}"" Title=""{DummyString}"" Iteration=""{One}"">
                        <Field Name=""CT{DummyString}"">0</Field>
                        <Title>{Three}</Title>
                    </Task>
                    <Task UID=""{Five}"" SPID=""{Five}"" Folder="""" ID=""{Five}"" Title=""{DummyString}"" Iteration=""{One}"">
                        <Field Name=""CT{DummyString}"">0</Field>
                        <Title>{Five}</Title>
                    </Task>
                </xmlcfg>";
            var document = new XmlDocument();
            var dataTable = new DataTable();
            var row = default(DataRow);
            var plannerProps = new PlannerProps()
            {
                sIterationCT = DummyString,
                bAgile = true
            };
            var hashLinks = new Hashtable()
            {
                [randomSource] = randomDestination,
                [randomDestination] = randomDestination,
            };

            document.LoadXml(xmlString);
            dataTable.Columns.Add(TaskUidColumnName);
            dataTable.Columns.Add(IDStringCaps);
            dataTable.Columns.Add(IDStringCaps.ToLower());
            dataTable.Columns.Add(IsPublishedColumnName);

            row = dataTable.NewRow();
            row[TaskUidColumnName] = Four;
            row[IDStringCaps] = One;
            row[IDStringCaps.ToLower()] = One;
            row[IsPublishedColumnName] = One;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[TaskUidColumnName] = Four;
            row[IDStringCaps] = Four;
            row[IDStringCaps.ToLower()] = Four;
            row[IsPublishedColumnName] = One;
            dataTable.Rows.Add(row);

            row = dataTable.NewRow();
            row[TaskUidColumnName] = Five;
            row[IDStringCaps] = Five;
            row[IDStringCaps.ToLower()] = Five;
            row[IsPublishedColumnName] = 0;
            dataTable.Rows.Add(row);

            spWeb.GetSiteDataSPSiteDataQuery = _ => dataTable;
            spContentType.NameGet = () => DummyString;
            spList.AddItem = () =>
            {
                validations += 1;
                return spListItem;
            };
            spList.AddItemStringSPFileSystemObjectTypeString = (_1, _2, _3) =>
            {
                validations += 1;
                return spListItem;
            };
            spListItem.ItemSetStringObject = (_, __) =>
            {
                validations += 1;
            };
            spListItem.ItemSetGuidObject = (_, __) =>
            {
                validations += 1;
            };
            spListItem.Recycle = () =>
            {
                validations += 1;
                return guid;
            };
            spFieldCollection.ContainsFieldString = _ => true;

            ShimPath.GetDirectoryNameString = input => input;
            ShimSPUrlUtility.CombineUrlStringString = (_, __) => SampleUrl;
            ShimSPUtility.GetUrlDirectoryString = _ => SampleUrl;
            ShimSPBaseCollection.AllInstances.GetEnumerator = _ => new List<SPContentType>()
            {
                spContentType
            }.GetEnumerator();
            ShimPublishJob.AllInstances.ensureFolderSPListString = (_, _1, _2) =>
            {
                validations += 1;
            };
            ShimPublishJob.MoveListItemToFolderSPListItemSPFolder = (_1, _2) =>
            {
                validations += 1;
            };
            ShimPublishJob.AllInstances.processTaskXmlNodeSPListItemHashtableSPWebStringString = (_, _1, _2, _3, _4, _5, _6) =>
            {
                validations += 1;
            };
            ShimBaseJob.AllInstances.updateProgressSingle = (_, __) =>
            {
                validations += 1;
            };
            ShimSqlCommand.AllInstances.ExecuteNonQuery = _ =>
            {
                validations += 1;
                return DummyInt;
            };

            privateObject.SetFieldOrProperty(HashLinksFieldName, nonPublicInstance, hashLinks);

            // Act
            privateObject.Invoke(
                PublishTasksMethodName,
                nonPublicInstance,
                new object[]
                {
                    document,
                    spList.Instance,
                    DummyInt.ToString(),
                    default(Hashtable),
                    DummyString,
                    DummyString,
                    plannerProps,
                    DummyInt.ToString()
                });

            // Assert
            validations.ShouldBe(37);
        }
    }
}