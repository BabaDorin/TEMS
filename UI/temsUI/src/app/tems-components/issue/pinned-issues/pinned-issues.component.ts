import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IssuesService } from 'src/app/services/issues-service/issues.service';
import { SnackService } from 'src/app/services/snack/snack.service';

@Component({
  selector: 'app-pinned-issues',
  templateUrl: './pinned-issues.component.html',
  styleUrls: ['./pinned-issues.component.scss']
})
export class PinnedIssuesComponent extends TEMSComponent implements OnInit {
  
  @Input() issues: ViewIssueSimplified[];
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
      this.fetchStatuses();

    if(this.issues == undefined)
      this.fetchPinnedIssues();
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

  fetchPinnedIssues(){
    this.subscriptions.push(
      this.issuesService.getPinnedTickets()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        console.log('pinned issues:');
        console.log(result);
        this.issues = result;
      })
    )
  }

  ticketUnpinned(eventData: ViewIssueSimplified, index: number){
    this.unPinned.emit(eventData);
    this.issues.splice(index, 1);
  }

  remove(index: number){
    this.issues.splice(index, 1);
  }
}
