import { Component, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { TranslateService } from '@ngx-translate/core';
import { MatTabLazyLoader } from 'src/app/helpers/mat-tab-lazy-loader.helper';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { IOption } from 'src/app/models/option.model';
import { AssetService } from 'src/app/services/asset.service';
import { PersonnelService } from 'src/app/services/personnel.service';
import { RoomsService } from 'src/app/services/rooms.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { EntityIssuesListComponent } from './../../entity-issues-list/entity-issues-list.component';
import { PinnedIssuesComponent } from './../pinned-issues/pinned-issues.component';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule } from '@angular/forms';
import { MatTabsModule } from '@angular/material/tabs';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { SummaryIssuesAnalyticsComponent } from '../../analytics/summary-issues-analytics/summary-issues-analytics.component';
import { ChipsAutocompleteComponent } from '../../../public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-view-issues',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatTabsModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule,
    SummaryIssuesAnalyticsComponent,
    PinnedIssuesComponent,
    ChipsAutocompleteComponent,
    EntityIssuesListComponent
  ],
  templateUrl: './view-issues.component.html',
  styleUrls: ['./view-issues.component.scss']
})
export class ViewIssuesComponent extends TEMSComponent implements OnInit {
  // How pinning / unPinning ticket works:
  // 1) When a ticket is pinned, entityIssuesList will emmit it via output event and will remove from it's
  // list of issues.
  // 2) On initialization, PinnedTicketComponent will search if there is any pinned ticket. If so, it will be displayed.
  // 3)  


  // equipment: Observable<IOption[]>;
  assetId: string = "any";

  // rooms: Observable<IOption[]>;
  roomId: string = "any";
  
  // personnel: Observable<IOption[]>;
  personnelAlreadySelected=[] as IOption[];
  personnelId: string = "any";

  filterIssueFormGroup = new FormGroup({
    equipment: new FormControl(),
    rooms: new FormControl(),
    personnel: new FormControl(),
  })
  matTabLazyLoader = new MatTabLazyLoader(2);

  @ViewChild('pinnedIssues') pinnedIssues: PinnedIssuesComponent;
  @ViewChild('entityOpenIssuesList') entityOpenIssuesList: EntityIssuesListComponent;
  @ViewChild('entityClosedIssuesList') entityClosedIssuesList: EntityIssuesListComponent;

  constructor(
    public assetService: AssetService,
    public roomService: RoomsService,
    public translate: TranslateService,
    public personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit(): void {

  }

  assetSelected(idk){
    let value = this.filterIssueFormGroup.controls.equipment.value;

    if(value[0] != undefined)
      this.assetId = value[0].value;
    else
      this.assetId = 'any';
  }

  roomSelected(idk){
    let value = this.filterIssueFormGroup.controls.rooms.value;

    if(value[0] != undefined)
      this.roomId = value[0].value;
    else
      this.roomId = "any";
  }

  personnelSelected(idk){
    let value = this.filterIssueFormGroup.controls.personnel.value;

    if(value[0] != undefined)
      this.personnelId = value[0].value;
    else
      this.personnelId = 'any';
  }

  // After unpin, the ticket is being included in the common ticket list
  ticketUnpinned(ticket: ViewIssueSimplified){
    if(ticket.dateClosed == undefined){
      this.entityOpenIssuesList.issues.push(ticket);
      return;
    };

    if(this.entityClosedIssuesList != undefined)
      this.entityClosedIssuesList.issues.push(ticket);
  }

  ticketPinned(ticket: ViewIssueSimplified){
    this.pinnedIssues.issues.push(ticket);
  }
}
