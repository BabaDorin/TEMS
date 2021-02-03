// Sidebar route Metadata
export interface RouteInfo{
  path: string;
  title: string;
  icon: string;
  isActive: boolean;
  showSubmenu: boolean;
  submenu: RouteInfo[];
}
