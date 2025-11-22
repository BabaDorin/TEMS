import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatMenuModule } from '@angular/material/menu';
import { IContainerAction, IGenericContainerModel } from './../../models/generic-container/IGenericContainer.model';

@Component({
  selector: 'app-generic-container',
  standalone: true,
  imports: [CommonModule, MatCardModule, MatButtonModule, MatIconModule, MatTooltipModule, MatMenuModule],
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
