import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-add-log',
  templateUrl: './add-log.component.html',
  styleUrls: ['./add-log.component.scss']
})
export class AddLogComponent extends TEMSComponent implements OnInit {

  // If one of the inputs is not null, it means that 
  // the equipment / personnel / room is alreay defined for the 
  // log that is being created, it means that choosing an equipment, room or persoonel
  // for this specific log is unavailable.

  // Otherwise, the user will have to choose to whom the log he is creating is addressed.
  // (Personnel, Room, Equipment).

  @Input() equipment: IOption[];
  @Input() room: IOption[];
  @Input() personnel: IOption[];
  @ViewChild('identifierChips', { static: false }) identifierChips: ChipsAutocompleteComponent;

  adresseeChosen: boolean = false;
  implicitAddressees; // the addresser that comes from outside
  selectedAddresseeType: string;
  selectAddressee: any;
  addresseeTypes = [
    { value: 'equipment', viewValue: 'Equipment' },
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  autoCompleteOptions = [];
  alreadySelectedOptions = [];

  chipsInputLabel = "";

  constructor(
    formlyParserService: FormlyParserService,
    private equipmentservice: EquipmentService,
    private roomService: RoomsService,
    private personnelService: PersonnelService) {
      super();
      this.formlyData.isVisible = true;
      this.formlyData.fields = formlyParserService.parseAddLog(new ViewLog());
  }

  ngOnInit(): void {
    this.adresseeChosen = this.equipment != undefined || this.room != undefined || this.personnel != undefined;

    // 
    if(this.adresseeChosen){
      if(this.equipment != undefined)
        this.implicitAddressees = { type: 'equipment', entities: this.equipment }
      
      if(this.room != undefined)
        this.implicitAddressees = { type: 'room', entities: this.room }

      if(this.personnel != undefined)
        this.implicitAddressees = { type: 'personnel', entities: this.personnel }

        this.selectedAddresseeType = this.implicitAddressees.type;
        this.alreadySelectedOptions = [ { value: this.implicitAddressees.entities[0].value } ];
    }
  }

  onAddresseeSelection() {
    // If we already have an andressee from outide, it won't react to any selections
    if(this.implicitAddressees != undefined)
      return;

    switch (this.selectedAddresseeType) {
      case 'equipment':
        this.chipsInputLabel = 'TEMSID or Serial Number...';
        this.subscriptions.push(this.equipmentservice.getAllAutocompleteOptions()
          .subscribe(response => {
            this.autoCompleteOptions = response;
          }))
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

  onSubmit(model) {
    // Primitive validation, might improve later.
    if(model.log.logType){
      model.selectedAddresseeType = this.selectedAddresseeType;

      // case 1: The Addresse comes from outside
      if(this.implicitAddressees != undefined)
      {
        model.log.addreesses = this.implicitAddressees;
      }
      // case 2: Adressees are indicated here
      else if(this.selectedAddresseeType != undefined && this.identifierChips.options.length > 0){
        model.log.addreesses = {
          type: this.selectedAddresseeType,
          entities: this.identifierChips.options
        };
      }

      console.log(model);
    }
  }
}
