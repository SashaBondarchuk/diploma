import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { EvaluationSessionService } from '@app/services/evaluation-session.service';
import { RoleKpiService } from '@app/services/role-kpi.service';
import { EvaluationService } from '@app/services/evaluation.service';
import { MessageService } from 'primeng/api';
import { EvaluationSessionViewModel } from '@app/models/evaluation-session.model';
import { RoleKPIModel } from '@app/models/role-kpi.model';
import { MenuItem } from 'primeng/api';
import { StepsModule } from 'primeng/steps';
import { TableModule } from 'primeng/table';
import { TableRowSelectEvent } from 'primeng/table';
import { StepperModule } from 'primeng/stepper';

@Component({
  selector: 'app-evaluate-employee',
  standalone: true,
  imports: [CommonModule, SharedModule, StepsModule, TableModule, StepperModule],
  templateUrl: './evaluate-employee.component.html',
  styleUrls: ['./evaluate-employee.component.scss'],
})
export class EvaluateEmployeeComponent implements OnInit, OnDestroy {
  activeIndex = 0;
  loading = false;
  roleKpiLoading = false;
  pendingSessions: EvaluationSessionViewModel[] = [];
  selectedSession: EvaluationSessionViewModel | null = null;
  roleKpis: RoleKPIModel[] = [];
  selectedRoleKpi: RoleKPIModel | null = null;
  evaluationForm: FormGroup;

  steps: MenuItem[] = [
    { label: 'Select Session' },
    { label: 'Select KPI' },
    { label: 'Add Evaluation' },
  ];

  private destroy$ = new Subject<void>();

  constructor(
    private evaluationSessionService: EvaluationSessionService,
    private roleKpiService: RoleKpiService,
    private evaluationService: EvaluationService,
    private messageService: MessageService,
    private fb: FormBuilder
  ) {
    this.evaluationForm = this.fb.group({
      score: [
        null,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
      comment: ['', [Validators.maxLength(500)]],
    });
  }

  ngOnInit(): void {
    this.loadPendingSessions();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadPendingSessions(): void {
    this.loading = true;
    this.evaluationSessionService
      .getPendingEvaluationSessions()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (sessions) => {
          this.pendingSessions = sessions;
          this.loading = false;
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load pending evaluation sessions',
          });
          this.loading = false;
        },
      });
  }

  onSessionSelect(event: TableRowSelectEvent): void {
    if (event.data) {
      this.selectedSession = event.data;
      this.loadRoleKpis(event.data.id);
    }
  }

  private loadRoleKpis(sessionId: number): void {
    this.roleKpiLoading = true;
    this.roleKpiService
      .getKpisToEvaluate(sessionId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (kpis) => {
          this.roleKpis = kpis  ?? [];
          this.roleKpiLoading = false;
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load KPIs to evaluate',
          });
          this.roleKpiLoading = false;
        },
      });
  }

  onRoleKpiSelect(event: TableRowSelectEvent): void {
    if (event.data) {
      this.selectedRoleKpi = event.data;
      this.evaluationForm.patchValue({
        score: null,
        comment: '',
      });
    }
  }

  onSubmitEvaluation(): void {
    if (
      this.evaluationForm.invalid ||
      !this.selectedSession ||
      !this.selectedRoleKpi
    ) {
      return;
    }

    const formValue = this.evaluationForm.value;
    const request = {
      evaluationSessionId: this.selectedSession.id,
      kpiId: this.selectedRoleKpi.kpiId,
      roleId: this.selectedRoleKpi.roleId,
      score: formValue.score,
      comment: formValue.comment || undefined,
    };

    this.loading = true;
    this.evaluationService
      .createEvaluation(request)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Evaluation submitted successfully',
          });
          this.loading = false;
          this.resetStepper();
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to submit evaluation',
          });
          this.loading = false;
        },
      });
  }

  nextStep(): void {
    this.activeIndex++;
  }

  prevStep(): void {
    this.activeIndex--;
    this.evaluationForm.markAsUntouched();
    this.evaluationForm.markAsPristine();
  }

  resetStepper(): void {
    this.activeIndex = 0;
    this.selectedSession = null;
    this.selectedRoleKpi = null;
    this.evaluationForm.reset();
  }

  hasAvatar(avatar: string | null | undefined): boolean {
    return !!avatar && avatar.trim().length > 0;
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }

  get remainingKpisCount(): number {
    return this.roleKpis.length;
  }
}
