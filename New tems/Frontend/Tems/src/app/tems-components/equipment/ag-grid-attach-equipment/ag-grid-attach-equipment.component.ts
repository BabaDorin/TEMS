import { ClaimService } from './../../../services/claim.service';
import { DefectCellRenderedComponent } from './../../../public/ag-grid/defect-cell-rendered/defect-cell-rendered.component';
import { ResponseFactory } from './../../../models/system/response.model';
import { BooleanCellRendererComponent } from './../../../public/ag-grid/boolean-cell-renderer/boolean-cell-renderer.component';
import { AttachEquipment } from './../../../models/equipment/attach-equipment.model';
import { EquipmentService } from 'src/app/services/equipment.service';
import { SnackService } from './../../../services/snack.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit, Output, EventEmitter, SimpleChanges, OnChanges } from '@angular/core';
import { EquipmentFilter } from 'src/app/helpers/filters/equipment.filter';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { CommonModule } from '@angular/common';
import { AgGridModule } from 'ag-grid-angular';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-ag-grid-attach-equipment',
  standalone: true,
  imports: [CommonModule, AgGridModule, MatButtonModule, MatProgressBarModule],
  templateUrl: './ag-grid-attach-equipment.component.html',
  styleUrls: ['./ag-grid-attach-equipment.component.scss']
})
export class AgGridAttachEquipmentComponent extends TEMSComponent implements OnChanges {

  @Input() equipmentFilter: EquipmentFilter;
  @Input() parentId: string;
  @Output() attached = new EventEmitter();

  loading: boolean = true;

  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;

  constructor(
    private translate: TranslateService,
    private snackService: SnackService,
    private equipmentService: EquipmentService,
    private claims: ClaimService
  ) {
    super();

    this.pagination = true;
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent,
      defectCellRenderedComponent: DefectCellRenderedComponent
    };

    this.defaultColDef = {
      resizable: true,
      filter: true,
      sortable: true,
      width: 100,
      minWidth: 50,
    }

    this.columnDefs = [
      {
        headerName: this.translate.instant('equipment.TEMSID'),
        field: 'temsId',
        lockPosition: true,
      },
      {
        headerName: this.translate.instant('equipment.serialNumber'),
        field: 'serialNumber',
        width: 130
      },
      {
        headerName: this.translate.instant('equipment.definition'),
        field: 'definition',
      },
      {
        headerName: this.translate.instant('equipment.type'),
        field: 'type'
      },
      {
        headerName: this.translate.instant('equipment.functional'),
        field: 'isDefect',
        sortable: false,
        cellRenderer: 'defectCellRenderedComponent',
        width: 120,
        suppressSizeToFit: true
      },
    ];

    if(this.claims.canManage){
      this.columnDefs.push({
        filter: false,
        sortable: false,
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.attach.bind(this),
          matIcon: 'link_on'
        },
      })
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (propertyChanged(changes, "equipmentFilter"))
      this.fetchEquipment();
  }

  attach(eventData) {
    if (this.parentId == undefined) {
      this.snackService.snack(ResponseFactory.Neutral("Parent not specified"));
      return;
    }

    let model = new AttachEquipment();
    model.parentId = this.parentId;
    model.childrenIds = [eventData.rowData.id];

    this.subscriptions.push(
      this.equipmentService.attach(model)
        .subscribe(result => {
          this.snackService.snack(result);

          if (result.status == 1) {
            this.gridApi.applyTransaction({ remove: [eventData.rowData] });
            this.attached.emit(eventData.rowData);
          }
        })
    );
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.fetchEquipment();
  }

  fetchEquipment() {
    if (this.equipmentFilter == undefined) {
      this.snackService.snack(ResponseFactory.Neutral("filter not provided"));
      return;
    }

    this.loading = true;
    this.subscriptions.push(this.equipmentService.getEquipmentSimplified(this.equipmentFilter)
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;

        this.rowData = result;
        this.sizeToFit();
      }));
  }

  sizeToFit() {
    this.gridApi.sizeColumnsToFit();
  }
}
