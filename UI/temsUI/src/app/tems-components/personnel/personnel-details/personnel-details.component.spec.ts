import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelDetailsComponent } from './personnel-details.component';

describe('PersonnelDetailsComponent', () => {
  let component: PersonnelDetailsComponent;
  let fixture: ComponentFixture<PersonnelDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelDetailsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
