import { Component, Input, OnInit } from '@angular/core';
import { ViewIssueSimplified } from './../../../models/communication/issues/view-issue-simplified.model';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { RouterModule } from '@angular/router';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { IssueStatusComponent } from '../issue-status/issue-status.component';

@Component({
  selector: 'app-issue-container-simplified',
  standalone: true,
  imports: [CommonModule, TranslateModule, RouterModule, MatButtonModule, MatCardModule, MatIconModule, IssueStatusComponent],
  templateUrl: './issue-container-simplified.component.html',
  styleUrls: ['./issue-container-simplified.component.scss']
})
export class IssueContainerSimplifiedComponent implements OnInit {

  @Input() issue: ViewIssueSimplified;
  
  constructor() { }

  ngOnInit(): void {
  }

}
