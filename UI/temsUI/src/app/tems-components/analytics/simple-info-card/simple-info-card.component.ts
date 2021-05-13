import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-simple-info-card',
  templateUrl: './simple-info-card.component.html',
  styleUrls: ['./simple-info-card.component.scss']
})
export class SimpleInfoCardComponent implements OnInit {

  @Input() title;
  @Input() mainText;
  @Input() secondaryText;
  
  constructor() { }

  ngOnInit(): void {
  }

}
