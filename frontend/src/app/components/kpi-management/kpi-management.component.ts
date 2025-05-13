import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { KpiService } from '@app/services/kpi.service';
import { Subject, takeUntil, debounceTime, distinctUntilChanged } from 'rxjs';
import { KpiMetric } from '@app/models/kpi.model';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { KpiModalComponent } from '@app/shared/components/kpi-modal/kpi-modal.component';
import { FormControl, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-kpi-management',
  standalone: true,
  imports: [CommonModule, SharedModule, KpiModalComponent, ReactiveFormsModule],
  templateUrl: './kpi-management.component.html',
  styleUrls: ['./kpi-management.component.scss'],
})
export class KpiManagementComponent implements OnInit, OnDestroy {
  kpis: KpiMetric[] = [];
  filteredKpis: KpiMetric[] = [];
  loading = false;
  selectedKpi: KpiMetric | null = null;
  showKpiModal = false;
  searchControl = new FormControl('');

  private destroy$ = new Subject<void>();

  constructor(
    private kpiService: KpiService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.loadKpis();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadKpis(): void {
    this.loading = true;
    this.kpiService
      .getAllKpis()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (kpis: KpiMetric[]) => {
          this.kpis = kpis;
          this.filteredKpis = kpis;
          this.loading = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load KPIs',
          });
          this.loading = false;
        },
      });
  }

  openAddDialog(): void {
    this.selectedKpi = null;
    this.showKpiModal = true;
  }

  openEditDialog(kpi: KpiMetric): void {
    this.selectedKpi = kpi;
    this.showKpiModal = true;
  }

  onKpiModalHide(): void {
    this.showKpiModal = false;
    this.selectedKpi = null;
  }

  onSaveKpi(request: { name: string }): void {
    if (this.selectedKpi) {
      this.kpiService
        .updateKpi(request, this.selectedKpi.id.toString())
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'KPI updated successfully',
            });
            this.loadKpis();
            this.showKpiModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update KPI',
            });
          },
        });
    } else {
      this.kpiService
        .createKpi(request)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'KPI created successfully',
            });
            this.loadKpis();
            this.showKpiModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to create KPI',
            });
          },
        });
    }
  }

  confirmDelete(kpi: KpiMetric): void {
    this.confirmationService.confirm({
      header: 'Delete KPI',
      message: `Are you sure you want to delete the KPI "${kpi.name}"?`,
      accept: () => {
        this.deleteKpi(kpi);
      },
    });
  }

  private deleteKpi(kpi: KpiMetric): void {
    this.kpiService
      .deleteKpi(kpi.id.toString())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'KPI deleted successfully',
          });
          this.loadKpis();
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to delete KPI',
          });
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300), distinctUntilChanged())
      .subscribe((searchTerm) => {
        this.filterKpis(searchTerm || '');
      });
  }

  private filterKpis(searchTerm: string): void {
    if (!searchTerm.trim()) {
      this.filteredKpis = this.kpis;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredKpis = this.kpis.filter((kpi) =>
      kpi.name.toLowerCase().includes(searchLower)
    );
  }
}
