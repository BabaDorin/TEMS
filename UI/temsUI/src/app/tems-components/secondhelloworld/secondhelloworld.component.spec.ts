import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SecondhelloworldComponent } from './secondhelloworld.component';

describe('SecondhelloworldComponent', () => {
  let component: SecondhelloworldComponent;
  let fixture: ComponentFixture<SecondhelloworldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ SecondhelloworldComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SecondhelloworldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
