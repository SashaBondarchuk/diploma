import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { RecommendationService } from '@app/services/recommendation.service';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { Recommendation } from '@app/models/recommendation.model';
import { MessageService } from 'primeng/api';
import { FormControl } from '@angular/forms';
import { AuthService } from '@app/services/auth.service';
import { UserPermission } from '@app/shared/user-permission';
import { RecommendationModalComponent } from '@app/shared/components/recommendation-modal/recommendation-modal.component';
import { Employee } from '@app/models/employee.model';
import { Router } from '@angular/router';
import { EmployeesService } from '@app/services/employees.service';

@Component({
  selector: 'app-recommendations',
  standalone: true,
  imports: [CommonModule, SharedModule, RecommendationModalComponent],
  templateUrl: './recommendations.component.html',
  styleUrls: ['./recommendations.component.scss'],
})
export class RecommendationsComponent implements OnInit, OnDestroy {
  recommendations: Recommendation[] = [];
  filteredRecommendations: Recommendation[] = [];
  loading = false;
  showRecommendationModal = false;
  searchControl = new FormControl('');
  canCreate = false;
  employees: Employee[] = [];

  private destroy$ = new Subject<void>();

  constructor(
    private recommendationService: RecommendationService,
    private employeesService: EmployeesService,
    private messageService: MessageService,
    private router: Router,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.checkPermissions();
    this.loadRecommendations();
    this.setupSearch();
    if (this.canCreate) {
      this.loadEmployees();
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadRecommendations(): void {
    this.loading = true;
    const request$ = this.canCreate
      ? this.recommendationService.getAllRecommendations()
      : this.recommendationService.getMyRecommendations();

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (recommendations: Recommendation[]) => {
        this.recommendations = recommendations;
        this.applySearch();
        this.loading = false;
      },
      error: (error: Error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load recommendations',
        });
        this.loading = false;
      },
    });
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

  openAddDialog(): void {
    this.showRecommendationModal = true;
  }

  onRecommendationModalHide(): void {
    this.showRecommendationModal = false;
  }

  onSaveRecommendation(recommendation: {
    employeeId: number;
    recommendationText: string;
    isVisibleToEmployee: boolean;
  }): void {
    this.recommendationService
      .createRecommendation(recommendation)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Recommendation created successfully',
          });
          this.loadRecommendations();
          this.showRecommendationModal = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create recommendation',
          });
          this.showRecommendationModal = false;
        },
      });
  }

  viewRecommendation(recommendation: Recommendation): void {
    this.router.navigate(['/home/recommendations', recommendation.id]);
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
      this.filteredRecommendations = this.recommendations;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredRecommendations = this.recommendations.filter(
      (recommendation) =>
        recommendation.recommendationText.toLowerCase().includes(searchLower) ||
        `${recommendation.employee.firstName} ${recommendation.employee.lastName}`
          .toLowerCase()
          .includes(searchLower)
    );
  }

  private checkPermissions(): void {
    this.canCreate = this.authService.hasPermission(UserPermission.CreateRecommendations);
  }

  formatUserAvatar(avatar: string | null): string {
    if (!avatar) return '';
    return avatar.startsWith('data:') ? avatar : `data:image/jpeg;base64,${avatar}`;
  }
}
