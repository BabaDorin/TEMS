import { Component, OnInit } from '@angular/core';
import { FieldWrapper } from '@ngx-formly/core';

@Component({
  selector: 'formly-wrapper',
  template: `
    <div class="border-left mt-2 pl-3">
        <ng-container #fieldComponent></ng-container>
    </div>
  `,
  styleUrls: ['./formly-wrapper.component.scss']
})
export class FormlyWrapperComponent extends FieldWrapper {

  // This wrapper stands here to simulate the reddit sub-tree effect
  ngOnInit(): void {
  }
}