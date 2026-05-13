import { CommonModule } from '@angular/common';
import { Component, Inject, Optional } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { AssetService } from 'src/app/services/asset.service';
import { CustomSelectComponent, SelectOption } from 'src/app/shared/custom-select/custom-select.component';

type AssetHistoryLogType = 'Maintenance log' | 'Component replacement' | 'Other';
type AssetHistoryLogCurrency = 'MDL' | 'EUR' | 'USD';

@Component({
  selector: 'app-asset-history-log-modal',
  standalone: true,
  imports: [CommonModule, FormsModule, MatDialogModule, CustomSelectComponent],
  templateUrl: './asset-history-log-modal.component.html'
})
export class AssetHistoryLogModalComponent {
  readonly typeOptions: SelectOption[] = [
    { value: 'Maintenance log', label: 'Maintenance log' },
    { value: 'Component replacement', label: 'Component replacement' },
    { value: 'Other', label: 'Other' }
  ];

  readonly currencyOptions: SelectOption[] = [
    { value: 'MDL', label: 'MDL' },
    { value: 'EUR', label: 'EUR' },
    { value: 'USD', label: 'USD' }
  ];

  selectedType: AssetHistoryLogType = 'Maintenance log';
  description = '';
  costIncluded = false;
  costAmount: number | null = null;
  costCurrency: AssetHistoryLogCurrency = 'MDL';
  isSaving = false;
  error: string | null = null;

  constructor(
    private assetService: AssetService,
    @Optional() public dialogRef: MatDialogRef<AssetHistoryLogModalComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: { assetId: string; assetTag?: string }
  ) {}

  get remainingChars(): number {
    return Math.max(0, 400 - this.description.length);
  }

  canSave(): boolean {
    const trimmedDescription = this.description.trim();
    if (!this.selectedType || trimmedDescription.length === 0 || trimmedDescription.length > 400) {
      return false;
    }

    if (!this.costIncluded) {
      return true;
    }

    return this.costAmount !== null && this.costAmount > 0 && !!this.costCurrency;
  }

  close(): void {
    this.dialogRef?.close();
  }

  onCostIncludedChange(): void {
    if (!this.costIncluded) {
      this.costAmount = null;
      this.costCurrency = 'MDL';
    }
  }

  save(): void {
    if (!this.data?.assetId || !this.canSave() || this.isSaving) {
      return;
    }

    this.isSaving = true;
    this.error = null;

    this.assetService.addHistoryLog(this.data.assetId, {
      type: this.selectedType,
      description: this.description.trim(),
      costIncluded: this.costIncluded,
      costAmount: this.costIncluded ? this.costAmount : null,
      costCurrency: this.costIncluded ? this.costCurrency : null
    }).subscribe({
      next: (response) => {
        this.isSaving = false;
        if (response?.success) {
          this.dialogRef?.close({ success: true });
          return;
        }

        this.error = 'Failed to save the log entry.';
      },
      error: (error) => {
        console.error('Error saving asset history log:', error);
        this.isSaving = false;
        this.error = 'Failed to save the log entry. Please try again.';
      }
    });
  }
}
