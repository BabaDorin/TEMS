import { TestBed } from '@angular/core/testing';

import { ManageCommunicationGuard } from './manage-communication.guard';

describe('ManageCommunicationGuard', () => {
  let guard: ManageCommunicationGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ManageCommunicationGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
