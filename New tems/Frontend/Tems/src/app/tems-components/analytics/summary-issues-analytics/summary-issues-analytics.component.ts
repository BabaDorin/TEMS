import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { AnalyticsService } from '../../../services/analytics.service';
import { SnackService } from '../../../services/snack.service';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { FractionCardComponent } from '../fraction-card/fraction-card.component';
import { PiechartCardComponent } from '../piechart-card/piechart-card.component';


@Component({
  selector: 'app-summary-issues-analytics',
  standalone: true,
  imports: [CommonModule, MatCardModule, TranslateModule, FractionCardComponent, PiechartCardComponent],
  templateUrl: './summary-issues-analytics.component.html',
  styleUrls: ['./summary-issues-analytics.component.scss']
})
export class SummaryIssuesAnalyticsComponent implements OnInit {

  @Input() assetId;
  @Input() roomId;
  @Input() personnelId;

  closingRate: PieChartData;
  closingByRate: PieChartData;
  openTicketStatusRate: PieChartData;
  openTickets: number;
  totalTickets: number;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    public translate: TranslateService
  ) {}

  private getEntityType(){
    if(this.roomId != undefined) return "room";
    if(this.personnelId != undefined) return "personnel";
    if(this.assetId != undefined) return "asset";
    return undefined;
  }

  private getEntityId(){
    return this.roomId ?? this.personnelId ?? this.assetId;
  }

  ngOnInit(): void {
    this.getTicketClosingRate();
    this.getOpenTicketStatusRate();
    this.getTicketClosingByRate();
    this.getTotalTickets();
    this.getOpenTickets();
  }

  getTotalTickets(){
    this.analyticsService.getAmountOfCreatedIssues(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.totalTickets = result;
      })
  }

  getOpenTickets(){
    this.analyticsService.getAmountOfOpenTickets(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.openTickets = result;
      })
  }

  getTicketClosingRate(){
    this.analyticsService.getTicketClosingRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.closingRate = result;
      })
  }

  getTicketClosingByRate(){
    this.analyticsService.getTicketClosingByRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.closingByRate = result;
      })
  }

  getOpenTicketStatusRate(){
    this.analyticsService.getOpenTicketStatusRate(this.getEntityType(), this.getEntityId())
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.openTicketStatusRate = result;
      })
  }
}
