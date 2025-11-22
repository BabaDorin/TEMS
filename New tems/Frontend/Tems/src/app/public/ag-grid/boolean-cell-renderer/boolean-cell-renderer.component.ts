import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { ICellRendererAngularComp } from 'ag-grid-angular';

@Component({
  selector: 'app-boolean-cell-renderer',
  standalone: true,
  imports: [CommonModule, MatIconModule],
  templateUrl: './boolean-cell-renderer.component.html',
  styleUrls: ['./boolean-cell-renderer.component.scss']
})
export class BooleanCellRendererComponent implements ICellRendererAngularComp {
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
