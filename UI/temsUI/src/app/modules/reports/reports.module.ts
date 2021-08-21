import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatSelectModule } from '@angular/material/select';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatOptionModule } from '@angular/material/core';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatRadioModule } from '@angular/material/radio';
import { MatStepperModule } from '@angular/material/stepper';
import { TranslateModule } from '@ngx-translate/core';
import { NgxPaginationModule } from 'ngx-pagination';
import { ReportContainerComponent } from 'src/app/tems-components/reports/report-container/report-container.component';
import { CreateReportTemplateComponent } from '../../tems-components/reports/create-report-template/create-report-template.component';
import { GeneratedReportContainerComponent } from './../../tems-components/reports/generated-report-container/generated-report-container.component';
import { ReportsComponent } from './../../tems-components/reports/reports.component';
import { ViewTemplateComponent } from './../../tems-components/reports/view-template/view-template.component';
import { TemsFormsModule } from './../tems-forms/tems-forms.module';
import { ReportsRoutingModule } from './reports-routing.module';


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
    MatSelectModule,
    MatInputModule,
    MatIconModule,
    MatRadioModule,
    MatCardModule,
    MatMenuModule,
    MatButtonModule,
    MatProgressBarModule,
    TranslateModule,
    NgxPaginationModule,
    MatOptionModule,
    MatTooltipModule,
    MatCheckboxModule
  ],
  exports: [
    ReportsComponent,
  ]
})
export class ReportsModule { }
