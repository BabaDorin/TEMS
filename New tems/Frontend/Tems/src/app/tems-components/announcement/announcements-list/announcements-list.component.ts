import { SnackService } from './../../../services/snack.service';
import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { ViewAnnouncement } from '../../../models/communication/announcement/view-announcement.model';
import { CommunicationService } from '../../../services/communication.service';
import { DialogService } from '../../../services/dialog.service';
import { ClaimService } from './../../../services/claim.service';
import { AddAnnouncementComponent } from './../add-announcement/add-announcement.component';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { MatMenuModule } from '@angular/material/menu';
import { DateTimeDisplayModule } from '../../../shared/date-time-display/date-time-display.module';

@Component({
  selector: 'app-announcements-list',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatCardModule,
    MatIconModule,
    TranslateModule,
    RouterModule,
    NgxPaginationModule,
    MatMenuModule,
    DateTimeDisplayModule
  ],
  templateUrl: './announcements-list.component.html',
  styleUrls: ['./announcements-list.component.scss']
})
export class AnnouncementsListComponent extends TEMSComponent implements OnInit {

  @Input() skip: number;
  @Input() take: number;
  @Input() displayCreateNew: boolean = false;

  announcements: ViewAnnouncement[];
  itemsPerPage = 10;
  pageNumber = 1;

  constructor(
    private communicationService: CommunicationService,
    public dialog: MatDialog,
    public claims: ClaimService,
    private dialogService: DialogService,
    private snackService: SnackService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.fetchAnnouncements();
  }

  fetchAnnouncements(){
    let endPoint = 
    (this.skip != undefined && this.take != undefined)
    ? this.communicationService.getAnnouncements(this.skip, this.take)
    : this.communicationService.getAnnouncements();

  this.subscriptions.push(
    endPoint
    .subscribe(result => {
      this.announcements = result;
    }));
  }

  addAnnouncement(){
    this.dialogService.openDialog(
      AddAnnouncementComponent,
      undefined,
      () => {
        this.fetchAnnouncements();
      }
    );
  }

  remove(index){
    this.subscriptions.push(
      this.communicationService.removeAnnouncement(this.announcements[index].id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        if(result?.status == 1)
          this.announcements.splice(index, 1);
      })
    )
  }
}
