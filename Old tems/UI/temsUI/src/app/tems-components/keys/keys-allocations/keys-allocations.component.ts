import { Component, Inject, Input, OnInit, Optional, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { TranslateService } from '@ngx-translate/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { KeysService } from 'src/app/services/keys.service';
import { PersonnelService } from '../../../services/personnel.service';
import { SnackService } from '../../../services/snack.service';
import { TokenService } from '../../../services/token.service';
import { AddKeyAllocation } from './../../../models/key/add-key-allocation.model';
import { IOption } from './../../../models/option.model';
import { TEMSComponent } from './../../../tems/tems.component';

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
    public keysService: KeysService,
    public personnelService: PersonnelService,
    private snackService: SnackService,
    private tokenService: TokenService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();

    if(dialogData != undefined){
      this.keysAlreadySelectedOptions = dialogData.keysAlreadySelectedOptions;
    }
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

        if(result.status == 1 && this.dialogRef != undefined){
          model.keys.value .forEach(key => {
            key.additional = model.personnel.value[0];
          });
          this.dialogRef.close();
        }
      }));
  }
}
