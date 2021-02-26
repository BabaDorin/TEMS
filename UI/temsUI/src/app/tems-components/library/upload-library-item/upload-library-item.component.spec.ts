import { ComponentFixture, TestBed } from '@angular/core/testing';

import { UploadLibraryItemComponent } from './upload-library-item.component';

describe('UploadLibraryItemComponent', () => {
  let component: UploadLibraryItemComponent;
  let fixture: ComponentFixture<UploadLibraryItemComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ UploadLibraryItemComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadLibraryItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
