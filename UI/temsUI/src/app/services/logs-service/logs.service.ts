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

  getLogsByRoomId(roomId: string){
    return this.getLogsByEquipmentId('1'); // testing purposes
  }

  getLogsByPersonnelId(personnelId: string){
    return this.getLogsByEquipmentId('1'); // testing purposes
  }

  getLogs(){
    return [
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
      new ViewLog(),
    ]
  }
}
