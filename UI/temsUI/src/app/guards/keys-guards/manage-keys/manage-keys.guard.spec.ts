import { TestBed } from '@angular/core/testing';

import { ManageKeysGuard } from './manage-keys.guard';

describe('ManageKeysGuard', () => {
  let guard: ManageKeysGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageKeysGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
