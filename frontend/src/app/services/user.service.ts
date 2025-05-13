import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { type Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { AuthService } from './auth.service';
import { UserPartial } from '@app/models/user.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private apiUrl = environment.apiUrl;

  constructor(private http: HttpClient, private authService: AuthService) {}

  getUserById(id: number): Observable<UserPartial> {
    return this.http.get<UserPartial>(`${this.apiUrl}/Users/${id}`).pipe(
      catchError((error) => {
        console.error(`Error fetching user with ID ${id}:`, error);
        return throwError(
          () => new Error(`Failed to load user with ID ${id}.`)
        );
      })
    );
  }

  getAllUsers(): Observable<UserPartial[]> {
    return this.http.get<UserPartial[]>(`${this.apiUrl}/Users`).pipe(
      catchError((error) => {
        console.error('Error fetching all users:', error);
        return throwError(() => new Error('Failed to load users.'));
      })
    );
  }
}
