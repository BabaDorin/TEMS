import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SelectTooltipComponent } from './select-tooltip.component';

describe('SelectTooltipComponent', () => {
  let component: SelectTooltipComponent;
  let fixture: ComponentFixture<SelectTooltipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SelectTooltipComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SelectTooltipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
