import { Component, OnInit } from '@angular/core';
import { RecentSale, recentSales } from './recent-table-data';

@Component({
  selector: 'app-recent-table',
  templateUrl: './recent-table.component.html',
  styleUrls: ['./recent-table.component.css']
})
export class RecentTableComponent implements OnInit {

  tableData: RecentSale[];

  constructor() {
    this.tableData = recentSales;
    console.log(this.tableData[4].Date.toDateString());
  }

  ngOnInit(): void {
  }

}
