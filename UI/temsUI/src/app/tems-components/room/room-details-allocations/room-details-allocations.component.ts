import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Input, OnInit } from '@angular/core';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';

@Component({
  selector: 'app-room-details-allocations',
  templateUrl: './room-details-allocations.component.html',
  styleUrls: ['./room-details-allocations.component.scss']
})
export class RoomDetailsAllocationsComponent implements OnInit {

  @Input() room: ViewRoomSimplified;
  equipmentFilter: EquipmentFilter;

  constructor() { 
    this.equipmentFilter = new EquipmentFilter();
  }

  ngOnInit(): void {
    if(this.room == undefined)
      return;
    
    this.equipmentFilter.rooms = [this.room.id];
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
  }
}
