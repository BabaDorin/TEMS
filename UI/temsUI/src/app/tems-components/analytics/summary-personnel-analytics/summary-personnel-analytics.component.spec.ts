import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryPersonnelAnalyticsComponent } from './summary-personnel-analytics.component';

describe('SummaryPersonnelAnalyticsComponent', () => {
  let component: SummaryPersonnelAnalyticsComponent;
  let fixture: ComponentFixture<SummaryPersonnelAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummaryPersonnelAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummaryPersonnelAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
