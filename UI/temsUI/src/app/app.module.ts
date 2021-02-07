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
    AnalyticsComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    BrowserAnimationsModule,
    FormsModule,
    ReactiveFormsModule,
    ChartsModule,
    FormlyModule.forRoot(),
    FormlyMaterialModule
  ],
  providers: [ThemeService],
  bootstrap: [AppComponent]
})
export class AppModule { }
