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

  @Input() keyId;
  @Input() roomId;
  @Input() personnelId;
  allocations: Observable<ViewKeyAllocation[]>;

  constructor(
    private keyService: KeysService
  ) { 
    super();
  }

  ngOnInit(): void {
    // this.getAllocations();
  }

  ngOnChanges(): void {
    this.getAllocations();
  }

  getAllocations(){
    this.subscriptions.push(this.keyService.getAllocations(this.keyId, this.roomId, this.personnelId)
      .subscribe(result => {
        // console.log(result);
        this.allocations = of(result);
      }));
  }
}
