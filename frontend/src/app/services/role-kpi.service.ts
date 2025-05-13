import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { RoleKPIModel } from '@app/models/role-kpi.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RoleKpiService {
  private apiUrl = `${environment.apiUrl}/rolekpis`;

  constructor(private http: HttpClient) {}

  getAllRoleKpis(): Observable<RoleKPIModel[]> {
    return this.http.get<RoleKPIModel[]>(this.apiUrl).pipe(
      catchError((error) => {
        console.error('Error fetching role KPIs:', error);
        return throwError(() => new Error('Failed to fetch role KPIs.'));
      })
    );
  }

  createRoleKpi(request: {
    roleId: number;
    kpiId: number;
    weight: number;
    minScore: number;
    maxScore: number;
    isAllowedToEvaluateExceptLead: boolean;
    scoreRangeDescription: string;
  }): Observable<RoleKPIModel> {
    return this.http.post<RoleKPIModel>(this.apiUrl, request).pipe(
      catchError((error) => {
        console.error('Error creating role KPI:', error);
        return throwError(() => new Error('Failed to create role KPI.'));
      })
    );
  }

  updateRoleKpi(
    request: {
      roleId: number;
      kpiId: number;
      weight: number;
      minScore: number;
      maxScore: number;
      isAllowedToEvaluateExceptLead: boolean;
      scoreRangeDescription: string;
    },
    roleId: number,
    kpiId: number
  ): Observable<RoleKPIModel> {
    return this.http
      .put<RoleKPIModel>(`${this.apiUrl}/${roleId}/${kpiId}`, request)
      .pipe(
        catchError((error) => {
          console.error('Error updating role KPI:', error);
          return throwError(() => new Error('Failed to update role KPI.'));
        })
      );
  }

  deleteRoleKpi(roleId: number, kpiId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${roleId}/${kpiId}`).pipe(
      catchError((error) => {
        console.error('Error deleting role KPI:', error);
        return throwError(() => new Error('Failed to delete role KPI.'));
      })
    );
  }
}
