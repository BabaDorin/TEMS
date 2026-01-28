import { IOption } from './../../../../models/option.model';
import { LazyLoaderService } from './../../../../services/lazy-loader.service';
import { TranslateService } from '@ngx-translate/core';
import { Component, EventEmitter, Inject, Input, OnInit, Optional, Output } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ViewEquipment } from 'src/app/models/asset/view-asset.model';
import { AssetService } from 'src/app/services/asset.service';
import { PersonnelDetailsGeneralComponent } from 'src/app/tems-components/personnel/personnel-details-general/personnel-details-general.component';
import { DialogService } from '../../../../services/dialog.service';
import { SnackService } from '../../../../services/snack.service';
import { Property } from './../../../../models/asset/view-property.model';
import { ClaimService } from './../../../../services/claim.service';
import { TEMSComponent } from './../../../../tems/tems.component';
import { RoomDetailsGeneralComponent } from './../../../room/room-details-general/room-details-general.component';
import { AttachAssetComponent } from './../../attach-asset/attach-asset.component';
import { ConfirmService } from 'src/app/confirm.service';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatTooltipModule } from '@angular/material/tooltip';
import { TranslateModule } from '@ngx-translate/core';
import { PropertyRenderComponent } from '../../../../public/property-render/property-render.component';
import { AssetLabelComponent } from '../../asset-label/asset-label.component';
import { AssetSerialNumberComponent } from '../../asset-serial-number/asset-serial-number.component';
import { ChildAssetContainerComponent } from '../../child-asset-container/child-asset-container.component';
import { AddAssetComponent } from '../../add-asset/add-asset.component';

@Component({
  selector: 'app-asset-details-general',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatMenuModule,
    MatProgressBarModule,
    MatTooltipModule,
    TranslateModule,
    PropertyRenderComponent,
    AssetLabelComponent,
    AssetSerialNumberComponent,
    ChildAssetContainerComponent,
    AddAssetComponent
  ],
  templateUrl: './asset-details-general.component.html',
  styleUrls: ['./asset-details-general.component.scss']
})
export class AssetDetailsGeneralComponent extends TEMSComponent implements OnInit {

  @Input() assetId: string;
  @Input() displayViewMore: boolean = false;
  @Output() archivationStatusChanged = new EventEmitter();
  
  headerClass; // muted when archieved
  editing = false;

  asset: ViewEquipment;
  generalProperties: Property[];
  detachedEquipment = [];

  get canAttach(){
    // returns true if equipment can have children
    // BEFREE: True if type has children. We don't care that much about definition children here.
    return this.asset.definition.children.length > 0 && this.claims.canManageAssets;
  }

  constructor(
    private assetService: AssetService,
    public claims: ClaimService,
    private dialogService: DialogService,
    private snackService: SnackService,
    public translate: TranslateService,
    private lazyLoader: LazyLoaderService,
    private confirmService: ConfirmService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any) {
    super();

    if(dialogData != undefined){
      this.assetId = this.dialogData.assetId;
      this.displayViewMore = this.dialogData.displayViewMore;
    }
  }

  ngOnInit(): void {
    this.fetchEquipment();
  }

  // Fetch equipment data having it's ID and prepare it to be displayed on the view 
  fetchEquipment(){
    this.subscriptions.push(this.assetService.getEquipmentByID(this.assetId)
    .subscribe(response => {
      if(this.snackService.snackIfError(response))
        return;
      
      this.asset = response;
      this.headerClass = (this.asset.isArchieved) ? 'text-muted' : '';

      this.generalProperties= [
        { displayName: this.translate.instant('equipment.identifier'), value: this.asset.definition.identifier},
        { displayName: this.translate.instant('equipment.type'), value: this.asset.type.name},
        { displayName: this.translate.instant('equipment.TEMSID'), value: this.asset.temsId },
        { displayName: this.translate.instant('equipment.serialNumber'), value: this.asset.serialNumber},
        { displayName: this.translate.instant('equipment.description'), dataType: 'string', name: 'description', value: this.asset.description},
      ];
  
      if(this.detachedEquipment != undefined){
        this.detachedEquipment = this.detachedEquipment.filter(q => {
          return this.asset.children.findIndex(eq => eq.value == q.value) == -1
        });
      }
    }));
  }

