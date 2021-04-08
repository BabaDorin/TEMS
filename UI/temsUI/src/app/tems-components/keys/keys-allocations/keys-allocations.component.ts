import { SnackService } from './../../../services/snack/snack.service';
import { FormControl, FormGroup } from '@angular/forms';
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

  @ViewChild('keysIdentifierChips') keysIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo') allocatedTo: ChipsAutocompleteComponent;

  keysAlreadySelectedOptions = [];
  keysChipsInputLabel = "Choose one or more keys to allocate...";
  dialogRef;

  constructor(
    private keysService: KeysService,
    private personnelService: PersonnelService,
    private snackService: SnackService
  ) { 
    super();
  }

  keyAllocationFormGroup: FormGroup;
  
  ngOnInit(): void {
    if(this.keys)
      this.keysIdentifierChips.filteredOptions = this.keys;

    this.keyAllocationFormGroup = new FormGroup({
      keys: new FormControl(this.keysAlreadySelectedOptions),
      personnel: new FormControl()
    });
  }

  submit(model){
    console.log('model:');
    console.log(model);
    if(model.keys.value == undefined 
      || model.keys.value.length == 0 
      || model.personnel.value == undefined
      || model.personnel.value.length == 0){
        this.snackService.snack({
          message: "Please, provide at least one key and one personnel",
          status: 0
        });
        return;
      }
    
    let addKeyAllocation: AddKeyAllocation = {
      keyIds: model.keys.value.map(q => q.value),
      personnelId: model.personnel.value.map(q => q.value)[0],
    }

    this.subscriptions.push(this.keysService.createAllocation(addKeyAllocation)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1 && this.dialogRef != undefined)
          this.dialogRef.close();
      }));
  }
}
