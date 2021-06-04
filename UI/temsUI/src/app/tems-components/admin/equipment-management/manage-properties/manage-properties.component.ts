import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-properties',
  templateUrl: './manage-properties.component.html',
  styleUrls: ['./manage-properties.component.scss']
})
export class ManagePropertiesComponent implements OnInit {

  @Input() canManage: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }

}
