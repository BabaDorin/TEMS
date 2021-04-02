import { PersonnelService } from 'src/app/services/personnel-service/personnel.service';
import { SnackService } from 'src/app/services/snack/snack.service';
import { ChipsAutocompleteComponent } from 'src/app/public/formly/chips-autocomplete/chips-autocomplete.component';
import { FormControl, FormGroup } from '@angular/forms';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { UserService } from 'src/app/services/user-service/user.service';

@Component({
  selector: 'app-connect-personnel-user',
  templateUrl: './connect-personnel-user.component.html',
  styleUrls: ['./connect-personnel-user.component.scss']
})
export class ConnectPersonnelUserComponent extends TEMSComponent implements OnInit {

  @Input() personnelId;
  @ViewChild('user') user: ChipsAutocompleteComponent;
  dialogRef;

  userAssociationFormGroup = new FormGroup({
    user: new FormControl()
  })
  constructor(
    public userService: UserService,
    private snackService: SnackService,
    private personnelService: PersonnelService
  ) {
    super();
  }

  ngOnInit(): void {
    if(this.personnelId == undefined){
      this.snackService.snack({message: "Please, provide a personnel record", status: 0})
      if(this.dialogRef != undefined)
        this.dialogRef.close();
    }
  }

  submit(){
    if(this.user.options == undefined || this.user.options.length != 1)
      return;
    
    this.subscriptions.push(
      this.personnelService.connectWithUser(this.personnelId, this.user.options[0].value)
      .subscribe(result => {
        this.snackService.snack(result);
        
        if(result.status == 1)
          this.dialogRef.close();
      })
    )
    
  }
}
