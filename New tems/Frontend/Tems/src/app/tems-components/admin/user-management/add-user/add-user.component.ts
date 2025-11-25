import { Component, Inject, OnInit, Optional, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatExpansionModule } from '@angular/material/expansion';
import { TranslateModule } from '@ngx-translate/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { CheckboxGroupComponent } from 'src/app/shared/forms/checkbox-group/checkbox-group.component';
import { Observable, of } from 'rxjs';
import { IViewRole } from 'src/app/models/roles/role.model';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { PersonnelService } from 'src/app/services/personnel.service';
import { RoleService } from 'src/app/services/role.service';
import { UserService } from 'src/app/services/user.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { CheckboxItem } from '../../../../models/checkboxItem.model';
import { FormlyData } from '../../../../models/formly/formly-data.model';
import { AddUser } from '../../../../models/identity/add-user.model';
import { IOption } from '../../../../models/option.model';
import { SnackService } from '../../../../services/snack.service';
import { TEMS_FORMS_IMPORTS } from 'src/app/modules/tems-forms/tems-forms.module';

@Component({
  selector: 'app-add-user',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatExpansionModule,
    TranslateModule,
    ChipsAutocompleteComponent,
    CheckboxGroupComponent,
    ...TEMS_FORMS_IMPORTS
  ],
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
    public translate: TranslateService,
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
          this.claims = result.map(q => ({ value: q.label, label: this.translate.instant('user.claimOptions.' + q.label), description: this.translate.instant('user.claimTips.' + q.additional), } as CheckboxItem));
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
          this.claims = result.map(q => ({ value: q.label, label: this.translate.instant('user.claimOptions.' + q.label), description: this.translate.instant('user.claimTips.' + q.additional), } as CheckboxItem));

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
                      if (this.snackService.snackIfError(result))
                        return;

                      this.formlyData.model = {
                        username: result.username,
                        fullName: result.fullName,
                        email: result.email,
                        phoneNumber: result.phoneNumber,
                        claims: result.claims
                      }

                      this.personnel.selectOptions = result.personnel != undefined ? [result.personnel] : [];
                      this.roles.selectOptions = result.roles != undefined ? result.roles.map(q => ({ label: q } as IOption)) : [];

                      
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
    let selectedRoles = this.roles.selectOptions.map(q => q.label) as string[];

    this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
    this.subscriptions = [];

    this.subscriptions.push(
      this.userService.getRoleClaims(selectedRoles)
        .subscribe(result => {
          let roleClaims = result;
          this.ensureClaimsSelected(roleClaims, true, uncheckUnselected);
        })
    )
  }

  markUserClaims() {
    this.subscriptions.push(
      this.userService.getUserClaims(this.userIdToUpdate)
        .subscribe(result => {
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
    this.formlyData.model.claims = this.claims.filter(q => q.checked).map(q => q.label);
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
      personnel: (this.personnel.selectOptions.length > 0)
        ? this.personnel.selectOptions[0]
        : undefined,
      roles: this.roles.selectOptions,
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
    this.roles.selectOptions = [];
    this.personnel.selectOptions = [];
    this.closeDialog();
  }

  closeDialog() {
    if (this.dialogRef != undefined) {
      this.dialogRef.close();
    }
  }
}
