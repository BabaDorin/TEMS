import { TokenService } from './../token-service/token.service';
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

  hasClaim(claim: string): boolean{
    return this.tokenService.hasClaim(claim);
  }

  ROUTES: RouteInfo[] = [
    {
      path: '',
      title: 'Equipment',
      icon: 'mdi mdi-desktop-mac menu-icon',
      isActive: false,
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),  
      showSubmenu: false,
      submenu: [
        {
          path: '/equipment/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_ENTITIES),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/equipment',
          title: 'Quick Access',
          icon: 'mdi mdi-format-horizontal-align-right menu-icon',
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/allocate',
          title: 'Allocate',
          icon: 'mdi mdi-transfer menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
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
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),  
      showSubmenu: false,
      submenu: [
        {
          path: '/rooms/view',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/rooms/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_ENTITIES),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/rooms',
          title: 'Quick Access',
          icon: 'mdi mdi-format-horizontal-align-right menu-icon',
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/rooms/map',
          title: 'College Map',
          icon: 'mdi mdi-map menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
          showSubmenu: false,
          submenu: []
        },
      ]
    },
    {
      path: '',
      title: 'Personnel',
      icon: 'mdi mdi-account-multiple menu-icon',
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),  
      isActive: false,
      showSubmenu: false,
      submenu: [
        {
          path: '/personnel/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/personnel/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_ENTITIES),  
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/quick-access/personnel',
          title: 'Quick Access',
          icon: 'mdi mdi-format-horizontal-align-right menu-icon',
          isShown: this.hasClaim(CAN_VIEW_ENTITIES),
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
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_ALLOCATE_KEYS),
      showSubmenu: false,
      submenu: [
        {
          path: '/keys/all',
          title: 'View',
          icon: 'mdi mdi-key-change menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_ALLOCATE_KEYS),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocations',
          title: 'View Allocations',
          icon: 'mdi mdi-account-search menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_ALLOCATE_KEYS),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocate',
          title: 'Allocate',
          icon: 'mdi mdi-account-key menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_ENTITIES) || this.hasClaim(CAN_ALLOCATE_KEYS),
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
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
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
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
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
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
      showSubmenu: false,
      submenu: [
        {
          path: '/library/all',
          title: 'View library',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/library/add',
          title: 'Upload item',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
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
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
      showSubmenu: false,
      submenu: []
    },
    {
      path: '',
      title: 'Administration',
      icon: 'mdi mdi-account-star menu-icon',
      isActive: false,
      isShown: this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION),
      showSubmenu: false,
      submenu: [
        {
          path: '/administration/equipment',
          title: 'Equipment',
          icon: 'mdi mdi-database-plus menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/users',
          title: 'Users',
          icon: 'mdi mdi-account-multiple-outline menu-icon',
          isShown: this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION),
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/roles',
          title: 'Roles',
          icon: 'mdi mdi-account-convert menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION),
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/system-configuration',
          title: 'Configuration',
          icon: 'mdi mdi-sitemap menu-icon',
          isActive: false,
          isShown: this.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION),
          showSubmenu: false,
          submenu: []
        }
      ]
    },
    {
      path: '/analytics',
      title: 'Analytics',
      icon: 'mdi mdi-chart-bar menu-icon',
      isActive: false,
      isShown: this.hasClaim(CAN_VIEW_ENTITIES) || this.hasClaim(CAN_MANAGE_ENTITIES),
      showSubmenu: false,
      submenu: []
    }
  ];
}
