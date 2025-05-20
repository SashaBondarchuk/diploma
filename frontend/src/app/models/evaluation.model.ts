import { EmployeePartial } from './employee.model';
import { RoleKPIModel } from './role-kpi.model';

export interface EvaluationDetailViewModel {
  id: number;
  score: number;
  comment: string;
  evaluationSessionId: number;
  kpiId: number;
  roleId: number;
  evaluatorId: number;
  evaluator: EmployeePartial;
  roleKpi: RoleKPIModel;
}
