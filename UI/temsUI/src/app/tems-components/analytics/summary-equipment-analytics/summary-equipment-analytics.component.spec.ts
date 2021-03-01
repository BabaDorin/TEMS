import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryEquipmentAnalyticsComponent } from './summary-equipment-analytics.component';

describe('SummaryEquipmentAnalyticsComponent', () => {
  let component: SummaryEquipmentAnalyticsComponent;
  let fixture: ComponentFixture<SummaryEquipmentAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummaryEquipmentAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummaryEquipmentAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
