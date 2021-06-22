import { LogsService } from './logs-service/logs.service';
import { TypeService } from './type-service/type.service';
import { DefinitionService } from './definition-service/definition.service';
import { ReportService } from './report-service/report.service';
import { KeysService } from './keys-service/keys.service';
import { PersonnelService } from './personnel-service/personnel.service';
import { RoomsService } from './rooms-service/rooms.service';
import { IssuesService } from './issues-service/issues.service';
import { EquipmentService } from './equipment-service/equipment.service';
import { IMap } from './../models/map.model';
import { ArchievedItem } from './../models/archieve/archieved-item.model';
import { Observable } from 'rxjs';
import { IOption } from './../models/option.model';
import { API_ARCH_URL } from './../models/backend.config';
import { TEMSService } from './tems-service/tems.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ArchieveService extends TEMSService {

  types = [
    'Equipment',
    'Issues',
    'Rooms',
    'Personnel',
    'Keys',
    'Report templates',
    'Equipment Allocations',
    'Logs',
    'Key allocations',
    'Properties',
    'Equipment Types',
    'Equipment Definitions',
  ];

  typeRemoveServicesDictionary: IMap<any>;

  constructor(
    private http: HttpClient,
    private equipmentService: EquipmentService,
    private issuesService: IssuesService,
    private roomsService: RoomsService,
    private personnelService: PersonnelService,
    private keysService: KeysService,
    private reportService: ReportService,
    private logsService: LogsService,
    private defintionService: DefinitionService,
    private typeService: TypeService,
  ) {
    super();

    this.typeRemoveServicesDictionary = {
      ["Equipment"]: (id) =>  this.equipmentService.removeEquipment(id),
      ["Properties"]: (id) => this.equipmentService.removeProperty(id),
      ["Issues"]: (id) =>this.issuesService.remove(id),
      ["Personnel"]: (id) =>this.personnelService.remove(id),
      ["Keys"]: (id) =>this.keysService.remove(id),
      ["Report templates"]: (id) =>this.reportService.remove(id),
      ["Equipment Allocations"]: (id) =>this.equipmentService.removeAllocation(id),
      ["Logs"]: (id) =>this.logsService.remove(id),
      ["Key allocations"]:(id) => this.keysService.removeAllocation(id),
      ["Equipment Types"]: (id) =>this.typeService.remove(id),
      ["Equipment Definitions"]: (id) =>this.defintionService.remove(id),
      ["Rooms"]:(id) => this.roomsService.remove(id)
    }
  }

  getArchievedItems(itemType: string): Observable<ArchievedItem[]> {
    return this.http.get<ArchievedItem[]>(
      API_ARCH_URL + '/getarchieveditems/' + itemType,
      this.httpOptions
    );
  }

  setArchivationStatus(itemType: string, itemId: string, status: boolean): Observable<any> {
    return this.http.get(
      API_ARCH_URL + '/setArchivationStatus/' + itemType + '/' + itemId + '/' + status,
      this.httpOptions
    );
  }

  removeEntity(entityType: string, itemId: string): Observable<any> {
    return this.typeRemoveServicesDictionary[entityType](itemId);
  }
}
