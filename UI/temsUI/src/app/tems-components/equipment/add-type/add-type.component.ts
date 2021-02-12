import { ChipsAutocompleteComponent } from './../../../public/formly/chips-autocomplete/chips-autocomplete.component';
import { Type } from './../../../models/equipment/view-type.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { AddType } from './../../../models/equipment/add-type.model';
import { FormlyFieldConfig, FormlyFormOptions } from '@ngx-formly/core';
import { FormControl, FormGroup } from '@angular/forms';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { Component, ElementRef, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';


@Component({
  selector: 'app-add-type',
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent implements OnInit {
  
  formGroup = new FormGroup({
    typeName: new FormControl(),
  });

  @ViewChild('parentChips') parentChips: ChipsAutocompleteComponent;
  @ViewChild('propertyChips') propertyChips: ChipsAutocompleteComponent;

  constructor(
    private equipmentService: EquipmentService,
    public dialogRef?: MatDialogRef<AddTypeComponent>) {
  }

  labelSelectParentType = "Select parent type / types";
  labelSelectProperties = "Select type's properties";

  ngOnInit(): void {
  }

  onSubmit(){
    // find a way to allow only selecting only data that exists in the auto-complete dropwdown
    let type = {
      typeName: this.formGroup.get('typeName').value,
      parentTypes: this.parentChips.options,
      properties: this.propertyChips.options
    }
    console.log(type);
  }
}