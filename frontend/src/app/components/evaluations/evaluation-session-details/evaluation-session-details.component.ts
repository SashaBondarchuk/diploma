import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { ActivatedRoute } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { EvaluationSessionService } from '@app/services/evaluation-session.service';
import { MessageService, ConfirmationService } from 'primeng/api';
import { EvaluationSessionViewModel } from '@app/models/evaluation-session.model';
import { EvaluationDetailViewModel } from '@app/models/evaluation.model';
import { EvaluationDetailsModalComponent } from '../../../shared/components/evaluation-details-modal/evaluation-details-modal.component';
import { EvaluationService } from '@app/services/evaluation.service';
import { EmployeeClassService } from '@app/services/employee-class.service';
import { UserPermission } from '@app/shared/user-permission';
import { AuthService } from '@app/services/auth.service';
import { RecommendationService } from '@app/services/recommendation.service';
import { UpdateRecommendationRequest } from '@app/models/recommendation.model';
import { RecommendationModalComponent } from '@app/shared/components/recommendation-modal/recommendation-modal.component';

@Component({
  selector: 'app-evaluation-session-details',
  standalone: true,
  imports: [CommonModule, SharedModule, EvaluationDetailsModalComponent, RecommendationModalComponent],
  templateUrl: './evaluation-session-details.component.html',
  styleUrls: ['./evaluation-session-details.component.scss'],
  providers: [ConfirmationService],
})
export class EvaluationSessionDetailsComponent implements OnInit, OnDestroy {
  session: EvaluationSessionViewModel | null = null;
  evaluations: EvaluationDetailViewModel[] = [];
  filteredEvaluations: EvaluationDetailViewModel[] = [];
  loading = false;
  reportLoading = false;
  showEvaluationModal = false;
  selectedEvaluation: EvaluationDetailViewModel | null = null;
  searchTerm = '';
  canManageEvaluations = false;
  showClassRecommendationModal = false;
  classRecommendation: UpdateRecommendationRequest | null = null;

  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private evaluationSessionService: EvaluationSessionService,
    private evaluationService: EvaluationService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private employeeClassService: EmployeeClassService,
    private authService: AuthService,
    private recommendationService: RecommendationService
  ) {}

  ngOnInit(): void {
    const sessionId = this.route.snapshot.paramMap.get('id');
    if (sessionId) {
      this.loadSessionDetails(sessionId);
    }
    this.checkPermissions();
  }

  private checkPermissions(): void {
    this.canManageEvaluations = this.authService.hasPermission(
      UserPermission.ManageEvaluations
    );
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadSessionDetails(sessionId: string): void {
    this.loading = true;
    this.evaluationSessionService
      .getEvaluationSessionById(sessionId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (session) => {
          this.session = session;
          this.loadEvaluations(sessionId);
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load evaluation session details',
          });
          this.loading = false;
        },
      });
  }

  private loadEvaluations(sessionId: string): void {
    this.evaluationService
      .getEvaluationsBySessionId(Number(sessionId))
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (evaluations) => {
          if (evaluations && evaluations.length > 0) {
            this.evaluations = evaluations;
            this.applySearch();
            this.loading = false;
          } else {
            this.loading = false;
          }
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load evaluations',
          });
          this.loading = false;
        },
      });
  }

  onSearch(event: Event): void {
    const target = event.target as HTMLInputElement;
    this.searchTerm = target.value;
    this.applySearch();
  }

  private applySearch(): void {
    if (!this.searchTerm.trim()) {
      this.filteredEvaluations = this.evaluations;
      return;
    }

    const searchLower = this.searchTerm.toLowerCase();
    this.filteredEvaluations = this.evaluations.filter(
      (evaluation) =>
        evaluation.roleKpi.kpiName.toLowerCase().includes(searchLower) ||
        `${evaluation.evaluator.firstName} ${evaluation.evaluator.lastName}`
          .toLowerCase()
          .includes(searchLower)
    );
  }

  viewEvaluationDetails(evaluation: EvaluationDetailViewModel): void {
    this.selectedEvaluation = evaluation;
    this.showEvaluationModal = true;
  }

  onEvaluationModalHide(): void {
    this.showEvaluationModal = false;
    this.selectedEvaluation = null;
  }

  hasAvatar(avatar: string | null | undefined): boolean {
    return !!avatar && avatar.trim().length > 0;
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }

  confirmEndSession(): void {
    this.confirmationService.confirm({
      header: 'End Evaluation Session',
      message:
        'Are you sure you want to end this evaluation session? This action cannot be undone.',
      accept: () => {
        if (this.session) {
          this.evaluationSessionService
            .endEvaluationSession(this.session.id)
            .pipe(takeUntil(this.destroy$))
            .subscribe({
              next: (evaluationSession) => {
                this.messageService.add({
                  severity: 'success',
                  summary: 'Success',
                  detail: 'Evaluation session ended successfully',
                });
                this.session = evaluationSession;
              },
              error: (error) => {
                this.messageService.add({
                  severity: 'error',
                  summary: 'Error',
                  detail: 'Failed to end evaluation session',
                });
              },
            });
        }
      },
    });
  }

  generateReport(): void {
    if (this.session) {
      this.reportLoading = true;
      this.evaluationSessionService
        .generateReport(this.session.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.reportLoading = false;
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Report generation completed',
            });
            this.session!.isReportAvailable = true;
          },
          error: (error) => {
            this.reportLoading = false;
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to generate report',
            });
          },
        });
    }
  }

  downloadReport(): void {
    if (this.session) {
      this.reportLoading = true;
      this.evaluationSessionService
        .downloadReport(this.session.id)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: ({ blob, filename }) => {
            const url = window.URL.createObjectURL(blob);
            const link = document.createElement('a');

            link.href = url;
            link.download = filename;

            document.body.appendChild(link);

            link.click();

            document.body.removeChild(link);
            window.URL.revokeObjectURL(url);
            this.reportLoading = false;
          },
          error: (error) => {
            this.reportLoading = false;
            console.error('Error downloading report:', error);
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: error.error || 'Failed to download report',
            });
          },
        });
    }
  }

  getClassStyleClassname(className: string): string {
    return this.employeeClassService.getClassStyleClassname(className);
  }

  openClassRecommendationModal(): void {
    if (!this.session?.class?.recommendedActions?.length) return;
    this.classRecommendation = {
      employeeId: this.session.employeeId,
      recommendationText: this.session.class.recommendedActions.join('\n'),
      isVisibleToEmployee: true
    };
    this.showClassRecommendationModal = true;
  }

  onSaveClassRecommendation(rec: UpdateRecommendationRequest): void {
    this.recommendationService.createRecommendation(rec)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Recommendation created successfully',
          });
          this.showClassRecommendationModal = false;
        },
        error: () => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create recommendation',
          });
          this.showClassRecommendationModal = false;
        }
      });
  }

  onClassRecommendationModalVisibleChange(visible: boolean): void {
    this.showClassRecommendationModal = visible;
  }
}
