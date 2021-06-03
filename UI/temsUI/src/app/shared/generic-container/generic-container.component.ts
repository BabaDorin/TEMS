import { IContainerAction, IGenericContainerModel } from './../../models/generic-container/IGenericContainer.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-generic-container',
  templateUrl: './generic-container.component.html',
  styleUrls: ['./generic-container.component.scss']
})
export class GenericContainerComponent implements OnInit {

  @Input() model: IGenericContainerModel;
  @Output() eventEmitted = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
    this.model.eventEmitted = (parameter?) => this.modelOutputEevent(parameter);
  }

  callAction(action: IContainerAction){
    action.action();
  }

  modelOutputEevent(parameter?){
    console.log('skr ' + parameter);
    this.eventEmitted.emit(parameter);
  }
}
