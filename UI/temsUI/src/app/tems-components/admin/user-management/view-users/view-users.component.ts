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
    public dialog: MatDialog
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.userService.getUsers()
      .subscribe(result => {
        console.log(result);
        this.users = result;
      })
    )
  }

  edit(userId: string){
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddUserComponent);

    dialogRef.componentInstance.userIdToUpdate = userId;
    dialogRef.componentInstance.dialogRef = dialogRef;

    dialogRef.afterClosed().subscribe(result => {

    })
  }
}
