import { TestBed } from '@angular/core/testing';

import { ManageEquipmentGuard } from './manage-equipment.guard';

describe('ManageEquipmentGuard', () => {
  let guard: ManageEquipmentGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageEquipmentGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
