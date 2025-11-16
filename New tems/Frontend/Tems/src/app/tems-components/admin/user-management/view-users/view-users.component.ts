import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { AgGridModule } from 'ag-grid-angular';
import { UserContainerComponent } from '../../../identity/user-container/user-container.component';
import { UserService } from 'src/app/services/user.service';
import { DialogService } from '../../../../services/dialog.service';
import { ViewUserSimplified } from './../../../../models/user/view-user.model';
import { TEMSComponent } from './../../../../tems/tems.component';

@Component({
  selector: 'app-view-users',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslateModule,
    NgxPaginationModule,
    AgGridModule,
    UserContainerComponent
  ],
  templateUrl: './view-users.component.html',
  styleUrls: ['./view-users.component.scss']
})
export class ViewUsersComponent extends TEMSComponent implements OnInit {

  users: ViewUserSimplified[];
  pageNumber: 1;

  constructor(
    private userService: UserService,
    private dialogService: DialogService
  ) {
    super();
  }

  ngOnInit(): void {
    this.fetchUsers();
  }

  fetchUsers(){
    this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
    this.subscriptions = [];
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
