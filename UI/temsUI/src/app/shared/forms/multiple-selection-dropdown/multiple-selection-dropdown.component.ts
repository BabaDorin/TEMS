import { FormControl } from '@angular/forms';
import { SnackService } from './../../../services/snack.service';
import { IOptionsEndpoint } from './../../../models/form/options-endpoint.model';
import { IOption } from './../../../models/option.model';
import { Component, Input, OnInit, OnChanges, SimpleChanges, Output, EventEmitter } from '@angular/core';
import { propertyChanged } from 'src/app/helpers/onchanges-helper';

@Component({
  selector: 'app-multiple-selection-dropdown',
  templateUrl: './multiple-selection-dropdown.component.html',
  styleUrls: ['./multiple-selection-dropdown.component.scss']
})
export class MultipleSelectionDropdownComponent implements OnInit, OnChanges{

  // Multiple selection dropdown: The input label is reprezented by the property with the same name.
  
  // Options are taken from 2 sources, where [options] is prioritized (If both options and endpoint are provided,
  // all of the options will be take only from options input property);
  
  // One can specify some "mandatory" options that will appear at the beginning of the list via preOptions input 
  
  // Everytime the user checks some options, the selectionChanged(selectedOptions) event is fired, where selected
  // options represent a list containing values of selected options.
  
  @Input() label: string = "dropdown";
  @Input() options: IOption[] = [];
  @Input() endPoint: IOptionsEndpoint;
  @Input() preOptions: IOption[] = []; 

  dropdownFormControl: FormControl = new FormControl();
  dropdownOptions: IOption[] = this.preOptions;

  @Output() selectionChanged = new EventEmitter();

  constructor(
    private snackService: SnackService
  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    // Reloads options if either options or endPoint suffered changes

    if(propertyChanged(changes, "options")){
      this.readOptionsFromInput();
      return;
    }

    if(propertyChanged(changes, "endPoint")){
      this.readOptionsFromEndpoint();
      return;
    }
  }

  ngOnInit(): void {
    if(this.options != undefined){
      this.readOptionsFromInput();
      return;
    }

    if(this.endPoint != undefined){
      this.readOptionsFromEndpoint();
      return;
    }
  }

  selectionChange(eventData){
    this.selectionChanged.emit(eventData.value);
  }

  private readOptionsFromEndpoint() {
    this.endPoint.getOptions()
      .subscribe(result => {
        if (this.snackService.snackIfError(result)) {
          this.dropdownOptions = [];
          return;
        }

        this.dropdownOptions = this.preOptions.concat(result);
      });
  }

  private readOptionsFromInput() {
    this.dropdownOptions = this.preOptions.concat(this.options);
  }
}
