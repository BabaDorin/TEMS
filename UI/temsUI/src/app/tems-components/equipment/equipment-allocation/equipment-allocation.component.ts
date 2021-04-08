import { FormGroup, FormControl, Validators } from '@angular/forms';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { AddAllocation } from './../../../models/allocation/add-allocation.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { IOption } from 'src/app/models/option.model';
import { AllocationService } from 'src/app/services/allocation-service/allocation.service';
import { SnackService } from 'src/app/services/snack/snack.service';

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
  allocatedToChipsInputLabel = "Choose one...";

  // Alocate to [select]
  selectedAllocateToType: string;
  allocateToTypes = [
    { value: 'room', viewValue: 'Room' },
    { value: 'personnel', viewValue: 'Personnel' }
  ];
  alocateeEndPoint;

  equipmentAllocationFormGroup: FormGroup; 

  constructor(
    private equipmentService: EquipmentService,
    private roomService: RoomsService,
    private snackService: SnackService,
    private personnelService: PersonnelService,
    private allocationService: AllocationService
  ) {
    super();
  }

  ngOnInit(): void {
    this.selectedAllocateToType = "room";
    this.allocatedToChipsInputLabel = 'Room identifier...';
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
        this.allocatedToChipsInputLabel = 'Room identifier...';
        this.alocateeEndPoint = this.roomService;
        break;
      case 'personnel':
        this.allocatedToChipsInputLabel = 'Name...';
        this.alocateeEndPoint = this.personnelService;
        break;
    }

    this.equipmentAllocationFormGroup.controls.allocateTo.setValue([] as IOption[]);
    this.allocatedTo.alreadySelected = [] as IOption[];
  }

  submit(model) {
    console.log(model);

    if (model.equipment.value.length == 0 || model.allocateTo.value.length != 1) {
      this.snackService.snack({
        message: "Please, provide at least one equipment and one allocatee",
        status: 0
      });

      return;
    }

    let addAllocation: AddAllocation = {
      equipments: model.equipment.value,
      allocateToType: model.allocateToType.value,
      allocateToId: model.allocateTo.value[0].value,
    }

    console.log(addAllocation);

    this.subscriptions.push(this.allocationService.createAllocation(addAllocation)
      .subscribe(result => {
        this.snackService.snack(result);
      }))
  }
}
