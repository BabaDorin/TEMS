import { TranslateService } from '@ngx-translate/core';
import { Component, Inject, OnInit, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, FormGroup, FormBuilder, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { Observable } from 'rxjs';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../../services/dialog.service';
import { TypeService } from '../../../services/type.service';
import { AddPropertyComponent } from '../add-property/add-property.component';
import { AddType } from './../../../models/asset/add-type.model';
import { AssetType } from './../../../models/asset/view-type.model';
import { IOption } from './../../../models/option.model';
import { AssetService } from './../../../services/asset.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { ReactiveFormsModule } from '@angular/forms';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';


@Component({
  selector: 'app-add-type',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatSelectModule,
    MatIconModule,
    TranslateModule,
    ChipsAutocompleteComponent
  ],
  templateUrl: './add-type.component.html',
  styleUrls: ['./add-type.component.scss']
})

export class AddTypeComponent extends TEMSComponent implements OnInit {

  // Provide a value for this field and it will update the record
  updateTypeId: string;

  formGroup: FormGroup;

  get parents() { return this.formGroup.controls.parents; }
  get properties() { return this.formGroup.controls.properties; }

  parentTypeOptions: IOption[] = [];
  propertyOptions: IOption[] = [];
  parentTypeAlreadySelected: IOption[] = [];
  propertyAlreadySelected: IOption[] = [];

  constructor(
    private assetService: AssetService,
    private dialogService: DialogService,
    private typeService: TypeService,
    private snackService: SnackService,
    public translate: TranslateService,
    @Optional() public dialogRef: MatDialogRef<AddTypeComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();
    this.updateTypeId = this.updateTypeId ?? this.dialogData?.updateTypeId;

    this.formGroup = new FormGroup({
      parents: new FormControl(),
      name: new FormControl('', Validators.required),
      properties: new FormControl(''),
    });
  }

  ngOnInit(): void {
    this.fetchTypes();
    this.fetchProperties();

    if (this.updateTypeId != undefined)
      this.update();
  }

  update() {
    this.subscriptions.push(
      this.typeService.getFullType(this.updateTypeId)
        .subscribe(result => {
          let resultType: AssetType = result;
          let updateType = new AddType();

          updateType.name = resultType.name;
          updateType.parents = resultType.parents != undefined
            ? resultType.parents
            : [];
          updateType.properties = resultType.properties != undefined
            ? resultType.properties.map(q => ({ value: q.id, label: q.displayName }))
            : [];

          if (updateType.parents != undefined) this.parentTypeAlreadySelected = updateType.parents;
          if (updateType.properties != undefined) this.propertyAlreadySelected = updateType.properties;

          this.formGroup.setValue(updateType);
        })
    )
  }

  onSubmit() {
    let addType: AddType = {
      id: this.updateTypeId,
      parents: this.parents == undefined ? [] as IOption[] : this.parents.value as IOption[],
      name: this.formGroup.controls.name.value,
      properties: this.properties == undefined ? [] as IOption[] : this.properties.value as IOption[]
    }

    let endPoint: Observable<any> = this.assetService.addType(addType);
    if (addType.id != undefined)
      endPoint = this.assetService.updateType(addType);

    this.subscriptions.push(
      endPoint
        .subscribe(result => {
          this.snackService.snack(result);
          if (result.status == 1)
            this.dialogRef.close();
        })
    )
  }

  addProperty() {
    this.dialogService.openDialog(
      AddPropertyComponent,
      undefined,
      () => {
        this.subscriptions.forEach(s => s.unsubscribe());
        this.subscriptions = [];
        this.fetchProperties();
      });
  }

  fetchTypes() {
    this.subscriptions.push(this.typeService.getAllAutocompleteOptions().subscribe(response => {
      this.parentTypeOptions = response;
      if (this.parentTypeOptions.length == 0)
        this.formGroup.controls.parents.disable();
    }));
  }

  fetchProperties() {
    this.subscriptions.push(this.assetService.getProperties().subscribe(response => {
      this.propertyOptions = response;

      if (this.propertyOptions.length == 0)
        this.formGroup.controls.properties.disable();
      else
        this.formGroup.controls.properties.enable();
    }));
  }
}