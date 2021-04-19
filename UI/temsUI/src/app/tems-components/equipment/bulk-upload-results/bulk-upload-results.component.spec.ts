import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BulkUploadResultsComponent } from './bulk-upload-results.component';

describe('BulkUploadResultsComponent', () => {
  let component: BulkUploadResultsComponent;
  let fixture: ComponentFixture<BulkUploadResultsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BulkUploadResultsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BulkUploadResultsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
