import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { FormlyModule } from '@ngx-formly/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { DateTimeDisplayModule } from './../../shared/date-time-display/date-time-display.module';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { ViewAnnouncementsComponent } from './../../tems-components/communication/view-announcements/view-announcements.component';
import { AddAnnouncementComponent } from './../../tems-components/announcement/add-announcement/add-announcement.component';
import { AnnouncementsListComponent } from './../../tems-components/announcement/announcements-list/announcements-list.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

@NgModule({
  declarations: [
    AnnouncementsListComponent,
    AddAnnouncementComponent,
    ViewAnnouncementsComponent
  ],
  imports: [
    CommonModule,
    RouterModule,
    MatIconModule,
    MatCardModule,
    MatButtonModule,
    DateTimeDisplayModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    FormlyModule
  ],
  exports: [
    AnnouncementsListComponent,
    AddAnnouncementComponent,
    ViewAnnouncementsComponent
  ]
})
export class AnnouncementModule { }
