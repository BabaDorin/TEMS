import { IOption } from 'src/app/models/option.model';
import { ViewUser, ViewUserSimplified } from './../../models/user/view-user.model';
import { LoginModel } from './../../models/identity/login.model';
import { API_USER_URL, API_AUTH_URL, API_URL } from './../../models/backend.config';
import { TEMSService } from './../tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AddUser } from './../../models/identity/add-user.model';
import { Role } from '../../models/role.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class UserService extends TEMSService {

  role: Role;

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

  getRoleClaims(roles: string[]): Observable<string[]>{
    if(roles == undefined || roles.length == 0) return of([]);

    return this.http.get<string[]>(
      API_USER_URL + '/getroleclaims/' + roles,
      this.httpOptions
    );
  }

  getUserClaims(userId: string): Observable<string[]>{
    if(userId == undefined) return of([]);

    return this.http.get<string[]>(
      API_USER_URL + '/getuserclaims/' + userId,
      this.httpOptions
    );
  }
  
  fetchClaims(): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_USER_URL + '/getclaims',
      this.httpOptions
    );
  }


  addUser(user: AddUser): Observable<any>{
    return this.http.post(
      API_USER_URL+ '/adduser',
      JSON.stringify(user),
      this.httpOptions
    );
  }

  updateUser(user: AddUser): Observable<any>{
    return this.http.post(
      API_USER_URL+ '/updateuser',
      JSON.stringify(user),
      this.httpOptions
    );
  }

  isLoggedIn(): Observable<any>{
    return this.http.get(
      API_AUTH_URL + '/isauthenticated',
      this.httpOptions
    );
  }

  logIn(loginModel): Observable<any>{
    return this.http.post(
      API_USER_URL + '/login',
      JSON.stringify(loginModel),
      this.httpOptions
    );
  }

  signOut(): Observable<any>{
    let token = localStorage.getItem('token');
    return this.http.post(
      API_URL + "/auth/signout/",
      JSON.stringify(token),
      this.httpOptions
    );
  }

  getUsers(): Observable<ViewUserSimplified[]>{
    return this.http.get<ViewUserSimplified[]>(
      API_USER_URL + '/getusers',
      this.httpOptions
    );
  }

  getUserSimplifiedById(userId: string): Observable<ViewUserSimplified>{
    return this.http.get<ViewUserSimplified>(
      API_USER_URL + '/getsimplifiedbyid/' + userId,
      this.httpOptions 
    );
  }

  removeUser(userId: string): Observable<any>{
    return this.http.get(
      API_USER_URL + '/removeUser/' + userId,
      this.httpOptions 
    );
  }

  getUser(userId: string): Observable<ViewUser>{
    return this.http.get<ViewUser>(
      API_USER_URL + '/getuser/' + userId,
      this.httpOptions
    );
  }

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
}
