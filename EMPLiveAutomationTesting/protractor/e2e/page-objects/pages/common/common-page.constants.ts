import {HomePageConstants} from '../homepage/home-page.constants';
import {MyWorkplaceConstants} from '../my-workplace/my-workplace.constants';
import {CreateNewPageConstants} from '../items-page/create-new-page.constants';
import * as path from 'path';

export class CommonPageConstants {
    static readonly pageName = 'Create New';
    static filesDirectoryName = 'files';
    static readonly currentDir = __dirname;
    static readonly tabPanel = 'Tab Panel';
    static readonly calendarContent = 'Calendar Content';
    static readonly createView = 'create View';
    static readonly title = 'Title';
    static readonly columnDescription = 'Column Desciption';
    static readonly viewType = 'View Type';
    static readonly createdView = 'created View';
    static readonly viewAll = 'View All';
    static readonly columnType = 'Column Type';
    static readonly columnName = 'Column Name';
    static readonly newItem = 'New Item';
    static readonly calendar = 'Calendar';
    static readonly attachFilePopupTitle = 'Attach File';
    static readonly last = 'Last';
    static readonly fileUpload = 'fileUpload';
    static readonly column = 'Column Popup';
    static readonly dropDown: string = 'dropdown-toggle';

    static get viewDropDownLabels() {
        return {
            createPublicView: 'Create View'
        };
    }

    static get imageFile() {
        return {
            fileType: '.jpg',
            jpegFileName: 'jpeg-image',
            filePath: () => {
                return path.join(this.currentDir, this.filesDirectoryName, this.imageFile.jpegFileName + this.imageFile.fileType);
            }
        };
    }

    static get documentFile() {
        return {
            fileType: '.pdf',
            documentFileName: 'pdf-file',
            filePath: () => {
                return path.join(this.currentDir, this.filesDirectoryName, this.documentFile.documentFileName + this.documentFile.fileType);
            }
        };
    }

    static get dataConstants() {
        return {
            dev: 'dev'
        };
    }

    static get and() {
        return ' and ';
    }

    static get pagePostFix() {
        return {
            newItem: ' - New Item',
            editItem: ' - Edit Item'
        };
    }

    static get ribbonLabels() {
        return {
            save: 'Save',
            cancel: 'Cancel',
            viewItem: 'ViewItem',
            editItem: 'EditItem',
            editCost: 'EPKCosts',
            addTask: 'AddTask-Large',
            attachFile: 'AttachFile',
            editTeam: 'Edit Team',
            close: 'Close',
            saveAndClose: 'Save &Close',
            assignmentPlanner: 'Assignment Planner',
            viewReports: 'View Reports',
            resourceAvailableVsPlannedByDept: 'Resource Available vs. Planned by Dept',
            resourceCapacityHeatMap: 'Resource Capacity Heat Map',
            resourceCommitments: 'Resource Commitments',
            resourceRequirements: 'Resource Requirements',
            resourceWorkVsCapacity: 'Resource Work vs. Capacity'
        };
    }

    static get disabledribbonbuttonsId() {
        return {
            saveAndClose: 'SaveCloseButton'
        };
    }

    static get formLabels() {
        return {
            save: 'Save',
            cancel: 'Cancel',
            close: 'Close',
            ok: 'OK',
            okWithSmallK: 'Ok',
            add: '< Add',
            remove: 'Remove >',
            topSave: 'onetidSaveItemtop'
        };
    }

    static get newPublicViewformLabels() {
        return {
            title: 'NewViewName',
            publicView: 'PersonalView1',
            scheduleStatus: 'ShouldDisplayScheduleStatus',
            exposure: 'ShouldDisplayExposure',
            due: 'ShouldDisplayDue'
        };
    }

    static get ribbonMenuTitles() {
        return {
            hide: `Hide`,
            manage: `Manage`,
            items: `Items`,
            list: `List`
        };
    }

    static get costData() {
        return {
            firstData: '120.00',
            budgetData: '120',
            actualCostData: '130',
            benefitsData: '140',
        };
    }

    static get pageHeaders() {
        return {
            projects: HomePageConstants.navigationLabels.projects,
            myWorkplace: MyWorkplaceConstants.navigationLabels,
            createNew: CreateNewPageConstants.navigationLabels
        };
    }

    static get actionMenuIconTitles() {
        return {
            search: 'search',
            toggleFilters: 'toggle filters',
            defaultSort: 'default sort',
            groupBy: 'Group by',
            selectColumns: 'select columns',
            view: 'View: ',
            settings: 'settings'
        };
    }

    static get contextMenuOptions() {
        return {
            viewItem: 'View Item',
            editItem: 'Edit Item',
            workFlows: 'Workflows',
            permissions: 'Permissions',
            deleteItem: 'Delete Item',
            comments: 'Comments',
            editTeam: 'Edit Team',
            costPlan: 'Cost Plan',
            editPlan: 'Edit Plan'
        };
    }

    static get menuContainerIds() {
        return {
            navigation: 'EPMLiveNav',
            createNew: 'epm-nav-sub-new',
            myWorkplace: 'epm-nav-sub-workplace-static-links'
        };
    }

    static get versionComment() {
        return {
            first: 'New Version of previously updated file',
            second: '0.2'
        };
    }

    static get statuses() {
        return {
            notStarted: 'Not Started',
            inProgress: 'In Progress',
            completed: 'Completed',
            deferred: 'Deferred',
            waitingOnSomeoneElse: 'Waiting on someone else'
        };
    }

    static get priorities() {
        return {
            high: '(1) High',
            normal: '(2) Normal',
            low: '(3) Low',
        };
    }

    static get portfolioTypes() {
        return {
            other: 'Other',
            customer: 'Customer',
            development: 'Development',
            enhancement: 'Enhancement',
            support: 'Support',
            program: 'Program',
            marketing: 'Marketing'
        };
    }

    static get portfolioGoals() {
        return {
            customerSatisfaction: 'Customer Satisfaction',
            growBusiness: 'Grow Business',
            runBusiness: 'Run Business',
            transformation: 'Transformation'
        };
    }

    static get states() {
        return {
            proposed: '(1) Proposed',
            active: '(2) Active',
            closed: '(3) Closed',
        };
    }

    static get projectUpdate() {
        return {
            manual: 'Manual',
            scheduleDriven: 'Schedule Driven'
        };
    }

    static get overallHealth() {
        return {
            onTrack: '(1) On Track',
            atRisk: '(2) At Risk',
            offTrack: '(3) Off Track',
        };
    }

    static get teamId() {
        return {
            currentTeam: 'TeamGrid',
            resourcePool: 'ResourceGrid'
        };
    }

    static get paginationTitle() {
        return {
            next: 'Next',
            previous: 'Previous'
        };
    }

    static get dropdownShowAllButton() {
        return {
            project: 'Project_ddlShowAll',
            timeOffType: 'TimeOffType_ddlShowAll',
            role: 'Role_ddlShowAll',
            holidaySchedule: 'HolidaySchedule_ddlShowAll',
            workHours: 'WorkHours_ddlShowAll',
            portfolio: 'Portfolio_ddlShowAll',
        };
    }

    static get searchControl() {
        return {
           searchComponentDropdown : 'Component dropdown',
           searchOperatorDropdown: 'Search Operator dropdown',
           searchField : 'Search Field',
        };
    }

    static get messages() {
        return {
           noDataFound : 'No data found',
        };
    }
}
