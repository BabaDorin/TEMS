import { Component, Input, OnInit } from '@angular/core';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';

@Component({
  selector: 'app-personnel-details-logs',
  templateUrl: './personnel-details-logs.component.html',
  styleUrls: ['./personnel-details-logs.component.scss']
})
export class PersonnelDetailsLogsComponent implements OnInit {

  @Input() personnel: ViewPersonnelSimplified;
  constructor() { }

  ngOnInit(): void {
  }

}
