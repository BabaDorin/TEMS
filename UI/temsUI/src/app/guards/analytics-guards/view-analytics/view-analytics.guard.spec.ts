import { TestBed } from '@angular/core/testing';

import { ViewAnalyticsGuard } from './view-analytics.guard';

describe('ViewAnalyticsGuard', () => {
  let guard: ViewAnalyticsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewAnalyticsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
