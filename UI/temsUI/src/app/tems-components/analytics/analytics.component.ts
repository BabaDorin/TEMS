import { Component, OnInit } from '@angular/core';
import { AnalyticsService } from 'src/app/services/analytics.service';


@Component({
  selector: 'app-analytics',
  templateUrl: './analytics.component.html',
  styleUrls: ['./analytics.component.scss'],
  providers: [ AnalyticsService ]
})
export class AnalyticsComponent implements OnInit {

  constructor(private analyticsService: AnalyticsService) { }

  ngOnInit(): void {
  }
}
