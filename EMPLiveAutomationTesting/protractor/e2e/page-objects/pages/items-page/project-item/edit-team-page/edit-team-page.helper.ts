import {StepLogger} from '../../../../../../core/logger/step-logger';
import {PageHelper} from '../../../../../components/html/page-helper';
import {EditTeamPage} from './edit-team-page.po';
import {CommonPage} from '../../../common/common.po';
import {WaitHelper} from '../../../../../components/html/wait-helper';

export class EditTeamPageHelper {
    static async clickviewReport() {
        StepLogger.step('click on view report');
        await PageHelper.click(EditTeamPage.viewReport);
    }

    static async clickResourceCapacityHeatMap() {
        StepLogger.step('click on resource Capacity');
        await PageHelper.click(EditTeamPage.viewReportMenus.resourceCapacityHeatMap);
    }

    static async clickResourceCommitments() {
        StepLogger.step('click on resource commitments');
        await PageHelper.click(EditTeamPage.viewReportMenus.resourceCommitments);
    }

    static async resourceAvailableVsPlannedByDept() {
        StepLogger.step('click on resource Capacity');
        await PageHelper.click(EditTeamPage.viewReportMenus.resourceAvailableVsPlannedByDept);
    }

    static async resourceWorkVsCapacity() {
        StepLogger.step('click on resource Capacity');
        await PageHelper.click(EditTeamPage.viewReportMenus.resourceAvailableVsPlannedByDept);
    }

    static async closeEditTeamPopUp() {
        StepLogger.step('click on resource commitments');
        await PageHelper.click(CommonPage.ribbonItems.close);
    }

    static async closeEditTeamPopUpAndSwitchToDefaultContent() {
        StepLogger.subStep('Click "Close" button in "Edit Team" window');
        await PageHelper.click(CommonPage.ribbonItems.close);

        StepLogger.subStep('Switch to default content');
        await WaitHelper.waitForElementToBeHidden(CommonPage.ribbonItems.close);
        await PageHelper.switchToDefaultContent();
    }
}