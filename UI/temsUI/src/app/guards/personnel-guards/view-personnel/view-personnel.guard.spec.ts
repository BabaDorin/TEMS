import { TestBed } from '@angular/core/testing';

import { ViewPersonnelGuard } from './view-personnel.guard';

describe('ViewPersonnelGuard', () => {
  let guard: ViewPersonnelGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewPersonnelGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
