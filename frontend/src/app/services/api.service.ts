import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get<T>(endpoint: string, params?: any, headers?: HttpHeaders): Observable<T> {
    const options = {
      params: new HttpParams({ fromObject: params || {} }),
      headers: headers,
    };

    return this.http
      .get<T>(`${this.baseUrl}/${endpoint}`, options)
      .pipe(catchError(this.handleError));
  }

  post<T>(endpoint: string, data: any, headers?: HttpHeaders): Observable<T> {
    return this.http
      .post<T>(`${this.baseUrl}/${endpoint}`, data, { headers })
      .pipe(catchError(this.handleError));
  }

  put<T>(endpoint: string, data: any, headers?: HttpHeaders): Observable<T> {
    return this.http
      .put<T>(`${this.baseUrl}/${endpoint}`, data, { headers })
      .pipe(catchError(this.handleError));
  }

  delete<T>(endpoint: string, headers?: HttpHeaders): Observable<T> {
    return this.http
      .delete<T>(`${this.baseUrl}/${endpoint}`, { headers })
      .pipe(catchError(this.handleError));
  }

  private handleError(error: any) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      errorMessage = `Error: ${error.error.message}`;
    } else {
      errorMessage = `Error code: ${error.status}\nMessage: ${error.message}`;
    }
    console.error(errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
