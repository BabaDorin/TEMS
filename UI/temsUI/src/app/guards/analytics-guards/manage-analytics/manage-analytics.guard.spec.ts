import { TestBed } from '@angular/core/testing';

import { ManageAnalyticsGuard } from './manage-analytics.guard';

describe('ManageAnalyticsGuard', () => {
  let guard: ManageAnalyticsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageAnalyticsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
