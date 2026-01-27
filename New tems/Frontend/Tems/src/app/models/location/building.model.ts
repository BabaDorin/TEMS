export interface Building {
  id: string;
  siteId: string;
  name: string;
  code: string;
  description?: string;
  addressLine?: string;
  numberOfFloors?: number;
  isActive: boolean;
  managerContact?: string;
  createdAt: Date;
  updatedAt: Date;
  createdBy?: string;
}
