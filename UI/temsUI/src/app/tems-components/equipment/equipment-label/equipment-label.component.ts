import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-label',
  templateUrl: './equipment-label.component.html',
  styleUrls: ['./equipment-label.component.scss']
})
export class EquipmentLabelComponent implements OnInit {

  @Input() mainText = "PR20003";
  constructor() { }

  ngOnInit(): void {
  }

}
