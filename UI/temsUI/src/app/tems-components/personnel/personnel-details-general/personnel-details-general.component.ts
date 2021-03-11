import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { ViewPersonnel } from './../../../models/personnel/view-personnel.model';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';

@Component({
  selector: 'app-personnel-details-general',
  templateUrl: './personnel-details-general.component.html',
  styleUrls: ['./personnel-details-general.component.scss']
})
export class PersonnelDetailsGeneralComponent implements OnInit, OnChanges {

  @Input() personnel: ViewPersonnel;

  personnelProperties: Property[];
  edit: boolean = false;

  constructor() { 
  }

  ngOnInit(): void {
    
  }

  ngOnChanges(): void {
    if(this.personnel != undefined)
      this.personnelProperties = [
        { displayName: 'Name', value: this.personnel.name },
        { displayName: 'Position', value: "display them in a fancy way" },
        { displayName: 'Phone Number', value: this.personnel.phoneNumber },
        { displayName: 'Email', value: this.personnel.email },
        { displayName: 'Active equipment allocations', value: this.personnel.allocatedEquipments },
        { displayName: 'Active tickets', value: this.personnel.activeTickets },
        { displayName: 'Room supervisories', value: "Display them in a fancy way" },
      ]
  }
}
