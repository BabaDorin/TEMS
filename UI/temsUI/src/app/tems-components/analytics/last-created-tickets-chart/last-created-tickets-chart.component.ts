import { DatePipe } from '@angular/common';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from '../../../services/snack.service';
import { AnalyticsService } from 'src/app/services/analytics.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-last-created-tickets-chart',
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
    private datePipe: DatePipe
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.showCreatedIssues)
      this.subscriptions.push(
        this.analyticsService.getAmountOfLastCreatedTickets(this.start, this.end, this.interval)
        .subscribe(result => {
          console.log(result);
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
          label: '# of created tickets',
          data: this.ratesCreated.rates.map(q => q.item2),
          borderWidth: 1,
          fill: false,
        }
      )
    }

    if(this.showClosedIssues && this.ratesClosed != undefined){
      this.areaChartData.push(
        {
          label: '# of closed tickets',
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
