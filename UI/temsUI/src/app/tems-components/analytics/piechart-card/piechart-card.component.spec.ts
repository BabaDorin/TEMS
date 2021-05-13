import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PiechartCardComponent } from './piechart-card.component';

describe('PiechartCardComponent', () => {
  let component: PiechartCardComponent;
  let fixture: ComponentFixture<PiechartCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PiechartCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PiechartCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
