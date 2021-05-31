import { SnackService } from './../../../services/snack/snack.service';
import { ReportService } from './../../../services/report-service/report.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Downloader } from './../../../shared/downloader/fileDownloader';
import { GeneratedReport } from './../../../models/report/generated-report.model';
import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-generated-report-container',
  templateUrl: './generated-report-container.component.html',
  styleUrls: ['./generated-report-container.component.scss']
})
export class GeneratedReportContainerComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;
  @Input() report: GeneratedReport;
  @Output() reportRemoved = new EventEmitter();

  downloader: Downloader;

  constructor(
    private reportService: ReportService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  removeGeneratedReport() {
    this.subscriptions.push(
      this.reportService.removeReport(this.report.id)
        .subscribe(result => {
          this.snackService.snack(result);

          if (result.status == 1) {
            this.reportRemoved.emit(this.report.id);
          }
        })
    )
  }

  printFromGeneratedReport() {
    if (this.downloader == undefined) this.downloader = new Downloader();

    this.subscriptions.push(
      this.reportService.getReport(this.report.id)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.downloader.downloadFile(result, "Report.xlsx");
        })
    )
  }
}