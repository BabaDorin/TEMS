import { TestBed } from '@angular/core/testing';

import { ViewCommunicationGuard } from './view-communication.guard';

describe('ViewCommunicationGuard', () => {
  let guard: ViewCommunicationGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(ViewCommunicationGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
