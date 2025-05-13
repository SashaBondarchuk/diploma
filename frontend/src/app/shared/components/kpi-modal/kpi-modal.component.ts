import {
  Component,
  Input,
  Output,
  EventEmitter,
  OnChanges,
  SimpleChanges,
} from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { KpiMetric } from '@app/models/kpi.model';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';

@Component({
  selector: 'app-kpi-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
  ],
  templateUrl: './kpi-modal.component.html',
  styleUrls: ['./kpi-modal.component.scss'],
})
export class KpiModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() kpi: KpiMetric | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<{ name: string }>();

  saving = false;
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(50)]],
    });
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
    if (this.kpi) {
      this.form.patchValue({
        name: this.kpi.name,
      });
    } else {
      this.form.reset();
    }
  }

  private resetForm(): void {
    this.form.reset();
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.saving = false;
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    this.save.emit(this.form.value);
  }

  onHide(): void {
    this.visibleChange.emit(false);
  }
}
