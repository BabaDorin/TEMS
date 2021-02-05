import { Role } from './../../models/role.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  role: Role;

  constructor() {
    this.role = {
      canManageEquipment: false,
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
      canViewCommunication: true,
      canViewLibrary: true,
      canViewReports: true,
      canViewAnalytics: true,
    }
  }

}
