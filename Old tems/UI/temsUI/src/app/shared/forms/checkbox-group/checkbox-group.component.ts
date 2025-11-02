import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CheckboxItem } from 'src/app/models/checkboxItem.model';

@Component({
  selector: 'app-checkbox-group',
  templateUrl: './checkbox-group.component.html',
  styleUrls: ['./checkbox-group.component.scss']
})
export class CheckboxGroupComponent implements OnInit {
  @Input() options = Array<CheckboxItem>();
  @Input() label: string;
  @Output() toggle = new EventEmitter<any[]>();

  constructor() { }

  ngOnInit(): void {
  }

  onToggle() {
    const checkedOptions = this.options.filter(x => x.checked);
    this.toggle.emit(checkedOptions.map(x => x.value));
  }
}
