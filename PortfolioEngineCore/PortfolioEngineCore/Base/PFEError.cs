namespace PortfolioEngineCore
{
    internal enum PFEError
    {
        CheckSecurity = 1000,
        ActivationCantLoadPId = 10001,
        ActivationNoPId,
        ActivationNoCompany,
        ActivationInvalidPId,
        ActivationInvalidCompany,
        GetResourcePermissions = 11000,
        GetResPermDataRootNotFound,
        GetResPermDataResEleNotFound,
        GetResPermDataUsernameAttrNotFound,
        GetResourceIdNoUsername,
        GetResourceId,
        FindById,
        FindResourceById,
        UpdateResource,
        UpdateResourceValidation,
        CustomFieldNotFound,
        InvalidLookup,
        UpdateResourceGroupMembership,
        UpdateDepartments = 4000,
        UD_NoLookupTable,
        DeleteDepartments = 4030,
        CanDeleteDepartment = 4040,
        CanDeleteCostCategoryRole = 4041,
        CanDeleteCostCategoryRolebyCCRId = 4042,
        CountRoleCategories = 4043,
        UpdateRoles = 4045,
        UpdateCategoriesFromRoles = 4046,
        DeleteLookup = 4050,
        CanDeleteLookupValue = 4051,
        CanDeleteLookupValueasCC = 4052,
        DeleteRole = 4053,
        DeleteCCRole = 4054,
        UpdateWorkSchedule = 4055,
        CanDeleteWorkSchedule = 4059,
        DeleteWorkSchedule = 4060,
        UpdateHolidaySchedule = 4065,
        CanDeleteResourceGroup = 4068,
        CanDeleteHolidaySchedule = 4069,
        DeleteHolidaySchedule = 4070,
        UpdatePersonalItems = 4075,
        DeletePersonalItem = 4080,
        UpdateResourceTimeoff = 4085,
        DeleteResourceTimeoff = 4090,
        UpdateScheduledWork = 4095,
        UpdateListWork = 4096,
        DeleteListWork = 4097,
        DeletePIListWork = 4098,
        PostCostValues = 4099,
        PostTimesheetData = 5001,
        GetCCRs = 5010,
        GetDepts = 5011,
        GetWHs = 5012,
        GetHOLs = 5013,
        GetPersonalItems = 5014,
        DBCantLoadConnectionString = 20001,
        DBCantOpenDB = 20002,
        FieldTypeNotFound = 30000,
        BaseAccessDenied = 9999
    }
}