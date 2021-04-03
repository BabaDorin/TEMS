import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-issue-container',
  templateUrl: './issue-container.component.html',
  styleUrls: ['./issue-container.component.scss']
})
export class IssueContainerComponent implements OnInit {

  @Input() issue;
  @Input() statuses: IOption[];
  @Input() readonly: boolean = false;
  @Output() solve = new EventEmitter(); 
  @Output() reopen = new EventEmitter(); 
  @Output() remove = new EventEmitter(); 
  @Output() statusChanged = new EventEmitter(); 

  constructor() { }

  ngOnInit(): void {
  }

  solveIssue(){
    this.solve.emit();
  }

  reopenIssue(){
    this.reopen.emit();
  }

  removeIssue(){
    this.remove.emit();
  }

  issueStatusChanged($event){
    this.statusChanged.emit($event);
  }
}
