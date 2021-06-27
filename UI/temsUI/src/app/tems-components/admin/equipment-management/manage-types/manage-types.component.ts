import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-types',
  templateUrl: './manage-types.component.html',
  styleUrls: ['./manage-types.component.scss']
})
export class ManageTypesComponent implements OnInit {

  @Input() canManage: boolean = false;

  constructor() { }

  ngOnInit(): void {
  }

}
