﻿function initializeAnalytics() {
    (function (a, $$, $) {

        //====== FAVORITES ========================
        var favoritesData =
            "<Data>" +
                "<Param key=\"SiteId\">" + $$.currentSiteId + "</Param>" +
                "<Param key=\"WebId\">" + $$.currentWebId + "</Param>" +
                "<Param key=\"ListId\">" + $$.currentListId + "</Param>" +
                "<Param key=\"ListIconClass\">" + $$.currentListIcon + "</Param>" +
                "<Param key=\"ItemId\">" + $$.currentItemID + "</Param>" +
                "<Param key=\"FileIsNull\">" + $$.currentFileIsNull + "</Param>" +
                "<Param key=\"FString\">" + $$.currentUrl + "</Param>" +
                "<Param key=\"Type\">1</Param>" +
                "<Param key=\"UserId\">" + $$.currentUserId + "</Param>" +
                "<Param key=\"PageTitle\">" + $$.pageName + "</Param>" +
                "</Data>";
        function loadFavoriteStatus() {
            $.ajax({
                type: 'POST',
                url: epmLive.currentWebFullUrl + '/_vti_bin/WorkEngine.asmx/Execute',
                data: "{ Function: 'LoadFavoriteStatus', Dataxml: '" + favoritesData + "' }",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d) {
                        var resp = $$.parseJson(response.d);
                        var result = resp.Result;
                        if ($$.responseIsSuccess(result) && result['#text'] === 'false') {
                            $('#favoritesStar').fadeIn(1000);
                        } else if ($$.responseIsSuccess(result) && result['#text'] === 'true') {
                            $('#favoritesStar').addClass('icon-star-active');
                            $('#favoritesStar').fadeIn(1000);
                        }


                        $("#favoritesStar").click(function () {
                            if (!$(this).hasClass('icon-star-active')) {
                                var viewDiv = document.createElement('div');
                                viewDiv.innerHTML = document.getElementById('fav_Add_DivTemp').innerHTML;

                                var options = {
                                    html: viewDiv,
                                    height: 110,
                                    width: 265,
                                    title: "Add Favorite",
                                    dialogReturnValueCallback: function (diagResult, retVal) {
                                        if (diagResult === 1) {
                                            add(retVal);
                                        }
                                    }
                                };

                                SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
                            } else {
                                $.ajax({
                                    type: 'POST',
                                    url: epmLive.currentWebFullUrl + '/_vti_bin/WorkEngine.asmx/Execute',
                                    data: "{ Function: 'RemoveFavorites', Dataxml: '" + favoritesData + "' }",
                                    contentType: 'application/json; charset=utf-8',
                                    dataType: 'json',
                                    success: function (response) {
                                        if (response.d) {
                                            var resp = epmLive.parseJson(response.d);
                                            var result = resp.Result;

                                            if (epmLive.responseIsSuccess(result) && result['#text']) {
                                                $('#favoritesStar').removeClass('icon-star-active');
                                                SP.UI.Notify.addNotification('Favorite removed', false);
                                                var sa = result['#text'].split(',');
                                                // asynchronously update nav
                                                window.epmLiveNavigation.removeLink({
                                                    kind: 0, // 0 - FA, 1 - RI, 2 - FW, 3 - WS
                                                    id: sa[0],
                                                    //webId: use webid for type 3 removal,
                                                });

                                            } else {
                                                //onError(response);
                                            }
                                        } else {
                                            //onError(response);
                                        }
                                    },
                                    error: function (response) {
                                        //onError(response);
                                    }
                                });
                            }

                        });
                    }
                },
                error: function (response) {
                    //onError(response);
                }
            });
        }
        function add(title) {
            $.ajax({
                type: 'POST',
                url: epmLive.currentWebFullUrl + '/_vti_bin/WorkEngine.asmx/Execute',
                data: "{ Function: 'AddFavorites', Dataxml: '" +
                    "<Data>" +
                    "<Param key=\"SiteId\">" + $$.currentSiteId + "</Param>" +
                    "<Param key=\"WebId\">" + $$.currentWebId + "</Param>" +
                    "<Param key=\"ListId\">" + $$.currentListId + "</Param>" +
                    "<Param key=\"ListIconClass\">" + $$.currentListIcon + "</Param>" +
                    "<Param key=\"ItemId\">" + $$.currentItemID + "</Param>" +
                    "<Param key=\"FString\">" + $$.currentUrl + "</Param>" +
                    "<Param key=\"Type\">1</Param>" +
                    "<Param key=\"UserId\">" + $$.currentUserId + "</Param>" +
                    "<Param key=\"Title\">" + title + "</Param>" +
                    "</Data>' }",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function(response) {
                    if (response.d) {
                        var resp = epmLive.parseJson(response.d);
                        var result = resp.Result;

                        if (epmLive.responseIsSuccess(result) && result['#text']) {
                            //onSuccess(result);
                            $('#favoritesStar').addClass('icon-star-active');
                            SP.UI.Notify.addNotification('New favorite added', false);
                            var sa = result['#text'].split(',');
                            // asynchronously update nav
                            window.epmLiveNavigation.registerLink({
                                kind: 0, // 0 - FA, 1 - RI, 2 - FW, 3 - WS
                                id: sa[0],
                                title: sa[6],
                                url: sa[9],
                                cssClass: sa[7],
                                siteId: sa[1],
                                webId: sa[2],
                                listId: sa[3],
                                itemId: sa[4],
                                external: false 
                            });
                        } else {
                            //onError(response);
                        }
                    } else {
                        //onError(response);
                    }
                },
                error: function(response) {
                    //onError(response);
                }
            });
        }
        loadFavoriteStatus();
        //====== FREQUENT ======================
        var frequentData =
            "<Data>" +
                "<Param key=\"SiteId\">" + $$.currentSiteId + "</Param>" +
                "<Param key=\"WebId\">" + $$.currentWebId + "</Param>" +
                "<Param key=\"ListId\">" + $$.currentListId + "</Param>" +
                "<Param key=\"ListIconClass\">" + $$.currentListIcon + "</Param>" +
                "<Param key=\"ListTitle\">" + $$.currentListTitle + "</Param>" +
                "<Param key=\"Type\">3</Param>" +
                "<Param key=\"UserId\">" + $$.currentUserId + "</Param>" +
            "</Data>";
        
        function countFrequentApps() {
            $.ajax({
                type: 'POST',
                url: epmLive.currentWebFullUrl + '/_vti_bin/WorkEngine.asmx/Execute',
                data: "{ Function: 'LoadFavoriteStatus', Dataxml: '" + frequentData + "' }",
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (response) {
                    if (response.d) {
                        var resp = $$.parseJson(response.d);
                        var result = resp.Result;
                        if ($$.responseIsSuccess(result) && result['#text'] === 'false') {
                            $('#favoritesStar').fadeIn(1000);
                        } else if ($$.responseIsSuccess(result) && result['#text'] === 'true') {
                            $('#favoritesStar').addClass('icon-star-active');
                            $('#favoritesStar').fadeIn(1000);
                        }
                        
                        
                    }
                },
                error: function (response) {
                    //onError(response);
                }
            });
        }
        
        countFrequentApps();
        
        //====== RECENT ========================

        //====== HELPER FUNCTIONS =============

        var turnOnFav = function() {
            if (!$('#favoritesStar').hasClass('icon-star-active')) {
                $('#favoritesStar').addClass('icon-star-active');
            }
        };

        var turnOffFav = function() {
            if ($('#favoritesStar').hasClass('icon-star-active')) {
                $('#favoritesStar').removeClass('icon-star-active');
            }
        };

    })(window.Analytics = window.Analytics || {}, window.epmLive, window.jQuery);
}
ExecuteOrDelayUntilScriptLoaded(initializeAnalytics, 'EPMLive.js');