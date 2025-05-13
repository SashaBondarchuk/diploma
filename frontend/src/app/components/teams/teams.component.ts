import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { TeamService } from '@app/services/team.service';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';
import { Team, TeamWithEmployees } from '@app/models/team.model';
import { MessageService } from 'primeng/api';
import { ConfirmationService } from 'primeng/api';
import { FormControl } from '@angular/forms';
import { AuthService } from '@app/services/auth.service';
import { TeamModalComponent } from '../../shared/components/team-modal/team-modal.component';
import { UserPermission } from '@app/shared/user-permission';

@Component({
  selector: 'app-teams',
  standalone: true,
  imports: [CommonModule, SharedModule, TeamModalComponent],
  templateUrl: './teams.component.html',
  styleUrls: ['./teams.component.scss'],
})
export class TeamsComponent implements OnInit, OnDestroy {
  teams: Team[] = [];
  filteredTeams: Team[] = [];
  loading = false;
  selectedTeam: TeamWithEmployees | null = null;
  showTeamModal = false;
  isViewModeSelected = false;
  searchControl = new FormControl('');
  canEdit = false;

  private destroy$ = new Subject<void>();

  constructor(
    private teamService: TeamService,
    private messageService: MessageService,
    private confirmationService: ConfirmationService,
    public authService: AuthService
  ) {}

  ngOnInit(): void {
    this.checkPermissions();
    this.loadTeams();
    this.setupSearch();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadTeams(): void {
    this.loading = true;
    this.teamService
      .getAllTeams()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (teams: Team[]) => {
          this.teams = teams;
          this.applySearch();
          this.loading = false;
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to load teams',
          });
          this.loading = false;
        },
      });
  }

  openAddDialog(): void {
    this.selectedTeam = null;
    this.showTeamModal = true;
  }

  openEditDialog(team: Team): void {
    this.teamService.getTeamById(team.id.toString()).subscribe({
      next: (team) => {
        this.selectedTeam = team;
        this.showTeamModal = true;
      },
      error: (error) => {
        console.error('Error loading team:', error);
      },
    });
  }

  openViewDialog(team: Team): void {
    this.teamService.getTeamById(team.id.toString()).subscribe({
      next: (team) => {
        this.selectedTeam = team;
        this.showTeamModal = true;
        this.isViewModeSelected = true;
      },
      error: (error) => {
        console.error('Error loading team:', error);
      },
    });
  }

  onTeamModalHide(): void {
    this.showTeamModal = false;
    this.selectedTeam = null;
    this.isViewModeSelected = false;
  }

  onSaveTeam(team: Partial<Team>): void {
    if (this.selectedTeam) {
      this.teamService
        .updateTeam(team, this.selectedTeam.id.toString())
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Team updated successfully',
            });
            this.loadTeams();
            this.showTeamModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to update team',
            });
            this.showTeamModal = false;
          },
        });
    } else {
      this.teamService
        .createTeam(team)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: () => {
            this.messageService.add({
              severity: 'success',
              summary: 'Success',
              detail: 'Team created successfully',
            });
            this.loadTeams();
            this.showTeamModal = false;
          },
          error: (error: Error) => {
            this.messageService.add({
              severity: 'error',
              summary: 'Error',
              detail: 'Failed to create team',
            });
            this.showTeamModal = false;
          },
        });
    }
  }

  confirmDelete(team: Team): void {
    this.confirmationService.confirm({
      header: 'Delete Team',
      message: `Are you sure you want to delete team "${team.name}"?`,
      accept: () => {
        this.deleteTeam(team);
      },
    });
  }

  private deleteTeam(team: Team): void {
    this.teamService
      .deleteTeam(team.id.toString())
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.messageService.add({
            severity: 'success',
            summary: 'Success',
            detail: 'Team deleted successfully',
          });
          this.loadTeams();
        },
        error: (error: Error) => {
          this.messageService.add({
            severity: 'error',
            summary: 'Error',
            detail: 'Failed to delete team',
          });
        },
      });
  }

  private setupSearch(): void {
    this.searchControl.valueChanges
      .pipe(takeUntil(this.destroy$), debounceTime(300), distinctUntilChanged())
      .subscribe((searchTerm) => {
        this.applySearch();
      });
  }

  private applySearch(): void {
    const searchTerm = this.searchControl.value || '';
    if (!searchTerm.trim()) {
      this.filteredTeams = this.teams;
      return;
    }

    const searchLower = searchTerm.toLowerCase();
    this.filteredTeams = this.teams.filter(
      (team) =>
        team.name.toLowerCase().includes(searchLower) ||
        `${team.teamLeadFirstName} ${team.teamLeadLastName}`
          .toLowerCase()
          .includes(searchLower)
    );
  }

  private checkPermissions(): void {
    this.canEdit = this.authService.hasPermission(UserPermission.ManageTeams);
  }

  formatUserAvatar(avatar: string): string {
    return avatar.startsWith('data:') ? avatar : `data:image/jpeg;base64,${avatar}`;
  }
}
