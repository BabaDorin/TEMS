import { CurrencyPipe } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { AnalyticsService } from '../../../services/analytics.service';
import { SnackService } from '../../../services/snack.service';
import { PieChartData } from './../../../models/analytics/pieChart-model';
import { TEMSComponent } from './../../../tems/tems.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { PiechartCardComponent } from '../piechart-card/piechart-card.component';
import { SimpleInfoCardComponent } from '../simple-info-card/simple-info-card.component';

@Component({
  selector: 'app-summary-asset-analytics',
  standalone: true,
  imports: [CommonModule, MatCardModule, TranslateModule, PiechartCardComponent, SimpleInfoCardComponent],
  templateUrl: './summary-asset-analytics.component.html',
  styleUrls: ['./summary-asset-analytics.component.scss']
})
export class SummaryAssetAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() roomId: string;
  @Input() personnelId: string;

  prepared: boolean = false;
  assetTotalAmount: number;
  assetTotalCost: string;
  assetUtilizationRate: PieChartData;
  assetTypeRate: PieChartData;
  assetAllocationRate: PieChartData;

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
        this.assetTotalAmount = result;
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
        this.assetTotalCost = this.cp.transform(result, 'MDL ');
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
        this.assetUtilizationRate = result;
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
        this.assetTypeRate = result;
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
        this.assetAllocationRate = result;
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

  //     })
  //   )  
  // }
}
