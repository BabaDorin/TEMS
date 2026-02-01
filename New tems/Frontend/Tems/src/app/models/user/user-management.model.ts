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
