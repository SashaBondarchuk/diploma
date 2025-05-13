import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { UserService } from '@services/user.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AddEditEmployee } from '@models/employee.model';
import { UserProfileModalComponent } from '@app/shared/components/user-profile-modal/user-profile-modal.component';
import { User, UserPartial } from '@app/models/user.model';
import { UserRole } from '@app/shared/user-role';
import { AddUpdateEmployeeRequest } from '@app/models/requests/add-update-employee.request';
import { MessageService } from 'primeng/api';
import { EmployeesService } from '@app/services/employees.service';
import { ToastModule } from 'primeng/toast';

@Component({
  selector: 'app-role-assignments',
  standalone: true,
  imports: [CommonModule, SharedModule, UserProfileModalComponent, ToastModule],
  templateUrl: './role-assignments.component.html',
  styleUrls: ['./role-assignments.component.scss'],
})
export class RoleAssignmentsComponent implements OnInit, OnDestroy {
  users: UserPartial[] = [];
  loading = true;
  selectedUser: User | null = null;
  selectedEmployee: AddEditEmployee | null = null;
  profileModalVisible = false;

  private destroy$ = new Subject<void>();

  constructor(
    private userService: UserService,
    private employeesService: EmployeesService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.loadEmployees();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  openProfileModal(user: User): void {
    this.selectedUser = user;
    this.profileModalVisible = true;
    this.selectedEmployee = this.getEmployeeToAdd(user);
  }

  onProfileModalHide(): void {
    this.profileModalVisible = false;
    this.selectedUser = null;
    this.selectedEmployee = null;
  }

  onSaveEmployee(employee: AddUpdateEmployeeRequest): void {
    if (!this.selectedUser?.id) {
      this.messageService.add({
        severity: 'error',
        summary: 'Error',
        detail: 'User ID is missing',
      });
      return;
    }

    this.employeesService
      .addEmployee(employee)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Employee profile created successfully',
          });
          this.onProfileModalHide();
          this.loadEmployees();
        },
        error: (error: Error) => {
          console.error('Error creating employee profile:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create employee profile',
          });
          this.onProfileModalHide();
        },
      });
  }

  private loadEmployees(): void {
    this.loading = true;
    this.userService
      .getAllUsers()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (users: UserPartial[]) => {
          this.users = users.filter(
            (user) => user.roleName == UserRole.Unassigned
          );
          this.loading = false;
        },
        error: (error: Error) => {
          console.error('Error loading employees:', error);
          this.loading = false;
        },
      });
  }

  private getEmployeeToAdd(user: User): AddEditEmployee {
    return {
      user: user,
      userId: user.id,
    };
  }
}
