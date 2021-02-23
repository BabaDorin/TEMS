import { PersonnelService } from './../../../services/personnel-service/personnel.service';
import { RoomsService } from './../../../services/rooms-service/rooms.service';
import { EquipmentService } from 'src/app/services/equipment-service/equipment.service';
import { Component, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { Observable } from 'rxjs';
import { map, startWith } from 'rxjs/operators';
import { ActivatedRoute, Router } from "@angular/router"

@Component({
  selector: 'app-quick-access',
  templateUrl: './quick-access.component.html',
  styleUrls: ['./quick-access.component.scss']
})
export class QuickAccessComponent implements OnInit {

  myControl = new FormControl();
  options: string[] = [];
  filteredOptions: Observable<string[]>;
  type: string; // equipment, room or personnel

  header; label; placeholder;

  constructor(
    private equipmentService: EquipmentService,
    private router: Router,
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ) {
  }

  ngOnInit() {
    this.checkRoute();
  }

  onSubmit() {
    // TODO: check is myControl.value is a valid identifier
    this.router.navigate(["/" + this.type + "/details/" + this.myControl.value]);
  }


  private _filter(value: string): string[] {
    const filterValue = value.toLowerCase();
    return this.options.filter(option => option.toLowerCase().indexOf(filterValue) === 0);
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
            this.options = this.equipmentService.getAllAutocompleteOptions()
              .map(q => q.value);
            this.header = "Find equipment by TEMSID or Serial Number";
            this.label = "Indentifier";
            this.placeholder = "LPB021";
            break;
    
          case 'rooms':
            this.options = this.roomService.getAllAutocompleteOptions()
              .map(q => q.value);
            this.header = "Find room by identifier";
            this.label = "Indentifier";
            this.placeholder = "104";
            break;
    
          case 'personnel':
            this.options = this.personnelService.getAllAutocompleteOptions()
              .map(q => q.value);
            this.header = "Find personnel by name";
            this.label = "Indentifier";
            this.placeholder = "104";
            break;
        }
    
        this.filteredOptions = this.myControl.valueChanges.pipe(
          startWith(''),
          map(value => this._filter(value))
        );
      }
    });
  }
}
