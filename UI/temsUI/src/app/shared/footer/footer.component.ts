import { TokenService } from './../../services/token.service';
import { BugReportComponent } from './../../tems-components/forms/bug-report/bug-report.component';
import { LazyLoaderService } from './../../services/lazy-loader.service';
import { DialogService } from './../../services/dialog.service';
import { Component, OnInit, ViewContainerRef } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {

  currentYear: number = 2021;
  bugReportOn: boolean = false;

  constructor(
    private dialgoService: DialogService,
    private lazyLoader: LazyLoaderService,
    private tokenService: TokenService
  ) { 
  }

  ngOnInit() {
    this.currentYear = new Date().getFullYear()
  }

  async reportBug(){
    await this.lazyLoader.loadModule('bug-report/bug-report.module');
    this.dialgoService.openDialog(BugReportComponent);
  }

  toggleBugReporting(){
    // user not logged in
    if(!this.tokenService.tokenExists()){
      this.bugReportOn = false;
      return;
    }

    this.bugReportOn = !this.bugReportOn;
  }
}
