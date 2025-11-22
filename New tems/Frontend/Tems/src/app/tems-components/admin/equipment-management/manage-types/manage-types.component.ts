import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { EquipmentTypesListComponent } from '../equipment-types-list/equipment-types-list.component';

@Component({
  selector: 'app-manage-types',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslateModule,
    AgGridModule,
    EquipmentTypesListComponent
  ],
  templateUrl: './manage-types.component.html',
  styleUrls: ['./manage-types.component.scss']
})
export class ManageTypesComponent implements OnInit {

  @Input() canManage: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
