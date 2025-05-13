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
  user: User;
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