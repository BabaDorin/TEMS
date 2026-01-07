export interface Asset {
  id: string;
  assetTag: string;
  serialNumber?: string;
  status: AssetStatus;
  assetTypeId: string;
  assetTypeName: string;
  definition: AssetDefinitionSnapshot;
  purchaseInfo?: PurchaseInfo;
  location?: AssetLocation;
  assignment?: AssetAssignment;
  parentAssetId?: string;
  childAssetIds: string[];
  maintenanceHistory: MaintenanceRecord[];
  customFields?: { [key: string]: any };
  notes?: string;
  isArchived: boolean;
  archivedAt?: Date;
  archivedBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AssetDefinitionSnapshot {
  definitionId: string;
  name: string;
  manufacturer?: string;
  model?: string;
  description?: string;
  specifications: AssetSpecification[];
  snapshotTakenAt: Date;
}

export interface AssetSpecification {
  propertyId: string;
  name: string;
  value: any;
  dataType: string;
  unit?: string;
}

export interface PurchaseInfo {
  vendor?: string;
  purchaseDate?: Date;
  purchasePrice?: number;
  warrantyExpiration?: Date;
  invoiceNumber?: string;
}

export interface AssetLocation {
  building?: string;
  floor?: string;
  room?: string;
  updatedAt: Date;
}

export interface AssetAssignment {
  assignedToUserId?: string;
  assignedToUserName?: string;
  assignedAt?: Date;
  assignedDate?: Date;
  assignedBy?: string;
  notes?: string;
  dueDate?: Date;
}

export interface MaintenanceRecord {
  date: Date;
  description: string;
  performedBy?: string;
  cost?: number;
  notes?: string;
}

export enum AssetStatus {
  Available = 'AVAILABLE',
  InUse = 'IN_USE',
  UnderMaintenance = 'UNDER_MAINTENANCE',
  Retired = 'RETIRED'
}

export interface CreateAssetRequest {
  assetTag: string;
  serialNumber?: string;
  status: AssetStatus;
  assetTypeId: string;
  definitionId: string;
  purchaseInfo?: PurchaseInfo;
  location?: AssetLocation;
  assignment?: AssetAssignment;
  parentAssetId?: string;
  customFields?: { [key: string]: any };
  notes?: string;
}

export interface UpdateAssetRequest {
  assetTag?: string;
  serialNumber?: string;
  status?: AssetStatus;
  purchaseInfo?: PurchaseInfo;
  location?: AssetLocation;
  assignment?: AssetAssignment;
  parentAssetId?: string;
  customFields?: { [key: string]: any };
  notes?: string;
}
