import { LazyLoaderService } from './../../../services/lazy-loader.service';
import { ReportFromFilterComponent } from './../../reports/report-from-filter/report-from-filter.component';
import { TemplateFromFilter } from './../../../models/report/add-report.model';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { MultipleSelectionDropdownComponent } from './../../../shared/forms/multiple-selection-dropdown/multiple-selection-dropdown.component';
import { TypeService } from './../../../services/type.service';
import { TypeEndpoint } from './../../../helpers/endpoints/type.endpoint';
import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { ViewEquipmentSimplified } from 'src/app/models/equipment/view-equipment-simplified.model';
import { AddLogComponent } from 'src/app/tems-components/communication/add-log/add-log.component';
import { CreateIssueComponent } from 'src/app/tems-components/issue/create-issue/create-issue.component';
import { DialogService } from '../../../services/dialog.service';
import { SnackService } from '../../../services/snack.service';
import { EquipmentAllocationComponent } from '../equipment-allocation/equipment-allocation.component';
import { IOption } from './../../../models/option.model';
import { ClaimService } from './../../../services/claim.service';
import { AgGridEquipmentComponent } from './../ag-grid-equipment/ag-grid-equipment.component';

@Component({
  selector: 'app-view-equipment',
  templateUrl: './view-equipment.component.html',
  styleUrls: ['./view-equipment.component.scss']
})
export class ViewEquipmentComponent implements OnInit {

  // typePreOptions: IOption[] = [];
  typeEndpoint: TypeEndpoint;
  equipmentFilter: EquipmentFilter;

  @ViewChild('typeSelection') typeSelection: MultipleSelectionDropdownComponent;
  @ViewChild('agGridEquipment') agGridEquipment: AgGridEquipmentComponent;
  includeDerived:boolean = false;

  constructor(
    public dialogService: DialogService,
    public router: Router,
    private snackService: SnackService,
    public claims: ClaimService,
    public translate: TranslateService,
    private typeService: TypeService,
    private lazyLoader: LazyLoaderService
  ) {
    this.typeEndpoint = new TypeEndpoint(this.typeService, this.includeDerived);

    this.equipmentFilter = new EquipmentFilter();
    this.equipmentFilter.includeChildren = this.includeDerived;
  }

  ngOnInit(): void {
  }

  typesChanged(eventData){
    if(eventData == undefined)
      return;
    
    this.equipmentFilter.types = eventData;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
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

  async generateReport(){
    await this.lazyLoader.loadModuleAsync('reports/report-from-filter.module.ts');
    this.dialogService.openDialog(
      ReportFromFilterComponent,
      [
        { label: 'equipmentFilter', value: this.equipmentFilter }
      ]
    );
  }

  includeDerivedChanged(){
    this.equipmentFilter.includeChildren = this.includeDerived;
    this.equipmentFilter = Object.assign(new EquipmentFilter(), this.equipmentFilter);
    this.typeEndpoint = new TypeEndpoint(this.typeService, this.includeDerived);
  }
}
