﻿function ULS_SP() {
    if (ULS_SP.caller) {
        ULS_SP.caller.ULSTeamName = "Windows SharePoint Services 4";
        ULS_SP.caller.ULSFileName = "/_layouts/epmlive/WEDispFormPageComponent.js";
    }
}


Type.registerNamespace('WEDispFormPageComponent');

// RibbonApp Page Component
WEDispFormPageComponent.PageComponent = function () {
    ULS_SP();
    WEDispFormPageComponent.PageComponent.initializeBase(this);
}


WEDispFormPageComponent.PageComponent.initialize = function (editform) {
    ULS_SP();
    ExecuteOrDelayUntilScriptLoaded(Function.createDelegate(null,
    WEDispFormPageComponent.PageComponent.initializePageComponent), 'SP.Ribbon.js');
}


WEDispFormPageComponent.PageComponent.initializePageComponent = function () {
    ULS_SP();
    var ribbonPageManager = SP.Ribbon.PageManager.get_instance();
    if (null !== ribbonPageManager) {
        ribbonPageManager.addPageComponent(WEDispFormPageComponent.PageComponent.instance);
        ribbonPageManager.get_focusManager().requestFocusForComponent(
    WEDispFormPageComponent.PageComponent.instance);
    }
}


WEDispFormPageComponent.PageComponent.refreshRibbonStatus = function () {
    SP.Ribbon.PageManager.get_instance().get_commandDispatcher().executeCommand(
  Commands.CommandIds.ApplicationStateChanged, null);
}


