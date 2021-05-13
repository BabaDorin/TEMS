import { RoomLabelService } from './services/room-label.service';
import { TypeService } from './services/type-service/type.service';
import { MenuService } from './services/menu-service/menu.service';
import { TokenService } from './services/token-service/token.service';
import { AuthInterceptor } from './auth.interceptor';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { EquipmentService } from './services/equipment-service/equipment.service';
import { MatInputModule } from '@angular/material/input';
import { PersonnelService } from './services/personnel-service/personnel.service';
import { RoomsService } from './services/rooms-service/rooms.service';
import { MatFormFieldModule } from '@angular/material/form-field';
import { QuickAccessComponent } from './tems-components/equipment/quick-access/quick-access.component';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
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
import { TodoComponent } from './public/apps/todo-list/todo/todo.component';
import { SpinnerComponent } from './shared/spinner/spinner.component';
import { ContentAnimateDirective } from './shared/directives/content-animate.directive';
import { TodoListComponent } from './public/apps/todo-list/todo-list.component';
import { TagInputModule } from 'ngx-chips';
import { MatDialogModule } from '@angular/material/dialog';
import { TEMSComponent } from './tems/tems.component';
import { TemsFormsModule } from './modules/tems-forms/tems-forms.module';
import { MatSnackBar } from '@angular/material/snack-bar';
import { SnackComponent } from './public/snack/snack.component';
import { DefinitionService } from './services/definition-service/definition.service';
import { ConnectPersonnelUserComponent } from './tems-components/connect-personnel-user/connect-personnel-user.component';
import { CurrencyPipe } from '@angular/common';

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
    QuickAccessComponent,
    TEMSComponent,
    SnackComponent,
    ConnectPersonnelUserComponent,
    ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    NgbModule,
    MatDialogModule,
    BrowserAnimationsModule,
    ChartsModule,
    HttpClientModule,
    BrowserModule,
    BrowserAnimationsModule,
    TagInputModule,
    HttpClientModule,
    MatInputModule,
    MatFormFieldModule,
    TemsFormsModule,
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
    ThemeService,
    RoomsService,
    PersonnelService,
    EquipmentService,
    TokenService,
    MenuService,
    MatSnackBar,
    TypeService,
    DefinitionService,
    RoomLabelService,
    CurrencyPipe,
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
