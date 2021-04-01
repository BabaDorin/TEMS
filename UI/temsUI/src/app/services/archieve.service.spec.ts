import { TestBed } from '@angular/core/testing';

import { ArchieveService } from './archieve.service';

describe('ArchieveService', () => {
  let service: ArchieveService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(ArchieveService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
