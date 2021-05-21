import { ViewUserSimplified } from './../../../models/user/view-user.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from 'src/app/services/snack/snack.service';
import { UserService } from 'src/app/services/user-service/user.service';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-cards-list',
  templateUrl: './user-cards-list.component.html',
  styleUrls: ['./user-cards-list.component.scss']
})
export class UserCardsListComponent extends TEMSComponent implements OnInit {

  @Input() users: ViewUserSimplified[];
  @Input() ofRole: string;

  constructor(
    private userService: UserService,
    private snackService: SnackService
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
        console.log('result: ');
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.users = result;
      })
    )
  }

}
