import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class DropdownManagerService {
  private closeAllSubject = new Subject<number>();
  
  closeAll$ = this.closeAllSubject.asObservable();

  notifyOpen(instanceId: number): void {
    this.closeAllSubject.next(instanceId);
  }
}
