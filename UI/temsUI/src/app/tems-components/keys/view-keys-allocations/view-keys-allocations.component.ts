import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild} from '@angular/core';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { IOption } from 'src/app/models/option.model';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';
import { Observable } from 'rxjs';
import { of } from 'rxjs';
import { filter, map } from 'rxjs/operators';

@Component({
  selector: 'app-view-keys-allocations',
  templateUrl: './view-keys-allocations.component.html',
  styleUrls: ['./view-keys-allocations.component.scss']
})
export class ViewKeysAllocationsComponent extends TEMSComponent implements OnInit {
  
  @ViewChild('viewKeysAllocationsList') viewKeysAllocationsList;
  keys: Observable<IOption[]>;
  filteredKeys: Observable<IOption[]>;
  keyId: string = "any";
  rooms: Observable<IOption[]>;
  roomId: string = "any";
  personnel: Observable<IOption[]>;
  personnelId: string = "any";

  constructor(
    private keysService: KeysService,
    private roomService: RoomsService,
    private personnelService: PersonnelService
  ){
    super();
  }

  ngOnInit(): void {
    console.log('ngoninit');

    this.subscriptions.push(this.keysService.getAutocompleteOptions()
      .subscribe(result => {
        console.log('keysAutocomplete')
        console.log(result);
        this.keys = of(result);
        this.filteredKeys = this.keys;
      }));

    this.subscriptions.push(this.roomService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log('rooms autocomplete')
        console.log(result);
        this.rooms = of(result);
      }));

    this.subscriptions.push(this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log('personnel')
        console.log(result);
        this.personnel = of(result);
      }));
  }

  keySelected(key){
    this.keyId = key;
  }

  roomSelected(room){
    this.roomId = room;

    if(room == "any")
      this.filteredKeys = this.keys;
    else if(this.keys != undefined){
      this.filteredKeys = this.keys.pipe(
        map(q => q.filter(q => q.additional == this.roomId))
      )

      this.keyId = "any";
    }
  }
  
  personnelSelected(key){
    this.personnelId = key;
  }
}




