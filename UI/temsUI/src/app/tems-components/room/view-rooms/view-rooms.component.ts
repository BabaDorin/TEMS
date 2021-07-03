import { AgGridRoomsComponent } from './../ag-grid-rooms/ag-grid-rooms.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { AddLogComponent } from '../../communication/add-log/add-log.component';
import { CreateIssueComponent } from '../../issue/create-issue/create-issue.component';
import { ViewRoomSimplified } from './../../../models/room/view-room-simplified.model';
import { ClaimService } from './../../../services/claim.service';
import { AddRoomComponent } from './../add-room/add-room.component';

@Component({
  selector: 'app-view-rooms',
  templateUrl: './view-rooms.component.html',
  styleUrls: ['./view-rooms.component.scss']
})
export class ViewRoomsComponent implements OnInit {

  @ViewChild('agGrid') agGridRooms: AgGridRoomsComponent;
  
  constructor(
    private dialogService: DialogService,
    private snackService: SnackService,
    public claims: ClaimService
  ) { 

  }

  ngOnInit(): void {

  }

  fetchRooms(){
    this.agGridRooms.fetchRooms();
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
        this.fetchRooms();
      }
    )
  }

  
}
