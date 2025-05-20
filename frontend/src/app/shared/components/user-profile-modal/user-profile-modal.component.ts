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
import { AddEditEmployee } from '@models/employee.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { TeamService } from '@app/services/team.service';
import { RoleService } from '@app/services/role.service';
import { Team } from '@app/models/team.model';
import { Role } from '@app/models/role.model';
import { forkJoin } from 'rxjs';
import { AddUpdateEmployeeRequest } from '@models/employee.model';
import { UserRole } from '@app/shared/user-role';
import { UserPartial } from '@app/models/user.model';

@Component({
  selector: 'app-user-profile-modal',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './user-profile-modal.component.html',
  styleUrls: ['./user-profile-modal.component.scss'],
})
export class UserProfileModalComponent implements OnChanges {
  @Input() visible = false;
  @Input() header: string = 'Create Employee Profile';
  @Input() employee: AddEditEmployee | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<AddUpdateEmployeeRequest>();

  teams: Team[] = [];
  roles: Role[] = [];
  loadingTeamsAndRoles = false;
  saving = false;
  currentDate = new Date();

  editForm: FormGroup;

  constructor(
    private fb: FormBuilder,
    private teamService: TeamService,
    private roleService: RoleService
  ) {
    this.editForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.maxLength(50)]],
      lastName: ['', [Validators.required, Validators.maxLength(50)]],
      phoneNumber: [
        '',
        [
          Validators.required,
          Validators.pattern(/^\+?[1-9][0-9]{6,14}(?:[\s\-][0-9]+)*$/),
        ],
      ],
      birthDate: ['', Validators.required],
      teamId: ['', Validators.required],
      roleId: ['', Validators.required],
      isTeamLead: [false],
    });
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible']) {
      if (this.visible) {
        this.loadTeamsAndRoles();
      } else {
        this.resetForm();
      }
    }
  }

  private loadTeamsAndRoles(): void {
    this.loadingTeamsAndRoles = true;
    forkJoin({
      teams: this.teamService.getAllTeams(),
      roles: this.roleService.getAllRoles(),
    }).subscribe({
      next: (result) => {
        this.teams = result.teams;
        this.roles = result.roles.filter(
          (r) => r.roleName !== UserRole.Unassigned
        );
        this.loadingTeamsAndRoles = false;
        this.initializeForm();
      },
      error: (error) => {
        console.error('Error loading teams and roles:', error);
        this.loadingTeamsAndRoles = false;
        this.resetForm();
      },
    });
  }

  private initializeForm(): void {
    if (this.employee) {
      const roleId =
        (this.employee.user as UserPartial).roleName === UserRole.Unassigned
          ? null
          : (this.employee.user as UserPartial).roleId;
      this.editForm.patchValue({
        firstName: this.employee.firstName || '',
        lastName: this.employee.lastName || '',
        phoneNumber: this.employee.phoneNumber || '',
        birthDate: this.employee.birthDate
          ? new Date(this.employee.birthDate)
          : null,
        teamId: this.employee.teamId || null,
        roleId: roleId || null,
        isTeamLead: this.employee.isTeamLead || false,
      });
    }
  }

  private resetForm(): void {
    this.editForm.reset();
    this.editForm.markAsPristine();
    this.editForm.markAsUntouched();
    this.saving = false;
  }

  onHide(): void {
    this.visibleChange.emit(false);
  }

  saveChanges(): void {
    if (this.editForm.invalid || !this.employee) {
      this.editForm.markAllAsTouched();
      return;
    }

    this.saving = true;
    const updatedEmployee: AddUpdateEmployeeRequest = {
      firstName: this.editForm.value.firstName,
      lastName: this.editForm.value.lastName,
      phoneNumber: this.editForm.value.phoneNumber,
      birthDate: this.editForm.value.birthDate.toISOString(),
      teamId: this.editForm.value.teamId,
      roleId: this.editForm.value.roleId,
      isTeamLead: this.editForm.value.isTeamLead,
      userId: this.employee.userId,
      avatar: this.employee.avatar || null,
    };

    this.save.emit(updatedEmployee);
  }
}
