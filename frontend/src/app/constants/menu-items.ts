import type { MenuItemModel } from '@models/menu-item.model';
import { UserPermission } from '@app/shared/user-permission';

export const SPECIAL_ROUTES: Record<string, string> = {
  '/home/employee-profile': 'Employee Profile',
  '/home/recommendations/': 'Recommendation',
};

export const ALL_MENU_ITEMS: MenuItemModel[] = [
  {
    id: 1,
    label: 'Administration',
    icon: 'pi pi-cog',
    children: [
      {
        id: 111,
        label: 'Role Assignments',
        icon: 'pi pi-caret-right',
        route: '/home/admin/role-assignments',
        permissions: [UserPermission.ManageEmployees],
      },
      {
        id: 112,
        label: 'Roles & Permissions',
        icon: 'pi pi-caret-right',
        route: '/home/admin/roles-management',
        permissions: [UserPermission.ManageRoles],
      },
      {
        id: 113,
        label: 'Key Performance Indicators',
        icon: 'pi pi-caret-right',
        route: '/home/admin/kpi-management',
        permissions: [UserPermission.ManageKpis],
      },
      {
        id: 114,
        label: `Role's KPI`,
        icon: 'pi pi-caret-right',
        route: '/home/admin/role-kpis-management',
        permissions: [UserPermission.ManageRoles, UserPermission.ManageKpis],
      },
    ],
  },
  {
    id: 2,
    label: 'Teams',
    icon: 'pi pi-users',
    route: '/home/teams',
    permissions: [UserPermission.Base],
  },
  {
    id: 3,
    label: 'Employees',
    icon: 'pi pi-user',
    route: '/home/employees',
    permissions: [UserPermission.Base],
  },
  {
    id: 7,
    label: 'Evaluations',
    icon: 'pi pi-th-large',
    route: '/evaluations/all',
    permissions: [UserPermission.ViewAllEvaluations],
  },
  {
    id: 8,
    label: 'Evaluate Employees',
    icon: 'pi pi-chart-bar',
    route: '/evaluations',
    permissions: [UserPermission.EvaluateTeamMembers],
  },
  {
    id: 9,
    label: 'Recommendations',
    icon: 'pi pi-comment',
    route: '/home/recommendations',
    permissions: [UserPermission.Base],
  },
];
