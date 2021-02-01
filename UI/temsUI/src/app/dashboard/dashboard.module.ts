import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Routes, RouterModule } from '@angular/router';
import { ChartsModule } from 'ng2-charts';
import { DashboardComponent } from './dashboard.component';
import { TotalVariablesComponent } from './dashboard-components/total-variables/total-variables.component';
import { YearlychartComponent } from './dashboard-components/yearlychart/yearlychart.component';
import { RecentTableComponent } from './dashboard-components/recent-table/recent-table.component';
import { RecentCommentsComponent } from './dashboard-components/recent-comments/recent-comments.component';

import { ChatListingComponent } from './dashboard-components/chat-listing/chat-listing.component';


const routes: Routes = [
  {
    path: '',
    data: {
      title: 'Dashboard',
      urls: [
        { title: 'Dashboard', url: '/dashboard' },
        { title: 'Dashboard' }
      ]
    },
    component: DashboardComponent
  }
];

@NgModule({
  imports: [FormsModule, CommonModule, RouterModule.forChild(routes), ChartsModule],
  declarations: [DashboardComponent, TotalVariablesComponent, YearlychartComponent, 
    RecentTableComponent, RecentCommentsComponent, ChatListingComponent]
})
export class DashboardModule {

}
