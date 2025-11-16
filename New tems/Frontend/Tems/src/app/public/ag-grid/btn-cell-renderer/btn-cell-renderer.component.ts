import { Component, Input, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { ICellRendererAngularComp } from 'ag-grid-angular';
import { IAfterGuiAttachedParams, ICellRendererParams } from 'ag-grid-community';

@Component({
  selector: 'app-btn-cell-renderer',
  standalone: true,
  imports: [CommonModule, MatButtonModule, MatIconModule],
  templateUrl: './btn-cell-renderer.component.html',
  styleUrls: ['./btn-cell-renderer.component.scss']
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
    const params = {
      event: $event,
      rowData: this.params.node.data
    }

    this.params.onClick(params);
  }

  ngOnDestroy() {

  }
}
