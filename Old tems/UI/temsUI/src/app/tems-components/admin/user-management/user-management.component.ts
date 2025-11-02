import { Component, OnInit, ViewChild } from '@angular/core';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../../services/dialog.service';
import { AddUserComponent } from './add-user/add-user.component';
import { ViewUsersComponent } from './view-users/view-users.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.scss']
})
export class UserManagementComponent extends TEMSComponent implements OnInit {

  @ViewChild('viewUsers') viewUsers: ViewUsersComponent;

  constructor(
    private dialogService: DialogService
  ) { 
    super();
  }

  ngOnInit(): void {
  }

  addUser(){
    this.dialogService.openDialog(
      AddUserComponent,
      undefined,
      () => {
        this.viewUsers.fetchUsers();
      }
    )
  }
}
