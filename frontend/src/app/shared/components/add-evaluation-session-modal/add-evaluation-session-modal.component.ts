import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Employee } from '@app/models/employee.model';
import { AddEvaluationSessionRequest } from '@app/models/evaluation-session.model';

@Component({
  selector: 'app-add-evaluation-session-modal',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './add-evaluation-session-modal.component.html',
  styleUrls: ['./add-evaluation-session-modal.component.scss'],
})
export class AddEvaluationSessionModalComponent {
  @Input() visible = false;
  @Input() employees: Employee[] = [];
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<AddEvaluationSessionRequest>();

  form: FormGroup;
  saving = false;
  allowedEndDate = new Date(new Date().setDate(new Date().getDate() + 2));

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      endDate: [null, [Validators.required]],
      employeeId: [null, [Validators.required]],
    });
  }

  onSave(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    this.saving = true;
    const formValue = this.form.value;
    const request: AddEvaluationSessionRequest = {
      ...formValue,
      endDate: formValue.endDate.toISOString(),
    };

    this.save.emit(request);
  }

  onHide(): void {
    this.visibleChange.emit(false);
    this.form.reset();
    this.form.markAsPristine();
    this.form.markAsUntouched();
    this.saving = false;
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }
}
