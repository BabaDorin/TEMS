import { ProfileTicketsComponent } from './../profile/profile-tickets/profile-tickets.component';
import { ProfileAllocationsComponent } from './../profile/profile-allocations/profile-allocations.component';
import { ProfileSettingsComponent } from './../profile/profile-settings/profile-settings.component';
import { ProfileGeneralComponent } from './../profile/profile-general/profile-general.component';
import { ViewProfile } from './../../models/profile/view-profile.model';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { TEMSComponent } from './../../tems/tems.component';
import { ActivatedRoute, Router } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { SnackService } from './../../services/snack/snack.service';
import { UserService } from './../../services/user-service/user.service';
import { Component, OnInit, ViewChild } from '@angular/core';
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

  activePage = ProfileGeneralComponent;

  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private activatedRouter: ActivatedRoute,
    private tokenService: TokenService,
    private location: Location,
    private router: Router
  ) {
    super();
  }

  ngOnInit(): void {
    this.location.onUrlChange(url => {
      this.getActivePage(url);
    });
    
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
        this.getActivePage();
      })
    )
  }

  viewPage(newPage: string){
    this.location.go('/profile/view/' + this.userId + '?active=' + newPage);
  }

  getActivePage(url?){
    if(url == undefined) url = this.router.url;

    console.log(url);

    if(url == undefined || url.indexOf('?active') == -1){
      this.activePage = ProfileGeneralComponent;      
      return;
    }

    let newPage = url.split('?active=')[1];

    switch(newPage){
      case 'general': this.activePage = ProfileGeneralComponent; break;
      case 'allocations': this.activePage = ProfileAllocationsComponent; break;
      case 'issues': this.activePage = ProfileTicketsComponent; break;
      case 'settings': this.activePage = ProfileSettingsComponent; break;
      default: this.activePage = ProfileGeneralComponent; break;
    }
  }
}
