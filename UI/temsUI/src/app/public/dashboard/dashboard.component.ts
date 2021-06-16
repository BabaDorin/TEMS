import { CAN_VIEW_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { Component, OnInit } from '@angular/core';
import { CAN_MANAGE_ENTITIES } from 'src/app/models/claims';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {

  canManage: boolean = false;
  canView: boolean = false;

  toggleProBanner(event) {
    event.preventDefault();
    document.querySelector('body').classList.toggle('removeProbanner');
  }

  constructor(
    private tokenService: TokenService
  ) { 
    this.canManage = tokenService.hasClaim(CAN_MANAGE_ENTITIES);
    this.canView = tokenService.hasClaim(CAN_VIEW_ENTITIES);
  }

  ngOnInit() {
  }
}
