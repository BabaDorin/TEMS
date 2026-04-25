import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
import { AssetService } from '../../services/asset.service';
import { TicketService } from '../../services/ticket.service';
import { UserService } from '../../services/user.service';
import { TokenService } from '../../services/token.service';
import { Ticket } from '../../models/ticket/ticket.model';
import { forkJoin, of } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import { trigger, transition, style, animate, stagger, query } from '@angular/animations';

interface QuickAction {
  icon: string;
  title: string;
  description: string;
  route: string;
  color: string;
  gradient: string;
}

interface Feature {
  icon: string;
  title: string;
  description: string;
}

interface StatCard {
  icon: string;
  value: string;
  label: string;
  trend?: string;
  color: string;
}

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatButtonModule,
    MatIconModule,
    MatCardModule,
    TranslateModule
  ],
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  animations: [
    trigger('fadeInUp', [
      transition(':enter', [
        style({ opacity: 0, transform: 'translateY(20px)' }),
        animate('600ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 1, transform: 'translateY(0)' }))
      ])
    ]),
    trigger('staggerFadeIn', [
      transition('* => *', [
        query(':enter', [
          style({ opacity: 0, transform: 'translateY(20px)' }),
          stagger(100, [
            animate('600ms cubic-bezier(0.4, 0, 0.2, 1)', style({ opacity: 1, transform: 'translateY(0)' }))
          ])
        ], { optional: true })
      ])
    ])
  ]
})
export class HomeComponent implements OnInit {
  isAuthenticated = false;
  userName = '';
  currentUserId = '';
  loadingDashboard = false;
  assignedAssetCount = 0;
  systemAssetCount = 0;
  ticketStats: { label: string; value: number; icon: string; color: string }[] = [];
  dashboardActions: QuickAction[] = [];

  features: Feature[] = [
    {
      icon: 'speed',
      title: 'Lightning Fast',
      description: 'Built for performance with modern web technologies'
    },
    {
      icon: 'security',
      title: 'Secure & Reliable',
      description: 'Enterprise-grade security and data protection'
    },
    {
      icon: 'groups',
      title: 'Team Collaboration',
      description: 'Work seamlessly with your team in real-time'
    },
    {
      icon: 'cloud_done',
      title: 'Cloud-Based',
      description: 'Access your data anywhere, anytime'
    },
    {
      icon: 'auto_awesome',
      title: 'Smart Automation',
      description: 'Automate workflows and save valuable time'
    },
    {
      icon: 'insights',
      title: 'Advanced Analytics',
      description: 'Make data-driven decisions with powerful insights'
    }
  ];

  stats: StatCard[] = [
  ];

  constructor(
    private authService: AuthService,
    private router: Router,
    private assetService: AssetService,
    private ticketService: TicketService,
    private userService: UserService,
    private tokenService: TokenService
  ) {}

