import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-asset-serial-number',
  standalone: true,
  templateUrl: './asset-serial-number.component.html',
  styleUrls: ['./asset-serial-number.component.scss']
})
export class AssetSerialNumberComponent implements OnInit {

  @Input() mainText;
  
  constructor() { }

  ngOnInit(): void {
  }

}
