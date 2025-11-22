import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-profile-allocations',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './profile-allocations.component.html',
  styleUrls: ['./profile-allocations.component.scss']
})
export class ProfileAllocationsComponent implements OnInit {

  profile;
  constructor() { }

  ngOnInit(): void {
  }

}
