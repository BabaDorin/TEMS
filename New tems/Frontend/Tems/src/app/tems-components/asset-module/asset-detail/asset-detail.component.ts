import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { AssetService } from 'src/app/services/asset.service';
import { Asset } from 'src/app/models/asset/asset.model';
import { AssetLabelComponent } from '../../asset/asset-label/asset-label.component';

@Component({
  selector: 'app-asset-detail',
  standalone: true,
  imports: [CommonModule, AssetLabelComponent],
  templateUrl: './asset-detail.component.html',
  styleUrls: ['./asset-detail.component.scss']
})
export class AssetDetailComponent implements OnInit {
  asset: Asset | null = null;
  loading = true;
  error: string | null = null;
  activeTab: 'overview' | 'specifications' | 'purchase' | 'maintenance' | 'history' = 'overview';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private assetService: AssetService
  ) {}

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.loadAsset(id);
    } else {
      this.error = 'No asset ID provided';
      this.loading = false;
    }
  }

  loadAsset(id: string) {
    this.loading = true;
    this.error = null;
    
    this.assetService.getById(id).subscribe({
      next: (asset) => {
        this.asset = asset;
        this.loading = false;
      },
      error: (error) => {
        console.error('Error loading asset:', error);
        this.error = 'Failed to load asset details';
        this.loading = false;
      }
    });
  }

  goBack() {
    this.router.navigate(['/assets/view']);
  }

  editAsset() {
    // Future: Navigate to edit view
    console.log('Edit asset:', this.asset?.id);
  }

  deleteAsset() {
    if (!this.asset) return;
    
    if (confirm(`Are you sure you want to delete asset ${this.asset.assetTag}?`)) {
      this.assetService.delete(this.asset.id).subscribe({
        next: () => {
          this.router.navigate(['/assets/view']);
        },
        error: (error) => {
          console.error('Error deleting asset:', error);
          alert('Failed to delete asset');
        }
      });
    }
  }

  getSpecificationsArray(): { key: string; value: string }[] {
    if (!this.asset?.definition?.specifications) return [];
    return this.asset.definition.specifications.map(spec => ({
      key: spec.name || spec.propertyId,
      value: this.formatSpecValue(spec.value, spec.unit)
    }));
  }

  formatSpecValue(value: any, unit?: string): string {
    if (value === null || value === undefined) return '—';
    const stringValue = typeof value === 'boolean' ? (value ? 'Yes' : 'No') : String(value);
    return unit ? `${stringValue} ${unit}` : stringValue;
  }

  getStatusClass(status: string): string {
    switch (status) {
      case 'AVAILABLE':
        return 'bg-green-100 text-green-800';
      case 'IN_USE':
        return 'bg-blue-100 text-blue-800';
      case 'UNDER_MAINTENANCE':
        return 'bg-yellow-100 text-yellow-800';
      case 'RETIRED':
        return 'bg-red-100 text-red-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'AVAILABLE':
        return 'Available';
      case 'IN_USE':
        return 'In Use';
      case 'UNDER_MAINTENANCE':
        return 'Under Maintenance';
      case 'RETIRED':
        return 'Retired';
      default:
        return status;
    }
  }
}
