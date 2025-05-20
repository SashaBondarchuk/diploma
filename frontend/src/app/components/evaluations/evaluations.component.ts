import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { AuthService } from '@app/services/auth.service';
import { UserPermission } from '@app/shared/user-permission';
import { EvaluationSessionsComponent } from './evaluation-sessions/evaluation-sessions.component';
import { EmployeeClassesComponent } from './employee-classes/employee-classes.component';

@Component({
  selector: 'app-evaluations',
  standalone: true,
  imports: [
    CommonModule,
    SharedModule,
    EvaluationSessionsComponent,
    EmployeeClassesComponent,
  ],
  templateUrl: './evaluations.component.html',
  styleUrls: ['./evaluations.component.scss'],
})
export class EvaluationsComponent implements OnInit {
  canManageEvaluations = false;

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.checkPermissions();
  }

  private checkPermissions(): void {
    this.canManageEvaluations = this.authService.hasPermission(
      UserPermission.ManageEvaluations
    );
  }
}
