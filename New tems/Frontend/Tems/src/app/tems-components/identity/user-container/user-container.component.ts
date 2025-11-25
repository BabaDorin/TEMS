import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { TranslateModule } from '@ngx-translate/core';
import { ConfirmService } from 'src/app/confirm.service';
import { AddUserComponent } from 'src/app/tems-components/admin/user-management/add-user/add-user.component';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { UserService } from '../../../services/user.service';
import { ViewUserSimplified } from './../../../models/user/view-user.model';

@Component({
  selector: 'app-user-container',
  standalone: true,
  imports: [
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    TranslateModule
  ],
  templateUrl: './user-container.component.html',
  styleUrls: ['./user-container.component.scss']
})
export class UserContainerComponent extends TEMSComponent implements OnInit {

  @Input() user: ViewUserSimplified;
  @Output() removed = new EventEmitter();
  
  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private confirmService: ConfirmService
  ) {
    super();
  }

  ngOnInit(): void {

  }

  edit(){
    this.dialogService.openDialog(
      AddUserComponent,
      [{value: this.user.id, label: "userIdToUpdate"}],
      () => {
        this.userService.getUserSimplifiedById(this.user.id)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          
          this.user = result;
        })
      }
    )
  }

  async remove(){
    if(!await this.confirmService.confirm("Are you sure you want to remove that user?"))
      return;

    this.subscriptions.push(
      this.userService.archieveUser(this.user.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.removed.emit();
      })
    )
  }

}
