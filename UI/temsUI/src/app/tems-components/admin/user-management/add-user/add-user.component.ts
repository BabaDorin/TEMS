import { TranslateService } from '@ngx-translate/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyData } from '../../../../models/formly/formly-data.model';
import { IOption } from '../../../../models/option.model';
import { SnackService } from '../../../../services/snack.service';
import { CheckboxItem } from '../../../../models/checkboxItem.model';
import { Observable, of } from 'rxjs';
import { AddUser } from '../../../../models/identity/add-user.model';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { specCharValidator } from 'src/app/models/validators';
import { UserService } from 'src/app/services/user.service';
import { PersonnelService } from 'src/app/services/personnel.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { RoleService } from 'src/app/services/role.service';
import { IViewRole } from 'src/app/models/roles/role.model';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent extends TEMSComponent implements OnInit {

  @ViewChild('rolesChips') roles: ChipsAutocompleteComponent;
  @ViewChild('personnelChips') personnel: ChipsAutocompleteComponent;
  formlyData = new FormlyData();

  userIdToUpdate: string;
  // roles: IViewRole[];
  claims: CheckboxItem[];
  rolesClaims: CheckboxItem[];
  temsRoles: IViewRole[];
  roleOptions = [] as IOption[];

  dialogRef: MatDialogRef<any>;

  constructor(
    public personnelService: PersonnelService,
    private userService: UserService,
    private roleService: RoleService,
    private snackService: SnackService,
    private formlyParserService: FormlyParserService,
    private translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if (dialogData != undefined)
      if (dialogData.userIdToUpdate != undefined)
        this.userIdToUpdate = dialogData.userIdToUpdate;
  }

  fetchRoles() {
    this.subscriptions.push(
      this.roleService.getRoles()
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.temsRoles = result ?? [];
          this.roleOptions = this.temsRoles.map(role => ({ value: role.id, label: role.name } as IOption)) ?? [];
        })
    )
  }

  fetchClaims() {
    this.subscriptions.push(
      this.userService.fetchClaims()
        .subscribe(result => {
          this.claims = result.map(q => ({ value: q.label, label: q.label, description: q.additional } as CheckboxItem));
          return of(0);
        })
    );
  }

  async ngOnInit() {
    this.formlyData.fields = this.formlyParserService.parseAddUser(this.userIdToUpdate != undefined);
    this.formlyData.isVisible = true;

    // 1: Fetch claims
    // 2: Fetch roles
    // 3: Fetch user data
    // BEFREE: Get rid of subscribe inside subscribe... (Use promise)

    this.subscriptions.push(
      this.userService.fetchClaims()
        .subscribe(result => {
          // 1 - Done, claims fetched
          this.claims = result.map(q => ({ value: q.label, label: q.label, description: q.additional } as CheckboxItem));

          this.subscriptions.push(
            this.roleService.getRoles()
              .subscribe(result => {
                // 2: Done - Roles fetched

                if (this.snackService.snackIfError(result))
                  return;

                this.temsRoles = result ?? [];
                this.roleOptions = this.temsRoles.map(role => ({ value: role.id, label: role.name } as IOption)) ?? [];

                if (this.userIdToUpdate == undefined)
                  return;

                this.subscriptions.push(
                  this.userService.getUser(this.userIdToUpdate)
                    .subscribe(result => {
                      // 3: Done, user data fetched
                      console.log('userdata:') 
                      console.log(result);

                      if (this.snackService.snackIfError(result))
                        return;

                      this.formlyData.model = {
                        username: result.username,
                        fullName: result.fullName,
                        email: result.email,
                        phoneNumber: result.phoneNumber,
                        claims: result.claims
                      }

                      this.personnel.options = result.personnel != undefined ? [result.personnel] : [];
                      this.roles.options = result.roles != undefined ? result.roles.map(q => ({ label: q } as IOption)) : [];

                      
                      // this.markUserClaims();
                      // mark user claims

                      this.ensureClaimsSelected(result.claims, false, false);
                      this.markRoleClaims(false);
                    })
                );
              })
          )
        })
    );
  }

  markRoleClaims(uncheckUnselected?) {
    let selectedRoles = this.roles.options.map(q => q.label) as string[];

    this.unsubscribeFromAll();

    this.subscriptions.push(
      this.userService.getRoleClaims(selectedRoles)
        .subscribe(result => {
          let roleClaims = result;
          console.log('role claims:');
          console.log(roleClaims);
          this.ensureClaimsSelected(roleClaims, true, uncheckUnselected);
        })
    )
  }

  markUserClaims() {
    this.subscriptions.push(
      this.userService.getUserClaims(this.userIdToUpdate)
        .subscribe(result => {
          console.log('user claims i got');
          console.log(result);
          let userClaims = result;
          this.ensureClaimsSelected(userClaims);
        })
    )
  }

  ensureClaimsSelected(claimsToBeSelected: string[], disableSelected: boolean = undefined, uncheckUnselected: boolean = undefined) {
    this.claims.forEach(q => {
      if (claimsToBeSelected.includes(q.label)) {
        q.checked = true;
        // If udnefined - leave as it is
        if(disableSelected != undefined)
          q.disabled = disableSelected;
      }
      else {
        // if undefined - leave as it is
        if(uncheckUnselected == true)
          q.checked = false;
        q.disabled = false;
      }
    });

    // Update the formly model
    this.formlyData.model.claims = this.claims.map(q => q.label);
  }

  onClaimsChange($event) {
    this.formlyData.model.claims = $event;
  }

  onSubmit() {
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

    let subscribeTo: Observable<any> = this.userIdToUpdate == undefined
      ? this.userService.addUser(addUser)
      : this.userService.updateUser(addUser)

    this.subscriptions.push(
      subscribeTo
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          
          if (result.status == 1) {
            this.clearForm();
            this.snackService.snack(result);
          }
        })
    )
  }

  clearForm() {
    this.formlyData.form.reset();
    this.roles.options = [];
    this.personnel.options = [];
    this.closeDialog();
  }

  closeDialog() {
    if (this.dialogRef != undefined) {
      this.dialogRef.close();
    }
  }
}
