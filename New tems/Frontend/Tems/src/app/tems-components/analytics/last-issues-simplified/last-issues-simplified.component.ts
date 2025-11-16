import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { TranslateModule } from '@ngx-translate/core';
import { AnalyticsService } from 'src/app/services/analytics.service';
import { IssuesService } from '../../../services/issues.service';
import { SnackService } from '../../../services/snack.service';
import { ViewIssueSimplified } from './../../../models/communication/issues/view-issue-simplified.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { IssueContainerSimplifiedComponent } from '../../issues/issue-container-simplified/issue-container-simplified.component';

@Component({
  selector: 'app-last-issues-simplified',
  standalone: true,
  imports: [CommonModule, MatCardModule, TranslateModule, IssueContainerSimplifiedComponent],
  templateUrl: './last-issues-simplified.component.html',
  styleUrls: ['./last-issues-simplified.component.scss']
})
export class LastIssuesSimplifiedComponent extends TEMSComponent implements OnInit {

  @Input() canManage: boolean = false;
  @Input() take: number = 3;
  @Input() simplifiedView: boolean = true;
  
  issues: ViewIssueSimplified[] = [];

  constructor(
    private snackService: SnackService,
    private issuesService: IssuesService,
    private analyticsService: AnalyticsService
  ) {
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.issuesService.getIssuesOfEntity(
        'any', 'any', true, false, 'recency', 0, this.take)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;

          this.issues = result;
        })
      );
  }
}
