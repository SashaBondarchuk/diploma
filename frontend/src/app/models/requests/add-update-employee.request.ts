import { Employee } from '../employee.model';

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
