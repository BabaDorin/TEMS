import { HttpClientModule } from '@angular/common/http';
import { ButtonTypeComponent } from './public/formly/button-type/button-type.component';
import { ChipsAutocompleteComponent } from './public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyParserService } from './services/formly-parser-service/formly-parser.service';
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AgGridModule } from 'ag-grid-angular';
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
import { ViewEquipmentComponent } from './tems-components/equipment/view-equipment/view-equipment.component';
import { AddEquipmentComponent } from './tems-components/equipment/add-equipment/add-equipment.component';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { ViewRoomsComponent } from './tems-components/room/view-rooms/view-rooms.component';
import { CollegeMapComponent } from './tems-components/college-map/college-map.component';
import { RoomAllocationComponent } from './tems-components/room/room-allocation/room-allocation.component';
import { ViewPersonnelComponent } from './tems-components/personnel/view-personnel/view-personnel.component';
import { AddPersonnelComponent } from './tems-components/personnel/add-personnel/add-personnel.component';
import { PersonnelAllocationComponent } from './tems-components/personnel/personnel-allocation/personnel-allocation.component';
import { ViewKeysComponent } from './tems-components/keys/view-keys/view-keys.component';
import { ViewKeysAllocationsComponent } from './tems-components/keys/view-keys-allocations/view-keys-allocations.component';
import { KeysAllocationsComponent } from './tems-components/keys/keys-allocations/keys-allocations.component';
import { ViewIssuesComponent } from './tems-components/issue/view-issues/view-issues.component';
import { CreateIssueComponent } from './tems-components/issue/create-issue/create-issue.component';
import { ViewAnnouncementsComponent } from './tems-components/communication/view-announcements/view-announcements.component';
import { ViewLogsComponent } from './tems-components/communication/view-logs/view-logs.component';
import { ViewLibraryComponent } from './tems-components/library/view-library/view-library.component';
import { ReportsComponent } from './tems-components/reports/reports.component';
import { EquipmentManagementComponent } from './tems-components/admin/equipment-management/equipment-management.component';
import { UserManagementComponent } from './tems-components/admin/user-management/user-management.component';
import { RoleManagementComponent } from './tems-components/admin/role-management/role-management.component';
import { SystemConfigComponent } from './tems-components/admin/system-config/system-config.component';
import { EquipmentAllocationComponent } from './tems-components/equipment/equipment-allocation/equipment-allocation.component';
import { AnalyticsComponent } from './tems-components/analytics/analytics.component';
import { FormlyModule } from '@ngx-formly/core';
import { FormlyMaterialModule } from '@ngx-formly/material';
import { EquipmentService } from './services/equipment-service/equipment.service';
import { FormlyWrapperComponent } from './public/formly/formly-wrapper/formly-wrapper.component';
import { AddDefinitionComponent } from './tems-components/equipment/add-definition/add-definition.component';
import { AddTypeComponent } from './tems-components/equipment/add-type/add-type.component';
import { TagInputModule } from 'ngx-chips';
import { MatChipsModule } from '@angular/material/chips';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatFormFieldModule } from '@angular/material/form-field';
import {MatButtonModule} from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { SelectTooltipComponent } from './public/formly/select-tooltip/select-tooltip.component';
import { InputTooltipComponent } from './public/formly/input-tooltip/input-tooltip.component';
import { MatTooltipModule } from '@angular/material/tooltip';




