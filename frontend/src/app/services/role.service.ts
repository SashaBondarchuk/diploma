import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { type Observable, catchError, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import type { Role } from '@models/role.model';
import { ROLE_CLASSES } from '@app/constants/role-classes';
import { UserPermission } from '@app/shared/user-permission';
import { Employee } from '@models/employee.model';

interface AddUpdateRoleRequest {
  roleName: string;
  permissionIds: number[];
}

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly apiUrl = `${environment.apiUrl}/roles`;
  private readonly adminPermissions: UserPermission[] = [
    UserPermission.ManageRoles,
    UserPermission.ManageKpis,
    UserPermission.ManageEmployees,
    UserPermission.ManageTeams,
    UserPermission.ManageEvaluations,
  ];

  private readonly specialPermissions: UserPermission[] = [
    UserPermission.CreateRecommendations,
    UserPermission.ViewAllEvaluations
  ];

  constructor(private http: HttpClient) {}

  getAllRoles(): Observable<Role[]> {
    return this.http.get<Role[]>(this.apiUrl).pipe(
      catchError((error) => {
        console.error('Error fetching roles:', error);
        return throwError(() => new Error('Failed to load roles.'));
      })
    );
  }

  getRoleById(id: string): Observable<Role> {
    return this.http
      .get<Role>(`${this.apiUrl}/${id}`)
      .pipe(
        catchError((error) => {
          console.error(`Error fetching role with ID ${id}:`, error);
          return throwError(
            () => new Error(`Failed to load role with ID ${id}.`)
          );
        })
      );
  }

  createRole(role: AddUpdateRoleRequest): Observable<Role> {
    return this.http.post<Role>(this.apiUrl, role).pipe(
      catchError((error) => {
        console.error('Error creating role:', error);
        return throwError(() => new Error('Failed to create role.'));
      })
    );
  }

  updateRole(role: AddUpdateRoleRequest, id: string): Observable<Role> {
    return this.http.put<Role>(`${this.apiUrl}/${id}`, role).pipe(
      catchError((error) => {
        console.error('Error updating role:', error);
        return throwError(() => new Error('Failed to update role.'));
      })
    );
  }

  deleteRole(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error) => {
        console.error(`Error deleting role with ID ${id}:`, error);
        return throwError(
          () => new Error(`Failed to delete role with ID ${id}.`)
        );
      })
    );
  }

  getRoleClass(employee: Employee | null): string {
    if (!employee?.user?.role?.permissions) {
      return ROLE_CLASSES['default'];
    }

    return this.determineRoleClass(employee.user.role.permissions);
  }

  getRoleClassByPermissions(permissions: { name: string }[]): string {
    if (!permissions?.length) {
      return ROLE_CLASSES['default'];
    }

    return this.determineRoleClass(permissions);
  }

  private determineRoleClass(permissions: { name: string }[]): string {
    const permissionNames = permissions.map(p => p.name);

    if (permissionNames.some(p => this.adminPermissions.includes(p as UserPermission))) {
      return ROLE_CLASSES['admin'];
    }

    if (permissionNames.some(p => this.specialPermissions.includes(p as UserPermission))) {
      return ROLE_CLASSES['special'];
    }

    if (permissionNames.includes(UserPermission.EvaluateTeamMembersLead)) {
      return ROLE_CLASSES['lead'];
    }

    if (permissionNames.includes(UserPermission.EvaluateTeamMembers)) {
      return ROLE_CLASSES['member'];
    }

    return ROLE_CLASSES['default'];
  }
}

