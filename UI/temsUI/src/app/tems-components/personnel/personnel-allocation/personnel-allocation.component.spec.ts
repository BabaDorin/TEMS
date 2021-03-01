import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PersonnelAllocationComponent } from './personnel-allocation.component';

describe('PersonnelAllocationComponent', () => {
  let component: PersonnelAllocationComponent;
  let fixture: ComponentFixture<PersonnelAllocationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ PersonnelAllocationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PersonnelAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
