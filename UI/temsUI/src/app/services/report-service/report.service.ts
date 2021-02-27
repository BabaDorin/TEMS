import { ViewReportSimplified } from './../../models/report/view-report-simplified.model';
import { ViewRoomSimplified } from 'src/app/models/room/view-room-simplified.model';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ReportService {

  constructor() { }

  getCustomReports(){
    return [
      new ViewReportSimplified() // TODO: Interface to display default and, after that, custom report templates
    ]
  }
}
