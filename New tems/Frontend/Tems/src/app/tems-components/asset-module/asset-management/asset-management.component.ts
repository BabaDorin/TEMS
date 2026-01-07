import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AssetPropertyManagementComponent } from './asset-property-management/asset-property-management.component';
import { AssetTypeManagementComponent } from './asset-type-management/asset-type-management.component';
import { AssetDefinitionManagementComponent } from './asset-definition-management/asset-definition-management.component';

@Component({
  selector: 'app-asset-management',
  standalone: true,
  imports: [
    CommonModule,
    AssetPropertyManagementComponent,
    AssetTypeManagementComponent,
    AssetDefinitionManagementComponent
  ],
  templateUrl: './asset-management.component.html',
  styleUrls: ['./asset-management.component.scss']
})
export class AssetManagementComponent {
  activeTab: 'properties' | 'types' | 'definitions' = 'properties';

  setActiveTab(tab: 'properties' | 'types' | 'definitions') {
    this.activeTab = tab;
  }
}
