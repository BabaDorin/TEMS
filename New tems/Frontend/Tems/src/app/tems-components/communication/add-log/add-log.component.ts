import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { TranslateService } from '@ngx-translate/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { LogsService } from 'src/app/services/logs.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { SnackService } from '../../../services/snack.service';
import { AddLog } from './../../../models/communication/logs/add-log.model';
import { IOption } from './../../../models/option.model';
import { AssetService } from './../../../services/asset.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';

@Component({
  selector: 'app-add-log',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    TranslateModule,
    ChipsAutocompleteComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './add-log.component.html',
  styleUrls: ['./add-log.component.scss']
})
export class AddLogComponent extends TEMSComponent implements OnInit {

  @Input() equipment: IOption[];
  @Input() room: IOption[];
  @Input() personnel: IOption[];

  @ViewChild('addresseesChips') addresseesChips: ChipsAutocompleteComponent;

  // Mainly used for toggling chipsInput's visibility
  adresseeChosen: boolean = false;
  
  selectedAddresseeType: string;
  selectAddressee: any;
  addresseeTypes = [
    { value: 'asset', viewValue: 'Equipment' },
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];

  public formlyData = {
    isVisible: false,
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  autoCompleteOptions = [];
  alreadySelectedOptions = [];
  adresseeEndPoint;
  chipsInputLabel = "";

  constructor(
    private formlyParserService: FormlyParserService,
    private equipmentservice: AssetService,
    private roomService: RoomsService,
    private logsService: LogsService,
    private snackService: SnackService,
    public translate: TranslateService,
    private personnelService: PersonnelService,
    @Optional() public dialogRef: MatDialogRef<AddLogComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any) {
    super();

    if(this.dialogData != undefined){
      this.equipment = dialogData.equipment;
      this.room = dialogData.room;
      this.personnel = dialogData.personnel;
    }
  }

  ngOnInit(): void {
    let implicitAddressees;
    this.adresseeEndPoint = this.equipmentservice;

    this.adresseeChosen = this.equipment != undefined || this.room != undefined || this.personnel != undefined;

    if (this.adresseeChosen) {
      if (this.equipment != undefined){
        implicitAddressees = { type: 'asset', entities: this.equipment }
        this.adresseeEndPoint = this.equipmentservice
      }

      if (this.room != undefined){
        implicitAddressees = { type: 'room', entities: this.room }
        this.adresseeEndPoint = this.roomService
      }

      if (this.personnel != undefined){
        implicitAddressees = { type: 'personnel', entities: this.personnel }
        this.adresseeEndPoint = this.personnelService
      }

      this.selectedAddresseeType = implicitAddressees.type;

      this.onAddresseeSelection(implicitAddressees.entities);
    }

    this.formlyData.fields = this.formlyParserService.parseAddLog();
  }

  onAddresseeSelection(alreadySelected?: IOption[]) {
    switch (this.selectedAddresseeType) {

      case 'asset':
        this.chipsInputLabel = 'TEMSID or Serial Number...';
        this.adresseeEndPoint = this.equipmentservice
        break;
      
      case 'room':
        this.chipsInputLabel = 'Room identifier...';
        this.adresseeEndPoint = this.roomService
        break;
      
      case 'personnel':
        this.chipsInputLabel = 'Name...';
        this.adresseeEndPoint = this.personnelService
        break;
    }

    this.alreadySelectedOptions = (alreadySelected == undefined)
      ? []
      : alreadySelected;
    this.adresseeChosen = true;
  }

  onSubmit() {
    if(this.addresseesChips.selectOptions.length == 0)
      return;
        
    this.formlyData.model.log.addresseesType = this.selectedAddresseeType;
    this.formlyData.model.log.addressees = this.addresseesChips.selectOptions;

    let addLog = this.formlyData.model.log as AddLog;

    this.subscriptions.push(this.logsService.addLog(addLog)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.dialogRef.close();
      }))
  }
}