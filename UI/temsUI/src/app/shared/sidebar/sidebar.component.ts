import { SidebarManager } from './sidebar-shared-functionalities';
import { MenuService } from './../../services/menu-service/menu.service';
import { Role } from '../../models/role.model';
import { Component, OnInit } from '@angular/core';
import { menuItems } from './menu-items';
import { RouteInfo } from './sidebar.metadata';
import { UserService } from 'src/app/services/user-service/user.service';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  providers: [UserService]
})
export class SidebarComponent implements OnInit {
  public uiBasicCollapsed = false;
  public samplePagesCollapsed = false;
  public sidebarNavItems: RouteInfo[] = [];
  private sidebarManager: SidebarManager;

  constructor(
    private userservice: UserService,
    private menuService: MenuService
  ) {

  }

  ngOnInit() {
    const body = document.querySelector('body');

    // this.sidebarNavItems = this.menuService.ROUTES.filter(sidebarnavItem => sidebarnavItem);
    this.sidebarNavItems = this.menuService.ROUTES;

    document.querySelectorAll('.sidebar .nav-item').forEach(function (el) {
      el.addEventListener('mouseover', function () {
        if (body.classList.contains('sidebar-icon-only')) {
          el.classList.add('hover-open');
        }
      });
      el.addEventListener('mouseout', function () {
        if (body.classList.contains('sidebar-icon-only')) {
          el.classList.remove('hover-open');
        }
      });
    });
  }

  optionSelected(){
    console.log('got here');
    // Toggle sidebar automatically if the screen width is under 1150px
    if(window.screen.width <= 1150){
      if(this.sidebarManager == undefined)
        this.sidebarManager = new SidebarManager();
      
      this.sidebarManager.toggleSidebar();
    }
  }
}
