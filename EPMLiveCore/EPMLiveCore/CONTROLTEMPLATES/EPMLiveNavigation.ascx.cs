﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using EPMLiveCore.Infrastructure;
using EPMLiveCore.Infrastructure.Navigation;
using Microsoft.SharePoint.WebControls;

namespace EPMLiveCore.CONTROLTEMPLATES
{
    [MdsCompliant(true)]
    public partial class EPMLiveNavigation : UserControl
    {
        private const string LAYOUT_PATH = "/_layouts/15/epmlive/";
        private string _selectedTlNode;

        public IEnumerable<NavNode> TopNodes { get; set; }
        public IEnumerable<NavNode> BottomNodes { get; set; }

        public IEnumerable<NavNode> AllNodes
        {
            get
            {
                List<NavNode> nodes = TopNodes.ToList();
                nodes.AddRange(BottomNodes);

                return nodes;
            }
        }

        public string SelectedTlNode
        {
            get { return _selectedTlNode ?? "epm-nav-top-ql"; }
        }

        public bool Pinned { get; private set; }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            foreach (string style in new[] {"controls/navigation/glyphs.min", "controls/navigation/epmnav.min"})
            {
                SPPageContentManager.RegisterStyleFile(LAYOUT_PATH + "stylesheets/" + style + ".css");
            }

            EPMLiveScriptManager.RegisterScript(Page,
                new[] {"libraries/jquery.min", "@libraries/jquery.cookie", "@EPMLive.Navigation"});

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
        }

        protected void Page_Load(object sender, EventArgs e) { }

        protected void OnTreeViewPreRender(object sender, EventArgs e)
        {
            ((SPTreeView) sender).Sort();
        }
    }
}