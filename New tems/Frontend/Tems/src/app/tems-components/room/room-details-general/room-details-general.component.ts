import { Component, EventEmitter, Inject, Input, OnInit, Optional, Output, OnDestroy } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { ConfirmService } from 'src/app/confirm.service';
import { Property } from 'src/app/models/asset/view-property.model';
import { RoomsService } from 'src/app/services/rooms.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { TokenService } from '../../../services/token.service';
import { ViewRoom } from './../../../models/room/view-room.model';
import { AddRoomComponent } from './../add-room/add-room.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { PropertyRenderComponent } from 'src/app/public/property-render/property-render.component';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Component({
  selector: 'app-room-details-general',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatProgressBarModule,
    RouterModule,
    TranslateModule,
    PropertyRenderComponent
  ],
  templateUrl: './room-details-general.component.html',
  styleUrls: ['./room-details-general.component.scss']
})
export class RoomDetailsGeneralComponent extends TEMSComponent implements OnInit, OnDestroy {

  @Input() roomId: string;
  @Output() archivationStatusChanged = new EventEmitter<void>();

  headerClass: string = '';
  canManage: boolean = false;
  displayViewMore: boolean = false;
  room: any;
  roomProperties: Property[] = [];
  subscriptions: any[] = [];

  protected destroy$ = new Subject<void>();

  constructor(
    private roomService: RoomsService,
    private route: Router,
    private dialogService: DialogService,
    private snackService: SnackService,
    private tokenService: TokenService,
    private confirmService: ConfirmService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    if (this.dialogData != undefined) {
      this.roomId = this.dialogData.roomId;
    }
  }

  ngOnInit(): void {
    this.fetchRoom();
  }

  fetchRoom(): void {
    this.subscriptions.push(
      this.roomService.getRoomById(this.roomId)
        .pipe(takeUntil(this.destroy$))
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.room = result;
          this.roomProperties = [
            { displayName: 'Identifier', value: this.room.identifier } as Property,
            { displayName: 'Description', value: this.room.description } as Property,
            { displayName: 'Floor', value: this.room.floor } as Property,
            { displayName: 'Active issues', value: this.room.activeTickets } as Property,
          ];
        })
    );
  }

  async archieve(): Promise<void> {
    if (!this.room.isArchieved && !await this.confirmService.confirm("Are you sure you want to archieve this room? Allocations and logs associated with this room will get archieved as well."))
      return;

    let newArchivationStatus = !this.room.isArchieved;
    this.subscriptions.push(
      this.roomService.archieveRoom(this.roomId, newArchivationStatus)
        .pipe(takeUntil(this.destroy$))
        .subscribe(result => {
          this.snackService.snack(result);

          if (result.status == 1)
            this.room.isArchieved = newArchivationStatus;

          this.archivationStatusChanged.emit(this.room.isArchieved);
        })
    );
  }

  edit(): void {
    this.dialogService.openDialog(
      AddRoomComponent,
      [{ label: "roomId", value: this.roomId }],
      () => {
        this.fetchRoom();
      }
    );
  }

  viewMore(): void {
    this.displayViewMore = !this.displayViewMore;
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
  }
}
