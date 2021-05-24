import { style } from '@angular/animations';
import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { ICellRendererParams, IAfterGuiAttachedParams } from 'ag-grid-community';

@Component({
  selector: 'app-btn-cell-renderer',
  styleUrls: ['./btn-cell-renderer.component.scss'],
  templateUrl: './btn-cell-renderer.component.html'
})
export class BtnCellRendererComponent implements ICellRendererAngularComp, OnDestroy {
  
  params;
  @Input() label: string;
  @Input() matIcon: string;
  @Input() matIconClass: string = "text-primary";

  refresh(params: ICellRendererParams): boolean {
    return true;
  }
  
  afterGuiAttached?(params?: IAfterGuiAttachedParams): void {
    
  }

  agInit(params: any): void {
    this.params = params;
    this.label = this.params.label || undefined;
    this.matIcon = this.params.matIcon || undefined;
    this.matIconClass = this.params.matIconClass || this.matIconClass;
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
