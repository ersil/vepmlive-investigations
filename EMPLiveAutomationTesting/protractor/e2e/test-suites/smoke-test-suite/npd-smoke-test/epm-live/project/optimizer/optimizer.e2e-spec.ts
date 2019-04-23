import {browser} from 'protractor';
import {SuiteNames} from '../../../../../helpers/suite-names';
import {LoginPage} from '../../../../../../page-objects/pages/login/login.po';
import {PageHelper} from '../../../../../../components/html/page-helper';
import {StepLogger} from '../../../../../../../core/logger/step-logger';
import {ValidationsHelper} from '../../../../../../components/misc-utils/validation-helper';
import {TextboxHelper} from '../../../../../../components/html/textbox-helper';
import {CommonPageHelper} from '../../../../../../page-objects/pages/common/common-page.helper';
import {CommonPage} from '../../../../../../page-objects/pages/common/common.po';
import {CommonPageConstants} from '../../../../../../page-objects/pages/common/common-page.constants';
import {WaitHelper} from '../../../../../../components/html/wait-helper';
import {HomePage} from '../../../../../../page-objects/pages/homepage/home.po';
import {OptimizerPage} from '../../../../../../page-objects/pages/items-page/project-item/optimizer-page/optimizer.po';
import {OptimizerPageHelper} from '../../../../../../page-objects/pages/items-page/project-item/optimizer-page/optimizer-page.helper';
import {OptimizerPageConstants} from '../../../../../../page-objects/pages/items-page/project-item/optimizer-page/optimizer-page.constants';
import {ExpectationHelper} from '../../../../../../components/misc-utils/expectation-helper';

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

    it('Save View  - [785307]', async () => {
        StepLogger.caseId = 785307;
        // preConditions -Navigation Panel> Projects> Select project(s)> Click on 'Optimizer'
        await CommonPageHelper.navigateToItemPageUnderNavigation(
            HomePage.navigation.projects.projects,
            CommonPage.pageHeaders.projects.projectsCenter,
            CommonPageConstants.pageHeaders.projects.projectCenter,
        );
        StepLogger.verification('Project Center opened and project selected');
        await expect(await PageHelper.isElementDisplayed(CommonPage.pageHeaders.projects.projectsCenter)).toBe(true,
            ValidationsHelper.getPageDisplayedValidation(CommonPageConstants.pageHeaders.projects.projectCenter));
        await CommonPageHelper.selectRecordFromGrid();
        // optimizer option
        await PageHelper.click(CommonPage.ribbonItems.optimizer);
        await browser.sleep(PageHelper.timeout.m);
        // await browser.sleep(PageHelper.timeout.s);
        await CommonPageHelper.switchToFirstContentFrame();

        StepLogger.stepId(1);
        StepLogger.step('Click on "View" tab');
        await PageHelper.click(OptimizerPage.getTabOptions.view);
        await WaitHelper.waitForElementToBeDisplayed(OptimizerPage.viewManagementOptions.saveView, PageHelper.timeout.s);
        StepLogger.verification('View tab details displayed');
        await expect(await PageHelper.isElementDisplayed(OptimizerPage.viewManagementOptions.saveView)).toBe(true,
            ValidationsHelper.getPageDisplayedValidation(OptimizerPageConstants.viewTab));

        StepLogger.stepId(2);
        StepLogger.step('Click on "Select columns"');
        await PageHelper.click(OptimizerPage.viewManagementOptions.selectColumns);
        // Step #3 is inside this function
        const selectedColNames = await OptimizerPageHelper.selectColumns();

        StepLogger.stepId(4);
        StepLogger.step('Click on "OK" button');
        await PageHelper.click(OptimizerPage.getSelectColumnsPopup.ok);
        await browser.sleep(PageHelper.timeout.s);
        await OptimizerPageHelper.verifyColumnNamesInGrid(selectedColNames);

        StepLogger.stepId(5);
        StepLogger.step('Click on "Save View" from ribbon panel');
        await PageHelper.click(OptimizerPage.viewManagementOptions.saveView);
        StepLogger.verification('Save view popup displayed');
        await expect(await PageHelper.isElementDisplayed(OptimizerPage.saveViewPopup.viewName)).toBe(true,
            ValidationsHelper.getDisplayedValidation(OptimizerPageConstants.saveView));

        StepLogger.stepId(6);
        StepLogger.step('Provide "View Name" and check "Personal View" checkbox');
        const label = OptimizerPage.saveViewPopup;
        await WaitHelper.waitForElementToBeDisplayed(label.viewName, PageHelper.timeout.s);
        // Generating unique save name
        const uniqueId = PageHelper.getUniqueId();
        const viewName = `${OptimizerPageConstants.newViewName}${uniqueId}`;
        await TextboxHelper.sendKeys(label.viewName, viewName);
        const personalViewSelected = await label.personalView.isSelected();
        if (!personalViewSelected) {
            await PageHelper.click(label.personalView);
        }

        StepLogger.stepId(7);
        StepLogger.step('Click on "Ok" button');
        await PageHelper.click(label.ok);
        await WaitHelper.waitForElementToHaveExactText(OptimizerPage.viewManagementOptions.currentViewDropdown, viewName);
        // Taking time to reflect
        StepLogger.verification('View saved and displayed under "Current View"');
        await expect(await PageHelper.getText(OptimizerPage.viewManagementOptions.currentViewDropdown))
            .toEqual(viewName, ValidationsHelper.getNewViewShouldDisplayed(viewName));

        StepLogger.stepId(8);
        StepLogger.step('Click on "Close"');
        // await PageHelper.click(OptimizerPage.getTabOptions.optimizer);
        // await browser.sleep(PageHelper.timeout.xs);
        await PageHelper.click(OptimizerPage.getCloseOptimizerViewTab);
        StepLogger.verification('Optimizer window closed');
        await ExpectationHelper.verifyNotDisplayedStatus(OptimizerPage.getCloseOptimizerWindow,
            OptimizerPageConstants.viewTab);

        StepLogger.stepId(9);
        StepLogger.step('From project center page select same projects and click on "Optimizer" button');
        await PageHelper.click(CommonPageHelper.getRibbonButtonByText(CommonPageConstants.ribbonLabels.optimizer));
        // Takes time to load iframe
        await browser.sleep(PageHelper.timeout.m);
        await CommonPageHelper.switchToFirstContentFrame();

        StepLogger.stepId(10);
        StepLogger.step('Go to "View" Tab and Expand "Current View" drop down');
        await PageHelper.click(OptimizerPage.getTabOptions.view);
        await OptimizerPageHelper.selectPreviouslySavedView(viewName);

        StepLogger.stepId(11);
        StepLogger.step('Select that view');
        // Takes time to update the table
        await browser.sleep(PageHelper.timeout.xs);
        await OptimizerPageHelper.verifyColumnNamesInGrid(selectedColNames);
    });
});