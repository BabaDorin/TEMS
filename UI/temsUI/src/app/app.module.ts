import { DialogService } from 'src/app/services/dialog.service';
import { ReactiveFormsModule } from '@angular/forms';
import { DialogModule } from './modules/dialog/dialog.module';
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
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
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
import { ViewLibraryModule } from './modules/library/view-library/view-library.module';
import { QuickAccessModule } from './modules/quick-access/quick-access.module';
import { DashboardComponent } from './public/dashboard/dashboard.component';
import { ClaimService } from './services/claim.service';
import { DefinitionService } from './services/definition.service';
import { EquipmentService } from './services/equipment.service';
import { MenuService } from './services/menu.service';
import { PersonnelService } from './services/personnel.service';
import { RoleService } from './services/role.service';
import { RoomLabelService } from './services/room-label.service';
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
    ],
  imports: [
    LastIssuesSimplifiedModule,
    AppRoutingModule,
    NgbModule,
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
    ViewLibraryModule,
    NgxPaginationModule,

    MatDialogModule,
    ReactiveFormsModule,
    MatButtonModule,
    ChartsModule,
    MatIconModule,
    ViewLibraryModule,
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
    RoomLabelService,
    CurrencyPipe,
    DatePipe,
    PaginatePipe,
    DecimalPipe,
    TranslatePipe,
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
