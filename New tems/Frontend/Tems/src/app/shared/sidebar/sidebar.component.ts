import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NavigationEnd, Router, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { filter, Subscription } from 'rxjs';
import { ClaimService } from 'src/app/services/claim.service';
import { RouteInfo } from './sidebar.metadata';
import { MenuService } from 'src/app/services/menu.service';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule
  ],
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit, OnDestroy {
  public isVisible = true;
  private bodyClassObserver?: MutationObserver;
  private routerSubscription?: Subscription;

  constructor(
    public claims: ClaimService,
    public menuService: MenuService,
    private router: Router
  ) {

  }

  ngOnInit() {
    // Check initial sidebar state from body class
    this.isVisible = !document.body.classList.contains('sidebar-hidden');
    
    // Listen to body class changes for sidebar toggle
    this.bodyClassObserver = new MutationObserver(() => {
      this.isVisible = !document.body.classList.contains('sidebar-hidden');
    });
    
    this.bodyClassObserver.observe(document.body, {
      attributes: true,
      attributeFilter: ['class']
    });

    this.syncMenuStateWithRoute();

    this.routerSubscription = this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe(() => this.syncMenuStateWithRoute());
  }

  ngOnDestroy() {
    this.bodyClassObserver?.disconnect();
    this.routerSubscription?.unsubscribe();
  }

  optionSelected(){
    let currentWidth = document.body.clientWidth;

    // Toggle sidebar automatically if the document width is under 1150px
    if(currentWidth <= 1150){
      document.body.classList.add('sidebar-hidden');
    }
  }

  closePreviousSubmenu(clickedNavItem: RouteInfo){
    if(clickedNavItem.submenu != undefined && clickedNavItem.submenu.length > 0){
      clickedNavItem.showSubmenu = !clickedNavItem.showSubmenu;
      clickedNavItem.isActive = clickedNavItem.showSubmenu || this.hasActiveSubmenu(clickedNavItem);
      return;
    }

    clickedNavItem.isActive = !clickedNavItem.isActive;
  }

  hasActiveSubmenu(navItem: RouteInfo): boolean {
    return navItem.submenu?.some(submenu => this.isCurrentRoute(submenu.path)) ?? false;
  }

  private syncMenuStateWithRoute(): void {
    for (const navItem of this.menuService.ROUTES) {
      if (!navItem.submenu?.length) {
        navItem.isActive = this.isCurrentRoute(navItem.path);
        continue;
      }

      const hasActiveSubmenu = this.hasActiveSubmenu(navItem);
      navItem.isActive = hasActiveSubmenu;
      navItem.showSubmenu = hasActiveSubmenu || navItem.showSubmenu;
    }
  }

  private isCurrentRoute(path: string): boolean {
    if (!path) {
      return false;
    }

    return this.router.isActive(path, {
      paths: 'exact',
      queryParams: 'ignored',
      fragment: 'ignored',
      matrixParams: 'ignored'
    });
  }
}
