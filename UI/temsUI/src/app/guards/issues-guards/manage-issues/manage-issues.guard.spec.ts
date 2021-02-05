import { TestBed } from '@angular/core/testing';

import { ManageIssuesGuard } from './manage-issues.guard';

describe('ManageIssuesGuard', () => {
  let guard: ManageIssuesGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageIssuesGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
