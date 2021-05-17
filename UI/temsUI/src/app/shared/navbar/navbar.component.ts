import { SnackService } from './../../services/snack/snack.service';
import { ViewNotification } from './../../models/communication/notification/view-notification.model';
import { UserService } from './../../services/user-service/user.service';
import { AuthService } from './../../services/auth.service';
import { TEMSComponent } from './../../tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
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
  notifications = [] as ViewNotification[];

  constructor(
    config: NgbDropdownConfig,
    private route: Router,
    private authService: AuthService,
    private userService: UserService,
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

  removeNotification(notificationId: string){
    this.subscriptions.push(
      this.userService.removeNotification(notificationId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
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

  fetchLastNotifications(){
    this.subscriptions.push(
      this.userService.getLastNotifications()
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.notifications = result;
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
}
