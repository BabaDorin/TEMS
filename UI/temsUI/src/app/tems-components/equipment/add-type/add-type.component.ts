import { IOption } from './../../../models/option.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ComponentType } from '@angular/cdk/portal';
import { AddPropertyComponent } from '../add-property/add-property.component';


@Component({
  selector: 'app-add-type',
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent implements OnInit {
  
  formGroup = new FormGroup({
    parents: new FormControl(),
    typeName: new FormControl('', Validators.required),
    properties: new FormControl(),
  });

  constructor(
    private equipmentService: EquipmentService,
    public dialog: MatDialog,
    public dialogRef?: MatDialogRef<AddTypeComponent>) {
  }

  parentTypeOptions: IOption[]; 
  propertyOptions: IOption[];
  parentTypeAlreadySelected: IOption[] = [];
  propertyAlreadySelected: IOption[] = [];

  ngOnInit(): void {
    this.parentTypeOptions = this.equipmentService.getTypes();
    this.propertyOptions = this.equipmentService.getProperties().map(q => ({id: q.id, value: q.displayName}))
  }

  // To be implemented
  update(){
    let objectFromServer = {
      parents: [
        {id: '1', value: 'parent from server 1'},
        {id: '2', value: 'parent from server 2'},
        {id: '3', value: 'parent from server 3'}
      ],
      typeName: 'TypeFromServer',
      properties: [
        {id: '1', value: 'prop from server 1'},
        {id: '2', value: 'prop from server 2'},
        {id: '3', value: 'prop from server 3'}
      ],
    }

    this.parentTypeAlreadySelected = objectFromServer.parents;
    this.propertyAlreadySelected = objectFromServer.properties;

    this.formGroup.setValue(objectFromServer);
  }

  onSubmit(){
    // Send to API
    console.log(this.formGroup.value);
  }

  addProperty(){
    console.log('gonna open');
    this.openDialog();
  }

  openDialog(): void {
    let dialogRef: MatDialogRef<any>;
        dialogRef = this.dialog.open(AddPropertyComponent);

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}