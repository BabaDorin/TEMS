import { Component, Input, OnInit } from '@angular/core';
import { Property } from './../../models/equipment/view-property.model';

@Component({
  selector: 'app-property-render',
  templateUrl: './property-render.component.html',
  styleUrls: ['./property-render.component.scss']
})
export class PropertyRenderComponent implements OnInit {

  @Input() properties: Property[];

  constructor() { }

  ngOnInit(): void {
    console.log(this.properties);
  }

}
