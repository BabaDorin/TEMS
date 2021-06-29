import { TranslateModule } from '@ngx-translate/core';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatMenuModule } from '@angular/material/menu';
import { MatCardModule } from '@angular/material/card';
import { MatRadioModule } from '@angular/material/radio';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatStepperModule } from '@angular/material/stepper';
import { GeneratedReportContainerComponent } from './../../tems-components/reports/generated-report-container/generated-report-container.component';
import { ViewTemplateComponent } from './../../tems-components/reports/view-template/view-template.component';
import { NgxPaginationModule } from 'ngx-pagination';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { CreateReportTemplateComponent } from '../../tems-components/reports/create-report-template/create-report-template.component';
import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { ReportsRoutingModule } from './reports-routing.module';
import { ReportContainerComponent } from 'src/app/tems-components/reports/report-container/report-container.component';
import { MatOptionModule } from '@angular/material/core';

@NgModule({
  declarations: [
    ReportsComponent,
    ReportContainerComponent,
    CreateReportTemplateComponent,
    ViewTemplateComponent,
    GeneratedReportContainerComponent
  ],
  imports: [
    CommonModule,
    ReportsRoutingModule,
    TemsFormsModule,
    MatStepperModule,
    MatFormFieldModule,
    MatInputModule,
    MatIconModule,
    MatRadioModule,
    MatCardModule,
    MatMenuModule,
    MatProgressBarModule,
    TranslateModule,
    NgxPaginationModule,
    MatOptionModule,
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
