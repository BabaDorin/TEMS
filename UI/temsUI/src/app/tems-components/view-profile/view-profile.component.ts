import { Location } from '@angular/common';
import { Component, Injector, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { DialogService } from '../../services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { TokenService } from '../../services/token.service';
import { UserService } from '../../services/user.service';
import { ProfileSettingsComponent } from '../profile/profile-settings/profile-settings.component';
import { TEMSComponent } from './../../tems/tems.component';
import { ProfileAnalyticsComponent } from './../profile/profile-analytics/profile-analytics.component';
import { ProfileGeneralComponent } from './../profile/profile-general/profile-general.component';
import { ProfileTicketsComponent } from './../profile/profile-tickets/profile-tickets.component';


@Component({
  selector: 'app-view-profile',
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.scss']
})
export class ViewProfileComponent extends TEMSComponent implements OnInit {

  userId: string;
  isCurrentUser: boolean;
  profile;
  activePage: any = ProfileGeneralComponent;
  injector: Injector;
  
  profilePhotoUrl = "assets/svgs/default_avatar.svg";

  constructor(
    private userService: UserService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private activatedRouter: ActivatedRoute,
    private tokenService: TokenService,
    private location: Location,
    private router: Router,
    private inj: Injector,
  ) {
    super();
  };

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

    this.isCurrentUser = this.userId == this.tokenService.getClaimValue('UserID');

    this.subscriptions.push(
      this.userService.getProfileData(this.userId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.profile = result;
        this.injector = Injector.create([
          { provide: ViewProfile, useValue: this.profile },
          { provide: Boolean, useValue: this.isCurrentUser },
        ], this.inj);
        this.getActivePage();
      })
    )
  }

  viewPage(newPage: string){
    this.location.go('/profile/view/' + this.userId + '?active=' + newPage);
  }

  getActivePage(url?){
    if(url == undefined) url = this.router.url;

    if(url == undefined || url.indexOf('?active') == -1){
      this.activePage = ProfileGeneralComponent;      
      return;
    }

    let newPage = url.split('?active=')[1];

    switch(newPage){
      case 'general': this.activePage = ProfileGeneralComponent; break;
      case 'issues': this.activePage = ProfileTicketsComponent; break;
      case 'settings': this.activePage = ProfileSettingsComponent; break;
      case 'analytics': this.activePage = ProfileAnalyticsComponent; break;
      default: this.activePage = ProfileGeneralComponent; break;
    }
  }
}
