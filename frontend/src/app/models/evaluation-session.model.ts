import { EmployeeClassModel } from './employee-class.model';
import { EmployeePartial } from './employee.model';

export interface EvaluationSessionViewModel {
  id: number;
  name: string;
  startDate: Date;
  endDate: Date;
  evaluationFinishedDate: Date | null;
  employeeId: number;
  classId: number | null;
  weightedScore: number | null;
  isReportAvailable: boolean;

  employee: EmployeePartial;
  class: EmployeeClassModel | null;
}

export interface AddEvaluationSessionRequest {
  name: string;
  endDate: Date;
  employeeId: number;
}
