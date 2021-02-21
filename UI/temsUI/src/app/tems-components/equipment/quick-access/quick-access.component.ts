import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import {Router} from "@angular/router"

@Component({
  selector: 'app-quick-access',
  templateUrl: './quick-access.component.html',
  styleUrls: ['./quick-access.component.scss']
})
export class QuickAccessComponent implements OnInit {

  myControl = new FormControl();
  options: string[] = [];
  filteredOptions: Observable<string[]>;

  constructor(
    private equipmentService: EquipmentService,
    private router: Router
  ){

  }

  onSubmit(){
    // check if it exists
    this.router.navigate(['/equipment/details/' + this.myControl.value]);
  }

  ngOnInit() {
    this.options = this.equipmentService.getAllAutocompleteOptions()
      .map(q => q.value); 

    this.filteredOptions = this.myControl.valueChanges.pipe(
      startWith(''),
      map(value => this._filter(value))
    );
  }

  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();

    return this.options.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
  }
}
