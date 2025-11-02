import { Component, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { UserService } from 'src/app/services/user.service';
import { DialogService } from '../../../../services/dialog.service';
import { ViewUserSimplified } from './../../../../models/user/view-user.model';
import { TEMSComponent } from './../../../../tems/tems.component';

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
