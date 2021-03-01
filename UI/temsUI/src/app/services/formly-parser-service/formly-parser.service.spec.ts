import { TestBed } from '@angular/core/testing';

import { FormlyParserService } from './formly-parser.service';

describe('FormlyParserService', () => {
  let service: FormlyParserService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(FormlyParserService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
