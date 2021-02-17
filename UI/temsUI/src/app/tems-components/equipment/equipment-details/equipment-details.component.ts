import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-equipment-details',
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent implements OnInit {

  equipmentId;
  // equipment: ViewEquipment;
  
  constructor(private _Activatedroute:ActivatedRoute) 
  { 
    this.equipmentId=this._Activatedroute.snapshot.paramMap.get("id");
  }

  ngOnInit(): void {
  }

}
