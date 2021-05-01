import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-report-container',
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.scss']
})
export class ReportContainerComponent implements OnInit {

  @Input() template;  
  @Output() editTemplate = new EventEmitter();
  @Output() removeTemplate = new EventEmitter();
  @Output() generateReport = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
  }

  edit(templateId: string){
    this.editTemplate.emit(templateId);
  }

  remove(templateId: string)
  {
    this.removeTemplate.emit(templateId);
  }

  genReport(templateId: string){
    this.generateReport.emit(templateId);
  }
}
