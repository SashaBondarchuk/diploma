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
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { SharedModule } from '@app/shared/shared.module';
import { Employee, EmployeePartial } from '@app/models/employee.model';
import {
  Recommendation,
  UpdateRecommendationRequest,
} from '@app/models/recommendation.model';
import { EmployeesService } from '@app/services/employees.service';

@Component({
  selector: 'app-recommendation-modal',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './recommendation-modal.component.html',
  styleUrls: ['./recommendation-modal.component.scss'],
})
export class RecommendationModalComponent implements OnChanges, OnInit {
  @Input() visible = false;
  @Input() employees: Employee[] = [];
  @Input() employee: EmployeePartial | null = null;
  @Input() recommendationToCreate: UpdateRecommendationRequest | null = null;
  @Input() recommendation: Recommendation | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<{
    employeeId: number;
    recommendationText: string;
    isVisibleToEmployee: boolean;
  }>();

  saving = false;
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private employeesService: EmployeesService
  ) {
    this.form = this.fb.group({
      employeeId: ['', Validators.required],
      recommendationText: [
        '',
        [Validators.required, Validators.maxLength(500)],
      ],
      isVisibleToEmployee: [false],
    });
  }

  ngOnInit(): void {
    if (!this.employees || this.employees.length === 0) {
      this.employeesService.getAllEmployees().subscribe({
        next: (employees) => {
          this.employees = employees;
          this.initializeForm();
        },
        error: () => {
          this.employees = [];
          this.initializeForm();
        },
      });
    } else {
      this.initializeForm();
    }
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
    this.form.reset();
    this.employees = this.employees.map((e) => ({
      ...e,
      fullName: `${e.firstName} ${e.lastName}`,
    }));

    if (this.recommendation) {
      const employee = this.employees.find(
        (e) => e.id === this.recommendation?.employeeId
      );
      this.form.patchValue({
        employeeId: employee?.id,
        recommendationText: this.recommendation.recommendationText,
        isVisibleToEmployee: this.recommendation.isVisibleToEmployee,
      });
      this.form.get('employeeId')?.disable();
    } else if (this.recommendationToCreate) {
      const employee = this.employees.find(
        (e) => e.id === this.recommendationToCreate!.employeeId
      );
      this.form.patchValue({
        employeeId: employee?.id,
        recommendationText: this.recommendationToCreate.recommendationText,
        isVisibleToEmployee: this.recommendationToCreate.isVisibleToEmployee,
      });
      this.form.get('employeeId')?.disable();
    } else {
      this.form.get('employeeId')?.enable();
      this.form.patchValue({
        isVisibleToEmployee: false,
      });
    }
  }

  private resetForm(): void {
    this.form.reset();
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.saving = false;
    this.form.get('employeeId')?.enable();
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
      employeeId:
        formValue.employeeId ??
        this.recommendation?.employeeId ??
        this.recommendationToCreate?.employeeId,
    };

    this.save.emit(request);
  }

  onHide(): void {
    this.visibleChange.emit(false);
  }
}
