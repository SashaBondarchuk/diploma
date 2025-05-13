import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { RoleService } from '@app/services/role.service';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { Role } from '@app/models/role.model';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { RoleModalComponent } from '@app/shared/components/role-modal/role-modal.component';
import { PERMISSION_CLASSES } from '@app/constants/permission-classes';
import { UserRole } from '@app/shared/user-role';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-roles-management',
  standalone: true,
  imports: [
    CommonModule,
    SharedModule,
    RoleModalComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './roles-management.component.html',
  styleUrls: ['./roles-management.component.scss'],
})
export class RolesManagementComponent implements OnInit, OnDestroy {
  roles: Role[] = [];
  filteredRoles: Role[] = [];
  loading = false;
  selectedRole: Role | null = null;
  showRoleModal = false;
  searchControl = new FormControl('');

  private destroy$ = new Subject<void>();

  constructor(
    private roleService: RoleService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadRoles();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRoles(): void {
    this.loading = true;
    this.roleService
      .getAllRoles()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (roles: Role[]) => {
          this.roles = roles.filter(
            (role) => role.roleName !== UserRole.Unassigned
          );
          this.filteredRoles = this.roles;
          this.loading = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load roles',
          });
          this.loading = false;
        },
      });
  }

  openAddDialog(): void {
    this.selectedRole = null;
    this.showRoleModal = true;
  }

  openEditDialog(role: Role): void {
    this.selectedRole = role;
    this.showRoleModal = true;
  }

  onRoleModalHide(): void {
    this.showRoleModal = false;
    this.selectedRole = null;
  }

  onSaveRole(request: { roleName: string; permissionIds: number[] }): void {
    if (this.selectedRole) {
      this.roleService
        .updateRole(request, this.selectedRole.id.toString())
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Role updated successfully',
            });
            this.loadRoles();
            this.showRoleModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update role',
            });
          },
        });
    } else {
      this.roleService
        .createRole(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Role created successfully',
            });
            this.loadRoles();
            this.showRoleModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to create role',
            });
          },
        });
    }
  }

  confirmDelete(role: Role): void {
    this.confirmationService.confirm({
      header: 'Delete Role',
      message: `Are you sure you want to delete the role "${role.roleName}"?`,
      accept: () => {
        this.deleteRole(role);
      },
    });
  }

  getPermissionClass(permissionName: string): string {
    return PERMISSION_CLASSES[permissionName] || 'permission-default';
  }

  private deleteRole(role: Role): void {
    this.roleService
      .deleteRole(role.id.toString())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Role deleted successfully',
          });
          this.loadRoles();
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to delete role',
          });
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300), distinctUntilChanged())
      .subscribe((searchTerm) => {
        this.filterRoles(searchTerm || '');
      });
  }

  private filterRoles(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.filteredRoles = this.roles;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredRoles = this.roles.filter((role) =>
      role.roleName.toLowerCase().includes(searchLower)
    );
  }
}
