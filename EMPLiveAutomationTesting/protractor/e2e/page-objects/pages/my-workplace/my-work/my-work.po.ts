import {By, element} from 'protractor';
import {MyWorkPageConstants} from './my-work-page.constants';
import {CommonPageHelper} from '../../common/common-page.helper';
import {ComponentHelpers} from '../../../../components/devfactory/component-helpers/component-helpers';
import {CommonPageConstants} from '../../common/common-page.constants';
import {HtmlHelper} from '../../../../components/misc-utils/html-helper';
import {AnchorHelper} from '../../../../components/html/anchor-helper';
import {ElementHelper} from '../../../../components/html/element-helper';

export class MyWorkPage {

    static get newItem() {
        return element(By.xpath(`//*[contains(@id,'${MyWorkPageConstants.title.newItem}')]
        //span[contains(@class,'ms-cui-ctl-largelabel')]`));
    }

    static get newItemMenu() {
        const labels = MyWorkPageConstants.title;
        return {
            changesItem: this.menuItem(labels.changesItem),
            issuesItem: this.menuItem(labels.issues),
            risksItem: this.menuItem(labels.risks),
            timeOffItem: this.menuItem(labels.timeOff),
            toDoItem: this.menuItem(labels.toDo),
        };
    }

    static get widowTitleName() {
        const labels = MyWorkPageConstants.pageName;
        return {
            changes: this.dialogWindowTitle(labels.changes),
            issues: this.dialogWindowTitle(labels.issues),
            risks: this.dialogWindowTitle(labels.risks),
            timeOff: this.dialogWindowTitle(labels.timeOff),
            toDo: this.dialogWindowTitle(labels.toDo),
        };
    }

    static get inputs() {
        const labels = MyWorkPageConstants.inputLabels;
        return {
            heading: element(By.id(labels.heading)),
            title: CommonPageHelper.getTextBoxByLabel(labels.title),
            project: CommonPageHelper.getFirstAutoCompleteByLabel(labels.project),
            assignedTo: this.getInputByTitle(labels.assignedTo),
            start: CommonPageHelper.getElementByTitle(labels.start),
            finish: CommonPageHelper.getElementByTitle(labels.finish),
            timeOffType: CommonPageHelper.getFirstAutoCompleteByLabel(labels.project),
            requestor: this.getInputByTitle(labels.requestor),
        };
    }

    static get dropdownAll() {
        const dropdownLabel = CommonPageConstants.dropdownShowAllButton;
        return {
            project: element(By.id(dropdownLabel.project)),
            timeOffType: element(By.id(dropdownLabel.timeOffType)),
            timeOffInput: this.selectDropdownOption(MyWorkPageConstants.inputDropdownValues.Holiday),
        };
    }

    static get editPageDropdown() {
        const idXpath = `//*[contains(@id,'${MyWorkPageConstants.edit}')]`;
        const anchorXpath = `//a[contains(@class,'${HtmlHelper.attributeValue.dropdown}')]`;
        const xpath = `${idXpath}${anchorXpath}`;
        return element(By.xpath(xpath));
    }

    static get disabledStopEditingOption() {
        const stopEditingXpath = `//a[contains(@id,'${MyWorkPageConstants.editPageActions.stopEditing}') and @aria-disabled="true"]`;
        return element(By.xpath(stopEditingXpath));
    }

    static get editPageMenuOption() {
        return AnchorHelper.getAnchorById(MyWorkPageConstants.editPageActions.editPage, true);
    }

    static get editPage() {
        return element(By.css('#DeltaPlaceHolderMain'));
    }

    static get selectedPageTab() {
        return element(By.xpath(`.//*[@aria-selected="true" and @title='${CommonPageConstants.ribbonMenuTitles.page}']`));
    }

    static get selectRibbonTabs() {
        const tabNames = MyWorkPageConstants.ribbonTabs;
        return {
            page: this.getLinkByTitle(tabNames.page),
            hide: this.getLinkByTitle(tabNames.hide),
            manage: this.getLinkByTitle(tabNames.manage),
            views: this.getLinkByTitle(tabNames.views)
        };
    }

    static get getViewRibbonOptions() {
        const viewRibbonlabel = MyWorkPageConstants.viewRibbonOptions;
        return {
            saveView: AnchorHelper.getAnchorById(viewRibbonlabel.saveView),
            renameView: AnchorHelper.getAnchorById(viewRibbonlabel.renameView),
            deleteView: AnchorHelper.getAnchorById(viewRibbonlabel.deleteView),
            currentViewDropdown: AnchorHelper.getAnchorById(viewRibbonlabel.currentViewDropdown),
            selectColumns: AnchorHelper.getAnchorById(viewRibbonlabel.selectColumns),
            currentView: ElementHelper.getElementByText(viewRibbonlabel.currentView),
            showFilters: AnchorHelper.getAnchorById(viewRibbonlabel.showFilters),
            removeSorting: AnchorHelper.getAnchorById(viewRibbonlabel.removeSorting),
            manageCurrentViewDropdown: AnchorHelper.getAnchorById(viewRibbonlabel.manageCurrentViewDropdown),
        };
    }

