import { IOption } from './../../../models/option.model';
import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { ActivatedRoute, Router } from "@angular/router"
import { TEMSComponent } from 'src/app/tems/tems.component';

@Component({
  selector: 'app-quick-access',
  templateUrl: './quick-access.component.html',
  styleUrls: ['./quick-access.component.scss']
})
export class QuickAccessComponent extends TEMSComponent implements OnInit {

  optionInput = new FormControl();
  options: IOption[] = [];
  filteredOptions: Observable<IOption[]>;
  selectedOptionId: string;
  type: string; // equipment, room or personnel

  header; label; placeholder;

  constructor(
    private equipmentService: EquipmentService,
    private router: Router,
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit() {
    this.checkRoute();
  }

  onSubmit() {
    if(this.optionInput.value.length == 0)
      return;

    let selectedOptionByLabel = this.options
      .find(q => q.label.toLowerCase() == this.optionInput.value.toLowerCase());

    if(selectedOptionByLabel == undefined)
      return;

    // 1) The value has not been selected within drowdown options
    if(this.selectedOptionId == undefined){

      if(selectedOptionByLabel == undefined){
        console.log('not found');
        return;
      }
      else
        this.selectedOptionId = selectedOptionByLabel.value;
    }
    else
    {
      // Option was selected via dropwdown, but we still have to check if the
      // option from mat-input matches with selectedOptionId
      let selectedOptionById = this.options
        .find(q => q.value == this.selectedOptionId);
      
      // Does not Match
      if(selectedOptionById.label != this.optionInput.value)
        this.selectedOptionId = selectedOptionByLabel.value
    }

    this.router.navigate(["/" + this.type + "/details/" + this.selectedOptionId]);
  }

  private _filter(value: string): IOption[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(option => option.label.toLowerCase().indexOf(filterValue) === 0);
  }

  // Subscribes to route changes
  private checkRoute() {
    this.activatedroute.params.subscribe(params => {
      if (params['type']) {
        this.type = this.activatedroute.snapshot.paramMap.get("type");

        if (['equipment', 'rooms', 'personnel'].indexOf(this.type) == -1) {
          this.router.navigate(['/error-pages/404']);
        }
    
        switch (this.type) {
          case 'equipment':
            this.subscriptions.push(this.equipmentService.getAllAutocompleteOptions()
              .subscribe(response => {
                this.options = response;
              }))

              this.header = "Find equipment by TEMSID or Serial Number";
            this.label = "Indentifier";
            this.placeholder = "LPB021";
            break;
    
          case 'rooms':
            // this.options = this.roomService.getAllAutocompleteOptions()
            //   .map(q => q.value);
            this.header = "Find room by identifier";
            this.label = "Indentifier";
            this.placeholder = "104";
            break;
    
          case 'personnel':
            // this.options = this.personnelService.getAllAutocompleteOptions()
            //   .map(q => q.value);
            this.header = "Find personnel by name";
            this.label = "Indentifier";
            this.placeholder = "104";
            break;
        }
    
        this.filteredOptions = this.optionInput.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value))
        );
      }
    });
  }

  optionSelected(optionId: string){
    console.log('Option selected: ' + optionId);
    this.selectedOptionId = optionId;
  }

  findIdByLabel(label: string){
    let option = this.options.find(q => q.label.toLowerCase() == label.toLowerCase());

    if(option != undefined)
      return option.value;

    return undefined;
  }
}
