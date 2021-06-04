import { ViewIssueSimplified } from './../../../models/communication/issues/view-issue-simplified.model';
import { AnalyticsService } from 'src/app/services/analytics-service/analytics.service';
import { IssuesService } from './../../../services/issues-service/issues.service';
import { SnackService } from './../../../services/snack/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-last-issues-simplified',
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
          console.log(result);
          if(this.snackService.snackIfError(result))
            return;

          this.issues = result;
        })
      );
  }
}