export interface AssetProperty {
  id: string;
  name: string;
  category: string;
  dataType: PropertyDataType;
  enumValues?: string[];
  unit?: string;
  defaultValidation?: PropertyValidation;
  description?: string;
  createdBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface PropertyValidation {
  type: string;
  maxLength?: number;
  pattern?: string;
  min?: number;
  max?: number;
  unit?: string;
  enumValues?: string[];
}

export enum PropertyDataType {
  String = 'STRING',
  Number = 'NUMBER',
  Boolean = 'BOOLEAN',
  Date = 'DATE',
  Enum = 'ENUM'
}

export interface CreateAssetPropertyRequest {
  name: string;
  category: string;
  dataType: PropertyDataType;
  enumValues?: string[];
  unit?: string;
  defaultValidation?: PropertyValidation;
  description?: string;
}

export interface UpdateAssetPropertyRequest {
  name?: string;
  category?: string;
  dataType?: PropertyDataType;
  enumValues?: string[];
  unit?: string;
  defaultValidation?: PropertyValidation;
  description?: string;
}
