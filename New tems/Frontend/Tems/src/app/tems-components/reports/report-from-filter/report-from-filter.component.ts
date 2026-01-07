import { DownloadService } from './../../../download.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { ResponseFactory } from './../../../models/system/response.model';
import { SnackService } from './../../../services/snack.service';
import { TranslateService } from '@ngx-translate/core';
import { PersonnelService } from './../../../services/personnel.service';
import { FormGroup, FormControl } from '@angular/forms';
import { ReportService } from './../../../services/report.service';
import { AssetFilter } from './../../../helpers/filters/asset.filter';
import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ReportFromFilter } from '../../../models/report/add-report.model';
import { hasElements, isNullOrEmpty, IsNullOrUndefined } from '../../../helpers/validators/validations';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { TranslateModule } from '@ngx-translate/core';
import { TEMS_FORMS_IMPORTS } from 'src/app/shared/constants/tems-forms-imports.const';
import { ChipsAutocompleteComponent } from '../../../public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-report-from-filter',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatSelectModule,
    MatButtonModule,
    TranslateModule,
    ChipsAutocompleteComponent,
    ...TEMS_FORMS_IMPORTS
  ],
  templateUrl: './report-from-filter.component.html',
  styleUrls: ['./report-from-filter.component.scss']
})
export class ReportFromFilterComponent extends TEMSComponent implements OnInit {

  dialogRef;

  @Input() assetFilter: AssetFilter;
  
  formGroup = new FormGroup({
    name: new FormControl(),
    header: new FormControl(),
    properties: new FormControl(),
    footer: new FormControl(),
    signatories: new FormControl()
  });

  constructor(
    private reportService: ReportService,
    public personnelService: PersonnelService,
    public translate: TranslateService,
    private snackService: SnackService,
    private downloadService: DownloadService,
    private activatedRoute: ActivatedRoute,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();
    if(dialogData != undefined){
      this.assetFilter = dialogData.assetFilter;
    }
  }

  ngOnInit(): void {
  }

  generateReport(){
    let formVal = this.formGroup.value;

    if(!hasElements(formVal.properties.commonProperties))
    {
      this.snackService.snack(ResponseFactory.Neutral("Please, select at least one report property. The report can not contain 0 columns."));
      return;  
    }

    if(IsNullOrUndefined(this.assetFilter))
    {
      this.snackService.snack(ResponseFactory.Neutral("No equipment filter has been specified."));
      return;  
    }

    let viewModel = new ReportFromFilter();
    viewModel.commonProperties = formVal.properties.commonProperties;
    viewModel.filter = this.assetFilter;
    viewModel.footer = formVal.footer;
    viewModel.header = formVal.header;
    viewModel.name = formVal.name;
    viewModel.signatories = formVal.signatories?.map(q => q.label);

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
