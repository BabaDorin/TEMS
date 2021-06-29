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
            isShown: this.tokenService.canViewEntities(),
            showSubmenu: false,
            submenu: [
              {
                path: '/equipment/all',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/equipment/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/equipment',
                title: menu.quickAccess,
                icon: 'mdi mdi-format-horizontal-align-right menu-icon',
                isShown: this.tokenService.canViewEntities(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/equipment/allocate',
                title: menu.allocate,
                icon: 'mdi mdi-transfer menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/equipment/allocations',
                title: menu.allocations,
                icon: 'mdi mdi-transfer menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
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
            isShown: this.tokenService.canViewEntities(),
            showSubmenu: false,
            submenu: [
              {
                path: '/rooms/view',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/rooms/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/rooms',
                title: menu.quickAccess,
                icon: 'mdi mdi-format-horizontal-align-right menu-icon',
                isShown: this.tokenService.canViewEntities(),
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
            isShown: this.tokenService.canViewEntities(),
            isActive: false,
            showSubmenu: false,
            submenu: [
              {
                path: '/personnel/all',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/personnel/add',
                title: menu.add,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/quick-access/personnel',
                title: menu.quickAccess,
                icon: 'mdi mdi-format-horizontal-align-right menu-icon',
                isShown: this.tokenService.canViewEntities(),
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
            isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
            showSubmenu: false,
            submenu: [
              {
                path: '/keys/all',
                title: menu.view,
                icon: 'mdi mdi-key-change menu-icon',
                isActive: false,
                isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/keys/allocations',
                title: menu.allocations,
                icon: 'mdi mdi-account-search menu-icon',
                isActive: false,
                isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/keys/allocate',
                title: menu.allocate,
                icon: 'mdi mdi-account-key menu-icon',
                isActive: false,
                isShown: this.tokenService.canAllocateKeys(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: '',
            title: menu.issues,
            icon: 'mdi mdi-information menu-icon',
            isActive: false,
            isShown: true,
            showSubmenu: false,
            submenu: [
              {
                path: '/issues/all',
                title: menu.view,
                icon: 'mdi mdi-alert menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/issues/create',
                title: menu.create,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: '',
            title: menu.communication,
            icon: 'mdi mdi-information-variant menu-icon',
            isShown: true,
            isActive: false,
            showSubmenu: false,
            submenu: [
              {
                path: '/communication/announcements',
                title: menu.announcements,
                icon: 'mdi mdi-bullhorn menu-icon',
                isActive: false,
                isShown: true,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/communication/logs',
                title: menu.logs,
                icon: 'mdi mdi-format-align-center menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/communication/sendemail',
                title: menu.emails,
                icon: 'mdi mdi-email menu-icon',
                isActive: false,
                isShown: this.tokenService.canSendEmails(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: '',
            title: menu.library,
            icon: 'mdi mdi-microsoft menu-icon',
            isActive: false,
            isShown: this.tokenService.canViewEntities(),
            showSubmenu: false,
            submenu: [
              {
                path: '/library/all',
                title: menu.view,
                icon: 'mdi mdi-view-list menu-icon',
                isActive: false,
                isShown: this.tokenService.canViewEntities(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/library/add',
                title: menu.upload,
                icon: 'mdi mdi-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageEntities(),
                showSubmenu: false,
                submenu: []
              },
            ]
          },
          {
            path: 'reports',
            title: menu.report,
            icon: 'mdi mdi-printer menu-icon',
            isActive: false,
            isShown: this.tokenService.canViewEntities(),
            showSubmenu: false,
            submenu: []
          },
          {
            path: '',
            title: menu.administration,
            icon: 'mdi mdi-account-star menu-icon',
            isActive: false,
            isShown: this.tokenService.canManageSystemConfiguration(),
            showSubmenu: false,
            submenu: [
              {
                path: '/administration/equipment',
                title: menu.equipment,
                icon: 'mdi mdi-database-plus menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageSystemConfiguration(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/users',
                title: menu.users,
                icon: 'mdi mdi-account-multiple-outline menu-icon',
                isShown: this.tokenService.canManageSystemConfiguration(),
                isActive: false,
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/system-configuration',
                title: menu.configuration,
                icon: 'mdi mdi-sitemap menu-icon',
                isActive: false,
                isShown: this.tokenService.canManageSystemConfiguration(),
                showSubmenu: false,
                submenu: []
              },
              {
                path: '/administration/system-logs',
                title: menu.systemLogs,
                icon: 'mdi mdi-playlist-remove',
                isActive: false,
                isShown: this.tokenService.canManageSystemConfiguration(),
                showSubmenu: false,
                submenu: []
              }
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
            isShown: this.tokenService.canManageEntities(),
            showSubmenu: false,
            submenu: []
          }
        ]
      })
  }
}
