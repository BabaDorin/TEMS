import { CAN_MANAGE_ENTITIES } from './../../models/claims';
import { TokenService } from './../../services/token-service/token.service';
import { SnackService } from './../../services/snack/snack.service';
import { Router } from '@angular/router';
import { DialogService } from './../../services/dialog-service/dialog.service';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';
import { IssuesService } from './../../services/issues-service/issues.service';
import { Component, Input, OnInit, OnChanges, ViewChild, Output } from '@angular/core';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import * as confetti from 'canvas-confetti';
import EventEmitter from 'events';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss'],
})
export class EntityIssuesListComponent extends TEMSComponent implements OnInit, OnChanges {

  // for ticket fetching
  @Input() equipmentId;
  @Input() personnelId;
  @Input() roomId;
  @Input() addIssueEnabled: boolean = true;
  @Input() endPoint;

  // for AddIssueComponent
  @Input() equipment: IOption[] = [];
  @Input() rooms: IOption[] = [];
  @Input() personnel: IOption[] =[];

  @Input() onlyClosed: boolean = false;
  @Input() includingClosed: boolean = false;
  @Input() showIncludeClosed: boolean = true;
  @Input() includePinned: boolean = true;

  @ViewChild('includeClosedToggle') includeClosedToggle;
  @ViewChild('issuesPanel', { static: true }) issuesPanel;

  @Output() pinned = new EventEmitter();

  public issues: ViewIssueSimplified[];
  canManage = false;
  statuses: IOption[];
  loading = true;
  pageNumber = 1;

  confettiAbused:number = 0;
  timeOfLastFiredConfetti: Date;
  confettiCanceled = false;

  constructor(
    private issuesService: IssuesService,
    private dialogService: DialogService,
    private snackService: SnackService,
    private router: Router,
    private tokenService: TokenService
  ) { 
    super();
  }
 
  ngOnInit(): void {
    this.loading = true;
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES);
    this.getStatuses();
    this.getIssues();  
  }

  ngOnChanges(): void {
    if(this.cancelFirstOnChange){
      this.cancelFirstOnChange = false;
      return;
    }
    
    this.getIssues();
  }

  getIssues(){
    this.loading = true;

    this.endPoint =  this.issuesService.getIssues(
      this.equipmentId ?? "any", 
      this.roomId ?? "any", 
      this.personnelId ?? "any", 
      this.includingClosed, this.onlyClosed);

    this.subscriptions.push(
      this.endPoint
      .subscribe(result => {
        if(this.snackService.snackIfError(this.issues))
          return;
        
        this.issues = result;
        this.loading = false;
      }))
  }

  sortBy(criterium: string){
    if(criterium == 'date')
    {
      this.issues = this.issues.sort((a,b)=> new Date(a.dateCreated).getTime() - new Date(b.dateCreated).getTime());
      return;
    }

    this.issues = this.issues.sort((a,b) => (parseInt(a.status.additional) > parseInt(b.status.additional)) ? 1 : ((b.status.additional > a.status.additional) ? -1 : 0));
  }

  getStatuses(){
    this.subscriptions.push(
      this.issuesService.getStatuses()
      .subscribe(result => {
        this.statuses = result;
      })
    )
  }

  ticketPinned(eventData, index){
    if(!this.includePinned)
      return;

    this.pinned.emit(eventData);
    this.issues.splice(index, 1);
  }

  solve(index){
    let selectedIssue = this.issues[index];
      selectedIssue.dateClosed = new Date;
      let d1 = new Date(selectedIssue.dateClosed);
      let d2 = new Date(selectedIssue.dateCreated);
      let difference = Math.abs(d1.getTime() - d2.getTime()) / 36e5;
      this.snackService.snack({message: "🎉🎉 Let's close them all!", status: 1}, 'default-snackbar')

    if(difference <= 24)
      this.launchConfetti();
  }

  private addIssue(){
    this.dialogService.openDialog(
      CreateIssueComponent,
      [
        { label: "equipmentAlreadySelected", value: this.equipment},
        { label: "roomsAlreadySelected", value: this.rooms},
        { label: "personnelAlreadySelected", value: this.personnel},
      ],
      () => {
        this.ngOnInit();
      }
    )
  }

  includeClosedChanged(){
    this.includingClosed = !this.includingClosed;
    this.getIssues()
  }

  remove(index){
    this.issues.splice(index, 1);
  }

  launchConfetti(){
    if(this.confettiCanceled)
      return;

    if(this.timeOfLastFiredConfetti == undefined)
      this.timeOfLastFiredConfetti = new Date;
    else{
      if(Math.abs((new Date().getTime() - this.timeOfLastFiredConfetti.getTime()) / 1000) < 10){
        this.confettiAbused++;
        if(this.confettiAbused >= 3)
          this.confettiCanceled = true;
      }
      else
        this.confettiAbused = 0;
    }


    if(this.confettiCanceled){
      this.snackService.snack({message: "Țajiunji.", status: 0});
      return;
    }
  
    confetti.create(undefined, { resize: true, useWorker: true })({
      particleCount: 130,
      spread: 130,
      origin: { y: 0.6 }
    });
  }
}

