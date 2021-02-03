import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CollegeMapComponent } from './college-map.component';

describe('CollegeMapComponent', () => {
  let component: CollegeMapComponent;
  let fixture: ComponentFixture<CollegeMapComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CollegeMapComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CollegeMapComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
