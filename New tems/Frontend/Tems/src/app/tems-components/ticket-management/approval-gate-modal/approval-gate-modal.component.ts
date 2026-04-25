import { CommonModule } from '@angular/common';
import { Component, Inject, OnDestroy, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { catchError, debounceTime, distinctUntilChanged, of, Subject, Subscription, switchMap, tap } from 'rxjs';
import { ApprovalGate, ApprovalGateRequest } from 'src/app/models/ticket/ticket.model';
import { UserLookupDto } from 'src/app/models/user/user-management.model';
import { TicketService } from 'src/app/services/ticket.service';
import { UserService } from 'src/app/services/user.service';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

interface ApprovalGateModalData {
  ticketId: string;
  gate?: ApprovalGate | null;
}

@Component({
  selector: 'app-approval-gate-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, CustomSelectComponent],
  templateUrl: './approval-gate-modal.component.html',
  styleUrls: ['./approval-gate-modal.component.scss']
})
export class ApprovalGateModalComponent implements OnInit, OnDestroy {
  ticketId: string;
  gate: ApprovalGate | null;
  gateTitle = '';
  gateJustification = '';
  allApproversRequired = false;
  approverIds: string[] = [];
  approverOptions: SelectOption[] = [];
  loading = false;
  submitting = false;
  errorMessage = '';

  private readonly searchTerms$ = new Subject<string>();
  private readonly subscriptions = new Subscription();

  constructor(
    public dialogRef: MatDialogRef<ApprovalGateModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ApprovalGateModalData,
    private ticketService: TicketService,
    private userService: UserService
  ) {
    this.ticketId = data.ticketId;
    this.gate = data.gate ?? null;
    this.populateGate();
  }

  ngOnInit(): void {
    this.subscriptions.add(this.searchTerms$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      tap(() => {
        this.loading = true;
        this.errorMessage = '';
      }),
      switchMap((searchText) =>
        this.userService.searchUsersByName(searchText, 12).pipe(
          catchError(() => {
            this.errorMessage = 'Failed to load users.';
            return of([] as UserLookupDto[]);
          })
        )
      )
    ).subscribe((users) => {
      this.approverOptions = this.mapUsers(users);
      this.ensureSelectedUsersVisible();
      this.loading = false;
    }));

    this.searchTerms$.next('');
  }

  ngOnDestroy(): void {
    this.subscriptions.unsubscribe();
    this.searchTerms$.complete();
  }

  close(): void {
    this.dialogRef.close();
  }

  onSearchChange(searchText: string): void {
    this.searchTerms$.next(searchText);
  }

  async save(): Promise<void> {
    if (!this.gateTitle.trim() || this.approverIds.length === 0 || this.submitting) {
      return;
    }

    this.submitting = true;
    this.errorMessage = '';

    const request: ApprovalGateRequest = {
      title: this.gateTitle.trim(),
      justification: this.gateJustification.trim(),
      allApproversRequired: this.allApproversRequired,
      approverUserIds: this.approverIds
    };

    const stream$ = this.gate?.approvalGateId
      ? this.ticketService.updateApprovalGate(this.ticketId, this.gate.approvalGateId, request)
      : this.ticketService.createApprovalGate(this.ticketId, request);

    stream$.subscribe({
      next: (response) => {
        this.dialogRef.close({ success: true, gate: response.gate });
      },
      error: () => {
        this.errorMessage = 'Failed to save approval gate.';
        this.submitting = false;
      }
    });
  }

  private populateGate(): void {
    if (!this.gate) {
      return;
    }

    this.gateTitle = this.gate.title || '';
    this.gateJustification = this.gate.justification || '';
    this.allApproversRequired = !!this.gate.allApproversRequired;
    this.approverIds = (this.gate.approvers || []).map((approver) => approver.userId);
  }

  private mapUsers(users: UserLookupDto[]): SelectOption[] {
    return users.map((user) => ({
      value: user.id,
      label: user.displayName || user.name || user.email || user.id
    }));
  }

  private ensureSelectedUsersVisible(): void {
    const missingIds = this.approverIds.filter((id) => !this.approverOptions.some((option) => option.value === id));
    if (missingIds.length === 0) {
      return;
    }

    missingIds.forEach((userId) => {
      this.userService.getUserPreviewById(userId).subscribe({
        next: (user) => {
          const label = user.firstName && user.lastName
            ? `${user.firstName} ${user.lastName}`
            : user.username || user.email || user.id;

          this.approverOptions = [
            { value: user.id, label },
            ...this.approverOptions.filter((option) => option.value !== user.id)
          ];
        }
      });
    });
  }
}
