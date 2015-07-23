﻿(function() { 
    'use strict';

    window.toggleSearch = function() {
        var $sbox = $('#search-box-container');
        var $sicon = $('#search-container a');
        var $sinput = $($sbox.find('input').get(0));

        if ($sbox.is(':visible')) {
            $sicon.css('color', '#00668E');
        } else {
            $sicon.css('color', '#FFFFFF');
        }

        $sbox.toggle('fast',function() {
            $sinput.focus();
        });
    };

    window.updateProfilePic = function () {
        $('body').trigger('epm.updateProfilePic');
        OpenPopUpPageWithTitle(window.epmLive.currentSiteUrl + '/_layouts/15/epmlive/UploadProfilePicture.aspx', profilePicUpdated, null, null, 'Update Profile Photo');
    };

    window.profilePicUpdated = function(status, picUrl) {
        if (status === 1) {
            $('#EPMLiveProfilePic').attr('src', picUrl + '?v' + new Date().getTime()).show();
        }
    };

    window.walkme_ready = function() {
        var $supportLink = $('#epm-support-link');
        $supportLink.attr('href', '#');
        $supportLink.removeAttr('target');

        $supportLink.click(function(event) {
            $('.walkme-menu-click-close').after('<a id="support-link" style="right: 31px !important; top:  8px !important; width: 220px !important; height: 25px !important; z-index: 2147483647 !important; position: absolute !important; font-size: 14px; color: #1F80C8" href="http://support.epmlive.com" target="_blank">Visit our Support Community</a>');
            window.WalkMePlayerAPI.toggleMenu();

            event.stopPropagation();
        });

        $('body').click(function () {
            if ($('#walkme-menu-open').is(':visible')) {
                window.WalkMePlayerAPI.toggleMenu();
            }
        });
    };

    window.walkme_player_event = function(eventData) {
        if (eventData.Type === "AfterMenuOpen") {
            $('#epm-support-link').css('color', '#FFFFFF');
        }

        if (eventData.Type === "BeforeMenuClose") {
            $('#epm-support-link').css('color', '#00668E');
        }
    };

    var epmLiveTweaks = function () {
        $(function() {
            var $siteTitle = $('#site-title');
            var $pageTitle = $('#DeltaPlaceHolderPageTitleInTitleArea');

            if ($siteTitle.text().trim() === $pageTitle.text().trim()) {
                $siteTitle.addClass('epm-no-page-title');
                $pageTitle.hide();
            }
        });
        
        $(document).click(function(e) {
            if ($('#search-box-container').is(':visible')) {
                if ($(e.target).parents('#search-box-container').length === 0 && $(e.target).parents('#search-container').length === 0) {
                    toggleSearch();
                }
            }
        });

        var addUpdateProfilePicLink = function () {
            if (window.isIE8) {

                var insertAfter = function(referenceNode, newNode) {
                    referenceNode.parentNode.insertBefore(newNode, referenceNode.nextSibling.nextSibling);
                };

                var $menuBox = $('#welcomeMenuBox');
                
                if ($menuBox.length > 0) {
                    var $menuItems = $($('#welcomeMenuBox').get(0).getElementsByTagName('ie:menuitem'));
                    if ($menuItems.length > 0) {
                        insertAfter($menuItems.get(0), $('<ie:menuitem menugroupid="100" description="Update your profile photo." text="Update Profile Photo" onmenuclick="javascript:window.updateProfilePic();" type="option" id="epm-update-profile-pic" enabled="true" checked="false" onmenuclick_original="javascript:window.updateProfilePic();" text_original="Update Profile Photo" description_original="Update your profile photo." valorig=""></ie:menuitem>').get(0));
                    } else {
                        window.setTimeout(function() {
                            addUpdateProfilePicLink();
                        }, 1);
                    }
                } else {
                    window.setTimeout(function () {
                        addUpdateProfilePicLink();
                    }, 1);
                }
            } else {
                var $menuitems = $('#welcomeMenuBox').find('ie\\:menuitem');

                if ($menuitems.length > 0) {
                    $('<ie:menuitem menugroupid="100" description="Update your profile photo." text="Update Profile Photo" onmenuclick="javascript:window.updateProfilePic();" type="option" id="epm-update-profile-pic" enabled="true" checked="false" onmenuclick_original="javascript:window.updateProfilePic();" text_original="Update Profile Photo" description_original="Update your profile photo." valorig=""></ie:menuitem>').insertAfter('#' + $menuitems.get(0).id);
                } else {
                    window.setTimeout(function () {
                        addUpdateProfilePicLink();
                    }, 1);
                }
            }
        };

        var configureSearchBox = function() {
            var $sbtn = $('#search-container');
            var $sbox = $('#search-box-container');
            var $sinput = $($sbox.find('input').get(0));

            if ($sbtn.length === 0 || $sbox.length === 0) {
                window.setTimeout(function() {
                    configureSearchBox();
                }, 1);
            } else {
                if ($sbox.find('input').length !== 1) {
                    $sbtn.hide();
                    $sbox.hide();
                } else {
                    $sinput.attr('placeholder', 'Search...');
                }
            }
        };

        var configureTooltips = function() {
            if ($.isFunction($.fn.tooltip)) {
                var $el = $('[data-toggle="tooltip"]');
                if ($el.length > 0) {
                    window.setTimeout(function() {
                        $el.tooltip();
                        $el.hover(function() {
                            $(this).tooltip('show');
                        }, function() {
                            $(this).tooltip('hide');
                        });
                    }, 1000);
                } else {
                    window.setTimeout(function () {
                        configureTooltips();
                    }, 100);
                }
            } else {
                window.setTimeout(function() {
                    configureTooltips();
                }, 100);
            }
        };

        var monitorGridMessages = function() {
            try {
                Grids.OnDebug = function (grid, level, message) {
                    if (message[1].indexOf('from different TreeGrid version, it can cause problems or errors. Always use (or modify)') !== -1) {
                        var clearLog = function () {
                            var debugWindow = document.getElementById('_TreeGridDebug');

                            if (debugWindow) {
                                debugWindow.innerHTML = '';
                                debugWindow.style.display = 'none';

                                document.getElementById('_TreeGridDebugButtons').style.display = 'none';
                                Grids.DebugHidden = 1;

                                console.log(message[1]);
                            } else {
                                window.setTimeout(function () {
                                    clearLog();
                                }, 1);
                            }
                        };

                        clearLog();
                    }
                };
            } catch (e) {
            }
        };

        var configureEnterpriseSearch = function() {
            $(document).ready(function () {
                if (window.location.href.indexOf("osssearchresults.aspx") > -1) {

                    $('#DeltaPlaceHolderLeftNavBar').parent().show();
                    var s4Workspace = $(document.getElementById('s4-workspace'));

                    if (s4Workspace.hasClass('epm-nav-pinned')) {
                        $('#DeltaPlaceHolderLeftNavBar').parent().css('left', '230px');
                        s4Workspace.attr('style', s4Workspace.attr('style') + ' margin-left: 425px !important;');
                    }
                    else {
                        $('#DeltaPlaceHolderLeftNavBar').parent().css('left', '50px');
                        s4Workspace.attr('style', s4Workspace.attr('style') + ' margin-left: 245px !important;');
                    }
                }
            });
        };

        var configurePopMenuFromChevron = function() {
            var ori_popMenuFromChevron = PopMenuFromChevron;
            window.PopMenuFromChevron = function(event) {
                ori_popMenuFromChevron(event);

                var tries = 0;

                var moveMenu = function() {
                    if (++tries >= 20)return;

                    var $div = $('div.ms-core-menu-box');
                    if ($div.length) {
                        $div.css('left', 10);
                    } else {
                        window.setTimeout(moveMenu, 100);
                    }
                };

                window.setTimeout(function() {
                    moveMenu();
                }, 100);
            }
        };

        window.fixGridMenus = function() {
            window.setTimeout(function() {
                $('.js-callout-mainElement').each(function() {
                    var $menu = $(this);
                    
                    if ($menu.is(':visible')) {
                        var menuOffset = $menu.offset();
                        var leftOffset = $('#s4-workspace').offset().left;
                        
                        if (menuOffset.left < leftOffset) {
                            $menu.offset({ top: menuOffset.top, left: leftOffset + 12 });
                        }
                    }
                });
            }, 0);
        };

        configureSearchBox();
        addUpdateProfilePicLink();
        configureTooltips();
        configurePopMenuFromChevron();

        window.setTimeout(function() {
            monitorGridMessages();
        }, 2000);

        ExecuteOrDelayUntilScriptLoaded(configureEnterpriseSearch, 'EPMLive.Navigation.Initialized');
    };

    ExecuteOrDelayUntilScriptLoaded(epmLiveTweaks, 'jquery.min.js');
})();

