import { SnackService } from './../../../services/snack/snack.service';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { AnalyticsService } from './../../../services/analytics-service/analytics.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-summary-issues-analytics',
  templateUrl: './summary-issues-analytics.component.html',
  styleUrls: ['./summary-issues-analytics.component.scss']
})
export class SummaryIssuesAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() equipmentId;
  @Input() roomId;
  @Input() personnelId;

  closingRate: PieChartData;
  closingByRate: PieChartData;
  openTicketStatusRate: PieChartData;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService
  ) {
    super();
  }

  private getEntityType(){
    if(this.roomId != undefined) return "room";
    if(this.personnelId != undefined) return "personnel";
    if(this.equipmentId != undefined) return "equipment";
    return undefined;
  }

  private getEntityId(){
    return this.roomId ?? this.personnelId ?? this.equipmentId;
  }

  ngOnInit(): void {
    this.getTicketClosingRate();
    this.getOpenTicketStatusRate();
    this.getTicketClosingByRate();
  }

  getTicketClosingRate(){
    this.subscriptions.push(
      this.analyticsService.getTicketClosingRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;

        this.closingRate = result;
      })
    )
  }

  getTicketClosingByRate(){
    this.subscriptions.push(
      this.analyticsService.getTicketClosingByRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;

        this.closingByRate = result;
      })
    )
  }

  getOpenTicketStatusRate(){
    this.subscriptions.push(
      this.analyticsService.getOpenTicketStatusRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;

        this.openTicketStatusRate = result;
      })
    )
  }
}
