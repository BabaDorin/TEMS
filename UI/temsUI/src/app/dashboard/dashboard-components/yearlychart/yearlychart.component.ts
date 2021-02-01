import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-yearlychart',
  templateUrl: './yearlychart.component.html',
  styleUrls: ['./yearlychart.component.css']
})
export class YearlychartComponent implements OnInit {

  public lineChartLegend = false;
  public lineChartType = 'line';

  constructor() { }

  ngOnInit(): void {
  }

  // lineChart
  public lineChartData: Array<any> = [
    { data: [5, 2, 7, 4, 5, 3, 5, 4], label: 'Iphone' },
    { data: [2, 5, 2, 6, 2, 5, 2, 4], label: 'Ipad' }
  ];

  public lineChartLabels: Array<string> = [
    '1',
    '2',
    '3',
    '4',
    '5',
    '6',
    '7',
    '8',
  ];

  public lineChartOptions = {
    responsive: true,
    maintainAspectRatio: false
  };

  public lineChartColors: Array<Object> = [
    {
      // grey
      backgroundColor: 'rgba(41, 98, 255,0.1)',
      borderColor: '#98a6ad',
      pointBackgroundColor: '#98a6ad',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: '#98a6ad'
    },
    {
      // dark grey
      backgroundColor: 'rgba(116, 96, 238,0.1)',
      borderColor: '#2cabe3',
      pointBackgroundColor: '#2cabe3',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: '#2cabe3'
    }
  ];



}
