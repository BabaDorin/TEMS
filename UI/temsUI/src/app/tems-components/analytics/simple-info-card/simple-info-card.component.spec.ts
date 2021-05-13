import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SimpleInfoCardComponent } from './simple-info-card.component';

describe('SimpleInfoCardComponent', () => {
  let component: SimpleInfoCardComponent;
  let fixture: ComponentFixture<SimpleInfoCardComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SimpleInfoCardComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SimpleInfoCardComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
