import { API_USER_URL } from './../../models/backend.config';
import { TEMSService } from './../tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AddUser } from './../../models/identity/add-user.model';
import { IOption } from 'src/app/models/option.model';
import { Role } from '../../models/role.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService extends TEMSService {
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

  constructor(
    private http: HttpClient
  ) {
    super();
    this.role = this.admin;
  }

  getRoles(): IOption[]{
    return [
      { value: '1', label: 'Utilizator'},
      { value: '2', label: 'Personal'},
      { value: '3', label: 'Tehnician'},
      { value: '4', label: 'Administrator'},
    ]
  }

  addUser(user: AddUser): Observable<any>{
    return this.http.post(
      API_USER_URL+ '/adduser',
      JSON.stringify(user),
      this.httpOptions
    );
  }
}
