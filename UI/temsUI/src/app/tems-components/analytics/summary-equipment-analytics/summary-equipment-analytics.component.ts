import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from './../../../services/snack/snack.service';
import { AnalyticsService } from './../../../services/analytics-service/analytics.service';
import { Component, Input, OnInit } from '@angular/core';
import { CurrencyPipe } from '@angular/common';

@Component({
  selector: 'app-summary-equipment-analytics',
  templateUrl: './summary-equipment-analytics.component.html',
  styleUrls: ['./summary-equipment-analytics.component.scss']
})
export class SummaryEquipmentAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() roomId: string;
  @Input() personnelId: string;

  equipmentTotalAmount: number;
  equipmentTotalCost: string;
  equipmentUtilizationRate: PieChartData;
  equipmentTypeRate: PieChartData;
  equipmentAllocationRate: PieChartData;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    private cp: CurrencyPipe
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
        this.equipmentTotalCost = this.cp.transform(result, 'MDL ');
      })
    )  
  }

  getEquipmentUtilizationRate(){
    let serviceMethod = this.analyticsService.getEquipmentUtilizationRate(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentUtilizationRate = result;

        console.log(result);
      })
    )  
  }

  getEquipmentTypeRate(){
    let serviceMethod = this.analyticsService.getEquipmentTypeRate(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentTypeRate = result;
      })
    )  
  }

  getEquipmentAllocationRate(){
    let serviceMethod = this.analyticsService.getEquipmentAllocationRate(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentAllocationRate = result;

        console.log(result);
      })
    )  
  }

  getEquipmentWorkabilityRate(){
    let serviceMethod = this.analyticsService.getEquipmentWorkabilityRate(
      this.getEntityType(),
      this.getEntityId()
    );

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.equipmentAllocationRate = result;

        console.log(result);
      })
    )  
  }

  ngOnInit(): void {
    this.getEquipmentAmount();
    this.getEquipmentTotalCost();
    this.getEquipmentUtilizationRate();
    this.getEquipmentTypeRate();
    this.getEquipmentAllocationRate();
    this.getEquipmentWorkabilityRate();
  }
}
