import { ConfirmService } from './../../../confirm.service';
import { ClaimService } from './../../../services/claim.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { BtnCellRendererComponent } from 'src/app/public/ag-grid/btn-cell-renderer/btn-cell-renderer.component';
import { DialogService } from 'src/app/services/dialog.service';
import { SnackService } from 'src/app/services/snack.service';
import { PersonnelService } from '../../../services/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelDetailsGeneralComponent } from './../personnel-details-general/personnel-details-general.component';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridModule } from 'ag-grid-angular';

@Component({
  selector: 'app-ag-grid-personnel',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule, AgGridModule],
  templateUrl: './ag-grid-personnel.component.html',
  styleUrls: ['./ag-grid-personnel.component.scss']
})
export class AgGridPersonnelComponent extends TEMSComponent implements OnInit {

  @Input() columnDefs: any[];
  @Input() rowData: any[];
  @Input() defaultColDef: any;
  @Input() pagination: boolean = true;
  @Input() paginationPageSize: number = 10;
  @Input() frameworkComponents: any = {};

  gridApi;
  gridColumnApi;
  loading: boolean = true;

  personnel: ViewPersonnelSimplified[];

  constructor(
    private personnelService: PersonnelService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private translate: TranslateService,
    private claims: ClaimService,
    private confirmService: ConfirmService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.pagination = true;
    this.paginationPageSize = 20;

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
        cellRenderer: BtnCellRendererComponent,
        cellRendererParams: {
          onClick: this.details.bind(this),
          matIcon: 'more_horiz'
        },
        width: 100,
        suppressSizeToFit: true,
      },
    ];

    if(this.claims.canManage){
      this.columnDefs.push({
        cellRenderer: BtnCellRendererComponent,
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

  async archieve(e){
    if(!await this.confirmService.confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
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
