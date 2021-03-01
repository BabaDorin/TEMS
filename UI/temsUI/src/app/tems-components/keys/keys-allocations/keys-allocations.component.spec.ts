import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KeysAllocationsComponent } from './keys-allocations.component';

describe('KeysAllocationsComponent', () => {
  let component: KeysAllocationsComponent;
  let fixture: ComponentFixture<KeysAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KeysAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(KeysAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
