import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelDetailsLogsComponent } from './personnel-details-logs.component';

describe('PersonnelDetailsLogsComponent', () => {
  let component: PersonnelDetailsLogsComponent;
  let fixture: ComponentFixture<PersonnelDetailsLogsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelDetailsLogsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelDetailsLogsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
