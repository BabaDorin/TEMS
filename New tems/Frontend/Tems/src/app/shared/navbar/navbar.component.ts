import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatBadgeModule } from '@angular/material/badge';
import { TranslateModule } from '@ngx-translate/core';
import { MatButtonModule } from '@angular/material/button';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
import { DialogService } from 'src/app/services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { UserService } from '../../services/user.service';
import { ViewNotification } from './../../models/communication/notification/view-notification.model';
import { AuthService } from './../../services/auth.service';
import { ViewNotificationsComponent } from './../../tems-components/notifications/view-notifications/view-notifications.component';
import { TEMSComponent } from './../../tems/tems.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatIconModule,
    MatMenuModule,
    MatBadgeModule,
    TranslateModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  providers: [NgbDropdownConfig]
})
export class NavbarComponent extends TEMSComponent implements OnInit {
  public iconOnlyToggled = false;
  public sidebarToggled = false;
  public username: string;
  public profilePhotoB64: string;
  public loggedIn: boolean;
  public refreshing: boolean = false;

  notifications = [] as ViewNotification[];
  newNotifications = [] as ViewNotification[];
  // @ViewChild('notificationDropdown') notificationDropdown: NgbDropdown; 

  constructor(
    config: NgbDropdownConfig,
    private route: Router,
    private authService: AuthService,
    private userService: UserService,
    private dialogService: DialogService,
    private snackService: SnackService) {
      super();
      config.placement = 'bottom-right';
  }

  signOut(){
    this.authService.signOut().then(() => {
      window.location.href = '';
    });
  }

  markNotificationsAsSeen(){
    this.newNotifications = this.notifications.filter(q => q.seen == false);
    if(this.newNotifications == undefined || this.newNotifications.length == 0)
      return;


    this.subscriptions.push(
      this.userService.markNotificationsAsSeen(this.newNotifications.map(q => q.id))
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
          
          this.newNotifications.forEach(q => q.seen = true);
          this.newNotifications = [];
      })
    )
  }

  ngOnInit() {
    let token = localStorage.getItem('token');
    if(token == undefined)
    {
      this.loggedIn = false;
      return;
    }

    this.loggedIn = true;
    let jwtData = token.split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)
    this.username = decodedJwtData.Username;
    this.fetchLastNotifications();
    this.fetchMinifiedProfilePhoto();
  }

  fetchMinifiedProfilePhoto(){
    // Temporarily disabled - backend endpoint not yet implemented
    // TODO: Implement profile photo endpoint in backend API
    // this.subscriptions.push(
    //   this.userService.fetchMinifiedProfilePhoto()
    //   .subscribe(result => {
    //     this.profilePhotoB64 = result;
    //   })
    // );
  }

  fetchLastNotifications(refreshTriggered: boolean = false){
    // Temporarily disabled - backend endpoint not yet implemented
    // TODO: Implement notifications endpoint in backend API
    this.refreshing = true;

    // this.subscriptions.push(
    //   this.userService.getLastNotifications()
    //   .subscribe(result => {
    //     if(this.snackService.snackIfError(result))
    //       return;
    //     
    //     this.notifications = result.slice(0, 5);
    //     this.newNotifications = this.notifications.filter(q => q.seen == false);
    //     this.refreshing = false;
    //
    //     if(refreshTriggered)
    //       this.markNotificationsAsSeen();
    //   })
    // );
    
    this.refreshing = false;
  }

  // toggle sidebar in small devices
  toggleOffcanvas() {
    document.querySelector('.sidebar-offcanvas').classList.toggle('active');
  }

  // toggle sidebar
  toggleSidebar() {
    let body = document.querySelector('body');
    if((!body.classList.contains('sidebar-toggle-display')) && (!body.classList.contains('sidebar-absolute'))) {
      this.iconOnlyToggled = !this.iconOnlyToggled;
      if(this.iconOnlyToggled) {
        body.classList.add('sidebar-icon-only');
      } else {
        body.classList.remove('sidebar-icon-only');
      }
    } else {
      this.sidebarToggled = !this.sidebarToggled;
      if(this.sidebarToggled) {
        body.classList.add('sidebar-hidden');
      } else {
        body.classList.remove('sidebar-hidden');
      }
    }
  }

  // toggle right sidebar
  toggleRightSidebar() {
    document.querySelector('#right-sidebar').classList.toggle('open');
  }

  displayAllNotifications(){
    this.dialogService.openDialog(
      ViewNotificationsComponent,
      undefined,
      () => {
        this.fetchLastNotifications();
      }
    );
  }
}
