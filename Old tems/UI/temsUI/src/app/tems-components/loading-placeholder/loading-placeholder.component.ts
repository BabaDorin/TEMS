import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-loading-placeholder',
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
