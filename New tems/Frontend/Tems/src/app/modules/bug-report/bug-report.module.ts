import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { TranslateModule } from '@ngx-translate/core';
import { BugReportComponent } from './../../tems-components/forms/bug-report/bug-report.component';
import { ComponentFactory, ComponentFactoryResolver, NgModule } from '@angular/core';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule,
    BugReportComponent,
    ReactiveFormsModule,
    FormsModule,
    MatInputModule,
    MatIconModule,
    MatButtonModule,
    MatButtonToggleModule,
    TranslateModule,
    MatFormFieldModule
  ],
  exports: [
    BugReportComponent
  ]
})
export class BugReportModule { 
  constructor(private componentFactoryResolver: ComponentFactoryResolver) { }

  public resolveComponent(): ComponentFactory<BugReportComponent> {
    return this.componentFactoryResolver.resolveComponentFactory(BugReportComponent);
  }
}