    static get viewsPopup() {
        const viewPopuplabel = MyWorkPageConstants.viewsPopUp;
        return {
            title: AnchorHelper.getItemById(viewPopuplabel.title),
            name: this.saveViewElements(viewPopuplabel.name),
            defaultView: this.saveViewElements(viewPopuplabel.defaultView),
            personalView: this.saveViewElements(viewPopuplabel.personalView),
            ok: this.saveViewElements(viewPopuplabel.ok),
            cancel: this.saveViewElements(viewPopuplabel.cancel),
            newName: this.saveViewElements(viewPopuplabel.newName),
        };
    }

    static get getCurrentView() {
        return element(By.css('.ms-cui-dd-text>a'));
    }

    static get stopEditing() {
        return element(By.xpath('//li[@id= "Ribbon.WebPartPage.Edit"]//a[1]'));
    }

    static get validationMessage() {
        return element(By.css('span[role="alert"]'));
    }

    static get buttonsOnPopup() {
        const buttonsNameLabel = MyWorkPageConstants.buttonsOnPopup;
        return {
            save: element(By.css(`input[value='${buttonsNameLabel.save}']`)),
            cancel: element(By.css(`td > input[value='${buttonsNameLabel.cancel}']`)),
        };
    }

    static get assignedToSuggestions() {
        return element(By.xpath('//div[contains(@id,"AssignedTo")]//li[1]'));
    }

    static get manageTabRibbonItems() {
        const manageTabRibbonItemsLabel = MyWorkPageConstants.manageTabRibbonItems;
        return {
            viewItem: AnchorHelper.getAnchorById(manageTabRibbonItemsLabel.viewItem),
            editItem: AnchorHelper.getAnchorById(manageTabRibbonItemsLabel.editItem),
            comments: AnchorHelper.getAnchorById(manageTabRibbonItemsLabel.comments),
        };
    }

    static get closeIconOnPopup() {
        return element(By.id('Ribbon.ListForm.Edit.Commit.Cancel-Large'));
    }

    static get commentsPopupDetails() {
        const commentsPopUpLabels = MyWorkPageConstants.commentsPopupDetails;
        const commentLinksSection = `table.customCommentItem .socialcomment-cmdlink>a`;
        const editCommentSection = `.commentsContainer .ms-socialCommentLoading`;
        return {
            cc: element(By.name(commentsPopUpLabels.cc)),
            commentTextArea: element(By.id(commentsPopUpLabels.commentTextArea)),
            post: element(By.id(commentsPopUpLabels.post)),
            edit: element.all(By.cssContainingText('td>span.socialcomment-cmdlink>a', MyWorkPageConstants.commentsPopupDetails.edit)).last(),
            delete: element(By.css(`${commentLinksSection}:last-child`)),
            editCommentTextArea: element(By.css('.commentsContainer [id*="socialCommentInputBox"]')),
            editPost: element(By.css(`${editCommentSection} [title="${commentsPopUpLabels.editPost}"]`)),
            editCancel: element(By.css(`${editCommentSection} [title="${commentsPopUpLabels.editCancel}"]`)),
        };
    }

    static get selectViewNameOtherThanDefault() {
        return element(By.xpath('//div[text()="Personal Views"]/following-sibling::ul[1]'));
    }

    static get secondItemInViewsDropdown() {
        return element.all(By.xpath(`//li[@class='ms-cui-menusection-items'][2]`)).get(0);
    }

    static get headerOptions() {
        const headerOptionLabels = MyWorkPageConstants.headerOptions;
        return {
            active: element(By.id(headerOptionLabels.active)),
            completed: element(By.id(headerOptionLabels.completed)),
            ellipses: element(By.id(headerOptionLabels.ellipses)),
        };
    }

    static get ellipsesDropdownForItem() {
        const ellipsesDropdownForItemLabels = MyWorkPageConstants.ellipsesDropdownForItem;
        return {
            viewItem: ElementHelper.getElementByText(ellipsesDropdownForItemLabels.viewItem),
            deleteItem: ElementHelper.getElementByText(ellipsesDropdownForItemLabels.deleteItem),
            editItem: ElementHelper.getElementByText(ellipsesDropdownForItemLabels.editItem)
        };
    }

    static get selectColumnsPopup() {
        const selectColumnsPopupLabel = CommonPageConstants.selectColumnsPopup;
        return {
            ok: ElementHelper.getElementByText(selectColumnsPopupLabel.ok),
            hideAll: ElementHelper.getElementByText(selectColumnsPopupLabel.hideAll),
            cancel: ElementHelper.getElementByText(selectColumnsPopupLabel.cancel),
            column: element.all(By.css(`.${selectColumnsPopupLabel.column}`)),
            columnChecked: element(By.css(`.${selectColumnsPopupLabel.columnChecked}`)),
            eachSelectedColumn: element(By.css(`.${selectColumnsPopupLabel.columnChecked}>div`)),
            showAll: ElementHelper.getElementByText(selectColumnsPopupLabel.showAll),
            columnUnchecked: element(By.css(`.${selectColumnsPopupLabel.columnUnchecked}`)),
            columnNameWithImages: element.all(By.css(`.${selectColumnsPopupLabel.column}>img`)),
            allSelectedColumn: element.all(By.css(`.${selectColumnsPopupLabel.columnChecked}>div`)),
        };
    }

