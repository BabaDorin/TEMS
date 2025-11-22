import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from './../../../services/auth.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule
  ],
  templateUrl: './login.component.html'
})
export class LoginComponent extends TEMSComponent implements OnInit {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) { 
    super();
  }

  ngOnInit() {
    if (this.authService.isLoggedIn()) {
      this.router.navigate(['/tems']);
    }
  }

  loginWithDuende() {
    this.authService.logIn();
  }

  goToRegister() {
    this.router.navigate(['/register']);
  }
}
