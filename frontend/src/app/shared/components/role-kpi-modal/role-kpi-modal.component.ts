import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
  OnInit,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { Role } from '@app/models/role.model';
import { KpiMetric } from '@app/models/kpi.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { RoleKPIModel } from '@app/models/role-kpi.model';
import { SharedModule } from '@app/shared/shared.module';

@Component({
  selector: 'app-role-kpi-modal',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './role-kpi-modal.component.html',
  styleUrls: ['./role-kpi-modal.component.scss'],
})
export class RoleKpiModalComponent implements OnChanges, OnInit {
  @Input() visible = false;
  @Input() roleKpi: RoleKPIModel | null = null;
  @Input() roles: Role[] = [];
  @Input() kpis: KpiMetric[] = [];
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<{
    roleId: number;
    kpiId: number;
    weight: number;
    minScore: number;
    maxScore: number;
    isAllowedToEvaluateExceptLead: boolean;
    scoreRangeDescription: string;
  }>();

  saving = false;
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      roleId: ['', Validators.required],
      kpiId: ['', Validators.required],
      weight: [
        null,
        [Validators.required, Validators.min(0), Validators.max(100)],
      ],
      minScore: [
        { value: 0, disabled: true },
        [Validators.required, Validators.min(0)],
      ],
      maxScore: [
        { value: 100, disabled: true },
        [Validators.required, Validators.min(0)],
      ],
      isAllowedToEvaluateExceptLead: [false],
      scoreRangeDescription: [
        '',
        [Validators.required, Validators.maxLength(200)],
      ],
    });
  }

  ngOnInit(): void {
    this.initializeForm();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible']) {
      if (this.visible) {
        this.initializeForm();
      } else {
        this.resetForm();
      }
    }
  }

  private initializeForm(): void {
    if (this.roleKpi) {
      this.form.patchValue({
        roleId: this.roleKpi.roleId,
        kpiId: this.roleKpi.kpiId,
        weight: this.roleKpi.weight * 100,
        minScore: 1,
        maxScore: 100,
        isAllowedToEvaluateExceptLead:
          this.roleKpi.isAllowedToEvaluateExceptLead,
        scoreRangeDescription: this.roleKpi.scoreRangeDescription,
      });
      this.form.get('roleId')?.disable();
      this.form.get('kpiId')?.disable();
    } else {
      this.form.reset();
      this.form.patchValue({
        minScore: 1,
        maxScore: 100,
        isAllowedToEvaluateExceptLead: false,
      });
      this.form.get('roleId')?.enable();
      this.form.get('kpiId')?.enable();
    }
  }

  private resetForm(): void {
    this.form.reset();
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.saving = false;
    this.form.get('roleId')?.enable();
    this.form.get('kpiId')?.enable();
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }
    
    this.saving = true;
    const formValue = this.form.value;
    
    const request = {
      ...formValue,
      minScore: 1,
      maxScore: 100,
      
      weight: formValue.weight / 100,
    };

    this.save.emit(request);
  }

  onHide(): void {
    this.visibleChange.emit(false);
  }
}
