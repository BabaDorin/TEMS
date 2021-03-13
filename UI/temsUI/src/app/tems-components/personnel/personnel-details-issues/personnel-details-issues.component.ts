import { IOption } from 'src/app/models/option.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-personnel-details-issues',
  templateUrl: './personnel-details-issues.component.html',
  styleUrls: ['./personnel-details-issues.component.scss']
})
export class PersonnelDetailsIssuesComponent implements OnInit {

  @Input() personnel : ViewPersonnelSimplified;
  personnelAlreadySelected: IOption;

  constructor() { }

  ngOnInit(): void {
    this.personnelAlreadySelected = {
      value: this.personnel.id,
      label: this.personnel.name,
    }
  }
}
