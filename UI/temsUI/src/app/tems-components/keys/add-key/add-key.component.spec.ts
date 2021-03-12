import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddKeyComponent } from './add-key.component';

describe('AddKeyComponent', () => {
  let component: AddKeyComponent;
  let fixture: ComponentFixture<AddKeyComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddKeyComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddKeyComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
