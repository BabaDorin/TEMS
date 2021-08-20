import { Component } from '@angular/core';
import { ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'app-defect-cell-rendered',
  templateUrl: './defect-cell-rendered.component.html',
  styleUrls: ['./defect-cell-rendered.component.scss']
})
export class DefectCellRenderedComponent {

  public cellValue: boolean;

  // gets called once before the renderer is used
  agInit(params: ICellRendererParams): void {
      this.cellValue = this.getValueToDisplay(params);
  }

  // gets called whenever the cell refreshes
  refresh(params: ICellRendererParams) {
      // set value into cell again
      this.cellValue = this.getValueToDisplay(params);
  }

  getValueToDisplay(params: ICellRendererParams) {
      return params.valueFormatted ? params.valueFormatted : params.value;
  }
}
