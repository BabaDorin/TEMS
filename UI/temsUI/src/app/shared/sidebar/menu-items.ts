import { Role } from '../../models/role.model';
import { RouteInfo } from './sidebar.metadata';

// Sidebar Menu Items State

//       Items
//         > View Items
//         > Add Items
//         > Quick Access
//         > Allocate

//       Rooms
//         > View Rooms
//         > College map
//         > Allocate

//       Personnnel
//         > View Personnnel
//         > Add Personnnel
//         > Allocate

//       Keys
//         > View Keys
//         > View Allocations
//         > Allocate

//       Issues
//         > View Issues
//         > Create Issues

//       Communication
//         > Announcements
//         > Logs

//       Libary

//       Report Printing

//       Administration
//         > Equipment Manager
//         > User Manager
//         > Role Manager
//         > System Configuration

export class menuItems {

  constructor(private role: Role) { }

  ROUTES: RouteInfo[] = [
    {
      path: '',
      title: 'Equipment',
      icon: 'mdi mdi-desktop-mac menu-icon',
      isActive: false,
      isShown: this.role.canViewEquipment || this.role.canManageEquipment || this.role.canAllocateEquipment,      
      showSubmenu: false,
      submenu: [
        {
          path: '/equipment/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.role.canViewEquipment || this.role.canManageEquipment,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.role.canManageEquipment,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/quick-access',
          title: 'Quick Access',
          icon: 'mdi mdi-format-horizontal-align-right menu-icon',
          isShown: this.role.canViewEquipment || this.role.canManageEquipment,      
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/equipment/allocate',
          title: 'Allocate',
          icon: 'mdi mdi-transfer menu-icon',
          isActive: false,
          isShown: this.role.canManageEquipment || this.role.canAllocateEquipment,      
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
      isShown: this.role.canViewRooms || this.role.canManageRooms || this.role.canAllocateEquipment,      
      showSubmenu: false,
      submenu: [
        {
          path: '/rooms/view',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.role.canViewRooms || this.role.canManageRooms,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/rooms/add',
          title: '+ Add new',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.role.canViewRooms || this.role.canManageRooms,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/rooms/map',
          title: 'College Map',
          icon: 'mdi mdi-map menu-icon',
          isActive: false,
          isShown: this.role.canViewRooms || this.role.canManageRooms,      
          showSubmenu: false,
          submenu: []
        },
        // {
        //   path: '/rooms/allocate',
        //   title: 'Allocate',
        //   icon: 'mdi mdi-transfer menu-icon',
        //   isShown: this.role.canManageRooms || this.role.canAllocateEquipment,      
        //   isActive: false,
        //   showSubmenu: false,
        //   submenu: []
        // },
      ]
    },
    {
      path: '',
      title: 'Personnel',
      icon: 'mdi mdi-account-multiple menu-icon',
      isShown: this.role.canViewPersonnel || this.role.canManagePersonnel || this.role.canAllocateEquipment,  
      isActive: false,
      showSubmenu: false,
      submenu: [
        {
          path: '/personnel/all',
          title: 'View',
          icon: 'mdi mdi-view-list menu-icon',
          isActive: false,
          isShown: this.role.canViewPersonnel || this.role.canManagePersonnel,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/personnel/add',
          title: 'Add',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.role.canManagePersonnel,      
          showSubmenu: false,
          submenu: []
        },
        // {
        //   path: '/personnel/allocate',
        //   title: 'Allocate',
        //   icon: 'mdi mdi-transfer menu-icon',
        //   isActive: false,
        //   isShown: this.role.canManagePersonnel || this.role.canAllocateEquipment,
        //   showSubmenu: false,
        //   submenu: []
        // },
      ]
    },
    {
      path: '',
      title: 'Keys',
      icon: 'mdi mdi-key-variant menu-icon',
      isActive: false,
      isShown: this.role.canViewKeys || this.role.canManageKeys || this.role.canAllocateKeys,
      showSubmenu: false,
      submenu: [
        {
          path: '/keys/all',
          title: 'View',
          icon: 'mdi mdi-key-change menu-icon',
          isActive: false,
          isShown: this.role.canViewKeys || this.role.canManageKeys,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocations',
          title: 'View Allocations',
          icon: 'mdi mdi-account-search menu-icon',
          isActive: false,
          isShown: this.role.canManageKeys  || this.role.canAllocateKeys,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/keys/allocate',
          title: 'Allocate',
          icon: 'mdi mdi-account-key menu-icon',
          isActive: false,
          isShown: this.role.canManageKeys || this.role.canAllocateKeys,
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
      isShown: this.role.canViewIssues || this.role.canManageIssues || this.role.canCreateIssues,      
      showSubmenu: false,
      submenu: [
        {
          path: '/issues/all',
          title: 'View',
          icon: 'mdi mdi-alert menu-icon',
          isActive: false,
          isShown: this.role.canViewIssues || this.role.canManageIssues,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/issues/create',
          title: 'Create New',
          icon: 'mdi mdi-plus menu-icon',
          isActive: false,
          isShown: this.role.canManageIssues || this.role.canCreateIssues,      
          showSubmenu: false,
          submenu: []
        },
      ]
    },
    {
      path: '',
      title: 'Communication',
      icon: 'mdi mdi-information-variant menu-icon',
      isShown: this.role.canViewCommunication || this.role.canManageCommunication,      
      isActive: false,
      showSubmenu: false,
      submenu: [
        {
          path: '/communication/announcements',
          title: 'Announcements',
          icon: 'mdi mdi-bullhorn menu-icon',
          isActive: false,
          isShown: this.role.canViewCommunication || this.role.canManageCommunication,      
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/communication/logs',
          title: 'Logs',
          icon: 'mdi mdi-format-align-center menu-icon',
          isActive: false,
          isShown: this.role.canViewCommunication || this.role.canManageCommunication,      
          showSubmenu: false,
          submenu: []
        },
      ]
    },
    {
      path: '/library/all',
      title: 'Library',
      icon: 'mdi mdi-microsoft menu-icon',
      isActive: false,
      isShown: this.role.canViewLibrary || this.role.canManageLibrary,      
      showSubmenu: false,
      submenu: []
    },
    {
      path: 'reports/general',
      title: 'Report Printing',
      icon: 'mdi mdi-printer menu-icon',
      isActive: false,
      isShown: this.role.canViewReports || this.role.canManageReports,      
      showSubmenu: false,
      submenu: []
    },
    {
      path: '',
      title: 'Administration',
      icon: 'mdi mdi-account-star menu-icon',
      isActive: false,
      isShown: this.role.hasAdminRights,      
      showSubmenu: false,
      submenu: [
        {
          path: '/administration/equipment-management',
          title: 'Equipment',
          icon: 'mdi mdi-database-plus menu-icon',
          isActive: false,
          isShown: this.role.hasAdminRights,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/user-management',
          title: 'Users',
          icon: 'mdi mdi-account-multiple-outline menu-icon',
          isShown: this.role.hasAdminRights,
          isActive: false,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/role-management',
          title: 'Roles',
          icon: 'mdi mdi-account-convert menu-icon',
          isActive: false,
          isShown: this.role.hasAdminRights,
          showSubmenu: false,
          submenu: []
        },
        {
          path: '/administration/system-configuration',
          title: 'Configuration',
          icon: 'mdi mdi-sitemap menu-icon',
          isActive: false,
          isShown: this.role.hasAdminRights,
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
      isShown: this.role.canViewAnalytics,
      showSubmenu: false,
      submenu: []
    }
  ];  
}