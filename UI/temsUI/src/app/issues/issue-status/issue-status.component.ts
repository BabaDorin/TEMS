import { ViewIssueSimplified } from './../../models/communication/issues/view-issue-simplified.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-issue-status',
  templateUrl: './issue-status.component.html',
  styleUrls: ['./issue-status.component.scss']
})
export class IssueStatusComponent implements OnInit {

  @Input() issue: ViewIssueSimplified;
  
  constructor() { }

  ngOnInit(): void {
  }

}
