import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { AuthService } from '../../services/auth.service';
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

  quickActions: QuickAction[] = [
    {
      icon: 'devices',
      title: 'Asset Management',
      description: 'Track and manage all your technical assets',
      route: '/assets',
      color: 'blue',
      gradient: 'from-blue-500 to-blue-600'
    },
    {
      icon: 'confirmation_number',
      title: 'Support Tickets',
      description: 'Create and manage support requests',
      route: '/tickets',
      color: 'purple',
      gradient: 'from-purple-500 to-purple-600'
    },
    {
      icon: 'inventory_2',
      title: 'Inventory',
      description: 'Monitor equipment and supplies',
      route: '/inventory',
      color: 'green',
      gradient: 'from-green-500 to-green-600'
    },
    {
      icon: 'analytics',
      title: 'Reports',
      description: 'View insights and analytics',
      route: '/reports',
      color: 'orange',
      gradient: 'from-orange-500 to-orange-600'
    }
  ];

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
    {
      icon: 'devices',
      value: '1,234',
      label: 'Total Assets',
      trend: '+12%',
      color: 'blue'
    },
    {
      icon: 'pending_actions',
      value: '23',
      label: 'Active Tickets',
      trend: '-8%',
      color: 'purple'
    },
    {
      icon: 'inventory',
      value: '567',
      label: 'In Stock',
      trend: '+5%',
      color: 'green'
    },
    {
      icon: 'trending_up',
      value: '98%',
      label: 'Uptime',
      trend: '+2%',
      color: 'orange'
    }
  ];

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.authService.isAuthenticated$.subscribe(isAuth => {
      this.isAuthenticated = isAuth;
      if (isAuth) {
        this.userName = this.authService.getUserName();
      }
    });
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
