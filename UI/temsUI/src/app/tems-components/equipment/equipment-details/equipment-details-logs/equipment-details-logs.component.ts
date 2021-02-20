import { EquipmentService } from './../../../../services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';

@Component({
  selector: 'app-equipment-details-logs',
  templateUrl: './equipment-details-logs.component.html',
  styleUrls: ['./equipment-details-logs.component.scss']
})
export class EquipmentDetailsLogsComponent implements OnInit {

  @Input() equipmentId;
  logs: ViewLog[];

  constructor(private logsService: LogsService) { 

  }

  ngOnInit(): void {
    this.logs = this.logsService.getLogsByEquipmentId(this.equipmentId);
  }
}
