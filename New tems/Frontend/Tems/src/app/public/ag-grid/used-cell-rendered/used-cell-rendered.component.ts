import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-used-cell-rendered',
  standalone: true,
  imports: [CommonModule, MatIconModule, MatCheckboxModule],
  templateUrl: './used-cell-rendered.component.html',
  styleUrls: ['./used-cell-rendered.component.scss']
})
export class UsedCellRenderedComponent implements ICellRendererAngularComp {
  value: boolean;

  get cellValue() {
    return this.value;
  }

  agInit(params: any): void {
    this.value = params.value;
  }

  refresh(params: any) {
    this.value = params.value;
    return true;
  }
}