/*
 * Toastr
 * Version 2.0.1
 * Copyright 2012 John Papa and Hans Fjällemark.  
 * All Rights Reserved.  
 * Use, reproduction, distribution, and modification of this code is subject to the terms and 
 * conditions of the MIT license, available at http://www.opensource.org/licenses/mit-license.php
 *
 * Author: John Papa and Hans Fjällemark
 * Project: https://github.com/CodeSeven/toastr
 */
; (function (define) {
    define(['jquery'], function ($) {
        return (function () {
            var version = '2.0.1';
            var $container;
            var listener;
            var toastId = 0;
            var toastType = {
                error: 'error',
                info: 'info',
                success: 'success',
                warning: 'warning'
            };

            var toastr = {
                clear: clear,
                error: error,
                getContainer: getContainer,
                info: info,
                options: {},
                subscribe: subscribe,
                success: success,
                version: version,
                warning: warning
            };

            return toastr;

            //#region Accessible Methods
            function error(message, title, optionsOverride) {
                return notify({
                    type: toastType.error,
                    iconClass: getOptions().iconClasses.error,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function info(message, title, optionsOverride) {
                return notify({
                    type: toastType.info,
                    iconClass: getOptions().iconClasses.info,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function subscribe(callback) {
                listener = callback;
            }

            function success(message, title, optionsOverride) {
                return notify({
                    type: toastType.success,
                    iconClass: getOptions().iconClasses.success,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function warning(message, title, optionsOverride) {
                return notify({
                    type: toastType.warning,
                    iconClass: getOptions().iconClasses.warning,
                    message: message,
                    optionsOverride: optionsOverride,
                    title: title
                });
            }

            function clear($toastElement) {
                var options = getOptions();
                if (!$container) { getContainer(options); }
                if ($toastElement && $(':focus', $toastElement).length === 0) {
                    $toastElement[options.hideMethod]({
                        duration: options.hideDuration,
                        easing: options.hideEasing,
                        complete: function () { removeToast($toastElement); }
                    });
                    return;
                }
                if ($container.children().length) {
                    $container[options.hideMethod]({
                        duration: options.hideDuration,
                        easing: options.hideEasing,
                        complete: function () { $container.remove(); }
                    });
                }
            }
            //#endregion

            //#region Internal Methods

            function getDefaults() {
                return {
                    tapToDismiss: true,
                    toastClass: 'toast',
                    containerId: 'toast-container',
                    debug: false,

                    showMethod: 'fadeIn', //fadeIn, slideDown, and show are built into jQuery
                    showDuration: 300,
                    showEasing: 'swing', //swing and linear are built into jQuery
                    onShown: undefined,
                    hideMethod: 'fadeOut',
                    hideDuration: 1000,
                    hideEasing: 'swing',
                    onHidden: undefined,

                    extendedTimeOut: 1000,
                    iconClasses: {
                        error: 'toast-error',
                        info: 'toast-info',
                        success: 'toast-success',
                        warning: 'toast-warning'
                    },
                    iconClass: 'toast-info',
                    positionClass: 'toast-top-right',
                    timeOut: 5000, // Set timeOut and extendedTimeout to 0 to make it sticky
                    titleClass: 'toast-title',
                    messageClass: 'toast-message',
                    target: 'body',
                    closeHtml: '<button>&times;</button>',
                    newestOnTop: true
                };
            }

            function publish(args) {
                if (!listener) {
                    return;
                }
                listener(args);
            }

            function notify(map) {
                var
                    options = getOptions(),
                    iconClass = map.iconClass || options.iconClass;

                if (typeof (map.optionsOverride) !== 'undefined') {
                    options = $.extend(options, map.optionsOverride);
                    iconClass = map.optionsOverride.iconClass || iconClass;
                }

                toastId++;

                $container = getContainer(options);
                var
                    intervalId = null,
                    $toastElement = $('<div/>'),
                    $titleElement = $('<div/>'),
                    $messageElement = $('<div/>'),
                    $closeElement = $(options.closeHtml),
                    response = {
                        toastId: toastId,
                        state: 'visible',
                        startTime: new Date(),
                        options: options,
                        map: map
                    };

                if (map.iconClass) {
                    $toastElement.addClass(options.toastClass).addClass(iconClass);
                }

                if (map.title) {
                    $titleElement.append(map.title).addClass(options.titleClass);
                    $toastElement.append($titleElement);
                }

                if (map.message) {
                    $messageElement.append(map.message).addClass(options.messageClass);
                    $toastElement.append($messageElement);
                }

                if (options.closeButton) {
                    $closeElement.addClass('toast-close-button');
                    $toastElement.prepend($closeElement);
                }

                $toastElement.hide();
                if (options.newestOnTop) {
                    $container.prepend($toastElement);
                } else {
                    $container.append($toastElement);
                }


                $toastElement[options.showMethod](
                    { duration: options.showDuration, easing: options.showEasing, complete: options.onShown }
                );
                if (options.timeOut > 0) {
                    intervalId = setTimeout(hideToast, options.timeOut);
                }

                $toastElement.hover(stickAround, delayedhideToast);
                if (!options.onclick && options.tapToDismiss) {
                    $toastElement.click(hideToast);
                }
                if (options.closeButton && $closeElement) {
                    $closeElement.click(function (event) {
                        event.stopPropagation();
                        hideToast(true);
                    });
                }

                if (options.onclick) {
                    $toastElement.click(function () {
                        options.onclick();
                        hideToast();
                    });
                }

                publish(response);

                if (options.debug && console) {
                    console.log(response);
                }

                return $toastElement;

                function hideToast(override) {
                    if ($(':focus', $toastElement).length && !override) {
                        return;
                    }
                    return $toastElement[options.hideMethod]({
                        duration: options.hideDuration,
                        easing: options.hideEasing,
                        complete: function () {
                            removeToast($toastElement);
                            if (options.onHidden) {
                                options.onHidden();
                            }
                            response.state = 'hidden';
                            response.endTime = new Date(),
                            publish(response);
                        }
                    });
                }

                function delayedhideToast() {
                    if (options.timeOut > 0 || options.extendedTimeOut > 0) {
                        intervalId = setTimeout(hideToast, options.extendedTimeOut);
                    }
                }

                function stickAround() {
                    clearTimeout(intervalId);
                    $toastElement.stop(true, true)[options.showMethod](
                        { duration: options.showDuration, easing: options.showEasing }
                    );
                }
            }
            function getContainer(options) {
                if (!options) { options = getOptions(); }
                $container = $('#' + options.containerId);
                if ($container.length) {
                    return $container;
                }
                $container = $('<div/>')
                    .attr('id', options.containerId)
                    .addClass(options.positionClass);
                $container.appendTo($(options.target));
                return $container;
            }

            function getOptions() {
                return $.extend({}, getDefaults(), toastr.options);
            }

            function removeToast($toastElement) {
                if (!$container) { $container = getContainer(); }
                if ($toastElement.is(':visible')) {
                    return;
                }
                $toastElement.remove();
                $toastElement = null;
                if ($container.children().length === 0) {
                    $container.remove();
                }
            }
            //#endregion

        })();
    });
}(typeof define === 'function' && define.amd ? define : function (deps, factory) {
    if (typeof module !== 'undefined' && module.exports) { //Node
        module.exports = factory(require(deps[0]));
    } else {
        window['toastr'] = factory(window['jQuery']);
    }
}));