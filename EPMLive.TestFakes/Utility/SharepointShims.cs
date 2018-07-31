﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Fakes;

namespace EPMLive.TestFakes.Utility
{
    public class SharepointShims
    {
        public ShimSPWeb WebShim { get; private set; }
        public ShimSPSite SiteShim { get; private set; }
        public ShimSPList ListShim { get; private set; }
        public ShimSPListCollection ListsShim { get; private set; }
        public ShimSPUser UserShim { get; private set; }

        public string ServerRelativeUrl { get; private set; }
        public string WorkspaceUrl { get; private set; }
        public bool DbUpdateExecuted { get; private set; }

        private SharepointShims(
            string serverRelativeUrl = "test-server-url",
            string workspaceUrl = "test-workspace-url")
        {
            ServerRelativeUrl = serverRelativeUrl;
            WorkspaceUrl = workspaceUrl;

            UserShim = new ShimSPUser();
            SiteShim = InitializeSPSiteShim();
            ListShim = InitializeSPListShim();
            ListsShim = InitializeSPListCollectionShim();
            WebShim = InitializeSPWebShim();
        }

        public static SharepointShims ShimSharepointCalls()
        {
            var result = new SharepointShims();
            result.InitializeStaticShims();

            return result;
        }

        private void InitializeStaticShims()
        {
            ShimSPContext.CurrentGet = () => new ShimSPContext
            {
                WebGet = () => WebShim
            };
        }

        private ShimSPWeb InitializeSPWebShim()
        {
            return new ShimSPWeb
            {
                CurrentUserGet = () => UserShim,
                ServerRelativeUrlGet = () => ServerRelativeUrl,
                ListsGet = () => ListsShim,
                SiteGet = () => SiteShim
            };
        }

        private ShimSPListCollection InitializeSPListCollectionShim()
        {
            return new ShimSPListCollection
            {
                ItemGetString = (key) => ListShim
            };
        }

        private ShimSPList InitializeSPListShim()
        {
            return new ShimSPList
            {
                ItemsGet = () => new ShimSPListItemCollection
                {
                    Add = () => new ShimSPListItem
                    {
                        ItemSetStringObject = (liName, value) =>
                        {
                            if (liName == "URL")
                            {
                                WorkspaceUrl = value.ToString();
                            }
                        },
                        Update = () => DbUpdateExecuted = true
                    }
                },
                FormsGet = () => new ShimSPFormCollection
                {
                    ItemGetPAGETYPE = (pageType) => new ShimSPForm
                    {
                        ServerRelativeUrlGet = () => ServerRelativeUrl
                    }
                },
                FieldsGet = () => new ShimSPFieldCollection
                {
                    GetFieldByInternalNameString = (internalName) => new ShimSPField()
                },
                GetItemsSPQuery = (query) => new ShimSPListItemCollection()
                    .Bind(new ShimSPListItem[] { })
            };
        }

        private ShimSPSite InitializeSPSiteShim()
        {
            return new ShimSPSite();
        }
    }
}
