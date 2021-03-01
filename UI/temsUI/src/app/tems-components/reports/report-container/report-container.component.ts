import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-report-container',
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.scss']
})
export class ReportContainerComponent implements OnInit {

  @Input() template;  
  constructor() { }

  ngOnInit(): void {
  }

}
