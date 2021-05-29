import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentAllocationContainerComponent } from './equipment-allocation-container.component';

describe('EquipmentAllocationContainerComponent', () => {
  let component: EquipmentAllocationContainerComponent;
  let fixture: ComponentFixture<EquipmentAllocationContainerComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentAllocationContainerComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentAllocationContainerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
