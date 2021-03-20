import { ScrollingModule } from '@angular/cdk/scrolling';
import { TEMSComponent } from './../../tems/tems.component';
import { UserService } from './../../services/user-service/user.service';
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
  private loggedIn: boolean;
  
  constructor(
    config: NgbDropdownConfig,
    private route: Router,
    private userService: UserService) {
      super();
      config.placement = 'bottom-right';
  }

  isLoggedIn(): boolean{
    return localStorage.getItem('token') != null
  }

  signOut(){
    localStorage.removeItem('token');
    // this.route.navigateByUrl('');
    window.location.reload()
  }

  ngOnInit() {
    this.loggedIn = localStorage.getItem('token') != null;
    // this.subscriptions.push(
    //   this.userService.isLoggedIn()
    //   .subscribe(result => {
    //     console.log(result);

    //     if(result.status == 1)
    //       this.loggedIn = true;
    //     else
    //       this.loggedIn = false;
    //   })
    // );
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
