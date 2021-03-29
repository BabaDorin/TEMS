import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ViewEquipmentAllocationsComponent } from './view-equipment-allocations.component';

describe('ViewEquipmentAllocationsComponent', () => {
  let component: ViewEquipmentAllocationsComponent;
  let fixture: ComponentFixture<ViewEquipmentAllocationsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ViewEquipmentAllocationsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ViewEquipmentAllocationsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
