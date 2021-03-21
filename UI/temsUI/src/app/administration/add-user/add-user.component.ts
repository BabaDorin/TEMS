import { CheckboxItem } from './../../models/checkboxItem.model';
import { Observable } from 'rxjs';
import { RoleService } from './../../services/role-service/role.service';
import { AddUser } from './../../models/identity/add-user.model';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { specCharValidator } from 'src/app/models/validators';
import { UserService } from 'src/app/services/user-service/user.service';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent extends TEMSComponent implements OnInit {

  userIdToUpdate: string;
  dialogRef: MatDialogRef<any>;
  claims: CheckboxItem[];
  rolesClaims: CheckboxItem[];

  // It will get merged with formlyData soon.
  @ViewChild('rolesChips') roles;
  @ViewChild('personnelChips') personnel;

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  rolesOptions: IOption[];
  personnelOptions: IOption[];

  constructor(
    private personnelService: PersonnelService,
    private userService: UserService,
    private roleService: RoleService,
    private formlyParserService: FormlyParserService,
  ) { 
    super();
  }

  ngOnInit(): void {
    console.log(this.dialogRef);
    this.formlyData.fields = this.formlyParserService.parseAddUser(this.userIdToUpdate != undefined);

    this.subscriptions.push(
      this.roleService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);
        this.rolesOptions = result;
      })
    )

    this.subscriptions.push(
      this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        this.personnelOptions = result;
      })
    );

    if(this.userIdToUpdate != undefined){
      this.subscriptions.push(
        this.userService.getUser(this.userIdToUpdate)
        .subscribe(result => {
          console.log('resss');
          console.log(result);
          console.log(this.formlyData.model);
          this.formlyData.model = {
            username: result.username,
            fullName: result.fullName,
            email: result.email,
            phoneNumber: result.phoneNumber,
            claims: result.claims
          }

          this.personnel.options = result.personnel != undefined ?  [ result.personnel ] : [];
          this.roles.options = result.roles != undefined ? result.roles.map(q => ({value: '0', label: q})) : [];
        })
      )
    }

    this.fetchClaims();
  }

  markRoleClaims(){
    let selectedRoles = this.roles.options.map(q => q.label) as string[];
    this.unsubscribeFromAll();
    this.subscriptions.push(
      this.userService.getRoleClaims(selectedRoles)
      .subscribe(result => {
        console.log(result);
        this.formlyData.model.claims = result;
        this.claims.forEach(q => {
            q.checked = (this.formlyData.model.claims as string[]).includes(q.value);
        })
      })
    )
  }

  fetchClaims(){
    if(this.claims != undefined)
      return;
    
    this.subscriptions.push(
      this.userService.fetchClaims()
      .subscribe(result => {
        console.log(result);
        this.claims = result.map(q => ({value: q.label, label: q.label, description: q.additional } as CheckboxItem));
      })
    )
  }

  onClaimsChange($event){
    this.formlyData.model.claims = $event;
  }

  onSubmit(){
    let userModel = this.formlyData.model;
    let addUser: AddUser = {
      id: this.userIdToUpdate,
      username: userModel.username,
      password: userModel.password,
      fullName: userModel.fullName,
      email: userModel.email,
      phoneNumber: userModel.phoneNumber,
      personnel: (this.personnel.options.length > 0) 
        ? this.personnel.options[0] 
        : undefined,
      roles: this.roles.options,
      claims: userModel.claims
    }

    let apiResponse;

    let subscribeTo: Observable<any> = this.userIdToUpdate == undefined 
      ? this.userService.addUser(addUser)
      : this.userService.updateUser(addUser)

    this.subscriptions.push(
      subscribeTo
      .subscribe(result => {
        console.log(result);
          apiResponse = result;
          if(apiResponse.status == 1){
            this.clearForm();
          }
      })
    )
  }

  clearForm(){
    this.formlyData.form.reset();
    this.roles.options = [];
    this.personnel.options = [];
    this.closeDialog();
  }

  closeDialog(){
    if(this.dialogRef != undefined)
    {
      this.dialogRef.close();
    }
  }
}
