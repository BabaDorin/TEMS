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

export const ROUTES: RouteInfo[] = [
  {
    path: '',
    title: 'Equipment',
    icon: 'mdi mdi-desktop-mac menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: [
      {
        path: '/equipment/all',
        title: 'View',
        icon: 'mdi mdi-view-list menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/add',
        title: 'Add',
        icon: 'mdi mdi-plus menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/quick-access',
        title: 'Quick Access',
        icon: 'mdi mdi-format-horizontal-align-right menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/allocate',
        title: 'Allocate',
        icon: 'mdi mdi-transfer menu-icon',
        isActive: false,
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
    showSubmenu: false,
    submenu: [
      {
        path: '/rooms/all',
        title: 'View',
        icon: 'mdi mdi-view-list menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/college-map',
        title: 'College Map',
        icon: 'mdi mdi-map menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/rooms/allocate',
        title: 'Allocate',
        icon: 'mdi mdi-transfer menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Personnel',
    icon: 'mdi mdi-account-multiple menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: [
      {
        path: '/personnel/all',
        title: 'View',
        icon: 'mdi mdi-view-list menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/personnel/add',
        title: 'Add',
        icon: 'mdi mdi-plus menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/personnel/allocate',
        title: 'Allocate',
        icon: 'mdi mdi-transfer menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Keys',
    icon: 'mdi mdi-key-variant menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: [
      {
        path: '/keys/all',
        title: 'View',
        icon: 'mdi mdi-key-change menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/keys/allocations',
        title: 'View Allocations',
        icon: 'mdi mdi-account-search menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/keys/allocate',
        title: 'Allocate',
        icon: 'mdi mdi-account-key menu-icon',
        isActive: false,
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
    showSubmenu: false,
    submenu: [
      {
        path: '/issues/all',
        title: 'View',
        icon: 'mdi mdi-alert menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/issues/create',
        title: 'Create New',
        icon: 'mdi mdi-plus menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Communication',
    icon: 'mdi mdi-information-variant menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: [
      {
        path: '/communication/announcements',
        title: 'Announcements',
        icon: 'mdi mdi-bullhorn menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/communication/logs',
        title: 'Logs',
        icon: 'mdi mdi-format-align-center menu-icon',
        isActive: false,
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
    showSubmenu: false,
    submenu: []
  },
  {
    path: 'reports/general',
    title: 'Report Printing',
    icon: 'mdi mdi-printer menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: []
  },
  {
    path: '',
    title: 'Administration',
    icon: 'mdi mdi-account-star menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: [
      {
        path: '/administration/equipment-management',
        title: 'Equipment Management',
        icon: 'mdi mdi-database-plus menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/user-management',
        title: 'User Management',
        icon: 'mdi mdi-account-multiple-outline menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/role-management',
        title: 'Role Management',
        icon: 'mdi mdi-account-convert menu-icon',
        isActive: false,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/system-configuration',
        title: 'System Configuration menu-icon',
        icon: 'mdi mdi-sitemap',
        isActive: false,
        showSubmenu: false,
        submenu: []
      }
    ]
  },
  {
    path: '/administration/system-configuration',
    title: 'Ciuta Ion',
    icon: 'mdi mdi-wheelchair-accessibility menu-icon',
    isActive: false,
    showSubmenu: false,
    submenu: []
  }
];
