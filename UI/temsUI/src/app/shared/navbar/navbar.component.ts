import { ViewNotificationsComponent } from './../../tems-components/notifications/view-notifications/view-notifications.component';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { SnackService } from './../../services/snack/snack.service';
import { ViewNotification } from './../../models/communication/notification/view-notification.model';
import { UserService } from './../../services/user-service/user.service';
import { AuthService } from './../../services/auth.service';
import { TEMSComponent } from './../../tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbDropdown, NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
import { Router } from '@angular/router';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss'],
  providers: [NgbDropdownConfig]
})
export class NavbarComponent extends TEMSComponent implements OnInit {
  public iconOnlyToggled = false;
  public sidebarToggled = false;
  public username: string;
  private loggedIn: boolean;
  private refreshing: boolean = false;

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
    console.log('got here');
    this.subscriptions.push(
      this.authService.signOut()
      .subscribe());
    this.route.navigateByUrl('');
    window.location.reload()
  }

  markNotificationsAsSeen(){
    this.newNotifications = this.notifications.filter(q => q.seen == false);
    console.log('all notifs')
    console.log(this.notifications);

    console.log('new notifs');
    console.log(this.newNotifications);
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
  }

  fetchLastNotifications(refreshTriggered: boolean = false){
    // If refreshTriggered == true => user fetched notifications manually, using the fresh button.
    this.refreshing = true;

    this.subscriptions.push(
      this.userService.getLastNotifications()
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.notifications = result;
        this.newNotifications = this.notifications.filter(q => q.seen == false);
        console.log(this.newNotifications);
        this.refreshing = false;

        if(refreshTriggered)
          this.markNotificationsAsSeen();
      })
    )
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
