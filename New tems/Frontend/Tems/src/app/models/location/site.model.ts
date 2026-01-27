export interface Site {
  id: string;
  name: string;
  code: string;
  description?: string;
  addressLine?: string;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  timezone: string;
  isActive: boolean;
  managerContact?: string;
  createdAt: Date;
  updatedAt: Date;
  createdBy?: string;
}
