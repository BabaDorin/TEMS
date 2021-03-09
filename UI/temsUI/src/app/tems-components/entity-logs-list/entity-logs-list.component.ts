import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from 'src/app/models/option.model';
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
export class EntityLogsListComponent extends TEMSComponent implements OnInit {

  @Input() equipment: IOption;
  @Input() room: IOption;
  @Input() personnel: IOption;

  logs: ViewLog[];
  @Input() addLogEnabled: boolean = true;


  constructor(
    private logsService: LogsService,
    public dialog: MatDialog
  ) { 
    super();
  }

  ngOnInit(): void {
    // if(this.equipment == undefined && this.room == undefined && this.personnel == undefined){
    //   console.warn('EntityLogsListComponent requires an entity in order to display logs');
    //   return;
    // }
    
    if(this.equipment)
      this.subscriptions.push(this.logsService.getLogsByEquipmentId(this.equipment.value)
        .subscribe(result => {
          console.log(result);
          this.logs = result;
        }))

        // Uncomment when service is done
    // if(this.room)
    //   this.logs = this.logsService.getLogsByRoomId(this.room.value);

    // if(this.personnel)
    //   this.logs = this.logsService.getLogsByPersonnelId(this.personnel.value);
    
    // if(this.logs == undefined)
    //   this.logs = this.logsService.getLogs();
  }

  private addLog(){
    // Opens a dialog containing an instance of AddLogComponent
    
    let dialogRef: MatDialogRef<any>;
    dialogRef = this.dialog.open(AddLogComponent); 
    
    if(this.equipment){
      dialogRef.componentInstance.equipment = [
        {
          value: this.equipment.value, 
          label: this.equipment.label
        }];
    }

    if(this.room){
      dialogRef.componentInstance.room = [
        {
          value: this.room.value, 
          label: this.room.label
        }];
    }

    if(this.personnel){
      dialogRef.componentInstance.personnel = [
        {
          value: this.personnel.value, 
          label: this.personnel.label
        }];
    }

    dialogRef.afterClosed().subscribe(result => {
      // Stuff
    });
  }
}
