import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { catchError, Observable } from 'rxjs';
import { EvaluationSessionViewModel } from '@app/models/evaluation-session.model';
import { AddEvaluationSessionRequest } from '@app/models/evaluation-session.model';

@Injectable({
  providedIn: 'root',
})
export class EvaluationSessionService {
  private evaluationSessionsEndpoint = 'evaluationSessions';

  constructor(private apiService: ApiService) {}

  getAllEvaluationSessions(): Observable<EvaluationSessionViewModel[]> {
    return this.apiService
      .get<EvaluationSessionViewModel[]>(this.evaluationSessionsEndpoint)
      .pipe(
        catchError((error) => {
          console.error('Error fetching evaluation sessions:', error);
          throw new Error('Error fetching evaluation sessions');
        })
      );
  }

  getEvaluationSessionById(id: string): Observable<EvaluationSessionViewModel> {
    return this.apiService
      .get<EvaluationSessionViewModel>(
        `${this.evaluationSessionsEndpoint}/${id}`
      )
      .pipe(
        catchError((error) => {
          console.error('Error fetching evaluation session by ID:', error);
          throw new Error('Error fetching evaluation session by ID');
        })
      );
  }

  getFilteredEvaluationSessions(params: {
    [key: string]: string;
  }): Observable<EvaluationSessionViewModel[]> {
    return this.apiService
      .get<EvaluationSessionViewModel[]>(
        this.evaluationSessionsEndpoint,
        params
      )
      .pipe(
        catchError((error) => {
          console.error('Error fetching filtered evaluation sessions:', error);
          throw new Error('Error fetching filtered evaluation sessions');
        })
      );
  }

  getPendingEvaluationSessions(): Observable<EvaluationSessionViewModel[]> {
    return this.apiService
      .get<EvaluationSessionViewModel[]>(
        `${this.evaluationSessionsEndpoint}/pending-to-be-evaluated`
      )
      .pipe(
        catchError((error) => {
          console.error('Error fetching pending evaluation sessions:', error);
          throw new Error('Error fetching pending evaluation sessions');
        })
      );
  }

  createEvaluationSession(
    session: AddEvaluationSessionRequest
  ): Observable<EvaluationSessionViewModel> {
    return this.apiService
      .post<EvaluationSessionViewModel>(
        this.evaluationSessionsEndpoint,
        session
      )
      .pipe(
        catchError((error) => {
          console.error('Error creating evaluation session:', error);
          throw new Error('Error creating evaluation session');
        })
      );
  }

  endEvaluationSession(id: number): Observable<EvaluationSessionViewModel> {
    return this.apiService
      .post<EvaluationSessionViewModel>(
        `${this.evaluationSessionsEndpoint}/end/${id}`,
        {}
      )
      .pipe(
        catchError((error) => {
          console.error('Error ending evaluation session:', error);
          throw new Error('Error ending evaluation session');
        })
      );
  }

  generateReport(id: number): Observable<EvaluationSessionViewModel> {
    return this.apiService
      .post<EvaluationSessionViewModel>(
        `${this.evaluationSessionsEndpoint}/generate-report/${id}`,
        {}
      )
      .pipe(
        catchError((error) => {
          console.error('Error generating report:', error);
          throw new Error('Error generating report');
        })
      );
  }

  downloadReport(id: number): Observable<{ blob: Blob, filename: string }> {
    return this.apiService
      .getBlob(`${this.evaluationSessionsEndpoint}/report/${id}`)
      .pipe(
        catchError((error) => {
          console.error('Error downloading report:', error);
          throw new Error('Error downloading report');
        })
      );
  }

  deleteEvaluationSession(id: number): Observable<void> {
    return this.apiService
      .delete<void>(`${this.evaluationSessionsEndpoint}/${id}`)
      .pipe(
        catchError((error) => {
          console.error('Error deleting evaluation session:', error);
          throw new Error('Error deleting evaluation session');
        })
      );
  }
}
