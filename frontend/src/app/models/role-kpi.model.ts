export interface RoleKPIModel {
  roleId: number;
  roleName: string;
  kpiId: number;
  kpiName: string;
  weight: number;
  minScore: number;
  maxScore: number;
  isAllowedToEvaluateExceptLead: boolean;
  scoreRangeDescription: string;
}
