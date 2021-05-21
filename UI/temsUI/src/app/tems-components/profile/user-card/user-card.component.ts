import { ViewUserSimplified } from './../../../models/user/view-user.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-user-card',
  templateUrl: './user-card.component.html',
  styleUrls: ['./user-card.component.scss']
})
export class UserCardComponent implements OnInit {

  @Input() user: ViewUserSimplified;
  constructor() { }

  ngOnInit(): void {
  }

}
