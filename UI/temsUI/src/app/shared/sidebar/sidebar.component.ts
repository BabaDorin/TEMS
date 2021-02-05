import { Role } from './../../models/role.model';
import { Component, OnInit } from '@angular/core';
import { menuItems } from './menu-items';
import { RouteInfo } from './sidebar.metadata';
import { UserService } from 'src/app/services/user-service/user.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  providers: [ UserService ]
})
export class SidebarComponent implements OnInit {
  public uiBasicCollapsed = false;
  public samplePagesCollapsed = false;
  public sidebarNavItems: RouteInfo[] = [];

  constructor(private userservice: UserService) { }

  ngOnInit() {
    const body = document.querySelector('body');

    this.sidebarNavItems = new menuItems(this.userservice.role).ROUTES.filter(sidebarnavItem => sidebarnavItem);
    console.log(this.sidebarNavItems.length);

    // add class 'hover-open' to sidebar navitem while hover in sidebar-icon-only menu
    document.querySelectorAll('.sidebar .nav-item').forEach(function (el) {
      el.addEventListener('mouseover', function() {
        if(body.classList.contains('sidebar-icon-only')) {
          el.classList.add('hover-open');
        }
      });
      el.addEventListener('mouseout', function() {
        if(body.classList.contains('sidebar-icon-only')) {
          el.classList.remove('hover-open');
        }
      });
    });
  }

}
