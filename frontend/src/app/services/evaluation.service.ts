import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { catchError, Observable, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import { EvaluationDetailViewModel } from '@app/models/evaluation.model';

@Injectable({
  providedIn: 'root',
})
export class EvaluationService {
  private apiUrl = `${environment.apiUrl}/evaluations`;

  constructor(private http: HttpClient) {}

  createEvaluation(request: {
    evaluationSessionId: number;
    kpiId: number;
    roleId: number;
    score: number;
    comment?: string;
  }): Observable<EvaluationDetailViewModel> {
    return this.http.post<EvaluationDetailViewModel>(this.apiUrl, request).pipe(
      catchError((error) => {
        console.error('Error creating evaluation:', error);
        return throwError(() => new Error('Failed to create evaluation.'));
      })
    );
  }

  getEvaluationsBySessionId(sessionId: number): Observable<EvaluationDetailViewModel[]> {
    return this.http.get<EvaluationDetailViewModel[]>(`${this.apiUrl}/session/${sessionId}`).pipe(
      catchError((error) => {
        console.error('Error fetching evaluations:', error);
        return throwError(() => new Error('Failed to fetch evaluations.'));
      })
    );
  }

  updateEvaluation(
    id: number,
    request: {
      score: number;
      comment?: string;
    }
  ): Observable<EvaluationDetailViewModel> {
    return this.http.put<EvaluationDetailViewModel>(`${this.apiUrl}/${id}`, request).pipe(
      catchError((error) => {
        console.error('Error updating evaluation:', error);
        return throwError(() => new Error('Failed to update evaluation.'));
      })
    );
  }

  deleteEvaluation(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`).pipe(
      catchError((error) => {
        console.error('Error deleting evaluation:', error);
        return throwError(() => new Error('Failed to delete evaluation.'));
      })
    );
  }
}
