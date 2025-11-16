import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { EntityLogsListComponent } from '../../entity-logs-list/entity-logs-list.component';

@Component({
  selector: 'app-view-logs',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule,
    EntityLogsListComponent
  ],
  templateUrl: './view-logs.component.html',
  styleUrls: ['./view-logs.component.scss']
})
export class ViewLogsComponent implements OnInit {

  constructor() { }

  ngOnInit(): void {
  }
  
}
