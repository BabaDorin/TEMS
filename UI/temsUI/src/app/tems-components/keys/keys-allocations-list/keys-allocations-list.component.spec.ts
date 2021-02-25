import { ComponentFixture, TestBed } from '@angular/core/testing';

import { KeysAllocationsListComponent } from './keys-allocations-list.component';

describe('KeysAllocationsListComponent', () => {
  let component: KeysAllocationsListComponent;
  let fixture: ComponentFixture<KeysAllocationsListComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ KeysAllocationsListComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(KeysAllocationsListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
