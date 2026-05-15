import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AgGridAngular } from 'ag-grid-angular';
import { ColDef, GridReadyEvent } from 'ag-grid-community';
import { PurchaseOrder } from 'src/app/models/purchase-order/purchase-order.model';
import { PurchaseOrderService } from 'src/app/services/purchase-order.service';
import { ThemeService } from 'src/app/services/theme.service';
import { UserService } from 'src/app/services/user.service';
import { PurchaseOrderPreviewModalComponent } from '../purchase-order-preview-modal/purchase-order-preview-modal.component';

@Component({
  selector: 'app-view-purchase-orders',
  standalone: true,
  imports: [CommonModule, AgGridAngular, PurchaseOrderPreviewModalComponent],
  templateUrl: './view-purchase-orders.component.html',
  styleUrls: ['./view-purchase-orders.component.scss']
})
export class ViewPurchaseOrdersComponent implements OnInit {
  rowData: PurchaseOrder[] = [];
  selectedPurchaseOrder: PurchaseOrder | null = null;
  showPreviewModal = false;
  loading = false;
  currentUserId = '';

  columnDefs: ColDef[] = [
    {
      headerName: 'PO Number',
      field: 'poNumber',
      flex: 1.2,
      minWidth: 180,
      cellClass: 'font-medium text-[#007aff] dark:text-[#0a84ff]'
    },
    {
      headerName: 'Vendor',
      field: 'vendor',
      flex: 1.3,
      minWidth: 180
    },
    {
      headerName: 'Accountable',
      field: 'accountableDisplayName',
      flex: 1.2,
      minWidth: 180
    },
    {
      headerName: 'PO Amount',
      field: 'amount',
      flex: 1,
      minWidth: 140,
      valueFormatter: params => this.formatCurrency(params.data?.amount, params.data?.currency)
    },
    {
      headerName: 'Used',
      field: 'usedAmount',
      flex: 1,
      minWidth: 140,
      valueFormatter: params => this.formatCurrency(params.data?.usedAmount, params.data?.currency)
    },
    {
      headerName: 'Available',
      field: 'availableAmount',
      flex: 1,
      minWidth: 150,
      cellRenderer: params => {
        const value = Number(params.value || 0);
        const currency = params.data?.currency || '';
        const className = value < 0
          ? 'text-red-600 dark:text-red-400'
          : value === 0
            ? 'text-green-600 dark:text-green-400'
            : 'text-amber-600 dark:text-amber-400';
        return `<span class="${className} font-medium">${this.formatCurrency(value, currency)}</span>`;
      }
    },
    {
      headerName: 'Items',
      field: 'itemCount',
      flex: 0.7,
      minWidth: 100
    },
    {
      headerName: 'Created',
      field: 'createdAt',
      flex: 1,
      minWidth: 150,
      valueFormatter: params => new Date(params.value).toLocaleDateString('en-GB', {
        day: '2-digit',
        month: 'short',
        year: 'numeric'
      })
    }
  ];

  defaultColDef: ColDef = {
    sortable: true,
    filter: true,
    resizable: true
  };

  constructor(
    private purchaseOrderService: PurchaseOrderService,
    private themeService: ThemeService,
    private userService: UserService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadPurchaseOrders();
    this.userService.getMyUser().subscribe({
      next: user => {
        this.currentUserId = user.id || '';
      }
    });
  }

  get gridThemeClass(): string {
    return this.themeService.isDarkMode ? 'ag-theme-quartz-dark' : 'ag-theme-quartz';
  }

  loadPurchaseOrders(): void {
    this.loading = true;
    this.purchaseOrderService.getAll().subscribe({
      next: purchaseOrders => {
        this.rowData = purchaseOrders;
        this.loading = false;
      },
      error: () => {
        this.rowData = [];
        this.loading = false;
      }
    });
  }

  onGridReady(params: GridReadyEvent): void {
    params.api.sizeColumnsToFit();
  }

  onRowClicked(event: any): void {
    this.selectedPurchaseOrder = event.data;
    this.showPreviewModal = true;
  }

  closePreviewModal(): void {
    this.showPreviewModal = false;
    this.selectedPurchaseOrder = null;
  }

  canDeletePurchaseOrder(purchaseOrder: PurchaseOrder | null): boolean {
    if (!purchaseOrder || !this.currentUserId) {
      return false;
    }

    return purchaseOrder.createdByUserId === this.currentUserId || purchaseOrder.accountableUserId === this.currentUserId;
  }

  deleteSelectedPurchaseOrder(): void {
    if (!this.selectedPurchaseOrder) {
      return;
    }

    this.purchaseOrderService.delete(this.selectedPurchaseOrder.id).subscribe({
      next: () => {
        this.closePreviewModal();
        this.loadPurchaseOrders();
      }
    });
  }

  openAsset(assetId: string): void {
    this.closePreviewModal();
    this.router.navigate(['/assets', assetId]);
  }

  private formatCurrency(amount: number, currency: string): string {
    return `${Number(amount || 0).toFixed(2)} ${currency || ''}`.trim();
  }
}
