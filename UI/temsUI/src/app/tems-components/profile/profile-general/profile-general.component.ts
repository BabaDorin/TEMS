import { ViewProfile } from 'src/app/models/profile/view-profile.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-profile-general',
  templateUrl: './profile-general.component.html',
  styleUrls: ['./profile-general.component.scss']
})
export class ProfileGeneralComponent implements OnInit {

  @Input() profile;

  constructor(prof: ViewProfile) {
    console.log(prof);
    this.profile = prof;
  }

  ngOnInit(): void {
    console.log('eee');
    console.log(this.profile);
  }
}
