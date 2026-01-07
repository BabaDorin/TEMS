import { IncludeAssetLabelsComponent } from './../../shared/include-asset-tags/include-asset-tags.component';
import { ResponseFactory } from './../../models/system/response.model';
import { LogFilter } from './../../helpers/filters/log.filter';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';
import { IOption } from 'src/app/models/option.model';
import { LogsService } from 'src/app/services/logs.service';
import { DialogService } from '../../services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { AddLogComponent } from '../communication/add-log/add-log.component';
import { ClaimService } from './../../services/claim.service';
import { TEMSComponent } from './../../tems/tems.component';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { LogContainerComponent } from '../communication/log-container/log-container.component';
import { LoadingPlaceholderComponent } from '../loading-placeholder/loading-placeholder.component';

@Component({
  selector: 'app-entity-logs-list',
  standalone: true,
  imports: [
    CommonModule,
    MatIconModule,
    MatButtonModule,
    MatMenuModule,
    MatProgressBarModule,
    TranslateModule,
    NgxPaginationModule,
    IncludeAssetLabelsComponent,
    LogContainerComponent,
    LoadingPlaceholderComponent
  ],
  templateUrl: './entity-logs-list.component.html',
  styleUrls: ['./entity-logs-list.component.scss'],
})
export class EntityLogsListComponent extends TEMSComponent implements OnInit {

  @Input() equipment: IOption;
  @Input() room: IOption;
  @Input() personnel: IOption;
  @Input() addLogEnabled: boolean = true;

  @ViewChild('includeEquipmentLabels') includeEquipmentLabels: IncludeAssetLabelsComponent;

  itemsPerPage = 10;
  totalItems = 0;
  pageNumber=1;

  logs: ViewLog[];
  loading: boolean = true;  
  logsEndpoint;
  filter: LogFilter;
  defaultLabels = ['Equipment'];

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
    this.filter.assetId = this.equipment?.value;
    this.filter.roomId = this.room?.value;
    this.filter.personnelId = this.personnel?.value;
    this.filter.includeLabels = this.includeEquipmentLabels?.value ?? this.defaultLabels;
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
    this.fetchLogs();
  }

  fetchLogs(){
    this.buildFilter();
    if(!this.validateFilter())
      return;
      
    this.getTotalItems();

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
      selectedEntityType = "asset";
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
