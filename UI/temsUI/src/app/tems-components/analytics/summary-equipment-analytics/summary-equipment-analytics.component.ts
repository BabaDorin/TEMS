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

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService
  ) {
    super();
  }

  getEquipmentAmount(){
    let serviceMethod = this.analyticsService.getEquipmentAmount();
    
    if(this.roomId != undefined)
      serviceMethod = this.analyticsService.getEquipmentAmount('room', this.roomId);
      
    if(this.personnelId != undefined)
      serviceMethod = this.analyticsService.getEquipmentAmount('personnel', this.personnelId);

    this.subscriptions.push(
      serviceMethod
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.equipmentTotalAmount = result;
      })
    )  
  }

  ngOnInit(): void {
    this.getEquipmentAmount();
  }
}
