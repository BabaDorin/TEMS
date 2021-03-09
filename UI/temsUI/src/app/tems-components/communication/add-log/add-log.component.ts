import { IOption } from './../../../models/option.model';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-add-log',
  templateUrl: './add-log.component.html',
  styleUrls: ['./add-log.component.scss']
})
export class AddLogComponent extends TEMSComponent implements OnInit {

  @Input() equipment: IOption[];
  @Input() room: IOption[];
  @Input() personnel: IOption[];

  @ViewChild('addresseesChips') addresseesChips: ChipsAutocompleteComponent;

  // Mainly used for toggling chipsInput's visibility
  adresseeChosen: boolean = false;
  
  selectedAddresseeType: string;
  selectAddressee: any;
  addresseeTypes = [
    { value: 'equipment', viewValue: 'Equipment' },
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  autoCompleteOptions = [];
  alreadySelectedOptions = [];

  chipsInputLabel = "";

  constructor(
    private formlyParserService: FormlyParserService,
    private equipmentservice: EquipmentService,
    private roomService: RoomsService,
    private logsService: LogsService,
    private personnelService: PersonnelService) {
    super();
  }

  ngOnInit(): void {
    let implicitAddressees;
    this.adresseeChosen = this.equipment != undefined || this.room != undefined || this.personnel != undefined;

    if (this.adresseeChosen) {
      if (this.equipment != undefined)
        implicitAddressees = { type: 'equipment', entities: this.equipment }

      if (this.room != undefined)
        implicitAddressees = { type: 'room', entities: this.room }

      if (this.personnel != undefined)
        implicitAddressees = { type: 'personnel', entities: this.personnel }

      this.selectedAddresseeType = implicitAddressees.type;

      this.subscriptions.push(this.logsService.getLogTypes()
        .subscribe(response => {
          this.formlyData.fields = this.formlyParserService.parseAddLog(response);
          this.onAddresseeSelection(implicitAddressees.entities);
        }))
    }
  }

  onAddresseeSelection(alreadySelected?: IOption[]) {

    super.unsubscribeFromAll();

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

    this.alreadySelectedOptions = (alreadySelected == undefined)
      ? []
      : alreadySelected;
    this.adresseeChosen = true;
  }

  onSubmit() {
    if(this.addresseesChips.options.length > 0){
      this.formlyData.model.log.addresseesType = this.selectedAddresseeType;
      this.formlyData.model.log.addressees = this.addresseesChips.options;

      console.log(this.formlyData.model);
    }
    else
    {
      alert('Provide at least one addressee.');
    }
  }
}
