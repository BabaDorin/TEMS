import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-manage-definitions',
  templateUrl: './manage-definitions.component.html',
  styleUrls: ['./manage-definitions.component.scss']
})
export class ManageDefinitionsComponent implements OnInit {

  @Input() canManage: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }
}
