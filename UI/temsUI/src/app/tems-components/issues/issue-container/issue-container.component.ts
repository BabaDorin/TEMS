import { SnackService } from 'src/app/services/snack/snack.service';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { IssuesService } from './../../../services/issues-service/issues.service';
import { Router } from '@angular/router';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-issue-container',
  templateUrl: './issue-container.component.html',
  styleUrls: ['./issue-container.component.scss']
})
export class IssueContainerComponent extends TEMSComponent implements OnInit {

  @Input() issue: ViewIssueSimplified;
  @Input() statuses: IOption[];
  @Input() readonly: boolean = false;
  @Input() isPinned: boolean = false;
 
  @Output() solve = new EventEmitter(); 
  @Output() reopen = new EventEmitter(); 
  @Output() remove = new EventEmitter(); 
  @Output() statusChanged = new EventEmitter();
  @Output() pinned = new EventEmitter();
  @Output() unPinned = new EventEmitter();

  constructor(
    private router: Router,
    private issuesService: IssuesService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  solveIssue(){
    this.subscriptions.push(
      this.issuesService.closeIssue(this.issue.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.solve.emit();
      })
    )
  }

  reopenIssue(){
    this.subscriptions.push(
      this.issuesService.reopenIssue(this.issue.id)
      .subscribe(result => {
        if(result.status == 0){
          this.snackService.snack(result);
          return;
        }

        this.issue.dateClosed = undefined;
        this.reopen.emit();
      })
    )
  }

  removeIssue(){
    if(!confirm("Are you sure you want to remove that issue?"))
      return;

    this.subscriptions.push(
      this.issuesService.archieve(this.issue.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.remove.emit();
      })
    )
  }

  issueStatusChanged($event){
    this.subscriptions.push(
      this.issuesService.changeIssueStatus(this.issue.id, $event.value.value)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

          this.issue.status = $event.value;
        this.statusChanged.emit();
      })
    )
  }

  pin(){
    this.changePinStatus();
  }

  unPin(){
    this.changePinStatus();
  }

  changePinStatus(){
    let endPoint = this.issuesService.changePinStatus(this.issue.id, !this.isPinned);
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.isPinned = !this.isPinned;
        
        if(this.isPinned)
          this.pinned.emit(this.issue);
        else
          this.unPinned.emit(this.issue);
      })
    )
  }
}
