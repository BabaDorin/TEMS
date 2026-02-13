export interface UserDto {
  id: string;
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  avatarUrl?: string;
  tenantIds: string[];
  keycloakId?: string;
  roles: string[];
  createdAt: string;
  updatedAt: string;
  assetCounts?: { [typeName: string]: number };
}

export interface RoleDto {
  id: string;
  name: string;
  description?: string;
}

export interface GetAllUsersResponse {
  users: UserDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface CreateUserRequest {
  username: string;
  email: string;
  firstName?: string;
  lastName?: string;
  temporaryPassword?: string;
  initialRoles?: string[];
}

export interface CreateUserResponse {
  success: boolean;
  message?: string;
  user?: UserDto;
}

export interface DeleteUserResponse {
  success: boolean;
  message?: string;
}

export interface UpdateUserRolesRequest {
  id: string;
  roles: string[];
}

export interface UpdateUserRolesResponse {
  success: boolean;
  message?: string;
  user?: UserDto;
}

export interface GetAllRolesResponse {
  roles: RoleDto[];
}

export interface UserAssetsResponse {
  assets: UserAssetDto[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

export interface UserAssetDto {
  id: string;
  assetTag: string;
  serialNumber?: string;
  status: string;
  definition?: {
    definitionId: string;
    name: string;
    assetTypeId: string;
    assetTypeName: string;
    manufacturer: string;
    model: string;
  };
  locationId?: string;
  location?: {
    building?: string;
    room?: string;
    desk?: string;
  };
  assignment?: {
    assignedToUserId: string;
    assignedToName: string;
    assignedAt: string;
    assignmentType: string;
  };
  createdAt?: string;
}

export interface UserAssetCountResponse {
  count: number;
}
