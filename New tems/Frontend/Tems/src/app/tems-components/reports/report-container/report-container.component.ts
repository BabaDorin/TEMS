import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { ConfirmService } from '../../../confirm.service';
import { SnackService } from '../../../services/snack.service';
import { DialogService } from '../../../services/dialog.service';
import { ReportService } from '../../../services/report.service';
import { ViewReportSimplified } from './../../../models/report/view-report-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { ViewTemplateComponent } from './../view-template/view-template.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatMenuModule } from '@angular/material/menu';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-report-container',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatMenuModule, MatIconModule, TranslateModule],
  templateUrl: './report-container.component.html',
  styleUrls: ['./report-container.component.scss']
})
export class ReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() template: ViewReportSimplified;  
  @Input() canManage: boolean = false;
  @Output() templateRemoved = new EventEmitter<string>();
  @Output() generateReport = new EventEmitter<string>();

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
          this.templateRemoved.emit(this.template.id);
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
