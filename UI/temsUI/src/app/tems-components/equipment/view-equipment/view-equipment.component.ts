import { ClaimService } from './../../../services/claim.service';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { SnackService } from '../../../services/snack.service';
import { DialogService } from '../../../services/dialog.service';
import { Router } from '@angular/router';
import { IOption } from './../../../models/option.model';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { AgGridEquipmentComponent } from './../ag-grid-equipment/ag-grid-equipment.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { EquipmentAllocationComponent } from '../equipment-allocation/equipment-allocation.component';

@Component({
  selector: 'app-view-equipment',
  templateUrl: './view-equipment.component.html',
  styleUrls: ['./view-equipment.component.scss']
})
export class ViewEquipmentComponent implements OnInit {

  @ViewChild('agGridEquipment') agGridEquipment: AgGridEquipmentComponent;

  includeDerived:boolean = false;

  constructor(
    public dialogService: DialogService,
    public router: Router,
    private snackService: SnackService,
    public claims: ClaimService
  ) {

  }

  ngOnInit(): void {
  }

  addLogSelected(){
    if(!this.claims.canManage)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      AddLogComponent,
      [{label: "equipment", value: selectedNodes }]
    )
  }

  addIssue(){
    if(!this.claims.canManage)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      CreateIssueComponent,
      [{label: "equipmentAlreadySelected", value: selectedNodes }]
    )
  }

  allocateSelected() {
    if(!this.claims.canManage)
      return;

    let selectedNodes = this.getSelectedNodes()
    if(selectedNodes == undefined)
      return;

    this.dialogService.openDialog(
      EquipmentAllocationComponent,
      [{label: "equipment", value: selectedNodes }]
    )
  }

  getSelectedNodes(): IOption[] {
    let selectedNodes = this.agGridEquipment.getSelectedNodes();

    if (selectedNodes.length == 0)
      return;
    
    if(selectedNodes.length > 20){
      this.snackService.snack({message: "You can not treat more than 20 items at a time", status: 0})
      return;
    }

    selectedNodes = (this.agGridEquipment.getSelectedNodes() as ViewEquipmentSimplified[])
      .map(node => ({value: node.id, label: node.temsIdOrSerialNumber} as IOption));

    return selectedNodes;
  }

  addNew(){
    if(!this.claims.canManage)
      return;

    this.router.navigate(["/equipment/add"]);
  }

  toggleIncludeDerived(){
    this.includeDerived = !this.includeDerived;
  }
}
