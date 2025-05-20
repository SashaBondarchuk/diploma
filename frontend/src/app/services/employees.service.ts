import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { catchError, Observable } from 'rxjs';
import { Employee } from '@models/employee.model';
import { AddUpdateEmployeeRequest } from '@models/employee.model';

@Injectable({
  providedIn: 'root',
})
export class EmployeesService {
  private employeesEndpoint = 'employees';

  constructor(private apiService: ApiService) {}

  getEmployeeById(id: string): Observable<Employee> {
    return this.apiService
      .get<Employee>(`${this.employeesEndpoint}/${id}`)
      .pipe(
        catchError((error) => {
          console.error('Error fetching employee:', error);
          throw new Error('An error occurred while fetching the employee.');
        })
      );
  }

  getAllEmployees(): Observable<Employee[]> {
    return this.apiService.get<Employee[]>(`${this.employeesEndpoint}`).pipe(
      catchError((error) => {
        console.error('Error fetching employees:', error);
        throw new Error('An error occurred while fetching the employees.');
      })
    );
  }

  addEmployee(employee: AddUpdateEmployeeRequest): Observable<Employee> {
    return this.apiService
      .post<Employee>(`${this.employeesEndpoint}`, employee)
      .pipe(
        catchError((error) => {
          console.error('Error adding employee:', error);
          throw new Error('An error occurred while adding the employee.');
        })
      );
  }

  updateEmployee(
    employee: AddUpdateEmployeeRequest,
    id: number
  ): Observable<Employee> {
    return this.apiService
      .put<Employee>(`${this.employeesEndpoint}/${id}`, employee)
      .pipe(
        catchError((error) => {
          console.error('Error updating employee:', error);
          throw new Error('An error occurred while updating the employee.');
        })
      );
  }
}
