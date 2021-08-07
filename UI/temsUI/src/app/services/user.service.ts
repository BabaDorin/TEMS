import { ProfilePhotoViewModel } from './../models/profile/change-profile-photo.model';
import { AccountGeneralInfoModel } from '../models/identity/account-general-info.model';
import { EmailPreferencesModel } from '../models/identity/email-preferences.model';
import { ChangePasswordModel } from '../models/identity/change-password.model';
import { IOption } from 'src/app/models/option.model';
import { ViewUser, ViewUserSimplified } from '../models/user/view-user.model';
import { LoginModel } from '../models/identity/login.model';
import { API_USER_URL, API_AUTH_URL, API_URL, API_PROFILE_URL, API_ALL_URL, API_NOTIF_URL } from '../models/backend.config';
import { TEMSService } from './tems.service';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { AddUser } from '../models/identity/add-user.model';
import { Role } from '../models/role.model';
import { Injectable } from '@angular/core';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { ViewNotification } from 'src/app/models/communication/notification/view-notification.model';

@Injectable({
  providedIn: 'root'
})
export class UserService extends TEMSService {

  role: Role;

  constructor(
    private http: HttpClient
  ) {
    super();
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
    return this.http.put(
      API_USER_URL+ '/updateuser',
      JSON.stringify(user),
      this.httpOptions
    );
  }

  getUsers(role?: string): Observable<ViewUserSimplified[]>{
    let endPoint = API_USER_URL + '/getusers';
    if(role != undefined)
      endPoint += '/' + role; 
    
    return this.http.get<ViewUserSimplified[]>(
      endPoint,
      this.httpOptions
    );
  }

  getAllAutocompleteOptions(filter: string): Observable<IOption[]>{
    return this.http.get<IOption[]>(
      API_USER_URL + '/getallautocompleteoptions/' + filter,
      this.httpOptions
    );
  }

  getUserSimplifiedById(userId: string): Observable<ViewUserSimplified>{
    return this.http.get<ViewUserSimplified>(
      API_USER_URL + '/getsimplifiedbyid/' + userId,
      this.httpOptions 
    );
  }

  archieveUser(userId: string, archivationStatus?: boolean): Observable<any>{
    if(archivationStatus == undefined) archivationStatus = true; // to be archieved

    return this.http.get(
      API_USER_URL + '/archieve/' + userId + '/' + archivationStatus,
      this.httpOptions 
    );
  }

  remove(userId: string): Observable<any>{
    return this.http.delete(
      API_USER_URL + '/remove/' + userId,
      this.httpOptions 
    );
  }

  getUser(userId: string): Observable<ViewUser>{
    return this.http.get<ViewUser>(
      API_USER_URL + '/getuser/' + userId,
      this.httpOptions
    );
  }

  getProfileData(userId: string): Observable<ViewProfile>{
    return this.http.get<ViewProfile>(
      API_PROFILE_URL + '/get/' + userId,
      this.httpOptions
    );
  }

  changePassword(changePasswordModel: ChangePasswordModel): Observable<any>{
    return this.http.put(
      API_USER_URL + '/changePassword',
      JSON.stringify(changePasswordModel),
      this.httpOptions
    );
  }

  changeEmailPreferences(emailPreferencesModel: EmailPreferencesModel): Observable<any>{
    return this.http.put(
      API_USER_URL + '/changeEmailPreferences',
      JSON.stringify(emailPreferencesModel),
      this.httpOptions
    );
  }

  editAccountGeneralInfo(accountGeneralInfoModel: AccountGeneralInfoModel): Observable<any>{
    return this.http.put(
      API_USER_URL + '/editAccountGeneralInfo',
      JSON.stringify(accountGeneralInfoModel),
      this.httpOptions
    );
  }

  getLastNotifications(take?: number): Observable<ViewNotification[]>{
    let endPoint = API_NOTIF_URL +  "/getlastnotifications";
    if(take != undefined)
      endPoint += '/' + take;
    
    return this.http.get<ViewNotification[]>(
      endPoint,
      this.httpOptions
    );
  }

  getAllNotifications(skip?: number, take?: number): Observable<ViewNotification[]>{
    let endPoint =  API_NOTIF_URL + "/getallnotifications";
    if(skip != undefined && take != undefined)
      endPoint += '/' + skip + '/' + take;

    return this.http.get<ViewNotification[]>(
      endPoint,
      this.httpOptions
    );
  }
  
  removeNotification(notificationId: string): Observable<any>{
    return this.http.delete(
      API_NOTIF_URL + '/remove/' + notificationId,
      this.httpOptions
    );
  }

  changeProfilePhoto(profilePhotoViewModel: ProfilePhotoViewModel): Observable<any>{  
    const formData = new FormData();
    formData.append('userId', profilePhotoViewModel.userId);
    formData.append('photo', profilePhotoViewModel.photo, profilePhotoViewModel.photo?.name ?? 'photo.jpg');

    return this.http.post(
      API_USER_URL + '/changeProfilePhoto',
      formData
    );
  }

  markNotificationsAsSeen(notificationIds: string[]): Observable<any>{
    let params = new HttpParams();
    notificationIds.forEach(q => {
      params = params.append('notificationIds', q)
    });

    return this.http.get(
      API_NOTIF_URL + '/markAsSeen',
      {params: params}
    );
  }
}
