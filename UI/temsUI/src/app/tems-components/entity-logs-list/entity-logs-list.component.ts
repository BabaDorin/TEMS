import { ClaimService } from './../../services/claim.service';
import { SnackService } from '../../services/snack.service';
import { DialogService } from '../../services/dialog.service';
import { TEMSComponent } from './../../tems/tems.component';
import { IOption } from 'src/app/models/option.model';
import { Component, Input, OnInit } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { AddLogComponent } from '../communication/add-log/add-log.component';
import { LogsService } from 'src/app/services/logs.service';

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

  itemsPerPage = 10;
  totalItems = 0;

  logs: ViewLog[];
  loading: boolean = true;  
  logsEndpoint;

pageNumber=1;

  constructor(
    private logsService: LogsService,
    private dialoService: DialogService,
    private snackService: SnackService,
    private claims: ClaimService
  ) { 
    super();
  }

  getTotalItems(){
    let endPoint;
    if(this.equipment != undefined)
      endPoint = this.logsService.getTotalItems('equipment', this.equipment.value);

    if(this.personnel != undefined)
      endPoint = this.logsService.getTotalItems('personnel', this.personnel.value);

    if(this.room != undefined)
      endPoint = this.logsService.getTotalItems('room', this.room.value);

    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.totalItems = result;
      })
    )
  }

  ngOnInit(): void {
    this.getTotalItems();
    
    if(this.equipment)
      this.logsEndpoint = (pageNumber: number, itemsPerPage: number) => this.logsService.getLogsByEquipmentId(this.equipment.value, pageNumber, itemsPerPage);
    if(this.room)
      this.logsEndpoint = (pageNumber: number, itemsPerPage: number) => this.logsService.getLogsByRoomId(this.room.value, pageNumber, itemsPerPage);
    if(this.personnel)
      this.logsEndpoint = (pageNumber: number, itemsPerPage: number) => this.logsService.getLogsByPersonnelId(this.personnel.value, pageNumber, itemsPerPage);

    this.fetchLogs();
  }

  fetchLogs(){
    if(this.logsEndpoint != undefined){
      this.subscriptions.push(
        this.logsEndpoint(this.pageNumber, this.itemsPerPage)
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

  logRemoved(index: number){
    this.logs.splice(index, 1);
  }
}
