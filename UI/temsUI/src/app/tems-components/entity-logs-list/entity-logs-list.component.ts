import { Component, Input, OnInit } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { ViewPersonnelSimplified } from 'src/app/models/personnel/view-personnel-simplified.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { AddLogComponent } from '../communication/add-log/add-log.component';

@Component({
  selector: 'app-entity-logs-list',
  templateUrl: './entity-logs-list.component.html',
  styleUrls: ['./entity-logs-list.component.scss']
})
export class EntityLogsListComponent implements OnInit {

  @Input() equipment: ViewEquipmentSimplified;
  @Input() room: ViewRoomSimplified;
  @Input() personnel: ViewPersonnelSimplified;

  logs: ViewLog[];
  @Input() addLogEnabled: boolean = true;


  constructor(
    private logsService: LogsService,
    public dialog: MatDialog
  ) { 
    
  }

  ngOnInit(): void {
    // if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
    //   console.warn('EntityLogsListComponent requires an entity in order to display logs');
    //   return;
    // }

    if(this.equipment)
      this.logs = this.logsService.getLogsByEquipmentId(this.equipment.id);

    if(this.room)
      this.logs = this.logsService.getLogsByRoomId(this.room.id);

    if(this.personnel)
      this.logs = this.logsService.getLogsByPersonnelId(this.personnel.id);
    
    if(this.logs == undefined)
      this.logs = this.logsService.getLogs();
  }

  private addLog(){
    // Opens a dialog containing an instance of AddLogComponent
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddLogComponent); 
    
    if(this.equipment){
      dialogRef.componentInstance.equipment = [
        {
          id: this.equipment.id, 
          value: this.equipment.temsidOrSn
        }];
    }

    if(this.room){
      dialogRef.componentInstance.room = [
        {
          id: this.room.id, 
          value: this.room.identifier
        }];
    }

    if(this.personnel){
      dialogRef.componentInstance.personnel = [
        {
          id: this.personnel.id, 
          value: this.personnel.name
        }];
    }

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
