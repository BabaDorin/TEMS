import { Type } from './../../../models/equipment/view-type.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { AddType } from './../../../models/equipment/add-type.model';
import { FormlyFieldConfig, FormlyFormOptions } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';
import { FormlyParserService } from './../../../services/formly-parser-service/formly-parser.service';
import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';

@Component({
  selector: 'app-add-type',
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})
export class AddTypeComponent implements OnInit {

  constructor(
    private formlyParserService : FormlyParserService,
    private equipmentService: EquipmentService,
    public dialogRef?: MatDialogRef<AddTypeComponent>) {
  }

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  ngOnInit(): void {
    let addType = new AddType();
    this.equipmentService.getTypes().forEach(type => {
      addType.parents.push({
        id: type.id,
        name: type.name,
        parents: [],
        properties: [],
      })
    });
    
    this.formlyData.form = new FormGroup({});
    this.formlyData.model = new AddType();
    this.formlyData.fields = this.formlyParserService.parseAddType(addType);
    this.formlyData.isVisible = true;
  }

  onSubmit(model){
    console.log(model);
    this.dialogRef.close();
  }

  

  submit() {
    // alert(JSON.stringify(this.model));
  }
}