import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { type Observable, catchError, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import type { Recommendation, UpdateRecommendationRequest } from '@models/recommendation.model';

interface CreateRecommendationRequest {
  employeeId: number;
  recommendationText: string;
  isVisibleToEmployee: boolean;
}

@Injectable({
  providedIn: 'root',
})
export class RecommendationService {
  private readonly apiUrl = `${environment.apiUrl}/recommendations`;

  constructor(private http: HttpClient) {}

  getAllRecommendations(): Observable<Recommendation[]> {
    return this.http.get<Recommendation[]>(this.apiUrl).pipe(
      catchError((error) => {
        console.error('Error fetching recommendations:', error);
        return throwError(() => new Error('Failed to load recommendations.'));
      })
    );
  }

  getMyRecommendations(): Observable<Recommendation[]> {
    return this.http.get<Recommendation[]>(`${this.apiUrl}/mine`).pipe(
      catchError((error) => {
        console.error('Error fetching my recommendations:', error);
        return throwError(() => new Error('Failed to load my recommendations.'));
      })
    );
  }

  createRecommendation(recommendation: CreateRecommendationRequest): Observable<Recommendation> {
    return this.http.post<Recommendation>(this.apiUrl, recommendation).pipe(
      catchError((error) => {
        console.error('Error creating recommendation:', error);
        return throwError(() => new Error('Failed to create recommendation.'));
      })
    );
  }

  getRecommendationById(id: number): Observable<Recommendation> {
    return this.http.get<Recommendation>(`${this.apiUrl}/${id}`).pipe(
      catchError((error) => {
        console.error(`Error fetching recommendation with ID ${id}:`, error);
        return throwError(() => new Error(`Failed to load recommendation with ID ${id}.`));
      })
    );
  }

  updateRecommendation(id: number, recommendation: UpdateRecommendationRequest): Observable<Recommendation> {
    return this.http.put<Recommendation>(`${this.apiUrl}/${id}`, recommendation).pipe(
      catchError((error) => {
        console.error(`Error updating recommendation with ID ${id}:`, error);
        return throwError(() => new Error(`Failed to update recommendation with ID ${id}.`));
      })
    );
  }
}
