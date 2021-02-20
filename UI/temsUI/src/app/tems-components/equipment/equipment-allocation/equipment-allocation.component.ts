import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-equipment-allocation',
  templateUrl: './equipment-allocation.component.html',
  styleUrls: ['./equipment-allocation.component.scss']
})
export class EquipmentAllocationComponent implements OnInit {

  // There are 4 ways this component will get displayed.
  // 1) Without any input parameters being sent 
  // 2) With the equipment already defined
  // 3) With the room already defined
  // 4) With the personnel already defined

  @Input() equipment; // {id, value}
  @Input() room; // {id, value}
  @Input() personnel; // {id, value}

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

  }

  ngOnInit(): void {
    this.equipmentAutoCompleteOptions = this.equipmentService.getAllAutocompleteOptions();
    if(this.equipment != null)
      this.equipmentAlreadySelectedOptions = [ this.equipment ];
    
    this.selectedAllocateToType="room";
    this.allocatedToChipsInputLabel = 'Room identifier...';
    this.allocatedToAutoCompleteOptions = this.roomService.getAllAutocompleteOptions();
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
}
