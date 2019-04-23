import {SuiteNames} from '../../helpers/suite-names';
import {LoginPage} from '../../../page-objects/pages/login/login.po';
import {PageHelper} from '../../../components/html/page-helper';
import {StepLogger} from '../../../../core/logger/step-logger';
import {CommonPageHelper} from '../../../page-objects/pages/common/common-page.helper';
import {CommonPage} from '../../../page-objects/pages/common/common.po';
import {CommonPageConstants} from '../../../page-objects/pages/common/common-page.constants';
import {WorkspacePageHelper} from '../../../page-objects/pages/workspaces/workspace-page.helper';

describe(SuiteNames.endToEndSuite, () => {
    let loginPage: LoginPage;

    beforeEach(async () => {

        await PageHelper.maximizeWindow();
        loginPage = new LoginPage();
        await loginPage.goToAndLogin();
    });

    it('Hide the panel via "Hide" button. - [743197]', async () => {
        StepLogger.caseId = 743197;
        StepLogger.stepId(1);
        await CommonPageHelper.clickLhsSideBarMenuIcon(CommonPage.sidebarMenus.workspaces);
        await CommonPageHelper.verifyPanelHeaderDisplayed(CommonPage.sidebarMenuPanelHeader.workspaces,
            CommonPageConstants.sidebarMenuPanelHeader.workspaces);
        await WorkspacePageHelper.verifyWorkpaceListingInMenuPanel();

        StepLogger.stepId(2);
        await WorkspacePageHelper.expandEllipsisAndSelectEditTeamOption();
        await WorkspacePageHelper.verifyEditTeamWindowOpened();

        StepLogger.stepId(3);
        await WorkspacePageHelper.clickOnCloseButton();
        await WorkspacePageHelper.verifyEditTeamWindowClosed();
    });
});