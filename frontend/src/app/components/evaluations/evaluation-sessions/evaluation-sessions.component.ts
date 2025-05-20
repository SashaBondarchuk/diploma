import { Component, OnInit, OnDestroy, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { EvaluationSessionService } from '@app/services/evaluation-session.service';
import { EmployeesService } from '@app/services/employees.service';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { EvaluationSessionViewModel } from '@app/models/evaluation-session.model';
import { Employee } from '@app/models/employee.model';
import { AddEvaluationSessionRequest } from '@app/models/evaluation-session.model';
import { Router } from '@angular/router';
import { AddEvaluationSessionModalComponent } from '@app/shared/components/add-evaluation-session-modal/add-evaluation-session-modal.component';
import { EmployeeClassService } from '@app/services/employee-class.service';

@Component({
  selector: 'app-evaluation-sessions',
  standalone: true,
  imports: [CommonModule, SharedModule, AddEvaluationSessionModalComponent],
  templateUrl: './evaluation-sessions.component.html',
  styleUrls: ['./evaluation-sessions.component.scss'],
})
export class EvaluationSessionsComponent implements OnInit, OnDestroy {
  @Input() canManageEvaluations = false;

  evaluationSessions: EvaluationSessionViewModel[] = [];
  filteredSessions: EvaluationSessionViewModel[] = [];
  employees: Employee[] = [];
  loading = false;
  showAddModal = false;
  selectedSession: EvaluationSessionViewModel | null = null;

  searchForm: FormGroup;

  private destroy$ = new Subject<void>();

  constructor(
    private evaluationSessionService: EvaluationSessionService,
    private employeesService: EmployeesService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    private router: Router,
    private fb: FormBuilder,
    private employeeClassService: EmployeeClassService
  ) {
    this.searchForm = this.fb.group({
      searchTerm: [''],
      employeeId: [null],
      isFinished: [null],
    });
  }

  ngOnInit(): void {
    this.loadEmployees();
    this.loadEvaluationSessions();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadEmployees(): void {
    this.employeesService
      .getAllEmployees()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (employees) => {
          this.employees = employees.map((e) => ({
            ...e,
            fullName: `${e.firstName} ${e.lastName}`,
          }));
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load employees',
          });
        },
      });
  }

  private loadEvaluationSessions(): void {
    this.loading = true;
    const filters = this.searchForm.value;
    const params: { [key: string]: string } = {};

    if (filters.employeeId) {
      params['employeeId'] = filters.employeeId.toString();
    }
    if (filters.isFinished === true) {
      params['isFinished'] = 'true';
    }

    let request$ = this.evaluationSessionService.getAllEvaluationSessions();
    if (Object.keys(params).length > 0) {
      request$ =
        this.evaluationSessionService.getFilteredEvaluationSessions(params);
    }

    request$.pipe(takeUntil(this.destroy$)).subscribe({
      next: (sessions) => {
        this.evaluationSessions = sessions;
        this.applySearch();
        this.loading = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load evaluation sessions',
        });
        this.loading = false;
      },
    });
  }

  openAddDialog(): void {
    this.showAddModal = true;
  }

  onAddModalHide(): void {
    this.showAddModal = false;
  }

  onSaveSession(session: AddEvaluationSessionRequest): void {
    this.evaluationSessionService
      .createEvaluationSession(session)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Evaluation session created successfully',
          });
          this.loadEvaluationSessions();
          this.showAddModal = false;
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to create evaluation session',
          });
        },
      });
  }

  viewSessionDetails(session: EvaluationSessionViewModel): void {
    this.router.navigate(['/home/evaluation-session', session.id]);
  }

  confirmDelete(session: EvaluationSessionViewModel): void {
    this.confirmationService.confirm({
      header: 'Delete Evaluation Session',
      message: `Are you sure you want to delete evaluation session "${session.name}"?`,
      accept: () => {
        this.deleteSession(session);
      },
    });
  }

  private deleteSession(session: EvaluationSessionViewModel): void {
    this.evaluationSessionService
      .deleteEvaluationSession(session.id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Evaluation session deleted successfully',
          });
          this.loadEvaluationSessions();
        },
        error: (error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to delete evaluation session',
          });
        },
      });
  }

  hasAvatar(avatar: string | null | undefined): boolean {
    return !!avatar && avatar.trim().length > 0;
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }

  private setupSearch(): void {
    this.searchForm.valueChanges
      .pipe(
        takeUntil(this.destroy$),
        distinctUntilChanged((prev, curr) => {
          return (
            prev.isFinished === curr.isFinished &&
            prev.employeeId === curr.employeeId
          );
        })
      )
      .subscribe(() => {
        const isEmployeeIdDirty = this.searchForm.get('employeeId')?.dirty;
        const isIsFinishedDirty = this.searchForm.get('isFinished')?.dirty;

        if (isEmployeeIdDirty || isIsFinishedDirty) {
          this.loadEvaluationSessions();
        }
      });

    this.searchForm
      .get('searchTerm')
      ?.valueChanges.pipe(
        takeUntil(this.destroy$),
        debounceTime(200),
        distinctUntilChanged()
      )
      .subscribe(() => {
        this.applySearch();
      });
  }

  private applySearch(): void {
    const searchTerm =
      this.searchForm.get('searchTerm')?.value?.toLowerCase() || '';
    if (!searchTerm) {
        this.filteredSessions = this.evaluationSessions;
        return;
    }

    this.filteredSessions = this.evaluationSessions?.filter((session) =>
      session.name.toLowerCase().includes(searchTerm)
    );
  }

  getClassStyleClassname(className: string): string {
    return this.employeeClassService.getClassStyleClassname(className);
  }
}
