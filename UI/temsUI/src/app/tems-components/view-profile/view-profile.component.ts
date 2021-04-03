import { ViewProfile } from './../../models/profile/view-profile.model';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { TEMSComponent } from './../../tems/tems.component';
import { ActivatedRoute } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { SnackService } from './../../services/snack/snack.service';
import { UserService } from './../../services/user-service/user.service';
import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common'


@Component({
  selector: 'app-view-profile',
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.scss']
})
export class ViewProfileComponent extends TEMSComponent implements OnInit {

  userId: string;
  isCurrentUser: boolean;
  profile = new ViewProfile();

  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private activatedRouter: ActivatedRoute,
    private tokenService: TokenService,
    private location: Location
  ) {
    super();
  }

  ngOnInit(): void {
    this.userId = this.activatedRouter.snapshot.paramMap.get("id");
    let currentUserId = this.tokenService.getClaimValue('UserID');

    if(this.userId == undefined && currentUserId != undefined){
      this.userId = currentUserId,
      this.location.go('/profile/view/' + currentUserId);
    }

    this.isCurrentUser = (this.userId == this.tokenService.getClaimValue('UserID')) || this.tokenService.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
    
    this.subscriptions.push(
      this.userService.getProfileData(this.userId)
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;

        this.profile = result;
      })
    )
  }
}
