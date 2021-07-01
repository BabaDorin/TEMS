import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { IContainerAction, IGenericContainerModel } from './../../models/generic-container/IGenericContainer.model';

@Component({
  selector: 'app-generic-container',
  templateUrl: './generic-container.component.html',
  styleUrls: ['./generic-container.component.scss']
})
export class GenericContainerComponent implements OnInit {

  @Input() model: IGenericContainerModel;
  // @Input() canManage: boolean = false;
  
  @Output() eventEmitted = new EventEmitter();

  constructor() { }

  ngOnInit(): void {
    this.model.eventEmitted = (parameter?) => this.mdlOutputEevent(parameter);
  }

  callAction(action: IContainerAction){
    action.action();
  }

  mdlOutputEevent(parameter?){
    this.eventEmitted.emit(parameter);
  }
}
