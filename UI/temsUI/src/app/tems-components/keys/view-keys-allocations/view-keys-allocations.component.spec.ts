import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewKeysAllocationsComponent } from './view-keys-allocations.component';

describe('ViewKeysAllocationsComponent', () => {
  let component: ViewKeysAllocationsComponent;
  let fixture: ComponentFixture<ViewKeysAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewKeysAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewKeysAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