  // Toggles the 'editing' property, which triggers the view to display it's data is a more "muted" way
  edit(){
    this.editing = true;    
  }

  viewMore(){
    if(this.dialogRef != undefined)
      this.dialogRef.close();
  }

  viewParent(){
    if(this.asset.parent == undefined)
      return;
    
    this.dialogService.openDialog(
      AssetDetailsGeneralComponent,
      [
        { label: "displayViewMore", value: true },
        { label: "assetId", value: this.asset.parent.value },
      ]
    );
  }

  // Displays the 'AttachAssetComponent' in a mat-dialog
  async attach(){
    await this.lazyLoader.loadModuleAsync('tems-ag-grid/attach-asset-ag-grid.module.ts');

    let dialog = this.dialogService.openDialog(
      AttachAssetComponent,
      [{label: "asset", value: this.asset}]
    );

    this.subscriptions.push(
      dialog.componentInstance.childAttached
      .subscribe(attachedEq => this.attachedFromAttachView(attachedEq))
    );
  }

  // equipment's index within equipment's children list
  detached(index: number){
    // child is of type IOption, label => identifier, value => id, additional => flag indicating whether is it atached or not.
    let detachedChild = this.asset.children[index];
    this.detachedEquipment.push(detachedChild);
    this.asset.children.splice(index, 1);
  };

  // equipment's index within detachedEquipment list (makes sense)
  attached(indexFromDetachedEqList: number){
    let attachedChild = this.detachedEquipment[indexFromDetachedEqList];
    this.detachedEquipment.splice(indexFromDetachedEqList, 1);
    this.asset.children.push(attachedChild);
  }

  // when some equipment is attached via AttachAssetComponent
  attachedFromAttachView(attachedEq: IOption){
    let childEqIndexFromDetached = this.detachedEquipment.findIndex(q => q.value == attachedEq.value);
    if(childEqIndexFromDetached != -1)
      this.detachedEquipment.splice(childEqIndexFromDetached, 1);
    
    this.asset.children.push(attachedEq);
  }

  // Display allocatee information in a mat-dialog
  viewAllocatee(){
    // Allocated to personnel
    if(this.asset.personnel != undefined){
      this.dialogService.openDialog(
        PersonnelDetailsGeneralComponent,
        [
          { label: "personnelId", value: this.asset.personnel.value },
          { label: "displayViewMore", value: true }
        ]
      );
    }

    // Allocated to room
    if(this.asset.room != undefined){
      this.dialogService.openDialog(
        RoomDetailsGeneralComponent,
        [
          { label: "roomId", value: this.asset.room.value },
          { label: "displayViewMore", value: true }
        ]
      );
    }
  }

  // Menu actions
  async archieve(){
    if(!this.asset.isArchieved && !await this.confirmService.confirm("Are you sure you want to archive this item? It will result in archieving all of it's logs and allocations"))
      return;

    let newArchivationStatus = !this.asset.isArchieved;
    this.subscriptions.push(
      this.assetService.archieveEquipment(this.assetId, newArchivationStatus)
      .subscribe(result => {
        this.snackService.snack(result);

        if(result.status == 1)
          this.asset.isArchieved = newArchivationStatus;
          this.headerClass = (this.asset.isArchieved) ? 'text-muted' : '';

        this.archivationStatusChanged.emit(this.asset.isArchieved);

        this.ngOnInit();
      })
    )
  }

  changeWorkingState(){
    this.subscriptions.push(
      this.assetService.changeWorkingState(this.assetId)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.asset.isDefect = !this.asset.isDefect;
        this.generalProperties[this.generalProperties.length-1].value = this.asset.isDefect;
      })
    );
  }

  changeUsingState(){
    this.subscriptions.push(
      this.assetService.changeUsingState(this.assetId)
      .subscribe(result =>{
        if(this.snackService.snackIfError(result))
          return;
      
        this.asset.isUsed = !this.asset.isUsed;
        this.generalProperties[this.generalProperties.length-2].value = this.asset.isUsed;
      })
    );
  }
}
