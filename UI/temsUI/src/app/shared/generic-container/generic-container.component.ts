import { IContainerAction, IGenericContainerModel } from './../../models/generic-container/IGenericContainer.model';
import { Component, Input, OnInit } from '@angular/core';

@Component({
  selector: 'app-generic-container',
  templateUrl: './generic-container.component.html',
  styleUrls: ['./generic-container.component.scss']
})
export class GenericContainerComponent implements OnInit {

  @Input() model: IGenericContainerModel 
  
  constructor() { }

  ngOnInit(): void {

  }

  callAction(action: IContainerAction){
    action.action();
  }
}
