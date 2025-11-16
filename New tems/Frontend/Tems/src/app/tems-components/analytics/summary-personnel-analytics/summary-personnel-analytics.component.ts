import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-summary-personnel-analytics',
  standalone: true,
  imports: [CommonModule, MatCardModule, TranslateModule],
  templateUrl: './summary-personnel-analytics.component.html',
  styleUrls: ['./summary-personnel-analytics.component.scss']
})
export class SummaryPersonnelAnalyticsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }

}
