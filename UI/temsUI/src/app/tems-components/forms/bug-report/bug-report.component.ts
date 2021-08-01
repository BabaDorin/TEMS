import { SnackService } from './../../../services/snack.service';
import { TokenService } from './../../../services/token.service';
import { BugReportService } from './../../../services/bug-report.service';
import { BugReport } from './../../../models/bug-report/bug-report.model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-bug-report',
  templateUrl: './bug-report.component.html',
  styleUrls: ['./bug-report.component.scss']
})
export class BugReportComponent implements OnInit {

  dialogRef;
  selectedFilesLabel = "";

  reportFormGroup = new FormGroup({
     reportType: new FormControl("bug", Validators.required),
     description: new FormControl('', Validators.required),
     attachments: new FormControl()
  });

  constructor(
    public translate: TranslateService,
    private bugReportService: BugReportService,
    private snackService: SnackService
  ) { }

  ngOnInit(): void {
  }

  onSubmit(){
    let model = new BugReport();
    model.reportType = this.reportFormGroup.value["reportType"];
    model.description = this.reportFormGroup.value["description"];
    model.attachments = this.reportFormGroup.value["attachments"];

    this.bugReportService.sendReport(model)
    .subscribe(result => {
      console.log(result);
    })
  }

  onFilesSelected(files) {
    this.selectedFilesLabel = '';
    if(files == undefined || files.length == 0)
      return;
    
    let modelFiles = [] as File[];
    let totalLength = 0;

    for(let  i = 0; i < files.length; i++){
      modelFiles.push(files[i]);
      totalLength += files[i].size;
      this.selectedFilesLabel += files[i].name + ', ';
    };

    if(totalLength > 20 * 1024 * 1024){
      this.snackService.snack({ message: "Maximum size limit of 20mb was excedeed. NO files selected.", status: 2 })
      this.selectedFilesLabel = '';
      modelFiles = [];
    }

    this.reportFormGroup.controls["attachments"].setValue(modelFiles);
  }
}
