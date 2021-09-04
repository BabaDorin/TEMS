import { ResponseFactory } from './../../models/system/response.model';
import { LogFilter } from './../../helpers/filters/log.filter';
import { Component, Input, OnInit } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { IOption } from 'src/app/models/option.model';
import { LogsService } from 'src/app/services/logs.service';
import { DialogService } from '../../services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { AddLogComponent } from '../communication/add-log/add-log.component';
import { ClaimService } from './../../services/claim.service';
import { TEMSComponent } from './../../tems/tems.component';

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
  pageNumber=1;

  logs: ViewLog[];
  loading: boolean = true;  
  logsEndpoint;
  filter: LogFilter;

  constructor(
    private logsService: LogsService,
    private dialoService: DialogService,
    private snackService: SnackService,
    public claims: ClaimService
  ) { 
    super();
  }

  buildFilter(){
    this.filter = new LogFilter();
    this.filter.skip = (this.pageNumber - 1) * this.itemsPerPage;
    this.filter.take = this.itemsPerPage;
    this.filter.equipmentId = this.equipment?.value;
    this.filter.roomId = this.room?.value;
    this.filter.personnelId = this.personnel?.value;
  }

  validateFilter(): boolean{
    if(!this.filter.isValid){
      this.snackService.snack(ResponseFactory.Neutral("Please, provide an entity whose logs to be fetched"));
      return false;
    }

    return true;
  }

  getTotalItems(){
    let endPoint;
    endPoint = this.logsService.getTotalItems(this.filter);

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
    this.buildFilter();
    if(!this.validateFilter())
      return;
    
    this.getTotalItems();
    this.fetchLogs();
  }

  fetchLogs(){
    this.loading = true;
    this.subscriptions.push(
      this.logsService.getEntityLogs(this.filter)
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;
        
        this.logs = result;
      })
    );
  }

  addLog(){   
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
