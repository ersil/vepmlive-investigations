import {browser} from 'protractor';
import {CommonPageHelper} from '../../../../../page-objects/pages/common/common-page.helper';
import {StepLogger} from '../../../../../../core/logger/step-logger';
import {CommonPage} from '../../../../../page-objects/pages/common/common.po';
import {ValidationsHelper} from '../../../../../components/misc-utils/validation-helper';
import {ProjectItemPage} from '../../../../../page-objects/pages/items-page/project-item/project-item.po';
import {PageHelper} from '../../../../../components/html/page-helper';
import {ProjectItemPageHelper} from '../../../../../page-objects/pages/items-page/project-item/project-item-page.helper';
import {WaitHelper} from '../../../../../components/html/wait-helper';
import {CommonPageConstants} from '../../../../../page-objects/pages/common/common-page.constants';
import {HomePage} from '../../../../../page-objects/pages/homepage/home.po';
import {SuiteNames} from '../../../../helpers/suite-names';
import {LoginPage} from '../../../../../page-objects/pages/login/login.po';
import {ElementHelper} from '../../../../../components/html/element-helper';
import {ProjectItemPageConstants} from '../../../../../page-objects/pages/items-page/project-item/project-item-page.constants';

describe(SuiteNames.smokeTestSuite, () => {
    let loginPage: LoginPage;

    beforeEach(async () => {

        await PageHelper.maximizeWindow();
        loginPage = new LoginPage();
        await loginPage.goToAndLogin();
    });

    afterEach(async () => {
        await StepLogger.takeScreenShot();
    });

    it('Creating New Task and Assigning Task to resource functionality - [1124259]', async () => {
        StepLogger.caseId = 1124259;
        const uniqueId = PageHelper.getUniqueId();
        const taskElement = ElementHelper.getElementByText(uniqueId);

        StepLogger.stepId(1);
        StepLogger.step('Select "Navigation" icon  from left side menu');
        StepLogger.step('Select Projects -> Projects from the options displayed');

        await CommonPageHelper.navigateToItemPageUnderNavigation(
            HomePage.navigation.projects.projects,
            CommonPage.pageHeaders.projects.projectsCenter,
            CommonPageConstants.pageHeaders.projects.projectCenter,
        );

        StepLogger.verification('Project Center page is displayed');
        await expect(await PageHelper.isElementDisplayed(CommonPage.pageHeaders.projects.projectsCenter, true))
            .toBe(true,
                ValidationsHelper.getPageDisplayedValidation(CommonPageConstants.pageHeaders.projects.projectCenter));

        StepLogger.stepId(2);
        StepLogger.step('Select the ellipses button (...) displayed on right side of the project created as per pre requisites');
        StepLogger.step('Select "Edit Plan" from the menu items displayed');
        await CommonPageHelper.actionTakenViaContextMenu(CommonPage.project,
            CommonPage.contextMenuOptions.editPlan, );

        StepLogger.step('Select "Project Planner" in "Select Planner" window (if displayed)');
        await ProjectItemPageHelper.selectPlannerIfPopUpAppears(ProjectItemPage.selectPlanner.projectPlanner);

        StepLogger.verification('"Project Planner" window is displayed');
        await expect(await PageHelper.isElementDisplayed(CommonPage.pageHeaders.projects.projectPlanner))
            .toBe(true,
                ValidationsHelper.getPageDisplayedValidation(CommonPageConstants.pageHeaders.projects.projectPlanner));

        StepLogger.stepId(3);
        StepLogger.step('On Project planner page click on "+ Task" button');
        await browser.sleep(PageHelper.timeout.m);
        await WaitHelper.waitForElementToBeHidden(CommonPage.plannerbox);
        await CommonPageHelper.deleteTask();
        await PageHelper.click(CommonPage.ribbonItems.addTask);

        StepLogger.verification('New row to add Task is displayed in Project Planner window below the Project Name node');
        await expect(await PageHelper.isElementDisplayed(ProjectItemPage.selectTaskName))
            .toBe(true, ValidationsHelper.getDisplayedValidation(CommonPageConstants.pageHeaders.projects.tasks));

        StepLogger.stepId(4);
        StepLogger.step('Enter a Name for the Task [Ex: Task 1 Smoke Test Project 1] in "Task Name" column');
        await PageHelper.actionSendKeys(uniqueId);
        await PageHelper.click(ProjectItemPage.selectTaskName);

        StepLogger.verification('Name entered for the task is displayed in "Task Name" column' +
            ' [Ex: Task 1 Smoke Test Project 1]');
        await expect(await ElementHelper.getText(ProjectItemPageHelper.newTasksFields.title)).toBe(uniqueId,
            ValidationsHelper.getFieldShouldHaveValueValidation(ProjectItemPageConstants.newTaskFields.title, uniqueId));

        StepLogger.stepId(5);
        StepLogger.step('Click on "Assigned To" column');
        await WaitHelper.staticWait(PageHelper.timeout.s);
        await WaitHelper.waitForElementToBePresent(ProjectItemPage.assignToDropDown);
        await ElementHelper.actionHoverOver(ProjectItemPage.assignToDropDown);
        await PageHelper.click(ProjectItemPage.assignToDropDown);

        StepLogger.verification('List of users drop down with a check box on Right side of user name is displayed');
        const user = await ElementHelper.getText(ProjectItemPageHelper.selectFirstAssign());
        await expect(await PageHelper.isElementDisplayed(ProjectItemPageHelper.selectFirstAssign())).toBe(true,
            ValidationsHelper.getDisplayedValidation(ProjectItemPageConstants.newTaskFields.assignedList));

        StepLogger.stepId(6);
        StepLogger.step('Select the check box for user to which the task need to be assigned [Ex: User1 User1]');
        await PageHelper.click(ProjectItemPageHelper.selectFirstAssign());

        StepLogger.step('Click OK button');
        await PageHelper.click(ProjectItemPageHelper.button.ok);

        StepLogger.verification('List of users drop down is closed');
        await expect(await ProjectItemPageHelper.selectFirstAssign().isPresent()).toBe(false,
            ValidationsHelper.getNotDisplayedValidation(ProjectItemPageConstants.newTaskFields.assignedList));

        StepLogger.verification('Selected user name is displayed in "Assigned To" column [Ex: User1 User1] in' +
            ' "Project Planner" window');
        await expect(await ElementHelper.getElementByText(user).isPresent()).toBe(true,
            ValidationsHelper.getDisplayedValidation(user));

        StepLogger.stepId(7);
        StepLogger.step('Click on Save button from ribbon panel');
        await ElementHelper.clickUsingJs(ProjectItemPage.save);

        StepLogger.verification('Changes done in "Project Planner" window are saved');
        // After save It need static wait(5 sec) and no element found which get change after save.
        await browser.sleep(PageHelper.timeout.s);
        await expect(await ElementHelper.getText(ProjectItemPageHelper.newTasksFields.title)).toBe(uniqueId,
            ValidationsHelper.getFieldShouldHaveValueValidation(ProjectItemPageConstants.newTaskFields.title, uniqueId));
        await expect(await ElementHelper.getElementByText(user).isPresent()).toBe(true,
            ValidationsHelper.getDisplayedValidation(user));

        StepLogger.stepId(8);
        StepLogger.step('Click on Publish button in Project Planner window');
        await ElementHelper.clickUsingJs(ProjectItemPage.publishButtton);

        StepLogger.step('Wait till the Publishing is completed [Publish Status will show the status]');
        StepLogger.verification('Project Planner details are published successfully');
        // Wait required to let it publish
        await browser.sleep(PageHelper.timeout.s);
        await expect(await PageHelper.isElementPresent(ProjectItemPage.publishstatus)).toBe(true,
            ValidationsHelper.getDisplayedValidation(ProjectItemPageConstants.itemOptions.publish));

        StepLogger.stepId(9);
        StepLogger.step('Click "Close" button in "Project Planner" window');
        await ElementHelper.clickUsingJs(ProjectItemPage.close);

        StepLogger.verification('Project Planner window is closed');
        await WaitHelper.waitForElementToBeDisplayed(CommonPage.pageHeaders.projects.projectsCenter);
        await expect(await CommonPage.pageHeaders.projects.projectPlanner.isPresent()).toBe(false,
            ValidationsHelper.getNotDisplayedValidation(CommonPageConstants.pageHeaders.projects.projectPlanner));

        StepLogger.verification('Project Center page is displayed');
        await expect(await PageHelper.isElementDisplayed(CommonPage.pageHeaders.projects.projectsCenter)).toBe(true,
            ValidationsHelper.getPageDisplayedValidation(CommonPageConstants.pageHeaders.projects.projectCenter));

        StepLogger.stepId(10);
        StepLogger.step('Go to Tasks [Left panel> Navigation > Tasks]');
        await CommonPageHelper.navigateToItemPageUnderNavigation(
            HomePage.navigation.projects.tasks,
            CommonPage.pageHeaders.projects.taskCenter,
            CommonPageConstants.pageHeaders.projects.tasks,
        );

        StepLogger.verification('Newly added Task [Ex: Task 1 Smoke Test Project 1] is displayed in Task List');
        await expect(await PageHelper.isElementDisplayed(taskElement)).toBe(true,
            ValidationsHelper.getDisplayedValidation(uniqueId));

    });
});
