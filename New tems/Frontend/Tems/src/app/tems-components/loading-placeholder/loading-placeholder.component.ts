import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-loading-placeholder',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './loading-placeholder.component.html',
  styleUrls: ['./loading-placeholder.component.scss']
})
export class LoadingPlaceholderComponent implements OnInit {

  numberOfPlaceholders = 5;
  placeholders = [];
  constructor() { }

  ngOnInit(): void {
    for(let i = 0; i < this.numberOfPlaceholders; i++){
      this.placeholders.push('placeholder');
    }
  }

}
