import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { TranslateService } from '@ngx-translate/core';
import { IOption } from 'src/app/models/option.model';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { AssetService } from 'src/app/services/asset.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { PersonnelService } from '../../../services/personnel.service';
import { RoomsService } from '../../../services/rooms.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-quick-access',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatAutocompleteModule,
    MatIconModule,
    TranslateModule,
    ChipsAutocompleteComponent
  ],
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
    private assetService: AssetService,
    private router: Router,
    private activatedroute: ActivatedRoute,
    private roomService: RoomsService,
    private personnelService: PersonnelService,
    private snackService: SnackService,
    public translate: TranslateService
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
        if (['asset', 'rooms', 'personnel'].indexOf(this.type) == -1) {
          this.router.navigate(['/error-pages/404']);
        }

        this.prepareUnderType();
      }
    });
  }

  prepareUnderType() {
    this.subscriptions.push(
      this.translate.stream('quickAccess')
      .subscribe(translation => {
        let quickAccess = translation;

        switch (this.type) {
          case 'asset':
            this.endPoint = this.assetService;
            this.header = quickAccess.assetHeader;
            this.label = quickAccess.assetLabel;
            break;
    
          case 'rooms':
            this.endPoint = this.roomService;
            this.header = quickAccess.roomHeader;
            this.label = quickAccess.roomLabel;
            break;
    
          case 'personnel':
            this.endPoint = this.personnelService;
            this.header = quickAccess.personnelHeader;
            this.label = quickAccess.personnelLabel;
            break;
        }
      })
    )
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
