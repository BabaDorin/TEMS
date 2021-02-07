export interface Role{
    canManageEquipment: boolean;
    canManageRooms: boolean;
    canManagePersonnel: boolean;
    canManageKeys: boolean;
    canManageIssues: boolean;
    canManageCommunication: boolean;
    canManageLibrary: boolean;
    canManageReports: boolean;
    hasAdminRights: boolean;

    canViewEquipment: boolean;
    canViewRooms: boolean;
    canViewPersonnel: boolean;
    canViewKeys: boolean,
    canViewIssues: boolean,
    canViewCommunication: boolean;
    canViewLibrary: boolean;
    canViewReports: boolean;
    canViewAnalytics: boolean;

    canCreateIssues: boolean;
    canAllocateEquipment: boolean;
    canAllocateKeys: boolean;
}