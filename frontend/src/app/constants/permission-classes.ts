import { UserPermission } from '@app/shared/user-permission';

export const PERMISSION_CLASSES: Record<string, string> = {
  [UserPermission.Base]: 'permission-base',
  [UserPermission.ManageRoles]: 'permission-admin',
  [UserPermission.ManageKpis]: 'permission-admin',
  [UserPermission.ManageEmployees]: 'permission-admin',
  [UserPermission.ManageTeams]: 'permission-admin',
  [UserPermission.ManageEvaluations]: 'permission-admin',
  [UserPermission.CreateRecommendations]: 'permission-special',
  [UserPermission.ViewAllEvaluations]: 'permission-special',
  [UserPermission.EvaluateTeamMembersLead]: 'permission-lead',
  [UserPermission.EvaluateTeamMembers]: 'permission-member',
};
