import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelDetailsGeneralComponent } from './personnel-details-general.component';

describe('PersonnelDetailsGeneralComponent', () => {
  let component: PersonnelDetailsGeneralComponent;
  let fixture: ComponentFixture<PersonnelDetailsGeneralComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelDetailsGeneralComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelDetailsGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
