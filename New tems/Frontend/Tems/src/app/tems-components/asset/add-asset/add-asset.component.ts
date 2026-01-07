import { Component, EventEmitter, Input, OnInit, Output, Inject, Optional } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AddEquipment } from 'src/app/models/asset/add-asset.model';
import { DefinitionService } from 'src/app/services/definition.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Definition } from '../../../models/asset/add-definition.model';
import { DialogService } from '../../../services/dialog.service';
import { FormlyParserService } from '../../../services/formly-parser.service';
import { SnackService } from '../../../services/snack.service';
import { TypeService } from '../../../services/type.service';
import { AddTypeComponent } from '.././add-type/add-type.component';
import { BulkUploadComponent } from '../bulk-upload/bulk-upload.component';
import { SICFileUploadResult } from './../../../models/asset/bulk-upload-result.model';
import { FormlyData } from './../../../models/formly/formly-data.model';
import { IOption } from './../../../models/option.model';
import { AssetService } from './../../../services/asset.service';
import { AddDefinitionComponent } from './../add-definition/add-definition.component';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { BulkUploadResultsComponent } from '../bulk-upload-results/bulk-upload-results.component';

@Component({
  selector: 'app-add-asset',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
    BulkUploadResultsComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-asset.component.html',
  styleUrls: ['./add-asset.component.scss'],
  providers: []
})
export class AddAssetComponent extends TEMSComponent implements OnInit {

  // BEFREE: Divide this component across multiple smaller and more maintanable components

  @Input() updateEquipmentId: string;
  @Input() updateAssetDefinitionId: string;

  @Output() done = new EventEmitter();
  @Output() goBack = new EventEmitter();

  updateEquipment: AddEquipment;

  types: IOption[] = [];
  selectedType: string;

  definitionsOfType: IOption[];
  selectedDefinition: IOption;
  selectedFullDefinition: Definition = undefined;

  // For selecting type and definition
  formGroup = new FormGroup({
    assetType: new FormControl(),
    assetDefinition: new FormControl(),
    properties: new FormControl(),
  });

  // For providing equipment data
  formlyData = new FormlyData();

  bulkUploadResults: SICFileUploadResult[];

  constructor(
    private assetService: AssetService,
    private typeService: TypeService,
    private definitionService: DefinitionService,
    private formlyParserService: FormlyParserService,
    private dialogService: DialogService,
    private snackService: SnackService,
    @Optional() public dialogRef: MatDialogRef<AddAssetComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    super();
  }

  ngOnInit(): void {
    if (this.updateEquipmentId != undefined) {
      this.edit();
      return;
    }

    this.fetchTypes();
  }

  fetchTypes() {
    this.subscriptions.push(
      this.typeService.getAllAutocompleteOptions()
        .subscribe(response => {
          if (this.snackService.snackIfError(response))
            return;

          this.types = response;
        }));
  }

  // Fetches all of selected type's definitions (called after selecting a type in order to display 
  // definition options)
  fetchDefinitionsOfType() {

    if (this.types.find(q => q.value == this.selectedType) == undefined) {
      this.definitionsOfType = undefined;
      return;
    }

    this.subscriptions.push(
      this.definitionService.getDefinitionsOfType(this.selectedType)
        .subscribe(response => {
          if (this.snackService.snackIfError(response))
            return;

          this.definitionsOfType = response;
        }))
  }

  // Cleans the forms and retrieves the definitions of the newly selected type
  onTypeChanged(eventData) {
    if (eventData.value == undefined)
      return;

    this.wipeAddAssetFormly();
    this.fetchDefinitionsOfType();
  }

