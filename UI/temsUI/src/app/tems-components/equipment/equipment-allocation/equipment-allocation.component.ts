import { TEMSComponent } from './../../../tems/tems.component';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { ViewEquipmentSimplified } from './../../../models/equipment/view-equipment-simplified.model';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { IOption } from 'src/app/models/option.model';

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

  @ViewChild('equipmentIdentifierChips', { static: false }) equipmentIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo', { static: false }) allocatedTo: ChipsAutocompleteComponent;

  // Equipment autocomplete chips:
  equipmentAutoCompleteOptions = [];
  equipmentAlreadySelectedOptions = [];
  equipmentChipsInputLabel = "Choose one or more equipments...";

  // AllocatedToAutocompleteChips
  allocatedToAutoCompleteOptions = [];
  allocatedToAlreadySelectedOptions = [];
  allocatedToChipsInputLabel = "Choose one...";
  allodatedToMaxSelections = 1;

  // Alocate to [select]
  allocateToTypes = [
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];
  selectedAllocateToType: string;

  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.equipmentService.getAllAutocompleteOptions()
      .subscribe(response => {
        this.equipmentAutoCompleteOptions = response;
      }))

    this.selectedAllocateToType="room";
    this.allocatedToChipsInputLabel = 'Room identifier...';
    this.allocatedToAutoCompleteOptions = this.roomService.getAllAutocompleteOptions();
    
    if(this.equipment != undefined)
      this.equipmentAlreadySelectedOptions = this.equipment;
    
    if(this.room != undefined){
      this.allocatedToAlreadySelectedOptions = this.room;
      this.selectedAllocateToType = 'room';
    }

    if(this.personnel != undefined){
      this.allocatedToAlreadySelectedOptions = this.personnel;
      this.selectedAllocateToType = 'personnel';
    }
  }

  onSelection(){
    switch (this.selectedAllocateToType) {
      case 'room':
        this.allocatedToChipsInputLabel = 'Room identifier...';
        this.allocatedToAutoCompleteOptions = this.roomService.getAllAutocompleteOptions();
        break;
      case 'personnel':
        this.allocatedToChipsInputLabel = 'Name...';
        this.allocatedToAutoCompleteOptions = this.personnelService.getAllAutocompleteOptions();
        break;
    }

    this.allocatedToAlreadySelectedOptions = [];
  }

  submit(){
    if(this.equipmentIdentifierChips.options.length<1 || this.allocatedTo.options.length != 1)
      return;

    let model = {
      equipment: this.equipmentIdentifierChips.options,
      allocateTo: {
        type: this.selectedAllocateToType,
        identifier: this.allocatedTo.options
      }
    }

  }
}
