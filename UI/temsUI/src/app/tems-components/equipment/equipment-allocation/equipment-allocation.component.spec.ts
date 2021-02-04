import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentAllocationComponent } from './equipment-allocation.component';

describe('EquipmentAllocationComponent', () => {
  let component: EquipmentAllocationComponent;
  let fixture: ComponentFixture<EquipmentAllocationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentAllocationComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentAllocationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
