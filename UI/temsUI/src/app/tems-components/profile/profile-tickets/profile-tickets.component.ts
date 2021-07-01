import { Component, Inject, OnInit } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { IssuesService } from '../../../services/issues.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';

@Component({
  selector: 'app-profile-tickets',
  templateUrl: './profile-tickets.component.html',
  styleUrls: ['./profile-tickets.component.scss']
})
export class ProfileTicketsComponent extends TEMSComponent implements OnInit {

  profile: ViewProfile;
  lastClosedIssues: ViewIssueSimplified[] = [];
  closedIssuesEndPoint;
  showMoreClosed: boolean = false;

  lastCreatedIssues: ViewIssueSimplified[] = [];
  createdIssuesEndPoint;
  showMoreCreated: boolean = false;

  lastAssignedIssues: ViewIssueSimplified[] = [];
  assignedIssuesEndPoint;
  showMoreAssigned: boolean = false;
  isCurrentUser: boolean;

  constructor(
    @Inject(ViewProfile) prof,
    @Inject(Boolean) isCurrentUser,
    private issueService: IssuesService,
    private snackService: SnackService) {
    super();
    console.log(prof);
    this.profile = prof;
    this.isCurrentUser = isCurrentUser;
  };

  ngOnInit(): void {
    this.closedIssuesEndPoint = this.issueService.getIssuesOfEntity('user closed', this.profile.id, true, true, 'recency closed');
    this.createdIssuesEndPoint = this.issueService.getIssuesOfEntity('user created', this.profile.id, true, false, 'recency');
    this.assignedIssuesEndPoint = this.issueService.getIssuesOfEntity('user assigned', this.profile.id, true, false, 'priority');

    this.subscriptions.push(
      this.issueService.getIssuesOfEntity('user closed', this.profile.id, true, true, 'recency closed', 0, 5)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.lastClosedIssues = result;
        }),

      this.issueService.getIssuesOfEntity('user created', this.profile.id, true, false, 'recency', 0, 5)
        .subscribe(result => {
          console.log('ets');
          console.log(result);
          if (this.snackService.snackIfError(result))
            return;

          this.lastCreatedIssues = result;
        }),

      this.issueService.getIssuesOfEntity('user assigned', this.profile.id, true, false, 'priority', 0, 5)
        .subscribe(result => {
          if (this.snackService.snackIfError(result))
            return;

          this.lastAssignedIssues = result;
        }),
    )
  };
}
