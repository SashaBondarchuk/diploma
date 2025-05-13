import { UserPermission } from '@app/shared/user-permission';

export const PERMISSION_DESCRIPTIONS: Record<string, string> = {
  [UserPermission.Base]: 'Base Access',
  [UserPermission.ManageRoles]: 'Manage Roles',
  [UserPermission.ManageKpis]: 'Manage KPIs',
  [UserPermission.ManageEmployees]: 'Manage Employees',
  [UserPermission.ManageTeams]: 'Manage Teams',
  [UserPermission.ManageEvaluations]: 'Manage Evaluations',
  [UserPermission.CreateRecommendations]: 'Create Recommendations',
  [UserPermission.ViewAllEvaluations]: 'View All Evaluations',
  [UserPermission.EvaluateTeamMembersLead]: 'Evaluate Team Members (Lead)',
  [UserPermission.EvaluateTeamMembers]: 'Evaluate Team Members',
};
