import { IOption } from 'src/app/models/option.model';
import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { CreateIssueComponent } from './../../../issue/create-issue/create-issue.component';
import { IssuesService } from './../../../../services/issues-service/issues.service';
import { Component, OnInit, Input } from '@angular/core';
import { ViewIssue } from 'src/app/models/communication/issues/view-issue';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-equipment-details-issues',
  templateUrl: './equipment-details-issues.component.html',
  styleUrls: ['./equipment-details-issues.component.scss']
})
export class EquipmentDetailsIssuesComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  equipmentAlreadySelected : IOption;
  constructor() { }

  ngOnInit(): void {
    this.equipmentAlreadySelected = {
      value: this.equipment.id,
      label: this.equipment.temsIdOrSerialNumber,
    }
  }
}
