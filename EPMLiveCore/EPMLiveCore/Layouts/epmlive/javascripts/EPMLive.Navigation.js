﻿(function () {
    var loadNav = function() {
        $(function() {
            var $ = window.jQuery;

            var epmLiveService = (function() {
                var _execute = function (method, data, onSuccess, onError) {
                    var $$ = window.epmLive;
                    
                    $.ajax({
                        type: 'POST',
                        url: $$.currentWebUrl + '/_vti_bin/WorkEngine.asmx/Execute',
                        data: "{ Function: '" + method + "', Dataxml: '" + data + "' }",
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json',
                        success: function(response) {
                            if (response.d) {
                                var resp = $$.parseJson(response.d);
                                var result = resp.Result;
                                
                                if ($$.responseIsSuccess(result)) {
                                    onSuccess(result);
                                } else {
                                    onError(response);
                                }
                            } else {
                                onError(response);
                            }
                        },
                        error: function(response) {
                            onError(response);
                        }
                    });
                };

                return {
                    execute: _execute
                };
            })();

            var epmLiveNavigation = (function() {
                var animSpeed = 300;
                var nodeClass = 'epm-nav-node';
                var hoverClass = 'epm-nav-node-hover';
                var openedClass = 'epm-nav-node-opened';
                var selectedClass = 'epm-nav-node-selected';

                var selectedTlNodeCookie = 'epmnav-selected-tlnode';
                var selectedLinkCookie = 'epmnav-selected-link';
                var expandStateCookie = 'epmnav-expand-state';
                var pinStateCookie = 'epmnav-pin-state';
                var cookieOptions = { expires: 365, path: '/' };
                
                var $sn = $('#epm-nav-sub');

                var tlNodes = [];

                var navNode = function(el) {
                    var categories = {};

                    var _el = el;
                    var _$el = $(el);
                    var _$sm = $('#epm-nav-sub-' + _$el.data('id'));

                    var _id = _el.id;
                    var _provider = _$el.data('linkprovider');

                    var _selected = function() {
                        return _$el.parent().hasClass(selectedClass);
                    };

                    var isOpened = function() {
                        return _$el.parent().hasClass(openedClass);
                    };

                    var _select = function(select) {
                        if (select) {
                            _$el.parent().addClass(selectedClass);
                        } else {
                            _$el.parent().removeClass(selectedClass);
                        }
                    };

                    var _close = function() {
                        _$sm.hide();
                        _$el.parent().removeClass(openedClass);
                    };

                    var _closeNav = function() {
                        if ($sn.is(':visible')) {
                            $sn.hide('slide', { direction: 'left' }, animSpeed);
                        }

                        _close();
                    };

                    var _registerLink = function(link) {
                        if (link.seprator) {
                            registerSeprator(link.category);
                        } else {
                            registerCategory(link.category);
                            registerLink(link);
                        }
                    };

                    var registerLink = function(link) {
                        var category = link.category;

                        if (!category) {
                            category = '__STATIC__';
                        }

                        var li = '<li id="' + link.id + '" class="epm-nav-node" style="display:none;"><a id="epm-nav-link-' + link.id + '" href="' + link.url + '" alt="' + link.title + '">' + link.title + '</a></li>';

                        categories[category].$el.append(li);

                        if (link.external) {
                            $('#epm-nav-link-' + link.id).attr('target', '_blank');
                        }

                        if (link.visible) {
                            $('#' + link.id).show();
                        }
                    };

                    var registerSeprator = function(category) {
                        var seprator = '<div class="epm-nav-sub-sep"></div>';

                        if (category) {
                            $('#' + calculateCatId(category)).append(seprator);
                        } else {
                            _$sm.append(seprator);
                        }
                    };

                    var registerCategory = function(category) {
                        if (!category) {
                            category = '__STATIC__';
                        }

                        if (!categories[category]) {
                            var id = calculateCatId(category);
                            var defaultCssClass = 'epm-nav-node-collapsed';

                            if (category === '__STATIC__') {
                                defaultCssClass = 'epm-nav-node-static';
                            }

                            if (category !== '__STATIC__') {
                                _$sm.append('<div id="' + id + '-header" class="epm-nav-node epm-nav-node-root epm-nav-cat"><span class="' + defaultCssClass + '">&nbsp;</span><span class="epm-nav-cat-title" alt="' + category + '">' + category + '</span></div>');
                            }

                            _$sm.append('<ul id="' + id + '" class="epm-nav-links ' + defaultCssClass + '"></ul>');

                            categories[category] = {
                                id: id,
                                $el: $('#' + id)
                            };
                        }
                    };

                    var calculateCatId = function(category) {
                        return _$sm.get(0).id + '-' + category.toLowerCase().replace(/ /g, '-').replace(/_/g, '').replace(/[^\w-]+/g, '').replace(/--/g, '-');
                    };

                    var showNode = function() {
                        _$sm.show();
                        _$el.parent().addClass(openedClass);
                    };

                    var openMenu = function() {
                        showNode();

                        if (!$sn.is(':visible')) {
                            $sn.show('slide', { direction: 'left' }, animSpeed);
                        }
                    };

                    _$el.click(function() {
                        if (_selected()) {
                            if (isOpened() && $.cookie(pinStateCookie) === 'unpinned') {
                                _closeNav();
                            } else {
                                openMenu();
                            }
                        } else {
                            for (var n in tlNodes) {
                                var nde = tlNodes[n];

                                if (nde.id !== _id) {
                                    if (nde.selected()) {
                                        nde.select(false);
                                        nde.close();
                                    }
                                }
                            }

                            _select(true);
                            openMenu();

                            $.cookie(selectedTlNodeCookie, _id, cookieOptions);
                        }
                    });

                    return {
                        id: _id,
                        provider: _provider,
                        selected: _selected,
                        select: _select,
                        close: _close,
                        closeNav: _closeNav,
                        registerLink: _registerLink,
                        el: _el,
                        $el: _$el,
                        $menu: _$sm
                    };
                };

                var hideMenu = function () {
                    for (var n = 0; n < tlNodes.length; n++) {
                        var node = tlNodes[n];
                        if (node.selected()) {
                            node.closeNav();
                            break;
                        }
                    }
                };

                var changePinState = function (state) {
                    var elements = ['s4-ribbonrow', 's4-workspace'];
                    for (var e in elements) {
                        var $e = $('#' + elements[e]);
                        $e.removeClass('epm-nav-pinned');
                        $e.removeClass('epm-nav-unpinned');
                        $e.addClass('epm-nav-' + state);
                    }

                    var $pin = $('#epm-nav-pin');
                    $pin.removeClass('epm-nav-pin-pinned');
                    $pin.removeClass('epm-nav-pin-unpinned');
                    $pin.addClass('epm-nav-pin-' + state);

                    if (state === 'unpinned') {
                        hideMenu();
                    }

                    $sn.data('pinstate', state);
                    $.cookie(pinStateCookie, state, cookieOptions);
                };

                var togglePinned = function () {
                    if ($sn.data('pinstate') === 'pinned') {
                        changePinState('unpinned');
                    } else {
                        changePinState('pinned');
                    }
                };

                var saveExpandState = function (nodeId, data) {
                    var state = $.parseJSON($.cookie(expandStateCookie)) || {};
                    state[nodeId] = data;
                    $.cookie(expandStateCookie, JSON.stringify(state), cookieOptions);
                };

                var registerEvents = function () {
                    var hoverNode = window.TreeView_HoverNode;
                    var unhoverNode = window.TreeView_UnhoverNode;
                    var toggleNode = window.TreeView_ToggleNode;

                    window.TreeView_HoverNode = function (data, el) {
                        var node = $(el);

                        if (node.hasClass(nodeClass)) {
                            node.parent().parent().parent().addClass(hoverClass);
                        } else {
                            hoverNode(data, el);
                        }
                    };

                    window.TreeView_UnhoverNode = function (el) {
                        var node = $(el);

                        if (node.hasClass(nodeClass)) {
                            $(el).parent().parent().parent().removeClass(hoverClass);
                        } else {
                            unhoverNode(el);
                        }
                    };

                    window.TreeView_ToggleNode = function (data, x, elNav, y, elNodes) {
                        var d = [];
                        var nodeId = null;

                        var parents = $(elNav).parents('div');
                        for (var i = 0; i < parents.length; i++) {
                            var p = parents[i];
                            var $p = $(p);

                            if ($p.hasClass('epm-nav-sub')) {
                                nodeId = p.id;
                                var nodes = getLinkNodes(nodeId);
                                for (var j = 0; j < nodes.length; j++) {
                                    var n = nodes[j];
                                    var state = $(n).is(':visible');

                                    if ((n.id).replace('Nodes', '') === elNav.id) {
                                        state = !state;
                                    }

                                    d[j] = state;
                                }

                                break;
                            }
                        }

                        saveExpandState(nodeId, d);
                        toggleNode(data, x, elNav, y, elNodes);
                    };

                    var clickables = ['suiteBar', 's4-ribbonrow', 's4-workspace'];
                    for (var c in clickables) {
                        $('#' + clickables[c]).click(function () {
                            if ($sn.data('pinstate') === 'unpinned') {
                                hideMenu();
                            }
                        });
                    }

                    var $pin = $('#epm-nav-pin');

                    $pin.click(function () {
                        togglePinned();
                    });

                    $sn.hover(function () {
                        $pin.show();
                    }, function () {
                        $pin.hide();
                    });
                    
                    $sn.slimScroll({
                        height: $sn.height(),
                        width: $sn.width()
                    });

                    var $ss = $('#epm-nav').find('.slimScrollDiv');
                    var $sb = $ss.find('.slimScrollBar');

                    $ss.css('position', 'absolute');
                    $ss.css('left', '50px');
                    $sn.css('left', '0');
                    $sb.css('z-index', 1001);

                    $('a.epm-nav-node').click(function () {
                        var index = -1;

                        var $link = $(this);
                        var parent = $link.parents('div')[0];
                        var $siblings = $(parent).siblings('div');

                        if ($siblings) {
                            var $nodes = getLinkNodes($($siblings[0]).parent().parent().attr('id'));

                            for (var i = 0; i < $nodes.length; i++) {
                                var s = $nodes[i];
                                if (s.id === parent.id) {
                                    index = i;
                                }
                            }
                        }

                        $.cookie(selectedLinkCookie, JSON.stringify({ index: index, uri: $link.attr('href') }), cookieOptions);
                    });
                };

                var getSelectedSubLevelNode = function () {
                    return ($.cookie(selectedTlNodeCookie) || 'epm-nav-top-ql').replace('epm-nav-top-', 'epm-nav-sub-');
                };

                var getLinkNodes = function (menu) {
                    return $('#' + menu).find('.epm-nav-sub-menu').find('div[id^=E]');
                };

                var expandNodes = function () {
                    var expandState = $.parseJSON($.cookie(expandStateCookie));

                    if (expandState) {
                        var selectedNode = getSelectedSubLevelNode();

                        var snExpandStatus = expandState[selectedNode];
                        if (snExpandStatus) {
                            if (selectedNode === 'epm-nav-sub-ql') {
                                var nodes = getLinkNodes('epm-nav-sub-ql');
                                for (var i = 0; i < nodes.length; i++) {
                                    if (snExpandStatus[i]) {
                                        var nodesId = nodes[i].id;
                                        var nodeId = (nodesId).replace('Nodes', '');

                                        TreeView_ToggleNode(window.EPMLiveNav_Data, 0, document.getElementById(nodeId), ' ', document.getElementById(nodesId));
                                    }
                                }
                            }
                        }
                    }
                };

                var selectLink = function () {
                    var link = $.parseJSON($.cookie(selectedLinkCookie));
                    if (link) {
                        var index = link.index;
                        var uri = link.uri;
                        if (window.location.href.indexOf(uri) !== -1) {
                            var $menu = $('#' + getSelectedSubLevelNode());

                            if (!index || index === -1) {
                                $menu.find('a[href="' + uri + '"]:first').parents('table').addClass(selectedClass);
                            } else {
                                var $nodes = getLinkNodes($menu.parent().attr('id'));
                                for (var i = 0; i < $nodes.length; i++) {
                                    if (i === index) {
                                        $($nodes[i]).find('a[href="' + uri + '"]:first').parents('table').addClass(selectedClass);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                };

                var loadLinks = function () {
                    var providers = [];

                    for (var i = 0; i < tlNodes.length; i++) {
                        var node = tlNodes[i];

                        if (!node.selected()) {
                            var provider = node.provider;
                            if (provider) {
                                providers.push(provider);
                            }
                        }
                    }

                    if (providers.length > 0) {
                        epmLiveService.execute('GetNavigationLinks', providers.join(), function (response) {
                            var webUrl = epmLive.currentWebUrl;
                            var page = webUrl + window.location.href.split(webUrl)[1];
                            
                            for (var providerName in response.Nodes) {
                                var navLink = response.Nodes[providerName].NavLink;
                                for (var nl in navLink) {
                                    var link = navLink[nl];

                                    for (var j = 0; j < tlNodes.length; j++) {
                                        var tlNode = tlNodes[j];
                                        if (tlNode.provider === providerName) {
                                            tlNode.registerLink({
                                                id: link['@Id'],
                                                title: link['@Title'],
                                                url: link['#cdata'].replace(/{page}/g, page),
                                                category: link['@Category'],
                                                cssClass: link['@CssClass'],
                                                order: parseInt(link['@Order']),
                                                external: link['@Exernal'] === 'True',
                                                visible: link['@Visible'] === 'True',
                                                seprator: link['@Separator'] === 'True'
                                            });
                                        }
                                    }
                                }
                            }
                        }, function (response) {
                            console.log(response);
                        });
                    }
                };

                var _init = function () {
                    var pinState = $.cookie(pinStateCookie);

                    $('#epm-nav-top').find("[data-role='top-nav-node']").each(function () {
                        tlNodes.push(new navNode(this));
                    });

                    changePinState(pinState || 'pinned');
                    expandNodes();
                    selectLink();
                    registerEvents();
                    
                    ExecuteOrDelayUntilScriptLoaded(loadLinks, 'EPMLive.js');
                };

                return {
                    init: _init
                };
            })();

            epmLiveNavigation.init();
        });
    };
    
    function onJqueryLoaded() {
        ExecuteOrDelayUntilScriptLoaded(loadNav, 'EPMLiveNavigation');
    }
    
    ExecuteOrDelayUntilScriptLoaded(onJqueryLoaded, 'jquery.min.js');
})();