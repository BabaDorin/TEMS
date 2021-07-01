import { Component, EventEmitter, Input, OnChanges, OnInit, Output, ViewChild } from '@angular/core';
import * as confetti from 'canvas-confetti';
import { ViewIssueSimplified } from 'src/app/models/communication/issues/view-issue-simplified.model';
import { IOption } from 'src/app/models/option.model';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { DialogService } from '../../services/dialog.service';
import { IssuesService } from '../../services/issues.service';
import { SnackService } from '../../services/snack.service';
import { ClaimService } from './../../services/claim.service';
import { CreateIssueComponent } from './../issue/create-issue/create-issue.component';

@Component({
  selector: 'app-entity-issues-list',
  templateUrl: './entity-issues-list.component.html',
  styleUrls: ['./entity-issues-list.component.scss'],
})
export class EntityIssuesListComponent extends TEMSComponent implements OnInit, OnChanges {

  @Input() issues: ViewIssueSimplified[];

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
    public claims: ClaimService,
  ) { 
    super();
  }
 
  ngOnInit(): void {
    this.loading = true;
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
    this.endPoint =  this.issuesService.getIssues(
      this.equipmentId ?? "any", 
      this.roomId ?? "any", 
      this.personnelId ?? "any", 
      this.includingClosed, this.onlyClosed);

    this.subscriptions.push(
      this.endPoint
      .subscribe(result  => {
        this.loading = false;

        if(this.snackService.snackIfError(this.issues))
          return;
        
        let auxIssues: ViewIssueSimplified[] = result;

        if(!this.includePinned)
          this.issues = auxIssues.filter(q => !q.isPinned);
        else
          this.issues = auxIssues;
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
    if(this.includePinned)
      return;

    this.pinned.emit(eventData);
    this.issues.splice(index, 1);
  }

  solve(index){
    if(!this.claims.canManage)
      return;

    let selectedIssue = this.issues[index];
    let d1 = new Date(selectedIssue.dateClosed);
    let d2 = new Date(selectedIssue.dateCreated);
    let difference = Math.abs(d1.getTime() - d2.getTime()) / 36e5;
    this.snackService.snack({message: "🎉🎉 Let's close them all!", status: 1}, 'default-snackbar')

    if(difference <= 24)
      this.launchConfetti();
  }

  addIssue(){
    this.dialogService.openDialog(
      CreateIssueComponent,
      [
        { label: "equipmentAlreadySelected", value: this.equipment},
        { label: "roomsAlreadySelected", value: this.rooms},
        { label: "personnelAlreadySelected", value: this.personnel},
      ],
      () => {
        this.getIssues();  
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

  // BEFREE - Needs improvement
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
      this.snackService.snack({message: ":)", status: 0});
      return;
    }
  
    confetti.create(undefined, { resize: true, useWorker: true })({
      particleCount: 130,
      spread: 130,
      origin: { y: 0.6 }
    });
  }
}

