import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddPersonnelComponent } from './add-personnel.component';

describe('AddPersonnelComponent', () => {
  let component: AddPersonnelComponent;
  let fixture: ComponentFixture<AddPersonnelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddPersonnelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddPersonnelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
