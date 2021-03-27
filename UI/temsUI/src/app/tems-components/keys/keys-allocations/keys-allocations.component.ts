import { AddKeyAllocation } from './../../../models/key/add-key-allocation.model';
import { TEMSComponent } from './../../../tems/tems.component';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { IOption } from './../../../models/option.model';
import { Component, OnInit, Input, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';

@Component({
  selector: 'app-keys-allocations',
  templateUrl: './keys-allocations.component.html',
  styleUrls: ['./keys-allocations.component.scss']
})
export class KeysAllocationsComponent extends TEMSComponent implements OnInit {

  @Input() keys: IOption[];

  @ViewChild('keysIdentifierChips', { static: false }) keysIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo', { static: false }) allocatedTo: ChipsAutocompleteComponent;

  keysAlreadySelectedOptions = [];
  keysChipsInputLabel = "Choose one or more keys to allocate...";

  personnelAutocompleteOptions = [];

  constructor(
    private keysService: KeysService,
    private personnelService: PersonnelService
  ) { 
    super();
  }

  
  ngOnInit(): void {
    if(this.keys)
      this.keysIdentifierChips.filteredOptions = this.keys;
  }

  submit(){
    if(this.keysIdentifierChips.options.length == 0 || this.allocatedTo.options.length == 0)
      return;

    let allocation = {
      keys: this.keysIdentifierChips.options,
      personnel: this.allocatedTo.options
    }

    let addKeyAllocation: AddKeyAllocation = {
      keyIds: this.keysIdentifierChips.options.map(q => q.value),
      personnelId: this.allocatedTo.options.map(q => q.value)[0],
    }

    this.subscriptions.push(this.keysService.createAllocation(addKeyAllocation)
      .subscribe(result => {
        console.log(result);
      }));
  }
}
