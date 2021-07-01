import { Component, Input, OnInit } from '@angular/core';
import { ViewKeyAllocation } from 'src/app/models/key/view-key-allocation.model';
import { DialogService } from 'src/app/services/dialog.service';
import { KeysService } from 'src/app/services/keys.service';
import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { SnackService } from '../../../services/snack.service';

@Component({
  selector: 'app-keys-allocations-list',
  templateUrl: './keys-allocations-list.component.html',
  styleUrls: ['./keys-allocations-list.component.scss']
})
export class KeysAllocationsListComponent extends TEMSComponent implements OnInit {

  @Input() keyId: string;
  @Input() roomId: string;
  @Input() personnelId: string;
  @Input() canManage: boolean;

  allocations: ViewKeyAllocation[];
  cancelOnChange: boolean = true;
  loading: boolean = true;
  pageNumber = 1;

  constructor(
    private keyService: KeysService,
    private snackService: SnackService,
    private dialogService: DialogService,
  ) { 
    super();
  }

  ngOnInit(): void {
    if(this.cancelOnChange){
      this.cancelOnChange = false;
      return;
    }

    this.getAllocations();
  }

  ngOnChanges(): void {
    this.getAllocations();
  }

  getAllocations(){
    this.loading = true;
    this.subscriptions.push(this.keyService.getAllocations(this.keyId, this.roomId, this.personnelId)
      .subscribe(result => {
        this.loading = false;
        if(this.snackService.snackIfError(result))
          return;

        this.allocations = result;
      }));
  }

  delete(allocationId: string, index: number){
    if(!confirm('Are you sure you want to remove that allocation?'))
      return;
    
    this.subscriptions.push(
      this.keyService.archieveAllocation(allocationId)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.allocations.splice(index, 1);
      })
    )
  }

  createAllocation(){
    this.dialogService.openDialog(
      KeysAllocationsComponent,
      undefined,
      () => {
        this.getAllocations();
      }
    );
  }
}
