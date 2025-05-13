import { Component, Input, Output, EventEmitter, OnInit, AfterViewInit, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { Employee } from '@models/employee.model';
import { JwtPayload } from '@models/auth.model';
import { RoleService } from '@app/services/role.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnChanges {
  @Input() userData: JwtPayload | null = null;
  @Input() employeeData: Employee | null = null;
  @Output() logoutEvent = new EventEmitter<void>();
  
  constructor(private roleService: RoleService) {}

  ngOnChanges(changes: SimpleChanges): void {
    this.loadUserAvatar();
  }

  logout(): void {
    this.logoutEvent.emit();
  }

  loadUserAvatar(): void {
    if (!this.employeeData || !this.employeeData.avatar) {
      return;
    }
    this.employeeData!.avatar = `data:image/png;base64,${this.employeeData!.avatar}`;
  }

  getRoleClass(roleName: string): string {
    return this.roleService.getRoleClass(this.employeeData);
  }
}
