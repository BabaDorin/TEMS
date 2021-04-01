import { SnackService } from './../../services/snack/snack.service';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from 'src/app/models/option.model';
import { Component, Input, OnInit } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { LogsService } from 'src/app/services/logs-service/logs.service';
import { AddLogComponent } from '../communication/add-log/add-log.component';

@Component({
  selector: 'app-entity-logs-list',
  templateUrl: './entity-logs-list.component.html',
  styleUrls: ['./entity-logs-list.component.scss'],
})
export class EntityLogsListComponent extends TEMSComponent implements OnInit {

  @Input() equipment: IOption;
  @Input() room: IOption;
  @Input() personnel: IOption;
  @Input() addLogEnabled: boolean = true;

  logs: ViewLog[];
  loading: boolean = true;  

  constructor(
    private logsService: LogsService,
    private dialoService: DialogService,
    private snackService: SnackService
  ) { 
    super();
  }

  ngOnInit(): void {
    let endPoint;

    if(this.equipment)
      endPoint = this.logsService.getLogsByEquipmentId(this.equipment.value);
    if(this.room)
        endPoint = this.logsService.getLogsByRoomId(this.room.value);
    if(this.personnel)
      endPoint = this.logsService.getLogsByPersonnelId(this.personnel.value);

    if(endPoint != undefined){
      this.subscriptions.push(
        endPoint
        .subscribe(result => {
          this.loading = false;
          if(this.snackService.snackIfError(result))
            return;
          this.logs = result;
          return;
        })
      )
    }

    if(this.equipment == undefined && this.room == undefined && this.personnel == undefined)
      this.subscriptions.push(this.logsService.getLogs()
        .subscribe(result => {
          console.log(result);
          this.logs = result;
          this.loading = false;
        }));
  }

  private addLog(){   
    let selectedEntityType: string;
    let selectedEntities: IOption[];

    if(this.equipment){
      selectedEntityType = "equipment";
      selectedEntities = [
        {
          value: this.equipment.value, 
          label: this.equipment.label
        }];
    }

    if(this.room){
      selectedEntityType = "room";
      selectedEntities = [
        {
          value: this.room.value, 
          label: this.room.label
        }];
    }

    if(this.personnel){
      selectedEntityType = "personnel";
      selectedEntities = [
        {
          value: this.personnel.value, 
          label: this.personnel.label
        }];
    }

    this.dialoService.openDialog(
      AddLogComponent,
      [{label: selectedEntityType, value: selectedEntities}],
      () => {
        this.ngOnInit();
      }
    )
  }

  remove(logId: string, index: number){
    if(!confirm("Are you sure you want to remove this log?"))
      return;

    this.subscriptions.push(
      this.logsService.archieve(logId)
      .subscribe(result => {
        console.log(result);
        this.snackService.snack(result);

        if(result.status == 1)
          this.logs.splice(index, 1);
      })
    )
  }
}
