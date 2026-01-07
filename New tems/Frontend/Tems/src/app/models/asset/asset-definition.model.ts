import { AssetSpecification } from './asset.model';

export interface AssetDefinition {
  id: string;
  assetTypeId: string;
  assetTypeName: string;
  name: string;
  manufacturer?: string;
  model?: string;
  specifications: AssetSpecification[];
  usageCount: number;
  tags: string[];
  isArchived: boolean;
  archivedAt?: Date;
  archivedBy?: string;
  createdAt: Date;
  updatedAt: Date;
}

export interface CreateAssetDefinitionRequest {
  assetTypeId: string;
  name: string;
  manufacturer?: string;
  model?: string;
  specifications: AssetSpecification[];
  tags?: string[];
}

export interface UpdateAssetDefinitionRequest {
  name?: string;
  manufacturer?: string;
  model?: string;
  specifications?: AssetSpecification[];
  tags?: string[];
}
