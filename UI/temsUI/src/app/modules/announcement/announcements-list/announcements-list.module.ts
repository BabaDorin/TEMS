import { RouterModule } from '@angular/router';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { AnnouncementsListComponent } from './../../../tems-components/announcement/announcements-list/announcements-list.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';



@NgModule({
  declarations: [
    AnnouncementsListComponent
  ],
  imports: [
    CommonModule,
    MatCardModule,
    MatIconModule,
    RouterModule,
    MatButtonModule
  ],
  exports:[
    AnnouncementsListComponent
  ]
})
export class AnnouncementsListModule { }
