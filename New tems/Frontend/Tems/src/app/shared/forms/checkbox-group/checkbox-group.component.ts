import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { CheckboxItem } from 'src/app/models/checkboxItem.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatIconModule } from '@angular/material/icon';
import { MatTooltipModule } from '@angular/material/tooltip';

@Component({
  selector: 'app-checkbox-group',
  standalone: true,
  imports: [CommonModule, FormsModule, MatCheckboxModule, MatIconModule, MatTooltipModule],
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
