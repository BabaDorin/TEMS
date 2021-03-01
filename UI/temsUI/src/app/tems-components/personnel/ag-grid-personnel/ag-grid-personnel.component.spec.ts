import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridPersonnelComponent } from './ag-grid-personnel.component';

describe('AgGridPersonnelComponent', () => {
  let component: AgGridPersonnelComponent;
  let fixture: ComponentFixture<AgGridPersonnelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridPersonnelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridPersonnelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
