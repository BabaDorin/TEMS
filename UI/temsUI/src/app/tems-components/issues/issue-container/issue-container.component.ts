import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-issue-container',
  templateUrl: './issue-container.component.html',
  styleUrls: ['./issue-container.component.scss']
})
export class IssueContainerComponent implements OnInit {

  @Input() issue;
  @Output() solve = new EventEmitter(); 
  @Output() reopen = new EventEmitter(); 
  @Output() remove = new EventEmitter(); 

  constructor() { }

  ngOnInit(): void {
  }

  solveIssue(id, index){
    this.solve.emit({id: id, index: index});
  }

  reopenIssue(id, index){
    this.reopen.emit({id: id, index: index});
  }

  removeIssue(id, index){
    this.remove.emit({id: id, index: index});
  }
}
