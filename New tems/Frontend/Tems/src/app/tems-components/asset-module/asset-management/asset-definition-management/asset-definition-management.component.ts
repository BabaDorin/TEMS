import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridApi, GridReadyEvent } from 'ag-grid-community';
import { AssetDefinitionService } from 'src/app/services/asset-definition.service';
import { AssetTypeService } from 'src/app/services/asset-type.service';
import { ThemeService } from 'src/app/services/theme.service';
import { AssetDefinition } from 'src/app/models/asset/asset-definition.model';
import { AssetType } from 'src/app/models/asset/asset-type.model';
import { AddDefinitionComponent } from '../../../asset/add-definition/add-definition.component';

@Component({
  selector: 'app-asset-definition-management',
  standalone: true,
  imports: [
    CommonModule,
    AgGridAngular,
    MatDialogModule
  ],
  templateUrl: './asset-definition-management.component.html',
  styleUrls: ['./asset-definition-management.component.scss']
})
export class AssetDefinitionManagementComponent implements OnInit {
  rowData: AssetDefinition[] = [];
  assetTypes: AssetType[] = [];
  gridApi!: GridApi;
  gridReady = false;

  columnDefs: ColDef[] = [
    {
      headerName: 'Name',
      field: 'name',
      flex: 2,
      minWidth: 200,
      cellClass: 'font-medium'
    },
    {
      headerName: 'Type',
      field: 'assetTypeName',
      flex: 1,
      minWidth: 150
    },
    {
      headerName: 'Manufacturer',
      field: 'manufacturer',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Model',
      field: 'model',
      flex: 1,
      minWidth: 150,
      valueFormatter: (params) => params.value || '—'
    },
    {
      headerName: 'Usage Count',
      field: 'usageCount',
      flex: 0.7,
      minWidth: 120,
      type: 'numericColumn'
    },
    {
      headerName: 'Status',
      field: 'isArchived',
      flex: 0.7,
      minWidth: 100,
      cellRenderer: (params: any) => {
        const isArchived = params.value;
        const className = isArchived ? 'text-gray-600' : 'text-green-600';
        return `<span class="${className}">${isArchived ? 'Archived' : 'Active'}</span>`;
      }
    },
    {
      headerName: 'Actions',
      flex: 1,
      minWidth: 150,
      cellRenderer: (params: any) => {
        return `
          <button class="action-edit-btn px-2 py-1 text-blue-600 hover:text-blue-800 text-sm mr-2">
            Edit
          </button>
          <button class="action-delete-btn px-2 py-1 text-red-600 hover:text-red-800 text-sm">
            Delete
          </button>
        `;
      },
      onCellClicked: (params) => {
        const target = params.event?.target as HTMLElement;
        if (target.classList.contains('action-edit-btn')) {
          this.editDefinition(params.data);
        } else if (target.classList.contains('action-delete-btn')) {
          this.deleteDefinition(params.data.id);
        }
      }
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true,
    flex: 1
  };

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  constructor(
    private assetDefinitionService: AssetDefinitionService,
    private assetTypeService: AssetTypeService,
    private dialog: MatDialog,
    private themeService: ThemeService
  ) {}

  ngOnInit(): void {
    this.loadDefinitions();
    this.loadTypes();
  }

  onGridReady(params: GridReadyEvent) {
    this.gridApi = params.api;
    this.gridReady = true;
    this.gridApi.sizeColumnsToFit();
  }

  loadDefinitions() {
    this.assetDefinitionService.getAll().subscribe({
      next: (definitions) => {
        this.rowData = definitions;
      },
      error: (error) => {
        console.error('Error loading definitions:', error);
      }
    });
  }

  loadTypes() {
    this.assetTypeService.getAll().subscribe({
      next: (types) => {
        this.assetTypes = types.filter(t => !t.isArchived);
      },
      error: (error) => {
        console.error('Error loading types:', error);
      }
    });
  }

  openCreateModal() {
    this.openDefinitionDialog();
  }

  editDefinition(definition: AssetDefinition) {
    this.openDefinitionDialog({ updateDefinitionId: definition.id, typeId: definition.assetTypeId });
  }

  deleteDefinition(id: string) {
    if (!confirm('Are you sure you want to delete this definition?')) return;

    this.assetDefinitionService.delete(id).subscribe({
      next: () => {
        this.loadDefinitions();
      },
      error: (error) => {
        console.error('Error deleting definition:', error);
      }
    });
  }

  private openDefinitionDialog(data?: { updateDefinitionId?: string; typeId?: string }) {
    const dialogRef = this.dialog.open(AddDefinitionComponent, {
      width: '960px',
      maxHeight: '90vh',
      data
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result !== undefined) {
        this.loadDefinitions();
        this.loadTypes();
      }
    });
  }
}
