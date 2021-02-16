import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AgGridEquipmentComponent } from './ag-grid-equipment.component';

describe('AgGridEquipmentComponent', () => {
  let component: AgGridEquipmentComponent;
  let fixture: ComponentFixture<AgGridEquipmentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ AgGridEquipmentComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AgGridEquipmentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
