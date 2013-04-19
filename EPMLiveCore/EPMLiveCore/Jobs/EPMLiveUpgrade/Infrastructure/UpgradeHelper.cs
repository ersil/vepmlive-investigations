﻿using System;

namespace EPMLiveCore.Jobs.EPMLiveUpgrade.Infrastructure
{
    internal enum EPMLiveVersion
    {
        V44,
        GENERIC
    }

    internal enum MessageKind
    {
        SUCCESS,
        FAILURE,
        SKIPPED
    }

    internal class UpgradeStepAttribute : Attribute
    {
        #region Properties (5) 

        public string Description { get; set; }

        public string Name
        {
            get
            {
                string version = string.Empty;

                switch (Version)
                {
                    case EPMLiveVersion.V44:
                        version = "4.4.0";
                        break;
                    case EPMLiveVersion.GENERIC:
                        version = "GENERIC";
                        break;
                }

                return version + "." + Order;
            }
        }

        public double Order { get; set; }

        public double SequenceOrder
        {
            get { return Version == EPMLiveVersion.GENERIC ? Order : Convert.ToDouble(Name.Replace(".", string.Empty)); }
        }

        public EPMLiveVersion Version { get; set; }

        #endregion Properties 
    }
}