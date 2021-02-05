import { TestBed } from '@angular/core/testing';

import { AllocateKeysGuard } from './allocate-keys.guard';

describe('AllocateKeysGuard', () => {
  let guard: AllocateKeysGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(AllocateKeysGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
