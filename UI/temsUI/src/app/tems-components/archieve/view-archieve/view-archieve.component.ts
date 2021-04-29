import { AgGridArchievedItemsComponent } from './../ag-grid-archieved-items/ag-grid-archieved-items.component';
import { CAN_MANAGE_SYSTEM_CONFIGURATION } from 'src/app/models/claims';
import { TokenService } from './../../../services/token-service/token.service';
import { Router, ActivatedRoute } from '@angular/router';
import { Component, OnInit, ViewChild } from '@angular/core';
import { Location } from '@angular/common'
import { TEMSComponent } from 'src/app/tems/tems.component';
import { ArchieveService } from 'src/app/services/archieve.service';
import { SnackService } from 'src/app/services/snack/snack.service';
import { ArchievedItem } from 'src/app/models/archieve/archieved-item.model';

@Component({
  selector: 'app-view-archieve',
  templateUrl: './view-archieve.component.html',
  styleUrls: ['./view-archieve.component.scss']
})
export class ViewArchieveComponent extends TEMSComponent implements OnInit {

  @ViewChild('agArchievedItems') agArchievedItems: AgGridArchievedItemsComponent;

  archievedItems: ArchievedItem[] = [];
  selectedType: string;
  canRemove: boolean = false;
  types = [
    'Equipment',
    'Issues',
    'Rooms',
    'Personnel',
    'Keys',
    'Report templates',
    'Equipment Allocations',
    'Logs',
    'Key allocations',
    'Properties',
    'Equipment Types',
    'Equipment Definitions',
  ];

  constructor(
    private archieveService: ArchieveService,
    private snackService: SnackService,
    private route: ActivatedRoute,
    private router: Router,
    private location: Location,
    private tokenService: TokenService
  ) {
    super();
  }

  onTypeChanged(){
    this.location.go('/archieve?itemType=' + this.selectedType);
  }

  ngOnInit(): void {
    this.canRemove = this.tokenService.hasClaim(CAN_MANAGE_SYSTEM_CONFIGURATION);
    this.CheckUrlAndFetchArchievedItems(this.router.url);

    this.location.onUrlChange(url => {
      this.CheckUrlAndFetchArchievedItems(url);
    });
  }

  CheckUrlAndFetchArchievedItems(url){
    if(url == undefined)
      return;
    
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
      );
  }

  dearchive(){
    let selected = this.agArchievedItems.getSelectedNodes() as ArchievedItem[];
    
    if(selected.length == 0)
      return;
    
    if(selected.length > 20)
    {
      this.snackService.snack({message: "You can't dearchive more than 20 items at a time", status: 0});
      return;
    }

    let archivationResult = "Success";
    for(let i = 0; i < selected.length; i++){
      this.subscriptions.push(
        this.archieveService.setArchivationStatus(this.selectedType, selected[i].id, false)
        .subscribe(result => {
          if(result.status == 0)
            archivationResult += selected[i].identifier + " failed, [" + result.message + "]\n";
          else
            this.agArchievedItems.removeItem(selected[i]);

          if(i == selected.length - 1){
            let status = 1;
            if(archivationResult != "Success")
              status = 0;

            this.snackService.snack(archivationResult, status);
          }
        })
      )
    }  
  }
}
