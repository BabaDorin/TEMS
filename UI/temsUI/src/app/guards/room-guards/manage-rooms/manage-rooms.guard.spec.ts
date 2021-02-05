import { TestBed } from '@angular/core/testing';

import { ManageRoomsGuard } from './manage-rooms.guard';

describe('ManageRoomsGuard', () => {
  let guard: ManageRoomsGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageRoomsGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
