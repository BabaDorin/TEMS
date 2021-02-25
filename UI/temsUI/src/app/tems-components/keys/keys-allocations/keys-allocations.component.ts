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
export class KeysAllocationsComponent implements OnInit {

  @Input() keys: IOption[];

  @ViewChild('keysIdentifierChips', { static: false }) keysIdentifierChips: ChipsAutocompleteComponent;
  @ViewChild('allocatedTo', { static: false }) allocatedTo: ChipsAutocompleteComponent;

  keysAutocompleteOptions = [];
  keysAlreadySelectedOptions = [];
  keysChipsInputLabel = "Choose one or more keys to allocate...";

  personnelAutocompleteOptions = [];

  constructor(
    private keysService: KeysService,
    private personnelService: PersonnelService
  ) { }

  
  ngOnInit(): void {
    if(this.keys)
      this.keysAutocompleteOptions = this.keys;
    else
      this.keysAutocompleteOptions = this.keysService.getAutocompleteOptions();
      
    this.personnelAutocompleteOptions = this.personnelService.getAllAutocompleteOptions();
  }

  submit(){
    if(this.keysIdentifierChips.options.length == 0 || this.allocatedTo.options.length == 0)
      return;

    let allocation = {
      keys: this.keysIdentifierChips.options,
      personnel: this.allocatedTo.options
    }

    console.log(allocation);
  }
}



// private formlyData = {
//   isVisible: false,
//   form: new FormGroup({}),
//   model: {} as any,
//   fields: [] as FormlyFieldConfig[],
// }
// let fields: FormlyFieldConfig[] =
//   [
//     {
//       key: 'issue',
//       fieldGroup: [
//         {
//           key: 'problem',
//           type: 'autocomplete',
//           templateOptions: {
//             required: true,
//             label: 'What is the problem?',
//             placeholder: 'Incarcare cartus...',
//             filter: (term) => of(term ? this.filterAutocomplete(term, frequentProblems) : frequentProblems.slice()),
//           },
//         },
//         {
//           key: 'problemDescription',
//           type: 'textarea',
//           templateOptions: {
//             label: 'Problem description - Helps a lot!',
//           },
//         },
//         {
//           key: 'status',
//           type: 'radio',
//           templateOptions: {
//             label: 'Radio',
//             required: true,
//             options: [
//               { value: 1, label: 'Urgent' },
//               { value: 2, label: 'Mediu' },
//               { value: 3, label: 'Pe viitor' },
//             ],
//           },
//         }
//       ]
//     }
//   ];
