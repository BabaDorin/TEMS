import { Component, OnInit } from '@angular/core';
import { MenuService } from '../../services/menu.service';
import { SidebarManager } from './sidebar-shared-functionalities';
import { RouteInfo } from './sidebar.metadata';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
  public uiBasicCollapsed = false;
  public samplePagesCollapsed = false;
  public sidebarNavItems: RouteInfo[] = [];
  private sidebarManager: SidebarManager;

  constructor(
    public menuService: MenuService
  ) {

  }

  ngOnInit() {
    const body = document.querySelector('body');

    // // this.sidebarNavItems = this.menuService.ROUTES.filter(sidebarnavItem => sidebarnavItem);
    // this.sidebarNavItems = this.menuService.ROUTES;

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
    let currentWidth = document.body.clientWidth 

    // Toggle sidebar automatically if the document width is under 1150px
    if(currentWidth <= 1150){
      if(this.sidebarManager == undefined)
        this.sidebarManager = new SidebarManager();
      
      this.sidebarManager.toggleSidebar();
    }
  }

  closePreviousSubmenu(clickedNavItem: RouteInfo){
    let toBeSet = false;

    if(clickedNavItem.showSubmenu != undefined)
      toBeSet = !clickedNavItem.showSubmenu;

    this.menuService.ROUTES.forEach(q => {
      q.showSubmenu = false;
      q.isActive = false;
      q.submenu.forEach(q => q.isActive = false)
    });

    if(clickedNavItem.submenu != undefined){
      clickedNavItem.showSubmenu = toBeSet;
      clickedNavItem.isActive = toBeSet;
      return;
    }

    clickedNavItem.isActive = !clickedNavItem.isActive;
  }
}
