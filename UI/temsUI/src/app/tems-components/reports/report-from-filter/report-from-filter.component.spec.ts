import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ReportFromFilterComponent } from './report-from-filter.component';

describe('ReportFromFilterComponent', () => {
  let component: ReportFromFilterComponent;
  let fixture: ComponentFixture<ReportFromFilterComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ReportFromFilterComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ReportFromFilterComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
