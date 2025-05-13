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
import { Role } from '@app/models/role.model';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { Permission } from '@app/models/permission.model';
import { UserPermission } from '@app/shared/user-permission';
import { PERMISSION_CLASSES } from '@app/constants/permission-classes';
import { ButtonModule } from 'primeng/button';
import { DialogModule } from 'primeng/dialog';
import { InputTextModule } from 'primeng/inputtext';
import { CheckboxModule } from 'primeng/checkbox';
import { PERMISSION_DESCRIPTIONS } from '@app/constants/permission-descriptions';

@Component({
  selector: 'app-role-modal',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ButtonModule,
    DialogModule,
    InputTextModule,
    CheckboxModule,
  ],
  templateUrl: './role-modal.component.html',
  styleUrls: ['./role-modal.component.scss'],
})
export class RoleModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() role: Role | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<{
    roleName: string;
    permissionIds: number[];
  }>();

  readonly permissions: Permission[] = Object.values(UserPermission).map(
    (name, index) => ({
      id: index + 1,
      name,
      description: PERMISSION_DESCRIPTIONS[name] || name,
    })
  );
  saving = false;
  form: FormGroup;

  constructor(private fb: FormBuilder) {
    this.form = this.fb.group({
      roleName: ['', [Validators.required, Validators.maxLength(50)]],
      permissions: [[], Validators.required],
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
    if (this.role) {
      this.form.patchValue({
        roleName: this.role.roleName,
        permissions: this.role.permissions.map((p) => p.name),
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
    const formValue = this.form.value;

    const selectedPermissions = this.permissions.filter((permission) =>
      formValue.permissions.includes(permission.name)
    );
    const selectedPermissionIds = selectedPermissions.map(
      (permission) => permission.id
    );

    this.save.emit({
      roleName: formValue.roleName,
      permissionIds: selectedPermissionIds,
    });
  }

  onHide(): void {
    this.visibleChange.emit(false);
  }

  getPermissionDescription(permission: string): string {
    return PERMISSION_DESCRIPTIONS[permission] || permission;
  }

  getPermissionClass(permission: string): string {
    return PERMISSION_CLASSES[permission] || 'permission-default';
  }
}
