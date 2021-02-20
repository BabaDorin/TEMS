import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-add-log',
  templateUrl: './add-log.component.html',
  styleUrls: ['./add-log.component.scss']
})
export class AddLogComponent implements OnInit {

  // If one of the inputs is not null, it means that 
  // the equipment / personnel / room is alreay defined for the 
  // log that is being created, it means that choosing an equipment, room or persoonel
  // for this specific log is unavailable.

  // Otherwise, the user will have to choose to whom the log he is creating is addressed.
  // (Personnel, Room, Equipment).

  @Input() equipmentId: string;
  @Input() roomId: string;
  @Input() personnelId: string;
  adresseeChosen: boolean = false;

  @ViewChild('identifierChips', { static: false }) identifierChips: ChipsAutocompleteComponent;
  
  selectedAddresseeType: string;

  addresseeTypes = [
    {value: 'equipment', viewValue: 'Equipment'},
    {value: 'room', viewValue: 'Room'},
    {value: 'personnel', viewValue: 'Personnel'}
  ];

  chipsInputLabel = "Identifier...";
  autoCompleteOptions = [];
  alreadySelectedOptions = [];

  constructor(
    private equipmentservice: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService) {
  }

  ngOnInit(): void { 
    this.adresseeChosen = this.equipmentId != undefined || this.roomId != undefined || this.personnelId != undefined;
  }
  
  onFoodSelection1() {
    switch(this.selectedAddresseeType){
      case 'equipment': 
        this.chipsInputLabel = 'TEMSID or Serial Number...'; 
        this.autoCompleteOptions = this.equipmentservice.getAllAutocompleteOptions();
        break;
      case 'room': 
        this.chipsInputLabel = 'Room identifier...'; 
        this.autoCompleteOptions = this.roomService.getAllAutocompleteOptions();
        break;
      case 'personnel': 
        this.chipsInputLabel = 'Name...'; 
        this.autoCompleteOptions = this.personnelService.getAllAutocompleteOptions();
        break;
    }

    this.alreadySelectedOptions = [];
    this.adresseeChosen = true;
  }
}
