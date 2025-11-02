import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmService } from 'src/app/confirm.service';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { IOption } from 'src/app/models/option.model';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { IssuesService } from '../../../services/issues.service';

@Component({
  selector: 'app-issue-container',
  templateUrl: './issue-container.component.html',
  styleUrls: ['./issue-container.component.scss']
})
export class IssueContainerComponent extends TEMSComponent implements OnInit {

  @Input() issue: ViewIssueSimplified;
  @Input() statuses: IOption[];
  @Input() readonly: boolean = false;
  @Input() canManage: boolean = false;
 
  @Output() solve = new EventEmitter(); 
  @Output() reopen = new EventEmitter(); 
  @Output() remove = new EventEmitter(); 
  @Output() statusChanged = new EventEmitter();
  @Output() pinned = new EventEmitter();
  @Output() unPinned = new EventEmitter();

  constructor(
    private router: Router,
    private issuesService: IssuesService,
    private snackService: SnackService,
    private confirmService: ConfirmService
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
  
        this.issue.dateClosed = new Date;
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

  async removeIssue(){
    if(!await this.confirmService.confirm("Are you sure you want to remove that issue?"))
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
    let endPoint = this.issuesService.changePinStatus(this.issue.id, !this.issue.isPinned);
    this.subscriptions.push(
      endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.issue.isPinned = !this.issue.isPinned;
        
        if(this.issue.isPinned)
          this.pinned.emit(this.issue);
        else
          this.unPinned.emit(this.issue);
      })
    )
  }
}
