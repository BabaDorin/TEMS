import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentDetailsAllocationsComponent } from './equipment-details-allocations.component';

describe('EquipmentDetailsAllocationsComponent', () => {
  let component: EquipmentDetailsAllocationsComponent;
  let fixture: ComponentFixture<EquipmentDetailsAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentDetailsAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentDetailsAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
