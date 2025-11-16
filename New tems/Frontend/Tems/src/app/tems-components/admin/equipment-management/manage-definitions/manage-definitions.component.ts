import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { EquipmentDefinitionsListComponent } from '../equipment-definitions-list/equipment-definitions-list.component';

@Component({
  selector: 'app-manage-definitions',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslateModule,
    AgGridModule,
    EquipmentDefinitionsListComponent
  ],
  templateUrl: './manage-definitions.component.html',
  styleUrls: ['./manage-definitions.component.scss']
})
export class ManageDefinitionsComponent implements OnInit {

  @Input() canManage: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }
}
