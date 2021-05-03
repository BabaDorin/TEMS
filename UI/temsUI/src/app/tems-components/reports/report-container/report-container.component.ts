import { SnackService } from 'src/app/services/snack/snack.service';
import { Report } from './../../../models/report/report.model';
import { ReportService } from './../../../services/report-service/report.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-report-container',
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.scss']
})
export class ReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() template: Report;  
  @Output() templateRemoved = new EventEmitter();
  @Output() generateReport = new EventEmitter();

  constructor(
    private router: Router,
    private reportService: ReportService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    console.log(this.template)
  }

  edit(){
    this.router.navigate(["/reports/updatetemplate/" + this.template.id]);
  }

  remove(){
    if(!confirm("Are you sure that you want to remove that report template?"))
      return;
    
    this.subscriptions.push(
      this.reportService.archieveTemplate(this.template.id)
      .subscribe(result => {
        this.snackService.snack(result);
        if(result.status == 1)
          this.templateRemoved.emit(this.template);
      })
    )
  }

  genReport(templateId: string){
    this.generateReport.emit(templateId);
  }
}
