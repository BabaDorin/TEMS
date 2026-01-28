import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { AgGridModule } from 'ag-grid-angular';
import { PropertiesListComponent } from '../properties-list/properties-list.component';

@Component({
  selector: 'app-manage-properties',
  standalone: true,
  imports: [
    CommonModule,
    MatButtonModule,
    MatIconModule,
    MatProgressBarModule,
    TranslateModule,
    AgGridModule,
    PropertiesListComponent
  ],
  templateUrl: './manage-properties.component.html',
  styleUrls: ['./manage-properties.component.scss']
})
export class ManagePropertiesComponent implements OnInit {

  @Input() canManage: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }
}
