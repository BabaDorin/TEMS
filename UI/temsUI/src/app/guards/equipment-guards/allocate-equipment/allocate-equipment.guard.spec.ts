import { TestBed } from '@angular/core/testing';

import { AllocateEquipmentGuard } from './allocate-equipment.guard';

describe('AllocateEquipmentGuard', () => {
  let guard: AllocateEquipmentGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(AllocateEquipmentGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
