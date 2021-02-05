import { TestBed } from '@angular/core/testing';

import { ViewRoomsGuard } from './view-rooms.guard';

describe('ViewRoomsGuard', () => {
  let guard: ViewRoomsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewRoomsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
