import { SnackService } from './../../../services/snack/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { IssuesService } from './../../../services/issues-service/issues.service';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { Component, Input, OnInit } from '@angular/core';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';

@Component({
  selector: 'app-profile-tickets',
  templateUrl: './profile-tickets.component.html',
  styleUrls: ['./profile-tickets.component.scss']
})
export class ProfileTicketsComponent extends TEMSComponent implements OnInit {

  @Input() profile: ViewProfile;
  lastClosedIssues: ViewIssueSimplified[];
  lastCreatedIssues: ViewIssueSimplified[];
  lastAssignedIssues: ViewIssueSimplified[];

  constructor(
    prof: ViewProfile,
    private issueService: IssuesService,
    private snackService: SnackService) {
    super();
    console.log(prof);
    this.profile = prof;
  }

  ngOnInit(): void {
    this.subscriptions.push(
      this.issueService.getIssuesOfEntity(
        'user closed', 
        this.profile.id,
        true,
        true,
        'recency closed',
        0, 5)
        .subscribe(result => {
          if(this.snackService.snackIfError(result))
            return;
          
          this.lastAssignedIssues = result; 
        })
    )
  }
}
