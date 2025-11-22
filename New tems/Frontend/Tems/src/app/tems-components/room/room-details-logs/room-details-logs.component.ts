import { Component, Input, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-room-details-logs',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  schemas: [NO_ERRORS_SCHEMA],
  templateUrl: './room-details-logs.component.html',
  styleUrls: ['./room-details-logs.component.scss']
})
export class RoomDetailsLogsComponent implements OnInit {
  @Input() room: any;
  roomOption: any;

  ngOnInit(): void {
    if (this.room) {
      this.roomOption = { value: this.room.id, label: this.room.name };
    }
  }
}
