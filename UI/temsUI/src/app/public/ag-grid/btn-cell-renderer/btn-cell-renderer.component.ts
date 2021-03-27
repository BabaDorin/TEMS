import { Component, OnDestroy, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams, IAfterGuiAttachedParams } from 'ag-grid-community';

@Component({
  selector: 'app-btn-cell-renderer',
  template: `
    <button mat-button (click)="onClick($event)">{{label}}</button>
  `,
})
export class BtnCellRendererComponent implements ICellRendererAngularComp, OnDestroy {
  
  params;
  label: string;

  refresh(params: ICellRendererParams): boolean {
    return true;
  }
  
  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
    
  }

  agInit(params: any): void {
    this.params = params;
    this.label = this.params.label || null;
  }

  onClick($event) {
    if (this.params.onClick instanceof Function) {
      // put anything into params u want pass into parents component
      const params = {
        event: $event,
        rowData: this.params.node.data
        // ...something
      }
      this.params.onClick(params);
    }
  }

  ngOnDestroy() {

  }
}
