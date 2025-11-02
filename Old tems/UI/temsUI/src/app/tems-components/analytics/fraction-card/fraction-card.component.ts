import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-fraction-card',
  templateUrl: './fraction-card.component.html',
  styleUrls: ['./fraction-card.component.scss']
})
export class FractionCardComponent implements OnInit {

  @Input() title;
  @Input() numerator;
  @Input() denominator;
  @Input() footNote;
  @Input() theme = "success"; // success or danger (good or bad)
  @Input() displayProgressBar;

  progressValueNow: number;

  constructor() { }

  ngOnInit(): void {
  }

}
