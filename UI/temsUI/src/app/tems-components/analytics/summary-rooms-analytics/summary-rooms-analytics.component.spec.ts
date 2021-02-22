import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SummaryRoomsAnalyticsComponent } from './summary-rooms-analytics.component';

describe('SummaryRoomsAnalyticsComponent', () => {
  let component: SummaryRoomsAnalyticsComponent;
  let fixture: ComponentFixture<SummaryRoomsAnalyticsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SummaryRoomsAnalyticsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SummaryRoomsAnalyticsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
