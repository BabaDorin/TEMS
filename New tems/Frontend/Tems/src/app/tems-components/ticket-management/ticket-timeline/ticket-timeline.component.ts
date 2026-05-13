import { Component, Input, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChangeLogAction, ChangeLogEntry } from 'src/app/models/changelog.model';
import { TicketService } from 'src/app/services/ticket.service';

@Component({
  selector: 'app-ticket-timeline',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ticket-timeline.component.html',
  styleUrls: ['./ticket-timeline.component.scss']
})
export class TicketTimelineComponent implements OnChanges {
  @Input() ticketId = '';

  entries: ChangeLogEntry[] = [];
  loading = false;
  error: string | null = null;
  totalCount = 0;
  pageNumber = 1;
  pageSize = 25;
  loadedCount = 0;

  constructor(private ticketService: TicketService) {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['ticketId']?.currentValue) {
      this.loadTimeline();
    }
  }

  loadTimeline(): void {
    if (!this.ticketId) {
      return;
    }

    this.loading = true;
    this.error = null;
    this.pageNumber = 1;

    this.ticketService.getHistory(this.ticketId, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        this.entries = response.entries || [];
        this.totalCount = response.totalCount || 0;
        this.loadedCount = this.entries.length;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load ticket history';
        this.loading = false;
      }
    });
  }

  loadMore(): void {
    if (!this.hasMore) {
      return;
    }

    this.pageNumber += 1;
    this.ticketService.getHistory(this.ticketId, this.pageNumber, this.pageSize).subscribe({
      next: (response) => {
        this.entries = [...this.entries, ...(response.entries || [])];
        this.totalCount = response.totalCount || this.totalCount;
        this.loadedCount = this.entries.length;
      }
    });
  }

  get hasMore(): boolean {
    return this.loadedCount < this.totalCount;
  }

  getTitle(entry: ChangeLogEntry): string {
    switch (entry.action) {
      case 'TicketCreated':
        return 'Ticket created';
      case 'TicketUpdated':
        return 'Ticket updated';
      case 'TicketApprovalGateAdded':
        return 'Approval gate added';
      case 'TicketApprovalGateRemoved':
        return 'Approval gate removed';
      case 'TicketStatusUpdated':
        return 'Status updated';
      case 'TicketApprovalGateReviewed':
        return 'Approval decision recorded';
      default:
        return 'History event';
    }
  }

  shouldShowTitle(entry: ChangeLogEntry): boolean {
    return this.getTitle(entry) !== 'History event';
  }

  getIcon(action: ChangeLogAction): string {
    switch (action) {
      case 'TicketCreated':
        return 'mdi-ticket-plus-outline';
      case 'TicketUpdated':
        return 'mdi-pencil-outline';
      case 'TicketApprovalGateAdded':
        return 'mdi-shield-plus-outline';
      case 'TicketApprovalGateRemoved':
        return 'mdi-shield-remove-outline';
      case 'TicketStatusUpdated':
        return 'mdi-progress-check';
      case 'TicketApprovalGateReviewed':
        return 'mdi-check-decagram-outline';
      default:
        return 'mdi-history';
    }
  }

  getAccentClass(action: ChangeLogAction): string {
    switch (action) {
      case 'TicketCreated':
        return 'bg-green-100 text-green-700 border-green-200 dark:bg-green-500/15 dark:text-green-300 dark:border-green-500/20';
      case 'TicketUpdated':
        return 'bg-blue-100 text-blue-700 border-blue-200 dark:bg-blue-500/15 dark:text-blue-300 dark:border-blue-500/20';
      case 'TicketApprovalGateAdded':
        return 'bg-violet-100 text-violet-700 border-violet-200 dark:bg-violet-500/15 dark:text-violet-300 dark:border-violet-500/20';
      case 'TicketApprovalGateRemoved':
        return 'bg-rose-100 text-rose-700 border-rose-200 dark:bg-rose-500/15 dark:text-rose-300 dark:border-rose-500/20';
      case 'TicketStatusUpdated':
        return 'bg-amber-100 text-amber-700 border-amber-200 dark:bg-amber-500/15 dark:text-amber-300 dark:border-amber-500/20';
      case 'TicketApprovalGateReviewed':
        return 'bg-teal-100 text-teal-700 border-teal-200 dark:bg-teal-500/15 dark:text-teal-300 dark:border-teal-500/20';
      default:
        return 'bg-gray-100 text-gray-700 border-gray-200 dark:bg-white/10 dark:text-gray-300 dark:border-white/10';
    }
  }

  getFieldChanges(entry: ChangeLogEntry): { fieldName: string; oldValue?: string; newValue?: string }[] {
    return Array.isArray(entry.details?.['changes']) ? entry.details?.['changes'] : [];
  }

}
