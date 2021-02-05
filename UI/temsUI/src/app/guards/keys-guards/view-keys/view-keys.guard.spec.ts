import { TestBed } from '@angular/core/testing';

import { ViewKeysGuard } from './view-keys.guard';

describe('ViewKeysGuard', () => {
  let guard: ViewKeysGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewKeysGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
