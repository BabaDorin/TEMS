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
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/equipment/all',
        title: 'View',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/add',
        title: 'Add',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/quick-access',
        title: 'Quick Access',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/equipment/allocate',
        title: 'Allocate',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Rooms',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/rooms/all',
        title: 'View',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/college-map',
        title: 'College Map',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/rooms/allocate',
        title: 'Allocate',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Personnel',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/personnel/all',
        title: 'View',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/personnel/add',
        title: 'Add',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/personnel/allocate',
        title: 'Allocate',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Keys',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/keys/all',
        title: 'View',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/keys/add',
        title: 'Add',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/keys/allocate',
        title: 'Allocate',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Issues',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/issues/all',
        title: 'View',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/issues/add',
        title: 'Create New',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '',
    title: 'Communication',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/communication/announcements',
        title: 'Announcements',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/communication/logs',
        title: 'Logs',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
    ]
  },
  {
    path: '/library/all',
    title: 'Library',
    isActive: true,
    showSubmenu: false,
    submenu: []
  },
  {
    path: '/report-printing/general',
    title: 'Report Printing',
    isActive: true,
    showSubmenu: false,
    submenu: []
  },
  {
    path: '',
    title: 'Administration',
    isActive: true,
    showSubmenu: false,
    submenu: [
      {
        path: '/administration/equipment-management',
        title: 'Equipment Management',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/user-management',
        title: 'User Management',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/role-management',
        title: 'Role Management',
        isActive: true,
        showSubmenu: false,
        submenu: []
      },
      {
        path: '/administration/system-configuration',
        title: 'System Configuration',
        isActive: true,
        showSubmenu: false,
        submenu: []
      }
    ]
  }
];
