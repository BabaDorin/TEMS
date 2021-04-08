import { KeysAllocationsComponent } from 'src/app/tems-components/keys/keys-allocations/keys-allocations.component';
import { DialogService } from 'src/app/services/dialog-service/dialog.service';
import { SnackService } from './../../../services/snack/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { ViewKeyAllocation } from 'src/app/models/key/view-key-allocation.model';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { Observable, of } from 'rxjs';

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
