import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-simple-info-card',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './simple-info-card.component.html',
  styleUrls: ['./simple-info-card.component.scss']
})
export class SimpleInfoCardComponent {
  @Input() title;
  @Input() theme: string = 'info';
  @Input() mainText: string;
  @Input() secondaryText;
  @Input() footNote;

  constructor() { }
}
