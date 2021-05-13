import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FractionCardComponent } from './fraction-card.component';

describe('FractionCardComponent', () => {
  let component: FractionCardComponent;
  let fixture: ComponentFixture<FractionCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ FractionCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FractionCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
