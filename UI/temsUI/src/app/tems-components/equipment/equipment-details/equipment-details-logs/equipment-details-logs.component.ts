import { ViewEquipmentSimplified } from './../../../../models/equipment/view-equipment-simplified.model';
import { EquipmentService } from './../../../../services/equipment-service/equipment.service';
import { Component, Input, OnInit } from '@angular/core';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';

@Component({
  selector: 'app-equipment-details-logs',
  templateUrl: './equipment-details-logs.component.html',
  styleUrls: ['./equipment-details-logs.component.scss']
})
export class EquipmentDetailsLogsComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  logs: ViewLog[];

  constructor(
    private logsService: LogsService,
    public dialog: MatDialog) { 
  }

  ngOnInit(): void {
    this.logs = this.logsService.getLogsByEquipmentId(this.equipment.id);
  }

  addLog(){
    this.openAddLogDialog();
  }
  
  openAddLogDialog(): void {
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddLogComponent); 
    dialogRef.componentInstance.equipment = [
      {
        id: this.equipment.id, 
        value: this.equipment.temsIdOrSerialNumber
      }];

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
