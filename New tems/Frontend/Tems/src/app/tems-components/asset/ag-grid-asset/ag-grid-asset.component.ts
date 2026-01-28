import { ConfirmService } from './../../../confirm.service';
import { ClaimService } from './../../../services/claim.service';
import { UsedCellRenderedComponent } from './../../../public/ag-grid/used-cell-rendered/used-cell-rendered.component';
import { DefectCellRenderedComponent } from './../../../public/ag-grid/defect-cell-rendered/defect-cell-rendered.component';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { AssetService } from './../../../services/asset.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { AssetDetailsGeneralComponent } from './../asset-details/asset-details-general/asset-details-general.component';
import { AssetFilter } from 'src/app/helpers/filters/asset.filter';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridModule } from 'ag-grid-angular';

@Component({
  selector: 'app-ag-grid-equipment',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule, AgGridModule],
  templateUrl: './ag-grid-asset.component.html',
  styleUrls: ['./ag-grid-asset.component.scss']
})

export class AgGridAssetComponent extends TEMSComponent implements OnChanges, OnInit {

  @Input() assetFilter: AssetFilter;

  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  rowSelection;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;
  loading: boolean = true;

  constructor(
    private assetService: AssetService,
    private dialogService: DialogService,
    private translate: TranslateService,
    private snackService: SnackService,
    private confirmService: ConfirmService,
    private claims: ClaimService) {
    super();

    this.pagination = true;
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent,
      defectCellRenderedComponent: DefectCellRenderedComponent,
      usedCellRendererComponent: UsedCellRenderedComponent
    };

    this.defaultColDef = {
      flex: 1,
      resizable: true,
      filter: true,
      sortable: true,
      width: 150,
      minWidth: 50,
    }

    this.columnDefs = [
      { 
        headerName: this.translate.instant('equipment.TEMSID'), 
        field: 'temsId',
        headerCheckboxSelection: true,
        headerCheckboxSelectionFilteredOnly: true,
        checkboxSelection: true,
        width: 180,
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
        headerName: this.translate.instant('equipment.assignee'), 
        field: 'assignee' 
      },
      { 
        headerName: this.translate.instant('equipment.type'), 
        field: 'type' 
      },
      {
        headerName: this.translate.instant('equipment.isUsed'), 
        field: 'isUsed', 
        sortable: false,
        cellRenderer: 'usedCellRendererComponent',
        width: 100
      },
      {
        headerName: this.translate.instant('equipment.functional'), 
        field: 'isDefect', 
        sortable: false,
        cellRenderer: 'defectCellRenderedComponent',
        width: 100
      },
      {
        filter: false,
        sortable: false,
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        },
        width: 100,
        suppressSizeToFit: true,
      },
    ];

    if(this.claims.canManageAssets) {
      this.columnDefs.push({
        cellRenderer: 'btnCellRendererComponent',
        filter: false,
        sortable: false,
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        },
        width: 100,
        suppressSizeToFit: true,
      });
    }
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(propertyChanged(changes, "assetFilter")){
        this.fetchEquipment();
    }
  }

  ngOnInit(): void {
    this.sizeToFit();
  }

  details(e) {
    this.dialogService.openDialog(
      AssetDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "assetId", value: e.rowData.id },
      ]
    )
  }

  async archieve(e){
    if(!await this.confirmService.confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
      return;
      
    this.subscriptions.push(
      this.assetService.archieveEquipment(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        if(result.status == 1)
          this.gridApi.applyTransaction({ remove: [e.rowData] });
      })
    );
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;
    this.fetchEquipment();
  }

  fetchEquipment(){
    this.loading = true;
    this.subscriptions.push(this.assetService.getEquipmentSimplified(this.assetFilter)
    .subscribe(result => {
      this.loading = false;
      if(this.snackService.snackIfError(result))
        return;

      this.rowData = result as any;
      this.sizeToFit();
    }));
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }

  sizeToFit() {
    this.gridApi.sizeColumnsToFit();
  }
}
