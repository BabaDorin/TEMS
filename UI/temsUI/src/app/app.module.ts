import { EntitySharedModuleModule } from './tems-components/entity-shared-module/entity-shared-module.module';
// import { PropertyRenderComponent } from './public/property-render/property-render.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { PersonnelService } from './services/personnel-service/personnel.service';
import { HttpClientModule } from '@angular/common/http';
import { ButtonTypeComponent } from './public/formly/button-type/button-type.component';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { ChartsModule, ThemeService } from 'ng2-charts';

import { AppComponent } from './app.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { SidebarComponent } from './shared/sidebar/sidebar.component';
import { FooterComponent } from './shared/footer/footer.component';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TodoComponent } from './public/apps/todo-list/todo/todo.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { ContentAnimateDirective } from './shared/directives/content-animate.directive';
import { TodoListComponent } from './public/apps/todo-list/todo-list.component';
import { FormlyModule } from '@ngx-formly/core';
import { FormlyMaterialModule } from '@ngx-formly/material';
import { FormlyWrapperComponent } from './public/formly/formly-wrapper/formly-wrapper.component';
import { TagInputModule } from 'ngx-chips';
import { MatDialogModule } from '@angular/material/dialog';
import { InputTooltipComponent } from './public/formly/input-tooltip/input-tooltip.component';
import { RepeatTypeComponent } from './public/formly/repeat-type/repeat-type.component';
import { AddEquipmentRepeatComponent } from './public/formly/add-equipment-repeat/add-equipment-repeat.component';
import { LogsService } from './services/logs-service/logs.service';
import { AutocompleteTypeComponent } from './public/formly/autocomplete-type/autocomplete-type.component';

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    DashboardComponent,
    TodoListComponent,
    TodoComponent,
    SpinnerComponent,
    ContentAnimateDirective,
    FormlyWrapperComponent,
    RepeatTypeComponent,
    AddEquipmentRepeatComponent,
    AutocompleteTypeComponent,
    QuickAccessComponent,
    
    // common for equipment, room and personnel modules
    // PropertyRenderComponent, 
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    MatDialogModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    FormlyModule,
    HttpClientModule,
    TagInputModule,
    BrowserModule,
    FormlyMaterialModule,
    BrowserAnimationsModule,
    EntitySharedModuleModule,
    
    TagInputModule,
    MatInputModule,
    MatAutocompleteModule,
    FormlyModule.forRoot({
      wrappers: [
        { name: 'formly-wrapper', component: FormlyWrapperComponent },
      ],
      types: [
        {
          name: 'input-tooltip',
          component: InputTooltipComponent,
          // wrappers: ['form-field'],
          defaultOptions: {
            type: 'text'
          }
        },
        {
          name: 'autocomplete',
          component: AutocompleteTypeComponent,
          wrappers: ['form-field'],
        },
        {
          name: 'button',
          component: ButtonTypeComponent,
          wrappers: ['form-field'],
          defaultOptions: {
            templateOptions: {
              btnType: 'default',
              type: 'button',
            },
          },
        },
        { name: 'repeat', component: RepeatTypeComponent },
        { name: 'eq-repeat', component: AddEquipmentRepeatComponent },
      ],
    }),
    FormlyMaterialModule,
  ],
  exports: [
    MatAutocompleteModule,
    MatIconModule,
  ],
  providers: [
    ThemeService,
    LogsService, 
    PersonnelService, 
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
