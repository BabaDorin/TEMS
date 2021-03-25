import { Observable, of } from 'rxjs';
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
  universalProperties: CheckboxItem[];
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

    this.fetchTypes();
    this.fetchDefinitionsOfTypes();
    this.fetchRooms();
    this.fetchPersonnel();

    this.sepparateBy = 'none';
    this.universalProperties = [
      new CheckboxItem('temsid', 'TEMSID'),
      new CheckboxItem('identifier', 'Identifier'),
      new CheckboxItem('serialNumber', 'Serial Number'),
      new CheckboxItem('description', 'Description'),
      new CheckboxItem('price', 'Price'),
    ];
    this.equipmentCommonProperties = this.universalProperties;
    this.equipmentCommonProperties.map(q => q.checked = true);
  }

  fetchTypes() {
    this.subscriptions.push(
      this.equipmentService.getTypes()
        .subscribe(result => {
          this.typesAutocompleteOptions = result;
        })
    );
  }

  fetchDefinitionsOfTypes() {
    let types: string[] = this.reportFormGroup.controls.types.value == undefined
      ? null
      : this.reportFormGroup.controls.types.value.map(q => q.value);

    this.subscriptions.push(
      this.equipmentService
        .getDefinitionsOfTypes(types)
        .subscribe(result => {
          console.log(result);
          this.definitionsAutocompleteOptions = result;
        })
    )
  }

  fetchRooms() {
    this.subscriptions.push(
      this.roomService.getAllAutocompleteOptions()
        .subscribe(result => {
          console.log(result);
          this.roomsAutocompleteOptions = result;
        }));
  }

  fetchPersonnel() {
    this.subscriptions.push(
      this.personnelService.getAllAutocompleteOptions()
        .subscribe(result => {
          console.log(result);
          this.personnelAutocompleteOptions = result;
        }));
  }

  typeAdded(eventData) {
    // getting definitions for selected types
    this.fetchDefinitionsOfTypes();
    this.findCommonAndSpecificPropertied();
  }

  typeRemoved(eventData) {
    this.fetchDefinitionsOfTypes();

    const index = this.typeSpecificProperties.findIndex(q => q.type == eventData);
    if (index > -1) {
      this.typeSpecificProperties.splice(index, 1);
    }

    this.putBackCommonProps();
    this.findCommonProps();
  }

  onCommonPropChange(eventData) {
    this.reportFormGroup.controls.commonProperties.setValue(eventData);
  }

  findCommonAndSpecificPropertied(){
    // 1. Put all common props back to where they belong
    // 2. Add or remove type specific properties
    // 3. Find common properties

    this.putBackCommonProps();

    // Getting specific properties of selected types
    let length = this.reportFormGroup.controls.types.value.length;
    for(let i = 0; i < length; i++){
      let element = this.reportFormGroup.controls.types.value[i];
      if (this.typeSpecificProperties.find(q => q.type == element) == undefined){
        this.subscriptions.push(
          this.equipmentService.getPropertiesOfType(element.value)
            .subscribe(result => {
              this.typeSpecificProperties.push(
              {
                type: element,
                properties: result.map(q => new CheckboxItem(q.additional, q.label))
              });
  
              this.findCommonProps();
            }
          )
        )
      }
    }
  }

  putBackCommonProps(){
    this.equipmentCommonProperties = this.equipmentCommonProperties
    .filter((el) => !this.universalProperties.map(q => q.label).includes(el.label));

    this.typeSpecificProperties.forEach(specific => {
      this.equipmentCommonProperties.forEach(common => {
        specific.properties.push(new CheckboxItem(common.value, common.label, common.checked))
      });
    });
  }

  findCommonProps(){
    this.equipmentCommonProperties = this.universalProperties;
    if (this.typeSpecificProperties.length < 2)
      return;

    let localCommonProperties = this.typeSpecificProperties[0].properties;

    for (let i = 1; i < this.typeSpecificProperties.length; i++) {
      console.log(localCommonProperties);
      console.log('and')
      console.log(this.typeSpecificProperties[i].properties);

      localCommonProperties = localCommonProperties
        .filter(value => this.typeSpecificProperties[i].properties
          .map(q => q.label)
          .includes(value.label));
    }

    console.log('found ' + localCommonProperties.length + ' common properties');

    this.typeSpecificProperties.forEach(element => {
      element.properties = element.properties
        .filter((value) => !localCommonProperties
          .map(q => q.label)
          .includes(value.label));
    });

    this.equipmentCommonProperties = this.equipmentCommonProperties.concat(localCommonProperties);
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

  submit() {
    console.log(this.reportFormGroup);
  }
}
