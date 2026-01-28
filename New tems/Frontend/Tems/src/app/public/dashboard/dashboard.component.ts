import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { MATERIAL_MODULES } from 'src/app/shared/constants/material-modules.const';
import { QuickAccessComponent } from 'src/app/tems-components/asset/quick-access/quick-access.component';
import { AnnouncementsListComponent } from 'src/app/tems-components/announcement/announcements-list/announcements-list.component';
import { UserCardsListComponent } from 'src/app/tems-components/profile/user-cards-list/user-cards-list.component';
import { LastCreatedTicketsChartComponent } from 'src/app/tems-components/analytics/last-created-tickets-chart/last-created-tickets-chart.component';
import { LastIssuesSimplifiedComponent } from 'src/app/tems-components/analytics/last-issues-simplified/last-issues-simplified.component';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule, 
    RouterModule, 
    TranslateModule, 
    ...MATERIAL_MODULES, 
    QuickAccessComponent, 
    AnnouncementsListComponent, 
    UserCardsListComponent, 
    LastCreatedTicketsChartComponent, 
    LastIssuesSimplifiedComponent
  ],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  claims: any = { canView: false, canManage: false };
  
  toggleProBanner(event: Event) {
    event.preventDefault();
    document.querySelector('body')?.classList.toggle('removeProbanner');
  }

  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    const identityClaims = this.authService.getIdentityClaims();
    if (identityClaims) {
      this.claims = {
        canView: identityClaims.can_manage_assets === 'true',
        canManage: identityClaims.can_manage_assets === 'true'
      };
    }
  }
}
