import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewKeysComponent } from './view-keys.component';

describe('ViewKeysComponent', () => {
  let component: ViewKeysComponent;
  let fixture: ComponentFixture<ViewKeysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewKeysComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewKeysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
