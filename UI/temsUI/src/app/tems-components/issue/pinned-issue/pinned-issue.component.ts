import { SnackService } from 'src/app/services/snack/snack.service';
import { IssuesService } from './../../../services/issues-service/issues.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, Input, OnInit, Output } from '@angular/core';
import EventEmitter from 'events';

@Component({
  selector: 'app-pinned-issue',
  templateUrl: './pinned-issue.component.html',
  styleUrls: ['./pinned-issue.component.scss']
})
export class PinnedIssueComponent extends TEMSComponent implements OnInit {

  @Input() issue;
  @Input() statuses;

  @Output() unPinned = new EventEmitter();

  constructor(
    private issuesService: IssuesService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.statuses == undefined)
      this.fetchStatuses;

    if(this.issue = undefined)
      this.fetchPinnedIssue();
  }

  fetchStatuses(){
    this.subscriptions.push(
      this.issuesService.getStatuses()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.statuses = result;
      }
    ));
  }

  fetchPinnedIssue(){
    this.subscriptions.push(
      this.issuesService.getPinnedTicket()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        
        this.issue = result;
      })
    )
  }

  ticketUnpinned(eventData){
    this.unPinned.emit(eventData);
    this.issue = undefined;
  }

  remove(){
    this.issue = undefined;
  }
}
