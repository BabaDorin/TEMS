import { Component, Input, OnInit } from '@angular/core';
import { ViewIssueSimplified } from './../../../models/communication/issues/view-issue-simplified.model';

@Component({
  selector: 'app-issue-container-simplified',
  templateUrl: './issue-container-simplified.component.html',
  styleUrls: ['./issue-container-simplified.component.scss']
})
export class IssueContainerSimplifiedComponent implements OnInit {

  @Input() issue: ViewIssueSimplified;
  
  constructor() { }

  ngOnInit(): void {
  }

}
