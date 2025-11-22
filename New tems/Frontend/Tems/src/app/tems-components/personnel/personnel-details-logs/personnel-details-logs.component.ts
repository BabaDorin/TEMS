import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { EntityLogsListComponent } from '../../entity-logs-list/entity-logs-list.component';

@Component({
  selector: 'app-personnel-details-logs',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    EntityLogsListComponent
  ],
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
