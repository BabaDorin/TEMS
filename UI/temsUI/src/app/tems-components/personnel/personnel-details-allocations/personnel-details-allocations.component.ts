import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-personnel-details-allocations',
  templateUrl: './personnel-details-allocations.component.html',
  styleUrls: ['./personnel-details-allocations.component.scss']
})
export class PersonnelDetailsAllocationsComponent implements OnInit {

  @Input() personnel: ViewPersonnelSimplified;
  personnelParameter;
  constructor() { }

  ngOnInit(): void {
    this.personnelParameter = [this.personnel.id];
  }

}
