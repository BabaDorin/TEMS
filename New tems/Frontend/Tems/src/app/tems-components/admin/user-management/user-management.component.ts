import { Component, OnInit, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../../services/dialog.service';
import { AddUserComponent } from './add-user/add-user.component';
import { ViewUsersComponent } from './view-users/view-users.component';

@Component({
  selector: 'app-user-management',
  standalone: true,
  imports: [CommonModule, MatTabsModule, MatCardModule, MatButtonModule, MatIconModule, TranslateModule, ViewUsersComponent],
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
