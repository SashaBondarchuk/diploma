import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { type Observable, catchError, throwError } from 'rxjs';
import { environment } from '@environments/environment';
import type { Team, TeamWithEmployees } from '@models/team.model';

@Injectable({
  providedIn: 'root',
})
export class TeamService {
  private apiUrl = environment.apiUrl;
  private teamsEndpoint = 'teams';

  constructor(private http: HttpClient) {}

  getAllTeams(): Observable<Team[]> {
    return this.http.get<Team[]>(`${this.apiUrl}/${this.teamsEndpoint}`).pipe(
      catchError((error) => {
        console.error('Error fetching teams:', error);
        return throwError(() => new Error('Failed to load teams.'));
      })
    );
  }

  getTeamById(id: string): Observable<TeamWithEmployees> {
    return this.http
      .get<TeamWithEmployees>(`${this.apiUrl}/${this.teamsEndpoint}/${id}`)
      .pipe(
        catchError((error) => {
          console.error(`Error fetching team with ID ${id}:`, error);
          return throwError(
            () => new Error(`Failed to load team with ID ${id}.`)
          );
        })
      );
  }

  createTeam(team: Partial<Team>): Observable<Team> {
    return this.http
      .post<Team>(`${this.apiUrl}/${this.teamsEndpoint}`, team)
      .pipe(
        catchError((error) => {
          console.error('Error creating team:', error);
          return throwError(() => new Error('Failed to create team.'));
        })
      );
  }

  updateTeam(team: Partial<Team>, id: string): Observable<Team> {
    return this.http
      .put<Team>(`${this.apiUrl}/${this.teamsEndpoint}/${id}`, team)
      .pipe(
        catchError((error) => {
          console.error('Error updating team:', error);
          return throwError(() => new Error('Failed to update team.'));
        })
      );
  }

  deleteTeam(id: string): Observable<void> {
    return this.http
      .delete<void>(`${this.apiUrl}/${this.teamsEndpoint}/${id}`)
      .pipe(
        catchError((error) => {
          console.error(`Error deleting team with ID ${id}:`, error);
          return throwError(
            () => new Error(`Failed to delete team with ID ${id}.`)
          );
        })
      );
  }
}
