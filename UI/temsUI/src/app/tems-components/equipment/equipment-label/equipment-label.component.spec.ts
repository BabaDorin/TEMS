import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentLabelComponent } from './equipment-label.component';

describe('EquipmentLabelComponent', () => {
  let component: EquipmentLabelComponent;
  let fixture: ComponentFixture<EquipmentLabelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentLabelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentLabelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
