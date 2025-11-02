import { Component, Input, OnInit } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';

@Component({
  selector: 'app-personnel-details-logs',
  templateUrl: './personnel-details-logs.component.html',
  styleUrls: ['./personnel-details-logs.component.scss']
})
export class PersonnelDetailsLogsComponent implements OnInit {

  @Input() personnel: ViewPersonnelSimplified;
  personnelOption: IOption;

  constructor() { }

  ngOnInit(): void {
    if(this.personnel != undefined)
      this.personnelOption = {
        value: this.personnel.id,
        label: this.personnel.name
      }
  }

}
