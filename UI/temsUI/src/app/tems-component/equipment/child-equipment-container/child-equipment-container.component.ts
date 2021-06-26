import { SnackService } from './../../../services/snack/snack.service';
import { EquipmentService } from './../../../services/equipment-service/equipment.service';
import { TEMSComponent } from './../../../tems/tems.component';
import { IOption } from './../../../models/option.model';
import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-child-equipment-container',
  templateUrl: './child-equipment-container.component.html',
  styleUrls: ['./child-equipment-container.component.scss']
})
export class ChildEquipmentContainerComponent extends TEMSComponent implements OnInit {

  @Input() childEquipment: IOption;
  @Input() canManage: boolean = false;
  @Input() detachable: boolean = true;

  @Output() detached = new EventEmitter();

  constructor(
    private equipmentService: EquipmentService,
    private snackService: SnackService
  ) {
    super();
  }

  ngOnInit(): void {

  }

  detach(){
    if(!confirm("Are you sure you want to detach this equipment from it's parent?"))
    return;

  this.subscriptions.push(
    this.equipmentService.detach(this.childEquipment.value)
    .subscribe(result => {
      if(this.snackService.snackIfError(result))
        return;

      this.detached.emit();
    })
  )
  }
  
}
