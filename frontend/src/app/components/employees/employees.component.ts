import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { Employee } from '@app/models/employee.model';
import { MessageService } from 'primeng/api';
import { FormControl } from '@angular/forms';
import { AuthService } from '@app/services/auth.service';
import { UserPermission } from '@app/shared/user-permission';
import { EmployeesService } from '@app/services/employees.service';
import { RoleService } from '@app/services/role.service';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.scss'],
})
export class EmployeesComponent implements OnInit, OnDestroy {
  employees: Employee[] = [];
  filteredEmployees: Employee[] = [];
  loading = false;
  searchControl = new FormControl('');
  canEdit = false;

  private destroy$ = new Subject<void>();

  constructor(
    private employeeService: EmployeesService,
    private messageService: MessageService,
    public authService: AuthService,
    private roleService: RoleService
  ) {}

  ngOnInit(): void {
    this.checkPermissions();
    this.loadEmployees();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadEmployees(): void {
    this.loading = true;
    this.employeeService
      .getAllEmployees()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (employees: Employee[]) => {
          this.employees = employees;
          this.applySearch();
          this.loading = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load employees',
          });
          this.loading = false;
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300), distinctUntilChanged())
      .subscribe(() => {
        this.applySearch();
      });
  }

  private applySearch(): void {
    const searchTerm = this.searchControl.value || '';
    if (!searchTerm.trim()) {
      this.filteredEmployees = this.employees;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredEmployees = this.employees.filter(
      (employee) =>
        employee.firstName.toLowerCase().includes(searchLower) ||
        employee.lastName.toLowerCase().includes(searchLower) ||
        employee.user?.role.roleName.toLowerCase().includes(searchLower) ||
        employee.team?.name.toLowerCase().includes(searchLower)
    );
  }

  private checkPermissions(): void {
    this.canEdit = this.authService.hasPermission(UserPermission.ManageEmployees);
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:') ? avatar : `data:image/jpeg;base64,${avatar}`;
  }

  getRoleClass(roleName: string): string {
    const employee = this.employees.find(e => e.user?.role?.roleName === roleName) || null;
    return this.roleService.getRoleClass(employee);
  }
}
