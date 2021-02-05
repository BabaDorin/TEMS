import { TestBed } from '@angular/core/testing';

import { ManageLibraryGuard } from './manage-library.guard';

describe('ManageLibraryGuard', () => {
  let guard: ManageLibraryGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageLibraryGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
