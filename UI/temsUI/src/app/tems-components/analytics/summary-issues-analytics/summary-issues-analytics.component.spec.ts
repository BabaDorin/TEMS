import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryIssuesAnalyticsComponent } from './summary-issues-analytics.component';

describe('SummaryIssuesAnalyticsComponent', () => {
  let component: SummaryIssuesAnalyticsComponent;
  let fixture: ComponentFixture<SummaryIssuesAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummaryIssuesAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummaryIssuesAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
