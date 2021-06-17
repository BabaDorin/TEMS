import { ClaimService } from './../../../services/claim.service';
import { AgGridPersonnelComponent } from './../ag-grid-personnel/ag-grid-personnel.component';
import { AddPersonnelComponent } from './../add-personnel/add-personnel.component';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { SnackService } from './../../../services/snack/snack.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { EmailService } from 'src/app/services/email.service';
import { SendEmailComponent } from '../../send-email/send-email.component';
import { CreateIssueComponent } from '../../issue/create-issue/create-issue.component';

@Component({
  selector: 'app-view-personnel',
  templateUrl: './view-personnel.component.html',
  styleUrls: ['./view-personnel.component.scss']
})
export class ViewPersonnelComponent implements OnInit {

  @ViewChild('agGridPersonnel') agGridPersonnel: AgGridPersonnelComponent;
  constructor(
    private snackService: SnackService,
    private emaiService: EmailService,
    private dialogService: DialogService,
    private claims: ClaimService
  ) { }

  ngOnInit(): void {
  }

  sendEmail(){
    if(!this.claims.canSendEmails)
      return;
      
    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      SendEmailComponent,
      [{label: "personnel", value: selectedNodes }]
    )
  }

  getSelectedNodes(): IOption[] {
    let selectedNodes = this.agGridPersonnel.getSelectedNodes();

    if (selectedNodes.length == 0)
      return;
    
    if(selectedNodes.length > 100){
      this.snackService.snack({message: "Too much people selcted :(", status: 0})
      return;
    }

    selectedNodes = (this.agGridPersonnel.getSelectedNodes() as ViewPersonnelSimplified[])
      .map(node => ({value: node.id, label: node.name} as IOption));

    return selectedNodes;
  }

  addNew(){
    if(!this.claims.canManage)
      return;

    this.dialogService.openDialog(
      AddPersonnelComponent,
      undefined,
      () => {
        this.agGridPersonnel.fetchPersonnel();
      }
    )
  }

  addIssue(){
    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      CreateIssueComponent,
      [{label: "personnelAlreadySelected", value: selectedNodes }]
    )
  }
}
