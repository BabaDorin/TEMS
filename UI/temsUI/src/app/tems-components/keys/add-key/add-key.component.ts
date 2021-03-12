import { AddKey } from './../../../models/key/add-key.model';
import { FormlyParserService } from 'src/app/services/formly-parser-service/formly-parser.service';
import { KeysService } from 'src/app/services/keys-service/keys.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, OnInit } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';
import { FormlyFieldConfig } from '@ngx-formly/core';
import { RoomsService } from 'src/app/services/rooms-service/rooms.service';

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

  constructor(
    private keysService: KeysService,
    private formlyParserService: FormlyParserService,
    private roomsService: RoomsService
  ) { 
    super();
  }

  ngOnInit(): void {
    this.subscriptions.push(this.roomsService.getAllAutocompleteOptions()
      .subscribe(result => {
        this.formlyData.fields = this.formlyParserService.parseAddKey(result);
      }));
  }

  onSubmit(){
    let addKey: AddKey = this.formlyData.model.key;
    console.log('formly');
    console.log(this.formlyData.model.keyddKey);
    console.log('addKey');
    console.log(addKey);
    this.subscriptions.push(this.keysService.createKey(addKey)
      .subscribe(result => {
        console.log(result);
      })) 
  }
}
