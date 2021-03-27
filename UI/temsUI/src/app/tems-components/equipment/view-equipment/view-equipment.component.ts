import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { SnackService } from './../../../services/snack/snack.service';
import { DialogService } from './../../../services/dialog-service/dialog.service';
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

  includeDerived = false;

  @ViewChild('agGridEquipment') agGridEquipment: AgGridEquipmentComponent;

  constructor(
    public dialogService: DialogService,
    public router: Router,
    private snackService: SnackService
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
      [{label: "equipment", value: selectedNodes }]
    )
  }

  allocateSelected() {
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
      this.snackService.snack({message: "You can add a log to more that 20 equipments", status: 0})
      return;
    }

    selectedNodes = (this.agGridEquipment.getSelectedNodes() as ViewEquipmentSimplified[])
      .map(node => ({value: node.id, label: node.temsIdOrSerialNumber} as IOption));

    return selectedNodes;
  }

  addNew(){
    this.router.navigate(["/equipment/add"]);
  }
}