  ngOnInit(): void {
    this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
      if (isAuth) {
        this.userName = this.authService.getUserName();
        this.currentUserId = this.tokenService.getUserId() || '';
        this.loadDashboard();
      } else {
        this.stats = [];
        this.dashboardActions = [];
        this.ticketStats = [];
        this.assignedAssetCount = 0;
        this.systemAssetCount = 0;
      }
    });
  }

  private loadDashboard(): void {
    this.loadingDashboard = true;

    const assetCount$ = this.tokenService.canManageAssets()
      ? this.assetService.getAll(undefined, 1, 1).pipe(
          map(response => response.totalCount || 0),
          catchError(() => of(0))
        )
      : this.userService.getMyAssetCount().pipe(
          map(response => response.count || 0),
          catchError(() => of(0))
        );

    const ticketCount$ = (this.tokenService.canOpenTickets() || this.tokenService.canManageTickets())
      ? this.ticketService.getAll(true).pipe(
          catchError(() => of([] as Ticket[]))
        )
      : of([] as Ticket[]);

    forkJoin({
      assetCount: assetCount$,
      tickets: ticketCount$
    }).subscribe({
      next: ({ assetCount, tickets }) => {
        this.systemAssetCount = this.tokenService.canManageAssets() ? assetCount : 0;
        this.assignedAssetCount = this.tokenService.canManageAssets() ? 0 : assetCount;
        this.ticketStats = this.buildTicketStats(tickets || []);
        this.dashboardActions = this.buildDashboardActions();
        this.stats = this.buildStatsCards();
        this.loadingDashboard = false;
      },
      error: () => {
        this.loadingDashboard = false;
        this.stats = this.buildStatsCards();
        this.dashboardActions = this.buildDashboardActions();
      }
    });
  }

  private buildTicketStats(tickets: Ticket[]): { label: string; value: number; icon: string; color: string }[] {
    if (!this.tokenService.canOpenTickets() && !this.tokenService.canManageTickets()) {
      return [];
    }

    const authoredTickets = this.tokenService.canManageTickets()
      ? tickets
      : tickets.filter(ticket => (ticket.reporter?.userId || '').toLowerCase() === this.currentUserId.toLowerCase());

    const approvalTickets = tickets.filter(ticket =>
      (ticket.approvalGates || []).some(gate =>
        (gate.approvers || []).some(approver =>
          (approver.userId || '').toLowerCase() === this.currentUserId.toLowerCase()
        )
      )
    );

    const source = this.tokenService.canManageTickets() ? tickets : authoredTickets;
    const byStatus = {
      new: source.filter(ticket => this.normalizeTicketStatus(ticket.currentStateId) === 'new').length,
      progress: source.filter(ticket => this.normalizeTicketStatus(ticket.currentStateId) === 'in-progress').length,
      closed: source.filter(ticket => this.normalizeTicketStatus(ticket.currentStateId) === 'closed').length
    };

    return [
      {
        label: this.tokenService.canManageTickets() ? 'Tickets in system' : 'My tickets',
        value: source.length,
        icon: 'confirmation_number',
        color: 'blue'
      },
      {
        label: 'New',
        value: byStatus.new,
        icon: 'fiber_new',
        color: 'yellow'
      },
      {
        label: 'In progress',
        value: byStatus.progress,
        icon: 'pending_actions',
        color: 'orange'
      },
      {
        label: 'Closed',
        value: byStatus.closed,
        icon: 'task_alt',
        color: 'green'
      },
      ...(approvalTickets.length > 0 ? [{
        label: 'For approval',
        value: approvalTickets.length,
        icon: 'fact_check',
        color: 'purple'
      }] : [])
    ];
  }

  private buildStatsCards(): StatCard[] {
    const cards: StatCard[] = [];

    if (this.tokenService.canManageAssets()) {
      cards.push({
        icon: 'devices',
        value: String(this.systemAssetCount),
        label: 'Assets in system',
        color: 'blue'
      });
    } else {
      cards.push({
        icon: 'devices',
        value: String(this.assignedAssetCount),
        label: 'Assets assigned to me',
        color: 'blue'
      });
    }

    cards.push(...this.ticketStats.map((stat) => ({
      icon: stat.icon,
      value: String(stat.value),
      label: stat.label,
      color: stat.color
    })));

    return cards;
  }

  private buildDashboardActions(): QuickAction[] {
    const actions: QuickAction[] = [];

    if (this.tokenService.canManageAssets()) {
      actions.push(
        {
          icon: 'devices',
          title: 'View Assets',
          description: 'Browse and manage all assets in the system',
          route: '/assets/view',
          color: 'blue',
          gradient: 'from-blue-500 to-blue-600'
        },
        {
          icon: 'tune',
          title: 'Asset Management',
          description: 'Edit asset types, definitions, and properties',
          route: '/assets/management',
          color: 'blue',
          gradient: 'from-blue-500 to-blue-600'
        }
      );
    }

    if (this.tokenService.canManageTickets()) {
      actions.push(
        {
          icon: 'confirmation_number',
          title: 'Tickets',
          description: 'See all tickets and manage workflow state',
          route: '/technical-support/tickets',
          color: 'purple',
          gradient: 'from-purple-500 to-purple-600'
        },
        {
          icon: 'schema',
          title: 'Ticket Types',
          description: 'Adjust ticket type structure and workflows',
          route: '/technical-support/ticket-types',
          color: 'purple',
          gradient: 'from-purple-500 to-purple-600'
        },
        {
          icon: 'support_agent',
          title: 'AI Support',
          description: 'Ask the internal support assistant',
          route: '/technical-support/ai-support',
          color: 'purple',
          gradient: 'from-purple-500 to-purple-600'
        }
      );
    } else if (this.tokenService.canOpenTickets()) {
      actions.push(
        {
          icon: 'confirmation_number',
          title: 'My Tickets',
          description: 'See the tickets you created or can approve',
          route: '/technical-support/tickets',
          color: 'purple',
          gradient: 'from-purple-500 to-purple-600'
        },
        {
          icon: 'support_agent',
          title: 'AI Support',
          description: 'Get technical guidance from the AI assistant',
          route: '/technical-support/ai-support',
          color: 'purple',
          gradient: 'from-purple-500 to-purple-600'
        }
      );
    }

    if (this.tokenService.canManageUsers()) {
      actions.push({
        icon: 'people',
        title: 'Manage Users',
        description: 'Create users, assign roles, and review access',
        route: '/administration/users',
        color: 'orange',
        gradient: 'from-orange-500 to-orange-600'
      });
    }

    actions.push({
      icon: 'person',
      title: 'My Profile',
      description: 'Review your profile and personal allocations',
      route: '/profile/view',
      color: 'green',
      gradient: 'from-green-500 to-green-600'
    });

    return actions;
  }

  get dashboardSummaryItems(): { label: string; value: string }[] {
    const items: { label: string; value: string }[] = [];

    if (this.tokenService.canManageAssets()) {
      items.push({ label: 'Assets in system', value: String(this.systemAssetCount) });
    } else {
      items.push({ label: 'Assets assigned to you', value: String(this.assignedAssetCount) });
    }

    if (this.tokenService.canManageTickets()) {
      items.push({ label: 'Tickets in system', value: String(this.ticketStats[0]?.value || 0) });
    } else if (this.tokenService.canOpenTickets()) {
      items.push({ label: 'Your tickets', value: String(this.ticketStats[0]?.value || 0) });
    }

    const approvalCount = this.ticketStats.find(stat => stat.label === 'For approval')?.value;
    if (approvalCount != undefined) {
      items.push({ label: 'Waiting for your review', value: String(approvalCount) });
    }

    if (this.tokenService.canManageUsers()) {
      items.push({ label: 'User management', value: 'Enabled' });
    }

    return items;
  }

  getCurrentGreeting(): string {
    const hour = new Date().getHours();
    if (hour < 12) return 'Good morning! Here\'s your overview';
    if (hour < 18) return 'Good afternoon! Here\'s what\'s happening';
    return 'Good evening! Here\'s your summary';
  }

  navigateTo(route: string): void {
    this.router.navigate([route]);
  }

  private normalizeTicketStatus(stateId: string): 'new' | 'in-progress' | 'closed' | null {
    const normalized = (stateId || '').trim().toLowerCase().replace(/\s+/g, '-').replace(/_+/g, '-');
    if (['new', 'open', 'state-new'].includes(normalized)) return 'new';
    if (['in-progress', 'state-in-progress', 'state-wip', 'wip', 'progress'].includes(normalized)) return 'in-progress';
    if (['closed', 'state-closed'].includes(normalized)) return 'closed';
    return null;
  }

  login(): void {
    this.authService.logIn();
  }

  getStarted(): void {
    if (this.isAuthenticated) {
      this.router.navigate(['/dashboard']);
    } else {
      this.authService.logIn();
    }
  }
}
