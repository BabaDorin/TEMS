import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelDetailsAllocationsComponent } from './personnel-details-allocations.component';

describe('PersonnelDetailsAllocationsComponent', () => {
  let component: PersonnelDetailsAllocationsComponent;
  let fixture: ComponentFixture<PersonnelDetailsAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelDetailsAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelDetailsAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
