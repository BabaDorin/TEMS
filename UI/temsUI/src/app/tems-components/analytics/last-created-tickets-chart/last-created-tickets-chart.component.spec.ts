import { ComponentFixture, TestBed } from '@angular/core/testing';

import { LastCreatedTicketsChartComponent } from './last-created-tickets-chart.component';

describe('LastCreatedTicketsChartComponent', () => {
  let component: LastCreatedTicketsChartComponent;
  let fixture: ComponentFixture<LastCreatedTicketsChartComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ LastCreatedTicketsChartComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(LastCreatedTicketsChartComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
