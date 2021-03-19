import { specCharValidator } from 'src/app/models/validators';
import { UserService } from 'src/app/services/user-service/user.service';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-add-user',
  templateUrl: './add-user.component.html',
  styleUrls: ['./add-user.component.scss']
})
export class AddUserComponent extends TEMSComponent implements OnInit {

  formGroup = new FormGroup({
    fullName: new FormControl(),
    email: new FormControl(),
    phoneNumber: new FormControl(),
    roles: new FormControl(),
    personnel: new FormControl(),
    username: new FormControl('', [ Validators.required, specCharValidator ]),
    password: new FormControl('', [ Validators.required]),
  });

  rolesOptions: IOption[];
  personnelOptions: IOption[];

  constructor(
    private personnelService: PersonnelService,
    private userService: UserService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.rolesOptions = this.userService.getRoles();
    this.subscriptions.push(this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        this.personnelOptions = result;
      }));
  }

  onSubmit(){
    console.log(this.formGroup);
  }
}
