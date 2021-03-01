import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-personnel-details',
  templateUrl: './personnel-details.component.html',
  styleUrls: ['./personnel-details.component.scss']
})
export class PersonnelDetailsComponent implements OnInit {

  @Input() personnelId;
  edit: boolean = false;
  personnelSimplified: ViewPersonnelSimplified;

  constructor(
    private activatedroute: ActivatedRoute,
    private personnelService: PersonnelService
  ) { 

  }

  ngOnInit(): void {
    if(this.personnelId == undefined)
      this.personnelId = this.activatedroute.snapshot.paramMap.get("id");
    this.edit=false;

    this.personnelSimplified = this.personnelService.getPersonnelSimplified(this.personnelId);
  }
}
