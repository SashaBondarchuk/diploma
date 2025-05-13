import { Role } from './role.model';

export interface User {
  id: number;
  email: string;
  roleId: number;
  role: Role;
}

export interface UserPartial {
  id: number;
  email: string;
  roleId: number;
  roleName: string;
}