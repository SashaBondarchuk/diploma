import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@app/shared/shared.module';
import { EvaluationDetailViewModel } from '@app/models/evaluation.model';

@Component({
  selector: 'app-evaluation-details-modal',
  standalone: true,
  imports: [CommonModule, SharedModule],
  templateUrl: './evaluation-details-modal.component.html',
  styleUrls: ['./evaluation-details-modal.component.scss'],
})
export class EvaluationDetailsModalComponent {
  @Input() visible = false;
  @Input() evaluation: EvaluationDetailViewModel | null = null;
  @Output() visibleChange = new EventEmitter<boolean>();

  onHide(): void {
    this.visibleChange.emit(false);
  }

  formatUserAvatar(avatar: string): string {
    return avatar?.startsWith('data:')
      ? avatar
      : `data:image/jpeg;base64,${avatar}`;
  }

  hasAvatar(avatar: string | null | undefined): boolean {
    return !!avatar && avatar.trim().length > 0;
  }
}
