import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-date-time-display',
  templateUrl: './date-time-display.component.html',
  styleUrls: ['./date-time-display.component.scss']
})
export class DateTimeDisplayComponent implements OnInit {

  @Input() title: string = "Date";

  // 1st use case: pass the date and select what to display
  @Input() date: Date;
  @Input() displayDate: boolean = true; // May, 10
  @Input() displayYear: boolean = true; // 2021
  @Input() displayHourMinute: boolean = false; // 13:30
  @Input() displayHourMinuteSecond: boolean = true; // 13:30:12

  // 2nd use case: pass the date, year and time as strings.
  // only passed data will be displayed (If you don't want to display time, don't pass it to the component).
  @Input() dateText: string;
  @Input() yearText: string;
  @Input() timeText: string;

  // Values that will get displayed
  finalDateValue: string;
  finalYearValue: string;
  finalTimeValue: string;

  monthDictionary = ["January", "February", "March", "April", "May", "June",
  "July", "August", "September", "October", "November", "December"
  ];

  constructor() { }

  ngOnInit(): void {
    this.date == undefined ? this.readFromTextInputs() : this.readFromDate();
  }

  readFromDate(){
    this.date = new Date(this.date.toString());

    if(this.displayHourMinuteSecond)
      this.displayHourMinute = true; // first the hour and minute is set, and then - seconds

    if(this.displayDate){
      this.finalDateValue = this.monthDictionary[this.date.getMonth()] + ", " + this.date.getDate();
    }

    if(this.displayYear){
      this.finalYearValue = this.date.getFullYear().toString();
    }

    if(this.displayHourMinute){
      this.finalTimeValue = this.ensureTwoDigitsTimeParameter(this.date.getHours().toString())
        + ':' + this.ensureTwoDigitsTimeParameter(this.date.getMinutes().toString());
    }

    if(this.displayHourMinuteSecond){
      this.finalTimeValue += ':' + this.ensureTwoDigitsTimeParameter(this.date.getSeconds().toString()) 
    }
  }

  readFromTextInputs(){
    this.finalDateValue = this.dateText;
    this.finalYearValue = this.yearText;
    this.finalTimeValue = this.timeText;
  }

  ensureTwoDigitsTimeParameter(parameter: string){
    // 3 => 03
    // 12 => 12

    return parameter.length == 1 ? '0' + parameter : parameter;
  }
}
