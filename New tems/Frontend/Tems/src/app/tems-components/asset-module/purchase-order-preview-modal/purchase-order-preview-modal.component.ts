import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { PurchaseOrder } from 'src/app/models/purchase-order/purchase-order.model';

@Component({
  selector: 'app-purchase-order-preview-modal',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './purchase-order-preview-modal.component.html',
  styleUrls: ['./purchase-order-preview-modal.component.scss']
})
export class PurchaseOrderPreviewModalComponent {
  @Input() show = false;
  @Input() purchaseOrder: PurchaseOrder | null = null;
  @Input() canDelete = false;

  @Output() closed = new EventEmitter<void>();
  @Output() deleteRequested = new EventEmitter<void>();
  @Output() assetSelected = new EventEmitter<string>();

  close(): void {
    this.closed.emit();
  }

  requestDelete(): void {
    this.deleteRequested.emit();
  }

  openAsset(assetId: string): void {
    this.assetSelected.emit(assetId);
  }

  printToPdf(): void {
    if (!this.purchaseOrder) {
      return;
    }

    const warning = this.getAmountWarning();
    const itemsHtml = this.purchaseOrder.items.map(item => `
      <tr>
        <td style="padding:8px 10px;border-bottom:1px solid #e5e7eb;">${item.assetTag}</td>
        <td style="padding:8px 10px;border-bottom:1px solid #e5e7eb;">${item.serialNumber || '—'}</td>
        <td style="padding:8px 10px;border-bottom:1px solid #e5e7eb;text-align:right;">${this.formatCurrency(item.price)}</td>
      </tr>
    `).join('');

    const printWindow = window.open('', '_blank', 'width=980,height=720');
    if (!printWindow) {
      return;
    }

    printWindow.document.write(`
      <html>
        <head>
          <title>Purchase Order ${this.purchaseOrder.poNumber}</title>
          <style>
            body { font-family: Arial, sans-serif; color: #111827; padding: 24px; }
            h1, h2, h3, p { margin: 0; }
            .grid { display: grid; grid-template-columns: repeat(3, minmax(0, 1fr)); gap: 16px; margin: 20px 0; }
            .card { border: 1px solid #e5e7eb; border-radius: 12px; padding: 14px; }
            .label { font-size: 12px; color: #6b7280; margin-bottom: 6px; display: block; text-transform: uppercase; letter-spacing: .05em; }
            .warning { margin: 18px 0; padding: 12px 14px; border-radius: 12px; background: #fff7ed; color: #9a3412; border: 1px solid #fdba74; }
            table { width: 100%; border-collapse: collapse; margin-top: 16px; }
            th { text-align: left; padding: 10px; background: #f9fafb; border-bottom: 1px solid #e5e7eb; }
          </style>
        </head>
        <body>
          <h1>Purchase Order ${this.purchaseOrder.poNumber}</h1>
          <p style="margin-top:8px;color:#4b5563;">Linked ticket: ${this.purchaseOrder.ticketHumanReadableId}</p>
          <div class="grid">
            <div class="card"><span class="label">Vendor</span><strong>${this.purchaseOrder.vendor}</strong></div>
            <div class="card"><span class="label">Accountable</span><strong>${this.purchaseOrder.accountableDisplayName}</strong></div>
            <div class="card"><span class="label">Created By</span><strong>${this.purchaseOrder.createdByDisplayName}</strong></div>
            <div class="card"><span class="label">PO Amount</span><strong>${this.formatCurrency(this.purchaseOrder.amount)}</strong></div>
            <div class="card"><span class="label">Used Amount</span><strong>${this.formatCurrency(this.purchaseOrder.usedAmount)}</strong></div>
            <div class="card"><span class="label">Available Amount</span><strong>${this.formatCurrency(this.purchaseOrder.availableAmount)}</strong></div>
          </div>
          <div style="margin-top:20px;">
            <span class="label">Description</span>
            <p style="line-height:1.6;">${this.escapeHtml(this.purchaseOrder.description)}</p>
          </div>
          ${warning ? `<div class="warning">${warning}</div>` : ''}
          <h3 style="margin-top:24px;">Assets in Purchase Order</h3>
          <table>
            <thead>
              <tr>
                <th>Asset Tag</th>
                <th>Serial Number</th>
                <th style="text-align:right;">Price (${this.purchaseOrder.currency})</th>
              </tr>
            </thead>
            <tbody>${itemsHtml || '<tr><td colspan="3" style="padding:12px 10px;">No assets linked yet.</td></tr>'}</tbody>
          </table>
        </body>
      </html>
    `);
    printWindow.document.close();
    printWindow.focus();
    printWindow.print();
  }

  formatCurrency(amount: number): string {
    if (!this.purchaseOrder) {
      return `${amount ?? 0}`;
    }

    return `${(amount ?? 0).toFixed(2)} ${this.purchaseOrder.currency}`;
  }

  hasAmountWarning(): boolean {
    return !!this.getAmountWarning();
  }

  getAmountWarning(): string {
    if (!this.purchaseOrder) {
      return '';
    }

    const difference = Number((this.purchaseOrder.amount - this.purchaseOrder.usedAmount).toFixed(2));
    if (difference === 0) {
      return '';
    }

    if (difference > 0) {
      return `Assets currently use ${this.formatCurrency(this.purchaseOrder.usedAmount)}, leaving ${this.formatCurrency(difference)} unallocated.`;
    }

    return `Assets exceed the PO amount by ${this.formatCurrency(Math.abs(difference))}.`;
  }

  private escapeHtml(value: string): string {
    return (value || '')
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }
}
