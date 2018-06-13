import {StepLogger} from '../../../../../core/logger/step-logger';
import {TextboxHelper} from '../../../../components/html/textbox-helper';
import {PageHelper} from '../../../../components/html/page-helper';
import {CommonPage} from '../../common/common.po';
import {CommonPageConstants} from '../../common/common-page.constants';
import {EventsPageConstants} from './events-page.constants';
import {EventsPage} from './events.po';
import {ValidationsHelper} from '../../../../components/misc-utils/validation-helper';
import {CommonPageHelper} from '../../common/common-page.helper';
import {MyWorkplacePage} from '../my-workplace.po';
import {WaitHelper} from '../../../../components/html/wait-helper';
import {browser} from 'protractor';
import {ElementHelper} from '../../../../components/html/element-helper';
import {CheckboxHelper} from '../../../../components/html/checkbox-helper';

export class EventsPageHelper {

    static async fillNewEventForm(title: string, stepLogger: StepLogger) {

        stepLogger.step(`Title *: New Event 1`);
        await TextboxHelper.sendKeys(EventsPage.titleTextField, title);

        stepLogger.step(`Select Category *: New Event 1`);
        await PageHelper.click(EventsPage.categoryField);
        await PageHelper.click(EventsPage.categoryOption);

        stepLogger.stepId(4);
        stepLogger.step('Click on save');
        await PageHelper.click(CommonPage.formButtons.save);

    }

    static async verifyNewEventCreated(stepLogger: StepLogger) {

        await PageHelper.switchToDefaultContent();
        stepLogger.verification('"New Event" page is closed');
        await expect(await CommonPage.formButtons.save.isPresent())
            .toBe(false,
                ValidationsHelper.getWindowShouldNotBeDisplayedValidation(EventsPageConstants.editPageName));

        stepLogger.verification('verify "New Event" get created');
        await expect(await PageHelper.isElementDisplayed(EventsPage.calenderBlock))
            .toBe(true, ValidationsHelper.getFieldDisplayedValidation(CommonPageConstants.title));
    }

    static async fillNewEventsFormAndVerifyEventCreated(title: string, stepLogger: StepLogger) {
        stepLogger.step(`Title *: New Event 1`);
        await TextboxHelper.sendKeys(EventsPage.titleTextField, title);

        stepLogger.step(`Select Category *: New Event 1`);
        await PageHelper.click(EventsPage.categoryField);
        await PageHelper.click(EventsPage.categoryOption);

        stepLogger.stepId(4);
        stepLogger.step('Click on save');
        await PageHelper.click(CommonPage.formButtons.save);
        await PageHelper.switchToDefaultContent();

        stepLogger.verification('"New Event" page is closed');
        await expect(await CommonPage.formButtons.save.isPresent())
            .toBe(false,
                ValidationsHelper.getWindowShouldNotBeDisplayedValidation(EventsPageConstants.editPageName));

        stepLogger.verification('verify "New Event" get created');
        await expect(await PageHelper.isElementDisplayed(EventsPage.calenderBlock))
            .toBe(true, ValidationsHelper.getFieldDisplayedValidation(CommonPageConstants.title));
    }

    static async createNewEvent() {
        const stepLogger = new StepLogger(786942);

        stepLogger.step('PRECONDITION: navigate to Events page');
        await CommonPageHelper.navigateToItemPageUnderMyWorkplace(MyWorkplacePage.navigation.events,
            CommonPage.pageHeaders.myWorkplace.events,
            CommonPageConstants.pageHeaders.myWorkplace.events,
            stepLogger);

        stepLogger.stepId(1);
        stepLogger.step('Click on "Events" tab displayed on top of "Events" page');
        await PageHelper.click(EventsPage.eventsTab);
        stepLogger.verification('Tab Panel of the Events should get displayed');
        await expect(await PageHelper.isElementDisplayed(CommonPage.tabPanel))
            .toBe(true, ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.tabPanel));

        stepLogger.stepId(2);
        stepLogger.step('Click on "New Event" option from Events tab panel');
        await PageHelper.click(EventsPage.newEvent);
        stepLogger.verification('"Events - New Item" window is displayed');
        await WaitHelper.getInstance().waitForElementToBeDisplayed(CommonPage.dialogTitles.first());
        await expect(await CommonPage.dialogTitles.first().getText())
            .toBe(EventsPageConstants.pageName, ValidationsHelper.getPageDisplayedValidation(EventsPageConstants.pageName));

        stepLogger.step('Switch to frame');
        await CommonPageHelper.switchToFirstContentFrame();
        stepLogger.stepId(3);
        stepLogger.step('Enter/Select required details in "Events - New Item" window');
        const labels = EventsPageConstants.inputLabels;
        const uniqueId = PageHelper.getUniqueId();
        const title = `${labels.title} ${uniqueId}`;
        await EventsPageHelper.fillNewEventForm(title, stepLogger);
        await EventsPageHelper.verifyNewEventCreated(stepLogger);
        return title;
    }

    static async createView(stepLogger: StepLogger, uniqueId: string, defaultView: boolean) {

        stepLogger.stepId(1);
        stepLogger.step('Click on CALENDAR tab');
        await PageHelper.click(EventsPage.calenderTab);

        stepLogger.verification('Contents of the CALENDAR tab should be displayed');
        await expect(await PageHelper.isElementDisplayed(CommonPage.calendearView))
            .toBe(true, ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.calendarContent));

        stepLogger.stepId(2);
        stepLogger.step('Click on Create View');
        await ElementHelper.clickUsingJs(EventsPage.createView);

        stepLogger.verification('View Type page should be displayed');
        await expect(browser.getTitle()).toEqual(CommonPageConstants.viewDropDownLabels.createPublicView,
            ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.viewType));

        stepLogger.stepId(3);
        stepLogger.step('Select any of the view [Example Standard View]');
        await PageHelper.click(EventsPage.standardViewType);

        stepLogger.verification('Create View popup should be displayed');
        await expect(browser.getTitle()).toEqual(CommonPageConstants.viewDropDownLabels.
            createPublicView, ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.createView));

        stepLogger.stepId(4);
        stepLogger.step('Provide value in required fields and check Make this the default view');

        if (defaultView === true) {
            await CheckboxHelper.markCheckbox(EventsPage.defaultCheckbox, true);
        }
        await TextboxHelper.sendKeys(EventsPage.viewName, uniqueId);

        stepLogger.stepId(5);
        stepLogger.step('Click on Ok button');
        await PageHelper.click(CommonPage.okButton);

        stepLogger.verification('View should be created and user should be navigated to event page');
        await PageHelper.click(EventsPage.rollOverEventList);
        await expect(await PageHelper.isElementDisplayed(ElementHelper.getElementByText(uniqueId)))
            .toBe(true, ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.createdView));

        stepLogger.stepId(6);
        stepLogger.step('Navigate to any other page and come back to Event page and from the CALENDAR tab, select' +
            ' any the Standard View which was created from the Current View drop-down');
        await PageHelper.click(CommonPageHelper.getbuttons.calender);
        await PageHelper.click(EventsPage.calenderTab);

        stepLogger.step('Expand Current View drop down');
        await PageHelper.click(EventsPage.currentView);
        await ElementHelper.clickUsingJs(ElementHelper.getElementByText(uniqueId));

        stepLogger.verification('Created view should be displayed in the list');
        await expect(await PageHelper.isElementDisplayed(ElementHelper.getElementByText(uniqueId)))
            .toBe(true, ValidationsHelper.getMenuDisplayedValidation(CommonPageConstants.createdView));

    }
}
