import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridKeysComponent } from './ag-grid-keys.component';

describe('AgGridKeysComponent', () => {
  let component: AgGridKeysComponent;
  let fixture: ComponentFixture<AgGridKeysComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridKeysComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridKeysComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
