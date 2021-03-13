import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { IOption } from 'src/app/models/option.model';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';

@Component({
  selector: 'app-view-issues',
  templateUrl: './view-issues.component.html',
  styleUrls: ['./view-issues.component.scss']
})
export class ViewIssuesComponent extends TEMSComponent implements OnInit {

  equipment: Observable<IOption[]>;
  equipmentId: string = "any";

  rooms: Observable<IOption[]>;
  roomId: string = "any";
  
  personnel: Observable<IOption[]>;
  personnelId: string = "any";


  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.equipmentService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log('keysAutocomplete')
        console.log(result);
        this.equipment = of(result);
      }));

    this.subscriptions.push(this.roomService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log('rooms autocomplete')
        console.log(result);
        this.rooms = of(result);
      }));

    this.subscriptions.push(this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log('personnel')
        console.log(result);
        this.personnel = of(result);
      }));
  }

  equipmentSelected(value){
    this.equipmentId = value;
  }

  roomSelected(value){
    this.roomId = value;
  }

  personnelSelected(value){
    this.personnelId = value;
  }
}
