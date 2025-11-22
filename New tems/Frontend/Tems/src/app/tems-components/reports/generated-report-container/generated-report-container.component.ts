import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { ReportService } from '../../../services/report.service';
import { SnackService } from '../../../services/snack.service';
import { GeneratedReport } from './../../../models/report/generated-report.model';
import { Downloader } from './../../../shared/downloader/fileDownloader';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-generated-report-container',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, RouterModule, TranslateModule],
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