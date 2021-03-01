import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelDetailsIssuesComponent } from './personnel-details-issues.component';

describe('PersonnelDetailsIssuesComponent', () => {
  let component: PersonnelDetailsIssuesComponent;
  let fixture: ComponentFixture<PersonnelDetailsIssuesComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelDetailsIssuesComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelDetailsIssuesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
