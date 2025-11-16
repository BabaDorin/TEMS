import { SnackService } from './../../../services/snack.service';
import { TokenService } from './../../../services/token.service';
import { BugReportService } from './../../../services/bug-report.service';
import { BugReport } from './../../../models/bug-report/bug-report.model';
import { FormGroup, FormControl, Validators } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, Inject, Optional } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-bug-report',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    TranslateModule
  ],
  templateUrl: './bug-report.component.html',
  styleUrls: ['./bug-report.component.scss']
})
export class BugReportComponent implements OnInit {

  selectedFilesLabel = "";
  sending = false;

  reportFormGroup = new FormGroup({
     reportType: new FormControl("bug", Validators.required),
     description: new FormControl('', Validators.required),
     attachments: new FormControl()
  });

  constructor(
    public translate: TranslateService,
    private bugReportService: BugReportService,
    private snackService: SnackService,
    @Optional() public dialogRef: MatDialogRef<BugReportComponent>,
    @Optional() @Inject(MAT_DIALOG_DATA) public data: any
  ) { }

  ngOnInit(): void {
  }

  onSubmit(){    
    let model = new BugReport();
    model.reportType = this.reportFormGroup.value["reportType"];
    model.description = this.reportFormGroup.value["description"];
    model.attachments = this.reportFormGroup.value["attachments"];

    this.sending = true;
    this.bugReportService.sendReport(model)
    .subscribe(result => {
      this.sending = false;

      this.snackService.snack(result);
      if(result.status = 1)
        this.dialogRef.close();
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
