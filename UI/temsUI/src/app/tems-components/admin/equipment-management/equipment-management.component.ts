import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { ClaimService } from './../../../services/claim.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit } from '@angular/core';
import { TokenService } from '../../../services/token.service';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../../models/claims';

@Component({
  selector: 'app-equipment-management',
  templateUrl: './equipment-management.component.html',
  styleUrls: ['./equipment-management.component.scss']
})
export class EquipmentManagementComponent implements OnInit {

  matTabLazyLoader = new MatTabLazyLoader(3);

  constructor(
    public claims: ClaimService,
    public translate: TranslateService
  ) {
  }

  ngOnInit(): void {
  }
}