WEDispFormPageComponent.PageComponent.prototype = {
    $Z1: "",
    $Z2: "",
    getFocusedCommands: function () {
        ULS_SP();
        return [];
    },
    getGlobalCommands: function () {
        ULS_SP();
        var $arr = [];


        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EditItem2');
        Array.add($arr, 'Ribbon.ListForm.Display.Associated.LinkedItems');
        Array.add($arr, 'Ribbon.ListForm.Display.Associated.LinkedItemsButton');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EPMLivePlanner');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.TaskPlanner');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.CreateWorkspace');
        Array.add($arr, 'Ribbon.ListForm.Display.Actions.Favorite');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.GoToWorkspace');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.BuildTeam');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EPKCost');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EPKRP');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EPKRPM');
        Array.add($arr, 'Ribbon.ListForm.Display.Manage.EPMINT');
        Array.add($arr, 'Ribbon.ListForm.Edit.Actions.ArchiveProject');
        Array.add($arr, 'Ribbon.ListForm.Edit.Actions.RestoreProject');
        return $arr;
    },
    isFocusable: function () {
        ULS_SP();
        return true;
    },
    receiveFocus: function () {
        ULS_SP();
        return true;
    },
    yieldFocus: function () {
        ULS_SP();
        return true;
    },
    canHandleCommand: function (commandId) {
        ULS_SP();

        try {
            this.updateRibbonLinks(commandId);
        } catch (e) { if (window.console) window.console.log(e); };
        
        switch (commandId) {
            case "Ribbon.ListForm.Display.Manage.EditItem2":
            case "Ribbon.ListForm.Display.Associated.LinkedItems":
            case "Ribbon.ListForm.Display.Associated.LinkedItemsButton":
            case "Ribbon.ListForm.Display.Manage.EPMLivePlanner":
            case "Ribbon.ListForm.Display.Manage.TaskPlanner":
            case "Ribbon.ListForm.Display.Manage.CreateWorkspace":
            case "Ribbon.ListForm.Display.Actions.Favorite":
            case "Ribbon.ListForm.Display.Manage.GoToWorkspace":
            case "Ribbon.ListForm.Display.Manage.BuildTeam":
            case "Ribbon.ListForm.Display.Manage.EPKCost":
            case "Ribbon.ListForm.Display.Manage.EPKRP":
            case "Ribbon.ListForm.Display.Manage.EPKRPM":
            case "Ribbon.ListForm.Display.Manage.EPMINT":
            case 'Ribbon.ListForm.Edit.Actions.ArchiveProject':
            case 'Ribbon.ListForm.Edit.Actions.RestoreProject':
                return true;
            default:
                return commandEnabled(commandId);
        };

       
    },

    handleCommand: function (commandId, properties, sequence) {
        ULS_SP();
        if (commandId === 'Ribbon.ListForm.Display.Manage.EditItem2') {
            var weburl = "";
            if (WEExtraParams != "")
                weburl = WEEditForm + "?ID=" + WEItemId + "&" + WEExtraParams + "&Source=" + WESource;
            else
                weburl = WEEditForm + "?ID=" + WEItemId + "&Source=" + WESource;

            if (WEDLG == "1") {
                var options = { url: weburl, width: 700, dialogReturnValueCallback: this.NewItemCallback };
                SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
            }
            else {
                location.href = weburl;
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Associated.LinkedItemsButton') {

            //sm("dlgPosting", 150, 50);

            //var listInfo = properties['CommandValueId'].split('.');

            var listInfo = properties.SourceControlId.split('.');

            dhtmlxAjax.post(WEWebUrl + "/_layouts/epmlive/gridaction.aspx?action=linkeditemspost", "listid=" + WEListId + "&lookups=" + escape(WETitle) + "&lookupid=" + WEItemId + "&field=" + listInfo[5] + "&LookupFieldList=" + listInfo[4], this.LinkedItemsButtonPost);

        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.TaskPlanner') {
            try {

                var weburl = WEWebUrl + "/_layouts/epmlive/gridaction.aspx?action=GoToTaskPlanner&webid=" + WEWebId + "&listid=" + WEListId + "&id=" + WEItemId + "&Source=" + WESource;

                if (document.location.href.toLowerCase().indexOf("&isdlg=1") > 0) {
                    window.open(weburl, '', config = 'width=' + screen.width + ',height=' + screen.height + ',top=0,left=0')
                }
                else
                    location.href = weburl;
            } catch (e) { }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.EPMLivePlanner') {

            try {

                var weburl = WEWebUrl + "/_layouts/epmlive/workplanner.aspx?listid=" + WEListId + "&id=" + WEItemId + "&Source=" + WESource;
                if (document.location.href.toLowerCase().indexOf("&isdlg=1") > 0) {
                    window.open(weburl, '', config = 'width=' + screen.width + ',height=' + screen.height + ',top=0,left=0')
                }
                else
                    location.href = weburl;

            } catch (e) { }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.CreateWorkspace') {
            CreateEPMLiveWorkspace(WEListId, WEItemId);
        }
        else if (commandId === 'Ribbon.ListForm.Display.Actions.Favorite') {
            if (!($('a[id="Ribbon.ListItem.EPMLive.FavoriteStatus-Large"]').find('img').attr('src') === '_layouts/epmlive/images/star-filled32.png')) {

                var viewDiv = document.createElement('div');
                viewDiv.innerHTML = '<div>' +
                                        '<div style="width: 250px; padding: 5px;"> Title:&nbsp;' +
                                            '<input type="text" value="' + epmLive.currentItemTitle + '" name="favItemTitle" id="favItemTitle" style="width:200px;">' +
                                            '<br>' +
                                            '<div style="clear: both; height: 20px;"></div>' +
                                            '<div style="margin-left: 45px;">' +
                                                '<input type="button" style="float: left; margin-right: 5px; width: 90px;" value="OK" onclick="$(\'#favItemTitle\').blur();SP.UI.ModalDialog.commonModalDialogClose(1, window.Analytics.getAddFavItemFromGridDynamicValue(this));" class="ms-ButtonHeightWidth" target="_self">' +
                                                '<input type="button" style="float:left;width:90px;" value="Cancel" onclick="SP.UI.ModalDialog.commonModalDialogClose(0, \'Cancel clicked\');" class="ms-ButtonHeightWidth" target="_self">' +
                                            '</div>' +
                                        '</div>' +
                                    '</div>';

                var options = {
                    html: viewDiv,
                    height: 90,
                    width: 250,
                    title: "Add Favorite Item",
                    dialogReturnValueCallback: function (diagResult, retVal) {
                        if (diagResult === 1) {
                            window.Analytics.addItemFav(retVal);
                        }
                    }
                };

                SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);

                //$(function () {
                //    var myVar = setTimeout(function () { setFocus(); }, 100);
                //    function setFocus() {
                //        if ($("#favItemTitle").length > 0) {
                //            $("#favItemTitle").focus().val(epmLive.currentItemTitle);
                //            clearInterval(myVar);
                //        }
                //        else {
                //            setTimeout(function (uniqueId) { setFocus(); }, 100);
                //        }
                //    }
                //});

            } else {
                window.Analytics.removeItemFav();
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.GoToWorkspace') {
            if (parent) {
                parent.location.href = WEWebUrl + "/_layouts/epmlive/gridaction.aspx?action=workspace&webid=" + WEWebId + "&listid=" + WEListId + "&id=" + WEItemId;
            }
            else {
                location.href = WEWebUrl + "/_layouts/epmlive/gridaction.aspx?action=workspace&webid=" + WEWebId + "&listid=" + WEListId + "&id=" + WEItemId;
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.BuildTeam') {
            var isSecRunning = window.epmLiveNavigation.isSecurityJobRunning(WEWebUrl, WEListId, WEItemId);
            if (isSecRunning) {
                alert("The team cannot be edited because the security queue job has not completed. This should be completed in less than a minute or so - please try again.");
            }
            else {
                var options = { url: WEWebUrl + "/_layouts/epmlive/buildteam.aspx?listid=" + WEListId + "&id=" + WEItemId, title: "Build Team", showMaximized: true, showClose: false, dialogReturnValueCallback: this.NewItemCallback };

                SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.EPKCost') {
            var FullId = WEWebId + "." + WEListId + "." + WEItemId;

            weburl = WEWebUrl + "/_layouts/ppm/costs.aspx?itemid=" + FullId + "&listid=" + WEListId + "&view=";

            var options = { url: weburl, showMaximized: true, showClose: false, dialogReturnValueCallback: this.NewItemCallback };

            SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.EPKRP') {
            var isResImportRunning = window.epmLiveNavigation.isImportResourceRunning();
            if (!isResImportRunning) {
                var FullId = WEWebId + "." + WEListId + "." + WEItemId;

                weburl = WEWebUrl + "/_layouts/ppm/rpeditor.aspx?itemid=" + FullId;

                var options = { url: weburl, showMaximized: true, showClose: false, dialogReturnValueCallback: this.NewItemCallback };

                SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
            }
            else {
                alert('The Resource Planner cannot be opened because there is an active resource import job running.');
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.EPKRPM') {
            var isResImportRunning = window.epmLiveNavigation.isImportResourceRunning();
            if (!isResImportRunning) {
                this.epkmulti('rpeditor');
            }
            else {
                alert('The Resource Planner cannot be opened because there is an active resource import job running.');
            }
        }
        else if (commandId === 'Ribbon.ListForm.Display.Manage.EPMINT') {
            OpenIntegrationPage(properties.SourceControlId.replace("EPMINT.", ""), WEListId, WEItemId);

        }
        else if (commandId === 'Ribbon.ListForm.Edit.Actions.ArchiveProject') {
            var action = "archiveproject";
            var message = "Are you sure you want to archive the project?";
            var waitMessage = "Moving project and all related records to archive. Please wait...";
            this.ArchiveRestoreProject(action, message, waitMessage);
        }
        else if (commandId === 'Ribbon.ListForm.Edit.Actions.RestoreProject') {
            var action = "restoreproject";
            var message = "Are you sure you want to restore the project?";
            var waitMessage = "Restoring project and all related records from archive. Please wait...";
            this.ArchiveRestoreProject(action, message, waitMessage);
        }
        else {
            return handleCommand(commandId, properties, sequence);
        }
    },
    NewItemCallback: function (dialogResult, returnValue) {
        if (dialogResult) {
            window.location.href = window.location.href;
        }
    },
    LinkedItemsButtonPost: function (ret) {
        //hm("dlgPosting");

        var ticket = ret.xmlDoc.ResponseText;

        if (ticket == undefined)
            ticket = ret.xmlDoc.responseText;

        if (ticket.indexOf("General Error") != 0) {
            var listInfo = ticket.split('|');

            var weburl = listInfo[0] + "/_layouts/epmlive/gridaction.aspx?action=linkeditems&list=" + listInfo[3] + "&field=" + listInfo[1] + "&LookupFieldList=" + listInfo[2] + "&Source=" + escape(document.location.href);
            var options = { url: weburl, showMaximized: true };

            SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
        }
        else {
            alert(ticket);
        }
    },

    ArchiveRestoreProject: function (action, message, waitMessage) {
        var callback = this.ArhiveRestoreProjectPost;
        var waitDialog = this.ShowWaitActionDialog;
        this.ShowConfirmActionDialog(message,
            function (confirm) {
                if (confirm) {
                    var url = WEWebUrl +
                        "/_layouts/epmlive/gridaction.aspx?action=" +
                        action +
                        "&" +
                        "listid=" +
                        WEListId +
                        "&id=" +
                        escape(WEItemId);
                    waitDialog(waitMessage);
                    jQuery.post(url, callback);
                }
            });
    },

    ArhiveRestoreProjectPost: function (response) {
        // close wait dialog
        SP.UI.ModalDialog.commonModalDialogClose(1, 'Done');

        // process response
        if (response) {
            if (response.indexOf("General Error") >= 0) {
                alert(response);
            }
            window.location.href = window.location.href;
        } else {
            alert("No response received");
        }
    },

    ShowConfirmActionDialog: function (message, callback) {
        var viewDiv = document.createElement('div');
        viewDiv.innerHTML = '<div>' +
            '<div style="width: 290px; padding: 0px;">' +
            '<p id="askYesNoText">' + message + '</p>' +
            '<div style="clear: both; height: 20px;"></div>' +
            '<div style="margin-left: 85px;">' +
            '<input type="button" style="float: left; margin-right: 5px; width: 90px;" value="OK" onclick="$(\'#askYesNoText\').blur();SP.UI.ModalDialog.commonModalDialogClose(1, window.Analytics.getAddFavItemFromGridDynamicValue(this));" class="ms-ButtonHeightWidth" target="_self">' +
            '<input type="button" style="float:left;width:90px;" value="Cancel" onclick="SP.UI.ModalDialog.commonModalDialogClose(0, \'Cancel clicked\');" class="ms-ButtonHeightWidth" target="_self">' +
            '</div>' +
            '</div>' +
            '</div>';

        var options = {
            html: viewDiv,
            height: 110,
            width: 290,
            title: epmLive.currentItemTitle,
            dialogReturnValueCallback: function (diagResult, retVal) {
                callback(diagResult === 1);
            }
        };

        SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
    },

    ShowWaitActionDialog: function (message) {
        var viewDiv = document.createElement('div');
        viewDiv.innerHTML = '<div>' +
            '<div style="width: 290px; padding: 0px;">' +
            '<p id="askYesNoText">' + message + '</p>' +
            '<div style="clear: both; height: 20px;"></div>' +
            '</div>' +
            '</div>';

        var options = {
            html: viewDiv,
            height: 110,
            width: 290,
            showClose: false,
            title: epmLive.currentItemTitle
        };

        SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
    },

    epkmulti: function (epkcontrol) {

        if (weburl == "/")
            weburl = "";

        this.$Z1 = epkcontrol

        dhtmlxAjax.post(WEWebUrl + "/_layouts/ppm/gridaction.aspx?action=postepkmultipage", "IDs=" + WEWebId + "." + WEListId + "." + WEItemId, this.epkmultiresponse);

    },
    epkmultiresponse: function (ret) {
        var ticket = ret.xmlDoc.ResponseText;

        if (ticket == undefined)
            ticket = ret.xmlDoc.responseText;

        var weburl = WEWebUrl + "/_layouts/ppm/gridaction.aspx?action=epkmultipage&ticket=" + ticket + "&epkcontrol=" + this.$Z1 + "&view=&listid=" + WEListId;

        function myCallback(dialogResult, returnValue) { }

        var options = { url: weburl, showMaximized: true, showClose: false, dialogReturnValueCallback: myCallback };

        SP.SOD.execute('SP.UI.Dialog.js', 'SP.UI.ModalDialog.showModalDialog', options);
    },
    updateRibbonLinks: function (commandId) {
        var weburl = "";
        var supportedCommands = {
            "Ribbon.ListForm.Display.Manage.EPMLivePlanner": { id: "Ribbon.ListItem.EPMLive.Planner", type: 'Large' },
            "Ribbon.ListForm.Display.Manage.EPKCost": { id: "Ribbon.ListItem.Manage.EPKCosts", type: 'Large' },
            "Ribbon.ListForm.Display.Manage.EPKRP": { id: "Ribbon.ListItem.Manage.EPKResourcePlanner", type: 'Large' },
            "Ribbon.ListForm.Display.Manage.BuildTeam": { id: "Ribbon.ListItem.EPMLive.BuildTeam", type: 'Medium' },
            "Ribbon.ListForm.Display.Manage.EditItem2": { id: "Ribbon.ListForm.Display.Manage.EditItem2", type: 'Large' },
        };
        
        var command = supportedCommands[commandId].id;
        var buttontype = supportedCommands[commandId].type;
        if (command === 'Ribbon.ListForm.Display.Manage.EditItem2') {
            var weburl = "";
            if (WEExtraParams != "")
                weburl = WEEditForm + "?ID=" + WEItemId + "&" + WEExtraParams + "&Source=" + WESource;
            else
                weburl = WEEditForm + "?ID=" + WEItemId + "&Source=" + WESource + "&isdlg=1";
        }
        else if (command === 'Ribbon.ListItem.EPMLive.Planner') {
            try {
                weburl = WEWebUrl + "/_layouts/epmlive/workplanner.aspx?listid=" + WEListId + "&id=" + WEItemId + "&Source=" + WESource + "&isdlg=1";
            } catch (e) { }
        }
        else if (command === 'Ribbon.ListItem.EPMLive.BuildTeam') {
            weburl = WEWebUrl + "/_layouts/epmlive/buildteam.aspx?listid=" + WEListId + "&id=" + WEItemId + "&isdlg=1";
        }
        else if (command === 'Ribbon.ListItem.Manage.EPKCosts') {
            var FullId = WEWebId + "." + WEListId + "." + WEItemId;
            weburl = WEWebUrl + "/_layouts/ppm/costs.aspx?itemid=" + FullId + "&listid=" + WEListId + "&view=" + "&isdlg=1";
        }
        else if (command === 'Ribbon.ListItem.Manage.EPKResourcePlanner') {
            var FullId = WEWebId + "." + WEListId + "." + WEItemId;

            weburl = WEWebUrl + "/_layouts/ppm/rpeditor.aspx?itemid=" + FullId + "&isdlg=1";
        }
        if (weburl != "") {
            this.updateRibbonLink(command, weburl, buttontype);
        }
    },
    updateRibbonLink: function (controlId, weburl, buttontype) {
        var buttonId = controlId + '-' + buttontype;
        var hrefValue = 'javascript:;';

        if (weburl) {
            hrefValue = weburl;
        };

        var element = document.getElementById(buttonId);
        if (element) {
            element.setAttribute('href', hrefValue);
        }
    },

}


// Register classes
WEDispFormPageComponent.PageComponent.registerClass('WEDispFormPageComponent.PageComponent',
    CUI.Page.PageComponent);
WEDispFormPageComponent.PageComponent.instance = new WEDispFormPageComponent.PageComponent();


// Notify waiting jobs
NotifyScriptLoadedAndExecuteWaitingJobs("/_layouts/epmlive/WEDispFormPageComponent.js");