import { Component, OnDestroy, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { ActivatedRoute } from '@angular/router';
import { RecommendationService } from '@app/services/recommendation.service';
import { Recommendation } from '@app/models/recommendation.model';
import { MessageService } from 'primeng/api';
import { RoleService } from '@app/services/role.service';
import { AuthService } from '@app/services/auth.service';
import { Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-recommendation-detail',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './recommendation-detail.component.html',
  styleUrls: ['./recommendation-detail.component.scss'],
})
export class RecommendationDetailComponent implements OnInit, OnDestroy {
  recommendation: Recommendation | null = null;
  loading = false;
  roleClass = '';
  private destroy$ = new Subject<void>();

  constructor(
    private route: ActivatedRoute,
    private recommendationService: RecommendationService,
    private messageService: MessageService,
    private roleService: RoleService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadRecommendation();
    this.authService.currentEmployee$.pipe(takeUntil(this.destroy$)).subscribe(employee => {
      this.roleClass = this.roleService.getRoleClass(employee);
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadRecommendation(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (!id) return;

    this.loading = true;
    this.recommendationService.getRecommendationById(Number(id)).subscribe({
      next: (recommendation) => {
        this.recommendation = recommendation;
        this.loading = false;
      },
      error: (error) => {
        this.messageService.add({
          severity: 'error',
          summary: 'Error',
          detail: 'Failed to load recommendation',
        });
        this.loading = false;
      },
    });
  }

  formatUserAvatar(avatar: string | null): string {
    if (!avatar) return '';
    return avatar.startsWith('data:') ? avatar : `data:image/jpeg;base64,${avatar}`;
  }

  getRoleClass(): string {
    return this.roleClass;
  }
} 