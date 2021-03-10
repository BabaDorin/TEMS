import { TEMSComponent } from './../../../tems/tems.component';
import { CheckboxItem } from '../../../models/checkboxItem.model';
import { IOption } from 'src/app/models/option.model';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { FormGroup, FormControl } from '@angular/forms';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-create-report-template',
  templateUrl: './create-report-template.component.html',
  styleUrls: ['./create-report-template.component.scss']
})
export class CreateReportTemplateComponent extends TEMSComponent implements OnInit {

  reportFormGroup: FormGroup;
  sepparateBy: string;
  equipmentCommonProperties: CheckboxItem[];
  typeSpecificProperties: { type: IOption, properties: CheckboxItem[] }[] = [];

  reportObjectOptions = [
    { value: 'equipment', viewValue: 'Equipment' },
    { value: 'rooms', viewValue: 'Rooms' },
    { value: 'Personnel', viewValue: 'Personnel' },
    { value: 'Allocations', viewValue: 'Allocations' }
  ];

  typesAutocompleteOptions: IOption[];
  definitionsAutocompleteOptions: IOption[];
  roomsAutocompleteOptions: IOption[];
  personnelAutocompleteOptions: IOption[];

  constructor(
    private roomService: RoomsService,
    private equipmentService: EquipmentService,
    private personnelService: PersonnelService,
  ) { 
    super();
  }

  ngOnInit(): void {
    this.reportFormGroup = new FormGroup({
      name: new FormControl(),
      description: new FormControl(),
      subject: new FormControl(),
      types: new FormControl(),
      definitions: new FormControl(),
      rooms: new FormControl(),
      personnel: new FormControl(),
      sepparateBy: new FormControl(),
      commonProperties: new FormControl(),
      specificProperties: new FormControl(),
      header: new FormControl(),
      footer: new FormControl(),
      signatories: new FormControl()
    });

    this.typesAutocompleteOptions = this.equipmentService.getTypesAutocomplete();
    this.definitionsAutocompleteOptions = this.equipmentService.getDefinitionsAutocomplete(
      this.reportFormGroup.controls.types.value);
    
    this.subscriptions.push(this.roomService.getAllAutocompleteOptions()
      .subscribe(response => {
        this.roomsAutocompleteOptions = response;
      }));
    
      this.personnelAutocompleteOptions = this.personnelService.getAllAutocompleteOptions();
    this.sepparateBy = 'none';
    this.equipmentCommonProperties = this.equipmentService.getCommonProperties();
    this.equipmentCommonProperties.map(q => q.checked = true);
  }
  submit() {
    console.log(this.reportFormGroup);
  }

  typeAdded(eventData) {
    // getting definitions for selected types
    this.definitionsAutocompleteOptions = this.equipmentService.getDefinitionsAutocomplete(eventData);

    // Getting specific properties of selected types
    this.reportFormGroup.controls.types.value.forEach(element => {
      if (this.typeSpecificProperties.find(q => q.type == element) == undefined) {
        this.typeSpecificProperties.push(
          {
            type: element,
            properties: this.equipmentService.getTypeSpecificProperties(element)
          });
      }
    });
  }

  typeRemoved(eventData) {
    // this.reportFormGroup.controls.types.value.remove(q => q.type == eventData)
    // const index = this.reportFormGroup.controls.types.value.indexOf(q => q.type == eventData);
    // if (index > -1) {
    //   this.reportFormGroup.controls.types.value.splice(index, 1);
    // }

    const index = this.typeSpecificProperties.findIndex(q => q.type == eventData);
    if(index> -1){
      this.typeSpecificProperties.splice(index,1);
    }
    // console.log(this.reportFormGroup.controls.types.value);
  }

  onCommonPropChange(eventData) {
    this.reportFormGroup.controls.commonProperties.setValue(eventData);
  }

  specificProperties: { type: IOption, properties: CheckboxItem[] }[] = [];
  onSpecificPropChange(eventData, type) {
    let typeSpecific = this.specificProperties.find(q => q.type == type);

    if (typeSpecific != undefined) {
      typeSpecific.properties = eventData;
    }
    else {
      this.specificProperties.push({ type: type, properties: eventData });
    }

    this.reportFormGroup.controls.specificProperties.setValue(this.specificProperties);
  }
}
