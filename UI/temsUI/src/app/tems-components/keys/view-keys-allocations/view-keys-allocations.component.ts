import { CAN_MANAGE_ENTITIES, CAN_ALLOCATE_KEYS } from './../../../models/claims';
import { TokenService } from '../../../services/token.service';
import { SnackService } from '../../../services/snack.service';
import { PersonnelService } from 'src/app/services/personnel.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild} from '@angular/core';
import { KeysService } from 'src/app/services/keys.service';
import { IOption } from 'src/app/models/option.model';
import { RoomsService } from 'src/app/services/rooms.service';
import { Observable } from 'rxjs';
import { of } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { Router } from '@angular/router';

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
  canManage: boolean = false;

  constructor(
    private keysService: KeysService,
    private roomService: RoomsService,
    private router: Router,
    private personnelService: PersonnelService,
    private snackService: SnackService,
    private tokenService: TokenService
  ){
    super();
  }

  ngOnInit(): void {
    this.canManage = this.tokenService.hasClaim(CAN_MANAGE_ENTITIES) || this.tokenService.hasClaim(CAN_ALLOCATE_KEYS);
    
    this.subscriptions.push(this.keysService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);

        if(this.snackService.snackIfError(result))
          return;

        console.log('keysAutocomplete')
        console.log(result);
        this.keys = of(result);
        this.filteredKeys = this.keys;
      }));

    this.subscriptions.push(this.roomService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);

        if(this.snackService.snackIfError(result))
          return;

        console.log('rooms autocomplete')
        console.log(result);
        this.rooms = of(result);
      }));

    this.subscriptions.push(this.personnelService.getAllAutocompleteOptions()
      .subscribe(result => {
        console.log(result);

        if(this.snackService.snackIfError(result))
          return;
        console.log('personnel')
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

  viewRoom(roomId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/rooms/details/' + roomId]));
  }
  
  viewPersonnel(personnelId: string){
    this.router.navigateByUrl('/', {skipLocationChange: true}).then(()=>
    this.router.navigate(['/personnel/details/' + personnelId]));
  }
}




