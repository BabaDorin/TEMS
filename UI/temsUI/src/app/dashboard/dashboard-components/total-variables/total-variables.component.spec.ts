import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { TotalVariablesComponent } from './total-variables.component';

describe('TotalVariablesComponent', () => {
  let component: TotalVariablesComponent;
  let fixture: ComponentFixture<TotalVariablesComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ TotalVariablesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TotalVariablesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
