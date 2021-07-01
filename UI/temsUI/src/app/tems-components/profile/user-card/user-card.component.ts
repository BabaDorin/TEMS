import { Component, Input, OnInit } from '@angular/core';
import { ViewUserSimplified } from './../../../models/user/view-user.model';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss']
})
export class UserCardComponent implements OnInit {

  @Input() user: ViewUserSimplified;
  @Input() canView: boolean = false;
  @Input() canManage: boolean = false;
  
  constructor() { }

  ngOnInit(): void {
  }

}
