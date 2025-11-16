import { Component, Input, OnInit, NO_ERRORS_SCHEMA } from '@angular/core';
import { CommonModule, DecimalPipe } from '@angular/common';
import { NgChartsModule } from 'ng2-charts';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { PieChartData } from '../../../models/analytics/pieChart-model';

@Component({
  selector: 'app-piechart-card',
  standalone: true,
  imports: [CommonModule, NgChartsModule, TranslateModule],
  schemas: [NO_ERRORS_SCHEMA],
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
