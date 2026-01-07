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
            title: menu.equipment,
            icon: 'mdi mdi-desktop-mac menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: [
              {
                path: '/asset/all',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/asset/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/equipment',
                title: menu.quickAccess,
                icon: 'mdi mdi-crosshairs-gps menu-icon',
                isShown: this.tokenService.canManageAssets(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/asset/allocate',
                title: menu.allocate,
                icon: 'mdi mdi-transfer menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/asset/allocations',
                title: menu.allocations,
                icon: 'mdi mdi-history menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/asset/generate-temsid',
                title: menu.generateTEMSID,
                icon: 'mdi mdi-label menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: '',
            title: menu.rooms,
            icon: 'mdi mdi-panorama-wide-angle menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: [
              {
                path: '/rooms/view',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/rooms/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/rooms',
                title: menu.quickAccess,
                icon: 'mdi mdi-crosshairs-gps menu-icon',
                isShown: this.tokenService.canManageAssets(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              },
              // {
              //   path: '/rooms/map',
              //   title: 'College Map',
              //   icon: 'mdi mdi-map menu-icon',
              //   isActive: false,
              //   isShown: this.hasClaim(CAN_VIEW_ENTITIES),
              //   showSubmenu: false,
              //   submenu: []
              // },
            ]
          },
          {
            path: '',
            title: menu.personnel,
            icon: 'mdi mdi-account-multiple menu-icon',
            isShown: this.tokenService.canManageAssets(),
            isActive: false,
            showSubmenu: false,
            submenu: [
              {
                path: '/personnel/all',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/personnel/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/personnel',
                title: menu.quickAccess,
                icon: 'mdi mdi-crosshairs-gps menu-icon',
                isShown: this.tokenService.canManageAssets(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              }
            ]
          },
          {
            path: '',
            title: menu.keys,
            icon: 'mdi mdi-key-variant menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets() || this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: [
              {
                path: '/keys/all',
                title: menu.view,
                icon: 'mdi mdi-key-change menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets() || this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/keys/allocations',
                title: menu.allocations,
                icon: 'mdi mdi-account-search menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets() || this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/keys/allocate',
                title: menu.allocate,
                icon: 'mdi mdi-account-key menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
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
                title: 'Assets',
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
            path: '/library/all',
            title: menu.library,
            icon: 'mdi mdi-view-list menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: []
          },
          {
            path: 'reports',
            title: menu.report,
            icon: 'mdi mdi-printer menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: []
          },
          {
            path: '',
            title: menu.administration,
            icon: 'mdi mdi-account-star menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: [
              {
                path: '/administration/equipment',
                title: menu.equipment,
                icon: 'mdi mdi-database-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/users',
                title: menu.users,
                icon: 'mdi mdi-account-multiple-outline menu-icon',
                isShown: this.tokenService.canManageAssets(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/system-configuration',
                title: menu.configuration,
                icon: 'mdi mdi-sitemap menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/system-logs',
                title: menu.systemLogs,
                icon: 'mdi mdi-menu menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/bug-reports',
                title: menu.bugReports,
                icon: 'mdi mdi-bug menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageAssets(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          //   path: '/analytics',
          //   title: 'Analytics',
          //   icon: 'mdi mdi-chart-bar menu-icon',
          //   isActive: false,
          //   isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
          //   showSubmenu: false,
          //   submenu: []
          // },
          {
            path: '/archieve',
            title: menu.archive,
            icon: 'mdi mdi-file-sync menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageAssets(),
            showSubmenu: false,
            submenu: []
          }
        ]
      })
  }
}
