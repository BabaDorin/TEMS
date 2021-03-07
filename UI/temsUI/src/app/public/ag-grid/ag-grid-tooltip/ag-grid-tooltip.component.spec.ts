import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridTooltipComponent } from './ag-grid-tooltip.component';

describe('AgGridTooltipComponent', () => {
  let component: AgGridTooltipComponent;
  let fixture: ComponentFixture<AgGridTooltipComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridTooltipComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridTooltipComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
