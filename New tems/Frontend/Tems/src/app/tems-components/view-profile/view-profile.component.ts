import { Location } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { UserService } from '../../services/user.service';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { UserProfile } from '../../models/user/user-profile.model';
import { Subject, forkJoin, of } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { TicketService } from '../../services/ticket.service';
import { TokenService } from '../../services/token.service';
import { Ticket } from '../../models/ticket/ticket.model';

@Component({
  selector: 'app-view-profile',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule
  ],
  templateUrl: './view-profile.component.html',
  styleUrls: ['./view-profile.component.scss']
})
export class ViewProfileComponent implements OnInit, OnDestroy {
  profile: UserProfile | null = null;
  loading = true;
  error = false;
  assetCount = 0;
  ticketCount = 0;
  openTicketCount = 0;
  closedTicketCount = 0;
  private destroy$ = new Subject<void>();

  constructor(
    private userService: UserService,
    private router: Router,
    private ticketService: TicketService,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.loading = true;
    this.error = false;

    this.userService.getProfile()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
      next: (profile) => {
        this.profile = profile;
        this.loadSummary();
        this.loading = false;
      },
        error: (err) => {
          console.error('Error loading profile:', err);
          this.error = true;
          this.loading = false;
      }
    });
  }

  private loadSummary(): void {
    const assetCount$ = this.userService.getMyAssetCount();
    const ticketCount$ = this.tokenService.canOpenTickets() || this.tokenService.canManageTickets()
      ? this.ticketService.getAll(true)
      : of([] as Ticket[]);

    forkJoin({
      assets: assetCount$,
      tickets: ticketCount$
    })
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: ({ assets, tickets }) => {
          this.assetCount = assets.count || 0;
          const authoredTickets = this.tokenService.canManageTickets()
            ? tickets
            : tickets.filter(ticket => this.profile && (ticket.reporter?.userId || '').toLowerCase() === this.profile.id.toLowerCase());

          this.ticketCount = authoredTickets.length;
          this.openTicketCount = authoredTickets.filter(ticket => this.normalizeTicketStatus(ticket.currentStateId) === 'new' || this.normalizeTicketStatus(ticket.currentStateId) === 'in-progress').length;
          this.closedTicketCount = authoredTickets.filter(ticket => this.normalizeTicketStatus(ticket.currentStateId) === 'closed').length;
        },
        error: () => {
          this.assetCount = 0;
          this.ticketCount = 0;
          this.openTicketCount = 0;
          this.closedTicketCount = 0;
        }
      });
  }

  private normalizeTicketStatus(stateId: string): 'new' | 'in-progress' | 'closed' | null {
    const normalized = (stateId || '').trim().toLowerCase().replace(/\s+/g, '-').replace(/_+/g, '-');
    if (['new', 'open', 'state-new'].includes(normalized)) return 'new';
    if (['in-progress', 'state-in-progress', 'state-wip', 'wip', 'progress'].includes(normalized)) return 'in-progress';
    if (['closed', 'state-closed'].includes(normalized)) return 'closed';
    return null;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
