import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { SnackService } from 'src/app/services/snack.service';
import { UserService } from 'src/app/services/user.service';
import { ViewUserSimplified } from './../../../models/user/view-user.model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-user-cards-list',
  templateUrl: './user-cards-list.component.html',
  styleUrls: ['./user-cards-list.component.scss']
})
export class UserCardsListComponent extends TEMSComponent implements OnInit {

  @Input() users: ViewUserSimplified[];
  @Input() ofRole: string;

  @Input() canView: boolean = false;
  @Input() canManage: boolean = false;

  constructor(
    private userService: UserService,
    private snackService: SnackService,
    public translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.users == undefined)
      this.fetchUsers();
  }

  fetchUsers(){
    this.subscriptions.push(
      this.userService.getUsers(this.ofRole)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.users = result;
      })
    )
  }

}
