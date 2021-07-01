import { Component, OnDestroy } from '@angular/core';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-tems',
  templateUrl: './tems.component.html',
  styleUrls: ['./tems.component.scss']
})
export class TEMSComponent implements OnDestroy{
  // Parent component for many others
  cancelFirstOnChange = true;

  subscriptions: Subscription[] = [];
  constructor() {
  }

  ngOnDestroy(): void {
    this.unsubscribeFromAll();
  }

  unsubscribeFromAll(){
    this.subscriptions.forEach(s => s.unsubscribe());
  }
}