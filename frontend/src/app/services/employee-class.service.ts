import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { catchError, Observable } from 'rxjs';
import { EmployeeClassModel } from '@models/employee-class.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeeClassService {
  private employeeClassEndpoint = 'employeeClasses';

  constructor(private apiService: ApiService) {}

  getEmployeeClassById(id: number): Observable<EmployeeClassModel> {
    return this.apiService
      .get<EmployeeClassModel>(`${this.employeeClassEndpoint}/${id}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching employee class:', error);
          throw new Error(
            'An error occurred while fetching the employee class.'
          );
        })
      );
  }

  getAllEmployeeClasses(): Observable<EmployeeClassModel[]> {
    return this.apiService
      .get<EmployeeClassModel[]>(`${this.employeeClassEndpoint}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching employee classes:', error);
          throw new Error(
            'An error occurred while fetching the employee classes.'
          );
        })
      );
  }

  getClassStyleClassname(className: string): string {
    const classMap: Record<string, string> = {
      'A': 'class-a',
      'B': 'class-b',
      'C': 'class-c',
      'D': 'class-d',
      'E': 'class-e',
    };

    return `employee-class-chip ${classMap[className] || 'class-default'}`;
  }
}
