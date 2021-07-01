import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { FormlyModule } from '@ngx-formly/core';
import { TranslateModule } from '@ngx-translate/core';
import { AddAnnouncementComponent } from './../../tems-components/announcement/add-announcement/add-announcement.component';
import { ViewAnnouncementsComponent } from './../../tems-components/communication/view-announcements/view-announcements.component';
import { AnnouncementsListModule } from './announcements-list/announcements-list.module';

@NgModule({
  declarations: [
    AddAnnouncementComponent,
    ViewAnnouncementsComponent
  ],
  imports: [
    CommonModule,
    // RouterModule,
    MatIconModule,
    // MatCardModule,
    MatButtonModule,
    // DateTimeDisplayModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    FormlyModule,
    AnnouncementsListModule
  ]
})
export class AnnouncementModule { }
