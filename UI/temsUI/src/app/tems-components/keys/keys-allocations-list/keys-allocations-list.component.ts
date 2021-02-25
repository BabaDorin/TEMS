import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { ViewKeyAllocation } from 'src/app/models/key/view-key-allocation.model';
import { KeysService } from 'src/app/services/keys-service/keys.service';

@Component({
  selector: 'app-keys-allocations-list',
  templateUrl: './keys-allocations-list.component.html',
  styleUrls: ['./keys-allocations-list.component.scss']
})
export class KeysAllocationsListComponent implements OnInit {

  @Input() keyId;
  allocations: ViewKeyAllocation[];

  constructor(
    private keyService: KeysService
  ) { 
  }

  ngOnChanges(changes: SimpleChanges): void {
    if(this.keyId)
      this.allocations = this.keyService.getAllocationsOfKey(this.keyId);
  }

  ngOnInit(): void {
    if(this.keyId)
      this.allocations = this.keyService.getAllocationsOfKey(this.keyId);
    else
      this.allocations = this.keyService.getAllocations();
  }
}
