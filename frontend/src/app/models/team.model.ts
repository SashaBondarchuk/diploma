import { EmployeePartial } from "./employee.model";

export interface Team {
  id: number;
  name: string;
  teamLeadId?: number;
  teamLeadAvatar?: string;
  teamLeadFirstName?: string;
  teamLeadLastName?: string;
}

export interface TeamWithEmployees {
  id: number;
  name: string;
  teamLeadId?: number;
  employees: EmployeePartial[];
}
