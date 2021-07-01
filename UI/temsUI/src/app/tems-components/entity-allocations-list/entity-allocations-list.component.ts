import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { ViewAllocationSimplified } from 'src/app/models/equipment/view-equipment-allocation.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { IOption } from 'src/app/models/option.model';
import { DialogService } from 'src/app/services/dialog.service';
import { AllocationService } from '../../services/allocation.service';
import { SnackService } from '../../services/snack.service';
import { EquipmentAllocationComponent } from '../equipment/equipment-allocation/equipment-allocation.component';
import { ViewPersonnelSimplified } from './../../models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from './../../models/room/view-room-simplified.model';
import { ClaimService } from './../../services/claim.service';
import { TEMSComponent } from './../../tems/tems.component';

@Component({
  selector: 'app-entity-allocations-list',
  templateUrl: './entity-allocations-list.component.html',
  styleUrls: ['./entity-allocations-list.component.scss']
})
export class EntityAllocationsListComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() equipment: ViewEquipmentSimplified; 
  @Input() room: ViewRoomSimplified; 
  @Input() personnel: ViewPersonnelSimplified; 

  @Input() equipmentIds: string[] = [];
  @Input() personnelIds: string[] = [];
  @Input() definitionIds: string[] = [];
  @Input() roomIds: string[] = [];
  @Input() onlyActive: boolean;
  @Input() onlyClosed: boolean;
  @Input() include: string;
  
  @Output() allocationCreated = new EventEmitter();
  equipmentNotAllocated = false;
  
  // Pagination
  pageNumber = 1;
  totalItems = 0;
  itemsPerPage = 10;

  allocations: ViewAllocationSimplified[];
  loading = true;

  constructor(
    private allocationService: AllocationService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private router: Router,
    public claims: ClaimService
  ) {
    super();
  }

  ngOnInit(): void {
    this.loading = true;
    this.readInputVariables();
    this.getTotalItems();
    this.fetchAllocations();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }

    this.loading = true;
    this.readInputVariables();
    this.getTotalItems();
    this.fetchAllocations();
  }
  
  getTotalItems(){
    let endPoint = this.allocationService.getTotalItems(
      this.equipmentIds,
      this.definitionIds,
      this.personnelIds,
      this.roomIds,
      this.include,
    );

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.totalItems = result;
      })
    )
  }

  readInputVariables(){
    if(this.equipment) this.equipmentIds = [this.equipment.id];
    if(this.room) this.roomIds = [this.room.id];
    if(this.personnel) this.personnelIds = [this.personnel.id];

    if(this.onlyActive == undefined) this.onlyActive = false;
    if(this.onlyClosed == undefined) this.onlyClosed = false;
    
    this.include = 'any';
    if(this.onlyActive) this.include = 'active';
    if(this.onlyClosed) this.include = 'returned';
  }

  fetchAllocations(){
    let endPoint = this.allocationService.getAllocations(
      this.equipmentIds,
      this.definitionIds,
      this.personnelIds,
      this.roomIds,
      this.include,
      this.pageNumber,
      this.itemsPerPage
    );

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;

        this.allocations = result;
        this.equipmentNotAllocated = (this.equipment != undefined && this.allocations.findIndex(q => q.dateReturned == null) == -1);
      })
    )
  }

  addAllocation(): void {
    let selectedEntityType: string;
    let selectedEntities: IOption[];

    if(this.equipment){
      selectedEntityType = "equipment";
      selectedEntities = [
        {
          value: this.equipment.id, 
          label: this.equipment.temsIdOrSerialNumber
        }];
    }

    if(this.room){
      selectedEntityType = "room";
      selectedEntities = [
        {
          value: this.room.id, 
          label: this.room.identifier
        }];
    }

    if(this.personnel){
      selectedEntityType = "personnel";
      selectedEntities = [
        {
          value: this.personnel.id, 
          label: this.personnel.name
        }];
    }

    this.dialogService.openDialog(
      EquipmentAllocationComponent,
      [{label: selectedEntityType, value: selectedEntities}],
      () => {
        this.fetchAllocations();
        this.allocationCreated.emit();
      }
    )
  }

  allocationReturned(index: number){
    if(this.onlyActive)
    this.allocations.splice(index, 1);
  }

  allocationRemoved(index:number){
    this.allocations.splice(index, 1);
  }
}