import {A11yModule} from '@angular/cdk/a11y';
import {ClipboardModule} from '@angular/cdk/clipboard';
import {DragDropModule} from '@angular/cdk/drag-drop';
import {PortalModule} from '@angular/cdk/portal';
import {ScrollingModule} from '@angular/cdk/scrolling';
import {CdkStepperModule} from '@angular/cdk/stepper';
import {CdkTableModule} from '@angular/cdk/table';
import {CdkTreeModule} from '@angular/cdk/tree';
import {MatBadgeModule} from '@angular/material/badge';
import {MatBottomSheetModule} from '@angular/material/bottom-sheet';
import {MatButtonToggleModule} from '@angular/material/button-toggle';
import {MatCardModule} from '@angular/material/card';
import {MatCheckboxModule} from '@angular/material/checkbox';
import {MatStepperModule} from '@angular/material/stepper';
import {MatDatepickerModule} from '@angular/material/datepicker';
import {MatDividerModule} from '@angular/material/divider';
import {MatExpansionModule} from '@angular/material/expansion';
import {MatGridListModule} from '@angular/material/grid-list';
import {MatListModule} from '@angular/material/list';
import {MatMenuModule} from '@angular/material/menu';
import {MatNativeDateModule, MatRippleModule} from '@angular/material/core';
import {MatPaginatorModule} from '@angular/material/paginator';
import {MatProgressBarModule} from '@angular/material/progress-bar';
import {MatProgressSpinnerModule} from '@angular/material/progress-spinner';
import {MatRadioModule} from '@angular/material/radio';
import {MatSelectModule} from '@angular/material/select';
import {MatSidenavModule} from '@angular/material/sidenav';
import {MatSliderModule} from '@angular/material/slider';
import {MatSlideToggleModule} from '@angular/material/slide-toggle';
import {MatSnackBarModule} from '@angular/material/snack-bar';
import {MatSortModule} from '@angular/material/sort';
import {MatTableModule} from '@angular/material/table';
import {MatTabsModule} from '@angular/material/tabs';
import {MatToolbarModule} from '@angular/material/toolbar';
import {MatTreeModule} from '@angular/material/tree';
import {OverlayModule} from '@angular/cdk/overlay';
import { RepeatTypeComponent } from './public/formly/repeat-type/repeat-type.component';
import { AddEquipmentRepeatComponent } from './public/formly/add-equipment-repeat/add-equipment-repeat.component';
import { SummaryEquipmentAnalyticsComponent } from './tems-components/analytics/summary-equipment-analytics/summary-equipment-analytics.component';
import { AgGridEquipmentComponent } from './tems-components/equipment/ag-grid-equipment/ag-grid-equipment.component';
import { EquipmentDetailsComponent } from './tems-components/equipment/equipment-details/equipment-details.component';
import { EquipmentDetailsGeneralComponent } from './tems-components/equipment/equipment-details/equipment-details-general/equipment-details-general.component';
import { EquipmentDetailsLogsComponent } from './tems-components/equipment/equipment-details/equipment-details-logs/equipment-details-logs.component';
import { EquipmentDetailsAllocationsComponent } from './tems-components/equipment/equipment-details/equipment-details-allocations/equipment-details-allocations.component';
import { ImageCarouselComponent } from './public/image-carousel/image-carousel.component';
import { PropertyRenderComponent } from './public/property-render/property-render.component';

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
    ViewEquipmentComponent,
    AddEquipmentComponent,
    QuickAccessComponent,
    ViewRoomsComponent,
    CollegeMapComponent,
    RoomAllocationComponent,
    ViewPersonnelComponent,
    AddPersonnelComponent,
    PersonnelAllocationComponent,
    ViewKeysComponent,
    ViewKeysAllocationsComponent,
    KeysAllocationsComponent,
    ViewIssuesComponent,
    CreateIssueComponent,
    ViewAnnouncementsComponent,
    ViewLogsComponent,
    ViewLibraryComponent,
    ReportsComponent,
    EquipmentManagementComponent,
    UserManagementComponent,
    RoleManagementComponent,
    SystemConfigComponent,
    EquipmentAllocationComponent,
    AnalyticsComponent,
    FormlyWrapperComponent,
    AddTypeComponent,
    AddDefinitionComponent,
    ChipsAutocompleteComponent,
    InputTooltipComponent,
    SelectTooltipComponent,
    ButtonTypeComponent,
    RepeatTypeComponent,
    AddEquipmentRepeatComponent,
    SummaryEquipmentAnalyticsComponent,
    AgGridEquipmentComponent,
    EquipmentDetailsComponent,
    EquipmentDetailsGeneralComponent,
    EquipmentDetailsLogsComponent,
    EquipmentDetailsAllocationsComponent,
    ViewRoomsComponent,
    ImageCarouselComponent,
    PropertyRenderComponent,
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
    HttpClientModule,
    TagInputModule,
    BrowserModule,
    ReactiveFormsModule,
    FormsModule,
    MatChipsModule,
    MatIconModule,
    MatAutocompleteModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatTooltipModule,
    FormlyMaterialModule,
    BrowserAnimationsModule,
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
    AgGridModule
  ],
  exports: [
    MatFormFieldModule, 
    MatInputModule,
    MatInputModule,
    MatButtonModule,
    MatTooltipModule,




    A11yModule,
    ClipboardModule,
    CdkStepperModule,
    CdkTableModule,
    CdkTreeModule,
    DragDropModule,
    MatAutocompleteModule,
    MatBadgeModule,
    MatBottomSheetModule,
    MatButtonModule,
    MatButtonToggleModule,
    MatCardModule,
    MatCheckboxModule,
    MatChipsModule,
    MatStepperModule,
    MatDatepickerModule,
    MatDialogModule,
    MatDividerModule,
    MatExpansionModule,
    MatGridListModule,
    MatIconModule,
    MatInputModule,
    MatListModule,
    MatMenuModule,
    MatNativeDateModule,
    MatPaginatorModule,
    MatProgressBarModule,
    MatProgressSpinnerModule,
    MatRadioModule,
    MatRippleModule,
    MatSelectModule,
    MatSidenavModule,
    MatSliderModule,
    MatSlideToggleModule,
    MatSnackBarModule,
    MatSortModule,
    MatTableModule,
    MatTabsModule,
    MatToolbarModule,
    MatTooltipModule,
    MatTreeModule,
    OverlayModule,
    PortalModule,
    ScrollingModule,
  ],
  providers: [ThemeService, EquipmentService, FormlyParserService],
  bootstrap: [AppComponent],
  entryComponents: [AddTypeComponent]
})
export class AppModule { }
