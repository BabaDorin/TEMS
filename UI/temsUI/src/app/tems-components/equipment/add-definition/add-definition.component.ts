import { AddDefinition } from './../../../models/equipment/add-definition.model';
import { MatDialogRef } from '@angular/material/dialog';
import { Component, OnInit } from '@angular/core';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { FormGroup } from '@angular/forms';

@Component({
  selector: 'app-add-definition',
  templateUrl: './add-definition.component.html',
  styleUrls: ['./add-definition.component.scss']
})
export class AddDefinitionComponent implements OnInit {

  constructor(
    private formlyParserService : FormlyParserService,
    private equipmentService: EquipmentService,
    public dialogRef?: MatDialogRef<AddDefinitionComponent>) {
  }

  private formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }

  ngOnInit(): void {
    let addDefinition = new AddDefinition();


    this.formlyData.model = new AddDefinition();
    // this.formlyData.fields = this.formlyParserService.AddDefinition(addDefinition);
    this.formlyData.fields = [];
    this.formlyData.isVisible = true;
  }


}
