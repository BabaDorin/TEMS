// Sidebar route Metadata
export interface RouteInfo{
  path: string;
  title: string;
  isActive: boolean;
  showSubmenu: boolean;
  submenu: RouteInfo[];
}
