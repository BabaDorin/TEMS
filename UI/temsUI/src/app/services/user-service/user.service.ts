import { Role } from '../../contracts/role.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  // Fake service.
  // Roles will be provided by the API

  role: Role;

  private admin = {
    canManageEquipment: true,
    canManageRooms: true,
    canManagePersonnel: true,
    canManageKeys: true,
    canManageIssues: true,
    canManageCommunication: true,
    canManageLibrary: true,
    canManageReports: true,
    hasAdminRights: true,

    canViewEquipment: true,
    canViewRooms: true,
    canViewPersonnel: true,
    canViewKeys: true,
    canViewIssues: true,
    canViewCommunication: true,
    canViewLibrary: true,
    canViewReports: true,
    canViewAnalytics: true,

    canCreateIssues: true,
    canAllocateEquipment: true,
    canAllocateKeys: true,
  };

  private technician = {
    canManageEquipment: false,
    canManageRooms: false,
    canManagePersonnel: false,
    canManageKeys: false,
    canManageIssues: true,
    canManageCommunication: true,
    canManageLibrary: true,
    canManageReports: true,
    hasAdminRights: false,

    canViewEquipment: true,
    canViewRooms: true,
    canViewPersonnel: true,
    canViewKeys: true,
    canViewIssues: true,
    canViewCommunication: true,
    canViewLibrary: true,
    canViewReports: true,
    canViewAnalytics: true,

    canCreateIssues: true,
    canAllocateEquipment: true,
    canAllocateKeys: false,
  };

  private personnel = {
    canManageEquipment: false,
    canManageRooms: false,
    canManagePersonnel: false,
    canManageKeys: false,
    canManageIssues: false,
    canManageCommunication: false,
    canManageLibrary: false,
    canManageReports: false,
    hasAdminRights: false,

    canViewEquipment: true,
    canViewRooms: true,
    canViewPersonnel: true,
    canViewKeys: true,
    canViewIssues: false,
    canViewCommunication: true,
    canViewLibrary: true,
    canViewReports: false,
    canViewAnalytics: true,

    canCreateIssues: true,
    canAllocateEquipment: false,
    canAllocateKeys: false,
  };

  private guest = {
    canManageEquipment: false,
    canManageRooms: false,
    canManagePersonnel: false,
    canManageKeys: false,
    canManageIssues: false,
    canManageCommunication: false,
    canManageLibrary: false,
    canManageReports: false,
    hasAdminRights: false,

    canViewEquipment: false,
    canViewRooms: false,
    canViewPersonnel: false,
    canViewKeys: false,
    canViewIssues: false,
    canViewCommunication: true,
    canViewLibrary: false,
    canViewReports: false,
    canViewAnalytics: false,

    canCreateIssues: true,
    canAllocateEquipment: false,
    canAllocateKeys: false,
  }

  constructor() {
    this.role = this.admin;
  }
}
