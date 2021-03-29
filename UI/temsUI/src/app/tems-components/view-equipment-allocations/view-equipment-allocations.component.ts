import { map } from 'rxjs/operators';
import { AllocationService } from 'src/app/services/allocation-service/allocation.service';
import { ViewAllocationSimplified } from './../../models/equipment/view-equipment-allocation.model';
import { IOption } from './../../models/option.model';
import { SnackService } from './../../services/snack/snack.service';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { PersonnelService } from './../../services/personnel-service/personnel.service';
import { DefinitionService } from './../../services/definition-service/definition.service';
import { RoomsService } from './../../services/rooms-service/rooms.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-view-equipment-allocations',
  templateUrl: './view-equipment-allocations.component.html',
  styleUrls: ['./view-equipment-allocations.component.scss']
})
export class ViewEquipmentAllocationsComponent extends TEMSComponent implements OnInit {

  equipments: IOption[] = [];
  rooms: IOption[] = [];
  definitions: IOption[] = [];
  personnel: IOption[] = [];
  
  allocations: ViewAllocationSimplified[] = [];
  loading = true;


  filterAllocationsFormGroup = new FormGroup({
    equipment: new FormControl(),
    rooms: new FormControl(),
    personnel: new FormControl(),
    definitions: new FormControl(),
  })

  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private definitionService: DefinitionService,
    private personnelService: PersonnelService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private allocationService: AllocationService
  ) { 
    super();
  }

  ngOnInit(): void {
  }

  filtersChanged(){
    // ugly, I know
    this.equipments = (this.filterAllocationsFormGroup.controls.equipment.value != undefined) 
      ? this.filterAllocationsFormGroup.controls.equipment.value.map(q => q.value)
      : [];

    this.rooms = (this.filterAllocationsFormGroup.controls.rooms.value != undefined)
      ? this.filterAllocationsFormGroup.controls.rooms.value.map(q => q.value)
      : [];

    this.definitions = (this.filterAllocationsFormGroup.controls.definitions.value != undefined)
      ? this.filterAllocationsFormGroup.controls.definitions.value.map(q => q.value)
      : [];

    this.personnel = (this.filterAllocationsFormGroup.controls.personnel.value != undefined)
      ? this.filterAllocationsFormGroup.controls.personnel.value.map(q => q.value)
      : [];
  }
}
