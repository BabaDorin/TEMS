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

  edit(userId: string, index: number){
    this.dialogService.openDialog(
      AddUserComponent,
      [{value: userId, label: "userIdToUpdate"}],
      () => {
        this.userService.getUserSimplifiedById(userId)
        .subscribe(result => {
          this.users[index] = result;
        })
      }
    )
  }

  remove(userId: string, index: number){
    if(!confirm("Are you sure you want to remove that user?"))
      return;

    console.log(index);
    this.subscriptions.push(
      this.userService.archieveUser(userId)
      .subscribe(result => {
        if(result.status == 1)
          this.users.splice(index, 1);
      })
    )
  }

  fetchUsers(){
    this.unsubscribeFromAll();
    this.subscriptions.push(
      this.userService.getUsers()
      .subscribe(result => {
        console.log(result);
        this.users = result;
      })
    )
  }
}
