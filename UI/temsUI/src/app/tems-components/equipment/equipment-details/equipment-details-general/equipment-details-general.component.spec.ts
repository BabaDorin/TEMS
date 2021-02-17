import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EquipmentDetailsGeneralComponent } from './equipment-details-general.component';

describe('EquipmentDetailsGeneralComponent', () => {
  let component: EquipmentDetailsGeneralComponent;
  let fixture: ComponentFixture<EquipmentDetailsGeneralComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EquipmentDetailsGeneralComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EquipmentDetailsGeneralComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
