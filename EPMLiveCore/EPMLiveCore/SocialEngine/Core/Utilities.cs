﻿using System.Linq;
using EPMLiveCore.Infrastructure;
using Microsoft.SharePoint;

namespace EPMLiveCore.SocialEngine.Core
{
    internal static class Utilities
    {
        #region Fields (1) 

        private const string IGNORED_LISTS_SETTING_KEY = "EPM_SS_Ignored_Lists";

        #endregion Fields 

        #region Methods (2) 

        // Public Methods (2) 

        public static string ConfigureDefaultIgnoredLists(SPWeb contextWeb)
        {
            string ignoredLists = CoreFunctions.getConfigSetting(contextWeb, IGNORED_LISTS_SETTING_KEY);
            if (!string.IsNullOrEmpty(ignoredLists)) return ignoredLists;

            const string IGNORED_LISTS =
                "EPMLiveFileStore,User Information List,Team,Department,Departments,Excel Reports,Holiday Schedules,My Timesheet,My Work,Non Work,Project Schedules,Report Library,Resource Center,Roles,Site Assets,Site Pages,Style Library,Work Hours";

            CoreFunctions.setConfigSetting(contextWeb, IGNORED_LISTS_SETTING_KEY, IGNORED_LISTS);

            return IGNORED_LISTS;
        }

        public static bool IsIgnoredList(string listTitle, SPWeb contextWeb)
        {
            var settingValue =
                (string)
                    CacheStore.Current.Get(IGNORED_LISTS_SETTING_KEY, new CacheStoreCategory(contextWeb).SocialStream,
                        () => CoreFunctions.getConfigSetting(contextWeb, IGNORED_LISTS_SETTING_KEY), true).Value;

            if (!string.IsNullOrEmpty(settingValue))
                return settingValue.Split(',').Any(list => list.Trim().ToLower().Equals(listTitle.ToLower()));

            string ignoredLists = ConfigureDefaultIgnoredLists(contextWeb);

            CacheStore.Current.Set(IGNORED_LISTS_SETTING_KEY, ignoredLists,
                new CacheStoreCategory(contextWeb).SocialStream, true);

            return false;
        }

        #endregion Methods 
    }
}