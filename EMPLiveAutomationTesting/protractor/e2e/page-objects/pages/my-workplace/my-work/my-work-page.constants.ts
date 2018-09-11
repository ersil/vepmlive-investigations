import {CommonPageConstants} from '../../common/common-page.constants';

export class MyWorkPageConstants {

    static readonly pagePrefix = 'My Work';
    static readonly editPageName = `${MyWorkPageConstants.pagePrefix}${CommonPageConstants.pagePostFix.editItem}`;
    static readonly edit = 'Edit.Edit';
    static readonly viewName = 'View';
    static readonly renameView = 'rView';
    static readonly currentView = 'Current View';
    static readonly manage = 'Manage tab';
    static readonly newItemPopupLabel = 'New Item popup';
    static readonly toDoNewItemPopupLabel = 'ToDo New Item popup';
    static readonly blankValidationMessage = 'You can\'t leave this blank.';
    static readonly validationMessageLabel = 'Validation Message';
    static readonly newItemListLabel = 'New Items list';
    static readonly adMembersLabel = 'admembers:';
    static readonly editItemLabel = 'Edit Item';
    static readonly commentsLabel = 'Comments';
    static readonly editCommentsLabel = 'Edit Comments';
    static readonly viewsRibbonLabel = 'Views Ribbon menu';
    static readonly saveViewPopupLabel = 'Save view popup';
    static readonly viewNameOtherThanDefault = 'Working On It';
    static readonly renameViewPopupLabel = 'Rename View Popup';
    static readonly noNameMessage = 'Please enter view name - view names cannot be blank.';

    static get pageName() {
        return {
            changes: `Changes${CommonPageConstants.pagePostFix.newItem}`,
            issues: `Issues${CommonPageConstants.pagePostFix.newItem}`,
            risks: `Risks${CommonPageConstants.pagePostFix.newItem}`,
            timeOff: `Time Off${CommonPageConstants.pagePostFix.newItem}`,
            toDo: `To Do${CommonPageConstants.pagePostFix.newItem}`,
        };
    }

    static get title() {
        return {
            newItem: 'NewItem',
            changesItem: 'NewItem.Changes',
            changes: 'Changes',
            issues: 'Issues',
            risks: 'Risks',
            timeOff: 'Time Off',
            toDo: 'To Do',
        };
    }

    static get inputLabels() {
        return {
            heading: 'dialogTitleSpan',
            title: 'Title *',
            project: 'Project *',
            assignedTo: 'Assigned To',
            start: 'Start Required Field',
            finish: 'Finish Required Field',
            timeOffType: 'Time Off Type *',
            requestor: 'Requestor',
        };
    }

    static get inputDropdownValues() {
        return {
            Holiday: 'Holiday',
            juryDuty: 'Jury Duty',
            sick: 'Sick',
            vacation: 'Vacation',
            doctorAppointment: 'Doctor Appointment',
        };
    }

    static get editPageActions() {
        return {
            stopEditing: 'Actions.Stop',
            editPage: 'Actions.Edit-Menu',
        };
    }

    static get ribbonTabs() {
        return{
            page: 'Page',
            hide: 'Hide',
            manage: 'Manage Work',
            views: 'Manage Views'
        };
    }

    static get viewRibbonOptions() {
        const viewSection = 'Ribbon.MyWork.Views';
        return{
            saveView: `${viewSection}.SaveView-Medium`,
            renameView: `${viewSection}.RenameView-Medium`,
            deleteView: `${viewSection}.DeleteView-Medium`,
            currentViewDropdown: `${viewSection}.CurrentViewDropDown`,
            selectColumns: `Ribbon.MyWork.Actions.SelectColoumns-Medium`
        };
    }

    static get viewsPopUp() {
        return{
            title: 'dialogTitleSpan',
            name: 'MWG_ViewName',
            defaultView: 'MWG_DefaultView',
            personalView: 'MWG_PersonalView',
            ok: 'OK',
            cancel: 'Cancel',
            newName: 'MWG_ViewNewName'
        };
    }

    static get newItemTypeLabels() {
        return {
            changes: `Changes`,
            issues: `Issues`,
            risks: `Risks`,
            timeOff: `Time Off`,
            toDo: `To Do`,
        };
    }

    static get buttonsOnPopup() {
        return {
            save: 'Save',
            cancel: 'Cancel'
        };
    }

    static get manageTabRibbonItems() {
        const manageTabRibbon = 'Ribbon.MyWork.Manage.';
        return {
            viewItem: `${manageTabRibbon}ViewItem-Large`,
            editItem: `${manageTabRibbon}EditItem-Large`,
            comments: `${manageTabRibbon}Comments-Large`,
        };
    }

    static get commentsPopupDetails() {
        return{
            cc: 'upLevelDiv',
            commentTextArea : 'tbCommentInput',
            post: 'postBtn',
            edit: 'Edit',
            delete: 'Delete',
            editCommentTextArea: 'socialCommentInputBox',
            editPost: 'Post',
            editCancel: 'Cancel',
        };
    }

    static get selectColumnsPopup() {
        return {
            ok: 'OK',
            hideAll: 'Hide all',
            cancel: 'Cancel',
            column: 'GMColumnsMenuItemText',
            columnChecked: 'GMMenuCheckedIconRight'
        };
    }
}
