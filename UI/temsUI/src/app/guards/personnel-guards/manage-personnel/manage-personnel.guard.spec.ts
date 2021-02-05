import { TestBed } from '@angular/core/testing';

import { ManagePersonnelGuard } from './manage-personnel.guard';

describe('ManagePersonnelGuard', () => {
  let guard: ManagePersonnelGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManagePersonnelGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
