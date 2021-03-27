import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { AddAllocation } from './../../../models/allocation/add-allocation.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { IOption } from 'src/app/models/option.model';
import { AllocationService } from 'src/app/services/allocation-service/allocation.service';

@Component({
  selector: 'app-equipment-allocation',
  templateUrl: './equipment-allocation.component.html',
  styleUrls: ['./equipment-allocation.component.scss']
})
export class EquipmentAllocationComponent extends TEMSComponent implements OnInit {

  // There are 4 ways this component will get displayed.
  // 1) Without any input parameters being sent 
  // 2) With the equipment already defined
  // 3) With the room already defined
  // 4) With the personnel already defined

  @Input() equipment: IOption[]; 
  @Input() room: IOption[];
  @Input() personnel: IOption[];

  @ViewChild('equipmentIdentifierChips') equipmentIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo') allocatedTo: ChipsAutocompleteComponent;

  equipmentAlreadySelectedOptions = [] as IOption[];

  allocatedToAlreadySelectedOptions = [] as IOption[]
  allocatedToChipsInputLabel = "Choose one...";

  // Alocate to [select]
  selectedAllocateToType: string;
  allocateToTypes = [
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];
  alocateeEndPoint;

  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService,
    private allocationService: AllocationService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.selectedAllocateToType="room";
    this.allocatedToChipsInputLabel = 'Room identifier...';
    this.alocateeEndPoint = this.roomService;

    if(this.equipment != undefined)
      this.equipmentAlreadySelectedOptions = this.equipment;
    
    if(this.room != undefined){
      this.allocatedToAlreadySelectedOptions = this.room;
      this.selectedAllocateToType = 'room';
      this.alocateeEndPoint = this.roomService;
    }

    if(this.personnel != undefined){
      this.allocatedToAlreadySelectedOptions = this.personnel;
      this.selectedAllocateToType = 'personnel';
      this.alocateeEndPoint = this.personnelService;
    }
  }

  onSelection(){
    switch (this.selectedAllocateToType) {
      case 'room':
        this.allocatedToChipsInputLabel = 'Room identifier...';
        this.alocateeEndPoint = this.roomService;
        break;
      case 'personnel':
        this.allocatedToChipsInputLabel = 'Name...';
        this.alocateeEndPoint = this.personnelService;
        break;
    }
  }

  submit(){
    if(this.equipmentIdentifierChips.options.length<1 || this.allocatedTo.options.length != 1)
      return;

    let addAllocation: AddAllocation = {
      equipments: this.equipmentIdentifierChips.options,
      allocateToType: this.selectedAllocateToType,
      allocateToId: this.allocatedTo.options[0].value,
    }

    console.log(addAllocation);

    this.subscriptions.push(this.allocationService.createAllocation(addAllocation)
      .subscribe(result => {
        console.log(result);
      }))
  }
}
