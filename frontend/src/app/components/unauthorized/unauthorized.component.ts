import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy, Component } from '@angular/core';
import { Router } from '@angular/router';
import { SharedModule } from '@app/shared/shared.module';
import { AuthService } from '@services/auth.service';

@Component({
  selector: 'app-unauthorized',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './unauthorized.component.html',
  styleUrl: './unauthorized.component.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class UnauthorizedComponent {
  constructor(private router: Router, private authService: AuthService) {}

  navigateToLogin(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
