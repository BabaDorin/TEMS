import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentSerialNumberComponent } from './equipment-serial-number.component';

describe('EquipmentSerialNumberComponent', () => {
  let component: EquipmentSerialNumberComponent;
  let fixture: ComponentFixture<EquipmentSerialNumberComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentSerialNumberComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentSerialNumberComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
