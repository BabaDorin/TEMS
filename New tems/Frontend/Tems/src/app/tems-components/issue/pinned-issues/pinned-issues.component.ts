import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { IssuesService } from 'src/app/services/issues.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { ClaimService } from './../../../services/claim.service';
import { IssueContainerComponent } from '../../issues/issue-container/issue-container.component';

@Component({
  selector: 'app-pinned-issues',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule,
    IssueContainerComponent
  ],
  templateUrl: './pinned-issues.component.html',
  styleUrls: ['./pinned-issues.component.scss']
})
export class PinnedIssuesComponent extends TEMSComponent implements OnInit {
  
  @Input() issues: ViewIssueSimplified[];
  @Input() statuses;

  @Output() unPinned = new EventEmitter();

  constructor(
    private issuesService: IssuesService,
    private snackService: SnackService,
    public claims: ClaimService
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
