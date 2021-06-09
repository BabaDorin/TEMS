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

  @ViewChild('unallocated') agGridUnallocatedKeys: AgGridKeysComponent;
  @ViewChild('allocated') agGridAllocatedKeys: AgGridKeysComponent;

  unallocatedKeys: ViewKeySimplified[] = [];
  allocatedKeys: ViewKeySimplified[] = [];
  loading: boolean = true;

  constructor(
    private keysService: KeysService,
    private dialogService: DialogService,
    private snackService: SnackService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.getAndDistributeKeys();
  }

  getAndDistributeKeys(){
    let keys = [] as ViewKeySimplified[];
    
    this.loading = true;

    this.subscriptions.push(
      this.keysService.getKeys()
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;
        
        keys = result;

        this.unallocatedKeys = keys.filter(key => key.allocatedTo.value == "--");
        this.allocatedKeys = keys.filter(key => key.allocatedTo.value != "--");
      }));
  }

  allocateSelectedKeys(){
    let selectedNodes = (this.agGridUnallocatedKeys.getSelectedNodes() as ViewKeySimplified[])
      .map(node => ({value: node.id, label: node.identifier} as IOption));

    if (selectedNodes.length == 0)
      return;

    this.agGridUnallocatedKeys.allocateKeys(selectedNodes);
  }

  keyAllocated(key: ViewKeySimplified){
    key.timePassed = 'recently allocated';
    // this.allocatedKeys = [key, ...this.agGridAllocatedKeys.keys];
    this.agGridAllocatedKeys.pushKey(key);
  }

  addKey(){
    this.dialogService.openDialog(
      AddKeyComponent,
      undefined,
      () => {
        this.getAndDistributeKeys();
      }
    )
  }

  singleKeyReturned(returnedKey: ViewKeySimplified){
    this.agGridUnallocatedKeys.pushKey(returnedKey);
  }

  returnMultipleKeys(){
    let selectedKeys = this.agGridAllocatedKeys.getSelectedNodes();

    if (selectedKeys.length == 0)
      return;

    this.agGridAllocatedKeys.returnKeys(selectedKeys);
  }
}
