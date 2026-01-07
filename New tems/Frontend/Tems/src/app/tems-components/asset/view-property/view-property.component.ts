import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { AssetService } from 'src/app/services/asset.service';
import { TEMSComponent } from 'src/app/tems/tems.component';
import { Property } from './../../../models/asset/view-property.model';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { AgGridModule } from 'ag-grid-angular';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-view-property',
  standalone: true,
  imports: [CommonModule, MatProgressBarModule, AgGridModule, TranslateModule],
  templateUrl: './view-property.component.html',
  styleUrls: ['./view-property.component.scss']
})
export class ViewPropertyComponent extends TEMSComponent implements OnInit {

  @Input() propertyId: string;
  property: Property = new Property();
  
  dialogRef;

  constructor(
    private assetService: AssetService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    super();
    
    if(dialogData != undefined){
      this.propertyId = this.dialogData.propertyId;
    }
  }

  ngOnInit(): void {
    if(this.propertyId == undefined)
      return;

    this.subscriptions.push(
      this.assetService.getPropertyById(this.propertyId)
      .subscribe(result => {
        this.property = result;
      })
    )
  }
}
