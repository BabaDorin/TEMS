import { TestBed } from '@angular/core/testing';

import { ViewIssuesGuard } from './view-issues.guard';

describe('ViewIssuesGuard', () => {
  let guard: ViewIssuesGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewIssuesGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
