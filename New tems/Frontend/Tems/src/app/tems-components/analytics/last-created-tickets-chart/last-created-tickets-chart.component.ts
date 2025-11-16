import { TranslateService } from '@ngx-translate/core';
import { DatePipe } from '@angular/common';
import { Component, Input, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { AnalyticsService } from 'src/app/services/analytics.service';
import { SnackService } from '../../../services/snack.service';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';
import { CommonModule } from '@angular/common';
import { NgChartsModule } from 'ng2-charts';

@Component({
  selector: 'app-last-created-tickets-chart',
  standalone: true,
  imports: [CommonModule, NgChartsModule],
  schemas: [NO_ERRORS_SCHEMA],
  templateUrl: './last-created-tickets-chart.component.html',
  styleUrls: ['./last-created-tickets-chart.component.scss']
})
export class LastCreatedTicketsChartComponent extends TEMSComponent implements OnInit {

  @Input() end: Date = new Date();
  @Input() start: Date = new Date(new Date().setDate(new Date().getDate() - 10));
  @Input() interval: string = "daily";
  @Input() showCreatedIssues = true;
  @Input() showClosedIssues = true;

  ratesCreated: PieChartData;
  ratesClosed: PieChartData;

  areaChartData = [];
  areaChartLabels = [];
  areaChartOptions = {
    scales: {
      yAxes: [{
          display: true,
          ticks: {
            suggestedMin: 0,
            stepSize: 1,
          },
      }]
    }
  };

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    private translate: TranslateService
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.showCreatedIssues)
      this.subscriptions.push(
        this.analyticsService.getAmountOfLastCreatedTickets(this.start, this.end, this.interval)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          
          this.ratesCreated = result;
          this.createChartData();    
        })
      );

    if(this.showClosedIssues)
      this.subscriptions.push(
        this.analyticsService.getAmountOfLastClosedTickets(this.start, this.end, this.interval)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          
          this.ratesClosed = result;
          this.createChartData();    
        })
      )
  }

  createChartData(){
    this.areaChartData = [];
    if(this.showCreatedIssues && this.ratesCreated != undefined){
      this.areaChartData.push(
        {
          label: this.translate.instant('analytics.labels.tickets created'),
          data: this.ratesCreated.rates.map(q => q.item2),
          borderWidth: 1,
          fill: false,
        }
      )
    }

    if(this.showClosedIssues && this.ratesClosed != undefined){
      this.areaChartData.push(
        {
          label:  this.translate.instant('analytics.labels.tickets closed'),
          data: this.ratesClosed.rates.map(q => q.item2),
          borderWidth: 1,
          fill: false,
        }
      )
    }

    let labels =  this.ratesCreated?.rates ?? this.ratesCreated?.rates;
    if(labels != undefined)
      this.areaChartLabels = labels.map(q => q.item1); 
  }
}
