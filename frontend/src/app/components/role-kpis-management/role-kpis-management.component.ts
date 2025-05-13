import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { RoleKpiService } from '@app/services/role-kpi.service';
import { RoleService } from '@app/services/role.service';
import { KpiService } from '@app/services/kpi.service';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { RoleKPIModel } from '@app/models/role-kpi.model';
import { Role } from '@app/models/role.model';
import { KpiMetric } from '@app/models/kpi.model';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { RoleKpiModalComponent } from '@app/shared/components/role-kpi-modal/role-kpi-modal.component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { UserRole } from '@app/shared/user-role';

@Component({
  selector: 'app-role-kpis-management',
  standalone: true,
  imports: [
    CommonModule,
    SharedModule,
    RoleKpiModalComponent,
    ReactiveFormsModule,
  ],
  templateUrl: './role-kpis-management.component.html',
  styleUrls: ['./role-kpis-management.component.scss'],
})
export class RoleKpisManagementComponent implements OnInit, OnDestroy {
  roleKpis: RoleKPIModel[] = [];
  filteredRoleKpis: RoleKPIModel[] = [];
  roles: Role[] = [];
  kpis: KpiMetric[] = [];
  loading = false;
  selectedRoleKpi: RoleKPIModel | null = null;
  showRoleKpiModal = false;
  searchControl = new FormControl('');

  private destroy$ = new Subject<void>();

  constructor(
    private roleKpiService: RoleKpiService,
    private roleService: RoleService,
    private kpiService: KpiService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadRoleKpis();
    this.loadRoles();
    this.loadKpis();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRoleKpis(): void {
    this.loading = true;
    this.roleKpiService
      .getAllRoleKpis()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (roleKpis: RoleKPIModel[]) => {
          this.roleKpis = roleKpis;
          this.applySearch();
          this.loading = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load role KPIs',
          });
          this.loading = false;
        },
      });
  }

  loadRoles(): void {
    this.roleService
      .getAllRoles()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (roles: Role[]) => {
          this.roles = roles.filter(
            (role) => role.roleName !== UserRole.Unassigned
          );
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load roles',
          });
        },
      });
  }

  loadKpis(): void {
    this.kpiService
      .getAllKpis()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (kpis: KpiMetric[]) => {
          this.kpis = kpis;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load KPIs',
          });
        },
      });
  }

  openAddDialog(): void {
    this.selectedRoleKpi = null;
    this.showRoleKpiModal = true;
  }

  openEditDialog(roleKpi: RoleKPIModel): void {
    this.selectedRoleKpi = roleKpi;
    this.showRoleKpiModal = true;
  }

  onRoleKpiModalHide(): void {
    this.showRoleKpiModal = false;
    this.selectedRoleKpi = null;
  }

  onSaveRoleKpi(request: {
    roleId: number;
    kpiId: number;
    weight: number;
    minScore: number;
    maxScore: number;
    isAllowedToEvaluateExceptLead: boolean;
    scoreRangeDescription: string;
  }): void {
    if (this.selectedRoleKpi) {
      this.roleKpiService
        .updateRoleKpi(
          {
            weight: request.weight,
            minScore: request.minScore,
            maxScore: request.maxScore,
            isAllowedToEvaluateExceptLead:
              request.isAllowedToEvaluateExceptLead,
            scoreRangeDescription: request.scoreRangeDescription,
            roleId: this.selectedRoleKpi.roleId,
            kpiId: this.selectedRoleKpi.kpiId,
          },
          this.selectedRoleKpi.roleId,
          this.selectedRoleKpi.kpiId
        )
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Role KPI updated successfully',
            });
            this.loadRoleKpis();
            this.showRoleKpiModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update role KPI',
            });
            this.showRoleKpiModal = false;
          },
        });
    } else {
      this.roleKpiService
        .createRoleKpi(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Role KPI created successfully',
            });
            this.loadRoleKpis();
            this.showRoleKpiModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to create role KPI',
            });
            this.showRoleKpiModal = false;
          },
        });
    }
  }

  confirmDelete(roleKpi: RoleKPIModel): void {
    this.confirmationService.confirm({
      header: 'Delete Role KPI',
      message: `Are you sure you want to delete this role KPI?`,
      accept: () => {
        this.deleteRoleKpi(roleKpi);
      },
    });
  }

  private deleteRoleKpi(roleKpi: RoleKPIModel): void {
    this.roleKpiService
      .deleteRoleKpi(roleKpi.roleId, roleKpi.kpiId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Role KPI deleted successfully',
          });
          this.loadRoleKpis();
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to delete role KPI',
          });
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300), distinctUntilChanged())
      .subscribe((searchTerm) => {
        this.applySearch();
      });
  }

  private applySearch(): void {
    const searchTerm = this.searchControl.value || '';
    if (!searchTerm.trim()) {
      this.filteredRoleKpis = this.roleKpis;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredRoleKpis = this.roleKpis.filter(
      (roleKpi) =>
        roleKpi.roleName.toLowerCase().includes(searchLower) ||
        roleKpi.kpiName.toLowerCase().includes(searchLower)
    );
  }

  getRoleClass(roleName: string): string {
    const role = this.roles.find(r => r.roleName === roleName);
    if (!role) {
      return 'role-default';
    }
    return this.roleService.getRoleClassByPermissions(role.permissions);
  }
}
