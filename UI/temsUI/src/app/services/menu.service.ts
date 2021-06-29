import { TokenService } from './token.service';
import { Injectable } from '@angular/core';
import { CAN_VIEW_ENTITIES, CAN_MANAGE_ENTITIES, CAN_ALLOCATE_KEYS, CAN_MANAGE_SYSTEM_CONFIGURATION } from 'src/app/models/claims';
import { RouteInfo } from 'src/app/shared/sidebar/sidebar.metadata';

@Injectable({
  providedIn: 'root'
})
export class MenuService {

  constructor(
    private tokenService: TokenService
  ) { }

  ROUTES: RouteInfo[] = [
    {
      path: '',
      title: 'Equipment',
      icon: 'mdi mdi-desktop-mac menu-icon',
      isActive: false,
      isShown: this.tokenService.canViewEntities(),  
      showSubmenu: false,
      submenu: [
        {
          path: '/equipment/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown:this.tokenService.canViewEntities(),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageEntities(),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/equipment',
          title: 'Quick Access',
          icon: 'mdi mdi-format-horizontal-align-right menu-icon',
          isShown: this.tokenService.canViewEntities(),
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/allocate',
          title: 'Allocate',
          icon: 'mdi mdi-transfer menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/allocations',
          title: 'Allocations',
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
      title: 'Rooms',
      icon: 'mdi mdi-panorama-wide-angle menu-icon',
      isActive: false,
      isShown: this.tokenService.canViewEntities(),  
      showSubmenu: false,
      submenu: [
        {
          path: '/rooms/view',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/rooms/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageEntities(),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/rooms',
          title: 'Quick Access',
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
      title: 'Personnel',
      icon: 'mdi mdi-account-multiple menu-icon',
      isShown: this.tokenService.canViewEntities(),  
      isActive: false,
      showSubmenu: false,
      submenu: [
        {
          path: '/personnel/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.tokenService.canViewEntities(),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/personnel/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageEntities(),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/personnel',
          title: 'Quick Access',
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
      title: 'Keys',
      icon: 'mdi mdi-key-variant menu-icon',
      isActive: false,
      isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
      showSubmenu: false,
      submenu: [
        {
          path: '/keys/all',
          title: 'View',
          icon: 'mdi mdi-key-change menu-icon',
          isActive: false,
          isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocations',
          title: 'View Allocations',
          icon: 'mdi mdi-account-search menu-icon',
          isActive: false,
          isShown: this.tokenService.canAllocateKeys() || this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocate',
          title: 'Allocate',
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
      title: 'Issues',
      icon: 'mdi mdi-information menu-icon',
      isActive: false,
      isShown: true,
      showSubmenu: false,
      submenu: [
        {
          path: '/issues/all',
          title: 'View',
          icon: 'mdi mdi-alert menu-icon',
          isActive: false,
          isShown: this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/issues/create',
          title: 'Create New',
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
      title: 'Communication',
      icon: 'mdi mdi-information-variant menu-icon',
      isShown: true,
      isActive: false,
      showSubmenu: false,
      submenu: [
        {
          path: '/communication/announcements',
          title: 'Announcements',
          icon: 'mdi mdi-bullhorn menu-icon',
          isActive: false,
          isShown: true,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/communication/logs',
          title: 'Logs',
          icon: 'mdi mdi-format-align-center menu-icon',
          isActive: false,
          isShown: this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/communication/sendemail',
          title: 'Send E-mails',
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
      title: 'Library',
      icon: 'mdi mdi-microsoft menu-icon',
      isActive: false,
      isShown: this.tokenService.canViewEntities(),
      showSubmenu: false,
      submenu: [
        {
          path: '/library/all',
          title: 'View library',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.tokenService.canViewEntities(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/library/add',
          title: 'Upload item',
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
      title: 'Report Printing',
      icon: 'mdi mdi-printer menu-icon',
      isActive: false,
      isShown: this.tokenService.canViewEntities(),
      showSubmenu: false,
      submenu: []
    },
    {
      path: '',
      title: 'Administration',
      icon: 'mdi mdi-account-star menu-icon',
      isActive: false,
      isShown: this.tokenService.canManageSystemConfiguration(),
      showSubmenu: false,
      submenu: [
        {
          path: '/administration/equipment',
          title: 'Equipment',
          icon: 'mdi mdi-database-plus menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageSystemConfiguration(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/users',
          title: 'Users',
          icon: 'mdi mdi-account-multiple-outline menu-icon',
          isShown: this.tokenService.canManageSystemConfiguration(),
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/system-configuration',
          title: 'Configuration',
          icon: 'mdi mdi-sitemap menu-icon',
          isActive: false,
          isShown: this.tokenService.canManageSystemConfiguration(),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/system-logs',
          title: 'System Logs',
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
      title: 'Archive',
      icon: 'mdi mdi-file-sync menu-icon',
      isActive: false,
      isShown: this.tokenService.canManageEntities(),
      showSubmenu: false,
      submenu: []
    }
  ];
}
