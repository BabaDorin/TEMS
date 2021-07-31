import { BugReportComponent } from './../../tems-components/forms/bug-report/bug-report.component';
import { LazyComponentService } from './../../services/lazy-loader.service';
import { DialogService } from './../../services/dialog.service';
import { Component, OnInit, ViewContainerRef } from '@angular/core';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {

  currentYear: number = 2021;

  constructor(
    private dialgoService: DialogService,
    private lazyLoader: LazyComponentService,
    public vcRef: ViewContainerRef
  ) { 
  }

  ngOnInit() {
    this.currentYear = new Date().getFullYear()
  }

  async reportBug(){
    await this.lazyLoader.loadComponent('bug-report/bug-report.module', this.vcRef);
    this.dialgoService.openDialog(BugReportComponent);
  }
}
