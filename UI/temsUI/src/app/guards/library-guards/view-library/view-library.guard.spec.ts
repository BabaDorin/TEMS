import { TestBed } from '@angular/core/testing';

import { ViewLibraryGuard } from './view-library.guard';

describe('ViewLibraryGuard', () => {
  let guard: ViewLibraryGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewLibraryGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
