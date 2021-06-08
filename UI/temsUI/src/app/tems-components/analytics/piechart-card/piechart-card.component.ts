import { PieChartData } from './../../../models/analytics/pieChart-model';
import { Component, Input, OnInit, OnChanges, SimpleChanges } from '@angular/core';

@Component({
  selector: 'app-piechart-card',
  templateUrl: './piechart-card.component.html',
  styleUrls: ['./piechart-card.component.scss']
})
export class PiechartCardComponent implements OnInit{

  @Input() title: string;
  @Input() rates: PieChartData;
  tuple: [string, number];

  chartData;
  chartLabels;
  total = 0;
  displayRates;

  chartOptions = {
    responsive: true,
    animation: {
      animateScale: true,
      animateRotate: true
    },
    legend: false,
  };

  constructor() { }

  ngOnInit(): void {
    this.buildChartData();
  }

  buildChartData(){
    if(this.rates == undefined)
      return;

    this.total = 0;

    this.rates.rates.forEach(rate => {
      this.total += rate.item2;
    });

    this.chartData = [
      {
        data: this.rates.rates.map(q => q.item2),
      }
    ];

    this.chartLabels = this.rates.rates.map(q => q.item1);
    this.displayRates = this.rates.rates.sort(q => q.item2).slice(0, 3);

    console.log(this.chartData);
    console.log(this.chartLabels);
  }

  // trafficChartColors = [
  //   {
  //     backgroundColor: [
  //       'rgba(177, 148, 250, 1)',
  //       'rgba(254, 112, 150, 1)',
  //       'rgba(132, 217, 210, 1)'
  //     ],
  //     borderColor: [
  //       'rgba(177, 148, 250, .2)',
  //       'rgba(254, 112, 150, .2)',
  //       'rgba(132, 217, 210, .2)'
  //     ]
  //   }
  // ];

}
