import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AddLogComponent } from './add-log.component';

describe('AddLogComponent', () => {
  let component: AddLogComponent;
  let fixture: ComponentFixture<AddLogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AddLogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AddLogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
