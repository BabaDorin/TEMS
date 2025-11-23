import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgbCollapseModule } from '@ng-bootstrap/ng-bootstrap';
import { ClaimService } from 'src/app/services/claim.service';
import { RouteInfo } from './sidebar.metadata';
import { SidebarManager } from './sidebar-shared-functionalities';
import { MenuService } from 'src/app/services/menu.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule,
    NgbCollapseModule
  ],
  templateUrl: './sidebar.component.html'
})
export class SidebarComponent implements OnInit {
  public uiBasicCollapsed = false;
  public samplePagesCollapsed = false;
  public sidebarNavItems: RouteInfo[] = [];
  private sidebarManager: SidebarManager;
  public isVisible = true;

  constructor(
    public claims: ClaimService,
    public menuService: MenuService
  ) {

  }

  ngOnInit() {
    // Check initial sidebar state from body class
    this.isVisible = !document.body.classList.contains('sidebar-hidden');
    
    // Listen to body class changes for sidebar toggle
    const observer = new MutationObserver(() => {
      this.isVisible = !document.body.classList.contains('sidebar-hidden');
    });
    
    observer.observe(document.body, {
      attributes: true,
      attributeFilter: ['class']
    });
  }

  optionSelected(){
    let currentWidth = document.body.clientWidth;

    // Toggle sidebar automatically if the document width is under 1150px
    if(currentWidth <= 1150){
      document.body.classList.add('sidebar-hidden');
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
