import { IOption } from './../../../models/option.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { FormControl, FormGroup } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { MatDialogRef } from '@angular/material/dialog';


@Component({
  selector: 'app-add-type',
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent implements OnInit {
  
  formGroup = new FormGroup({
    parents: new FormControl(),
    typeName: new FormControl(),
    properties: new FormControl(),
  });

  constructor(
    private equipmentService: EquipmentService,
    public dialogRef?: MatDialogRef<AddTypeComponent>) {
  }

  parentTypeOptions: IOption[]; 
  propertyOptions: IOption[];

  ngOnInit(): void {
    this.parentTypeOptions = this.equipmentService.getTypes();
    this.propertyOptions = this.equipmentService.getProperties().map(q => ({id: q.id, value: q.displayName}))
  }

  onSubmit(){
    // Send to API
    console.log(this.formGroup);
  }
}