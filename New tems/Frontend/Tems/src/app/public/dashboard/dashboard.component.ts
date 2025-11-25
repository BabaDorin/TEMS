import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { MATERIAL_MODULES } from 'src/app/modules/material/material.module';
import { QuickAccessComponent } from 'src/app/tems-components/equipment/quick-access/quick-access.component';
import { AnnouncementsListComponent } from 'src/app/tems-components/announcement/announcements-list/announcements-list.component';
import { UserCardsListComponent } from 'src/app/tems-components/profile/user-cards-list/user-cards-list.component';
import { LastCreatedTicketsChartComponent } from 'src/app/tems-components/analytics/last-created-tickets-chart/last-created-tickets-chart.component';
import { LastIssuesSimplifiedComponent } from 'src/app/tems-components/analytics/last-issues-simplified/last-issues-simplified.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule, ...MATERIAL_MODULES, QuickAccessComponent, AnnouncementsListComponent, UserCardsListComponent, LastCreatedTicketsChartComponent, LastIssuesSimplifiedComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {
  claims: any;
  
  toggleProBanner(event) {
    event.preventDefault();
    document.querySelector('body').classList.toggle('removeProbanner');
  }

  constructor() {}

  ngOnInit(): void {}
}
