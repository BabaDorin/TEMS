import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { ViewPersonnel } from './../../../models/personnel/view-personnel.model';
import { Component, Input, OnInit } from '@angular/core';
import { Property } from 'src/app/models/equipment/view-property.model';

@Component({
  selector: 'app-personnel-details-general',
  templateUrl: './personnel-details-general.component.html',
  styleUrls: ['./personnel-details-general.component.scss']
})
export class PersonnelDetailsGeneralComponent implements OnInit {

  @Input() personnelId;

  personnel: ViewPersonnel;
  personnelProperties: Property[];
  edit: boolean = false;

  constructor(
    private personnelService: PersonnelService
  ) { }

  ngOnInit(): void {
    this.personnel = this.personnelService.getPersonnelById(this.personnelId);

    this.personnelProperties = [
      { displayName: 'Name', value: this.personnel.name },
      { displayName: 'Position', value: this.personnel.position },
      { displayName: 'Phone Number', value: this.personnel.phoneNumber },
      { displayName: 'Email', value: this.personnel.email },
    ]
  }

}
