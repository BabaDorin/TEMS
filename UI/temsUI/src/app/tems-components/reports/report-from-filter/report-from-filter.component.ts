import { DownloadService } from './../../../download.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { ResponseFactory } from './../../../models/system/response.model';
import { SnackService } from './../../../services/snack.service';
import { TranslateService } from '@ngx-translate/core';
import { PersonnelService } from './../../../services/personnel.service';
import { FormGroup, FormControl } from '@angular/forms';
import { ReportService } from './../../../services/report.service';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ReportFromFilter } from 'src/app/models/report/add-report.model';
import { hasElements, isNullOrEmpty, IsNullOrUndefined } from 'src/app/helpers/validators/validations';

@Component({
  selector: 'app-report-from-filter',
  templateUrl: './report-from-filter.component.html',
  styleUrls: ['./report-from-filter.component.scss']
})
export class ReportFromFilterComponent extends TEMSComponent implements OnInit {

  dialogRef;

  @Input() equipmentFilter: EquipmentFilter;
  
  formGroup = new FormGroup({
    name: new FormControl(),
    header: new FormControl(),
    properties: new FormControl(),
    footer: new FormControl(),
    signatories: new FormControl()
  });

  constructor(
    private reportService: ReportService,
    private personnelService: PersonnelService,
    public translate: TranslateService,
    private snackService: SnackService,
    private downloadService: DownloadService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();
    if(dialogData != undefined){
      this.equipmentFilter = dialogData.equipmentFilter;
    }
  }

  ngOnInit(): void {
    console.log('filter that I got here');
    console.log(this.equipmentFilter);
  }

  generateReport(){
    let formVal = this.formGroup.value;
    console.log(formVal);

    if(!hasElements(formVal.properties.commonProperties))
    {
      this.snackService.snack(ResponseFactory.Neutral("Please, select at least one report property. The report can not contain 0 columns."));
      return;  
    }

    if(IsNullOrUndefined(this.equipmentFilter))
    {
      this.snackService.snack(ResponseFactory.Neutral("No equipment filter has been specified."));
      return;  
    }

    let viewModel = new ReportFromFilter();
    viewModel.commonProperties = formVal.properties.commonProperties;
    viewModel.filter = this.equipmentFilter;
    viewModel.footer == formVal.footer;
    viewModel.header = formVal.header;
    viewModel.name = formVal.name;
    viewModel.signatories = formVal.signatories;

    console.log('model that goes to service:');
    console.log(viewModel);

    this.subscriptions.push(
      this.reportService.generateReportFromFilter(viewModel)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        let fileName = (isNullOrEmpty(viewModel.name)) ? 'Report.xlsx' : viewModel.name + '.xlsx';
        this.downloadService.downloadFile(result, fileName);
      })
    )
  }
}
