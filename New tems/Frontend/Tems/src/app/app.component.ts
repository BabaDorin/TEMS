import { LazyLoaderService } from 'src/app/services/lazy-loader.service';
import { SnackService } from './services/snack.service';
import { UserService } from 'src/app/services/user.service';
import { Component, HostListener, OnInit } from '@angular/core';
import { NavigationEnd, NavigationStart, RouteConfigLoadEnd, RouteConfigLoadStart, Router } from '@angular/router';
import { TranslateService } from '@ngx-translate/core';
import { DialogService } from 'src/app/services/dialog.service';
import { SystemConfigurationService } from 'src/app/services/system-configuration.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Md5 } from 'ts-md5';
import { TokenService } from './services/token.service';
import { ViewLibraryComponent } from './tems-components/library/view-library/view-library.component';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FooterComponent } from './shared/footer/footer.component';
import { NavbarComponent } from './shared/navbar/navbar.component';
import { SidebarComponent } from './shared/sidebar/sidebar.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, RouterModule, FooterComponent, NavbarComponent, SidebarComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent extends TEMSComponent implements OnInit{
  title = 'Technical Asset Management System';

  showSidebar: boolean = true;
  showNavbar: boolean = true;
  showFooter: boolean = true;
  isLoading: boolean;

  constructor(
    public translate: TranslateService,
    private router: Router,
    private dialogService: DialogService,
    private tokenService: TokenService,
    private systemConfigurationService: SystemConfigurationService,
    private userService: UserService,
    private lazyLoad: LazyLoaderService,
    private snackService: SnackService) {
    
    super();
    this.setupBrowserlang();

    // Removing Sidebar, Navbar, Footer for Documentation, Error and Auth pages
    // When needed (For example, when loading the page...), Feel free to add more cases here.
    router.events.forEach((event) => { 
      if(event instanceof NavigationStart) {
        if((event['url'] == '/auth/login') || (event['url'] == '/auth/register') || (event['url'] == '/error-pages/404') || (event['url'] == '/error-pages/500') || (event['url'] == '/error-pages/403')) {
          this.showSidebar = false;
          this.showNavbar = false;
          this.showFooter = false;
          document.querySelector('.main-panel').classList.add('w-100');
          document.querySelector('.page-body-wrapper').classList.add('full-page-wrapper');
          document.querySelector('.content-wrapper').classList.remove('auth', 'auth-img-bg', );
          document.querySelector('.content-wrapper').classList.remove('auth', 'lock-full-bg');
          if((event['url'] == '/error-pages/404') || (event['url'] == '/error-pages/500')) {
            document.querySelector('.content-wrapper').classList.add('p-0');
          }
        } else {
          this.showSidebar = true;
          this.showNavbar = true;
          this.showFooter = true;
          document.querySelector('.main-panel').classList.remove('w-100');
          document.querySelector('.page-body-wrapper').classList.remove('full-page-wrapper');
          document.querySelector('.content-wrapper').classList.remove('auth', 'auth-img-bg');
          document.querySelector('.content-wrapper').classList.remove('p-0');
        }
      }
    });

    // Spinner for lazyload modules - Called automatically when RouteConfigLoadStart event
    // is fired
    router.events.forEach((event) => { 
      if (event instanceof RouteConfigLoadStart) {
          this.isLoading = true;
      } else if (event instanceof RouteConfigLoadEnd) {
          this.isLoading = false;
      }
    });
  }

  setupBrowserlang(){
    this.translate.addLangs(['en', 'ro']);
    this.translate.setDefaultLang('en');

    if(!this.tokenService.tokenExists()){
      // user not logged in
      this.translate.use(this.getBrowserLang());
      return;
    }

    // Use browser language - user preference endpoint not implemented
    this.translate.use(this.getBrowserLang());
  }

  getBrowserLang(): string{
    const browserLang = this.translate.getBrowserLang(); 
    return browserLang.match(/en|ro/) ? browserLang: 'ro';
  }
  
  ngOnInit() {
    // Scroll to top after route change
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
          return;
      }
      window.scrollTo(0, 0);
    });

    // Library password feature disabled - endpoint not implemented
    // if (this.tokenService.tokenExists()) {
    //   this.libraryPassword = 'disabled';
    // }
  }

  @HostListener('document:keypress', ['$event'])
  handleKeyboardEvent(event: KeyboardEvent) { 
    if(this.libraryPassword == undefined)
      return;

    if(event.key == '`')
    {
      this.lastPressedKeys = '';
      this.listen = !this.listen;
    }

    if(this.listen){
      if(this.lastPressedKeys.length == 0 && event.key == '`')
        return;

      this.lastPressedKeys += event.key;

      if(Md5.hashStr(this.lastPressedKeys) == this.libraryPassword){
        this.listen = false;
        this.lazyLoad.loadModuleAsync('library/library.module.ts').then(() => {
          this.dialogService.openDialog(
            ViewLibraryComponent,
            [{ label: 'accessPass', value: this.lastPressedKeys}]
          );
        });
      }
    }
  }

  lastPressedKeys = '';
  listen = false;
  libraryPassword;
}
