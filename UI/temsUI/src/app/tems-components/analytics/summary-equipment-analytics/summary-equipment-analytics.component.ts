import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from './../../../services/snack/snack.service';
import { AnalyticsService } from './../../../services/analytics-service/analytics.service';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-summary-equipment-analytics',
  templateUrl: './summary-equipment-analytics.component.html',
  styleUrls: ['./summary-equipment-analytics.component.scss']
})
export class SummaryEquipmentAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() roomId: string;
  @Input() personnelId: string;

  equipmentTotalAmount: number;
  equipmentTotalCost: number;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService
  ) {
    super();
  }

  getEntityType(){
    if(this.roomId != undefined) return "room";
    if(this.personnelId != undefined) return "personnel";
    return undefined;
  }

  getEntityId(){
    return this.roomId ?? this.personnelId;
  }

  getEquipmentAmount(){
    let serviceMethod = this.analyticsService.getEquipmentAmount(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentTotalAmount = result;
      })
    )  
  }

  getEquipmentTotalCost(){
    let serviceMethod = this.analyticsService.getEquipmentTotalCost(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentTotalCost = result;
      })
    )  
  }

  ngOnInit(): void {
    this.getEquipmentAmount();
    this.getEquipmentTotalCost();
  }
}
