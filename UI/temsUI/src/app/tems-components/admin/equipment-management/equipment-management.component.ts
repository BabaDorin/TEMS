import { CAN_MANAGE_SYSTEM_CONFIGURATION } from './../../../models/claims';
import { TokenService } from '../../../services/token.service';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-management',
  templateUrl: './equipment-management.component.html',
  styleUrls: ['./equipment-management.component.scss']
})
export class EquipmentManagementComponent implements OnInit {

  canManage: boolean;

  constructor(
    private tokenService: TokenService
  ) {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
  }

  ngOnInit(): void {
  }

}
