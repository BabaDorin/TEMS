import { TranslateService } from '@ngx-translate/core';
import { Component, Input, OnInit } from '@angular/core';
import { PieChartData } from './../../../models/analytics/pieChart-model';

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

  constructor(private translate: TranslateService) {
  }

  ngOnInit(): void {
    this.rates.rates.forEach(q => {
      let translateQuery = 'analytics.labels.' + q.item1;
      let translatedValue = this.translate.instant(translateQuery);
      if(translatedValue != translateQuery)
        q.item1 = translatedValue;
    })
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
  }

  trafficChartColors = [
    {
      backgroundColor: [
        '#41d3ed',
        '#5649e0',
        '#cd99f4',
        '#4587F7',
        '#F01EE8',
      ],
    }
  ];
}
