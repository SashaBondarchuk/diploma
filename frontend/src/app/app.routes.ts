import { Routes } from '@angular/router';
import { HomeComponent } from '@app/components/home/home.component';
import { LoginComponent } from '@app/components/login/login.component';
import { RegisterComponent } from '@app/components/register/register.component';
import { UnauthorizedComponent } from '@app/components/unauthorized/unauthorized.component';
import { AuthGuard } from '@app/guards/auth.guard';
import { PermissionGuard } from './guards/permission.guard';
import { EmployeeProfileComponent } from './components/employee-profile/employee-profile.component';
import { UserPermission } from './shared/user-permission';
import { RoleAssignmentsComponent } from './components/role-assignments/role-assignments.component';
import { RolesManagementComponent } from './components/roles-management/roles-management.component';
import { KpiManagementComponent } from './components/kpi-management/kpi-management.component';
import { RoleKpisManagementComponent } from './components/role-kpis-management/role-kpis-management.component';
import { TeamsComponent } from './components/teams/teams.component';
import { EmployeesComponent } from './components/employees/employees.component';
import { RecommendationsComponent } from './components/recommendations/recommendations.component';
import { RecommendationDetailComponent } from './components/recommendation-detail/recommendation-detail.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: 'home',
    component: HomeComponent,
    canActivate: [AuthGuard, PermissionGuard],
    data: {
      requiredPermission: UserPermission.Base,
    },
    children: [
      {
        path: 'employee-profile/:employeeId',
        component: EmployeeProfileComponent,
      },
      {
        path: 'admin/role-assignments',
        component: RoleAssignmentsComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPermission: UserPermission.ManageEmployees,
        },
      },
      {
        path: 'admin/roles-management',
        component: RolesManagementComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPermission: UserPermission.ManageRoles,
        },
      },
      {
        path: 'admin/kpi-management',
        component: KpiManagementComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPermission: UserPermission.ManageKpis,
        },
      },
      {
        path: 'admin/role-kpis-management',
        component: RoleKpisManagementComponent,
        canActivate: [PermissionGuard],
        data: {
          requiredPermissions: [
            UserPermission.ManageRoles,
            UserPermission.ManageKpis,
          ],
        },
      },
      {
        path: 'teams',
        component: TeamsComponent,
      },
      {
        path: 'employees',
        component: EmployeesComponent,
      },
      {
        path: 'recommendations',
        component: RecommendationsComponent,
      },
      {
        path: 'recommendations/:id',
        component: RecommendationDetailComponent,
      },
    ],
  },
  { path: 'unauthorized', component: UnauthorizedComponent },
  { path: '', redirectTo: '/home', pathMatch: 'full' },
  { path: '**', redirectTo: '/home' },
];
