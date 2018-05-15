export class HomePageConstants {
    static readonly pageName = 'Home page';
    static readonly comment = 'This is a test comment';

    static get navigationLabels() {
        return {
            projects: {
                requests: 'Requests',
                portfolios: 'Portfolios',
                projectPortfolios: 'Project Portfolios',
                projects: 'Projects',
                projectCenter: 'Project Center',
                tasks: 'Tasks',
                risks: 'Risks',
                issues: 'Issues',
                changes: 'Changes',
                documents: 'Documents',
                resources: 'Resources',
                reports: 'Reports',
                reporting: 'Reporting'
            }
        };
    }

    static get addADocumentWindow() {
        return{
            addADocumentTitle: 'Add a document',
            chooseFiles: 'Choose Files',
            overwriteExistingFilesLabel: 'Overwrite existing files',
        };
    }
}
