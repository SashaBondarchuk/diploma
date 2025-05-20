import { Team } from "./team.model";
import { User, UserPartial } from "./user.model";

export interface Employee {
  id: number;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: string;
  hireDate: string;
  teamId: number;
  userId: number;
  avatar: string;
  team: Team;
  user: User;
}

export interface AddEditEmployee {
  id?: number;
  firstName?: string;
  lastName?: string;
  phoneNumber?: string;
  birthDate?: string;
  hireDate?: string;
  teamId?: number;
  userId: number;
  avatar?: string;
  team?: Team;
  user: User | UserPartial;
  isTeamLead?: boolean;
}

export interface EmployeePartial {
  id: number;
  firstName: string;
  lastName: string;
  avatar: string;
  teamId: number;
  userId: number;
  user: UserPartial;
}

export interface AddUpdateEmployeeRequest {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  birthDate: string;
  teamId: number;
  isTeamLead?: boolean;
  userId: number;
  avatar: string | null;
  roleId: number;
}

export function mapToAddUpdateEmployeeRequest(
  employee: Employee
): AddUpdateEmployeeRequest {
  return {
    firstName: employee.firstName,
    lastName: employee.lastName,
    phoneNumber: employee.phoneNumber,
    birthDate: employee.birthDate,
    teamId: employee.teamId,
    userId: employee.userId,
    avatar: employee.avatar || null,
    roleId: employee.user.roleId,
  };
}
