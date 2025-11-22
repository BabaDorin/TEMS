import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FieldWrapper, FormlyModule } from '@ngx-formly/core';

@Component({
  selector: 'app-formly-wrapper',
  standalone: true,
  imports: [CommonModule, FormlyModule],
  templateUrl: './formly-wrapper.component.html',
  styleUrls: ['./formly-wrapper.component.scss']
})
export class FormlyWrapperComponent extends FieldWrapper {

  // This wrapper stands here to simulate the reddit sub-tree effect
  ngOnInit(): void {
  }
}