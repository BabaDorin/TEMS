import { AllocationService } from './../../../services/allocation-service/allocation.service';
import { Component, Input, OnInit } from '@angular/core';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { ViewKeyAllocation } from 'src/app/models/key/view-key-allocation.model';

@Component({
  selector: 'app-view-keys-allocations',
  templateUrl: './view-keys-allocations.component.html',
  styleUrls: ['./view-keys-allocations.component.scss']
})
export class ViewKeysAllocationsComponent implements OnInit {

  @Input() keyId;
  allocations: ViewKeyAllocation[];

  constructor(
    private keyService: KeysService
  ) { 

  }

  ngOnInit(): void {
    if(this.keyId)
      this.allocations = this.keyService.getAllocationsOfKey(this.keyId);
    else
      this.allocations = this.keyService.getAllocations();
  }
}
