import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, of, throwError } from 'rxjs';
import { catchError, map, switchMap, tap } from 'rxjs/operators';
import { environment } from '@environments/environment';
import { AuthRequest, AuthResponse, JwtPayload } from '@models/auth.model';
import { Employee } from '@models/employee.model';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = environment.apiUrl;
  private tokenKey = 'auth_token';
  private currentUserSubject = new BehaviorSubject<JwtPayload | null>(null);
  private currentEmployeeSubject = new BehaviorSubject<Employee | null>(null);

  currentUser$ = this.currentUserSubject.asObservable();
  currentEmployee$ = this.currentEmployeeSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {
    this.loadUserFromToken();
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/login`, { email, password })
      .pipe(
        tap((response) => {
          this.setToken(response.token);
          const payload = this.parseJwt(response.token);
          this.currentUserSubject.next(payload);
          this.router.navigate(['/home']);
        }),
        map(() => true),
        catchError((error) => {
          console.error('Login error:', error);
          return throwError(() => new Error(`Check your credentials.`));
        })
      );
  }

  register(email: string, password: string): Observable<boolean> {
    return this.http
      .post<AuthResponse>(`${this.apiUrl}/auth/register`, { email, password })
      .pipe(
        tap((response) => {
          this.setToken(response.token);
          const payload = this.parseJwt(response.token);
          this.currentUserSubject.next(payload);
          this.router.navigate(['/home']);
        }),
        map(() => true),
        catchError((error) => {
          console.error('Registration error:', error);
          return throwError(() => new Error(`Check your credentials.`));
        })
      );
  }

  logout(): void {
    localStorage.removeItem(this.tokenKey);
    this.currentUserSubject.next(null);
    this.currentEmployeeSubject.next(null);
    this.router.navigate(['/login']);
  }

  isLoggedIn(): boolean {
    return !!this.getToken() && !!this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem(this.tokenKey);
  }

  hasPermission(permission: string): boolean {
    const user = this.currentUserSubject.value;
    return !!user && user.permissions?.includes(permission);
  }

  fetchEmployeeData(): Observable<Employee> {
    return this.http.get<Employee>(`${this.apiUrl}/employees/me`);
  }

  setCurrentEmployee(): void {
    this.fetchEmployeeData().subscribe({
      next: (employee) => {
        this.currentEmployeeSubject.next(employee);
      },
      error: (error) => {
        console.error('Error loading employee data:', error);
        this.logout();
      },
    });
  }

  private setToken(token: string): void {
    localStorage.setItem(this.tokenKey, token);
  }

  private parseJwt(token: string): JwtPayload {
    const base64Url = token.split('.')[1];
    const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
    const jsonPayload = decodeURIComponent(
      atob(base64)
        .split('')
        .map((c) => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
        .join('')
    );
    return JSON.parse(jsonPayload);
  }

  private loadUserFromToken(): void {
    const token = this.getToken();
    if (token) {
      try {
        const payload = this.parseJwt(token);
        const isExpired = Date.now() >= payload.exp * 1000;

        if (isExpired) {
          this.logout();
          return;
        }

        this.currentUserSubject.next(payload);
      } catch (error) {
        console.error('Error parsing token:', error);
        this.logout();
      }
    }
  }
}
