import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { NgbDropdownConfig } from '@ng-bootstrap/ng-bootstrap';
import { DialogService } from 'src/app/services/dialog.service';
import { SnackService } from '../../services/snack.service';
import { UserService } from '../../services/user.service';
import { ThemeService } from '../../services/theme.service';
import { ViewNotification } from './../../models/communication/notification/view-notification.model';
import { AuthService } from './../../services/auth.service';
import { ViewNotificationsComponent } from './../../tems-components/notifications/view-notifications/view-notifications.component';
import { TEMSComponent } from './../../tems/tems.component';

@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    TranslateModule
  ],
  templateUrl: './navbar.component.html',
  providers: [NgbDropdownConfig]
})
export class NavbarComponent extends TEMSComponent implements OnInit {
  public iconOnlyToggled = false;
  public sidebarToggled = false;
  public username: string = '';
  public profilePhotoB64: string | undefined;
  public loggedIn: boolean = false;
  public refreshing: boolean = false;
  public showUserMenu: boolean = false;
  public showNotifications: boolean = false;

  notifications = [] as ViewNotification[];
  newNotifications = [] as ViewNotification[];

  constructor(
    config: NgbDropdownConfig,
    private route: Router,
    private authService: AuthService,
    private userService: UserService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private themeService: ThemeService) {
      super();
      config.placement = 'bottom-right';
  }

  get isDarkMode(): boolean {
    return this.themeService.isDarkMode;
  }

  toggleDarkMode(): void {
    this.themeService.toggleTheme();
  }

  navigateToLogin() {
    this.authService.logIn();
  }

  toggleUserMenu() {
    this.showUserMenu = !this.showUserMenu;
    if (this.showUserMenu) {
      this.showNotifications = false; // Close notifications if open
    }
  }

  toggleNotifications() {
    this.showNotifications = !this.showNotifications;
    if (this.showNotifications) {
      this.showUserMenu = false; // Close user menu if open
      this.markNotificationsAsSeen();
    }
  }

  signOut() {
    this.showUserMenu = false;
    this.authService.signOut();
  }

  markNotificationsAsSeen(){
    this.newNotifications = this.notifications.filter(q => q.seen == false);
    if(this.newNotifications == undefined || this.newNotifications.length == 0)
      return;


    this.subscriptions.push(
      this.userService.markNotificationsAsSeen(this.newNotifications.map(q => q.id))
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
          
          this.newNotifications.forEach(q => q.seen = true);
          this.newNotifications = [];
      })
    )
  }

  ngOnInit() {
    // Subscribe to authentication state changes (reactive)
    this.subscriptions.push(
      this.authService.isAuthenticated$.subscribe(isAuth => {
        console.log('[Navbar] Auth state changed:', isAuth);
        this.loggedIn = isAuth;
        
        if (isAuth) {
          this.username = this.authService.getUserName();
          this.fetchLastNotifications();
          this.fetchMinifiedProfilePhoto();
        } else {
          this.username = '';
          this.profilePhotoB64 = undefined;
          this.notifications = [];
          this.newNotifications = [];
        }
      })
    );
  }

  fetchMinifiedProfilePhoto(){
    // Temporarily disabled - backend endpoint not yet implemented
    // TODO: Implement profile photo endpoint in backend API
    // this.subscriptions.push(
    //   this.userService.fetchMinifiedProfilePhoto()
    //   .subscribe(result => {
    //     this.profilePhotoB64 = result;
    //   })
    // );
  }

  fetchLastNotifications(refreshTriggered: boolean = false){
    // Temporarily disabled - backend endpoint not yet implemented
    // TODO: Implement notifications endpoint in backend API
    this.refreshing = true;

    // this.subscriptions.push(
    //   this.userService.getLastNotifications()
    //   .subscribe(result => {
    //     if(this.snackService.snackIfError(result))
    //       return;
    //     
    //     this.notifications = result.slice(0, 5);
    //     this.newNotifications = this.notifications.filter(q => q.seen == false);
    //     this.refreshing = false;
    //
    //     if(refreshTriggered)
    //       this.markNotificationsAsSeen();
    //   })
    // );
    
    this.refreshing = false;
  }

  // toggle sidebar in small devices
  toggleOffcanvas() {
    document.querySelector('.sidebar-offcanvas').classList.toggle('active');
  }

  // toggle sidebar
  toggleSidebar() {
    document.querySelector('body').classList.toggle('sidebar-hidden');
  }

  // toggle right sidebar
  toggleRightSidebar() {
    document.querySelector('#right-sidebar').classList.toggle('open');
  }

  displayAllNotifications(){
    this.showNotifications = false;
    this.dialogService.openDialog(
      ViewNotificationsComponent,
      undefined,
      () => {
        this.fetchLastNotifications();
      }
    );
  }

  // Close dropdowns when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const target = event.target as HTMLElement;
    const clickedInside = target.closest('.relative');
    
    if (!clickedInside) {
      this.showUserMenu = false;
      this.showNotifications = false;
    }
  }
}
