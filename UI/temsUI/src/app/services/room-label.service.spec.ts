import { TestBed } from '@angular/core/testing';

import { RoomLabelService } from './room-label.service';

describe('RoomLabelService', () => {
  let service: RoomLabelService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(RoomLabelService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
