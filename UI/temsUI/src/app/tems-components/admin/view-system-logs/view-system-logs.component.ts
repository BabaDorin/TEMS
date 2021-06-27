import { LoggerViewModel } from './../../../models/system-configuration/logger-view-model';
import { SnackService } from '../../../services/snack.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { SystemConfigurationService } from 'src/app/services/system-configuration.service';
import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-view-system-logs',
  templateUrl: './view-system-logs.component.html',
  styleUrls: ['./view-system-logs.component.scss']
})
export class ViewSystemLogsComponent extends TEMSComponent implements OnInit {
  
  dateFormGroup = new FormGroup({
    date: new FormControl(),
  });
  
  logs: string;

  constructor(
    private systemConfigurationService: SystemConfigurationService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {
    // default date
    this.dateFormGroup.controls.date.setValue(new Date());
  }

  fetchLogs(){
    let selectedDate = new Date(this.dateFormGroup.controls.date.value);
    if(selectedDate == undefined)
      return;

    console.log('selected date');
    console.log(selectedDate);

    let loggerViewModel = new LoggerViewModel();
    loggerViewModel.date = selectedDate;
    
    this.subscriptions.push(
      this.systemConfigurationService.fetchSystemLogs(loggerViewModel)
      .subscribe(result => {
        if(this.snackService.snackIfError(result))
          return;

        this.logs = result;
      })
    )
  }
}
