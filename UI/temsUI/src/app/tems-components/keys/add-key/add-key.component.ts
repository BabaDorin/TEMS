import { SnackService } from 'src/app/services/snack.service';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { AddKey } from './../../../models/key/add-key.model';
import { FormlyParserService } from 'src/app/services/formly-parser.service';
import { KeysService } from 'src/app/services/keys.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit, ViewChild } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { RoomsService } from 'src/app/services/rooms.service';

@Component({
  selector: 'app-add-key',
  templateUrl: './add-key.component.html',
  styleUrls: ['./add-key.component.scss']
})
export class AddKeyComponent extends TEMSComponent implements OnInit {

  private formlyData = {
    form: new FormGroup({}),
    model: {} as any,
    fields: [] as FormlyFieldConfig[],
  }
  @ViewChild('room') room: ChipsAutocompleteComponent;
  dialogRef;

  constructor(
    private keysService: KeysService,
    private formlyParserService: FormlyParserService,
    private roomsService: RoomsService,
    private snackService: SnackService
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
