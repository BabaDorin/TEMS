import { Component, OnInit } from '@angular/core';
import { ClaimService } from './../../services/claim.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashboardComponent implements OnInit {

  toggleProBanner(event) {
    event.preventDefault();
    document.querySelector('body').classList.toggle('removeProbanner');
  }

  constructor(
    public claims: ClaimService
  ) { 
  }

  ngOnInit() {
  }
}
