import { Component, Input, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-room-details-issues',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  schemas: [NO_ERRORS_SCHEMA],
  templateUrl: './room-details-issues.component.html',
  styleUrls: ['./room-details-issues.component.scss']
})
export class RoomDetailsIssuesComponent implements OnInit {
  @Input() room: any;
  roomAlreadySelected: any;

  ngOnInit(): void {
    if (this.room) {
      this.roomAlreadySelected = { value: this.room.id, label: this.room.name };
    }
  }
}
