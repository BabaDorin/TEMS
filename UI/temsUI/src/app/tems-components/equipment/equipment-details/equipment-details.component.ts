import { Component, ElementRef, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-equipment-details',
  templateUrl: './equipment-details.component.html',
  styleUrls: ['./equipment-details.component.scss']
})
export class EquipmentDetailsComponent implements OnInit {

  @Input() equipmentId;
  edit: boolean;
  // equipment: ViewEquipment;
  

  constructor(private _Activatedroute: ActivatedRoute, private elementRef: ElementRef) {
    if(this.equipmentId == undefined)
      this.equipmentId = this._Activatedroute.snapshot.paramMap.get("id");
    this.edit=false;
  }

  ngOnInit(): void {
  }

  // // To avoid generating multiple components inside mat-tab (https://github.com/angular/components/issues/10938) 
  // public onTabAnimationDone(): void {
  //   const inactiveTabs = this.elementRef.nativeElement.querySelectorAll(
  //     '.mat-tab-body-active .mat-tab-body-content > .tab-container:not(:first-child)'
  //   );

  //   inactiveTabs.forEach(tab => tab.remove());
  // }
}
