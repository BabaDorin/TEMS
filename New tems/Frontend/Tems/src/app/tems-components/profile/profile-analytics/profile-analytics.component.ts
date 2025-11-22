import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { AnalyticsService } from 'src/app/services/analytics.service';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { FractionCardComponent } from '../../analytics/fraction-card/fraction-card.component';
import { SimpleInfoCardComponent } from '../../analytics/simple-info-card/simple-info-card.component';

@Component({
  selector: 'app-profile-analytics',
  standalone: true,
  imports: [
    CommonModule,
    TranslateModule,
    FractionCardComponent,
    SimpleInfoCardComponent
  ],
  templateUrl: './profile-analytics.component.html',
  styleUrls: ['./profile-analytics.component.scss']
})
export class ProfileAnalyticsComponent extends TEMSComponent implements OnInit {

  @Input() profile: ViewProfile;

  // uam stands for user amount
  // am stands for amount
  amOfCreatedIssues = 0;
  // amOfCloseddIssues = 0;
  // amOfOpenIssues = 0;
  
  uamOfCreatedIssues = 0;
  uamOfClosedIssues = 0;
  uamOfTicketsEverClosedByUser = 0;
  uamOfTicketsClosedByUserThatWereReopenedAfterwards = 0;

  probabilityOfTicketReopen = 0;

  constructor(
    private analyticsService: AnalyticsService,
    private snackService: SnackService,
    public translate: TranslateService) {
    super();
  }

  ngOnInit(): void {
    this.getAmountOfCreatedIssues();
    this.getAmountOfClosedIssues();
    this.getTicketsEverCreatedByUser();
    this.getTicketsClosedByUserThatWereReopenedAfterwards();
  }

  getAmountOfCreatedIssues(){
    this.subscriptions.push(
      this.analyticsService.getAmountOfCreatedIssues("user", this.profile.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.uamOfCreatedIssues = result;        
      })
    );

    this.subscriptions.push(
      this.analyticsService.getAmountOfCreatedIssues()
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.amOfCreatedIssues = result;
      })
    )
  }

  getAmountOfClosedIssues(){
    this.subscriptions.push(
      this.analyticsService.getAmountOfClosedIssues("user", this.profile.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.uamOfClosedIssues = result;
      })
    )
  }

  getTicketsEverCreatedByUser(){
    this.subscriptions.push(
      this.analyticsService.getAmountOfTicketsEverCreatedByUser(this.profile.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.uamOfTicketsEverClosedByUser = result;
      })
    )
  }

  // uamOfTicketsClosedByUserThatWereReopenedAfterwards
  getTicketsClosedByUserThatWereReopenedAfterwards(){
    this.subscriptions.push(
      this.analyticsService.getAmountOfTicketsClosedByUserThatWereReopenedAfterwards(this.profile.id)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;
        this.uamOfTicketsClosedByUserThatWereReopenedAfterwards = result;
      })
    )
  }
}
