import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { ClaimService } from './../../../services/claim.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { ManageTypesComponent } from './manage-types/manage-types.component';
import { ManagePropertiesComponent } from './manage-properties/manage-properties.component';
import { ManageDefinitionsComponent } from './manage-definitions/manage-definitions.component';

@Component({
  selector: 'app-asset-management',
  standalone: true,
  imports: [
    CommonModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    TranslateModule,
    ManageTypesComponent,
    ManagePropertiesComponent,
    ManageDefinitionsComponent
  ],
  templateUrl: './asset-management.component.html',
  styleUrls: ['./asset-management.component.scss']
})
export class AssetManagementComponent implements OnInit {

  matTabLazyLoader = new MatTabLazyLoader(3);

  constructor(
    public claims: ClaimService,
    public translate: TranslateService
  ) {
  }

  ngOnInit(): void {
  }
}
