import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TEMSComponent } from './tems.component';

describe('TEMSComponent', () => {
  let component: TEMSComponent;
  let fixture: ComponentFixture<TEMSComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TEMSComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TEMSComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
