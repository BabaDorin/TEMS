import { Component, OnInit, ViewChild} from '@angular/core';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-view-keys-allocations',
  templateUrl: './view-keys-allocations.component.html',
  styleUrls: ['./view-keys-allocations.component.scss']
})
export class ViewKeysAllocationsComponent implements OnInit {
  
  @ViewChild('viewKeysAllocationsList') viewKeysAllocationsList;
  keys: IOption[];
  keyId: string;

  constructor(
    private keysService: KeysService
  ){

  }

  ngOnInit(): void {
    this.keys = this.keysService.getAutocompleteOptions();
  }

  keySelected(key){
    this.keyId = key.id;
  }
}




