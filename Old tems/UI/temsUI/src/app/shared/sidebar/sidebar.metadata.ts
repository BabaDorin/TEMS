// Sidebar route Metadata
export interface RouteInfo{
  path: string;
  title: string;
  icon: string;
  isActive: boolean;
  isShown: boolean;
  showSubmenu: boolean;
  submenu: RouteInfo[];
}
