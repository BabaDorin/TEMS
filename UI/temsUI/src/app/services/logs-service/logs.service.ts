import { Injectable } from '@angular/core';
import { ViewLog } from 'src/app/models/communication/logs/view-logs.model';

@Injectable({
  providedIn: 'root'
})
export class LogsService {

  constructor() { }

  getLogsByEquipmentId(equipmentId: string){
    return [
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
    ];
  }
}
