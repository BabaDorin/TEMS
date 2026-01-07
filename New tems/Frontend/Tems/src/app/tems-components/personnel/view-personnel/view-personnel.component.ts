import { Component, OnInit, ViewChild } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { EmailService } from 'src/app/services/email.service';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { CreateIssueComponent } from '../../issue/create-issue/create-issue.component';
import { SendEmailComponent } from '../../send-email/send-email.component';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { ClaimService } from './../../../services/claim.service';
import { AddPersonnelComponent } from './../add-personnel/add-personnel.component';
import { AgGridPersonnelComponent } from './../ag-grid-personnel/ag-grid-personnel.component';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-view-personnel',
  standalone: true,
  imports: [
    CommonModule,
    MatCardModule,
    MatButtonModule,
    MatIconModule,
    TranslateModule,
    AgGridPersonnelComponent
  ],
  templateUrl: './view-personnel.component.html',
  styleUrls: ['./view-personnel.component.scss']
})
export class ViewPersonnelComponent implements OnInit {

  @ViewChild('agGridPersonnel') agGridPersonnel: AgGridPersonnelComponent;
  constructor(
    private snackService: SnackService,
    private emaiService: EmailService,
    private dialogService: DialogService,
    public claims: ClaimService
  ) { }

  ngOnInit(): void {
  }

  sendEmail(){
    if(!this.claims.canManageAssets)
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
    if(!this.claims.canManageAssets)
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
