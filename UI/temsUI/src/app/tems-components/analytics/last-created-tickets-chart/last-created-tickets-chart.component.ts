import { DatePipe } from '@angular/common';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from './../../../services/snack/snack.service';
import { AnalyticsService } from 'src/app/services/analytics-service/analytics.service';
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
  rates: PieChartData;

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

  areaChartColors = [
    {
      borderColor: 'rgba(255,99,132,1)',
      backgroundColor: 'rgba(255,99,132,.2)'
    }
  ];

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    private datePipe: DatePipe
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.analyticsService.getAmountOfLastCreatedTickets(this.start, this.end, this.interval)
      .subscribe(result => {
        console.log(result);
        if(this.snackService.snackIfError(result))
          return;
        
        this.rates = result;
        this.createChartData();    
      })
    )
  }

  createChartData(){
    if(this.rates == undefined)
      return;
    
    this.areaChartData = [{
      label: '# of created tickets',
      data: this.rates.rates.map(q => q.item2),
      borderWidth: 1,
      fill: false,
    }];

    // this.areaChartLabels = this.rates.rates.map(q => 
    //   this.datePipe.transform(new Date(q.item1), "MMM d, y"));

    let date = new Date(this.rates.rates[0].item1);
    console.log('date:');
    console.log(date);
    this.areaChartLabels = this.rates.rates.map(q => q.item1); 
  }
}
