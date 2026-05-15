import { CommonModule } from '@angular/common';
import { Component, Inject, OnInit } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { Router } from '@angular/router';
import { Asset } from 'src/app/models/asset/asset.model';
import { AssetService } from 'src/app/services/asset.service';

export interface AssetPreviewModalData {
  assetId: string;
}

@Component({
  selector: 'app-asset-preview-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule],
  templateUrl: './asset-preview-modal.component.html'
})
export class AssetPreviewModalComponent implements OnInit {
  asset: Asset | null = null;
  loading = true;
  error: string | null = null;

  constructor(
    @Inject(MAT_DIALOG_DATA) public data: AssetPreviewModalData,
    private dialogRef: MatDialogRef<AssetPreviewModalComponent>,
    private assetService: AssetService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.assetService.getById(this.data.assetId).subscribe({
      next: (asset) => {
        this.asset = asset;
        this.loading = false;
      },
      error: () => {
        this.error = 'Failed to load asset preview';
        this.loading = false;
      }
    });
  }

  close(): void {
    this.dialogRef.close();
  }

  openFullDetails(): void {
    if (!this.asset) {
      return;
    }

    this.dialogRef.close();
    this.router.navigate(['/assets', this.asset.id]);
  }

  getLocation(): string {
    const fullPath = this.asset?.locationDetails?.fullPath?.trim();
    if (fullPath) {
      return fullPath;
    }

    const parts = [
      this.asset?.location?.building,
      this.asset?.location?.floor,
      this.asset?.location?.room
    ].filter((value) => !!value);

    return parts.length > 0 ? parts.join(' / ') : '—';
  }

  getAssignee(): string {
    return this.asset?.assignment?.assignedToUserName || this.asset?.assignment?.assignedBy || '—';
  }

  getSpecifications(): Array<{ key: string; value: string }> {
    return (this.asset?.definition?.specifications || []).map((spec) => ({
      key: spec.name || spec.propertyId,
      value: this.formatSpecValue(spec.value, spec.unit)
    }));
  }

  private formatSpecValue(value: unknown, unit?: string): string {
    if (value === null || value === undefined || value === '') {
      return '—';
    }

    const baseValue = typeof value === 'boolean' ? (value ? 'Yes' : 'No') : String(value);
    return unit ? `${baseValue} ${unit}` : baseValue;
  }
}
