import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AnalyticsService } from '../../../services/analytics.service';
import { SnackService } from '../../../services/snack.service';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-summary-equipment-analytics',
  templateUrl: './summary-equipment-analytics.component.html',
  styleUrls: ['./summary-equipment-analytics.component.scss']
})
export class SummaryEquipmentAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() roomId: string;
  @Input() personnelId: string;

  prepared: boolean = false;
  equipmentTotalAmount: number;
  equipmentTotalCost: string;
  equipmentUtilizationRate: PieChartData;
  equipmentTypeRate: PieChartData;
  equipmentAllocationRate: PieChartData;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    public translate: TranslateService,
    private cp: CurrencyPipe,
  ) {
    super();
  }

  ngOnInit(): void {
    this.getEquipmentAmount();
    this.getEquipmentTotalCost();
    this.getEquipmentUtilizationRate();
    this.getEquipmentTypeRate();
    this.getEquipmentAllocationRate();
    // this.getEquipmentWorkabilityRate();
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

  // getEquipmentWorkabilityRate(){
  //   let serviceMethod = this.analyticsService.getEquipmentWorkabilityRate(
  //     this.getEntityType(),
  //     this.getEntityId()
  //   );

  //   this.subscriptions.push(
  //     serviceMethod
  //     .subscribe(result => {
  //       if(this.snackService.snackIfError(result))
  //         return;
  //       this.equipmentwork = result;

  //       console.log(result);
  //     })
  //   )  
  // }
}
