export interface AssetType {
  id: string;
  name: string;
  description?: string;
  parentTypeId?: string;
  parentTypeName?: string;
  properties: AssetTypeProperty[];
  isArchived: boolean;
  archivedAt?: Date;
  archivedBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface AssetTypeProperty {
  propertyId: string;
  propertyName: string;
  isRequired: boolean;
  displayOrder: number;
  defaultValue?: string;
  validation?: PropertyValidationDto;
}

export interface PropertyValidationDto {
  type: string;
  maxLength?: number;
  pattern?: string;
  min?: number;
  max?: number;
  unit?: string;
  enumValues?: string[];
}

export interface CreateAssetTypeRequest {
  name: string;
  description?: string;
  parentTypeId?: string;
  properties: AssetTypePropertyRequest[];
}

export interface AssetTypePropertyRequest {
  propertyId: string;
  isRequired: boolean;
  displayOrder: number;
  defaultValue?: string;
  validation?: PropertyValidationDto;
}

export interface UpdateAssetTypeRequest {
  name?: string;
  description?: string;
  parentTypeId?: string;
  properties?: AssetTypePropertyRequest[];
}
