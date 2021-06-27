import { TEMSComponent } from './../../../tems/tems.component';
import { SnackService } from '../../../services/snack.service';
import { EquipmentService } from 'src/app/services/equipment.service';
import { Component, OnInit, Output } from '@angular/core';
import { SICFileUploadResult } from 'src/app/models/equipment/bulk-upload-result.model';

@Component({
  selector: 'app-bulk-upload',
  templateUrl: './bulk-upload.component.html',
  styleUrls: ['./bulk-upload.component.scss']
})
export class BulkUploadComponent extends TEMSComponent implements OnInit {

  selectedFiles;
  feedback;
  dialogRef;
  uploadResults = [] as SICFileUploadResult[]; // Referenced in AddEquipmentComponent

  constructor(
    private equipmentService: EquipmentService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
  }

  onFilesSelected(files){
    this.selectedFiles = files;
  }

  uploadFiles(){
    
    if(this.selectedFiles == undefined || this.selectedFiles.length == 0){
      this.snackService.snack({message: "Select some files first", status: 0});
      return;
    }

    let formData = new FormData();
    for(let i = 0; i < this.selectedFiles.length; i++){
      formData.append(this.selectedFiles.item(i).name, this.selectedFiles.item(i));
    }
    
    this.feedback = "Processing... Please, do not close this modal until finished.";
    this.subscriptions.push(
      this.equipmentService.bulkUpload(formData)
      .subscribe(result => {
        console.log(result);

        if(this.snackService.snackIfError(result))
          return;

        if(Array.isArray(result)){
          this.uploadResults = result;
          this.dialogRef.close();
        }
      })
    )
  }
}
