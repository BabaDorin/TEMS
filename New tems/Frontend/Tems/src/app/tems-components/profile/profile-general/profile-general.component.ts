import { Component, Input, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { TranslateModule } from '@ngx-translate/core';
import { ViewProfile } from '../../../models/profile/view-profile.model';

@Component({
  selector: 'app-profile-general',
  standalone: true,
  imports: [CommonModule, RouterModule, MatCardModule, MatButtonModule, MatIconModule, TranslateModule],
  templateUrl: './profile-general.component.html',
  styleUrls: ['./profile-general.component.scss']
})
export class ProfileGeneralComponent implements OnInit {

  @Input() profile;

  constructor(prof: ViewProfile) {
    this.profile = prof;
  }

  ngOnInit(): void {
  }
}
