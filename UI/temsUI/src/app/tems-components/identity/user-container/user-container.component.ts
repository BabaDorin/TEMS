import { DialogService } from './../../../services/dialog-service/dialog.service';
import { SnackService } from './../../../services/snack/snack.service';
import { UserService } from './../../../services/user-service/user.service';
import { ViewUserSimplified } from './../../../models/user/view-user.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { AddUserComponent } from 'src/app/administration/add-user/add-user.component';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-user-container',
  templateUrl: './user-container.component.html',
  styleUrls: ['./user-container.component.scss']
})
export class UserContainerComponent extends TEMSComponent implements OnInit {

  @Input() user: ViewUserSimplified;
  @Output() removed = new EventEmitter();
  
  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService
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

  remove(userId: string, index: number){
    if(!confirm("Are you sure you want to remove that user?"))
      return;

    this.subscriptions.push(
      this.userService.archieveUser(userId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.removed.emit();
      })
    )
  }

}
