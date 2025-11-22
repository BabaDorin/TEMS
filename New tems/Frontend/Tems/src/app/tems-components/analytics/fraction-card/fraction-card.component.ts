import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatProgressBarModule } from '@angular/material/progress-bar';

@Component({
  selector: 'app-fraction-card',
  standalone: true,
  imports: [
    CommonModule,
    MatProgressBarModule
  ],
  templateUrl: './fraction-card.component.html',
  styleUrls: ['./fraction-card.component.scss']
})
export class FractionCardComponent {

  @Input() title;
  @Input() numerator: number;
  @Input() denominator: number;
  @Input() footNote;
  @Input() theme: string = 'primary'; // success or danger (good or bad)
  @Input() displayProgressBar;

  progressValueNow: number;

  constructor() { }

}