    static get ellipsesDropdown() {
        const ellipsesDropdown = MyWorkPageConstants.ellipsesDropdown;
        return {
            dropDownSection: element(By.id(ellipsesDropdown.dropDownSection)),
            workTypes: element(By.xpath(`//div[@id='MWG_PivotMenu']//span[text()="${ellipsesDropdown.workTypes}"]`)),
            close: element(By.id(ellipsesDropdown.close)),
            workTypeSubmenuItem: element(By.css(`#${ellipsesDropdown.workTypeSubmenu}>li:first-child`)),
        };
    }

    static get gridDetails() {
        const gridLabels = CommonPageConstants.gridDetails;
        return {
            title: element.all(By.css(`.${gridLabels.title} a`)),
            workingOn: element.all(By.css(`.${gridLabels.workingOn}`)),
            workType: element.all(By.css(`.${gridLabels.workType}>div`)),
            editTitle: element(By.css(`.${gridLabels.editTitle}`)),
            toEdit: element.all(By.css(`.${gridLabels.title}.GMCell.GMRightHtml.HideCol0Title`)),
            filter: element(By.css(`.${gridLabels.filter}`)),
            workTypeFilter: element(By.css(`.${gridLabels.workType}.${gridLabels.filter}`)),
            workTypeHeader: element(By.css(`.${gridLabels.workType}.${gridLabels.header}`)),
            workTypeColumnHeader: (label: string) => element(By.cssContainingText(`td.${gridLabels.workType}`, label)),
            sorted: element(By.css(`.${gridLabels.workType} + td > u.${gridLabels.sorted}`)),
            scroll: element(By.css(`.${gridLabels.scroll}`)),
            workTypeValues: element.all(By.css(`.${gridLabels.workType}.${gridLabels.cell}>div`)),
        };
    }

    static get newlyCreatedView() {
        const workSummary = MyWorkPageConstants.workSummaryLabel;
        return element(By.xpath(`//span[text()="${workSummary}"]/ancestor::li[1]/following-sibling::li[1]//span[@class="ms-cui-ctl-mediumlabel"]`));
    }

    static menuItem(option: string) {
        return element(By.css(`[id*='${option}']`));
    }

    static dialogWindowTitle(title: string) {
        return CommonPageHelper.getElementByTitle(`${title}`);
    }

    static getInputByTitle(text: string) {
        return element(By.css(`input[title*='${text}']`));
    }

    static selectValueFromSuggestions(text: string) {
        return element(By.xpath(`//div[${ComponentHelpers.getXPathFunctionForText(text)}]`));
    }

    static selectDropdownOption(option: string) {
        return element(By.xpath(`//div[${ComponentHelpers.getXPathFunctionForText(option)}]`));
    }

    static getLinkByTitle(text: string) {
        return element(By.css(`[title='${text}']`));
    }

    static saveViewElements(idOrAny: string) {
        return element(By.xpath(`//div[@class='ms-dlgBorder']//input[@*="${idOrAny}"]`));
    }

    static getItemByName(itemName: string) {
        return element(By.xpath(`//td[contains(@class,"EPMLiveMyWorkTitle")] //a[text()="${itemName}"]`));
    }

    static getCommentByName(commentName: string) {
        return element(By.xpath(`//div[@class="socialcomment-contents-TRC"  and text()="${commentName}"]`));
    }

    static getCurrentViewByName(viewName: string) {
        return element(By.xpath(`//a[@role="button"] //span[text()="${viewName}"]`));
    }

    static getColumnSelectedOnSelectColumnsPopup(column: string) {
        const columnChecked = CommonPageConstants.selectColumnsPopup.columnChecked;
        return element(By.xpath(`//div[contains(@class, "${columnChecked}")]/div[text()='${column}']`));
    }

    static columnDisplayed(column: string) {
        return element(By.xpath(`//td[contains(@class,"GMHeaderText") and (text()='${column}')]`));
    }

    static getColumnByNameOnSelectColumnsPopup(columnName: string) {
        return element(By.xpath(`//div[contains(@class,'GMColumnsMenuItemText') and text()='${columnName}']`));
    }

    static get searchItem() {
        return element(By.id('MWG_Search'));
    }

    static get topBar() {
        return element(By.css('div.ms-cui-topBar2'));
    }

    static get editPageOptionInsideDropDown() {
        return element(By.cssContainingText('span.ms-cui-ctl-mediumlabel.ms-cui-btn-title', MyWorkPageConstants.editPage));
    }

    static itemCreated(item: string) {
        return element(By.xpath(`//a[contains(text(),'${item}')]//ancestor::tr[contains(@class,'GMDataRow')]`));
    }

    static ellipsisIconOfItem(item: string) {
        return element(By.xpath(`//a[contains(text(),'${item}')]//ancestor::tr//span[@class='epm-menu-btn']`));
    }
}