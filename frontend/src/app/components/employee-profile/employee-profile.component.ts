import { Component, type OnInit, type OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeesService } from '@services/employees.service';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import type { Employee } from '@models/employee.model';
import { CommonModule, formatDate } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { ToastState } from '@app/shared/toast-state';
import { AuthService } from '../../services/auth.service';
import { UserPermission } from '@app/shared/user-permission';
import {
  AddUpdateEmployeeRequest,
  mapToAddUpdateEmployeeRequest,
} from '@app/models/requests/add-update-employee.request';
import { MessageService } from 'primeng/api';
import { FormBuilder, type FormGroup, Validators } from '@angular/forms';
import { Team } from '@app/models/team.model';
import { Role } from '@app/models/role.model';
import { UserProfileModalComponent } from '@app/shared/components/user-profile-modal/user-profile-modal.component';
import { AddEditEmployee } from '@models/employee.model';
import { PERMISSION_CLASSES } from '@app/constants/permission-classes';
import { RoleService } from '@app/services/role.service';

@Component({
  selector: 'app-employee-profile',
  standalone: true,
  imports: [CommonModule, SharedModule, UserProfileModalComponent],
  templateUrl: './employee-profile.component.html',
  styleUrls: ['./employee-profile.component.scss'],
})
export class EmployeeProfileComponent implements OnInit, OnDestroy {
  employee: Employee | null = null;

  teams: Team[] = [];
  roles: Role[] = [];
  loadingTeamsAndRoles = false;

  formattedHireDate = '';
  formattedBirthDate = '';
  currentDate = new Date();
  isTeamLead = false;
  canEditEmployeeProfile = false;

  editDialogVisible = false;
  selectedEmployee: AddEditEmployee | null = null;

  editForm: FormGroup;
  dialogHeader: string = 'Edit Employee Profile';

  saving = false;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private employeesService: EmployeesService,
    private authService: AuthService,
    private messageService: MessageService,
    private fb: FormBuilder,
    private roleService: RoleService
  ) {
    this.editForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      phoneNumber: [
        '',
        [Validators.required, Validators.pattern(/^\+?[0-9\s\-$$$$]+$/)],
      ],
      birthDate: [null, Validators.required],
      teamId: [null, Validators.required],
      roleId: [null, Validators.required],
      isTeamLead: [false],
    });
  }

  ngOnInit(): void {
    this.checkPermissions();
    this.subscribeOnRouteChanges();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private checkPermissions(): void {
    this.canEditEmployeeProfile = this.authService.hasPermission(
      UserPermission.ManageEmployees
    );
  }

  private subscribeOnRouteChanges(): void {
    this.route.paramMap.pipe(takeUntil(this.destroy$)).subscribe((params) => {
      const employeeId = params.get('employeeId');

      if (employeeId) {
        this.loadEmployeeData(employeeId);
      } else {
        console.error('Employee ID not found in route');
        this.router.navigate(['/home']);
      }
    });
  }

  private loadEmployeeData(employeeId: string): void {
    this.employeesService
      .getEmployeeById(employeeId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (employee) => {
          this.employee = employee;
          this.formatDates();
          this.checkIfTeamLead();
        },
        error: (error) => {
          console.error('Error loading employee data:', error);
          var toastState: ToastState = {
            toastMessage: {
              severity: 'error',
              summary: 'Error',
              detail: `Employee with ID ${employeeId} not found`,
            },
          };

          this.router.navigate(['/home'], {
            state: toastState,
          });
        },
      });
  }

  private formatDates(): void {
    if (this.employee) {
      this.formattedHireDate = formatDate(
        this.employee.hireDate,
        'dd.MM.yyyy',
        'uk-UA'
      );
      this.formattedBirthDate = formatDate(
        this.employee.birthDate,
        'dd.MM.yyyy',
        'uk-UA'
      );
    }
  }

  private checkIfTeamLead(): void {
    if (this.employee?.team) {
      this.isTeamLead = this.employee.id === this.employee.team.teamLeadId;
    } else {
      this.isTeamLead = false;
    }
  }

  formatUserAvatar(avatar: string): string {
    return `data:image/png;base64,${avatar}`;
  }

  onAvatarSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0 || !this.employee) return;

    const file = input.files[0];
    const reader = new FileReader();

    reader.onload = () => {
      const base64 = reader.result as string;
      this.employee!.avatar = base64;

      var employeeRequest = mapToAddUpdateEmployeeRequest(this.employee!);
      this.employeesService
        .updateEmployee(employeeRequest, this.employee!.id)
        .subscribe({
          next: () => {},
          error: (err) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update avatar',
            });
            console.error('Avatar update failed:', err);
          },
        });
    };

    reader.readAsDataURL(file);
  }

  getPermissionClass(permission: string): string {
    return PERMISSION_CLASSES[permission] || 'permission-default';
  }

  getRoleClass(roleName: string): string {
    return this.roleService.getRoleClass(this.employee);
  }

  openEditDialog(event: Event): void {
    event.preventDefault();
    if (!this.employee) return;

    this.selectedEmployee = {
      user: this.employee.user,
      userId: this.employee.userId,
      firstName: this.employee.firstName,
      lastName: this.employee.lastName,
      phoneNumber: this.employee.phoneNumber,
      birthDate: this.employee.birthDate,
      teamId: this.employee.team?.id,
      avatar: this.employee.avatar,
      isTeamLead: this.isTeamLead,
    };

    this.editDialogVisible = true;
  }

  onEditDialogHide(): void {
    this.editDialogVisible = false;
    this.selectedEmployee = null;
  }

  onSaveEmployee(employee: AddUpdateEmployeeRequest): void {
    if (!this.employee) return;

    this.employeesService
      .updateEmployee(employee, this.employee.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          this.employee = response;
          this.formatDates();
          this.checkIfTeamLead();

          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Employee profile updated successfully',
          });

          this.onEditDialogHide();
        },
        error: (error) => {
          console.error('Error updating employee:', error);
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update employee profile',
          });
        },
      });
  }
}
