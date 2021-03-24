import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-view-property',
  templateUrl: './view-property.component.html',
  styleUrls: ['./view-property.component.scss']
})
export class ViewPropertyComponent implements OnInit {

  @Input() propertyId: string;
  
  constructor() { }

  ngOnInit(): void {
  }

}
