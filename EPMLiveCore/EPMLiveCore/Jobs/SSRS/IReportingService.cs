﻿using System;

using Microsoft.SharePoint;

namespace EPMLiveCore.Jobs.SSRS
{
    public interface IReportingService
    {
        void CreateSiteCollectionMappedFolder(Guid siteCollectionId);
        void DeleteSiteCollectionMappedFolder(Guid siteCollectionId);
        void SyncReports(string siteCollectionId, SPDocumentLibrary reportLibrary, out string errors);
    }
}