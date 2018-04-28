export class RiskItemPageConstants {
    static readonly pageName = 'Risks - New Item';
    static readonly editPageName = 'Risks Page Edit Mode';

    static get inputLabels() {
        return {
            title: 'Title *',
            project: 'Project *',
            assignedTo: 'Assigned To',
            status: 'Status',
            priority: 'Priority',
            description: 'Description',
            startDate: 'Start Date',
            dueDate: 'Due Date',
            effort: 'Effort',
            comments: 'Comments',
            probability: 'Probability',
            impact: 'Impact',
            cost: 'Cost',
            mitigationPlan: 'Mitigation Plan',
            contingencyPlan: 'Contingency Plan',
            trigger: 'Trigger',
            triggerDescription: 'Trigger Description'
        };
    }

    static get columnNames() {
        return {
            assignedTo: 'Assigned To',
            comments: 'Comments',
            createdBy: 'Created By',
            daysOverdue: 'Days Overdue',
            description: 'Description',
            dueDate: 'Due Date',
            due: 'Due',
            effort: 'Effort',
            id: 'ID',
            modifiedBy: 'Modified By',
            priority: 'Priority',
            project: 'Project',
            relatedRisks: 'Related Risks',
            resolution: 'Resolution',
            scheduleStatus: 'Schedule Status',
            startDate: 'Start Date',
            status: 'Status',
            title: 'Title',
            specialColumns: 'Special Columns',
            ganttChart: 'Gantt Chart'
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
}
