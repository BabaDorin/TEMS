import { LogsService } from './logs.service';
import { TypeService } from './type.service';
import { DefinitionService } from './definition.service';
import { ReportService } from './report.service';
import { KeysService } from './keys.service';
import { PersonnelService } from './personnel.service';
import { RoomsService } from './rooms.service';
import { IssuesService } from './issues.service';
import { IMap } from './../models/map.model';
import { ArchievedItem } from './../models/archieve/archieved-item.model';
import { Observable } from 'rxjs';
import { IOption } from './../models/option.model';
import { API_ARCH_URL } from './../models/backend.config';
import { TEMSService } from './tems.service';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EquipmentService } from './equipment.service';

@Injectable({
  providedIn: 'root'
})
export class ArchieveService extends TEMSService {

  public types: IOption[] = [
    { label: 'Equipment', value: 'equipment'},
    { label: 'Issues', value: 'issues'},
    { label: 'Rooms', value: 'rooms'},
    { label: 'Personnel', value: 'personnel'},
    { label: 'Keys', value: 'keys'},
    { label: 'Report templates', value: 'reportTemplates'},
    { label: 'Equipment allocations', value: 'equipmentAllocations'},
    { label: 'Logs', value: 'logs'},
    { label: 'Key allocations', value: 'keyAllocations'},
    { label: 'Properties', value: 'properties'},
    { label: 'Equipment types', value: 'equipmentTypes'},
    { label: 'Equipment definitions', value: 'equipmentDefinitions'},
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
      ["equipment"]: (id) =>  this.equipmentService.removeEquipment(id),
      ["properties"]: (id) => this.equipmentService.removeProperty(id),
      ["issues"]: (id) =>this.issuesService.remove(id),
      ["personnel"]: (id) =>this.personnelService.remove(id),
      ["keys"]: (id) =>this.keysService.remove(id),
      ["reportTemplates"]: (id) =>this.reportService.remove(id),
      ["equipmentAllocations"]: (id) =>this.equipmentService.removeAllocation(id),
      ["logs"]: (id) =>this.logsService.remove(id),
      ["keyAllocations"]:(id) => this.keysService.removeAllocation(id),
      ["equipmentTypes"]: (id) =>this.typeService.remove(id),
      ["equipmentDefinitions"]: (id) =>this.defintionService.remove(id),
      ["rooms"]:(id) => this.roomsService.remove(id)
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
    console.log('entity type:' + entityType);
    console.log('entity id:' + itemId);

    return this.typeRemoveServicesDictionary[entityType](itemId);
  }
}
