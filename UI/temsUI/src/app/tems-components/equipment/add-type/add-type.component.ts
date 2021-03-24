import { TEMSComponent } from 'src/app/tems/tems.component';
import { EquipmentType } from './../../../models/equipment/view-type.model';
import { AddType } from './../../../models/equipment/add-type.model';
import { map } from 'rxjs/operators';
import { IOption } from './../../../models/option.model';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ComponentType } from '@angular/cdk/portal';
import { AddPropertyComponent } from '../add-property/add-property.component';
import { Subscription, Observable } from 'rxjs';


@Component({
  selector: 'app-add-type',
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent extends TEMSComponent implements OnInit {
  
  // Provide a value for this field and it will update the record
  updateTypeId: string;

  formGroup = new FormGroup({
    parents: new FormControl(),
    name: new FormControl('', Validators.required),
    properties: new FormControl('', Validators.required),
  });

  get parents(){
    return this.formGroup.controls.parents;
  }

  get properties(){
    return this.formGroup.controls.properties;
  }

  parentTypeOptions: IOption[]; 
  propertyOptions: IOption[];
  parentTypeAlreadySelected: IOption[] = [];
  propertyAlreadySelected: IOption[] = [];

  constructor(
    private equipmentService: EquipmentService,
    public dialog: MatDialog,
    public dialogRef?: MatDialogRef<AddTypeComponent>
    ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.equipmentService.getTypes().subscribe(response=>{
      this.parentTypeOptions = response;
      if(this.parentTypeOptions.length == 0)
        this.formGroup.controls.parents.disable();
    }));

    this.subscriptions.push(this.equipmentService.getProperties().subscribe(response => {
      this.propertyOptions = response.map(r => ({value: r.id, label: r.displayName}));
      if(this.propertyOptions.length == 0)
        this.formGroup.controls.properties.disable();
    }));

    if(this.updateTypeId != undefined)
      this.update();
  }

  update(){
    this.subscriptions.push(
      this.equipmentService.getFullType(this.updateTypeId)
      .subscribe(result =>{
        console.log(result);
        let resultType: EquipmentType = result;
        let updateType = new AddType();

        updateType.name = resultType.name;
        
        updateType.parents = resultType.parents != undefined
          ? resultType.parents
          : [];
        
        updateType.properties = resultType.properties != undefined
          ? resultType.properties.map(q => ({ value: q.id, label: q.displayName}))
          : [];

        if(updateType.parents != undefined) this.parentTypeAlreadySelected = updateType.parents;
        if(updateType.properties != undefined) this.propertyAlreadySelected = updateType.properties;

        this.formGroup.setValue(updateType);
      })
    )
  }

  onSubmit(){
    if(
      this.formGroup.controls.properties == null || 
      this.formGroup.controls.properties.value.length == 0){
      alert("At leas one property is required. If there isn't any, add some.")
    }
    else
    {
      let addType: AddType = {
        id: this.updateTypeId,
        parents: this.parents == undefined ? [] as IOption[] : this.parents.value as IOption[],
        name: this.formGroup.controls.name.value,
        properties: this.properties == undefined ? [] as IOption[] : this.properties.value as IOption[],
      }

      let endPoint: Observable<any> = this.equipmentService.addType(addType);
      if(addType.id != undefined)
        endPoint = this.equipmentService.updateType(addType);

      this.subscriptions.push(
        endPoint
        .subscribe(result => {
          console.log(result);
        })
      )
    }
  }

  addProperty(){
    let dialogRef: MatDialogRef<any>;
        dialogRef = this.dialog.open(AddPropertyComponent);

    dialogRef.afterClosed().subscribe(result => {
      console.log('The dialog was closed');
    });
  }
}