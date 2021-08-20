import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from 'src/app/services/dialog.service';
import { SnackService } from 'src/app/services/snack.service';
import { PersonnelService } from '../../../services/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelDetailsGeneralComponent } from './../personnel-details-general/personnel-details-general.component';

@Component({
  selector: 'app-ag-grid-personnel',
  templateUrl: './ag-grid-personnel.component.html',
  styleUrls: ['./ag-grid-personnel.component.scss']
})
export class AgGridPersonnelComponent extends TEMSComponent {

  gridApi;
  gridColumnApi;
  columnDefs;
  defaultColDef;
  rowData: [];
  frameworkComponents: any;
  pagination
  paginationPageSize;
  loading: boolean = true;

  personnel: ViewPersonnelSimplified[];

  constructor(
    private personnelService: PersonnelService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private translate: TranslateService
  ) { 
    super();

    this.pagination = true;
    this.paginationPageSize = 20;

    this.frameworkComponents = {
      btnCellRendererComponent: BtnCellRendererComponent
    };

    this.defaultColDef = {
      flex: 1,
      resizable: true,
      filter: true,
      sortable: true,
      width: 150,
      minWidth: 80,
    }

    this.columnDefs = [
      { 
        headerName: this.translate.instant('personnel.name'),
        field: 'name', 
        width: 200,
        checkboxSelection: true,
        headerCheckboxSelection: true,
        lockPosition: true
      },
      { 
        headerName: this.translate.instant('personnel.positions'),
        field: 'positions', 
      },
      { 
        headerName: this.translate.instant('entities.allocatedEquipment'),
        field: 'allocatedEquipments'
      },
      { 
        headerName: this.translate.instant('entities.activeTickets'),
        field: 'activeTickets'
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        },
        width: 100,
        suppressSizeToFit: true,
      },
      {
        cellRenderer: 'btnCellRendererComponent',
        cellRendererParams: {
          onClick: this.archieve.bind(this),
          matIcon: 'delete',
          matIconClass: 'text-muted'
        },
        width: 100,
        suppressSizeToFit: true,
      }
    ];
  }

  onGridReady(params) {
    this.gridApi = params.api;
    this.gridColumnApi = params.columnApi;

    this.fetchPersonnel();
  }

  fetchPersonnel(){
    this.loading = true;
    this.subscriptions.push(this.personnelService.getPersonnelSimplified(20, 20)
      .subscribe(result => {
        this.rowData = result;
        this.loading = false;
      }));
  }

  isFirstColumn(params) {
    var displayedColumns = params.columnApi.getAllDisplayedColumns();
    var thisIsFirstColumn = displayedColumns[0] === params.column;
    return thisIsFirstColumn;
  }

  details(e) {
    this.dialogService.openDialog(
      PersonnelDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "personnelId", value: e.rowData.id },
      ]
    );
  }

  archieve(e){
    if(!confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
    return;

    this.subscriptions.push(
      this.personnelService.archievePersonnel(e.rowData.id)
      .subscribe(result => {
        if(this.snackService.snack(result)){
          this.gridApi.applyTransaction({ remove: [e.rowData] });
        }
      })
    );
  }

  getSelectedNodes(){
    return this.gridApi.getSelectedNodes().map(q => q.data);
  }
}
