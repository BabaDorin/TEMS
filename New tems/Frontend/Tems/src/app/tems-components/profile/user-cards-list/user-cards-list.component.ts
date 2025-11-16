import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateService, TranslateModule } from '@ngx-translate/core';
import { UserService } from '../../../services/user.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from '../../../tems/tems.component';
import { UserCardComponent } from '../user-card/user-card.component';

@Component({
  selector: 'app-user-cards-list',
  standalone: true,
  imports: [CommonModule, UserCardComponent, TranslateModule],
  templateUrl: './user-cards-list.component.html',
  styleUrls: ['./user-cards-list.component.scss']
})
export class UserCardsListComponent extends TEMSComponent implements OnInit {
  @Input() users: any[];

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
