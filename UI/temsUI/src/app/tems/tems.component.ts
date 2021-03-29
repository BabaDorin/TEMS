import { MatDialog } from '@angular/material/dialog';
import { Component, Inject, Injectable, Injector, OnDestroy, OnInit, ReflectiveInjector } from '@angular/core';
import { Subscription } from 'rxjs';
import { MatSnackBar } from '@angular/material/snack-bar';

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