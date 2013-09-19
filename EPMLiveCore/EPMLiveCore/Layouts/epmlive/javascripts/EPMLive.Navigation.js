﻿(function () {
    'use strict';

    function initializeNavigation() {
        $(function () {
            var $ = window.jQuery;

            var $$ = (function () {
                var _currentWebUrl = function () {
                    return window.epmLiveNavigation.currentWebUrl;
                };

                var _parseJson = function (xml) {
                    return eval('(' + window.xml2json($.parseXML(xml), "") + ')');
                };

                var _responseIsSuccess = function (response) {
                    return response['@Status'] == 0;
                };

                return {
                    currentWebUrl: _currentWebUrl,
                    parseJson: _parseJson,
                    responseIsSuccess: _responseIsSuccess
                };
            })();

            var base64Service = (function () {
                var keyStr = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=';

                var _decode = function (input) {
                    var output = '';
                    var chr1, chr2, chr3 = '';
                    var enc1, enc2, enc3, enc4 = '';
                    var i = 0;

                    var base64Test = /[^A-Za-z0-9\+\/\=]/g;
                    if (base64Test.exec(input)) {
                        console.log('There were invalid base64 characters in the input text.\nValid base64 characters are A-Z, a-z, 0-9, "+", "/",and "="\nExpect errors in decoding.');
                    }

                    input = input.replace(/[^A-Za-z0-9\+\/\=]/g, "");

                    do {
                        enc1 = keyStr.indexOf(input.charAt(i++));
                        enc2 = keyStr.indexOf(input.charAt(i++));
                        enc3 = keyStr.indexOf(input.charAt(i++));
                        enc4 = keyStr.indexOf(input.charAt(i++));

                        chr1 = (enc1 << 2) | (enc2 >> 4);
                        chr2 = ((enc2 & 15) << 4) | (enc3 >> 2);
                        chr3 = ((enc3 & 3) << 6) | enc4;

                        output = output + String.fromCharCode(chr1);

                        if (enc3 != 64) {
                            output = output + String.fromCharCode(chr2);
                        }

                        if (enc4 != 64) {
                            output = output + String.fromCharCode(chr3);
                        }

                        chr1 = chr2 = chr3 = '';
                        enc1 = enc2 = enc3 = enc4 = '';

                    } while (i < input.length);

                    return unescape(output);
                };

                return {
                    decode: _decode
                };
            })();

            var epmLiveService = (function () {
                var _execute = function (method, data, onSuccess, onError) {
                    var webUrl = $$.currentWebUrl();

                    if (webUrl) {
                        $.ajax({
                            type: 'POST',
                            url: (webUrl + '/_vti_bin/WorkEngine.asmx/Execute').replace(/\/\//g, '/'),
                            data: "{ Function: '" + method + "', Dataxml: '" + data + "' }",
                            contentType: 'application/json; charset=utf-8',
                            dataType: 'json',
                            success: function (response) {
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
                            error: function (response) {
                                onError(response);
                            }
                        });
                    } else {
                        window.setTimeout(function () {
                            _execute(method, data, onSuccess, onError);
                        }, 1);
                    }
                };

                return {
                    execute: _execute
                };
            })();

            var jqCookie = $.cookie;
            $.cookie = function (cookieName, data, options) {
                cookieName = cookieName + '-u-' + window.epmLiveNavigation.currentUserId;

                if (jqCookie) {
                    if (!data) {
                        return jqCookie(cookieName);
                    }

                    jqCookie(cookieName, data, options);
                }
            };

            var epmLiveNavigation = (function () {
                var animSpeed = 300;
                var nodeClass = 'epm-nav-node';
                var hoverClass = 'epm-nav-node-hover';
                var openedClass = 'epm-nav-node-opened';
                var selectedClass = 'epm-nav-node-selected';
                var expandedClass = 'epm-nav-node-expanded';
                var collapsedClass = 'epm-nav-node-collapsed';

                var selectedTlNodeCookie = 'epmnav-selected-tlnode';
                var selectedLinkCookie = 'epmnav-selected-link';
                var expandStateCookie = 'epmnav-expand-state';
                var pinStateCookie = 'epmnav-pin-state';
                var cookieOptions = { expires: 365, path: '/' };

                var $sn = $('#epm-nav-sub');

                var tlNodes = [];

                var navNode = function (el) {
                    var categories = {};

                    var _el = el;
                    var _$el = $(el);
                    var _$sm = $('#epm-nav-sub-' + _$el.data('id'));

                    var _id = _el.id;
                    var _provider = _$el.data('linkprovider');

                    var _selected = function () {
                        return _$el.parent().hasClass(selectedClass);
                    };

                    var isOpened = function () {
                        return _$el.parent().hasClass(openedClass);
                    };

                    var _select = function (select) {
                        if (select) {
                            _$el.parent().addClass(selectedClass);
                        } else {
                            _$el.parent().removeClass(selectedClass);
                        }
                    };

                    var _close = function () {
                        _$sm.hide();
                        _$el.parent().removeClass(openedClass);
                    };

                    var _closeNav = function () {
                        if ($sn.is(':visible')) {
                            $sn.hide('slide', { direction: 'left' }, animSpeed);
                        }

                        _close();
                    };

                    var _registerWorkspace = function (link, workspaceTree) {
                        var node = new window.Telerik.Web.UI.RadTreeNode();

                        var parent = workspaceTree;

                        if (link.category) {
                            parent = workspaceTree.findNodeByValue(link.category) || parent;
                        }

                        node.set_text(link.title);
                        node.set_value(link.webId);

                        if (link.active) {
                            node.set_navigateUrl(link.url);
                        } else {
                            node.set_navigateUrl('javascript:;');
                        }

                        window.epmLiveNavigation.wsTeamDict = window.epmLiveNavigation.wsTeamDict || {};
                        window.epmLiveNavigation.wsTeamDict[link.webId] = link.itemId;

                        parent.get_nodes().add(node);
                    };

                    var _registerLink = function (link) {
                        if (link.seprator) {
                            registerSeprator(link.category);
                        } else if (link.url.toLowerCase() === 'header') {
                            registerCategory(link.category);
                            registerHeader(link);
                        } else if (link.url.toLowerCase() === 'placeholder') {
                            registerCategory(link.category);
                            registerPlaceHolder(link);
                        } else {
                            registerCategory(link.category);
                            registerLink(link);
                        }
                    };

                    var registerPlaceHolder = function (link) {
                        var category = link.category;

                        if (!category) {
                            category = '__STATIC__';
                        }

                        var li = '<li class="epm-nav-sub-placeholder">' + link.title + '</li>';

                        categories[category].$el.append(li);
                    };

                    var registerHeader = function (link) {
                        var category = link.category;

                        if (!category) {
                            category = '__STATIC__';
                        }

                        var cssClass = 'epm-nav-sub-header';

                        if (_$sm.find('li.epm-nav-section-header').length === 0) {
                            cssClass = 'epm-nav-section-header';
                        }

                        var li = '<li class="' + cssClass + '">' + link.title + '</li>';

                        categories[category].$el.append(li);

                        li = '<li class="epm-nav-sub-header-bottom"></li>';

                        categories[category].$el.append(li);
                    };

                    var registerLink = function (link) {
                        var category = link.category;

                        if (!category) {
                            category = '__STATIC__';
                        }

                        var isButton = false;

                        var classes = link.cssClass.split(' ');
                        for (var i = 0; i < classes.length; i++) {
                            if (classes[i] === 'epm-nav-button') {
                                isButton = true;
                                link.cssClass = link.cssClass.replace(/epm-nav-button/g, '');
                                break;
                            }
                        }

                        var icon = '';

                        if (link.cssClass) {
                            icon = '<span class="epm-nav-icon ' + link.cssClass + '"></span>';
                        }

                        var cssClass = 'epm-nav-node epm-nav-trans';

                        var providerId = categories[category].$el.parent().get(0).id;

                        if (providerId === 'epm-nav-sub-settings' || providerId === 'epm-nav-sub-workplace') {
                            cssClass = 'epm-nav-node';
                        }

                        if (isButton) {
                            cssClass = cssClass + ' epm-nav-button';
                            cssClass = cssClass.replace(/epm-nav-trans/g, '');
                        }

                        var target = '';

                        if (link.external) {
                            target = ' target="_blank"';
                        }

                        var html = '<li id="' + link.id + '" class="' + cssClass + '" style="display:none;">' + icon + '<a id="epm-nav-link-' + link.id + '" href="' + link.url + '" data-siteid="' + link.siteId + '" data-webid="' + link.webId + '" data-listid="' + link.listId + '" data-itemid="' + link.itemId + '"' + target + '><span>' + link.title + '</span></a></li>';

                        categories[category].$el.append(html);

                        var $link = $('#' + link.id);

                        if (link.visible) {
                            $link.show();
                        }

                        $link.find('a').click(function () {
                            var $a = $(this);
                            var $parent = $a.parent();

                            if (!$parent.hasClass('epm-nav-button')) {
                                $sn.find('a').each(function () {
                                    $(this).parent().removeClass(selectedClass);
                                });

                                $parent.addClass(selectedClass);

                                $.cookie(selectedLinkCookie, JSON.stringify({ id: $a.get(0).id, url: $a.attr('href') }), cookieOptions);
                            }
                        });
                    };

                    var registerSeprator = function (category) {
                        var seprator = '<div class="epm-nav-sub-sep"></div>';

                        if (category) {
                            $('#' + calculateCatId(category)).append(seprator);
                        } else {
                            _$sm.append(seprator);
                        }
                    };

                    var registerCategory = function (category) {
                        if (!category) {
                            category = '__STATIC__';
                        }

                        if (!categories[category]) {
                            var id = calculateCatId(category);
                            var defaultCssClass = 'epm-nav-node-expanded';

                            if (category === '__STATIC__') {
                                defaultCssClass = 'epm-nav-node-static';
                            }

                            if (category !== '__STATIC__') {
                                _$sm.append('<div id="' + id + '" class="epm-nav-node epm-nav-node-root epm-nav-cat"><span class="' + defaultCssClass + '">&nbsp;</span><span class="epm-nav-cat-title">' + category + '</span></div>');
                            }

                            _$sm.append('<ul id="' + id + '-links" class="epm-nav-links ' + defaultCssClass + '"></ul>');

                            categories[category] = {
                                id: id,
                                $el: $('#' + id + '-links')
                            };
                        }
                    };

                    var calculateCatId = function (category) {
                        return _$sm.get(0).id + '-' + category.toLowerCase().replace(/ /g, '-').replace(/_/g, '').replace(/[^\w-]+/g, '').replace(/--/g, '-');
                    };

                    var showNode = function () {
                        _$sm.fadeIn(300);
                        _$el.parent().addClass(openedClass);
                    };

                    var openMenu = function () {
                        showNode();

                        if (!$sn.is(':visible')) {
                            $sn.show('slide', { direction: 'left' }, animSpeed);
                        }
                    };

                    _$el.click(function () {
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
                        registerWorkspace: _registerWorkspace,
                        el: _el,
                        $el: _$el,
                        $menu: _$sm
                    };
                };

                function hideMenu() {
                    for (var n = 0; n < tlNodes.length; n++) {
                        var node = tlNodes[n];
                        if (node.selected()) {
                            node.closeNav();
                            break;
                        }
                    }
                }

                function changePinState(state) {
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
                }

                function togglePinned() {
                    if ($sn.data('pinstate') === 'pinned') {
                        changePinState('unpinned');
                    } else {
                        changePinState('pinned');
                    }
                }

                function getSelectedSubLevelNode() {
                    return ($.cookie(selectedTlNodeCookie) || 'epm-nav-top-ql').replace('epm-nav-top-', 'epm-nav-sub-');
                }

                function getLinkNodes(menu) {
                    return $('#' + menu).find('.epm-nav-sub-menu').find('div[id^=E]');
                }

                function selectLink() {
                    var link = $.parseJSON($.cookie(selectedLinkCookie));
                    if (link) {
                        if (link.id) {
                            $($sn.find('#' + link.id).get(0)).parent().addClass(selectedClass);
                        } else {
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
                    }
                }

                function saveLinkState($nav) {
                    var data = {};

                    $nav.find('.epm-nav-cat').each(function () {
                        var $cat = $(this);
                        data[$cat.text()] = $($cat.find('span')[0]).hasClass(expandedClass);
                    });

                    saveExpandState($nav.get(0).id, data);
                }

                function saveExpandState(nodeId, data) {
                    var state = $.parseJSON($.cookie(expandStateCookie)) || {};
                    state[nodeId] = data;
                    $.cookie(expandStateCookie, JSON.stringify(state), cookieOptions);
                }

                function expandNodes(provider) {
                    var expandState = $.parseJSON($.cookie(expandStateCookie));

                    if (expandState) {
                        var selectedNode = 'epm-nav-sub-ql';

                        if (provider) {
                            for (var m = 0; m < tlNodes.length; m++) {
                                var tlNode = tlNodes[m];

                                if (tlNode.provider === provider) {
                                    selectedNode = tlNode.id.replace('epm-nav-top-', 'epm-nav-sub-');
                                    break;
                                }
                            }
                        }

                        var snExpandStatus = expandState[selectedNode];
                        if (snExpandStatus) {
                            if (selectedNode === 'epm-nav-sub-ql') {
                                var nodes = getLinkNodes('epm-nav-sub-ql');

                                for (var i = 0; i < nodes.length; i++) {
                                    var node = nodes[i];
                                    var nodeId = node.id.replace('Nodes', '');

                                    if (!snExpandStatus[$('#' + nodeId.replace('EPMLiveNavn', 'EPMLiveNavt')).text()]) {
                                        TreeView_ToggleNode(window.EPMLiveNav_Data, 0, document.getElementById(nodeId), ' ', document.getElementById(node.id));
                                    }
                                }
                            } else {
                                var $cats = $('#' + selectedNode).find('.epm-nav-cat');

                                if (selectedNode === 'epm-nav-sub-settings') {
                                    $cats.each(function () {
                                        collapseLinks($(this));
                                    });
                                }

                                $cats.each(function () {
                                    var $cat = $(this);
                                    var key = $cat.text();
                                    var expand = snExpandStatus[key];

                                    if ($($cat.find('span')[0]).hasClass(expandedClass)) {
                                        if (!expand) {
                                            collapseLinks($cat);
                                        }
                                    } else {
                                        if (expand) {
                                            expandLinks($cat);
                                        }
                                    }
                                });
                            }
                        }
                    }
                }

                function expandLinks($cat) {
                    var catId = $cat.get(0).id;

                    var $span = $($cat.find('span')[0]);
                    var $ul = $('#' + catId + '-links');

                    $span.removeClass(collapsedClass);
                    $span.addClass(expandedClass);
                    $ul.removeClass(collapsedClass);
                    $ul.addClass(expandedClass);
                }

                function collapseLinks($cat) {
                    var catId = $cat.get(0).id;

                    var $span = $($cat.find('span')[0]);
                    var $ul = $('#' + catId + '-links');

                    $span.removeClass(expandedClass);
                    $span.addClass(collapsedClass);
                    $ul.removeClass(expandedClass);
                    $ul.addClass(collapsedClass);
                }

                function registerLinkEvents() {
                    var addTooltip = function () {
                        $('li.epm-nav-node').find('a').each(function () {
                            var $a = $(this);

                            if (!$a.attr('alt')) {
                                var $span = $a.find('span');
                                if ($span) {
                                    if ($a.width() < $span.width()) {
                                        $a.attr('alt', $span.text());
                                        $a.attr('title', $span.text());
                                    }
                                }
                            }
                        });
                    };

                    $('.epm-nav-cat').click(function () {
                        var $cat = $(this);
                        var $span = $($cat.find('span')[0]);

                        if ($span.hasClass(collapsedClass)) {
                            expandLinks($cat);
                        } else {
                            collapseLinks($cat);
                        }

                        saveLinkState($cat.parent());
                    });

                    $('#epm-nav-top li').click(function () {
                        addTooltip();
                    });

                    addTooltip();
                }

                function handleContextualCommand(id, webId, listId, itemId, command, kind) {
                    var removeLink = function (linkId, notifId) {
                        var remove = function (lid, nid) {
                            var $item = $('#' + lid);
                            $item.fadeOut(300, function () {
                                $item.remove();
                            });

                            if (nId) {
                                SP.UI.Notify.removeNotification(nid);
                            }
                        };

                        epmLiveService.execute('RemoveNavigationLink', linkId, function (response) {
                            remove(linkId, notifId);
                        }, function (response) {
                            remove(linkId, notifId);
                        });
                    };

                    var url = window.epmLiveNavigation.currentWebUrl;
                    var gaUrl = (url + '/_layouts/15/epmlive/gridaction.aspx?webid=' + webId + '&listid=' + listId + '&id=' + itemId + '&').replace(/\/\//g, '/');
                    var rpUrl = (url + '/_layouts/15/epmlive/redirectionproxy.aspx?webid=' + webId + '&listid=' + listId + '&id=' + itemId + '&').replace(/\/\//g, '/');

                    var redirectUrl = '';

                    switch (command) {
                        case 'nav:add':
                            redirectUrl = rpUrl + 'action=new';
                            break;
                        case 'nav:team':
                            var wId = '';
                            var lId = '';
                            var iId = '';

                            if (itemId === 'undefined') {
                                try {
                                    var info = window.epmLiveNavigation.wsTeamDict[webId].split('.');
                                    if (info[2] !== '-1') {
                                        wId = info[0];
                                        lId = info[1];
                                        iId = info[2];
                                    }
                                } catch(e) {
                                }
                            }

                            redirectUrl = (url + '/_layouts/15/epmlive/gridaction.aspx?').replace(/\/\//g, '/') + 'action=buildteam&webid=' + (wId || webId);

                            if (iId) {
                                redirectUrl = redirectUrl + '&listid=' + lId + '&id=' + iId;
                            }
                    }

                    if (!redirectUrl && command) {
                        if (command.indexOf('epkcommand:') !== -1) {
                            redirectUrl = gaUrl + 'action=epkcommand&subaction=' + command.split(':')[1];
                        } else {
                            redirectUrl = gaUrl + 'action=' + command;
                        }
                    }

                    switch (kind) {
                        case '0':
                            OpenCreateWebPageDialog(redirectUrl);
                            break;
                        case '1':
                            location.href = redirectUrl;
                            break;
                        case '2':
                            window.open(redirectUrl + '&IsDlg=1', '', 'height=100, width=200, toolbar=no, menubar=no, scrollbars=yes, resizable=yes,location=no, directories=no, status=yes');
                            break;
                        case '3':
                            window.open(redirectUrl + '&IsDlg=1', '', 'width=' + screen.width + ',height=' + screen.height + ',top=0,left=0, toolbar=no, menubar=no, scrollbars=yes, resizable=yes,location=no, directories=no, status=yes');
                            break;
                        case '5':
                            SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', { url: redirectUrl, width: 600, height: 500 });
                            break;
                        case '6':
                            SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', { url: redirectUrl, showMaximized: true });
                            break;
                        case '98':
                            if (command !== 'nav:addToFav') {
                                if (command !== 'nav:remove' && command !== 'nav:removeFavWS') {
                                    $.get(redirectUrl).always(function() {
                                        removeLink(id);
                                    });
                                } else {
                                    if (command === 'nav:removeFavWS') {
                                        $('#epm-nav-sub-workspaces-static-links').find('a').each(function() {
                                            var $ws = $(this);
                                            if ($ws.data('webid') === webId) {
                                                id = $ws.parent().get(0).id;
                                            }
                                        });
                                    }

                                    removeLink(id);

                                    if (command === 'nav:remove') {
                                        var $a = $('#' + id).find('a');

                                        window.Analytics.turnOffFav({
                                            siteid: $a.data('siteid'),
                                            webid: $a.data('webid'),
                                            listid: $a.data('listid'),
                                            itemid: $a.data('itemid'),
                                            url: $a.get(0).href
                                        });
                                    }
                                }
                            } else {
                                //var title = $($('#' + id).find('a').get(0)).text();

                                //var _$$ = window.epmLive;

                                //var data = '<Data><Param key="SiteId">' + _$$.currentSiteId + '</Param><Param key="WebId">' + _$$.currentWebId + '</Param><Param key="ListId">' + _$$.currentListId + '</Param><Param key="ListViewUrl">' + _$$.currentListViewUrl + '</Param><Param key="ListIconClass">' + _$$.currentListIcon + '</Param><Param key="ItemId">' + _$$.currentItemID + '</Param><Param key="FString">' + _$$.currentUrl + '</Param><Param key="Type">4</Param><Param key="UserId">' + _$$.currentUserId + '</Param><Param key="Title">' + title + '</Param><Param key="FileIsNull">' + _$$.currentFileIsNull + '</Param></Data>';
                                
                                //epmLiveService.execute('AddFavorites', data, function (response) {
                                //    var i = 0;
                                //}, function (response) {
                                //});
                            }
                            break;
                        case '99':
                            if (confirm('Are you sure you want to send the item(s) to the Recycle Bin?')) {
                                var nId = SP.UI.Notify.addNotification('Deleting Item...', true, '', null);
                                if (command !== 'nav:remove') {
                                    $.get(redirectUrl).always(function () {
                                        removeLink(id, nId);
                                    });
                                } else {
                                    removeLink(id, nId);
                                }
                            }
                            break;
                        default:
                            SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', { url: redirectUrl, width: 700 });
                            break;
                    }
                }

                function registerEvents() {
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
                        var cookieData = null;

                        var cdt = $.parseJSON($.cookie(expandStateCookie));
                        if (cdt) {
                            cdt = $.parseJSON($.cookie(expandStateCookie))['epm-nav-sub-ql'];
                            if (cdt) {
                                cookieData = cdt;
                            }
                        }

                        var d = cookieData || {};
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

                                    var nId = (n.id).replace('Nodes', '');

                                    if (nId === elNav.id) {
                                        state = !state;
                                    }

                                    d[$('#' + nId.replace('EPMLiveNavn', 'EPMLiveNavt')).text()] = state;
                                }

                                break;
                            }
                        }

                        saveExpandState(nodeId, d);
                        toggleNode(data, x, elNav, y, elNodes);
                    };

                    window.epmLiveNavigation.handleContextualCommand = function (id, webId, listId, itemId, command, kind) {
                        handleContextualCommand(id, webId, listId, itemId, command, kind);
                    };

                    $('td.epm-nav-node-root').click(function () {
                        var $td = $(this);
                        var id = $td.parent().find('a').get(0).id;

                        TreeView_ToggleNode(EPMLiveNav_Data, 0, document.getElementById(id), ' ', document.getElementById(id + 'Nodes'));
                    });

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

                    $sn.hover(function (event) {
                        $pin.show();
                    }, function () {
                        $pin.hide();
                    });

                    try {
                        $sn.slimScroll({
                            height: $sn.height(),
                            width: $sn.width()
                        });
                    } catch (e) {
                    }

                    var $ss = $('#epm-nav').find('.slimScrollDiv');
                    var $sb = $ss.find('.slimScrollBar');

                    $ss.css('position', 'absolute');
                    $ss.css('left', '50px');
                    $sn.css('left', '0');
                    $sb.css('z-index', 1001);

                    $(window).resize(function () {
                        var height = $('#epm-nav-top').height();
                        $ss.height(height);
                        $sn.height(height);
                    });

                    $('a.epm-nav-node').click(function () {
                        var $link = $(this);

                        $('#epm-nav-sub-ql').find('a').each(function () {
                            $(this).parent().parent().parent().parent().removeClass(selectedClass);
                        });

                        $link.parent().parent().parent().parent().addClass(selectedClass);

                        var index = -1;

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

                    window.epmLiveNavigation.snWidth = $sn.width();

                    var $wsTree;
                    
                    var workspaceTree = window.epmLiveNavigation.workspaceTree();                    
                    if (workspaceTree) {
                        $wsTree = $('#' + workspaceTree._element.id);
                    }

                    var snWidth = $sn.width();

                    var expandWorkspaceMenu = function () {
                        $('a.rtIn').each(function () {
                            var $a = $(this);
                            $a.attr('style', '');
                        });

                        $('.epm-nav-ws-node').each(function () {
                            $(this).attr('style', '');
                        });

                        var wsWidth = 0;

                        try {
                            wsWidth = $wsTree.width();
                        } catch(ex) {
                        } 
                        
                        var newWidth = 0;

                        if (snWidth < wsWidth) {
                            newWidth = wsWidth + 20 + 20;

                            $sn.stop().animate({ width: newWidth }, 300);
                            $sn.parent().stop().animate({ width: newWidth }, 300);
                        }
                        
                        $('.epm-nav-ws-node').each(function () {
                            var $div = $(this);

                            var width = (50 + (newWidth === 0 ? snWidth : newWidth)) - $div.offset().left;
                            $div.attr('style', 'width:' + width + 'px');
                        });

                        window.epmLiveNavigation.wsTreeExpanded = true;
                    };

                    var collapseWorkspaceTree = function () {
                        $sn.stop().animate({ width: snWidth }, 300);
                        $sn.parent().stop().animate({ width: snWidth }, 300);

                        $('.epm-nav-ws-node').each(function () {
                            $(this).attr('style', '');
                        });

                        window.epmLiveNavigation.resetWSNodeWidth();
                        window.epmLiveNavigation.wsTreeExpanded = false;
                    };

                    var expandOrCollapseWorkspaceMenu = function ($el) {
                        var classes = $el.attr('class').toLowerCase();

                        if (classes.indexOf('plus') !== -1) {
                            var onReady1 = function () {
                                if ($el.attr('class').toLowerCase().indexOf('minus') !== -1) {
                                    window.setTimeout(function () {
                                        expandWorkspaceMenu();
                                    }, 100);
                                } else {
                                    window.setTimeout(function () {
                                        onReady1();
                                    }, 1);
                                }
                            };

                            onReady1();
                        } else {
                            var onReady2 = function () {
                                if ($el.attr('class').toLowerCase().indexOf('plus') !== -1) {
                                    window.setTimeout(function () {
                                        collapseWorkspaceTree();
                                    }, 100);
                                } else {
                                    window.setTimeout(function () {
                                        onReady2();
                                    }, 1);
                                }
                            };

                            onReady2();
                        }
                    };

                    var $wsMenu = $('#epm-nav-sub-workspaces');

                    $('#EPMNavWorkspacesTree').hover(function () {
                        if (!window.epmLiveNavigation.wsTreeExpanded) {
                            expandWorkspaceMenu();
                        }
                    }, function () {
                        collapseWorkspaceTree();
                    });

                    $sn.hover(function () {
                    }, function () {
                        if ($wsMenu.is(':visible')) {
                            collapseWorkspaceTree();
                        }
                    });

                    $('a.rtIn').each(function () {
                        var $spans = $(this).parent().parent().find('span');
                        if ($spans.length > 1) {
                            var $span = $($spans.get(1));

                            if ($span) {
                                $span.click(function () {
                                    expandOrCollapseWorkspaceMenu($(this));
                                });

                                $span.hover(function () {
                                    window.epmLiveNavigation.wsNodeSelectorClass = $(this).attr('class');
                                });
                            }
                        }
                    });
                }

                function registerProviderLinks(response) {
                    var buildLink = function (link, page) {
                        return {
                            id: link['@Id'],
                            title: link['@Title'],
                            url: (link['#cdata'] || '').replace(/{page}/g, page),
                            category: link['@Category'],
                            cssClass: link['@CssClass'],
                            order: parseInt(link['@Order']),
                            siteId: link['@SiteId'],
                            webId: link['@WebId'],
                            listId: link['@ListId'],
                            itemId: link['@ItemId'],
                            external: link['@External'] === 'True',
                            visible: link['@Visible'] === 'True',
                            active: link['@Active'] === 'True',
                            seprator: link['@Separator'] === 'True'
                        };
                    };

                    var webUrl = $$.currentWebUrl();
                    var pg = webUrl + window.location.href.split(webUrl)[1];
                    var workspacesTitleRegistered = false;

                    for (var providerName in response.Nodes) {
                        var navLink = response.Nodes[providerName].NavLink;

                        if (!navLink.length) {
                            navLink = [navLink];
                        }

                        for (var nl in navLink) {
                            var lnk = navLink[nl];

                            for (var j = 0; j < tlNodes.length; j++) {
                                var tlNode = tlNodes[j];

                                if (tlNode.provider === providerName) {
                                    if (providerName !== 'Workspaces') {
                                        tlNode.registerLink(buildLink(lnk, pg));
                                    } else {
                                        var workspaceTree = window.epmLiveNavigation.workspaceTree();

                                        workspaceTree.trackChanges();

                                        var title = lnk['@Title'];

                                        if (workspacesTitleRegistered) {
                                            tlNode.registerWorkspace(buildLink(lnk, pg), workspaceTree);
                                        } else {
                                            tlNode.registerLink(buildLink(lnk, pg));

                                            if (title === 'All Workspaces') {
                                                workspacesTitleRegistered = true;
                                                $('#epm-nav-sub-workspaces-static-links').remove().insertBefore('#EPMNavWorkspacesTree');
                                            }
                                        }

                                        workspaceTree.commitChanges();
                                    }
                                }
                            }
                        }

                        if (providerName === 'Workspaces') {
                            window.epmLiveNavigation.resetWorkspaceTree = function () {
                                var wsTree = window.epmLiveNavigation.workspaceTree();

                                wsTree.trackChanges();

                                var nodes = wsTree.get_allNodes();
                                for (var i = 0; i < nodes.length; i++) {
                                    nodes[i].set_expanded(false);
                                }

                                var expandNode = function (webId) {
                                    var node = wsTree.findNodeByValue(webId);

                                    if (node && node != wsTree) {
                                        var parent = node.get_parent();

                                        if (parent !== wsTree && !parent.get_expanded()) {
                                            parent.set_expanded(true);

                                            expandNode(parent.get_value());
                                        }
                                    }
                                };

                                var wId = window.epmLiveNavigation.currentWebId;

                                var cNode = wsTree.findNodeByValue(wId);

                                if (cNode) {
                                    if (cNode.get_parent() !== wsTree) {
                                        expandNode(wId);
                                    } else {
                                        cNode.set_expanded(true);
                                    }

                                    cNode.set_selected(true);
                                }

                                wsTree.commitChanges();
                            };

                            var wsTree = window.epmLiveNavigation.workspaceTree();

                            $('a.rtIn').each(function () {
                                var $a = $(this);

                                var $parent = $a.parent();

                                $parent.append('<div id="' + wsTree.findNodeByText($a.text()).get_value() + '" class="epm-nav-ws-node"></div>');
                                $a.remove();

                                var text = $a.text();
                                $a.text('');
                                $a.append('<span>' + text + '</span>');
                                $parent.find('.epm-nav-ws-node').append($a);
                            });

                            window.epmLiveNavigation.resetWorkspaceTree();
                            $('#EPMNavWorkspacesTree').fadeIn(300);
                        }
                        else if (providerName === 'Applications') {
                            $($('#epm-nav-sub-new-static-links').find('.epm-nav-sub-header').get(0)).attr('style', 'padding-top: 0px !important');
                        } else if (providerName === 'Favorites') {
                            $($('#epm-nav-sub-favorites-static-links').find('.epm-nav-sub-header').get(0)).attr('style', 'padding-top: 0px !important');
                        }

                        window.SP.SOD.notifyScriptLoadedAndExecuteWaitingJobs('EPMLiveNavigation_' + providerName);

                        if (providerName !== 'Settings') {
                            expandNodes(providerName);
                        }
                    }
                }

                function loadLinks() {
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
                            registerProviderLinks(response);
                            registerLinkEvents();
                        }, function (response) {
                            console.log(response);
                        });
                    }
                }

                function registerStaticProviderLinks() {
                    var links = null;

                    if (window.epmLiveNavigation) {
                        links = window.epmLiveNavigation.staticProvider;
                    }

                    if (links) {
                        registerProviderLinks($$.parseJson(base64Service.decode(links)));
                    } else {
                        window.setTimeout(function () {
                            registerStaticProviderLinks();
                        }, 1);
                    }
                }

                function registerTopLevelNodes() {
                    $('#epm-nav-top').find("[data-role='top-nav-node']").each(function () {
                        tlNodes.push(new navNode(this));
                    });
                }

                window.epmLiveNavigation.removeLink = function (link) {
                    var $a = $sn.find('#epm-nav-link-' + link.id);
                    window.epmLiveNavigation.handleContextualCommand(link.id, $a.data('webid'), $a.data('listid'), $a.data('itemid'), 'nav:remove', '98');
                };
                
                window.epmLiveNavigation.registerLink = function(link) {
                    if (link) {
                        var providerName;

                        switch (link.kind) {
                        case 0:
                        case 1:
                            providerName = 'Favorites';
                            break;
                        case 2:
                        case 3:
                            providerName = 'Workspaces';
                            break;
                        }

                        if (providerName) {
                            for (var j = 0; j < tlNodes.length; j++) {
                                var tlNode = tlNodes[j];

                                if (tlNode.provider === providerName) {

                                    if (link.kind !== 3) {
                                        link.cssClass = 'epm-nav-sortable ' + link.cssClass;
                                    }

                                    tlNode.registerLink(link);

                                    var $li = $sn.find('#' + link.id);

                                    if (link.kind > -1 && link.kind < 3) {
                                        var $menu = $('#epm-nav-sub-' + providerName.toLowerCase() + '-static-links');
                                        
                                        $menu.find('.epm-nav-sub-placeholder').each(function() {
                                            var $ph = $(this);
                                            var remove = false;
                                            var text = $ph.text();
                                            
                                            if (link.kind === 0 && text === 'No pages') {
                                                remove = true;
                                            } else if (link.kind === 1 && text === 'No items') {
                                                remove = true;
                                            }
                                            
                                            if (remove) {
                                                $ph.remove();
                                            }
                                        });
                                        
                                        if (link.kind < 2) {
                                            $li.find('a').attr('style', 'width:115px !important;');
                                        }

                                        if (link.kind === 0 || link.kind === 2) {
                                            try {
                                                $li.remove().insertBefore($($menu.find('.epm-nav-sub-header').get(1)));
                                            } catch(e) {
                                            }
                                        }

                                        if (link.kind !== 2) {
                                            window.epmLiveNavigation.addFavoritesMenu($li);
                                        } else {
                                            window.epmLiveNavigation.addFavoriteWSMenu($li);
                                        }
                                    }

                                    if (link.kind !== 3) {
                                        $li.addClass('epm-nav-sortable');

                                        window.epmLiveNavigation.addDragger($li);
                                    }

                                    $li.fadeIn(300);

                                    break;
                                }
                            }
                        }
                    }
                };

                var _init = function () {
                    registerTopLevelNodes();
                    registerStaticProviderLinks();
                    changePinState($.cookie(pinStateCookie) || 'pinned');
                    expandNodes();
                    selectLink();
                    registerEvents();

                    ExecuteOrDelayUntilScriptLoaded(loadLinks, 'EPMLive.js');
                };

                return {
                    init: _init,
                    expandProviderNodes: expandNodes
                };
            })();

            epmLiveNavigation.init();

            var menuManager = (function () {
                var _setupMenu = function ($li, defaultCommands) {
                    defaultCommands = defaultCommands || [];

                    var $menu = $($li.find('.epm-nav-contextual-menu').get(0));

                    var setup = function (commands, $ca) {
                        var getIcon = function (command) {
                            switch (command.toLowerCase()) {
                                case 'nav:remove':
                                case 'nav:addtofav':
                                case 'nav:removefavws':
                                    return 'icon-star-6';
                                case 'view':
                                    return 'icon-info';
                                case 'edit':
                                    return 'icon-pencil-2';
                                case 'delete':
                                    return 'icon-close-3';
                                case 'approve':
                                    return 'icon-thumbs-up-4';
                                case 'comments':
                                    return 'icon-bubble-dots';
                                case 'buildteam':
                                    return 'icon-user-plus-3';
                                case 'workspace':
                                    return 'icon-tree-2';
                                default:
                                    return 'epm-nav-cm-blank';
                            }
                        };

                        var liId = $li.get(0).id;

                        if (commands.length) {
                            commands.push({ title: '--SEP--' });
                        }

                        for (var dc in defaultCommands) {
                            commands.push(defaultCommands[dc]);
                        }

                        $li.append('<ul class="epm-nav-contextual-menu"></ul>');

                        $menu = $($li.find('.epm-nav-contextual-menu').get(0));

                        for (var i = 0; i < commands.length; i++) {
                            var cmd = commands[i];

                            var webId = $ca.data('webid');

                            if (!webId || webId === 'undefined') {
                                try {
                                    webId = $ca.parent().get(0).id;
                                } catch(e) {
                                } 
                            }

                            if (cmd.title === '--SEP--') {
                                if (i !== commands.length - 1) {
                                    $menu.append('<li class="seprator"></li>');
                                }
                            } else {
                                $menu.append('<li><span class="epm-nav-cm-icon ' + getIcon(cmd.command) + '">&nbsp;</span><a href="javascript:epmLiveNavigation.handleContextualCommand(\'' + liId + '\',\'' + webId + '\',\'' + $ca.data('listid') + '\',\'' + $ca.data('itemid') + '\',\'' + cmd.command + '\',\'' + cmd.kind + '\');" style="width: 136px !important;">' + cmd.title + '</a></li>');

                                $menu.find('a').click(function() {
                                    hideMenu();
                                });
                            }
                        }

                        $menu.hover(function () {
                            window.epmNavHoveredNode = liId;
                        });

                        $('.epm-nav-node, .epm-nav-ws-node').hover(function () {
                            var id = this.id;
                            window.epmNavHoveredNode = id;

                            if (id !== liId) {
                                window.setTimeout(function () {
                                    if (window.epmNavHoveredNode === id) {
                                        hideMenu();
                                    }
                                }, 200);
                            }
                        });

                        $('.epm-nav-links, #EPMNavWorkspacesTree').hover(function () {
                        }, function () {
                            window.epmNavHoveredNode = null;

                            window.setTimeout(function () {
                                if (window.epmNavHoveredNode === null) {
                                    hideMenu();
                                }
                            }, 200);
                        });

                        $(document).on('click', '.epm-nav-dragger', function() {
                            hideMenu();
                        });

                        toggleMenu();
                    };

                    var showMenu = function () {
                        $menu.fadeIn(200);
                    };

                    var hideMenu = function () {
                        $menu.fadeOut(200);
                    };

                    var toggleMenu = function () {
                        if ($menu.is(':visible')) {
                            hideMenu();
                        } else {
                            showMenu();
                        }
                    };

                    if ($li.find('.epm-nav-contextual-menu').length === 0) {
                        var $a = $($li.find('a').get(0));

                        var data = '<Request><Params><SiteId>' + $a.data('siteid') + '</SiteId><WebId>' + $a.data('webid') + '</WebId><ListId>' + $a.data('listid') + '</ListId><ItemId>' + $a.data('itemid') + '</ItemId></Params></Request>';

                        epmLiveService.execute('GetContextualMenuItems', data, function (response) {
                            var commands = [];

                            var items = response.Items.Item;

                            if (items) {
                                if (!items.length) {
                                    items = [items];
                                }

                                for (var i = 0; i < items.length; i++) {
                                    var item = items[i];
                                    commands.push({ title: item['@Title'], command: item['@Command'], kind: item['@Kind'], imgUrl: item['@ImageUrl'] });
                                }
                            }

                            setup(commands, $a);
                        }, function (response) {
                            setup([], $a);
                        });
                    } else {
                        toggleMenu();
                    }
                };

                return {
                    setupMenu: _setupMenu
                };
            })();

            var manageSettings = function () {
                var settingsManager = (function () {
                    var _collapseAll = function ($sl) {
                        $sl.find('.epm-nav-cat').each(function () {
                            var $span = $($(this).find('span').get(0));
                            $span.removeClass('epm-nav-node-expanded');
                            $span.addClass('epm-nav-node-collapsed');
                        });

                        $sl.find('.epm-nav-links').each(function () {
                            var $list = $(this);

                            if (!$list.hasClass('epm-nav-node-static')) {
                                $list.removeClass('epm-nav-node-expanded');
                                $list.addClass('epm-nav-node-collapsed');
                            }
                        });
                    };

                    return {
                        collapseAll: _collapseAll
                    };
                })();

                var $ul = $('#epm-nav-sub-settings');

                settingsManager.collapseAll($ul);
                epmLiveNavigation.expandProviderNodes('Settings');
            };

            var manageFavorites = function () {
                var favoritesManager = (function () {
                    window.epmLiveNavigation.addDragger = function ($li) {
                        $li.prepend('<span class="epm-nav-dragger">&nbsp;</span>');

                        $li.hover(function () {
                            $($(this).find('.epm-nav-dragger').get(0)).css('visibility', 'visible');
                        }, function () {
                            $($(this).find('.epm-nav-dragger').get(0)).css('visibility', 'hidden');
                        });
                    };

                    window.epmLiveNavigation.addFavoritesMenu = function ($li) {
                        $li.append('<span class="epm-menu-btn"><span class="icon-ellipsis-horizontal"></span></span>');

                        $($li.find('.epm-menu-btn').get(0)).click(function() {
                            menuManager.setupMenu($li, [
                                { title: 'Remove', command: 'nav:remove', kind: '98' }
                            ]);
                        });
                    };

                    var _resetOrder = function ($list) {
                        var orders = [];

                        var lo = 0;
                        $list.find('.epm-nav-node').each(function () {
                            orders.push($(this).get(0).id + ':' + (++lo));
                        });

                        if (orders.length) {
                            epmLiveService.execute('ReorderLinks', orders.join(), function (response) {
                            }, function (response) {
                                console.log(response);
                            });
                        }
                    };

                    var _addDragger = function ($li) {
                        window.epmLiveNavigation.addDragger($li);
                    };

                    var _addMenu = function ($li) {
                        window.epmLiveNavigation.addFavoritesMenu($li);
                    };

                    return {
                        resetOrder: _resetOrder,
                        addDragger: _addDragger,
                        addMenu: _addMenu
                    };
                })();

                var $ul = $('#epm-nav-sub-favorites-static-links');

                $ul.find('.epm-nav-sortable').each(function () {
                    var $li = $(this).parent();

                    $li.addClass('epm-nav-sortable');
                    $($li.find('a').get(0)).attr('style', 'width:115px !important;');

                    favoritesManager.addDragger($li);
                    favoritesManager.addMenu($li);
                });

                try {
                    $ul.sortable({
                        items: 'li.epm-nav-sortable',
                        placeholder: 'epm-nav-drag-placeholder',
                        update: function (event, ui) {
                            favoritesManager.resetOrder($ul);
                        }
                    });

                    $ul.disableSelection();
                } catch (e) {
                }
            };

            var manageRecentItems = function () {
                var riManager = (function () {
                    var _addMenu = function ($li, rIndex) {
                        if ($li.index() < rIndex) {
                            addNewItemMenu($li);
                        }
                    };

                    var addNewItemMenu = function ($li) {
                        $li.append('<span class="epm-menu-btn"><span class="icon-plus-2"></span></span>');

                        $($li.find('a').get(0)).attr('style', 'width:115px !important;');

                        $($li.find('.epm-menu-btn').get(0)).click(function () {
                            var $a = $($li.find('a').get(0));
                            window.epmLiveNavigation.handleContextualCommand(null, $a.data('webid'), $a.data('listid'), null, 'nav:add', 0);
                        });
                    };

                    return {
                        addMenu: _addMenu
                    };
                })();

                var $ul = $('#epm-nav-sub-recent-static-links');

                var riIndex;

                $ul.find('.epm-nav-sub-header').each(function () {
                    var $el = $(this);
                    var header = $el.text();
                    var index = $el.index();

                    if (header === 'Recent Items') {
                        riIndex = index;
                    }
                });

                $ul.find('.epm-nav-node').each(function () {
                    riManager.addMenu($(this), riIndex);
                });
            };

            var manageWorkspaces = function () {
                var workspacesManager = (function () {
                    window.epmLiveNavigation.addFavoriteWSMenu = function($li) {
                        $li.append('<span class="epm-menu-btn"><span class="icon-ellipsis-horizontal"></span></span>');

                        $($li.find('.epm-menu-btn').get(0)).click(function () {
                            var liId = $li.get(0).id;

                            var commands = [];

                            if ($li.hasClass('epm-nav-ws-node')) {
                                var found = false;

                                $('#epm-nav-sub-workspaces-static-links').find('a').each(function () {
                                    if ($(this).data('webid') === liId) {
                                        found = true;
                                    }
                                });

                                if (found) {
                                    commands.push({ title: 'Remove', command: 'nav:removeFavWS', kind: 98 });
                                } else {
                                    commands.push({ title: 'Add', command: 'nav:addToFav', kind: 98 });
                                }
                            } else {
                                commands.push({ title: 'Remove', command: 'nav:removeFavWS', kind: 98 });
                            }

                            commands.push({ title: 'Manage team', command: 'nav:team', kind: 6 });

                            menuManager.setupMenu($li, commands);
                        });
                    };
                    
                    var _resetOrder = function ($list) {
                        var orders = [];

                        var lo = 0;
                        $list.find('li.epm-nav-sortable').each(function () {
                            orders.push($(this).get(0).id + ':' + (++lo));
                        });

                        if (orders.length) {
                            epmLiveService.execute('ReorderLinks', orders.join(), function (response) {
                            }, function (response) {
                                console.log(response);
                            });
                        }
                    };

                    var _addDragger = function ($li) {
                        $li.prepend('<span class="epm-nav-dragger">&nbsp;</span>');

                        $li.hover(function () {
                            $($(this).find('.epm-nav-dragger').get(0)).css('visibility', 'visible');
                        }, function () {
                            $($(this).find('.epm-nav-dragger').get(0)).css('visibility', 'hidden');
                        });
                    };

                    var _addMenu = function ($li) {
                        window.epmLiveNavigation.addFavoriteWSMenu($li);
                    };

                    var _configNodeWidth = function () {
                        window.epmLiveNavigation.resetWSNodeWidth = function () {
                            $('a.rtIn').each(function () {
                                var $a = $(this);
                                var $span = $($a.find('span').get(0));
                                var padding = 15;

                                var width = $span.width();
                                if (width) {
                                    var offset = $a.offset().left - 50;
                                    var total = (width + offset);

                                    var menuWidth = 180;
                                    var newWidth = menuWidth - offset - padding;

                                    $a.data('originalwidth', $a.width());
                                    $a.data('newwidth', newWidth);

                                    if (total > (menuWidth - padding)) {
                                        $a.width(newWidth);
                                    } else {
                                        $a.parent().width(newWidth);
                                    }
                                }
                            });
                        };

                        window.epmLiveNavigation.resetWSNodeWidth();
                    };

                    var _initSearch = function () {
                        var $sb = $('#EPMNavWSTSearch');

                        $sb.val('');
                        $sb.bindWithDelay('keyup', function () {
                            var term = $(this).val().trim();
                            if (term) {
                                if (term.length > 1) {
                                    filterTree(term.toLowerCase());
                                } else {
                                    showAllNodes();
                                }
                            } else {
                                showAllNodes();
                            }
                        }, 200);
                    };

                    var filterTree = function (term) {
                        var nodes = window.epmLiveNavigation.workspaceTree().get_allNodes();

                        for (var i = 0; i < nodes.length; i++) {
                            var node = nodes[i];
                            var nodeText = node.get_text().trim();

                            if (nodeText && nodeText.toLowerCase().indexOf(term) !== -1) {
                                setParentNodesVisible(node);
                                node.set_visible(true);
                            } else {
                                node.set_visible(false);
                            }
                        }
                    };

                    var showAllNodes = function () {
                        var nodes = window.epmLiveNavigation.workspaceTree().get_allNodes();

                        for (var i = 0; i < nodes.length; i++) {
                            nodes[i].set_visible(true);
                        }

                        window.epmLiveNavigation.resetWorkspaceTree();
                    };

                    var setParentNodesVisible = function (node) {
                        var pn = node.get_parent();

                        if (pn && pn !== window.epmLiveNavigation.workspaceTree()) {
                            pn.set_visible(true);
                            pn.set_expanded(true);
                            setParentNodesVisible(pn);
                        }
                    };

                    return {
                        resetOrder: _resetOrder,
                        addDragger: _addDragger,
                        addMenu: _addMenu,
                        initializeSearch: _initSearch,
                        configureNodeWidth: _configNodeWidth
                    };
                })();

                var $ul = $('#epm-nav-sub-workspaces-static-links');

                $ul.find('.epm-nav-sortable').each(function () {
                    var $li = $(this).parent();

                    $li.addClass('epm-nav-sortable');
                    $($li.find('a').get(0)).attr('style', 'width:125px !important; position: relative; top: 2px;');

                    workspacesManager.addDragger($li);
                    workspacesManager.addMenu($li);
                });

                try {
                    $ul.sortable({
                        items: 'li.epm-nav-sortable',
                        placeholder: 'epm-nav-drag-placeholder',
                        update: function () {
                            workspacesManager.resetOrder($ul);
                        }
                    });

                    $ul.disableSelection();
                } catch (e) {
                }

                workspacesManager.configureNodeWidth();
                workspacesManager.initializeSearch();

                $('#EPMNavWorkspacesTree').find('a').each(function () {
                    workspacesManager.addMenu($(this).parent());
                });
            };

            ExecuteOrDelayUntilScriptLoaded(manageSettings, 'EPMLiveNavigation_Settings');
            ExecuteOrDelayUntilScriptLoaded(manageFavorites, 'EPMLiveNavigation_Favorites');
            ExecuteOrDelayUntilScriptLoaded(manageWorkspaces, 'EPMLiveNavigation_Workspaces');
            ExecuteOrDelayUntilScriptLoaded(manageRecentItems, 'EPMLiveNavigation_RecentItems');
        });
    }

    function onJqueryLoaded() {
        ExecuteOrDelayUntilScriptLoaded(initializeNavigation, 'EPMLiveNavigation');
    }

    ExecuteOrDelayUntilScriptLoaded(onJqueryLoaded, 'jquery.min.js');
})();