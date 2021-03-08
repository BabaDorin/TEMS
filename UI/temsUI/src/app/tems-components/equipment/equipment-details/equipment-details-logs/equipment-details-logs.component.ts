import { IOption } from 'src/app/models/option.model';
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
  equipmentOption: IOption;

  constructor(
    public dialog: MatDialog) { 
  }

  ngOnInit(): void {
    if(this.equipment != undefined)
      this.equipmentOption = { 
        value: this.equipment.id,
        label: this.equipment.temsIdOrSerialNumber,
      };

    console.log('equipment option:');
    console.log(this.equipmentOption);

    console.log('equipment:')
    console.log(this.equipment);
  }

  addLog(){
    this.openAddLogDialog();
  }
  
  openAddLogDialog(): void {
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddLogComponent); 
    dialogRef.componentInstance.equipment = [
      {
        value: this.equipment.id, 
        label: this.equipment.temsIdOrSerialNumber
      }];

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
