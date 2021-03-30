import { DialogService } from './../../../services/dialog-service/dialog.service';
import { ViewPersonnelSimplified } from './../../../models/personnel/view-personnel-simplified.model';
import { SnackService } from './../../../services/snack/snack.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { EmailService } from 'src/app/services/email.service';
import { SendEmailComponent } from '../../send-email/send-email.component';

@Component({
  selector: 'app-view-personnel',
  templateUrl: './view-personnel.component.html',
  styleUrls: ['./view-personnel.component.scss']
})
export class ViewPersonnelComponent implements OnInit {

  @ViewChild('agGridPersonnel') agGridPersonnel;
  constructor(
    private snackService: SnackService,
    private emaiService: EmailService,
    private dialogService: DialogService
  ) { }

  ngOnInit(): void {
  }

  sendEmail(){
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
}
