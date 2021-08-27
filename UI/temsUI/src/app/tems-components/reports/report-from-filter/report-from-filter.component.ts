import { TranslateService } from '@ngx-translate/core';
import { PersonnelService } from './../../../services/personnel.service';
import { FormGroup, FormControl } from '@angular/forms';
import { ReportService } from './../../../services/report.service';
import { EquipmentFilter } from './../../../helpers/filters/equipment.filter';
import { Component, Inject, Input, OnInit, Optional } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';

@Component({
  selector: 'app-report-from-filter',
  templateUrl: './report-from-filter.component.html',
  styleUrls: ['./report-from-filter.component.scss']
})
export class ReportFromFilterComponent implements OnInit {

  dialogRef;

  @Input() equipmentFilter: EquipmentFilter;
  
  formGroup = new FormGroup({
    name: new FormControl(),
    header: new FormControl(),
    commonProperties: new FormControl(),
    footer: new FormControl(),
    signatories: new FormControl()
  });

  constructor(
    private reportService: ReportService,
    private personnelService: PersonnelService,
    public translate: TranslateService,
    @Optional() @Inject(MAT_DIALOG_DATA) public dialogData: any
  ) { 
    
    
    if(dialogData != undefined){
      this.equipmentFilter = dialogData.equipmentFilter;
    }
  }

  ngOnInit(): void {
    console.log('filter that I got here');
    console.log(this.equipmentFilter);
  }

  generateReport(){
    console.log(this.formGroup);
  }
}
