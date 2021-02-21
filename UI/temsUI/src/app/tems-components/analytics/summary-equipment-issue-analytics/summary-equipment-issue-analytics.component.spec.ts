import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryEquipmentIssueAnalyticsComponent } from './summary-equipment-issue-analytics.component';

describe('SummaryEquipmentIssueAnalyticsComponent', () => {
  let component: SummaryEquipmentIssueAnalyticsComponent;
  let fixture: ComponentFixture<SummaryEquipmentIssueAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummaryEquipmentIssueAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummaryEquipmentIssueAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
