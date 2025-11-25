import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { SidebarComponent } from 'src/app/shared/sidebar/sidebar.component';
import { SpinnerComponent } from 'src/app/shared/spinner/spinner.component';
import { Subject } from 'rxjs';

@Component({
  selector: 'app-tems',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    SidebarComponent,
    SpinnerComponent
  ],
  templateUrl: './tems.component.html',
  styleUrls: ['./tems.component.scss']
})
export class TEMSComponent implements OnInit, OnDestroy {
  // If a component is opened using Mat-Dialog, this will contain the dialog reference.
  dialogRef;
  
  // Parent component for many others
  cancelFirstOnChange = true;

  protected destroy$ = new Subject<void>();
  subscriptions: any[] = [];

  constructor() {
  }

  ngOnInit(): void {
    // Initialization logic can go here
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.subscriptions.forEach(s => { try { s?.unsubscribe?.(); } catch {} });
  }
}