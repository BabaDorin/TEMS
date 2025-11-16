import { Component, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule, FormGroup, FormBuilder } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

@Component({
  selector: 'app-view-equipment-allocations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatIconModule,
    TranslateModule
  ],
  schemas: [NO_ERRORS_SCHEMA],
  templateUrl: './view-equipment-allocations.component.html',
  styleUrls: ['./view-equipment-allocations.component.scss']
})
export class ViewEquipmentAllocationsComponent implements OnInit {
  filterAllocationsFormGroup: FormGroup;
  equipments: any[] = [];
  personnel: any[] = [];
  rooms: any[] = [];
  definitions: any[] = [];
  equipmentService: any;
  roomService: any;
  personnelService: any;
  definitionService: any;

  constructor(
    private fb: FormBuilder,
    public translate: TranslateService
  ) {
    this.filterAllocationsFormGroup = this.fb.group({
      equipment: [],
      rooms: [],
      personnel: [],
      definitions: []
    });
  }

  ngOnInit(): void {
  }

  filtersChanged(): void {
    // ugly, I know
    this.equipments = (this.filterAllocationsFormGroup.controls.equipment.value != undefined) 
      ? this.filterAllocationsFormGroup.controls.equipment.value.map(q => q.value)
      : [];

    this.rooms = (this.filterAllocationsFormGroup.controls.rooms.value != undefined)
      ? this.filterAllocationsFormGroup.controls.rooms.value.map(q => q.value)
      : [];

    this.definitions = (this.filterAllocationsFormGroup.controls.definitions.value != undefined)
      ? this.filterAllocationsFormGroup.controls.definitions.value.map(q => q.value)
      : [];

    this.personnel = (this.filterAllocationsFormGroup.controls.personnel.value != undefined)
      ? this.filterAllocationsFormGroup.controls.personnel.value.map(q => q.value)
      : [];
  }
}
