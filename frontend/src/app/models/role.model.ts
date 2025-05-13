import { Permission } from "./permission.model";

export interface Role {
  id: number;
  roleName: string;
  permissions: Permission[];
}
