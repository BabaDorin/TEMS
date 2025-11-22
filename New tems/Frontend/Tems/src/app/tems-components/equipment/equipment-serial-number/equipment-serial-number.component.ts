import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-equipment-serial-number',
  standalone: true,
  templateUrl: './equipment-serial-number.component.html',
  styleUrls: ['./equipment-serial-number.component.scss']
})
export class EquipmentSerialNumberComponent implements OnInit {

  @Input() mainText;
  
  constructor() { }

  ngOnInit(): void {
  }

}
