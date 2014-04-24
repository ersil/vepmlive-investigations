﻿using System.Linq;
using EPMLiveCore.SocialEngine.Contracts;
using EPMLiveCore.SocialEngine.Core;
using EPMLiveCore.SocialEngine.Events;

namespace EPMLiveCore.SocialEngine.Modules
{
    internal class GlobalValidator : ISocialEngineModule
    {
        #region Methods (1) 

        // Private Methods (1) 

        private void OnValidateActivity(ProcessActivityEventArgs args)
        {
            var validObjectKinds = new ObjectKind[] {};

            if (validObjectKinds.Contains(args.ObjectKind) ||
                args.ContextWeb.CurrentUser.ID != args.ContextWeb.Site.SystemAccount.ID) return;

            args.Cancel = true;
            args.CancellationMessage = "Ignoring activities performed by the System Account";
        }

        #endregion Methods 

        #region Implementation of ISocialEngineModule

        public void Initialize(SocialEngineEvents events)
        {
            events.OnValidateActivity += OnValidateActivity;
        }

        #endregion
    }
}