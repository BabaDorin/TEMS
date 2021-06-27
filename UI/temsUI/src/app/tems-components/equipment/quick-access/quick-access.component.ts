import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { SnackService } from 'src/app/services/snack.service';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { EquipmentService } from 'src/app/services/equipment.service';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router"
import { TEMSComponent } from 'src/app/tems/tems.component';
import { IOption } from 'src/app/models/option.model';

@Component({
  selector: 'app-quick-access',
  templateUrl: './quick-access.component.html',
  styleUrls: ['./quick-access.component.scss']
})
export class QuickAccessComponent extends TEMSComponent implements OnInit {

  @Input() type: string; // equipment, room or personnel
  header;
  label;
  placeholder;
  endPoint;
  @ViewChild('identifier') identifier: ChipsAutocompleteComponent;

  constructor(
    private equipmentService: EquipmentService,
    private router: Router,
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService,
    private personnelService: PersonnelService,
    private snackService: SnackService
  ) {
    super();
  }

  quickAccessFormGroup = new FormGroup({
    selectedEntities: new FormControl()
  })

  get selectedEntity() {
    let value = this.quickAccessFormGroup.controls.selectedEntities.value;
    if (value == undefined || value.length == 0) return undefined;
    return value[0]
  }

  ngOnInit() {
    if (this.identifier != undefined)
      this.identifier.alreadySelected = [] as IOption[];

    console.log('type = ' + this.type);
    this.getType();
  }

  private getType() {
    if (this.type != undefined) {
      this.prepareUnderType();
      return;
    }

    this.activatedroute.params.subscribe(params => {
      if (params['type']) {
        this.type = this.activatedroute.snapshot.paramMap.get("type");
        if (['equipment', 'rooms', 'personnel'].indexOf(this.type) == -1) {
          this.router.navigate(['/error-pages/404']);
        }

        this.prepareUnderType();
      }
    });
  }

  prepareUnderType() {
    switch (this.type) {
      case 'equipment':
        this.endPoint = this.equipmentService;
        this.header = "Find an equipment";
        this.label = "Find equipment by it's TEMSID or Serial Number";
        this.placeholder = "TEMSID or Serial Number";
        break;

      case 'rooms':
        this.endPoint = this.roomService;
        this.header = "Find a room";
        this.label = "Find room by identifier";
        this.placeholder = "Room's identifier";
        break;

      case 'personnel':
        this.endPoint = this.personnelService;
        this.header = "Find personnel";
        this.label = "Find personnel by name";
        this.placeholder = "Personnel's name";
        break;
    }
  }

  onSubmit() {
    if (this.selectedEntity == undefined) {
      this.snackService.snack({
        message: "Please, choose an entity from the dropdown",
        status: 0
      });
      return;
    }

    this.router.navigate(["/" + this.type + "/details/" + this.selectedEntity.value]);
  }
}
