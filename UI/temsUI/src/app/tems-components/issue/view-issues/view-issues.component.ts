import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit } from '@angular/core';
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
}
