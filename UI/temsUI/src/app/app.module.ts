import { ConfirmModule } from './modules/confirm/confirm.module';
import { DownloadService } from './download.service';
import { LazyLoaderService } from './services/lazy-loader.service';
import { DialogService } from 'src/app/services/dialog.service';
import { ReactiveFormsModule } from '@angular/forms';
import { MatMenuModule } from '@angular/material/menu';
import { CurrencyPipe, DatePipe, DecimalPipe } from '@angular/common';
import { HttpClient, HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatButtonModule } from '@angular/material/button';
import { MatDialogModule } from '@angular/material/dialog';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { TranslateLoader, TranslateModule, TranslatePipe } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';
import { ChartsModule, ThemeService } from 'ng2-charts';
import { PaginatePipe, NgxPaginationModule } from 'ngx-pagination';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { AuthInterceptor } from './auth.interceptor';
import { AnnouncementsListModule } from './modules/announcement/announcements-list/announcements-list.module';
import { ChipsAutocompleteModule } from './modules/chips-autocomplete/chips-autocomplete.module';
import { LastIssuesSimplifiedModule } from './modules/issues/last-issues-simplified/last-issues-simplified.module';
import { QuickAccessModule } from './modules/quick-access/quick-access.module';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { ClaimService } from './services/claim.service';
import { DefinitionService } from './services/definition.service';
import { EquipmentService } from './services/equipment.service';
import { MenuService } from './services/menu.service';
import { PersonnelService } from './services/personnel.service';
import { RoleService } from './services/role.service';
import { RoomsService } from './services/rooms.service';
import { TokenService } from './services/token.service';
import { TypeService } from './services/type.service';
import { ContentAnimateDirective } from './shared/directives/content-animate.directive';
import { FooterComponent } from './shared/footer/footer.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { SidebarComponent } from './shared/sidebar/sidebar.component';
import { SnackComponent } from './shared/snack/snack.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { LastCreatedTicketsChartComponent } from './tems-components/analytics/last-created-tickets-chart/last-created-tickets-chart.component';
import { ViewNotificationsComponent } from './tems-components/notifications/view-notifications/view-notifications.component';
import { TEMSComponent } from './tems/tems.component';
import { UserCardsModule } from './user-cards/user-cards.module';
import { TemsFormsModule } from './modules/tems-forms/tems-forms.module';
import { NgbCollapseModule, NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { DefectCellRenderedComponent } from './public/ag-grid/defect-cell-rendered/defect-cell-rendered.component';
import { UsedCellRenderedComponent } from './public/ag-grid/used-cell-rendered/used-cell-rendered.component';
import { FindByTemsidComponent } from './tems-components/equipment/find-by-temsid/find-by-temsid.component';

export function HttpLoaderFactory(http: HttpClient){
  return new TranslateHttpLoader(http);
}

@NgModule({
  declarations: [
    AppComponent,
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    DashboardComponent,
    SpinnerComponent,
    ContentAnimateDirective,
    TEMSComponent,
    SnackComponent,
    LastCreatedTicketsChartComponent,
    ViewNotificationsComponent,
    UsedCellRenderedComponent,
    DefectCellRenderedComponent,
    FindByTemsidComponent
    ],
  imports: [
    LastIssuesSimplifiedModule,
    AppRoutingModule,
    MatMenuModule,
    BrowserAnimationsModule,
    ChartsModule,
    BrowserModule,
    HttpClientModule,
    MatButtonModule,
    UserCardsModule,
    AnnouncementsListModule,
    ChipsAutocompleteModule,
    QuickAccessModule,
    NgxPaginationModule,
    ConfirmModule,
    MatDialogModule,
    ReactiveFormsModule,
    MatButtonModule,
    ChartsModule,
    MatIconModule,
    NgbDropdownModule,
    NgbCollapseModule,
    TemsFormsModule,
    TranslateModule.forRoot({
      loader: {
        provide: TranslateLoader,
        useFactory: HttpLoaderFactory,
        deps: [HttpClient]
      }
    })
  ],
  exports:[
    MatAutocompleteModule,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    RoleService,
    ThemeService,
    DialogService,
    RoomsService,
    PersonnelService,
    EquipmentService,
    TokenService,
    ClaimService,
    MenuService,
    MatSnackBar,
    TypeService,
    DefinitionService,
    CurrencyPipe,
    DatePipe,
    PaginatePipe,
    DecimalPipe,
    TranslatePipe,
    LazyLoaderService,
    DownloadService
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
