import { TestBed } from '@angular/core/testing';

import { CreateIssuesGuard } from './create-issues.guard';

describe('CreateIssuesGuard', () => {
  let guard: CreateIssuesGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(CreateIssuesGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
