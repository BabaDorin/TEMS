import { TEMSComponent } from './../../tems/tems.component';
import { Router, ActivatedRoute } from '@angular/router';
import { IOption } from './../../models/option.model';
import { SnackService } from './../../services/snack/snack.service';
import { ArchieveService } from './../../services/archieve.service';
import { Component, OnInit } from '@angular/core';
import { Location } from '@angular/common'

@Component({
  selector: 'app-archieve',
  templateUrl: './archieve.component.html',
  styleUrls: ['./archieve.component.scss']
})
export class ArchieveComponent extends TEMSComponent implements OnInit {

  archievedItems: IOption[] = [];
  selectedType: string;
  types = [
    'Equipment',
    'Issues',
    'Rooms',
    'Personnel',
    'Keys',
    'Report templates',
    'Equipment Allocations',
    'Equipment Logs',
    'Room Logs',
    'Personnel Logs',
    'Key allocations',
    'Properties',
    'Types',
    'Definitions',
  ]

  constructor(
    private archieveService: ArchieveService,
    private snackService: SnackService,
    private route: ActivatedRoute,
    private router: Router,
    private location: Location
  ) {
    super();
  }

  onTypeChanged(){
    this.location.go('/archieve?itemType=' + this.selectedType);
  }

  ngOnInit(): void {
    this.location.onUrlChange(url => {
      this.selectedType = url.split('?itemType=')[1];
      console.log(this.selectedType);

      if(this.selectedType == undefined || this.selectedType.length == 0)
        return;

      this.subscriptions.push(
        this.archieveService.getArchievedItems(this.selectedType)
        .subscribe(result =>{
          if(this.snackService.snackIfError(result))
            return;
            console.log(result);
          this.archievedItems = result;
        })      
      )
    });
  }
}
