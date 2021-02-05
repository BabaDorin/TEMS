import { TestBed } from '@angular/core/testing';

import { ManageReportsGuard } from './manage-reports.guard';

describe('ManageReportsGuard', () => {
  let guard: ManageReportsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageReportsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
