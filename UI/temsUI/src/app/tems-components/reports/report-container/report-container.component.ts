import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmService } from 'src/app/confirm.service';
import { SnackService } from 'src/app/services/snack.service';
import { DialogService } from '../../../services/dialog.service';
import { ReportService } from '../../../services/report.service';
import { Report } from './../../../models/report/report.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewTemplateComponent } from './../view-template/view-template.component';

@Component({
  selector: 'app-report-container',
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.scss']
})
export class ReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() template: Report;  
  @Input() canManage: boolean = false;
  @Output() templateRemoved = new EventEmitter();
  @Output() generateReport = new EventEmitter();

  constructor(
    private router: Router,
    private reportService: ReportService,
    private snackService: SnackService,
    private dialogService: DialogService,
    private confirmService: ConfirmService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  edit(){
    this.router.navigate(["/reports/updatetemplate/" + this.template.id]);
  }

  async remove(){
    if(!await this.confirmService.confirm("Are you sure that you want to remove that report template?"))
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

  viewTemplateDetails(){
    this.dialogService.openDialog(
      ViewTemplateComponent,
      [{ label: "template", value: this.template }]
    );
  }
}
