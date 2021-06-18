import { ClaimService } from './../../../services/claim.service';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { AddRoomComponent } from './../add-room/add-room.component';
import { SnackService } from './../../../services/snack/snack.service';
import { DialogService } from './../../../services/dialog-service/dialog.service';
import { Component, OnInit, ViewChild } from '@angular/core';
import { AddLogComponent } from '../../communication/add-log/add-log.component';
import { CreateIssueComponent } from '../../issue/create-issue/create-issue.component';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-view-rooms',
  templateUrl: './view-rooms.component.html',
  styleUrls: ['./view-rooms.component.scss']
})
export class ViewRoomsComponent implements OnInit {

  @ViewChild('agGrid') agGridRooms;
  
  constructor(
    private dialogService: DialogService,
    private snackService: SnackService,
    private claims: ClaimService
  ) { 

  }

  ngOnInit(): void {
  }

  addLogSelected(){
    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      AddLogComponent,
      [{label: "room", value: selectedNodes }]
    )
  }

  addIssue(){
    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      CreateIssueComponent,
      [{label: "roomsAlreadySelected", value: selectedNodes }]
    )
  }

  getSelectedNodes(): IOption[] {
    let selectedNodes = this.agGridRooms.getSelectedNodes();

    if (selectedNodes.length == 0)
      return;
    
    if(selectedNodes.length > 20){
      this.snackService.snack({message: "You can not treat more than 20 items at a time", status: 0})
      return;
    }

    selectedNodes = (this.agGridRooms.getSelectedNodes() as ViewRoomSimplified[])
      .map(node => ({value: node.id, label: node.identifier} as IOption));

    return selectedNodes;
  }

  addNew(){
    this.dialogService.openDialog(
      AddRoomComponent,
      undefined,
      () => {
        this.ngOnInit();
      }
    )
  }

  
}
