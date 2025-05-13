import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { ActivatedRoute, Router } from '@angular/router';
import { RecommendationService } from '@app/services/recommendation.service';
import {
  Recommendation,
  UpdateRecommendationRequest,
} from '@app/models/recommendation.model';
import { RoleService } from '@app/services/role.service';
import { AuthService } from '@app/services/auth.service';
import { Subject, takeUntil } from 'rxjs';
import { ToastState } from '@app/shared/toast-state';
import { UserPermission } from '@app/shared/user-permission';
import { RecommendationModalComponent } from '@app/shared/components/recommendation-modal/recommendation-modal.component';
import { Employee } from '@app/models/employee.model';
import { EmployeesService } from '@app/services/employees.service';
import { MessageService } from 'primeng/api';

@Component({
  selector: 'app-recommendation-detail',
  standalone: true,
  imports: [CommonModule, SharedModule, RecommendationModalComponent],
  templateUrl: './recommendation-detail.component.html',
  styleUrls: ['./recommendation-detail.component.scss'],
})
export class RecommendationDetailComponent implements OnInit, OnDestroy {
  recommendation: Recommendation | null = null;
  loading = false;
  roleClass = '';
  canManage = false;
  showRecommendationModal = false;
  employees: Employee[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private recommendationService: RecommendationService,
    private roleService: RoleService,
    private authService: AuthService,
    private employeesService: EmployeesService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.checkPermissions();
    this.loadRecommendation();
    if (this.canManage) {
      this.loadEmployees();
    }
    this.authService.currentEmployee$
      .pipe(takeUntil(this.destroy$))
      .subscribe((employee) => {
        this.roleClass = this.roleService.getRoleClass(employee);
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadRecommendation(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.loading = true;
    this.recommendationService.getRecommendationById(Number(id)).subscribe({
      next: (recommendation) => {
        this.recommendation = recommendation;
        this.loading = false;
      },
      error: (error) => {
        this.loading = false;
        var toastState: ToastState = {
          toastMessage: {
            severity: 'error',
            summary: 'Error',
            detail: `Recommendation with ID ${id} could not be loaded`,
          },
        };
        this.router.navigate(['/home/recommendations'], { state: toastState });
      },
    });
  }

  formatUserAvatar(avatar: string | null): string {
    if (!avatar) return '';
    return avatar.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }

  getRoleClass(): string {
    return this.roleClass;
  }

  private checkPermissions(): void {
    this.canManage = this.authService.hasPermission(
      UserPermission.CreateRecommendations
    );
  }

  private loadEmployees(): void {
    this.employeesService.getAllEmployees().subscribe({
      next: (employees: Employee[]) => {
        this.employees = employees;
      },
      error: (error: Error) => {
        console.error('Error loading employees:', error);
      },
    });
  }

  openEditDialog(): void {
    this.showRecommendationModal = true;
  }

  onSaveRecommendation(recommendation: UpdateRecommendationRequest): void {
    if (!this.recommendation) return;

    recommendation.employeeId = this.recommendation.employeeId;
    this.recommendationService
      .updateRecommendation(this.recommendation.id, recommendation)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedRecommendation) => {
          this.recommendation = updatedRecommendation;
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Recommendation updated successfully',
          });
          this.showRecommendationModal = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to update recommendation',
          });
          this.showRecommendationModal = false;
        },
      });
  }

  onRecommendationModalHide(): void {
    this.showRecommendationModal = false;
  }
}
