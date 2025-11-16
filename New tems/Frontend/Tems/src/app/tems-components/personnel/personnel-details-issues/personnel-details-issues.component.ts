import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { IOption } from 'src/app/models/option.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { SummaryIssuesAnalyticsComponent } from '../../analytics/summary-issues-analytics/summary-issues-analytics.component';
import { EntityIssuesListComponent } from '../../entity-issues-list/entity-issues-list.component';

@Component({
  selector: 'app-personnel-details-issues',
  standalone: true,
  imports: [
    CommonModule,
    SummaryIssuesAnalyticsComponent,
    EntityIssuesListComponent
  ],
  templateUrl: './personnel-details-issues.component.html',
  styleUrls: ['./personnel-details-issues.component.scss']
})
export class PersonnelDetailsIssuesComponent implements OnInit {

  @Input() personnel : ViewPersonnelSimplified;
  personnelAlreadySelected: IOption[];

  constructor() { }

  ngOnInit(): void {
    this.personnelAlreadySelected = [{
      value: this.personnel.id,
      label: this.personnel.name,
    }];
  }
}
