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

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent extends TEMSComponent implements OnInit {

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
    this.formlyData.fields = this.formlyParserService.parseAddUser();

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
  }

  onSubmit(){
    let userModel = this.formlyData.model.addUser;
    let addUser: AddUser = {
      username: userModel.username,
      password: userModel.password,
      fullName: userModel.fullName,
      email: userModel.email,
      phoneNumber: userModel.phoneNumber,
      personnel: (this.personnel.options.length > 0) 
        ? this.personnel.options[0] 
        : undefined,
      roles: this.roles.options,
    }

    this.subscriptions.push(
      this.userService.addUser(addUser)
      .subscribe(result => {
        console.log(result);
        if(result.status == 1)
          this.formlyData.form.reset();
          this.roles.options = [];
          this.personnel.options = [];
      })
    )
  }
}
