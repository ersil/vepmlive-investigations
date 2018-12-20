import {LoginPage} from '../../../../../page-objects/pages/login/login.po';
import {SuiteNames} from '../../../../helpers/suite-names';
import {PageHelper} from '../../../../../components/html/page-helper';
import {StepLogger} from '../../../../../../core/logger/step-logger';
import {HomePage} from '../../../../../page-objects/pages/homepage/home.po';
import {CommonPageHelper} from '../../../../../page-objects/pages/common/common-page.helper';
import {CommonPage} from '../../../../../page-objects/pages/common/common.po';
import {CommonPageConstants} from '../../../../../page-objects/pages/common/common-page.constants';
import {OptimizerPageHelper} from '../../../../../page-objects/pages/items-page/project-item/optimizer/optimizer-page.helper';
import { EditCostHelper } from '../../../../../page-objects/pages/items-page/project-item/edit-cost-page/edit-cost.helper';
import { ProjectItemPageHelper } from '../../../../../page-objects/pages/items-page/project-item/project-item-page.helper';
import { LoginPageHelper } from '../../../../../page-objects/pages/login/login-page.helper';

describe(SuiteNames.smokeTestSuite, () => {
    let loginPage: LoginPage;
    let project1 = '';
    let project2 = '';

    beforeAll(async () => {
        await new LoginPage().goToAndLogin();
        await CommonPageHelper.navigateToItemPageUnderNavigation(
            HomePage.navigation.projects.projects,
            CommonPage.pageHeaders.projects.projectsCenter,
            CommonPageConstants.pageHeaders.projects.projectCenter,
        );
        project1 = await EditCostHelper.createProjectWithCost();
        await EditCostHelper.clickCloseCostPlanner();
        project2 = await EditCostHelper.createProjectWithCost();
        await EditCostHelper.clickCloseCostPlanner();
        await LoginPageHelper.logout();
    });

    beforeEach(async () => {
        await PageHelper.maximizeWindow();
        loginPage = new LoginPage();
        await loginPage.goToAndLogin();
    });

    afterEach(async () => {
        await PageHelper.switchToDefaultContent();
        await StepLogger.takeScreenShot();
    });

    afterAll(async () => {
        await ProjectItemPageHelper.deleteProjectAndValidateIt(project1);
        await ProjectItemPageHelper.deleteProjectAndValidateIt(project2);
    });

    it('Create New Strategy in Optimizer Functionality - [1124301][BUG:SKYVERA-1844]', async () => {
        StepLogger.caseId = 1124301;
        const uniqueId = PageHelper.getUniqueId();

        StepLogger.stepId(1);
        await CommonPageHelper.navigateToItemPageUnderNavigation(
            HomePage.navigation.projects.projects,
            CommonPage.pageHeaders.projects.projectsCenter,
            CommonPageConstants.pageHeaders.projects.projectCenter,
        );

        StepLogger.stepId(2);
        StepLogger.stepId(3);
        await CommonPageHelper.optimizerViaRibbon();

        await CommonPageHelper.verifyNavigation();

        StepLogger.stepId(4);
        await OptimizerPageHelper.clickConfigrationButton();

        await OptimizerPageHelper.verifyConfigrationPopUPDisplayed();

        StepLogger.stepId(5);
        await OptimizerPageHelper.addAvilabelFiled();

        await OptimizerPageHelper.verifyConfigrationPopUpClosed();

        StepLogger.stepId(6);
        await OptimizerPageHelper.clickSaveStrategy();

        await OptimizerPageHelper.verifySaveStrategyPopUpOpen();

        StepLogger.stepId(7);
        StepLogger.stepId(8);
        await OptimizerPageHelper.saveStrategyValidateIt(uniqueId);
    });
});
