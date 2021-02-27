import { CheckboxItem } from '../../../models/checkboxItem.model';
import { IOption } from 'src/app/models/option.model';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { FormGroup, FormControl } from '@angular/forms';
import { flow } from 'lodash';
import { Component, OnInit } from '@angular/core';
import { filter, flatMap } from 'rxjs/operators';
import { Type } from 'src/app/models/equipment/view-type.model';

@Component({
  selector: 'app-create-report-template',
  templateUrl: './create-report-template.component.html',
  styleUrls: ['./create-report-template.component.scss']
})
export class CreateReportTemplateComponent implements OnInit {

  reportFormGroup: FormGroup;
  sepparateBy: string;
  equipmentCommonProperties: CheckboxItem[];
  typeSpecificProperties: { type: IOption, properties: CheckboxItem[] }[] = [];

  reportObjectOptions = [
    {value: 'equipment', viewValue: 'Equipment'},
    {value: 'rooms', viewValue: 'Rooms'},
    {value: 'Personnel', viewValue: 'Personnel'},
    {value: 'Allocations', viewValue: 'Allocations'}
  ];

  typesAutocompleteOptions: IOption[];
  definitionsAutocompleteOptions: IOption[];
  roomsAutocompleteOptions: IOption[];
  personnelAutocompleteOptions: IOption[];

  constructor(
    private roomService: RoomsService,
    private equipmentService: EquipmentService,
    private personnelService: PersonnelService,
  ) { }

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
    });

    this.typesAutocompleteOptions = this.equipmentService.getTypesAutocomplete();
    this.definitionsAutocompleteOptions = this.equipmentService.getDefinitionsAutocomplete(
      this.reportFormGroup.controls.types.value);
    this.roomsAutocompleteOptions = this.roomService.getAllAutocompleteOptions();
    this.personnelAutocompleteOptions = this.personnelService.getAllAutocompleteOptions();
    this.sepparateBy = 'none';
    this.equipmentCommonProperties = this.equipmentService.getCommonProperties();
  }
  submit(){
    console.log(this.reportFormGroup);
  }

  typeChanged(eventData){
    this.definitionsAutocompleteOptions = this.equipmentService.getDefinitionsAutocomplete(eventData);
    this.reportFormGroup.controls.types.value.forEach(element => {
      if(this.typeSpecificProperties.find(q => q.type == element) == undefined){
        this.typeSpecificProperties.push(
          {
          type: element, 
          properties: this.equipmentService.getTypeSpecificProperties(element)
          });
      }
    });
  }

  onCommonPropChange(eventData){
    this.reportFormGroup.controls.commonProperties.setValue(eventData);
  }
}
