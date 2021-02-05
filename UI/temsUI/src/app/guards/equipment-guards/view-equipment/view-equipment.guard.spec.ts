import { TestBed } from '@angular/core/testing';

import { ViewEquipmentGuard } from './view-equipment.guard';

describe('ViewEquipmentGuard', () => {
  let guard: ViewEquipmentGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewEquipmentGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
