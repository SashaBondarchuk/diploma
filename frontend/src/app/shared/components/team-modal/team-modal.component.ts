import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  Validators,
} from '@angular/forms';
import { AuthService } from '../../../services/auth.service';
import { Team, TeamWithEmployees } from '../../../models/team.model';
import { EmployeePartial } from '../../../models/employee.model';
import { UserPermission } from '@app/shared/user-permission';
import { SharedModule } from '@app/shared/shared.module';

@Component({
  selector: 'app-team-modal',
  standalone: true,
  imports: [
    CommonModule,
    SharedModule,
  ],
  templateUrl: './team-modal.component.html',
  styleUrls: ['./team-modal.component.scss'],
})
export class TeamModalComponent {
  @Input() visible = false;
  @Input() isViewModeSelected = false;
  @Input() team: TeamWithEmployees | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<Partial<Team>>();

  form: FormGroup;
  isViewMode = false;
  teamLead: EmployeePartial | null = null;
  teamMembers: EmployeePartial[] = [];

  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.form = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
    });
  }

  ngOnChanges(): void {
    if (this.team) {
      this.isViewMode =
        this.isViewModeSelected ||
        !this.authService.hasPermission(UserPermission.ManageTeams);
      this.initializeTeamMembers();
      this.initializeForm();
    } else {
      this.isViewMode = false;
      this.teamLead = null;
      this.teamMembers = [];
      this.form.reset();
      this.form.enable();
    }
  }

  private initializeTeamMembers(): void {
    this.teamLead =
      this.team!.employees.find((emp) => emp.id === this.team!.teamLeadId) ||
      null;
    this.teamMembers = this.team!.employees.filter(
      (emp) => emp.id !== this.team!.teamLeadId
    );
  }

  private initializeForm(): void {
    if (this.team) {
      this.form.patchValue({
        name: this.team.name,
      });

      if (this.isViewMode) {
        this.form.disable();
      } else {
        this.form.enable();
      }
    }
  }

  onSave(): void {
    if (this.form.valid) {
      this.save.emit(this.form.value);
    }
  }

  onCancel(): void {
    this.visibleChange.emit(false);
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:') ? avatar : `data:image/jpeg;base64,${avatar}`;
  }
}