  // Retrieves the full definition and build the formly form according to the data provided
  // by the definition
  onDefinitionChanged(eventData) {
    if (eventData.value == undefined)
      return;

    this.subscriptions.push(
      this.assetService.getFullDefinition(eventData.value)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;
          this.selectedFullDefinition = result;
          this.createAddAssetFormly();
        }));
  }

  // Calls formly parser service in order to build formly form structure
  // and sets the formly data object 
  createAddAssetFormly() {
    let addEq = this.assetService.generateAddAssetOfDefinition(this.selectedFullDefinition);
    let formlyFields = this.formlyParserService.parseAddEquipment(addEq, this.updateEquipmentId != undefined);

    this.formlyData.fields = formlyFields;

    this.formlyData.model = {
      assetDefinitionID: addEq.definition.id,
      identifier: this.selectedFullDefinition.identifier,
    };

    this.formlyData.isVisible = true;
  }

  // Wipes collected data except equipment type
  wipeAddAssetFormly() {
    this.selectedDefinition = { value: '', label: '' } as IOption;
    this.selectedFullDefinition = undefined;
    this.formlyData = new FormlyData();
  }

  openAddDefinition() {
    this.dialogService.openDialog(
      AddDefinitionComponent,
      [{ label: "typeId", value: this.selectedType }],
      () => {
        this.fetchDefinitionsOfType();
      });
  }

  openAddType() {
    this.dialogService.openDialog(
      AddTypeComponent,
      undefined,
      () => {
        this.fetchTypes();
      });
  }

  back() {
    this.goBack.emit();
  }

  // Opens the BulkUploadComponent in a dialog
  bulkUpload() {
    let dialogRef = this.dialogService.openDialog(BulkUploadComponent);

    dialogRef.afterClosed().subscribe(() => {
      this.bulkUploadResults = dialogRef.componentInstance["uploadResults"];
    });
  }

  // Clear bulk upload results
  clearResults() {
    this.bulkUploadResults = undefined;
  }

  // Gets updateEquipment's definition and build the formly-form accordingly
  edit() {
    if (this.updateAssetDefinitionId == undefined)
      return;

    this.subscriptions.push(
      this.assetService.getFullDefinition(this.updateAssetDefinitionId)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.selectedFullDefinition = result;

          this.createAddAssetFormly();
          this.fetchEquipmentData();
        })
    )
  }

  // Fetches equipment's information (for update case) and builds the formly model accordingly
  fetchEquipmentData() {
    this.subscriptions.push(
      this.assetService.getEquipmentToUpdate(this.updateEquipmentId)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.formlyData.wipeModel();

          this.formlyData.model.assetDefinitionID = this.selectedFullDefinition.id;
          this.formlyData.model.identifier = this.selectedFullDefinition.identifier;
          this.formlyData.model.temsid = result.temsid;
          this.formlyData.model.serialNumber = result.serialNumber;
          this.formlyData.model.isDefect = result.isDefect;
          this.formlyData.model.isUsed = result.isUsed;
          this.formlyData.model.description = result.description;
          this.formlyData.model.price = result.price;
          this.formlyData.model.currency = (result.currency != undefined) ? result.currency : this.selectedFullDefinition.currency;
          this.formlyData.model.purchaseDate = result.purchaseDate.toString().split('T')[0];
        })
    )
  }

  // Build an instance of AddAssetViewModel and sends it the server
  onSubmit(model) {
    let submitAddEquipment = new AddEquipment();
    submitAddEquipment = model;
    submitAddEquipment.id = this.updateEquipmentId;
    submitAddEquipment.children = [];

    // The following lines are here to identitfy children equipment from the
    // formly model.
    // Each array contains the '---' sequence, followed by the array index
    // For example, if a computer has 4 RAM chips and 2 GPUs, the information
    // will be prezented within the model the following way:

    // Computer object {
    //   other properties...

    //   RAM objects:
    //   {ram definition id}---{0}[ <---- 0 because it's the first array of Computer object
    //     {...},
    //     {...},
    //     {...},
    //     {...}  
    //   ];

    //   {cpu definition id}---{1}[
    //     {...},
    //     {...}
    //   ];
    // }

    // We don't have to worry about updating children objects because the user will have to 
    // access them separately in order to update them.

    var propNames = Object.getOwnPropertyNames(model);
    propNames.forEach(propName => {
      if (Array.isArray(model[propName]) && propName.indexOf("---") > -1) {
        model[propName].forEach(child => {
          let childEquipment = new AddEquipment();
          childEquipment = child;
          childEquipment.assetDefinitionID = propName.split('---')[0];
          submitAddEquipment.children.push(childEquipment);
        })
      }
    });

    // Send the collected data packed in an instance of AddAssetModel.
    let endPoint = this.updateEquipmentId == undefined
      ? this.assetService.addEquipment(submitAddEquipment)
      : this.assetService.updateEquipment(submitAddEquipment);

    this.subscriptions.push(
      endPoint
        .subscribe(result => {
          this.snackService.snack(result);

          if (result.status == 1 && this.updateEquipmentId == undefined) {
            this.createAddAssetFormly();
          }
      }));
  }
}