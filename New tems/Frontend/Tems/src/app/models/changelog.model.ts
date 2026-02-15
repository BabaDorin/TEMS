export interface ChangeLogEntry {
  id: string;
  action: ChangeLogAction;
  description: string;
  timestamp: string;
  performedByUserId?: string;
  performedByUserName?: string;
  references: { [key: string]: string | null };
  details?: { [key: string]: any };
}

export interface ChangeLogTimelineResponse {
  entries: ChangeLogEntry[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export type ChangeLogAction =
  | 'AssetCreated'
  | 'AssetUpdated'
  | 'AssetDeleted'
  | 'AssetAssignedToUser'
  | 'AssetUnassignedFromUser'
  | 'AssetAssignedToLocation'
  | 'AssetUnassignedFromLocation'
  | 'UserCreated'
  | 'UserDeleted'
  | 'UserRolesUpdated'
  | 'UserAssetAssigned'
  | 'UserAssetUnassigned'
  | 'LocationCreated'
  | 'LocationUpdated'
  | 'LocationDeleted'
  | 'LocationAssetAssigned'
  | 'LocationAssetUnassigned';

export type ChangeLogEntityType = 'Asset' | 'User' | 'Location';
