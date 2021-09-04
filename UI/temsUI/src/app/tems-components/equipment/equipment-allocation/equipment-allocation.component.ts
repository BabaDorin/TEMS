import { ResponseFactory } from './../../../models/system/response.model';
import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { AllocationService } from 'src/app/services/allocation.service';
import { EquipmentService } from 'src/app/services/equipment.service';
import { SnackService } from 'src/app/services/snack.service';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { AddAllocation } from './../../../models/allocation/add-allocation.model';
import { ClaimService } from './../../../services/claim.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-equipment-allocation',
  templateUrl: './equipment-allocation.component.html',
  styleUrls: ['./equipment-allocation.component.scss']
})
export class EquipmentAllocationComponent extends TEMSComponent implements OnInit {

  // There are 4 ways this component will get displayed.
  // 1) Without any input parameters being sent 
  // 2) With the equipment already defined
  // 3) With the room already defined
  // 4) With the personnel already defined

  @Input() equipment: IOption[];
  @Input() room: IOption[];
  @Input() personnel: IOption[];

  @ViewChild('equipmentIdentifierChips') equipmentIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo') allocatedTo: ChipsAutocompleteComponent;

  equipmentAlreadySelectedOptions = [] as IOption[];

  allocatedToAlreadySelectedOptions = [] as IOption[]
  allocatedToChipsInputLabel = this.translate.instant("form.hints.Choose one");

  // Alocate to [select]
  selectedAllocateToType: string;
  alocateeEndPoint;
  allocateToTypes = [
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];

  equipmentAllocationFormGroup: FormGroup;
  dialogRef;

  constructor(
    public equipmentService: EquipmentService,
    public roomService: RoomsService,
    private snackService: SnackService,
    public personnelService: PersonnelService,
    private allocationService: AllocationService,
    private claims: ClaimService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) {
    super();

    if(this.dialogData != undefined){
      this.equipment = dialogData.equipment;
      this.room = dialogData.room;
      this.personnel = dialogData.personnel;
    }
  }

  ngOnInit(): void {
    this.selectedAllocateToType = "room";
    this.allocatedToChipsInputLabel = this.translate.instant('form.hints.Room identifier');
    this.alocateeEndPoint = this.roomService;

    if (this.equipment != undefined)
      this.equipmentAlreadySelectedOptions = this.equipment;

    if (this.room != undefined) {
      this.allocatedToAlreadySelectedOptions = this.room;
      this.selectedAllocateToType = 'room';
      this.alocateeEndPoint = this.roomService;
    }

    if (this.personnel != undefined) {
      this.allocatedToAlreadySelectedOptions = this.personnel;
      this.selectedAllocateToType = 'personnel';
      this.alocateeEndPoint = this.personnelService;
    }

    this.equipmentAllocationFormGroup = new FormGroup({
      equipment: new FormControl(this.equipmentAlreadySelectedOptions, Validators.required),
      allocateTo: new FormControl(this.allocatedToAlreadySelectedOptions, Validators.required),
      allocateToType: new FormControl(this.selectedAllocateToType),
    })
  }

  onSelection() {
    switch (this.equipmentAllocationFormGroup.controls.allocateToType.value) {
      case 'room':
        this.allocatedToChipsInputLabel = this.translate.instant('form.hints.Room identifier');
        this.alocateeEndPoint = this.roomService;
        break;
      case 'personnel':
        this.allocatedToChipsInputLabel = this.translate.instant('form.hints.Name');;
        this.alocateeEndPoint = this.personnelService;
        break;
    }

    this.equipmentAllocationFormGroup.controls.allocateTo.setValue([] as IOption[]);
    this.allocatedTo.alreadySelected = [] as IOption[];
  }

  submit(model) {
    if (model.equipment.value.length == 0 || model.allocateTo.value.length != 1) {
      this.snackService.snack(ResponseFactory.Neutral("Please, provide at least one equipment and one allocatee"));
      return;
    }

    let addAllocation: AddAllocation = {
      equipments: model.equipment.value,
      allocateToType: model.allocateToType.value,
      allocateToId: model.allocateTo.value[0].value,
    }

    this.subscriptions.push(this.allocationService.createAllocation(addAllocation)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1 && this.dialogRef != undefined)
          this.dialogRef.close();
        
        this.clearModel();
      }))
  }

  clearModel(){
    this.equipmentIdentifierChips.options = [];
    this.allocatedTo.options = [];
  }
}
