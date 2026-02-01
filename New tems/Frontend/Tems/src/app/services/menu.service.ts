import { TokenService } from './token.service';
import { Injectable } from '@angular/core';
import { RouteInfo } from 'src/app/shared/sidebar/sidebar.metadata';
import { TranslateService } from '@ngx-translate/core';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  ROUTES: RouteInfo[];

  constructor(
    private tokenService: TokenService,
    private translate: TranslateService
  ) {
    this.translate.get(['menu'])
      .subscribe(translations => {
        let menu = translations.menu;

        this.ROUTES = [
          {
            path: '',
            title: 'Assets',
            icon: 'mdi mdi-package-variant menu-icon',
            isActive: false,
            isShown: true,
            showSubmenu: false,
            submenu: [
              {
                path: '/assets/view',
                title: 'View Assets',
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/assets/management',
                title: 'Asset Management',
                icon: 'mdi mdi-cog menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              }
            ]
          },
          {
            path: '',
            title: 'Locations',
            icon: 'mdi mdi-map-marker-radius menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: [
              {
                path: '/locations/view',
                title: 'View Locations',
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              }
            ]
          },
          {
            path: '',
            title: 'Technical Support',
            icon: 'mdi mdi-lifebuoy menu-icon',
            isShown: true,
            isActive: false,
            showSubmenu: false,
            submenu: [
              {
                path: '/technical-support/ticket-types',
                title: 'Ticket Types',
                icon: 'mdi mdi-file-document-outline menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/technical-support/tickets',
                title: 'Tickets',
                icon: 'mdi mdi-ticket menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: '',
            title: 'User Management',
            icon: 'mdi mdi-account-multiple menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageUsers(),
            showSubmenu: false,
            submenu: [
              {
                path: '/administration/users',
                title: 'Manage Users',
                icon: 'mdi mdi-account-multiple-outline menu-icon',
                isShown: this.tokenService.canManageUsers(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              }
            ]
          }
        ]
      })
  }
}
