﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using EPMLiveCore.API;
using EPMLiveCore.Infrastructure;
using EPMLiveCore.Infrastructure.Navigation;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace EPMLiveCore.CONTROLTEMPLATES
{
    [MdsCompliant(true)]
    public partial class EPMLiveNavigation : UserControl
    {
        #region Fields (2) 

        private const string LAYOUT_PATH = "/_layouts/15/epmlive/";
        private string _selectedTlNode;

        #endregion Fields 

        #region Properties (5) 

        public IEnumerable<NavNode> AllNodes
        {
            get
            {
                List<NavNode> nodes = TopNodes.ToList();
                nodes.AddRange(BottomNodes);

                return nodes;
            }
        }

        public IEnumerable<NavNode> BottomNodes { get; set; }

        public bool Pinned { get; private set; }

        public string SelectedTlNode
        {
            get { return _selectedTlNode ?? "epm-nav-top-ql"; }
        }

        public IEnumerable<NavNode> TopNodes { get; set; }

        #endregion Properties 

        #region Methods (3) 

        // Protected Methods (3) 

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (string style in new[] {"controls/navigation/glyphs.min", "controls/navigation/epmnav.min"})
            {
                SPPageContentManager.RegisterStyleFile(LAYOUT_PATH + "stylesheets/" + style + ".css");
            }

            EPMLiveScriptManager.RegisterScript(Page, new[]
            {
                "libraries/jquery.min", "@libraries/jquery.cookie", "libraries/slimScroll", "@EPMLive.Navigation"
            });

            HttpCookie selectedTlNodeCookie = Request.Cookies.Get("epmnav-selected-tlnode");
            if (selectedTlNodeCookie != null)
            {
                _selectedTlNode = selectedTlNodeCookie.Value;
            }

            Pinned = true;

            HttpCookie pinStateCookie = Request.Cookies.Get("epmnav-pin-state");
            if (pinStateCookie != null)
            {
                Pinned = pinStateCookie.Value.Equals("pinned");
            }

            LoadSelectedNodeLinks();
        }

        private void LoadSelectedNodeLinks()
        {
            if (SelectedTlNode.Equals("epm-nav-top-ql")) return;

            var nodeId = SelectedTlNode.Replace("epm-nav-top-", string.Empty);
            string provider = (from topNode in TopNodes
                where topNode.Id.Equals(nodeId)
                select topNode.LinkProvider).FirstOrDefault();

            var navService = new NavigationService(provider, SPContext.Current.Web);

            navService.GetHTMLForProvider(provider);
        }

        protected void OnTreeViewPreRender(object sender, EventArgs e)
        {
            ((SPTreeView) sender).Sort();
        }

        protected void Page_Load(object sender, EventArgs e) { }

        #endregion Methods 
    }
}