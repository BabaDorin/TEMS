import { TestBed } from '@angular/core/testing';

import { SystemConfigurationService } from './system-configuration.service';

describe('SystemConfigurationService', () => {
  let service: SystemConfigurationService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SystemConfigurationService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
