import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-personnel-details-issues',
  templateUrl: './personnel-details-issues.component.html',
  styleUrls: ['./personnel-details-issues.component.scss']
})
export class PersonnelDetailsIssuesComponent implements OnInit {

  @Input() personnel : ViewPersonnelSimplified;
  constructor() { }

  ngOnInit(): void {
  }
}
