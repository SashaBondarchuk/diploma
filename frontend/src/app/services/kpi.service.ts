import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { KpiMetric } from '@app/models/kpi.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class KpiService {
  private apiUrl = `${environment.apiUrl}/kpimetrics`;

  constructor(private http: HttpClient) {}

  getAllKpis(): Observable<KpiMetric[]> {
    return this.http.get<KpiMetric[]>(this.apiUrl);
  }

  createKpi(kpi: { name: string }): Observable<KpiMetric> {
    return this.http.post<KpiMetric>(this.apiUrl, kpi).pipe(
      catchError((error) => {
        console.error('Error creating KPI:', error);
        return throwError(() => new Error('Failed to create KPI.'));
      })
    );
  }

  updateKpi(kpi: { name: string }, id: string): Observable<KpiMetric> {
    return this.http.put<KpiMetric>(`${this.apiUrl}/${id}`, kpi).pipe(
      catchError((error) => {
        console.error('Error updating KPI:', error);
        return throwError(() => new Error('Failed to update KPI.'));
      })
    );
  }

  deleteKpi(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error) => {
        console.error('Error deleting KPI:', error);
        return throwError(() => new Error('Failed to delete KPI.'));
      })
    );
  }
}
