import { TranslateService } from '@ngx-translate/core';
import { Component, OnInit, ViewChild } from '@angular/core';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { KeysService } from 'src/app/services/keys.service';
import { RoomsService } from 'src/app/services/rooms.service';
import { SnackService } from 'src/app/services/snack.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { FormlyData } from './../../../models/formly/formly-data.model';
import { AddKey } from './../../../models/key/add-key.model';

@Component({
  selector: 'app-add-key',
  templateUrl: './add-key.component.html',
  styleUrls: ['./add-key.component.scss']
})
export class AddKeyComponent extends TEMSComponent implements OnInit {

  public formlyData = new FormlyData();

  @ViewChild('room') room: ChipsAutocompleteComponent;
  dialogRef;

  constructor(
    public keysService: KeysService,
    private formlyParserService: FormlyParserService,
    public roomsService: RoomsService,
    private snackService: SnackService,
    public translate: TranslateService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.formlyData.fields = this.formlyParserService.parseAddKey();
  }

  onSubmit(){
    if(this.room.options == undefined || this.room.options.length == 0){
      this.snackService.snack({
        message: "Please, associate the key with a room",
        status: 0
      });
      return;
    }
    
    let addKey: AddKey = this.formlyData.model.key;
    addKey.roomId = this.room.options[0].value;

    this.subscriptions.push(this.keysService.createKey(addKey)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1 && this.dialogRef != undefined)
          this.dialogRef.close();
      })) 
  }
}
