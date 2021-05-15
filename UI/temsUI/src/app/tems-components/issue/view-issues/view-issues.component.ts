import { PinnedIssuesComponent } from './../pinned-issues/pinned-issues.component';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { EntityIssuesListComponent } from './../../entity-issues-list/entity-issues-list.component';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IOption } from 'src/app/models/option.model';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-view-issues',
  templateUrl: './view-issues.component.html',
  styleUrls: ['./view-issues.component.scss']
})
export class ViewIssuesComponent extends TEMSComponent implements OnInit {
  // How pinning / unPinning ticket works:
  // 1) When a ticket is pinned, entityIssuesList will emmit it via output event and will remove from it's
  // list of issues.
  // 2) On initialization, PinnedTicketComponent will search if there is any pinned ticket. If so, it will be displayed.
  // 3)  


  // equipment: Observable<IOption[]>;
  equipmentId: string = "any";

  // rooms: Observable<IOption[]>;
  roomId: string = "any";
  
  // personnel: Observable<IOption[]>;
  personnelAlreadySelected=[] as IOption[];
  personnelId: string = "any";

  filterIssueFormGroup = new FormGroup({
    equipment: new FormControl(),
    rooms: new FormControl(),
    personnel: new FormControl(),
  })

  @ViewChild('pinnedIssues') pinnedIssues: PinnedIssuesComponent;
  @ViewChild('entityOpenIssuesList') entityOpenIssuesList: EntityIssuesListComponent;
  @ViewChild('entityClosedIssuesList') entityClosedIssuesList: EntityIssuesListComponent;

  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit(): void {

  }

  equipmentSelected(idk){
    let value = this.filterIssueFormGroup.controls.equipment.value;

    if(value[0] != undefined)
      this.equipmentId = value[0].value;
    else
      this.equipmentId = 'any';
  }

  roomSelected(idk){
    let value = this.filterIssueFormGroup.controls.rooms.value;

    if(value[0] != undefined)
      this.roomId = value[0].value;
    else
      this.roomId = "any";
    console.log(this.roomId);
  }

  personnelSelected(idk){
    let value = this.filterIssueFormGroup.controls.personnel.value;

    if(value[0] != undefined)
      this.personnelId = value[0].value;
    else
      this.personnelId = 'any';
  }

  // After unpin, the ticket is being included in the common ticket list
  ticketUnpinned(ticket: ViewIssueSimplified){
    if(ticket.dateClosed == undefined){
      this.entityOpenIssuesList.issues.push(ticket);
      return;
    };

    if(this.entityClosedIssuesList != undefined)
      this.entityClosedIssuesList.issues.push(ticket);
  }

  ticketPinned(ticket: ViewIssueSimplified){
    this.pinnedIssues.issues.push(ticket);
  }
}
