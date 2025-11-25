import { NgxPaginationModule } from 'ngx-pagination';
import { TEMS_FORMS_IMPORTS } from './../tems-forms/tems-forms.module';
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
  declarations: [],
  imports: [
    CommonModule,
    AddAnnouncementComponent,
    ViewAnnouncementsComponent,
    // RouterModule,
    MatIconModule,
    // MatCardModule,
    MatButtonModule,
    // DateTimeDisplayModule,
    TranslateModule,
    ReactiveFormsModule,
    FormsModule,
    ...TEMS_FORMS_IMPORTS,
    AnnouncementsListModule,
  ]
})
export class AnnouncementModule { }
