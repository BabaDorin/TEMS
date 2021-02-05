import { TestBed } from '@angular/core/testing';

import { ViewReportsGuard } from './view-reports.guard';

describe('ViewReportsGuard', () => {
  let guard: ViewReportsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewReportsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
