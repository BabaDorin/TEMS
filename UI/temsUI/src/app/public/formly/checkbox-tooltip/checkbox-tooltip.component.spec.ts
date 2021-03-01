import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CheckboxTooltipComponent } from './checkbox-tooltip.component';

describe('CheckboxTooltipComponent', () => {
  let component: CheckboxTooltipComponent;
  let fixture: ComponentFixture<CheckboxTooltipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CheckboxTooltipComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CheckboxTooltipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
