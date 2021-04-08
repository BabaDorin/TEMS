import { SnackService } from 'src/app/services/snack/snack.service';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';
import { ViewKeySimplified } from 'src/app/models/key/view-key.model';
import { AgGridKeysComponent } from 'src/app/tems-components/keys/ag-grid-keys/ag-grid-keys.component';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { IOption } from 'src/app/models/option.model';
import { AddKeyComponent } from '../add-key/add-key.component';

@Component({
  selector: 'app-view-keys',
  templateUrl: './view-keys.component.html',
  styleUrls: ['./view-keys.component.scss']
})
export class ViewKeysComponent extends TEMSComponent implements OnInit {

  @ViewChild('agGridKeys') agGridKeys: AgGridKeysComponent;
  unallocatedKeys: ViewKeySimplified[] = [];
  allocatedKeys: ViewKeySimplified[] = [];

  constructor(
    private keysService: KeysService,
    private dialogService: DialogService,
    private snackService: SnackService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.getKeys();
  }

  getKeys(){
    let keys = [] as ViewKeySimplified[];
    
    this.subscriptions.push(this.keysService.getKeys()
      .subscribe(result => {
        keys = result;

        this.unallocatedKeys = keys.filter(key => key.allocatedTo.value == "--");
        this.allocatedKeys = keys.filter(key => key.allocatedTo.value != "--");
      }));
  }

  allocateSelectedKeys(){
    let selectedNodes = (this.agGridKeys.getSelectedNodes() as ViewKeySimplified[])
      .map(node => ({value: node.id, label: node.identifier} as IOption));

    if (selectedNodes.length == 0)
      return;

    this.allocateKeys(selectedNodes);
  }

  allocateKeys(keys: IOption[]){
    this.dialogService.openDialog(
      KeysAllocationsComponent,
      [{label: "keysAlreadySelectedOptions", value: keys }],
      () => {
        this.getKeys();
      }
    )
  }

  addKey(){
    this.dialogService.openDialog(
      AddKeyComponent,
      undefined,
      () => {
        this.getKeys();
      }
    )
  }

  keyReturned(keyData){
    this.getKeys(); // NO!
  }
}
