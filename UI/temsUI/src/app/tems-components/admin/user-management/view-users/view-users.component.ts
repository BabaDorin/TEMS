import { DialogService } from './../../../../services/dialog-service/dialog.service';
import { AddUserComponent } from './../../../../administration/add-user/add-user.component';
import { ViewUserSimplified } from './../../../../models/user/view-user.model';
import { TEMSComponent } from './../../../../tems/tems.component';
import { UserService } from 'src/app/services/user-service/user.service';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-view-users',
  templateUrl: './view-users.component.html',
  styleUrls: ['./view-users.component.scss']
})
export class ViewUsersComponent extends TEMSComponent implements OnInit {

  users: ViewUserSimplified[];
  pageNumber: 1;

  constructor(
    private userService: UserService,
    public dialog: MatDialog,
    private dialogService: DialogService
  ) {
    super();
  }

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(){
    this.unsubscribeFromAll();
    this.subscriptions.push(
      this.userService.getUsers()
      .subscribe(result => {
        if(result != this.users)
          this.users = result;
      })
    )
  }

  userRemoved(index){
    this.users.splice(index, 1);
  }
}
