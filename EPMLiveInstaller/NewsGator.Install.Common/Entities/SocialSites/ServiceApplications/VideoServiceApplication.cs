﻿using System;
using Microsoft.SharePoint.Administration;
using NGAssemblies = NewsGator.Install.Common.Constants.Assemblies;
using NGTypes = NewsGator.Install.Common.Constants.TypeNames;
using NewsGator.Install.Common.Entities.SocialSites.Services;

namespace NewsGator.Install.Common.Entities.SocialSites.ServiceApplications
{
    internal static class VideoServiceApplication
    {
        internal static Type ServiceApplicationType
        {
            get
            {
                return Utilities.Types.TryGetType(NGTypes.VideoStream.VideoServiceApplicationTypeFullName, NGAssemblies.VideoStream.NewsGatorVideoStreamServiceLibrary);
            }
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static SPIisWebServiceApplication FindInstance()
        {
            return Utilities.ServiceApplication.FindServiceInstanceWhereOrDefault(x => x.GetType() == ServiceApplicationType);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static SPIisWebServiceApplication FindInstanceByTypeName()
        {
            return Utilities.ServiceApplication.FindServiceInstanceWhereOrDefault(x => x.GetType().FullName == NGTypes.VideoStream.VideoServiceApplicationTypeFullName);
        }

        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.LinkDemand, Name = "FullTrust")]
        internal static SPPersistedObject Create(string name, SPPersistedObject service, SPIisWebServiceApplicationPool pool, SPDatabaseParameters dbParameters)
        {
            return (SPPersistedObject)Utilities.Reflection.ExecuteMethod(ServiceApplicationType,
                ServiceApplicationType,
                "Create",
                new[] {
                    typeof(string),
                    VideoService.ServiceType,
                    typeof(SPIisWebServiceApplicationPool),
                    typeof(SPDatabaseParameters)
                },
                new object[]
                {
                    name,
                    service, 
                    pool,
                    dbParameters
                });
        }
    }
}
